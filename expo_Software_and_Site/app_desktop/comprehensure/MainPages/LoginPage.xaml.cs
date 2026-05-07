namespace comprehensure
{
    public partial class LoginPage : ContentPage
    {
        // Tracks whether the password is currently visible
        private bool _isPasswordVisible = false;

        public LoginPage(DataBaseControl.Models.LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetNavBarHasShadow(this, false);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsVisible = false,
                IsEnabled = false
            });
        }

        public class UserAccount // in simple terms this more of the name of the terms in the accounts.json
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Toggles the password field between masked and plain-text,
        /// and swaps the eye icon to match the current state.
        /// </summary>
        private void OnLoginTogglePasswordClicked(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            // Show or hide the password characters
            Password.IsPassword = !_isPasswordVisible;

            // Swap icon: eye_open when visible, eye_closed when hidden
            LoginTogglePasswordButton.Source = _isPasswordVisible
                ? "eye_open.png"
                : "eye_closed.png";
        }

        public async void BackButtonEvent(Object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetNavBarHasShadow(this, false);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsVisible = false,
                IsEnabled = false
            });
        }
    }
}
