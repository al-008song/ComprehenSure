using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace comprehensure.Models
{
    public class QuizFunc
    {
        // ── Firestore REST config ─────────────────────────────────────────
        private const string ProjectId  = "comprehensuredb-f9f7c";
        private const string Collection = "StoryPage";
        private static string BaseUrl =>
            $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";

        private static readonly HttpClient _client = new HttpClient();

        // storypageprogN  — written when the reader reaches the last story page (80%)
        private static readonly Dictionary<int, string> ProgressFields = new()
        {
            { 1, "storypageprog1" }, { 2, "storypageprog2" }, { 3, "storypageprog3" },
            { 4, "storypageprog4" }, { 5, "storypageprog5" }, { 6, "storypageprog6" },
            { 7, "storypageprog7" }, { 8, "storypageprog8" },
        };

       
        private static readonly Dictionary<int, string> CalculatedProgFields = new()
        {
            { 1, "calculatedprog1" }, { 2, "calculatedprog2" }, { 3, "calculatedprog3" },
            { 4, "calculatedprog4" }, { 5, "calculatedprog5" }, { 6, "calculatedprog6" },
            { 7, "calculatedprog7" }, { 8, "calculatedprog8" },
        };

        // ── Module lock variables ─────────────────────────────────────────
        // false = unlocked, true = locked
        // module 1 starts as false (unlocked), all others start as true (locked)
        // These are ONLY written to Firestore if they don't already exist there
        private static bool islockedmodule1 = false;
        private static bool islockedmodule2 = true;
        private static bool islockedmodule3 = true;
        private static bool islockedmodule4 = true;
        private static bool islockedmodule5 = true;
        private static bool islockedmodule6 = true;
        private static bool islockedmodule7 = true;
        private static bool islockedmodule8 = true;

      
        private static bool isminigamelocked = true;


    
       
        public static async Task InitializeLockFieldsAsync()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrWhiteSpace(uid)) return;

            // Fetch fields that already exist in this user's document
            HashSet<string> existing = await GetExistingFieldNamesAsync(uid);

            // Map each field name to its default value
            var defaults = new Dictionary<string, bool>
            {
                { "islockedmodule1", islockedmodule1 },
                { "islockedmodule2", islockedmodule2 },
                { "islockedmodule3", islockedmodule3 },
                { "islockedmodule4", islockedmodule4 },
                { "islockedmodule5", islockedmodule5 },
                { "islockedmodule6", islockedmodule6 },
                { "islockedmodule7", islockedmodule7 },
                { "islockedmodule8", islockedmodule8 },
                { "isminigamelocked", isminigamelocked },
            };

            var fieldsToWrite = new Dictionary<string, object>();
            var fieldPaths    = new List<string> { "UID" };
            fieldsToWrite["UID"] = new { stringValue = uid };

            foreach (var kvp in defaults)
            {
                // Skip if the field already exists in Firestore
                if (existing.Contains(kvp.Key)) continue;

                fieldsToWrite[kvp.Key] = new { booleanValue = kvp.Value };
                fieldPaths.Add(kvp.Key);
            }

            // Nothing new to write
            if (fieldsToWrite.Count <= 1)
            {
                System.Diagnostics.Debug.WriteLine("[QuizFunc:InitializeLockFields] All lock fields already exist – skipping.");
                return;
            }

            var maskQuery = string.Join("&", fieldPaths.ConvertAll(f => $"updateMask.fieldPaths={f}"));
            string url    = $"{BaseUrl}/{Collection}/{uid}?{maskQuery}";

            await PatchAsync(url, new { fields = fieldsToWrite }, "InitializeLockFields");
        }


        // ── Called when reader reaches the last story page ─────────────────
        /// <summary>Saves storypageprogN = 80 and UID to Firestore.</summary>
        public static async Task SaveProgressAsync(int storyNumber, int progress)
        {
            if (!ProgressFields.TryGetValue(storyNumber, out string fieldName)) return;

            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrWhiteSpace(uid)) return;

            string url = $"{BaseUrl}/{Collection}/{uid}" +
                         $"?updateMask.fieldPaths={fieldName}" +
                         $"&updateMask.fieldPaths=UID";

            var data = new
            {
                fields = new Dictionary<string, object>
                {
                    { fieldName, new { integerValue = progress.ToString() } },
                    { "UID",     new { stringValue  = uid } }
                }
            };

            await PatchAsync(url, data, $"SaveProgressAsync story {storyNumber}");
        }

       
        public static async Task SaveQuizProgressAsync(int storyNumber, int quizScore, int storyProgress = 80)
        {
            if (!CalculatedProgFields.TryGetValue(storyNumber, out string fieldName)) return;

            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrWhiteSpace(uid)) return;

            int calculatedProg = storyProgress + (quizScore * 2);

            string url = $"{BaseUrl}/{Collection}/{uid}" +
                         $"?updateMask.fieldPaths={fieldName}" +
                         $"&updateMask.fieldPaths=UID";

            var data = new
            {
                fields = new Dictionary<string, object>
                {
                    { fieldName, new { integerValue = calculatedProg.ToString() } },
                    { "UID",     new { stringValue  = uid } }
                }
            };

            await PatchAsync(url, data, $"SaveQuizProgressAsync story {storyNumber} calc={calculatedProg}");

            // ── Unlock next module if score is high enough ────────────────
            // calculatedProg >= 88 means story (80) + at least 4/10 correct (8pts)
            if (calculatedProg >= 88 && storyNumber < 8)
                await UnlockNextModuleAsync(uid, storyNumber + 1);
        }

        // ── Unlocks a module by setting its lock field to false ────────────
        /// <summary>
        /// Sets islockedmodule{moduleNumber} = false in Firestore.
        /// Only called internally when the previous module's score qualifies.
        /// </summary>
        public static async Task UnlockNextModuleAsync(string uid, int moduleNumber)
        {
            string lockField = $"islockedmodule{moduleNumber}";

            bool unlockMinigame = moduleNumber == 4;

            string url = $"{BaseUrl}/{Collection}/{uid}" +
                         $"?updateMask.fieldPaths={lockField}" +
                         $"&updateMask.fieldPaths=UID" +
                         (unlockMinigame ? "&updateMask.fieldPaths=isminigamelocked" : "");

            var fields = new Dictionary<string, object>
            {
                { lockField, new { booleanValue = false } },
                { "UID",     new { stringValue  = uid  } }
            };

            if (unlockMinigame)
                fields["isminigamelocked"] = new { booleanValue = false };

            await PatchAsync(url, new { fields }, $"UnlockNextModule module{moduleNumber}");
        }

        // ── Internal helpers ───────────────────────────────────────────────

        private static async Task<HashSet<string>> GetExistingFieldNamesAsync(string uid)
        {
            var result = new HashSet<string>();
            try
            {
                var response = await _client.GetAsync($"{BaseUrl}/{Collection}/{uid}");
                if (!response.IsSuccessStatusCode) return result;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("fields", out JsonElement fields))
                    foreach (var prop in fields.EnumerateObject())
                        result.Add(prop.Name);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[QuizFunc:GetExistingFields] Exception: {ex.Message}");
            }
            return result;
        }

        private static async Task PatchAsync(string url, object data, string tag)
        {
            try
            {
                var options  = new JsonSerializerOptions { PropertyNamingPolicy = null };
                var json     = JsonSerializer.Serialize(data, options);
                var response = await _client.PatchAsync(
                    url,
                    new StringContent(json, Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[QuizFunc:{tag}] Failed: {error}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[QuizFunc:{tag}] OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[QuizFunc:{tag}] Exception: {ex.Message}");
            }
        }
    }
}
