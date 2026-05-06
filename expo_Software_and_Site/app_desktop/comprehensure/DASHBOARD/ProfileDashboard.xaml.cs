namespace comprehensure.DASHBOARD;

using comprehensure.DataBaseControl.Models;

public partial class ProfileDashboard : ContentPage
{
    public ProfileDashboard(ProfileDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // ── Logout button — show popup ────────────────────────────────
    private void OnLogoutClicked(object sender, EventArgs e)
    {
        LogoutPopupOverlay.IsVisible = true;
    }

    // ── Popup: confirmed ──────────────────────────────────────────
    private async void OnLogoutConfirm(object sender, EventArgs e)
    {
        LogoutPopupOverlay.IsVisible = false;

        if (BindingContext is ProfileDashboardViewModel vm)
            await vm.ExecuteLogout();
    }

    // ── Popup: cancelled ─────────────────────────────────────────
    private void OnLogoutCancel(object sender, EventArgs e)
    {
        LogoutPopupOverlay.IsVisible = false;
    }
}
