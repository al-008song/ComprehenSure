namespace comprehensure.DataBaseControl.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Controls;

public class ProfileDashboardViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    
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

    // ── Save state ────────────────────────────────────────────────

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

    // ── Commands ──────────────────────────────────────────────────

    public ICommand logoutCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand EditProfileCommand { get; }
    public ICommand SaveProfileCommand { get; }
    public ICommand ChangePasswordCommand { get; }
    public ICommand HelpCommand { get; }

    public ProfileDashboardViewModel()
    {
        // ── Load profile from Preferences (written by MainDashboard) ──────────
        // MainDashboard.ApplyCachedProfile() sets "CachedUsername"
        // MainDashboard.RedirectIfNoUsername() reads/stores "SavedUserEmail"
        // Using the same keys means zero extra Firestore reads on this page.
        _usernameEdit = Preferences.Default.Get("CachedUsername", "Username");
        _userEmail    = Preferences.Default.Get("SavedUserEmail",  "user@email.com");

        // Sync dark-mode toggle with current app theme
        _isDarkModeEnabled = Application.Current?.RequestedTheme == AppTheme.Dark;

        logoutCommand       = new Command(async () => await ExecuteLogout());
        BackCommand         = new Command(async () => await Shell.Current.GoToAsync(".."));
        EditProfileCommand  = new Command(() => { /* open edit mode if needed */ });
        SaveProfileCommand  = new Command(async () => await ExecuteSaveProfile());
        ChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("/ChangePassword"));
        HelpCommand         = new Command(async () => await Shell.Current.GoToAsync("/HelpPage"));
    }

    // ── Save: mirrors MainDashboard.modulescoredb() pattern ───────────────────
    //  1. Validate input
    //  2. PATCH only the Username field in Firestore  userdata/{uid}
    //  3. Update "CachedUsername" in Preferences so MainDashboard's next call
    //     to ApplyCachedProfile() picks up the new value without an extra read
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

        IsSaving = true;
        try
        {
            // PATCH only the Username field — same approach as modulescoredb()
            string url = $"{BaseUrl}/userdata/{uid}?updateMask.fieldPaths=Username";

            var payload = new
            {
                fields = new
                {
                    Username = new { stringValue = newUsername }
                }
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
            var json    = JsonSerializer.Serialize(payload, options);

            var response = await _client.PatchAsync(
                url,
                new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[SaveProfile] Firestore error: {error}");
                await Shell.Current.DisplayAlert("Save Failed", "Could not update your username. Please try again.", "OK");
                return;
            }

            // Keep Preferences in sync so MainDashboard.ApplyCachedProfile()
            // reads the updated name the next time it runs — no Firestore read needed.
            Preferences.Default.Set("CachedUsername", newUsername);

            // Trim whitespace back into the binding
            UsernameEdit = newUsername;

            System.Diagnostics.Debug.WriteLine($"[SaveProfile] Username updated to '{newUsername}'");
            await Shell.Current.DisplayAlert("Saved", "Your username has been updated successfully.", "OK");
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

    // ── Called by code-behind after popup confirms ────────────────
    public async Task ExecuteLogout()
    {
        await Shell.Current.GoToAsync("///Login");
    }
}
