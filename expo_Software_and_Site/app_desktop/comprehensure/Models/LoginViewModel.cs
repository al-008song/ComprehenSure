using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System.Text.Json;
using comprehensure.Models;

namespace comprehensure.DataBaseControl.Models
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly string projectId = "comprehensuredb-f9f7c";
        private string BaseUrl =>
            $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents";
        private readonly HttpClient client = new HttpClient();

        private bool _isNavigating = false;

        public async Task Toastshow(string showtext)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            var toast = Toast.Make(showtext, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }

        public async Task getms()
        {
            string dateString = DateTime.Now.ToString("M/d/yyyy h:mm:ss.fff tt");
            DateTime dateValue = DateTime.Parse(dateString);
            string timems = dateValue.ToString("fff");

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                _ = Toastshow($"Ms Checker Login completed in: {timems} MS");
            else
                await Shell.Current.DisplayAlert("Ms Checker", $"Login completed in: {timems} MS", "OK");
        }

        [RelayCommand]
        public async Task GoToSignUp()
        {
            await Shell.Current.GoToAsync("SignUpPage");
        }

        public LoginViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        [ObservableProperty]
        private string _Password;

        [ObservableProperty]
        private string _Email;

        private async Task<bool> HasUsername(string uid)
        {
            string url = $"{BaseUrl}/userdata/{uid}";
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return false;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var fields = doc.RootElement.GetProperty("fields");

                bool hasUsername = false;
                string username  = string.Empty;
                int modules      = 0;
                int score        = 0;

                if (fields.TryGetProperty("UserHasUserName", out var hasUserNameProp))
                    if (hasUserNameProp.TryGetProperty("booleanValue", out var boolVal))
                        hasUsername = boolVal.GetBoolean();

                if (fields.TryGetProperty("Username", out var usernameProp))
                    username = usernameProp.GetProperty("stringValue").GetString() ?? string.Empty;

                if (fields.TryGetProperty("ModuleFinished", out var modProp))
                    int.TryParse(modProp.GetProperty("integerValue").GetString(), out modules);

                if (fields.TryGetProperty("ScoreOfTotal", out var scoreProp))
                    int.TryParse(scoreProp.GetProperty("integerValue").GetString(), out score);

                if (hasUsername && !string.IsNullOrEmpty(username))
                    UserCache.SaveUser(uid, _Email?.Trim() ?? string.Empty, username, modules, score);

                return hasUsername;
            }
            catch
            {
                return false;
            }
        }

        private (string title, string message) ParseFirebaseError(Exception ex)
        {
            string raw = ex.Message.ToLower();
            System.Diagnostics.Debug.WriteLine($"[LoginViewModel] Firebase error raw: {ex.Message}");

            if (raw.Contains("email_not_found") ||
                raw.Contains("user_not_found") ||
                raw.Contains("there is no user record"))
                return ("Account Not Found",
                        "No account exists with that email address.\nPlease check your email or sign up.");

            if (raw.Contains("invalid_password") ||
                raw.Contains("wrong_password") ||
                raw.Contains("invalid credential") ||
                raw.Contains("invalid login credentials") ||
                raw.Contains("invalid_login_credentials"))
                return ("Incorrect Password",
                        "The password you entered is wrong.\nPlease try again.");

            if (raw.Contains("invalid_email") ||
                raw.Contains("badly formatted"))
                return ("Invalid Email",
                        "The email address format looks incorrect.\nExample: yourname@gmail.com");

            if (raw.Contains("user_disabled"))
                return ("Account Disabled",
                        "This account has been disabled.\nPlease contact support.");

            if (raw.Contains("too_many_attempts") ||
                raw.Contains("too many attempts") ||
                raw.Contains("access_token_expired"))
                return ("Too Many Attempts",
                        "Too many failed login attempts.\nPlease wait a few minutes and try again.");

            if (raw.Contains("network") ||
                raw.Contains("unable to resolve") ||
                raw.Contains("connection"))
                return ("No Connection",
                        "Could not reach the server.\nPlease check your internet connection.");

            string readable = ex.Message.Contains(":")
                ? ex.Message.Split(':').Last().Trim()
                : ex.Message;

            if (string.IsNullOrWhiteSpace(readable))
                readable = ex.Message;

            return ("Login Failed", readable);
        }

        [RelayCommand]
        private async Task Login()
        {
            if (_isNavigating) return;
            _isNavigating = true;

            string emailcl    = _Email?.Trim();
            string passwordcl = _Password?.Trim();

            if (string.IsNullOrEmpty(emailcl))
            {
                await Shell.Current.DisplayAlert("Missing Email", "Please enter your email address.", "OK");
                _isNavigating = false;
                return;
            }

            if (!emailcl.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.DisplayAlert("Invalid Email",
                    "Only Gmail accounts are allowed.\nPlease use a @gmail.com address.", "OK");
                _isNavigating = false;
                return;
            }

            if (string.IsNullOrEmpty(passwordcl))
            {
                await Shell.Current.DisplayAlert("Missing Password", "Please enter your password.", "OK");
                _isNavigating = false;
                return;
            }

            try
            {
                var result = await _authClient.SignInWithEmailAndPasswordAsync(emailcl, passwordcl);
                _ = getms();

                string uid = result.User.Uid;

                if (UserCache.IsCached(uid))
                {
                    await Shell.Current.DisplayAlert("Login Complete", "Welcome back, " + UserCache.Username, "OK");
                    await QuizFunc.InitializeLockFieldsAsync();
                    await Shell.Current.GoToAsync("///MainDashboard");
                    return;
                }

                bool hasUsername = await HasUsername(uid);

                if (hasUsername)
                {
                    await Shell.Current.DisplayAlert("Login Complete", "Welcome, " + emailcl, "OK");
                    await QuizFunc.InitializeLockFieldsAsync();
                    await Shell.Current.GoToAsync("///MainDashboard");
                }
                else
                {
                    await Shell.Current.GoToAsync($"///UsernameReq?email={emailcl}&uid={uid}");
                }
            }
            catch (Firebase.Auth.FirebaseAuthHttpException ex)
            {
                var (title, message) = ParseFirebaseError(ex);
                await Shell.Current.DisplayAlert(title, message, "OK");
            }
            catch (Exception ex)
            {
                var (title, message) = ParseFirebaseError(ex);
                await Shell.Current.DisplayAlert(title, message, "OK");
            }
            finally
            {
                _isNavigating = false;
            }
        }
    }
}
