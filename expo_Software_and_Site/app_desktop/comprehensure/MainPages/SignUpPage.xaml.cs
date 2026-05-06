namespace comprehensure;

public partial class SignUpPage : ContentPage
{
    // private SwitchOffline _checker;
    public SignUpPage(DataBaseControl.Models.SignUpViewModel viewModel)
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

        
    

    private async void SignUpButton_Clicked(object sender, EventArgs e)
    {
        // await DisplayAlert("Sign Up", "Sign up button clicked!", "OK");
        // SwitchOffline.Checker();


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
