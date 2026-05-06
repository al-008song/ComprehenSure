namespace comprehensure.DataBaseControl.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

public class ProfileDashboardViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    // ── Profile ──────────────────────────────────────────────────

    private string _usernameEdit = "Username";
    public string UsernameEdit
    {
        get => _usernameEdit;
        set { _usernameEdit = value; OnPropertyChanged(); }
    }

    private string _userEmail = "user@email.com";
    public string UserEmail
    {
        get => _userEmail;
        set { _userEmail = value; OnPropertyChanged(); }
    }

    // ── Dark Mode ─────────────────────────────────────────────────

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
        {
            Application.Current.UserAppTheme = isDark
                ? AppTheme.Dark
                : AppTheme.Light;
        }
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
        // Sync toggle with current app theme on open
        _isDarkModeEnabled = Application.Current?.RequestedTheme == AppTheme.Dark;

        logoutCommand = new Command(async () => await ExecuteLogout());

        BackCommand = new Command(async () =>
            await Shell.Current.GoToAsync(".."));

        EditProfileCommand = new Command(() =>
        {
            // Navigate to edit profile or open edit mode
        });

        SaveProfileCommand = new Command(async () =>
        {
            // Persist UsernameEdit and UserEmail to your database here
            await Shell.Current.DisplayAlert("Saved", "Profile updated successfully.", "OK");
        });

        ChangePasswordCommand = new Command(async () =>
            await Shell.Current.GoToAsync("/ChangePassword"));

        HelpCommand = new Command(async () =>
            await Shell.Current.GoToAsync("/HelpPage"));
    }

    // ── Called by code-behind after popup confirms ────────────────
    public async Task ExecuteLogout()
    {
        await Shell.Current.GoToAsync("///Login");
    }
}
