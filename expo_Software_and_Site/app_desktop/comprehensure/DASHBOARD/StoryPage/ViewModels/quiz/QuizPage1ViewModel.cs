using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class QuizPage1ViewModel : ObservableObject
    {
        string uid = Preferences.Default.Get("SavedUserUid", "");

        private bool _locked;
        private bool _resultsSaved;   // prevent score duplication here DO NOT REMOVE
        private string _correctAnswer = "";

        public int StoryProgress { get; }
        public int CalculatedScore => Score * 2;

        private readonly string projectId = "comprehensuredb-f9f7c";
        private string BaseUrl => $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents";

        public int Score { get; private set; } = 0;
        public int TotalQuestions => _questions?.Count ?? 0;
        public int CurrentQuestionIndex => CurrentIndex;

        [ObservableProperty] private string _mainquestion;
        [ObservableProperty] private string _ch1bt;
        [ObservableProperty] private string _ch2bt;
        [ObservableProperty] private string _ch3bt;
        [ObservableProperty] private string _ch4bt;
        [ObservableProperty] private int _currentIndex = 0;
        [ObservableProperty] private string _questionNumber;

        private List<QuizItem> _questions = new();
        private readonly Random _rng = new Random();

        public QuizPage1ViewModel(int storyProgress = 0)
        {
            StoryProgress = storyProgress;
        }

        public async Task LoadQuestions()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("quiz1data.json");
            using var reader = new StreamReader(stream);
            string json = await reader.ReadToEndAsync();
            var loaded = JsonSerializer.Deserialize<List<QuizItem>>(json);
            _questions = loaded.OrderBy(_ => _rng.Next()).ToList();
            ShowQuestion(CurrentIndex);
        }

        private void ShowQuestion(int index)
        {
            if (_questions == null || index >= _questions.Count) return;
            var q = _questions[index];
            Mainquestion = q.Question;
            QuestionNumber = $"Question {index + 1} of {_questions.Count}";
            var choices = new List<(string Key, string Text)>
            {
                ("A", q.Choices.A), ("B", q.Choices.B),
                ("C", q.Choices.C), ("D", q.Choices.D),
            }.OrderBy(_ => _rng.Next()).ToList();
            _correctAnswer = q.Choices.GetByKey(q.Answer);
            Ch1bt = choices[0].Text; Ch2bt = choices[1].Text;
            Ch3bt = choices[2].Text; Ch4bt = choices[3].Text;
        }

        public bool CheckAnswer(string selectedKey)
        {
            string selectedText = selectedKey switch
            {
                "A" => Ch1bt, "B" => Ch2bt, "C" => Ch3bt, "D" => Ch4bt, _ => ""
            };
            bool correct = selectedText == _correctAnswer;
            if (correct) Score++;
            return correct;
        }

        public bool NextQuestion()
        {
            if (_questions == null) return false;
            CurrentIndex++;
            if (CurrentIndex >= _questions.Count) return false;
            ShowQuestion(CurrentIndex);
            return true;
        }

        public async Task SaveQuizResults()
        {
            if (_resultsSaved) return;
            _resultsSaved = true;

            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrEmpty(uid)) return;

            int currentModuleFinished = 0;
            int currentScore = 0;

            try
            {
                using var http = new HttpClient();
                var readResponse = await http.GetAsync($"{BaseUrl}/userdata/{uid}");
                if (readResponse.IsSuccessStatusCode)
                {
                    var readJson = await readResponse.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(readJson);
                    var fields = doc.RootElement.GetProperty("fields");
                    if (fields.TryGetProperty("ModuleFinished", out var mf) &&
                        mf.TryGetProperty("integerValue", out var mfVal))
                        currentModuleFinished = ReadFirestoreInt(mfVal);
                    if (fields.TryGetProperty("ScoreOfTotal", out var sc) &&
                        sc.TryGetProperty("integerValue", out var scVal))
                        currentScore = ReadFirestoreInt(scVal);
                }

                int newModuleFinished  = currentModuleFinished + 1;
                int newScoreOfTotal    = currentScore + Score;
                int newCalculatedScore = Score * 2;

                string url = $"{BaseUrl}/userdata/{uid}?updateMask.fieldPaths=ModuleFinished&updateMask.fieldPaths=ScoreOfTotal&updateMask.fieldPaths=CalculatedScore";
                var data = new
                {
                    fields = new
                    {
                        ModuleFinished  = new { integerValue = newModuleFinished.ToString() },
                        ScoreOfTotal    = new { integerValue = newScoreOfTotal.ToString() },
                        CalculatedScore = new { integerValue = newCalculatedScore.ToString() }
                    }
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
                var json = JsonSerializer.Serialize(data, options);
                var saveResponse = await http.PatchAsync(url, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

                if (!saveResponse.IsSuccessStatusCode)
                    System.Diagnostics.Debug.WriteLine($"[SaveQuizResults] Failed: {await saveResponse.Content.ReadAsStringAsync()}");
                else
                    System.Diagnostics.Debug.WriteLine($"[SaveQuizResults] Saved — ModuleFinished:{newModuleFinished}, ScoreOfTotal:{newScoreOfTotal}, CalculatedScore:{newCalculatedScore}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveQuizResults] Exception: {ex.Message}");
            }

            await QuizFunc.SaveQuizProgressAsync(storyNumber: 1, quizScore: Score);
        }

        private int ReadFirestoreInt(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String &&
                int.TryParse(element.GetString(), out int result)) return result;
            if (element.ValueKind == JsonValueKind.Number) return element.GetInt32();
            return 0;
        }
    }
}
