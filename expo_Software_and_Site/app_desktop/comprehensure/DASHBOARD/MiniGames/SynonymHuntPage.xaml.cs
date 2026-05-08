using System.Text.Json;
using Microsoft.Maui.Controls;

namespace comprehensure.DASHBOARD.MiniGames;

public partial class SynonymHuntPage : ContentPage
{
    //Firestore link
    private readonly string _projectId = "comprehensuredb-f9f7c";
    private string BaseUrl =>
        $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents";
    private readonly HttpClient _http = new HttpClient();

 
    private record Question(string Story, string QuestionText, string Answer, string[] Choices);

    private record Module(int Id, string Name, string Difficulty, Question[] Questions);

    private readonly Module[] _modules =
    [
        new(1, "The Forgotten Library", "Easy", new Question[]
        {
            new("One afternoon, a curious student named Leo decided to visit. Leo loved history.",
                "Find a word that means \"interested.\"",
                "curious",
                ["ancient", "curious", "visited", "lonely"]),
            new("One journal described an expedition where a group of travelers had disappeared.",
                "Find a word that means \"vanished.\"",
                "disappeared",
                ["traveled", "disappeared", "explored", "returned"]),
            new("Some even believed it contained secret writings from explorers who had traveled to faraway lands long ago.",
                "Find a word that means \"distant.\"",
                "faraway",
                ["ancient", "secret", "faraway", "forgotten"])
        }),
        new(2, "The Clockmaker's Secret", "Easy", new Question[]
        {
            new("Inside worked Mr. Alden, the town's clockmaker. He was an elderly man with silver hair and round glasses.",
                "Find a word that means \"old.\"",
                "elderly",
                ["elderly", "weary", "clever", "grumpy"]),
            new("The club decided to preserve the writings and inform the town about the forgotten explorers.",
                "Find a word that means \"tell.\"",
                "inform",
                ["preserve", "inform", "gather", "protect"]),
            new("Between a warm bakery that smelled of fresh bread and a cozy bookstore filled with novels stood an old clock shop.",
                "Find a word that means \"comfortable.\"",
                "cozy",
                ["warm", "ancient", "cozy", "narrow"])
        }),
        new(3, "The Observatory on the Hill", "Easy", new Question[]
        {
            new("It had shown him how to observe carefully, think patiently, and trust patterns supported by evidence.",
                "Find a word that means \"proof.\"",
                "evidence",
                ["patterns", "evidence", "knowledge", "records"]),
            new("It only seems chaotic from a distance. She invited him to look through the telescope.",
                "Find a word that means \"messy.\"",
                "chaotic",
                ["distant", "brilliant", "chaotic", "endless"]),
            new("Adrian felt disappointed. Dr. Vale, however, handed him a different task.",
                "Find a word that means \"sad.\"",
                "disappointed",
                ["curious", "frustrated", "disappointed", "confused"])
        }),
        new(4, "The Garden of Hidden Patterns", "Average", new Question[]
        {
            new("When she stepped inside, she immediately sensed that this was no ordinary place.",
                "Find a word that means \"normal.\"",
                "ordinary",
                ["familiar", "ordinary", "visible", "hidden"]),
            new("Mara had always been drawn to patterns. As a child, she noticed the repeating shapes in tiles.",
                "Find a word that means \"attracted.\"",
                "drawn",
                ["noticed", "drawn", "trained", "moved"]),
            new("Logic did not remove wonder; it deepened it. By the end of the summer, the garden no longer seemed mysterious.",
                "Find a word that means \"strange.\"",
                "mysterious",
                ["beautiful", "dangerous", "mysterious", "complicated"])
        }),
        new(5, "The Map of Quiet Decisions", "Average", new Question[]
        {
            new("His responsibility, he explained, was to maintain an accurate history of Riverton's development.",
                "Find a word that means \"correct.\"",
                "accurate",
                ["accurate", "detailed", "long", "hidden"]),
            new("Elias recognized a consistent principle: significant transformation rarely occurs all at once.",
                "Find a word that means \"change.\"",
                "transformation",
                ["principle", "decision", "transformation", "pattern"]),
            new("Shelves along the walls were filled with rolled documents, covered with careful handwriting and precise lines.",
                "Find a word that means \"exact.\"",
                "precise",
                ["neat", "careful", "precise", "small"])
        }),
        new(6, "The Weight of Paper Wings", "Average", new Question[]
        {
            new("When the paper wings were first revealed to the public, they were met with disbelief rather than admiration.",
                "Find a word that means \"showed.\"",
                "revealed",
                ["revealed", "created", "tested", "banned"]),
            new("Governments began requiring psychological screenings before granting flight permits.",
                "Find a word that means \"giving.\"",
                "granting",
                ["requiring", "granting", "revoking", "denying"]),
            new("A volunteer climbed a high platform overlooking a crowded square.",
                "Find a word that means \"helper.\"",
                "volunteer",
                ["officer", "volunteer", "stranger", "leader"])
        }),
        new(7, "The City of Silent Bargains", "Intermediate", new Question[]
        {
            new("She carried one herself — a truth so carefully guarded that even she rarely examined it directly.",
                "Find a word that means \"protected.\"",
                "guarded",
                ["hidden", "guarded", "revealed", "kept"]),
            new("When she finally opened it, she found no written confession, no recorded evidence.",
                "Find a word that means \"proof.\"",
                "evidence",
                ["confession", "evidence", "document", "story"]),
            new("She could expose what she now knew and unravel a powerful life or she could remain silent.",
                "Find a word that means \"break apart.\"",
                "unravel",
                ["expose", "destroy", "unravel", "silence"])
        }),
        new(8, "Whispers of the Iron Forest", "Intermediate", new Question[]
        {
            new("Each whisper felt deliberate. They tested her resolve, tempting her with alternate versions of herself.",
                "Find a word that means \"determination.\"",
                "resolve",
                ["courage", "resolve", "patience", "instinct"]),
            new("Inside, the forest defied reason. Leaves clanged softly against one another like distant chains.",
                "Find a word that means \"logic.\"",
                "reason",
                ["gravity", "reason", "nature", "silence"]),
            new("At first they were fragments of indistinct murmurs, like conversations overheard through walls.",
                "Find a word that means \"unclear.\"",
                "indistinct",
                ["distant", "indistinct", "constant", "fearful"])
        })
    ];

    private int _currentModuleIndex = 0;
    private int _currentQuestionIndex = 0;
    private bool _answered = false;
    private int _score = 0;
    private int _totalAnswered = 0;
    private int _totalCorrect = 0;
    private bool _scoreSaved = false;

    private readonly int[][][] _shuffled;

    private Border[] _choiceBorders = null!;
    private Border[] _choiceBadges  = null!;
    private Label[]  _choiceLabels  = null!;

    private TaskCompletionSource<bool>? _modulePopupTcs;

    public SynonymHuntPage()
    {
        InitializeComponent();

        var rng0 = new Random();
        rng0.Shuffle(_modules);

        _shuffled = new int[_modules.Length][][];
        var rng = new Random();
        for (int m = 0; m < _modules.Length; m++)
        {
            _shuffled[m] = new int[_modules[m].Questions.Length][];
            for (int q = 0; q < _modules[m].Questions.Length; q++)
            {
                var order = new[] { 0, 1, 2, 3 };
                rng.Shuffle(order);
                _shuffled[m][q] = order;
            }
        }

        _choiceBorders = [ChoiceABorder, ChoiceBBorder, ChoiceCBorder, ChoiceDBorder];
        _choiceBadges  = [ChoiceABadge,  ChoiceBBadge,  ChoiceCBadge,  ChoiceDBadge];
        _choiceLabels  = [ChoiceALabel,  ChoiceBLabel,  ChoiceCLabel,  ChoiceDLabel];

        LoadQuestion();

        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetNavBarHasShadow(this, false);
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsVisible = false,
            IsEnabled = false
        });
    }

   
    private Task<bool> ShowModulePopupAsync(
        string icon, string title, string message, string confirmText)
    {
        ModulePopupIcon.Text           = icon;
        ModulePopupTitle.Text          = title;
        ModulePopupMessage.Text        = message;
        ModulePopupConfirmButton.Text  = confirmText;
        ModulePopupOverlay.IsVisible   = true;

        _modulePopupTcs = new TaskCompletionSource<bool>();
        return _modulePopupTcs.Task;
    }

    private void OnModulePopupConfirm(object sender, EventArgs e)
    {
        ModulePopupOverlay.IsVisible = false;
        _modulePopupTcs?.TrySetResult(true);
    }

    private void OnModulePopupCancel(object sender, EventArgs e)
    {
        ModulePopupOverlay.IsVisible = false;
        _modulePopupTcs?.TrySetResult(false);
    }

    private void LoadQuestion()
    {
        ResultsPanel.IsVisible = false;
        GamePanel.IsVisible    = true;

        var mod   = _modules[_currentModuleIndex];
        var q     = mod.Questions[_currentQuestionIndex];
        var order = _shuffled[_currentModuleIndex][_currentQuestionIndex];

        _answered = false;

        ModuleLabel.Text = $"Module {_currentModuleIndex + 1} · {mod.Name}";

        QuestionNumberLabel.Text  = (_currentQuestionIndex + 1).ToString();
        QuestionCounterLabel.Text = $"{_currentQuestionIndex + 1} / {mod.Questions.Length}";
        QuestionLabel.Text        = q.QuestionText;
        StoryLabel.Text           = q.Story;

        SetDifficultyBadge(mod.Difficulty);

        for (int i = 0; i < 4; i++)
        {
            string choiceText = q.Choices[order[i]];
            _choiceLabels[i].Text = char.ToUpper(choiceText[0]) + choiceText[1..];
            ResetChoiceStyle(i);
        }

        FeedbackBorder.IsVisible = false;

        NextButton.IsEnabled = false;
        NextButton.Text = _currentQuestionIndex < mod.Questions.Length - 1
            ? "Next →"
            : "Finish Module";

        UpdateProgressBar();
    }

    private void SetDifficultyBadge(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                DifficultyBadge.BackgroundColor = Color.FromArgb("#E8F5E9");
                DifficultyBadge.Stroke          = new SolidColorBrush(Color.FromArgb("#A5D6A7"));
                DifficultyLabel.Text      = "🟢  Easy";
                DifficultyLabel.TextColor = Color.FromArgb("#2E7D32");
                break;
            case "Average":
                DifficultyBadge.BackgroundColor = Color.FromArgb("#FFF8E1");
                DifficultyBadge.Stroke          = new SolidColorBrush(Color.FromArgb("#FFD54F"));
                DifficultyLabel.Text      = "🟡  Average";
                DifficultyLabel.TextColor = Color.FromArgb("#F57F17");
                break;
            default: // Intermediate
                DifficultyBadge.BackgroundColor = Color.FromArgb("#FBE9E7");
                DifficultyBadge.Stroke          = new SolidColorBrush(Color.FromArgb("#FFAB91"));
                DifficultyLabel.Text      = "🔴  Intermediate";
                DifficultyLabel.TextColor = Color.FromArgb("#BF360C");
                break;
        }
    }

    private void OnChoiceTapped(object sender, TappedEventArgs e)
    {
        if (_answered) return;
        if (e.Parameter is not string param || !int.TryParse(param, out int choiceIndex)) return;

        _answered = true;

        var mod   = _modules[_currentModuleIndex];
        var q     = mod.Questions[_currentQuestionIndex];
        var order = _shuffled[_currentModuleIndex][_currentQuestionIndex];

        string selectedWord = q.Choices[order[choiceIndex]];
        bool isCorrect      = selectedWord == q.Answer;

        if (isCorrect)
        {
            _score += 10;
            _totalCorrect++;
            ScoreLabel.Text = _score.ToString();
        }
        _totalAnswered++;

        for (int i = 0; i < 4; i++)
        {
            string word = q.Choices[order[i]];
            if (word == q.Answer)
                SetChoiceCorrect(i);
            else if (i == choiceIndex && !isCorrect)
                SetChoiceWrong(i);
        }

        ShowFeedback(isCorrect, q.Answer, q.QuestionText);

        NextButton.IsEnabled = true;
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        var mod = _modules[_currentModuleIndex];

        if (_currentQuestionIndex < mod.Questions.Length - 1)
        {
            _currentQuestionIndex++;
            LoadQuestion();
        }
        else
        {
            bool moreModules = _currentModuleIndex < _modules.Length - 1;

            string icon    = moreModules ? "🎉" : "🏆";
            string title   = "Module Complete!";
            string message = moreModules
                ? $"Great work on \"{mod.Name}\"!\nCurrent score: {_score} pts\n\nProceed to the next module?"
                : $"You've finished all modules!\nFinal score: {_score} pts";
            string confirm = moreModules ? "Next Module" : "See Results";

            bool proceed = await ShowModulePopupAsync(icon, title, message, confirm);

            if (proceed)
            {
                if (moreModules)
                {
                    _currentModuleIndex++;
                    _currentQuestionIndex = 0;
                    LoadQuestion();
                }
                else
                {
                    ShowResults();
                }
            }
        }
    }

   
    private async Task SaveScoreToDbAsync(int sessionScore)
    {
        string uid = Preferences.Default.Get("SavedUserUid", "");
        if (string.IsNullOrWhiteSpace(uid))
        {
            System.Diagnostics.Debug.WriteLine("[SynonymHunt] No UID — score not saved.");
            return;
        }

        string url = $"{BaseUrl}/userdata/{uid}";

        try
        {
            
            var getResponse = await _http.GetAsync(url);
            int existingScore = 0;

            if (getResponse.IsSuccessStatusCode)
            {
                var getJson = await getResponse.Content.ReadAsStringAsync();
                using var getDoc = JsonDocument.Parse(getJson);

                if (getDoc.RootElement.TryGetProperty("fields", out var fields) &&
                    fields.TryGetProperty("ScoreOfTotal", out var scoreProp))
                {
                    if (scoreProp.TryGetProperty("integerValue", out var intVal))
                        int.TryParse(intVal.GetString(), out existingScore);
                    else if (scoreProp.TryGetProperty("doubleValue", out var dblVal))
                        existingScore = (int)dblVal.GetDouble();
                }
            }

    
            int newTotal = existingScore + sessionScore;

            var body = new
            {
                fields = new
                {
                    ScoreOfTotal = new { integerValue = newTotal.ToString() }
                }
            };

            var patchUrl = $"{url}?updateMask.fieldPaths=ScoreOfTotal";
            var content  = new StringContent(
                JsonSerializer.Serialize(body),
                System.Text.Encoding.UTF8,
                "application/json");

            var patchResponse = await _http.PatchAsync(patchUrl, content);

            if (patchResponse.IsSuccessStatusCode)
                System.Diagnostics.Debug.WriteLine(
                    $"[SynonymHunt] Score saved — session: {sessionScore}, total: {newTotal}");
            else
                System.Diagnostics.Debug.WriteLine(
                    $"[SynonymHunt] PATCH failed: {(int)patchResponse.StatusCode}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SynonymHunt] SaveScoreToDbAsync error: {ex.Message}");
        }
    }

 

    private void ShowResults()
    {
        GamePanel.IsVisible    = false;
        ResultsPanel.IsVisible = true;

        int pct = _totalAnswered > 0
            ? (int)Math.Round((double)_totalCorrect / _totalAnswered * 100)
            : 0;

        ResultsScoreLabel.Text = _score.ToString();
        StatCorrectLabel.Text  = _totalCorrect.ToString();
        StatMissedLabel.Text   = (_totalAnswered - _totalCorrect).ToString();
        StatAccuracyLabel.Text = $"{pct}%";

        if (pct >= 90)
        {
            ResultsTrophyLabel.Text = "🏆";
            ResultsTitleLabel.Text  = "Outstanding!";
        }
        else if (pct >= 70)
        {
            ResultsTrophyLabel.Text = "🥈";
            ResultsTitleLabel.Text  = "Great Work!";
        }
        else
        {
            ResultsTrophyLabel.Text = "📚";
            ResultsTitleLabel.Text  = "Keep Practicing!";
        }

        // Save score to Firestore when the game ends
        _ = SaveScoreToDbAsync(_score);
        _scoreSaved = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (!_scoreSaved && _score > 0)
        {
            _scoreSaved = true;
            _ = SaveScoreToDbAsync(_score);
        }
    }

    private void OnRestartClicked(object sender, EventArgs e)
    {
        _currentModuleIndex   = 0;
        _currentQuestionIndex = 0;
        _answered             = false;
        _score                = 0;
        _totalAnswered        = 0;
        _totalCorrect         = 0;
        _scoreSaved           = false;
        ScoreLabel.Text       = "0";

        var rng0 = new Random();
        rng0.Shuffle(_modules);

        var rng = new Random();
        for (int m = 0; m < _modules.Length; m++)
            for (int q = 0; q < _modules[m].Questions.Length; q++)
                rng.Shuffle(_shuffled[m][q]);

        LoadQuestion();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void ResetChoiceStyle(int i)
    {
        _choiceBorders[i].BackgroundColor = Color.FromArgb("#F2F6FB");
        _choiceBorders[i].Stroke          = new SolidColorBrush(Color.FromArgb("#CBDCEB"));
        _choiceLabels[i].TextColor        = Color.FromArgb("#0F2D4A");
        _choiceBadges[i].BackgroundColor  = Color.FromArgb("#FFFFFF");
        _choiceBadges[i].Stroke           = new SolidColorBrush(Color.FromArgb("#CBDCEB"));

        if (_choiceBadges[i].Content is Label badgeLabel)
            badgeLabel.TextColor = Color.FromArgb("#6B8CAE");
    }

    private void SetChoiceCorrect(int i)
    {
        _choiceBorders[i].BackgroundColor = Color.FromArgb("#E6F5EF");
        _choiceBorders[i].Stroke          = new SolidColorBrush(Color.FromArgb("#1e8a5e"));
        _choiceLabels[i].TextColor        = Color.FromArgb("#1e8a5e");
        _choiceBadges[i].BackgroundColor  = Color.FromArgb("#1e8a5e");
        _choiceBadges[i].Stroke           = new SolidColorBrush(Color.FromArgb("#1e8a5e"));

        if (_choiceBadges[i].Content is Label badgeLabel)
            badgeLabel.TextColor = Colors.White;
    }

    private void SetChoiceWrong(int i)
    {
        _choiceBorders[i].BackgroundColor = Color.FromArgb("#FDECEA");
        _choiceBorders[i].Stroke          = new SolidColorBrush(Color.FromArgb("#c0392b"));
        _choiceLabels[i].TextColor        = Color.FromArgb("#c0392b");
        _choiceBadges[i].BackgroundColor  = Color.FromArgb("#c0392b");
        _choiceBadges[i].Stroke           = new SolidColorBrush(Color.FromArgb("#c0392b"));

        if (_choiceBadges[i].Content is Label badgeLabel)
            badgeLabel.TextColor = Colors.White;
    }

    private void ShowFeedback(bool correct, string answer, string questionText)
    {
        FeedbackBorder.IsVisible = true;
        string meaning = questionText
            .Replace("Find a word that means ", "")
            .TrimEnd('.');

        if (correct)
        {
            FeedbackBorder.BackgroundColor = Color.FromArgb("#E6F5EF");
            FeedbackBorder.Stroke          = new SolidColorBrush(Color.FromArgb("#B8E6D4"));
            FeedbackIcon.Text              = "✓";
            FeedbackIcon.TextColor         = Colors.White;
            FeedbackLabel.Text             = $"Correct! \"{answer}\" means {meaning}";
            FeedbackLabel.TextColor        = Color.FromArgb("#1e8a5e");

            if (FeedbackIcon.Parent is Border iconBorder)
                iconBorder.BackgroundColor = Color.FromArgb("#1e8a5e");
        }
        else
        {
            FeedbackBorder.BackgroundColor = Color.FromArgb("#FFF3E0");
            FeedbackBorder.Stroke          = new SolidColorBrush(Color.FromArgb("#FFD699"));
            FeedbackIcon.Text              = "!";
            FeedbackIcon.TextColor         = Colors.White;
            FeedbackLabel.Text             = $"The answer is \"{answer}\" — it means {meaning}";
            FeedbackLabel.TextColor        = Color.FromArgb("#a05c00");

            if (FeedbackIcon.Parent is Border iconBorder)
                iconBorder.BackgroundColor = Color.FromArgb("#e67e22");
        }
    }

    private void UpdateProgressBar()
    {
        var mod      = _modules[_currentModuleIndex];
        double ratio = (double)_currentQuestionIndex / mod.Questions.Length;

        ProgressFill.AnchorX      = 0;
        ProgressFill.ScaleX       = ratio;
        ProgressFill.WidthRequest = 1000; 
    }
}
