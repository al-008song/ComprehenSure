using Microsoft.Maui.Controls;

namespace comprehensure.DASHBOARD;

public partial class MasterDictionaryPage : ContentPage
{
    private Button? _activeButton;
    private CancellationTokenSource? _cts;

    public MasterDictionaryPage()
    {
        InitializeComponent();
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetNavBarHasShadow(this, false);
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsVisible = false,
            IsEnabled = false
        });
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }


    private void OnEasyTabClicked(object sender, EventArgs e) => SwitchTab("Easy");
    private void OnAvgTabClicked(object sender, EventArgs e)  => SwitchTab("Average");
    private void OnIntTabClicked(object sender, EventArgs e)  => SwitchTab("Intermediate");

    private void SwitchTab(string tab)
    {
        EasyPanel.IsVisible = tab == "Easy";
        AvgPanel.IsVisible  = tab == "Average";
        IntPanel.IsVisible  = tab == "Intermediate";

        SetTabInactive(EasyTabBorder, EasyTab);
        SetTabInactive(AvgTabBorder,  AvgTab);
        SetTabInactive(IntTabBorder,  IntTab);

        switch (tab)
        {
            case "Easy":
                EasyTabBorder.BackgroundColor = Color.FromArgb("#1B4A1B");
                EasyTabBorder.Stroke          = new SolidColorBrush(Color.FromArgb("#2E7D32"));
                EasyTabBorder.StrokeThickness = 1.5;
                EasyTab.TextColor             = Color.FromArgb("#A5D6A7");
                break;
            case "Average":
                AvgTabBorder.BackgroundColor = Color.FromArgb("#3D3200");
                AvgTabBorder.Stroke          = new SolidColorBrush(Color.FromArgb("#F9A825"));
                AvgTabBorder.StrokeThickness = 1.5;
                AvgTab.TextColor             = Color.FromArgb("#FFE082");
                break;
            case "Intermediate":
                IntTabBorder.BackgroundColor = Color.FromArgb("#4A0F0F");
                IntTabBorder.Stroke          = new SolidColorBrush(Color.FromArgb("#C62828"));
                IntTabBorder.StrokeThickness = 1.5;
                IntTab.TextColor             = Color.FromArgb("#EF9A9A");
                break;
        }
    }

    private static void SetTabInactive(Border border, Button btn)
    {
        border.BackgroundColor = Color.FromArgb("#0F1A30");
        border.Stroke          = new SolidColorBrush(Color.FromArgb("#1E3A6B"));
        border.StrokeThickness = 1;
        btn.TextColor          = Color.FromArgb("#4A6A9A");
    }


    private async void OnSpeakClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        var word = btn.CommandParameter?.ToString();
        if (string.IsNullOrEmpty(word)) return;

        if (_activeButton == btn)
        {
            _cts?.Cancel();
            btn.Text = "🔊";
            _activeButton = null;
            return;
        }

        _cts?.Cancel();
        if (_activeButton is not null)
            _activeButton.Text = "🔊";

        _cts = new CancellationTokenSource();
        _activeButton = btn;
        btn.Text = "🔉";

        try
        {
            var settings = new SpeechOptions { Pitch = 1.0f, Volume = 1.0f };
            await TextToSpeech.Default.SpeakAsync(word, settings, _cts.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await DisplayAlert("Audio Error", $"Could not play audio: {ex.Message}", "OK");
        }
        finally
        {
            if (_activeButton == btn)
            {
                btn.Text = "🔊";
                _activeButton = null;
            }
        }
    }
}
