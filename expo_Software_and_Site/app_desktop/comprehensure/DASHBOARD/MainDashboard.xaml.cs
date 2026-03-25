namespace comprehensure.DASHBOARD;

public partial class MainDashboard : ContentPage 
{
	public MainDashboard()
	{
		InitializeComponent();
	}

	private async void ProgressButton_Clicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new ProgressDashboard());
    }
	private async void ModulesButton_Clicked(object sender, EventArgs e)
	{
		
        await Navigation.PushAsync(new ModulesDaskboard());
    }

	private async void ProfileButton_Clicked(object sender, EventArgs e)
	{
		
        await Navigation.PushAsync(new ProfileDashboard());
    }
	private async void AboutUsButton_Clicked(object sender, EventArgs e)
	{
		
        await Navigation.PushAsync(new AllAboutUS_Dashboard());
    }

}