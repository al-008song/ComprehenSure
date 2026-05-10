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


        public async Task UserName_requiremen()
        {

        }


        [RelayCommand]
        public async Task LoginPage()
        {
            
            await Shell.Current.GoToAsync("LoginPage");
        }

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
                await Shell.Current.DisplayAlert("Invalid Email", "Only Gmail accounts are allowed.", "OK");
                return;
            }

            try
            {
                await Task.Delay(3500);
                var result = await _authClient.CreateUserWithEmailAndPasswordAsync(emailcl, passwordcl);
                await Shell.Current.GoToAsync($"///UsernameReq?email={emailcl}&uid={result.User.Uid}");
            }
            catch (Exception ex)
            {
                string raw = ex.Message;
                string readable = raw.Contains(":") ? raw.Split(':').Last().Trim() : raw;
                await Shell.Current.DisplayAlert("Sign Up Failed", readable, "OK");
            }

        }

    }

}
