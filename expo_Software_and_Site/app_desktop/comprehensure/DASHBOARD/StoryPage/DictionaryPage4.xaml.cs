namespace comprehensure.DASHBOARD.StoryPage;

public partial class DictionaryPage4 : ContentPage
{
    private Button? _activeButton;
    private CancellationTokenSource? _cts;
    private Dictionary<string, Border> _wordCards = new();

    public DictionaryPage4()
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

        _wordCards = new Dictionary<string, Border>(StringComparer.OrdinalIgnoreCase)
        {
            { "algorithms", CardAlgorithms },
            { "calculation", CardCalculation },
            { "complex", CardComplex },
            { "confined", CardConfined },
            { "deliberate", CardDeliberate },
            { "eccentric", CardEccentric },
            { "extraordinary", CardExtraordinary },
            { "formula", CardFormula },
            { "gradually", CardGradually },
            { "intervals", CardIntervals },
            { "observation", CardObservation },
            { "patterns", CardPatterns },
            { "perception", CardPerception },
            { "rhythm", CardRhythm },
            { "sequence", CardSequence },
            { "structure", CardStructure },
            { "symmetry", CardSymmetry },
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
