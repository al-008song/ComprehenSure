namespace comprehensure.DASHBOARD;

using comprehensure.DataBaseControl.Models;

public partial class ChangePassword : ContentPage
{
    public ChangePassword(ChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
