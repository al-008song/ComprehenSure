using System.Text.Json;

namespace comprehensure
{
    public partial class App : Application
    {
        private readonly string projectId = "comprehensuredb-f9f7c";
        private string BaseUrl =>
            $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents";
        private readonly HttpClient client = new HttpClient();

        public App()
        {
            InitializeComponent();
            Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = new AppShell();

            // Delay navigation until after the Shell window is fully rendered,
            // so Shell.Current is non-null and GoToAsync won't silently fail.
            shell.Loaded += async (s, e) =>
            {
                // Brief pause so the initial ShellContent (Login) doesn't flash
                await Task.Delay(150);
                await NavigateToStartPageAsync();
            };

            return new Window(shell);
        }

        private async Task NavigateToStartPageAsync()
        {
            await Task.Delay(2000);

            string savedUid = Preferences.Default.Get("SavedUserUid", "");
            string savedEmail = Preferences.Default.Get("SavedUserEmail", "");

            // FIX: No UID = go to MainPage (landing/welcome screen), not Login directly
            if (string.IsNullOrEmpty(savedUid))
            {
                await Shell.Current.GoToAsync("///MainPage");
                return;
            }

            bool hasUsername = await CheckHasUsername(savedUid);

            if (hasUsername)
                await Shell.Current.GoToAsync("///MainDashboard");
            else
                await Shell.Current.GoToAsync($"///UsernameReq?email={Uri.EscapeDataString(savedEmail)}&uid={Uri.EscapeDataString(savedUid)}");
        }

        // OnStart() removed — navigation is now driven by the shell.Loaded event
        // above, which guarantees Shell.Current is ready before GoToAsync is called.

        private async Task<bool> CheckHasUsername(string uid)
        {
            string url = $"{BaseUrl}/userdata/{uid}";
            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var fields = doc.RootElement.GetProperty("fields");

                if (fields.TryGetProperty("UserHasUserName", out var hasUserNameProp))
                    if (hasUserNameProp.TryGetProperty("booleanValue", out var boolVal))
                        return boolVal.GetBoolean();

                if (fields.TryGetProperty("Username", out var usernameProp))
                {
                    string username = usernameProp.GetProperty("stringValue").GetString();
                    return !string.IsNullOrWhiteSpace(username);
                }

                return false;
            }
            catch
            {
                return !string.IsNullOrEmpty(uid);
            }
        }

        public static async Task HandleConnectivityAsync(NetworkAccess networkAccess)
        {
            if (networkAccess == NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Connected", "You are now online.", "OK");
            }
            else if (networkAccess == NetworkAccess.None)
            {
                await Shell.Current.DisplayAlert(
                    "Connection Lost",
                    "Network is required to use this app",
                    "OK"
                );
            }
        }

        private async void Connectivity_ConnectivityChanged(
            object sender,
            ConnectivityChangedEventArgs e
        )
        {
            await HandleConnectivityAsync(e.NetworkAccess);
        }
    }
}
