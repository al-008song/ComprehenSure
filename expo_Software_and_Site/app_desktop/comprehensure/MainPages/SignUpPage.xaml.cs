namespace comprehensure;

public partial class SignUpPage : ContentPage
{
    private bool _isPasswordVisible = false;

    public SignUpPage(DataBaseControl.Models.SignUpViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetNavBarHasShadow(this, false);
    }

    /// <summary>
    /// Toggles the password field between masked and plain-text,
    /// and swaps the eye icon to match the current state.
    /// </summary>
    private void OnSignUpTogglePasswordClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;

        PasswordNonConf.IsPassword = !_isPasswordVisible;

        SignUpTogglePasswordButton.Source = _isPasswordVisible
            ? "eye_open.png"
            : "eye_closed.png";
    }

    private async void SignUpButton_Clicked(object sender, EventArgs e)
    {
    }

    private async void OnLoginNavigationClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/LoginPage");
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
