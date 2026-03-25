using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;

namespace comprehensure.DataBaseControl.Models
{
    public partial class SignUpViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;

        public SignUpViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        [ObservableProperty]
        private string _Password;

        [ObservableProperty]
        private string _Email;

        [RelayCommand]
        private async Task SignUp()
        {
            string emailcl = _Email?.Trim();
            string passwordcl = _Password?.Trim();

            if (string.IsNullOrEmpty(emailcl))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter an email", "OK");
                return;
            }

            if (!emailcl.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.DisplayAlert(
                    "Invalid Email",
                    "Only Gmail accounts are allowed.",
                    "OK"
                );
                return;
            }

            try
            {
                await _authClient.CreateUserWithEmailAndPasswordAsync(emailcl, passwordcl);

                await Shell.Current.DisplayAlert("Sign Up", "Account Registered " + emailcl, "OK");
                await Shell.Current.GoToAsync("LoginPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Login Failed", ex.Message, "OK");
            }
        }
    }
}
