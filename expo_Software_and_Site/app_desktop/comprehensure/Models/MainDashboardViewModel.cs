using System.Text.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using comprehensure.DASHBOARD;
using comprehensure.Models;

namespace comprehensure.DataBaseControl.Models
{
    [QueryProperty(nameof(firstlogin), "firstwelcome")]
    public partial class MainDashboardViewModel : ObservableObject
    {
        private readonly string projectId = "comprehensuredb-f9f7c";
        private string BaseUrl =>
            $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents";
        private readonly HttpClient client = new HttpClient();

        private List<(string Name, int Score)> _cachedLeaderboard = new();
        private DateTime _leaderboardFetchedAt = DateTime.MinValue;
        private static readonly TimeSpan LeaderboardTtl = TimeSpan.FromMinutes(5);

        private string _cachedUsername = null;
        private double _cachedAccountComprehension = -1; // -1 = not yet loaded aka it be empty fr it shows up like this -

        [ObservableProperty]
        private string firstPlayerName = "—";

        [ObservableProperty]
        private string firstPlayerScore = "0 pts";

        [ObservableProperty]
        private string secondPlayerName = "—";

        [ObservableProperty]
        private string secondPlayerScore = "0 pts";

        [ObservableProperty]
        private string thirdPlayerName = "—";

        [ObservableProperty]
        private string thirdPlayerScore = "0 pts";

        [ObservableProperty]
        private string _UsernameEdit;

        [ObservableProperty]
        private int _score;

        [ObservableProperty]
        public bool firstlogin = false;

        [ObservableProperty]
        private double _strokeOffset = 0;

        [ObservableProperty]
        private double _accountComprehension;

        [ObservableProperty]
        private string _displayPercentage = "0%";

        [ObservableProperty]
        private string _scoreDisplay = "—";

        [ObservableProperty]
        private bool _isMinigameLocked = true;

        partial void OnIsMinigameLockedChanged(bool value)
        {
            OnPropertyChanged(nameof(MinigameOpacity));
            OnPropertyChanged(nameof(IsMinigameUnlocked));
        }

        public double MinigameOpacity => _isMinigameLocked ? 0.45 : 1.0;
        public bool IsMinigameUnlocked => !_isMinigameLocked;


        private const double MaxComprehension = 100.0;

        [RelayCommand]
        public async Task modules()
        {
            await Shell.Current.GoToAsync("///ModuleDashboard");
        }

        [RelayCommand]
        public async Task AboutUs()
        {
            await Shell.Current.GoToAsync(nameof(AboutUs));
        }

        [RelayCommand]
        public async Task NavigateToFourPics()
        {
            await Shell.Current.GoToAsync("///FourPicsOneTheme");
        }

        public async Task Toastshow(string showtext)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make(showtext, ToastDuration.Long, 14);
            await toast.Show(cancellationTokenSource.Token);
        }

   

        public MainDashboardViewModel()
        {
            _ = CalculateProgress();
            
        }

        private async Task RunPeriodicGhostCheckAsync()
        {
            await GhostUserChecker.CheckAndHandleGhostUserAsync();
        }

        private static int ReadFirestoreInt(JsonElement integerValueElement)
        {
            if (integerValueElement.ValueKind == JsonValueKind.String)
            {
                int.TryParse(integerValueElement.GetString(), out int parsed);
                return parsed;
            }
            if (integerValueElement.ValueKind == JsonValueKind.Number)
                return integerValueElement.GetInt32();
            return 0;
        }

        private static double ReadFirestoreDouble(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number)
                return element.GetDouble();
            if (
                element.ValueKind == JsonValueKind.String
                && double.TryParse(element.GetString(), out double parsed)
            )
                return parsed;
            return 0;
        }

        public async Task OnAppearing()
        {
           // await GhostUserChecker.CheckAndHandleGhostUserAsync(); 
           
            await Task.Delay(650);

            bool redirected = await RedirectIfNoUsername();
            if (redirected)
                return;

            _ = QuizFunc.InitializeLockFieldsAsync();

            await QuizFunc.SaveTotalProgressAsync();

            ApplyCachedProfile();

            await showloginwelcome(); // uses _cachedUsername it will not read anything (do not bother)

            await LoadAccountComprehensionFromDb();
            await LoadScoreOfTotalFromDb();

            IsMinigameLocked = await checkforminigameunlock();

            await Task.Delay(1050);

            await scoreboard();
        }

        public async Task<bool> checkforminigameunlock()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrWhiteSpace(uid))
                return true; 

            try
            {
                string url = $"{BaseUrl}/StoryPage/{uid}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return true;

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonSerializer.Deserialize<JsonElement>(json);

                if (
                    doc.TryGetProperty("fields", out var fields)
                    && fields.TryGetProperty("isminigamelocked", out var field)
                    && field.TryGetProperty("booleanValue", out var boolVal)
                )
                {
                    return boolVal.GetBoolean();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[checkforminigameunlock] Exception: {ex.Message}"
                );
            }

            return true; 
        }

        private async Task<bool> RedirectIfNoUsername()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            string email = Preferences.Default.Get("SavedUserEmail", "");

            if (string.IsNullOrEmpty(uid))
            {
                await Shell.Current.GoToAsync("///MainPage");
                return true;
            }

            string url = $"{BaseUrl}/userdata/{uid}";
            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    await Shell.Current.GoToAsync($"///UsernameReq?email={email}&uid={uid}");
                    return true;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var fields = doc.RootElement.GetProperty("fields");

                if (fields.TryGetProperty("UserHasUserName", out var hasUserNameProp))
                    if (
                        hasUserNameProp.TryGetProperty("booleanValue", out var boolVal)
                        && !boolVal.GetBoolean()
                    )
                    {
                        await Shell.Current.GoToAsync($"///UsernameReq?email={email}&uid={uid}");
                        return true;
                    }

                if (fields.TryGetProperty("Username", out var usernameProp))
                {
                    string username = usernameProp.GetProperty("stringValue").GetString();

                    if (string.IsNullOrWhiteSpace(username))
                    {
                        await Shell.Current.GoToAsync($"///UsernameReq?email={email}&uid={uid}");
                        return true;
                    }

                    _cachedUsername = username;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void ApplyCachedProfile()
        {
            if (!string.IsNullOrEmpty(_cachedUsername))
                UsernameEdit = _cachedUsername;
            else
                UsernameEdit = "User not found";

            if (_cachedAccountComprehension >= 0)
            {
                AccountComprehension = _cachedAccountComprehension;
                _ = CalculateProgress();
            }

       
            if (!string.IsNullOrEmpty(_cachedUsername))
                Preferences.Default.Set("CachedUsername", _cachedUsername);
        }

       
        public async Task showloginwelcome()
        {
            bool isFirst = Preferences.Default.Get("IsFirstLogin", false);
            if (isFirst)
            {
                string name = _cachedUsername ?? Preferences.Default.Get("CachedUsername", "User");
                await Shell.Current.DisplayAlert("Success", $"Welcome back, {name}", "OK");
                Preferences.Default.Set("IsFirstLogin", false);
            }
        }

        public async Task scoreboard(bool forceRefresh = false)
        {
            bool cacheValid =
                !forceRefresh
                && _cachedLeaderboard.Count > 0
                && (DateTime.UtcNow - _leaderboardFetchedAt) < LeaderboardTtl;

            if (cacheValid)
            {
                ApplyLeaderboard(_cachedLeaderboard);
                System.Diagnostics.Debug.WriteLine(
                    "[scoreboard] Served from cache — no Firestore read."
                );
                return;
            }

            string url = $"{BaseUrl}/userdata";
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[scoreboard] HTTP {(int)response.StatusCode}"
                    );
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("documents", out var documents))
                {
                    System.Diagnostics.Debug.WriteLine("[scoreboard] No 'documents' key found.");
                    return;
                }

                var entries = new List<(string Name, int Score)>();

                foreach (var document in documents.EnumerateArray())
                {
                    if (!document.TryGetProperty("fields", out var fields))
                        continue;

                    if (
                        !fields.TryGetProperty("Username", out var usernameProp)
                        || !usernameProp.TryGetProperty("stringValue", out var nameVal)
                    )
                        continue;

                    string name = nameVal.GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    int score = 0;
                    if (
                        fields.TryGetProperty("AccountComprehension", out var acProp)
                        && acProp.TryGetProperty("doubleValue", out var acVal)
                    )
                        score = (int)Math.Round(ReadFirestoreDouble(acVal));
                    else if (
                        fields.TryGetProperty("ScoreOfTotal", out var sotProp)
                        && sotProp.TryGetProperty("integerValue", out var sotVal)
                    )
                        score = ReadFirestoreInt(sotVal);
                    else if (
                        fields.TryGetProperty("ModuleFinished", out var mfProp)
                        && mfProp.TryGetProperty("integerValue", out var mfVal)
                    )
                        score = ReadFirestoreInt(mfVal);

                    entries.Add((name, score));
                }

                _cachedLeaderboard = entries.OrderByDescending(e => e.Score).Take(3).ToList();
                _leaderboardFetchedAt = DateTime.UtcNow;

                ApplyLeaderboard(_cachedLeaderboard);
                System.Diagnostics.Debug.WriteLine(
                    "[scoreboard] Fetched from Firestore and cached."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[scoreboard] Exception: {ex.Message}");
            }
        }

        private void ApplyLeaderboard(List<(string Name, int Score)> top3)
        {
            FirstPlayerName = top3.Count >= 1 ? top3[0].Name : "—";
            FirstPlayerScore = top3.Count >= 1 ? $"{top3[0].Score} pts" : "0 pts";
            SecondPlayerName = top3.Count >= 2 ? top3[1].Name : "—";
            SecondPlayerScore = top3.Count >= 2 ? $"{top3[1].Score} pts" : "0 pts";
            ThirdPlayerName = top3.Count >= 3 ? top3[2].Name : "—";
            ThirdPlayerScore = top3.Count >= 3 ? $"{top3[2].Score} pts" : "0 pts";
        }

    
        [RelayCommand]
        private async Task AddValue()
        {
            AccountComprehension++;
            await CalculateProgress();
            UpdateLeaderboardCacheForCurrentUser((int)Math.Round(AccountComprehension));
        }

        [RelayCommand]
        private async Task SubtractValue()
        {
            AccountComprehension--;
            await CalculateProgress();
            UpdateLeaderboardCacheForCurrentUser((int)Math.Round(AccountComprehension));
        }

       
        private void UpdateLeaderboardCacheForCurrentUser(int newScore)
        {
            if (_cachedLeaderboard == null || string.IsNullOrEmpty(_cachedUsername))
                return;

            var updated = _cachedLeaderboard
                .Where(e => e.Name != _cachedUsername)
                .Append((_cachedUsername, newScore))
                .OrderByDescending(e => e.Item2)
                .Take(3)
                .Select(e => (Name: e.Item1, Score: e.Item2))
                .ToList();

            _cachedLeaderboard = updated;
            ApplyLeaderboard(_cachedLeaderboard);
        }

    
        public double valuecheck()
        {
            if (AccountComprehension < 0)
                AccountComprehension = 0;
            else if (AccountComprehension > MaxComprehension)
                AccountComprehension = MaxComprehension;
            return AccountComprehension;
        }

        public async Task CalculateProgress()
        {
            valuecheck();
          
            StrokeOffset = -AccountComprehension * 0.25133;
            DisplayPercentage = $"{(int)Math.Round(AccountComprehension)}%";
        }

        [RelayCommand]
        public async Task ProfileDashboard()
        {
            await Shell.Current.GoToAsync("///ProfileDashboard");
        }

       
        public async Task LoadAccountComprehensionFromDb()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrEmpty(uid))
                return;

            string url = $"{BaseUrl}/StoryPage/{uid}";
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var fields = doc.RootElement.GetProperty("fields");

                if (
                    fields.TryGetProperty("AccountComprehension", out var acProp)
                    && acProp.TryGetProperty("doubleValue", out var acVal)
                )
                {
                    double value = ReadFirestoreDouble(acVal);
                    AccountComprehension = value;
                    _cachedAccountComprehension = value;
                    await CalculateProgress();
                    System.Diagnostics.Debug.WriteLine(
                        $"[LoadAccountComprehensionFromDb] AccountComprehension = {value}"
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[LoadAccountComprehensionFromDb] {ex.Message}"
                );
            }
        }
        public async Task LoadScoreOfTotalFromDb()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrEmpty(uid))
                return;

            string url = $"{BaseUrl}/userdata/{uid}";
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var fields = doc.RootElement.GetProperty("fields");

                if (
                    fields.TryGetProperty("ScoreOfTotal", out var sotProp)
                    && sotProp.TryGetProperty("integerValue", out var sotVal)
                )
                {
                    int score = ReadFirestoreInt(sotVal);
                    ScoreDisplay = $"{score} pts";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadScoreOfTotalFromDb] {ex.Message}");
            }
        }
    }
}
