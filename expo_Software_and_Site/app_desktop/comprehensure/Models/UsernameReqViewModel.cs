using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Firebase.Auth;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;

namespace comprehensure.DataBaseControl.Models
{
    [QueryProperty(nameof(UserEmail), "email")]
    [QueryProperty(nameof(UserUid), "uid")]
    public partial class UsernameReqViewModel : ObservableObject
    {
        [ObservableProperty]
        private string[] _achievements;

        [ObservableProperty]
        private string _userEmail;

        [ObservableProperty]
        private string _userUid;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _usertime = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");

        private string BaseUrl =>
            $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents";
        private readonly HttpClient client = new HttpClient();
        private readonly string projectId = "comprehensuredb-f9f7c";

        public async Task Toastshow(string showtext)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            var toast = Toast.Make(showtext, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }

        public UsernameReqViewModel() { }

        [RelayCommand]
        public async Task UsernameCheck()
        {
            // FIX:thank god may documentation
            // instead of silently killing navigation and kicking back to MainPage.
            try
            {
                if (string.IsNullOrWhiteSpace(_username))
                {
                    await Shell.Current.DisplayAlert("Missing Username", "Please enter a username.", "OK");
                    return;
                }

                bool emailAlreadyExists = await EmailExists(UserEmail);
                if (emailAlreadyExists)
                    return;

            
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

    
        private async Task<bool> EmailExists(string email)
        {
            string url  = $"{BaseUrl}:runQuery";
            string json = JsonSerializer.Serialize(new
            {
                structuredQuery = new
                {
                    from  = new[] { new { collectionId = "userdata" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "Email" },
                            op    = "EQUAL",
                            value = new { stringValue = email },
                        },
                    },
                    limit = 1,
                },
            });

            HttpResponseMessage response = await client.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            if (string.IsNullOrEmpty(result))
                return false;

            using var doc  = JsonDocument.Parse(result);
            var root       = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in root.EnumerateArray())
                {
                    if (element.TryGetProperty("document", out var documentProp))
                    {
                        if (documentProp.TryGetProperty("fields", out var fields))
                        {
                            // Pull the stored username so we can cache it.
                            string existingUsername = fields
                                .TryGetProperty("Username", out var uProp)
                                    ? uProp.GetProperty("stringValue").GetString() ?? string.Empty
                                    : string.Empty;

                            string existingUid = fields
                                .TryGetProperty("Uid", out var uidProp)
                                    ? uidProp.GetProperty("stringValue").GetString() ?? UserUid
                                    : UserUid;

                            int moduleFinished = 0;
                            int scoreOfTotal   = 0;

                            if (fields.TryGetProperty("ModuleFinished", out var mfProp) &&
                                mfProp.TryGetProperty("integerValue", out var mfVal))
                                int.TryParse(mfVal.GetString(), out moduleFinished);

                            if (fields.TryGetProperty("ScoreOfTotal", out var stProp) &&
                                stProp.TryGetProperty("integerValue", out var stVal))
                                int.TryParse(stVal.GetString(), out scoreOfTotal);

                            await Shell.Current.DisplayAlert(
                                "Account Found",
                                $"An account with this email already exists (username: \"{existingUsername}\"). Logging you in.",
                                "OK"
                            );

                            Preferences.Default.Set("SavedUserUid",   existingUid);
                            Preferences.Default.Set("SavedUserEmail", email);

                            UserCache.SaveUser(existingUid, email, existingUsername,
                                               moduleFinished, scoreOfTotal);

                            await Shell.Current.GoToAsync(
                                $"MainDashboard?uid={existingUid}&baseUrl={BaseUrl}");

                            return true; 
                        }
                    }
                }
            }

            return false;
        }

        private async Task<bool> UsernameExists(string username)
        {
            username = username.Trim().ToLower();
            Username = username;

            string url  = $"{BaseUrl}:runQuery";
            string json = JsonSerializer.Serialize(new
            {
                structuredQuery = new
                {
                    from  = new[] { new { collectionId = "userdata" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "Username" },
                            op    = "EQUAL",
                            value = new { stringValue = username },
                        },
                    },
                    limit = 1,
                },
            });

            HttpResponseMessage response = await client.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                
                await Shell.Current.DisplayAlert($"Error {(int)response.StatusCode}", result, "OK");
                return false;
            }

            if (string.IsNullOrEmpty(result))
            {
                await Shell.Current.DisplayAlert("Error", "Empty response from server.", "OK");
                return false;
            }

            using var doc  = JsonDocument.Parse(result);
            var root       = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in root.EnumerateArray())
                {
                    if (element.TryGetProperty("document", out var documentProp))
                    {
                        if (documentProp.TryGetProperty("fields", out _))
                        {
                            await Shell.Current.DisplayAlert(
                                "Username Taken",
                                $"\"{Username}\" is already in use. Please choose a different username.",
                                "OK"
                            );
                            return true;
                        }
                    }
                }
            }

          
            await UserCreation();
            return false;
        }

        public async Task UserCreation()
        {
            var data = new
            {
                fields = new
                {
                    Username        = new { stringValue  = Username },
                    Email           = new { stringValue  = UserEmail },
                    DeviceTimeOfReg = new { stringValue  = Usertime },
                    Uid             = new { stringValue  = UserUid },
                    ServerTimeOfReg = new { stringValue  = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
                    ModuleFinished  = new { integerValue = "0" },
                    ScoreOfTotal    = new { integerValue = "0" },
                    UserHasUserName = new { booleanValue = true },
                },
            };

            var options  = new JsonSerializerOptions { PropertyNamingPolicy = null };
            var json     = JsonSerializer.Serialize(data, options);
            var response = await client.PatchAsync(
                $"{BaseUrl}/userdata/{UserUid}",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert($"Error {(int)response.StatusCode}", result, "OK");
            }
            else
            {
                Preferences.Default.Set("SavedUserUid",   UserUid);
                Preferences.Default.Set("SavedUserEmail", UserEmail);

                UserCache.SaveUser(UserUid, UserEmail, Username, moduleFinished: 0, scoreOfTotal: 0);

                await Shell.Current.DisplayAlert("Success", "Account Registered for " + Username, "OK");
                await Shell.Current.GoToAsync($"MainDashboard?uid={UserUid}&baseUrl={BaseUrl}");
            }
        }
    }
}
