namespace comprehensure.DASHBOARD.StoryPage;

public partial class QuizPage4 : ContentPage
{
    private readonly QuizPage4ViewModel _viewModel;
    private string _selectedAnswer = "";

    private double _trackWidth = 0;

    public QuizPage4(QuizPage4ViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetNavBarHasShadow(this, false);
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsVisible = false,
            IsEnabled = false
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadQuestions();
        UpdateProgress();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        double contentWidth = Math.Min(width, 720) - 80;
        if (contentWidth > 0 && Math.Abs(contentWidth - _trackWidth) > 1)
        {
            _trackWidth = contentWidth;
            UpdateProgress();
        }
    }

    private void OnOptionClicked(object sender, TappedEventArgs e)
    {
        _selectedAnswer = e.Parameter?.ToString().ToUpper() ?? "";
        ResetOptionStyles();

        switch (_selectedAnswer)
        {
            case "A":
                ApplySelectedStyle(BorderA, CircleA);
                break;
            case "B":
                ApplySelectedStyle(BorderB, CircleB);
                break;
            case "C":
                ApplySelectedStyle(BorderC, CircleC);
                break;
            case "D":
                ApplySelectedStyle(BorderD, CircleD);
                break;
        }

        NextButton.IsEnabled = true;
    }

    private void ApplySelectedStyle(Border row, Border circle)
    {
        row.BackgroundColor = Color.FromArgb("#EBF4FF");
        row.Stroke = Color.FromArgb("#2E6BA8");
        circle.BackgroundColor = Color.FromArgb("#2E6BA8");
        circle.Stroke = Color.FromArgb("#2E6BA8");

        if (circle.Content is Label lbl)
            lbl.TextColor = Colors.White;
    }

    private void ResetOptionStyles()
    {
        foreach (var (row, circle) in new[]
        {
            (BorderA, CircleA),
            (BorderB, CircleB),
            (BorderC, CircleC),
            (BorderD, CircleD)
        })
        {
            row.BackgroundColor = Colors.White;
            row.Stroke = Color.FromArgb("#CBDCEB");
            circle.BackgroundColor = Colors.Transparent;
            circle.Stroke = Color.FromArgb("#CBDCEB");

            if (circle.Content is Label lbl)
                lbl.TextColor = Color.FromArgb("#2E6BA8");
        }
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        _viewModel.CheckAnswer(_selectedAnswer);

        _selectedAnswer = "";
        NextButton.IsEnabled = false;
        ResetOptionStyles();

        bool hasNext = _viewModel.NextQuestion();

        if (!hasNext)
        {
            await _viewModel.SaveQuizResults();
            ShowCompleteBanner();
        }
        else
        {
            UpdateProgress();
        }
    }

    private async void OnFinishClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/ModulesDashboard");
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
        PopupOverlay.IsVisible = true;
    }

    private async void OnPopupConfirm(object sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false;
        await Navigation.PopAsync();
    }

    private void OnPopupCancel(object sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false;
    }

    private void UpdateProgress()
    {
        if (_viewModel == null || _viewModel.TotalQuestions == 0) return;

        int current = _viewModel.CurrentQuestionIndex + 1;
        int total = _viewModel.TotalQuestions;

        double ratio = Math.Clamp((double)current / total, 0, 1);
        int percent = (int)Math.Round(ratio * 100);

        ProgressPercent.Text = $"{percent}%";
        ProgressFill.WidthRequest = _trackWidth > 0 ? _trackWidth * ratio : 0;
    }

    private void ShowCompleteBanner()
    {
        int score = _viewModel.Score;
        int total = _viewModel.TotalQuestions;
        double ratio = total > 0 ? (double)score / total : 0;

        QuestionCard.IsVisible = false;
        OptionsContainer.IsVisible = false;
        NextButtonRow.IsVisible = false;
        CompleteBanner.IsVisible = true;
        NextButton.IsEnabled = false;

        FinalScoreLabel.Text = $"{score} / {total}";
        ScoreMessageLabel.Text = ratio >= 0.8
            ? "Excellent work! You have a strong grasp of the story. 🎉"
            : ratio >= 0.5
                ? "Good effort! Review the story to strengthen your understanding."
                : "Keep practicing! Re-read the story and try again.";

        ProgressPercent.Text = "100%";
        ProgressFill.WidthRequest = _trackWidth;
    }
}
