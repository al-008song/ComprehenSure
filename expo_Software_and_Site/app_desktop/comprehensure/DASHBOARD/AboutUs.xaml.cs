using comprehensure.DataBaseControl.Models;

namespace comprehensure.DASHBOARD
{
    public partial class AboutUs : ContentPage
    {
        public AboutUs()
        {
            InitializeComponent();
            BindingContext = new AboutUsViewModel();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
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