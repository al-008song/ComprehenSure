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
        private int _cachedModuleFinished = -1; // -1 = not yet loaded

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
        private double _strokeOffset = 100;

        [ObservableProperty]
        private int _moduleFinished;

        [ObservableProperty]
        private string _displayPercentage = "0%";

        // ── Minigame lock ─────────────────────────────────────────────────────
        [ObservableProperty]
        private bool _isMinigameLocked = true;

        // Notifies XAML Opacity binding whenever the lock state changes
        partial void OnIsMinigameLockedChanged(bool value)
        {
            OnPropertyChanged(nameof(MinigameOpacity));
            OnPropertyChanged(nameof(IsMinigameUnlocked)); // add this line
        }

        public double MinigameOpacity => _isMinigameLocked ? 0.45 : 1.0;
        public bool IsMinigameUnlocked => !_isMinigameLocked;  // add this;
        // ─────────────────────────────────────────────────────────────────────

        private readonly int _moduleCount = 8;
        private readonly int score_count_max = 80;

       

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


        public async Task OnAppearing()
        {
            await Task.Delay(650);


            bool redirected = await RedirectIfNoUsername();
            if (redirected)
                return;


            _ = QuizFunc.InitializeLockFieldsAsync();


            ApplyCachedProfile();

            await showloginwelcome(); // uses _cachedUsername — no read

            // Load minigame lock state from Firestore
            IsMinigameLocked = await checkforminigameunlock();

            await Task.Delay(1050);


            await scoreboard();
        }

        // ── Reads isminigamelocked from Firestore for the current user ────────
        public async Task<bool> checkforminigameunlock()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrWhiteSpace(uid)) return true; // fail-safe: locked

            try
            {
                string url = $"{BaseUrl}/StoryPage/{uid}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode) return true;

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonSerializer.Deserialize<JsonElement>(json);

                if (doc.TryGetProperty("fields", out var fields) &&
                    fields.TryGetProperty("isminigamelocked", out var field) &&
                    field.TryGetProperty("booleanValue", out var boolVal))
                {
                    return boolVal.GetBoolean();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[checkforminigameunlock] Exception: {ex.Message}");
            }

            return true; // default locked on any error
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

                // ── Redirect checks ──────────────────────────────────────────
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

                    // ── Populate profile cache from this single read ──────────
                    _cachedUsername = username;
                }

                // Cache ModuleFinished from the same read
                if (
                    fields.TryGetProperty("ModuleFinished", out var moduleProp)
                    && moduleProp.TryGetProperty("integerValue", out var modVal)
                )
                {
                    _cachedModuleFinished = ReadFirestoreInt(modVal);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Apply cached profile to observable properties — zero Firestore reads
        // ─────────────────────────────────────────────────────────────────────
        private void ApplyCachedProfile()
        {
            if (!string.IsNullOrEmpty(_cachedUsername))
                UsernameEdit = _cachedUsername;
            else
                UsernameEdit = "User not found";

            if (_cachedModuleFinished >= 0)
            {
                ModuleFinished = _cachedModuleFinished;
                _ = CalculateProgress();
            }

            // Persist username to Preferences so other pages can read it
            // without a Firestore call
            if (!string.IsNullOrEmpty(_cachedUsername))
                Preferences.Default.Set("CachedUsername", _cachedUsername);
        }

        // ─────────────────────────────────────────────────────────────────────
        // showloginwelcome  –  uses cached username, NO Firestore read
        // ─────────────────────────────────────────────────────────────────────
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

        // ─────────────────────────────────────────────────────────────────────
        // scoreboard  –  cached; only re-fetches after LeaderboardTtl expires
        // ─────────────────────────────────────────────────────────────────────
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

        // ─────────────────────────────────────────────────────────────────────
        // Score commands  –  update local state instantly, write to Firestore,
        //                    then update the in-memory leaderboard cache so the
        //                    UI stays accurate WITHOUT an extra scoreboard read.
        // ─────────────────────────────────────────────────────────────────────
        [RelayCommand]
        private async Task AddValue()
        {
            ModuleFinished++;
            await modulescoredb();
        }

        [RelayCommand]
        private async Task SubtractValue()
        {
            ModuleFinished--;
            await modulescoredb();
        }

        public async Task modulescoredb()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrEmpty(uid))
                return;

            valuecheck();
            await CalculateProgress();

            // Update the leaderboard cache in memory so the UI reflects the
            // new score without a Firestore read.
            UpdateLeaderboardCacheForCurrentUser(ModuleFinished);

            string url = $"{BaseUrl}/userdata/{uid}?updateMask.fieldPaths=ModuleFinished";
            var data = new
            {
                fields = new { ModuleFinished = new { integerValue = ModuleFinished.ToString() } },
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
            var json = JsonSerializer.Serialize(data, options);

            try
            {
                var response = await client.PatchAsync(
                    url,
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[modulescoredb] Save failed: {error}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[modulescoredb] Saved ModuleFinished = {ModuleFinished}"
                    );
                    // NOTE: scoreboard() is intentionally NOT called here.
                    // The cache is already updated in-memory above.
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[modulescoredb] Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// After a local score change, update the in-memory leaderboard cache
        /// for the current user so the UI stays correct without a Firestore read.
        /// </summary>
        private void UpdateLeaderboardCacheForCurrentUser(int newScore)
        {
            if (_cachedLeaderboard == null || string.IsNullOrEmpty(_cachedUsername))
                return;

            // Replace or insert the current user's entry
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

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────
        public int valuecheck()
        {
            if (ModuleFinished < 0)
                ModuleFinished = 0;
            else if (ModuleFinished > _moduleCount)
                ModuleFinished = 8;
            return ModuleFinished;
        }

        public async Task CalculateProgress()
        {
            valuecheck();
            float resultModule = ((float)ModuleFinished / _moduleCount) * 100;
            StrokeOffset = -ModuleFinished * 4.9;
            DisplayPercentage = $"{resultModule}%";
        }

        [RelayCommand]
        public async Task ProfileDashboard()
        {
            await Shell.Current.GoToAsync("///ProfileDashboard");
        }

        // ─────────────────────────────────────────────────────────────────────
        // LoadModuleFinishedFromDb  –  kept for external callers but now also
        // updates the profile cache so the data stays consistent.
        // ─────────────────────────────────────────────────────────────────────
        public async Task LoadModuleFinishedFromDb()
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
                    fields.TryGetProperty("ModuleFinished", out var moduleProp)
                    && moduleProp.TryGetProperty("integerValue", out var modVal)
                )
                {
                    int value = ReadFirestoreInt(modVal);
                    ModuleFinished = value;
                    _cachedModuleFinished = value;
                    await CalculateProgress();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadModuleFinished] {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // AutoRefresh  –  REMOVED (was 2 Firestore reads every second).
        // If periodic background refresh is needed in future, use a minimum
        // interval of 5+ minutes and only refresh when the page is visible.
        // ─────────────────────────────────────────────────────────────────────
    }
}
