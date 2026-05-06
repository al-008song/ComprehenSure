using comprehensure.DataBaseControl.Models;

namespace comprehensure.DASHBOARD;

public partial class MainDashboard : ContentPage
{
    private readonly MainDashboardViewModel _viewModel;

    public MainDashboard(MainDashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();
        // AutoRefresh removed — was firing 2 Firestore reads/sec.
        // Data is now loaded on demand with in-memory caching.
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Nothing to stop — AutoRefresh no longer exists.
    }

    private async void OnMasterDictionaryClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MasterDictionaryPage());
    }

    private async void OnSynonymHuntClicked(object sender, EventArgs e)
    {
        // Safety net: block navigation if mini-games are still locked
        if (_viewModel.IsMinigameLocked) return;
        await Navigation.PushAsync(new MiniGames.SynonymHuntPage());
    }

    private async void OnOneWordClicked(object sender, EventArgs e)
    {
        // Safety net: block navigation if mini-games are still locked
        if (_viewModel.IsMinigameLocked) return;
        await Navigation.PushAsync(new MiniGames.OneThemePage());
    }
}
