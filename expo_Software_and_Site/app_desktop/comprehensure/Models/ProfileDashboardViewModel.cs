namespace comprehensure.DataBaseControl.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Controls;

public class ProfileDashboardViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

   
    public Action<string>? OnSaveSuccess { get; set; }

   
    public Action? OnNoChange { get; set; }

    private readonly string _projectId = "comprehensuredb-f9f7c";
    private string BaseUrl =>
        $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents";
    private readonly HttpClient _client = new HttpClient();

    private string _usernameEdit = string.Empty;
    public string UsernameEdit
    {
        get => _usernameEdit;
        set { _usernameEdit = value; OnPropertyChanged(); }
    }

    private string _userEmail = string.Empty;
    public string UserEmail
    {
        get => _userEmail;
        set { _userEmail = value; OnPropertyChanged(); }
    }

    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        set { _isSaving = value; OnPropertyChanged(); }
    }

    private bool _isDarkModeEnabled;
    public bool IsDarkModeEnabled
    {
        get => _isDarkModeEnabled;
        set
        {
            if (_isDarkModeEnabled == value) return;
            _isDarkModeEnabled = value;
            OnPropertyChanged();
            ApplyTheme(value);
        }
    }

    private static void ApplyTheme(bool isDark)
    {
        if (Application.Current is not null)
            Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }

    public ICommand logoutCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand EditProfileCommand { get; }
    public ICommand SaveProfileCommand { get; }
    public ICommand ChangePasswordCommand { get; }
    public ICommand HelpCommand { get; }

    public ProfileDashboardViewModel()
    {
        _usernameEdit = Preferences.Default.Get("CachedUsername", "Username");
        _userEmail    = Preferences.Default.Get("SavedUserEmail",  "user@email.com");

        _isDarkModeEnabled = Application.Current?.RequestedTheme == AppTheme.Dark;

        logoutCommand         = new Command(async () => await ExecuteLogout());
        BackCommand           = new Command(async () => await Shell.Current.GoToAsync(".."));
        EditProfileCommand    = new Command(() => { });
        SaveProfileCommand    = new Command(async () => await ExecuteSaveProfile());
        ChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("/ChangePassword"));
        HelpCommand           = new Command(async () => await Shell.Current.GoToAsync("/HelpPage"));
    }

    private async Task<bool> UsernameExists(string username)
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
                        field = new { fieldPath = "Username" },
                        op    = "EQUAL",
                        value = new { stringValue = username },
                    },
                },
                limit = 1,
            },
        });

        HttpResponseMessage response = await _client.PostAsync(
            url,
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[UsernameExists] Firestore error: {error}");
            await Shell.Current.DisplayAlert($"Error {(int)response.StatusCode}", "Could not verify username availability. Please try again.", "OK");
            return true;
        }

        string result = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(result))
            return false;

        using var doc = JsonDocument.Parse(result);
        var root      = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in root.EnumerateArray())
            {
                if (element.TryGetProperty("document", out var documentProp) &&
                    documentProp.TryGetProperty("fields", out _))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private async Task ExecuteSaveProfile()
    {
        string newUsername = UsernameEdit?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(newUsername))
        {
            await Shell.Current.DisplayAlert("Invalid Username", "Username cannot be empty.", "OK");
            return;
        }

        string uid = Preferences.Default.Get("SavedUserUid", "");
        if (string.IsNullOrEmpty(uid))
        {
            await Shell.Current.DisplayAlert("Error", "Could not identify your account. Please log in again.", "OK");
            return;
        }

        string currentUsername = Preferences.Default.Get("CachedUsername", string.Empty);
        if (string.Equals(newUsername, currentUsername, StringComparison.OrdinalIgnoreCase))
        {
            OnNoChange?.Invoke();
            return;
        }

        IsSaving = true;
        try
        {
            bool taken = await UsernameExists(newUsername.ToLower());
            if (taken)
            {
                await Shell.Current.DisplayAlert(
                    "Username Taken",
                    $"\"{newUsername}\" is already in use. Please choose a different username.",
                    "OK"
                );
                return;
            }

            string url = $"{BaseUrl}/userdata/{uid}?updateMask.fieldPaths=Username";

            var payload = new
            {
                fields = new
                {
                    Username = new { stringValue = newUsername }
                }
            };

            var options  = new JsonSerializerOptions { PropertyNamingPolicy = null };
            var json     = JsonSerializer.Serialize(payload, options);

            var response = await _client.PatchAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[SaveProfile] Firestore error: {error}");
                await Shell.Current.DisplayAlert("Save Failed", "Could not update your username. Please try again.", "OK");
                return;
            }

            Preferences.Default.Set("CachedUsername", newUsername);
            UsernameEdit = newUsername;

            System.Diagnostics.Debug.WriteLine($"[SaveProfile] Username updated to '{newUsername}'");

            // Fire the success popup in the View instead of a plain alert
            OnSaveSuccess?.Invoke(newUsername);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SaveProfile] Exception: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }

    public async Task ExecuteLogout()
    {
        await Shell.Current.GoToAsync("///MainPage");
            Preferences.Default.Clear();
    }
}
