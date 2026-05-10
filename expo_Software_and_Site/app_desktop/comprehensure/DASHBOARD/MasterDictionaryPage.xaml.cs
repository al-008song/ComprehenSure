using Microsoft.Maui.Controls;

namespace comprehensure.DASHBOARD;

public partial class MasterDictionaryPage : ContentPage
{
    private Button? _activeButton;
    private CancellationTokenSource? _cts;

    // word (lowercase) → (card Border, panel name)
    private readonly Dictionary<string, (Border Card, string Panel)> _wordCards = new(StringComparer.OrdinalIgnoreCase);

    // tracks which panel is active while searching
    private string _activeTab = "Easy";

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

        // Register all word cards with their panel
        foreach (var (word, card) in EasyWords()) _wordCards[word] = (card, "Easy");
        foreach (var (word, card) in AvgWords())  _wordCards[word] = (card, "Average");
        foreach (var (word, card) in IntWords())  _wordCards[word] = (card, "Intermediate");
    }

    // ── Word card registrations ───────────────────────────────────────────────

    private IEnumerable<(string, Border)> EasyWords() => new[]
    {
        ("Carved",     CardCarved),    ("Confident",  CardConfident), ("Cozy",      CardCozy),
        ("Creaking",   CardCreaking),  ("Curious",    CardCurious),   ("Decoded",   CardDecoded),
        ("Determined", CardDetermined),("Dusty",      CardDusty),     ("Expedition",CardExpedition),
        ("Faded",      CardFaded),     ("Fading",     CardFading),    ("Fogged",    CardFogged),
        ("Frustrated", CardFrustrated),("Harmony",    CardHarmony),   ("Maze",      CardMaze),
        ("Narrow",     CardNarrow),    ("Observe",    CardObserve),   ("Observing", CardObserving),
        ("Organized",  CardOrganized), ("Passage",    CardPassage),   ("Patient",   CardPatient),
        ("Patterns",   CardPatterns),  ("Preserve",   CardPreserve),  ("Qualities", CardQualities),
        ("Shorthand",  CardShorthand), ("Steady",     CardSteady),    ("Tension",   CardTension),
        ("Unusual",    CardUnusual),
    };

    private IEnumerable<(string, Border)> AvgWords() => new[]
    {
        ("Alignment",      CardAlignment),     ("Anticipation",  CardAnticipation), ("Archive",         CardArchive),
        ("Calculation",    CardCalculation),   ("Calculations",  CardCalculations), ("Caretaker",       CardCaretaker),
        ("Chaotic",        CardChaotic),       ("Clarity",       CardClarity),      ("Comparison",      CardComparison),
        ("Consistent",     CardConsistent),    ("Constellations",CardConstellations),("Deliberate",     CardDeliberate),
        ("Disciplined",    CardDisciplined),   ("Extraordinary", CardExtraordinary),("Fascinated",      CardFascinated),
        ("Gesturing",      CardGesturing),     ("Gradually",     CardGradually),    ("Horizon",         CardHorizon),
        ("Interconnected", CardInterconnected),("Intervals",     CardIntervals),    ("Invisible",       CardInvisible),
        ("Observation",    CardObservation),   ("Parchment",     CardParchment),    ("Precise",         CardPrecise),
        ("Precisely",      CardPrecisely),     ("Preserved",     CardPreserved),    ("Responsibility",  CardResponsibility),
        ("Rhythm",         CardRhythm),        ("Transformation",CardTransformation),
    };

    private IEnumerable<(string, Border)> IntWords() => new[]
    {
        ("Arbiters",     CardArbiters),     ("Consequence",  CardConsequence),  ("Defied",       CardDefied),
        ("Delicate",     CardDelicate),     ("Dismantle",    CardDismantle),    ("Engraved",     CardEngraved),
        ("Fascination",  CardFascination),  ("Foresight",    CardForesight),    ("Fracture",     CardFracture),
        ("Investigations",CardInvestigations),("Ledger",     CardLedger),       ("Precision",    CardPrecision),
        ("Predictable",  CardPredictable),  ("Resolve",      CardResolve),      ("Surrendered",  CardSurrendered),
        ("Suspended",    CardSuspended),    ("Translucent",  CardTranslucent),  ("Unnaturally",  CardUnnaturally),
        ("Unsettled",    CardUnsettled),    ("Vaulted",      CardVaulted),      ("Vulnerability",CardVulnerability),
        ("Withheld",     CardWithheld),
    };

    // ── Search ────────────────────────────────────────────────────────────────

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim() ?? "";
        ClearSearchBtn.IsVisible = query.Length > 0;

        if (string.IsNullOrEmpty(query))
        {
            // Restore all cards visible, re-apply active tab visibility
            foreach (var entry in _wordCards.Values)
                entry.Card.IsVisible = true;

            NoResultsLabel.IsVisible = false;
            SwitchTab(_activeTab); // restore panel visibility
            return;
        }

        // While searching: show all three panels so results from any tab appear
        EasyPanel.IsVisible = true;
        AvgPanel.IsVisible  = true;
        IntPanel.IsVisible  = true;

        int visibleCount = 0;
        foreach (var kvp in _wordCards)
        {
            bool match = kvp.Key.Contains(query, StringComparison.OrdinalIgnoreCase);
            kvp.Value.Card.IsVisible = match;
            if (match) visibleCount++;
        }

        NoResultsLabel.IsVisible = visibleCount == 0;
    }

    private void OnClearSearchClicked(object sender, EventArgs e)
    {
        SearchEntry.Text = "";
        SearchEntry.Focus();
    }

    // ── Tabs ──────────────────────────────────────────────────────────────────

    private void OnEasyTabClicked(object sender, EventArgs e) => SwitchTab("Easy");
    private void OnAvgTabClicked(object sender, EventArgs e)  => SwitchTab("Average");
    private void OnIntTabClicked(object sender, EventArgs e)  => SwitchTab("Intermediate");

    private void SwitchTab(string tab)
    {
        _activeTab = tab;

        // If user is actively searching, don't change panel visibility
        if (!string.IsNullOrEmpty(SearchEntry?.Text))
            return;

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
