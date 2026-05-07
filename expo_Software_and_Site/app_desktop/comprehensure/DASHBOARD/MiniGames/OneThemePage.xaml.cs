namespace comprehensure.DASHBOARD.MiniGames;

public partial class OneThemePage : ContentPage
{

    private class ThemeQuestion
    {
        public string ModuleName   { get; set; } = "";
        public string Answer       { get; set; } = "";
        public string ShuffledWord { get; set; } = "";
        public string Hint         { get; set; } = "";
        public string Img1         { get; set; } = "";
        public string Img2         { get; set; } = "";
        public string Img3         { get; set; } = "";
        public string Img4         { get; set; } = "";
    }

    private readonly List<ThemeQuestion> _allQuestions = new()
    {
        new ThemeQuestion
        {
            ModuleName   = "Module 1 · The Forgotten Library",
            Answer       = "CURIOSITY",
            ShuffledWord = "SIORUCITY",
            Hint         = "The invisible spark that leads a person to look behind a hidden shelf.",
            Img1 = "m1_curiosity_1.jpg", Img2 = "m1_curiosity_2.jpg",
            Img3 = "m1_curiosity_3.jpg", Img4 = "m1_curiosity_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 1 · The Forgotten Library",
            Answer       = "KNOWLEDGE",
            ShuffledWord = "WLEDGEKNO",
            Hint         = "Something precious and powerful that can be learned, shared, or hidden in quiet places.",
            Img1 = "m1_knowledge_1.jpg", Img2 = "m1_knowledge_2.jpg",
            Img3 = "m1_knowledge_3.jpg", Img4 = "m1_knowledge_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 1 · The Forgotten Library",
            Answer       = "HISTORY",
            ShuffledWord = "YRTOSIH",
            Hint         = "A collection of stories and choices from people who lived long before us.",
            Img1 = "m1_history_1.jpg", Img2 = "m1_history_2.jpg",
            Img3 = "m1_history_3.jpg", Img4 = "m1_history_4.jpg",
        },

        new ThemeQuestion
        {
            ModuleName   = "Module 2 · The Clockmaker's Secret",
            Answer       = "PATIENCE",
            ShuffledWord = "ECNEITAP",
            Hint         = "The quiet strength of slowing down to let a winding path reach its end.",
            Img1 = "m2_patience_1.jpg", Img2 = "m2_patience_2.jpg",
            Img3 = "m2_patience_3.jpg", Img4 = "m2_patience_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 2 · The Clockmaker's Secret",
            Answer       = "MESSAGE",
            ShuffledWord = "EGASSEM",
            Hint         = "A secret communication hidden within the shapes and patterns of a moving clock hand.",
            Img1 = "m2_message_1.jpg", Img2 = "m2_message_2.jpg",
            Img3 = "m2_message_3.jpg", Img4 = "m2_message_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 2 · The Clockmaker's Secret",
            Answer       = "WISDOM",
            ShuffledWord = "SDWMOI",
            Hint         = "A deep understanding of how to use your time and mind for things that truly matter.",
            Img1 = "m2_wisdom_1.jpg", Img2 = "m2_wisdom_2.jpg",
            Img3 = "m2_wisdom_3.jpg", Img4 = "m2_wisdom_4.jpg",
        },

        new ThemeQuestion
        {
            ModuleName   = "Module 3 · The Observatory on the Hill",
            Answer       = "VISION",
            ShuffledWord = "NOISIV",
            Hint         = "The bridge between what the eyes see and what the mind understands from a higher ground.",
            Img1 = "m3_vision_1.jpg", Img2 = "m3_vision_2.jpg",
            Img3 = "m3_vision_3.jpg", Img4 = "m3_vision_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 3 · The Observatory on the Hill",
            Answer       = "BALANCE",
            ShuffledWord = "ECNALAB",
            Hint         = "The silent weight of a thousand pieces fitting together so perfectly that nothing falls.",
            Img1 = "m3_balance_1.jpg", Img2 = "m3_balance_2.jpg",
            Img3 = "m3_balance_3.jpg", Img4 = "m3_balance_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 3 · The Observatory on the Hill",
            Answer       = "HIDDEN",
            ShuffledWord = "NEDDIH",
            Hint         = "A truth that stands in plain sight but wears a cloak of shadows until you change your height.",
            Img1 = "m3_hidden_1.jpg", Img2 = "m3_hidden_2.jpg",
            Img3 = "m3_hidden_3.jpg", Img4 = "m3_hidden_4.jpg",
        },

        new ThemeQuestion
        {
            ModuleName   = "Module 4 · The Garden of Hidden Patterns",
            Answer       = "INVESTIGATION",
            ShuffledWord = "GINIOTSAVETIN",
            Hint         = "A long walk through a hallway of 'whys' to find quiet details others miss.",
            Img1 = "m4_investigation_1.jpg", Img2 = "m4_investigation_2.jpg",
            Img3 = "m4_investigation_3.jpg", Img4 = "m4_investigation_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 4 · The Garden of Hidden Patterns",
            Answer       = "REVELATION",
            ShuffledWord = "AOITNRVAELE",
            Hint         = "The rewarding moment when the veil is finally pulled back and hidden truth reveals itself.",
            Img1 = "m4_revelation_1.jpg", Img2 = "m4_revelation_2.jpg",
            Img3 = "m4_revelation_3.jpg", Img4 = "m4_revelation_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 4 · The Garden of Hidden Patterns",
            Answer       = "HARMONY",
            ShuffledWord = "YNOMRAH",
            Hint         = "The delicate tension of a tightrope where opposite pulls create the surface to stand on.",
            Img1 = "m4_harmony_1.jpg", Img2 = "m4_harmony_2.jpg",
            Img3 = "m4_harmony_3.jpg", Img4 = "m4_harmony_4.jpg",
        },

        new ThemeQuestion
        {
            ModuleName   = "Module 5 · The Map of Quiet Decisions",
            Answer       = "ANTICIPATION",
            ShuffledWord = "NNTPIAIOCITA",
            Hint         = "The shadow of a mountain cast over a traveler long before they begin the climb.",
            Img1 = "m5_anticipation_1.jpg", Img2 = "m5_anticipation_2.jpg",
            Img3 = "m5_anticipation_3.jpg", Img4 = "m5_anticipation_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 5 · The Map of Quiet Decisions",
            Answer       = "INTENTION",
            ShuffledWord = "ENTTOININ",
            Hint         = "The silent spark inside a stone that decides exactly where the crack will form.",
            Img1 = "m5_intention_1.jpg", Img2 = "m5_intention_2.jpg",
            Img3 = "m5_intention_3.jpg", Img4 = "m5_intention_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 5 · The Map of Quiet Decisions",
            Answer       = "CONTINUITY",
            ShuffledWord = "NTICOUNYIT",
            Hint         = "The invisible river that flows through a thousand different forests but never loses its name.",
            Img1 = "m5_continuity_1.jpg", Img2 = "m5_continuity_2.jpg",
            Img3 = "m5_continuity_3.jpg", Img4 = "m5_continuity_4.jpg",
        },

        new ThemeQuestion
        {
            ModuleName   = "Module 6 · The Weight of Paper Wings",
            Answer       = "VULNERABILITY",
            ShuffledWord = "LIBERAVUNITYL",
            Hint         = "The thinness of a paper shield that only becomes unbreakable once you admit it can be torn.",
            Img1 = "m6_vulnerability_1.jpg", Img2 = "m6_vulnerability_2.jpg",
            Img3 = "m6_vulnerability_3.jpg", Img4 = "m6_vulnerability_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 6 · The Weight of Paper Wings",
            Answer       = "DISCIPLINE",
            ShuffledWord = "PDINELIISC",
            Hint         = "The invisible tether that keeps a wild kite steady in a storm, pulled tight by a focused mind.",
            Img1 = "m6_discipline_1.jpg", Img2 = "m6_discipline_2.jpg",
            Img3 = "m6_discipline_3.jpg", Img4 = "m6_discipline_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 6 · The Weight of Paper Wings",
            Answer       = "AWARENESS",
            ShuffledWord = "EESNRAWAS",
            Hint         = "The quiet observer who watches the shadow grow longer but does not run from the dark.",
            Img1 = "m6_awareness_1.jpg", Img2 = "m6_awareness_2.jpg",
            Img3 = "m6_awareness_3.jpg", Img4 = "m6_awareness_4.jpg",
        },

        new ThemeQuestion
        {
            ModuleName   = "Module 7 · The City of Silent Bargains",
            Answer       = "PRESERVATION",
            ShuffledWord = "ESEVARPTION",
            Hint         = "A glass case for time — stopping the clock so what is fragile today stays unbroken tomorrow.",
            Img1 = "m7_preservation_1.jpg", Img2 = "m7_preservation_2.jpg",
            Img3 = "m7_preservation_3.jpg", Img4 = "m7_preservation_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 7 · The City of Silent Bargains",
            Answer       = "RESONANCE",
            ShuffledWord = "SAEONNCER",
            Hint         = "A ghost in the wires — one strike on a string causes the whole room to hum with hidden energy.",
            Img1 = "m7_resonance_1.jpg", Img2 = "m7_resonance_2.jpg",
            Img3 = "m7_resonance_3.jpg", Img4 = "m7_resonance_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 7 · The City of Silent Bargains",
            Answer       = "RECIPROCATION",
            ShuffledWord = "ONCITACORPERI",
            Hint         = "The mirror that reaches out to take your shadow in return.",
            Img1 = "m7_reciprocation_1.jpg", Img2 = "m7_reciprocation_2.jpg",
            Img3 = "m7_reciprocation_3.jpg", Img4 = "m7_reciprocation_4.jpg",
        },

        new ThemeQuestion
        {
            ModuleName   = "Module 8 · The Whispers of the Iron Forest",
            Answer       = "INTROSPECTION",
            ShuffledWord = "NOISTCEPORTNI",
            Hint         = "A silent, internal search for a truth the mind has spent years trying to hide from itself.",
            Img1 = "m8_introspection_1.jpg", Img2 = "m8_introspection_2.jpg",
            Img3 = "m8_introspection_3.jpg", Img4 = "m8_introspection_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 8 · The Whispers of the Iron Forest",
            Answer       = "SURRENDER",
            ShuffledWord = "RDRSRDUEE",
            Hint         = "The ultimate defeat of the ego that paradoxically results in the only way to win the journey.",
            Img1 = "m8_surrender_1.jpg", Img2 = "m8_surrender_2.jpg",
            Img3 = "m8_surrender_3.jpg", Img4 = "m8_surrender_4.jpg",
        },
        new ThemeQuestion
        {
            ModuleName   = "Module 8 · The Whispers of the Iron Forest",
            Answer       = "STAGNATION",
            ShuffledWord = "GNAITATSNO",
            Hint         = "A state of being frozen in time because one is unwilling to pay the cost of moving forward.",
            Img1 = "m8_stagnation_1.jpg", Img2 = "m8_stagnation_2.jpg",
            Img3 = "m8_stagnation_3.jpg", Img4 = "m8_stagnation_4.jpg",
        },
    };


    private List<ThemeQuestion> _questions     = new();
    private int  _currentIndex  = 0;
    private int  _attemptsLeft  = 3;
    private int  _score         = 0;
    private int  _correctCount  = 0;
    private int  _skippedCount  = 0;
    private bool _roundFinished = false;

    private static readonly SolidColorBrush GreenStroke = new(Color.FromArgb("#B8E6D4"));
    private static readonly LinearGradientBrush GreenBackground = new()
    {
        StartPoint = new Point(0, 0),
        EndPoint   = new Point(1, 0),
        GradientStops = new GradientStopCollection
        {
            new GradientStop { Color = Color.FromArgb("#E6F5EF"), Offset = 0 },
            new GradientStop { Color = Color.FromArgb("#EAF7F2"), Offset = 1 },
        }
    };

    private static readonly SolidColorBrush RedStroke = new(Color.FromArgb("#FCA5A5"));
    private static readonly SolidColorBrush RedBackground = new(Color.FromArgb("#FEF2F2"));


    public OneThemePage()
    {
        InitializeComponent();
        StartNewGame();

        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetNavBarHasShadow(this, false);
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsVisible = false,
            IsEnabled = false
        });
    }

    private void StartNewGame()
    {
        var rng        = new Random();
        _questions     = _allQuestions.OrderBy(_ => rng.Next()).ToList();
        _currentIndex  = 0;
        _score         = 0;
        _correctCount  = 0;
        _skippedCount  = 0;
        _roundFinished = false;

        ResultsPanel.IsVisible = false;
        UpdateScoreLabel();
        LoadQuestion();
    }


    private void LoadQuestion()
    {
        if (_currentIndex >= _questions.Count)
        {
            ShowResults();
            return;
        }

        _attemptsLeft  = 3;
        _roundFinished = false;

        var q = _questions[_currentIndex];

        UpdateRoundPills();
        ModuleLabel.Text = q.ModuleName;

        SetImage(Img1, ImgLabel1, q.Img1);
        SetImage(Img2, ImgLabel2, q.Img2);
        SetImage(Img3, ImgLabel3, q.Img3);
        SetImage(Img4, ImgLabel4, q.Img4);

        Heart1.Text = "❤️";
        Heart2.Text = "❤️";
        Heart3.Text = "❤️";

        HintBorder.IsVisible  = false;
        HintContentLabel.Text = "";
        HintDescLabel.Text    = "";

        AnswerEntry.Text      = "";
        AnswerEntry.IsEnabled = true;

        FeedbackBorder.IsVisible   = false;
        FeedbackBorder.Stroke      = GreenStroke;
        FeedbackBorder.Background  = GreenBackground;
        FeedbackIcon.Text          = "✓";
        FeedbackLabel.Text         = "Correct!";
        PointsLabel.Text           = "+ 5 points";

        SubmitButton.IsVisible     = true;
        SkipButtonBorder.IsVisible = true;
    }

    private static void SetImage(Image img, Label label, string source)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!string.IsNullOrEmpty(source))
            {
                img.Source      = ImageSource.FromFile(source);
                img.IsVisible   = true;
                img.Opacity     = 1;
                label.IsVisible = false;

                if (img.Parent is Grid grid && grid.Parent is Border border)
                    border.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                img.Source      = null;
                img.IsVisible   = false;
                img.Opacity     = 0;
                label.IsVisible = true;

                if (img.Parent is Grid grid && grid.Parent is Border border)
                    border.Background = new SolidColorBrush(Color.FromArgb("#EFF4FB"));
            }
        });
    }


    private void OnSubmitClicked(object sender, EventArgs e)
    {
        if (_roundFinished) return;

        var q       = _questions[_currentIndex];
        var answer  = (AnswerEntry.Text ?? "").Trim().ToUpperInvariant();
        var correct = q.Answer.ToUpperInvariant();

        if (answer == correct)
        {
            _score        += 5;
            _correctCount++;
            _roundFinished = true;
            UpdateScoreLabel();

            FeedbackBorder.Stroke      = GreenStroke;
            FeedbackBorder.Background  = GreenBackground;
            FeedbackIcon.Text          = "✓";
            FeedbackLabel.Text         = "Correct!";
            PointsLabel.Text           = "+ 5 points";
            FeedbackBorder.IsVisible   = true;

            SubmitButton.IsVisible     = false;
            SkipButtonBorder.IsVisible = false;
            AnswerEntry.IsEnabled      = false;
        }
        else
        {
            _attemptsLeft--;
            LoseHeart();
            AnswerEntry.Text = "";

            if (_attemptsLeft == 2)
            {
                ShowBlankHint(q);
            }
            else if (_attemptsLeft == 1)
            {
                ShowShuffledHint(q);
            }
            else
            {
                _roundFinished = true;
                ShowAnswer(q);

                FeedbackBorder.Stroke      = RedStroke;
                FeedbackBorder.Background  = RedBackground;
                FeedbackIcon.Text          = "✕";
                FeedbackLabel.Text         = $"The answer was: {q.Answer}";
                PointsLabel.Text           = "No points this round";
                FeedbackBorder.IsVisible   = true;

                SubmitButton.IsVisible     = false;
                SkipButtonBorder.IsVisible = false;
                AnswerEntry.IsEnabled      = false;
            }
        }
    }


    private void ShowBlankHint(ThemeQuestion q)
    {
        HintBorder.IsVisible  = true;
        HintTitleLabel.Text   = "💡  HINT — How many letters?";
        HintContentLabel.Text = new string('_', q.Answer.Length);
        HintDescLabel.Text    = q.Hint;
    }

    private void ShowShuffledHint(ThemeQuestion q)
    {
        HintBorder.IsVisible  = true;
        HintTitleLabel.Text   = "🔀  HINT — Unscramble!";
        HintContentLabel.Text = q.ShuffledWord;
        HintDescLabel.Text    = q.Hint;
    }

    private void ShowAnswer(ThemeQuestion q)
    {
        HintBorder.IsVisible  = true;
        HintTitleLabel.Text   = "✔  ANSWER";
        HintContentLabel.Text = q.Answer;
        HintDescLabel.Text    = q.Hint;
    }


    private void LoseHeart()
    {
        var lost = 3 - _attemptsLeft;
        if (lost >= 1) Heart1.Text = "🖤";
        if (lost >= 2) Heart2.Text = "🖤";
        if (lost >= 3) Heart3.Text = "🖤";
    }


    private void UpdateRoundPills()
    {
        int display = (_currentIndex % 3) + 1;

        void SetPill(Border pill, Label lbl, int num)
        {
            bool active = num == display;
            pill.BackgroundColor = active ? Color.FromArgb("#0F2D4A") : Colors.White;
            pill.Stroke = active
                ? new SolidColorBrush(Colors.Transparent)
                : new SolidColorBrush(Color.FromArgb("#CBDCEB"));
            lbl.TextColor = active ? Colors.White : Color.FromArgb("#6B8CAE");
        }

        var p1l = (Label)Round1Pill.Content;
        var p2l = (Label)Round2Pill.Content;
        var p3l = (Label)Round3Pill.Content;

        SetPill(Round1Pill, p1l, 1);
        SetPill(Round2Pill, p2l, 2);
        SetPill(Round3Pill, p3l, 3);
    }

    private void UpdateScoreLabel()
    {
        ScoreLabel.Text = $"{_score}";
    }


    private void OnNextClicked(object sender, EventArgs e)
    {
        _currentIndex++;
        LoadQuestion();
    }

    private void OnSkipClicked(object sender, EventArgs e)
    {
        _skippedCount++;
        _currentIndex++;
        LoadQuestion();
    }


    private void ShowResults()
    {
        ResultsPanel.IsVisible = true;

        int total    = _questions.Count;
        int accuracy = total > 0 ? (int)Math.Round(_correctCount * 100.0 / total) : 0;

        ResultsTrophyLabel.Text = _correctCount >= total * 0.8 ? "🏆" :
                                  _correctCount >= total * 0.5 ? "🥈" : "🎯";
        ResultsTitleLabel.Text  = _correctCount >= total * 0.8 ? "Outstanding!" :
                                  _correctCount >= total * 0.5 ? "Well Done!"   : "Keep Practicing!";

        ResultsScoreLabel.Text = $"{_score}";
        StatCorrectLabel.Text  = $"{_correctCount}";
        StatSkippedLabel.Text  = $"{_skippedCount}";
        StatAccuracyLabel.Text = $"{accuracy}%";
    }

    private void OnPlayAgainClicked(object sender, EventArgs e)
    {
        StartNewGame();
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
