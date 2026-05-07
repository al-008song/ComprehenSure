using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace comprehensure.DataBaseControl.Models
{
    public partial class ModulesDashboardViewModel : ObservableObject
    {
        private const string ProjectId  = "comprehensuredb-f9f7c";
        private const string Collection = "StoryPage";
        private static string BaseUrl =>
            $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";
        private static readonly HttpClient _http = new HttpClient();

        [ObservableProperty] private bool isModule1Locked = false;
        [ObservableProperty] private bool isModule2Locked = true;
        [ObservableProperty] private bool isModule3Locked = true;
        [ObservableProperty] private bool isModule4Locked = true;
        [ObservableProperty] private bool isModule5Locked = true;
        [ObservableProperty] private bool isModule6Locked = true;
        [ObservableProperty] private bool isModule7Locked = true;
        [ObservableProperty] private bool isModule8Locked = true;

        [ObservableProperty] private double module1Progress;
        [ObservableProperty] private double module2Progress;
        [ObservableProperty] private double module3Progress;
        [ObservableProperty] private double module4Progress;
        [ObservableProperty] private double module5Progress;
        [ObservableProperty] private double module6Progress;
        [ObservableProperty] private double module7Progress;
        [ObservableProperty] private double module8Progress;

        [ObservableProperty] private string module1ProgressText = "0%";
        [ObservableProperty] private string module2ProgressText = "0%";
        [ObservableProperty] private string module3ProgressText = "0%";
        [ObservableProperty] private string module4ProgressText = "0%";
        [ObservableProperty] private string module5ProgressText = "0%";
        [ObservableProperty] private string module6ProgressText = "0%";
        [ObservableProperty] private string module7ProgressText = "0%";
        [ObservableProperty] private string module8ProgressText = "0%";

        [RelayCommand] public async Task startmodule1() => await Shell.Current.GoToAsync("StoryPage1");
        [RelayCommand] public async Task startmodule2() => await Shell.Current.GoToAsync("StoryPage2");
        [RelayCommand] public async Task startmodule3() => await Shell.Current.GoToAsync("StoryPage3");
        [RelayCommand] public async Task startmodule4() => await Shell.Current.GoToAsync("StoryPage4");
        [RelayCommand] public async Task startmodule5() => await Shell.Current.GoToAsync("StoryPage5");
        [RelayCommand] public async Task startmodule6() => await Shell.Current.GoToAsync("StoryPage6");
        [RelayCommand] public async Task startmodule7() => await Shell.Current.GoToAsync("StoryPage7");
        [RelayCommand] public async Task startmodule8() => await Shell.Current.GoToAsync("StoryPage8");

        /// <summary>
        /// Reads islockedmoduleN and calculatedprogN for all 8 modules
        /// from the user's Firestore document and updates bound properties.
        /// Call this from OnAppearing in the code-behind.
        /// </summary>
        public async Task LoadModuleDataAsync()
        {
            string uid = Preferences.Default.Get("SavedUserUid", "");
            if (string.IsNullOrWhiteSpace(uid)) return;

            try
            {
                int[] prog = new int[9]; // index 1–8
                bool anyUnlocked = false;

                var response = await _http.GetAsync($"{BaseUrl}/{Collection}/{uid}");
                if (!response.IsSuccessStatusCode) return;

                string json = await response.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(json))
                {
                    if (!doc.RootElement.TryGetProperty("fields", out JsonElement fields)) return;

                    for (int i = 1; i <= 8; i++)
                        prog[i] = ReadInt(fields, $"calculatedprog{i}", 0);
                }

                for (int i = 1; i <= 7; i++)
                {
                    if (prog[i] >= 88)
                    {
                        await comprehensure.Models.QuizFunc.UnlockNextModuleAsync(uid, i + 1);
                        anyUnlocked = true;
                    }
                }

                string jsonForLocks = anyUnlocked
                    ? await (await _http.GetAsync($"{BaseUrl}/{Collection}/{uid}")).Content.ReadAsStringAsync()
                    : json;

                using (var doc2 = JsonDocument.Parse(jsonForLocks))
                {
                    if (!doc2.RootElement.TryGetProperty("fields", out JsonElement fields2)) return;

                    IsModule1Locked = ReadBool(fields2, "islockedmodule1", false);
                    IsModule2Locked = ReadBool(fields2, "islockedmodule2", true);
                    IsModule3Locked = ReadBool(fields2, "islockedmodule3", true);
                    IsModule4Locked = ReadBool(fields2, "islockedmodule4", true);
                    IsModule5Locked = ReadBool(fields2, "islockedmodule5", true);
                    IsModule6Locked = ReadBool(fields2, "islockedmodule6", true);
                    IsModule7Locked = ReadBool(fields2, "islockedmodule7", true);
                    IsModule8Locked = ReadBool(fields2, "islockedmodule8", true);
                }

                for (int i = 1; i <= 8; i++)
                    SetProgress(i, prog[i]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ModulesDashboardViewModel:Load] {ex.Message}");
            }
        }


        private void SetProgress(int moduleNumber, int rawValue)
        {
            int clamped  = Math.Max(0, Math.Min(100, rawValue));
            double frac  = clamped / 100.0;
            string label = $"{clamped}%";

            switch (moduleNumber)
            {
                case 1: Module1Progress = frac; Module1ProgressText = label; break;
                case 2: Module2Progress = frac; Module2ProgressText = label; break;
                case 3: Module3Progress = frac; Module3ProgressText = label; break;
                case 4: Module4Progress = frac; Module4ProgressText = label; break;
                case 5: Module5Progress = frac; Module5ProgressText = label; break;
                case 6: Module6Progress = frac; Module6ProgressText = label; break;
                case 7: Module7Progress = frac; Module7ProgressText = label; break;
                case 8: Module8Progress = frac; Module8ProgressText = label; break;
            }
        }

        private static bool ReadBool(JsonElement fields, string name, bool fallback)
        {
            if (fields.TryGetProperty(name, out JsonElement el) &&
                el.TryGetProperty("booleanValue", out JsonElement bv))
                return bv.GetBoolean();
            return fallback;
        }

        private static int ReadInt(JsonElement fields, string name, int fallback)
        {
            if (!fields.TryGetProperty(name, out JsonElement el)) return fallback;

            if (el.TryGetProperty("integerValue", out JsonElement iv))
            {
                if (iv.ValueKind == JsonValueKind.String &&
                    int.TryParse(iv.GetString(), out int parsed))
                    return parsed;
                if (iv.ValueKind == JsonValueKind.Number &&
                    iv.TryGetInt32(out int n))
                    return n;
            }
            return fallback;
        }
    }
}
