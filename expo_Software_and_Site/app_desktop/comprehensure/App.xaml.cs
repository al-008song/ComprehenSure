using Microsoft.Extensions.DependencyInjection;

namespace comprehensure
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected override async void OnStart()
        {
            base.OnStart();

            await Task.Delay(500);

            
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var current = e.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Connected ", "You are now online.", "OK");

            }
            else
            {
                await Shell.Current.DisplayAlert("Connection Lost", "You are now offline.", "OK");
            }



        }
    }
}
