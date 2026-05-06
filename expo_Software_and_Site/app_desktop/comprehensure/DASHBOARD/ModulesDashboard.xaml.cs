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
}
