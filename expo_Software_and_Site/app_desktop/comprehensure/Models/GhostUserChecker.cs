using System;
using System.Net.Http;
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

        public static void Initialize(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        public static async Task CheckAndHandleGhostUserAsync()
        {
            string uid   = Preferences.Default.Get("SavedUserUid",   "");
            string email = Preferences.Default.Get("SavedUserEmail", "");

            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(email))
                return;

            if (_authClient?.User == null)
            {
                System.Diagnostics.Debug.WriteLine("[GhostUserChecker] No active Firebase session — skipping.");
                return;
            }

            bool userExists = await CheckUserStillExistsAsync();

            System.Diagnostics.Debug.WriteLine($"[GhostUserChecker] userExists={userExists} for uid={uid}");

            if (!userExists)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker] Ghost detected: {email} — purging.");

                await DeleteFirestoreDocumentAsync("StoryPage", uid);
                await DeleteFirestoreDocumentAsync("userdata",  uid);

                Preferences.Default.Remove("SavedUserUid");
                Preferences.Default.Remove("SavedUserEmail");
                Preferences.Default.Remove("SavedIdToken");
                Preferences.Default.Remove("SavedRefreshToken");
                Preferences.Default.Remove("CachedUsername");
                Preferences.Default.Remove("IsFirstLogin");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert(
                        "NON EXISTING USER FOUND",
                        "Please Sign-Up again",
                        "OK"
                    );
                    await Shell.Current.GoToAsync("///MainPage");
                });
            }
        }

        private static async Task<bool> CheckUserStillExistsAsync()
        {
            try
            {
                string token = await _authClient.User.GetIdTokenAsync(forceRefresh: true);
                return !string.IsNullOrWhiteSpace(token);
            }
            catch (FirebaseAuthException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:Check] FirebaseAuthException: {ex.Reason} — {ex.Message}");

                bool isGhost = ex.Reason == AuthErrorReason.UserNotFound
                            || ex.Reason == AuthErrorReason.UserDisabled
                            || ex.Message.Contains("USER_NOT_FOUND", StringComparison.OrdinalIgnoreCase)
                            || ex.Message.Contains("TOKEN_REVOKED",  StringComparison.OrdinalIgnoreCase);

                return !isGhost;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GhostUserChecker:Check] Exception: {ex.Message}");
                return true;
            }
        }

        private static async Task DeleteFirestoreDocumentAsync(string collection, string uid)
        {
            string url = $"{FirestoreBase}/{collection}/{uid}";
            try
            {
                var response = await _client.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
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
