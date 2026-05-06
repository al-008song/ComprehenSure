namespace comprehensure.DASHBOARD;

public partial class HelpPage : ContentPage
{
    // Tracks which FAQ is currently open (null = all closed)
    private int? _openFaq = null;

    public HelpPage()
    {
        InitializeComponent();
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    // ── Generic toggle helper ────────────────────────────────────────────────

    private void ToggleFaq(
        int faqNumber,
        Label answer,
        BoxView divider,
        Label chevron)
    {
        bool isOpening = _openFaq != faqNumber;

        // Close whatever is currently open first
        if (_openFaq != null)
            CollapseAll();

        if (isOpening)
        {
            answer.IsVisible   = true;
            divider.IsVisible  = true;
            chevron.Rotation   = 270;   // point downward
            _openFaq           = faqNumber;
        }
        else
        {
            _openFaq = null;
        }
    }

    private void CollapseAll()
    {
        FAQ1Answer.IsVisible  = false;  FAQ1Divider.IsVisible  = false;  FAQ1Chevron.Rotation = 90;
        FAQ2Answer.IsVisible  = false;  FAQ2Divider.IsVisible  = false;  FAQ2Chevron.Rotation = 90;
        FAQ3Answer.IsVisible  = false;  FAQ3Divider.IsVisible  = false;  FAQ3Chevron.Rotation = 90;
        FAQ4Answer.IsVisible  = false;  FAQ4Divider.IsVisible  = false;  FAQ4Chevron.Rotation = 90;
        FAQ5Answer.IsVisible  = false;  FAQ5Divider.IsVisible  = false;  FAQ5Chevron.Rotation = 90;
    }

    // ── Per-question handlers ────────────────────────────────────────────────

    private void FAQ1_Tapped(object sender, TappedEventArgs e) =>
        ToggleFaq(1, FAQ1Answer, FAQ1Divider, FAQ1Chevron);

    private void FAQ2_Tapped(object sender, TappedEventArgs e) =>
        ToggleFaq(2, FAQ2Answer, FAQ2Divider, FAQ2Chevron);

    private void FAQ3_Tapped(object sender, TappedEventArgs e) =>
        ToggleFaq(3, FAQ3Answer, FAQ3Divider, FAQ3Chevron);

    private void FAQ4_Tapped(object sender, TappedEventArgs e) =>
        ToggleFaq(4, FAQ4Answer, FAQ4Divider, FAQ4Chevron);

    private void FAQ5_Tapped(object sender, TappedEventArgs e) =>
        ToggleFaq(5, FAQ5Answer, FAQ5Divider, FAQ5Chevron);
}
