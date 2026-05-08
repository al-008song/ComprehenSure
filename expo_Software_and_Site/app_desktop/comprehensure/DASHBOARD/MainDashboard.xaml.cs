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

    private static bool IsEffectivelyEnabled(View view)
    {
        Element current = view;
        while (current != null)
        {
            if (current is VisualElement ve && !ve.IsEnabled)
                return false;
            current = current.Parent;
        }
        return true;
    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            if (!IsEffectivelyEnabled(view)) return;
            view.ScaleTo(view.StyleId == "dark" ? 1.02 : 1.015, 350, Easing.CubicOut);
            view.FadeTo(0.96, 300, Easing.CubicOut);
        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            if (!IsEffectivelyEnabled(view)) return;
            view.ScaleTo(1.0, 400, Easing.CubicOut);
            view.FadeTo(1.0, 350, Easing.CubicOut);
        }
    }

    private void OnPointerPressed(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            if (!IsEffectivelyEnabled(view)) return;
            view.ScaleTo(0.965, 200, Easing.CubicIn);
            view.FadeTo(0.88, 180, Easing.CubicIn);
        }
    }

    private void OnPointerReleased(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            if (!IsEffectivelyEnabled(view)) return;
            view.ScaleTo(1.01, 280, Easing.SpringOut);
            view.FadeTo(1.0, 260, Easing.CubicOut);

            Task.Delay(290).ContinueWith(_ =>
                MainThread.BeginInvokeOnMainThread(() =>
                    view.ScaleTo(1.0, 300, Easing.CubicOut)));
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private async void OnMasterDictionaryClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MasterDictionaryPage());
    }

    private async void OnSynonymHuntClicked(object sender, EventArgs e)
    {
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
    