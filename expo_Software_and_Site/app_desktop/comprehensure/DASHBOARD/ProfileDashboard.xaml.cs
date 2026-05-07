namespace comprehensure.DASHBOARD;

using comprehensure.DataBaseControl.Models;

public partial class ProfileDashboard : ContentPage
{
    public ProfileDashboard(ProfileDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        LogoutPopupOverlay.IsVisible = true;
    }

    private async void OnLogoutConfirm(object sender, EventArgs e)
    {
        LogoutPopupOverlay.IsVisible = false;

        if (BindingContext is ProfileDashboardViewModel vm)
            await vm.ExecuteLogout();
    }

    private void OnLogoutCancel(object sender, EventArgs e)
    {
        LogoutPopupOverlay.IsVisible = false;
    }
}
