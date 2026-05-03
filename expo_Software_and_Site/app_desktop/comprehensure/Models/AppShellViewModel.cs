using System.Windows.Input;

namespace comprehensure.Models
{
    public class AppShellViewModel
    {
        public ICommand LogOutCommand { get; }

        public AppShellViewModel()
        {
            LogOutCommand = new Command(async () =>
            {
                System.Diagnostics.Debug.WriteLine(">>> LogOutCommand FIRED");
                try
                {
                    Shell.Current.FlyoutIsPresented = false;
                    await Task.Delay(250);

                    System.Diagnostics.Debug.WriteLine(">>> Showing DisplayAlert");
                    bool confirmed = await Shell.Current.DisplayAlert(
                        "Log Out",
                        "Are you sure you want to log out?",
                        "Yes, log out",
                        "Cancel");

                    System.Diagnostics.Debug.WriteLine($">>> confirmed = {confirmed}");
                    if (confirmed)
                        await Shell.Current.GoToAsync("//MainPage");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($">>> EXCEPTION: {ex}");
                }
            });
        }
    }
}
