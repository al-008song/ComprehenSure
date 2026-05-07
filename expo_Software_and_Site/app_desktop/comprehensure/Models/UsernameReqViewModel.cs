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
            _ = UsernameExists(_username);
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
                _ = UserCreation();
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

            _ = UserCreation();
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
