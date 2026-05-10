using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Firebase.Auth;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace comprehensure.Models
{
    public static class GhostUserChecker
    {
        private const string ProjectId = "comprehensuredb-f9f7c";

        private static string FirestoreBase =>
            $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";

        private static readonly HttpClient _client = new();
        private static FirebaseAuthClient _authClient;
        private static string _apiKey;

        public static void Initialize(FirebaseAuthClient authClient, string apiKey)
        {
            _authClient = authClient;
            _apiKey = apiKey;
        }

        public static async Task CheckAndHandleGhostUserAsync()
        {
            string uid   = Preferences.Default.Get("SavedUserUid",   "");
            string email = Preferences.Default.Get("SavedUserEmail", "");

            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(email))
                return;

            if (_authClient?.User == null)
                return;

            string idToken = await GetIdTokenAsync();

            if (string.IsNullOrWhiteSpace(idToken))
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker] Could not get token — skipping.");
                return;
            }

            bool authUserExists = await VerifyUserExistsViaRestAsync(idToken);

            if (!authUserExists)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker] Firebase Auth: user {uid} not found — purging.");
                await PurgeAndRedirectAsync(uid, idToken);
                return;
            }

            bool docExists = await CheckUserDocumentExistsAsync(uid, idToken);

            if (!docExists)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker] Firestore: userdata/{uid} missing — purging.");
                await PurgeAndRedirectAsync(uid, idToken);
            }
        }

        private static async Task<string> GetIdTokenAsync()
        {
            try
            {
                return await _authClient.User.GetIdTokenAsync(forceRefresh: true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:Token] {ex.Message}");
                return null;
            }
        }

        private static async Task<bool> VerifyUserExistsViaRestAsync(string idToken)
        {
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={_apiKey}";
            try
            {
                var payload = JsonSerializer.Serialize(new { idToken });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(url, content);
                var json = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:AuthRest] {(int)response.StatusCode} {json}");

                if (!response.IsSuccessStatusCode)
                    return false;

                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("users", out var users))
                    return false;

                return users.GetArrayLength() > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:AuthRest] Exception: {ex.Message}");
                return true;
            }
        }

        private static async Task<bool> CheckUserDocumentExistsAsync(string uid, string idToken)
        {
            string url = $"{FirestoreBase}/userdata/{uid}";
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (!string.IsNullOrWhiteSpace(idToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[GhostUserChecker] userdata/{uid} returned {(int)response.StatusCode}.");
                    return false;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("fields", out var fields))
                    return false;

                if (!fields.TryGetProperty("Username", out var usernameProp))
                    return false;

                if (!usernameProp.TryGetProperty("stringValue", out var usernameVal))
                    return false;

                string username = usernameVal.GetString() ?? "";

                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker] Found Username='{username}' for uid={uid}.");

                return !string.IsNullOrWhiteSpace(username);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:DocCheck] Exception: {ex.Message}");
                return true;
            }
        }

        private static async Task PurgeAndRedirectAsync(string uid, string idToken)
        {
            await DeleteFirestoreDocumentAsync("StoryPage", uid, idToken);
            await DeleteFirestoreDocumentAsync("userdata",  uid, idToken);

            foreach (var key in new[]
            {
                "SavedUserUid", "SavedUserEmail", "SavedIdToken",
                "SavedRefreshToken", "CachedUsername", "IsFirstLogin"
            })
                Preferences.Default.Remove(key);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert(
                    "Account Not Found",
                    "Please sign up again.",
                    "OK"
                );
                await Shell.Current.GoToAsync("///MainPage");
            });
        }

        private static async Task DeleteFirestoreDocumentAsync(string collection, string uid, string idToken)
        {
            string url = $"{FirestoreBase}/{collection}/{uid}";
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                if (!string.IsNullOrWhiteSpace(idToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:Delete] {collection}/{uid} failed: {error}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:Delete] {collection}/{uid} OK.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:Delete] Exception: {ex.Message}");
            }
        }
    }
}
