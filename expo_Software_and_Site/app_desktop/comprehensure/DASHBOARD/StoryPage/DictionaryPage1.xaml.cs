namespace comprehensure.DASHBOARD.StoryPage;

public partial class DictionaryPage1 : ContentPage
{
    private Button? _activeButton;
    private CancellationTokenSource? _cts;

    // Map each word (lowercase) to its card Border
    private Dictionary<string, Border> _wordCards = new();

    public DictionaryPage1()
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

        // Build lookup after InitializeComponent so named elements are available
        _wordCards = new Dictionary<string, Border>(StringComparer.OrdinalIgnoreCase)
        {
            { "fading",      CardFading },
            { "creaking",    CardCreaking },
            { "dusty",       CardDusty },
            { "curious",     CardCurious },
            { "narrow",      CardNarrow },
            { "unusual",     CardUnusual },
            { "decoded",     CardDecoded },
            { "passage",     CardPassage },
            { "shorthand",   CardShorthand },
            { "frustrated",  CardFrustrated },
            { "observing",   CardObserving },
            { "expedition",  CardExpedition },
            { "organized",   CardOrganized },
            { "preserve",    CardPreserve },
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    // ── Search ────────────────────────────────────────────────────────────────

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim() ?? "";

        ClearSearchBtn.IsVisible = query.Length > 0;

        if (string.IsNullOrEmpty(query))
        {
            // Show all cards
            foreach (var card in _wordCards.Values)
                card.IsVisible = true;

            NoResultsLabel.IsVisible = false;
            return;
        }

        int visibleCount = 0;
        foreach (var kvp in _wordCards)
        {
            bool match = kvp.Key.Contains(query, StringComparison.OrdinalIgnoreCase);
            kvp.Value.IsVisible = match;
            if (match) visibleCount++;
        }

        NoResultsLabel.IsVisible = visibleCount == 0;
    }

    private void OnClearSearchClicked(object sender, EventArgs e)
    {
        SearchEntry.Text = "";
        SearchEntry.Focus();
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // ── Text-to-Speech ────────────────────────────────────────────────────────

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
            var settings = new SpeechOptions
            {
                Pitch  = 1.0f,
                Volume = 1.0f,
            };

            await TextToSpeech.Default.SpeakAsync(word, settings, _cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
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
