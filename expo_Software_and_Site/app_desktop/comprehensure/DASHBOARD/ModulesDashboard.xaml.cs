using System;
using Microsoft.Maui.Controls;

namespace comprehensure.DASHBOARD;

public partial class ModulesDashboard : ContentPage
{
    public ModulesDashboard(DataBaseControl.Models.ModulesDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Reload lock state + progress every time the page appears
    // (e.g. returning from a story/quiz will reflect updated data)
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DataBaseControl.Models.ModulesDashboardViewModel vm)
            await vm.LoadModuleDataAsync();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
        await Shell.Current.GoToAsync("///MainDashboard");
    }

    // ─── Pointer / hover handlers (matches MainDashboard smooth animations) ──────

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            view.ScaleTo(1.015, 350, Easing.CubicOut);
            view.FadeTo(0.96, 300, Easing.CubicOut);
        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            view.ScaleTo(1.0, 400, Easing.CubicOut);
            view.FadeTo(1.0, 350, Easing.CubicOut);
        }
    }

    private void OnPointerPressed(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            view.ScaleTo(0.965, 200, Easing.CubicIn);
            view.FadeTo(0.88, 180, Easing.CubicIn);
        }
    }

    private void OnPointerReleased(object sender, PointerEventArgs e)
    {
        if (sender is View view)
        {
            view.ScaleTo(1.01, 280, Easing.SpringOut);
            view.FadeTo(1.0, 260, Easing.CubicOut);

            Task.Delay(290).ContinueWith(_ =>
                MainThread.BeginInvokeOnMainThread(() =>
                    view.ScaleTo(1.0, 300, Easing.CubicOut)));
        }
    }
}
