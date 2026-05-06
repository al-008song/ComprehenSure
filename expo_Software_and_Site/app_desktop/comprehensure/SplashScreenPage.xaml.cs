namespace comprehensure;

public partial class SplashScreenPage : ContentPage
{
    public SplashScreenPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(3000); // show splash for 3 seconds
        await Shell.Current.GoToAsync("//SignUpPage");
    }
}