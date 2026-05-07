namespace comprehensure.DASHBOARD;

using comprehensure.DataBaseControl.Models;

public partial class ProfileDashboard : ContentPage
{
    public ProfileDashboard(ProfileDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.OnSaveSuccess = ShowSaveSuccessPopup;
        viewModel.OnNoChange = ShowNoChangePopup;
    }

    // ── Save success popup — show with new username ───────────────
    private void ShowSaveSuccessPopup(string newUsername)
    {
        SaveSuccessMessageLabel.Text = $"Your username has been changed to \"{newUsername}\" successfully.";
        SaveSuccessPopupOverlay.IsVisible = true;
    }

    // ── No change popup — show when username is unchanged ────────
    private void ShowNoChangePopup()
    {
        NoChangePopupOverlay.IsVisible = true;
    }

    // ── No change: dismissed ──────────────────────────────────────
    private void OnNoChangeDismiss(object sender, EventArgs e)
    {
        NoChangePopupOverlay.IsVisible = false;
    }

    // ── Save success: dismissed ───────────────────────────────────
    private void OnSaveSuccessDismiss(object sender, EventArgs e)
    {
        SaveSuccessPopupOverlay.IsVisible = false;
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
