using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;

namespace comprehensure.DataBaseControl.Models
{
    public partial class ChangePasswordViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;

        // ── Observable fields ─────────────────────────────────────────────────

        [ObservableProperty]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        // ── Constructor ───────────────────────────────────────────────────────

        public ChangePasswordViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        // ── Commands ──────────────────────────────────────────────────────────

        [RelayCommand]
        public async Task ChangePassword()
        {
            ClearError();

            // ── Validation ────────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                SetError("Please fill in all fields.");
                return;
            }

            if (NewPassword.Length < 6)
            {
                SetError("New password must be at least 6 characters.");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                SetError("New passwords do not match.");
                return;
            }

            if (NewPassword == CurrentPassword)
            {
                SetError("New password must be different from your current password.");
                return;
            }

            IsBusy = true;

            try
            {
                string email = Preferences.Default.Get("SavedUserEmail", "");

                if (string.IsNullOrWhiteSpace(email))
                {
                    SetError("Could not retrieve account email. Please log in again.");
                    return;
                }

                // ── Re-authenticate then change password ──────────────────────
                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, CurrentPassword);

                await userCredential.User.ChangePasswordAsync(NewPassword);

                await ToastShow("Password changed successfully!");

                // Clear fields after success
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                await Shell.Current.GoToAsync("..");
            }
            catch (FirebaseAuthException ex) when (ex.Reason == AuthErrorReason.WrongPassword)
            {
                SetError("Current password is incorrect.");
            }
            catch (FirebaseAuthException ex) when (ex.Reason == AuthErrorReason.WeakPassword)
            {
                SetError("New password is too weak. Use at least 6 characters.");
            }
            catch (FirebaseAuthException ex) when (ex.Reason == AuthErrorReason.TooManyAttemptsTryLater)
            {
                SetError("Too many attempts. Please try again later.");
            }
            catch (FirebaseAuthException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChangePassword] FirebaseAuthException: {ex.Message}");
                SetError("An error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChangePassword] Exception: {ex.Message}");
                SetError("An unexpected error occurred.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void SetError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        private void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }

        public async Task ToastShow(string showtext)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make(showtext, ToastDuration.Long, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
