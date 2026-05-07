using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage7 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;

        private readonly string[] _storyPages = new string[]
        {
            "In the city, people traded more than goods—they traded secrets. Not the whispered kind shared in confidence, but secrets with weight and measurable value, cataloged and priced by unseen arbiters who worked behind mirrored glass towers. It was said that the richer the secret, the more it could buy: influence over rivals, freedom from accusation, protection from ruin, even the extension of one's own life. Currency faded, contracts dissolved, but secrets endured.\r\nNo official record confirmed the system's existence, yet everyone behaved as if it did. Conversations paused when strangers passed. Laughter was measured. Even silence felt deliberate. The city thrived on this quiet awareness, as though beneath its streets ran an invisible exchange that balanced power more precisely than any law.\r\n",

            "Leora walked these streets with caution, her gaze steady but observant. Every passerby carried an invisible ledger, a tally of truths withheld and truths surrendered. She imagined those ledgers hovering faintly over each person's head, shifting with every decision. She carried one herself—a memory that had never touched anyone's lips, a truth so carefully guarded that even she rarely examined it directly. Yet she knew its weight. In the wrong hands, it could fracture reputations and dismantle alliances built over decades.\r\nTonight, necessity pressed harder than fear. She turned into a narrow alley where neon lights flickered weakly, casting the walls in fractured shades of red and blue. At the alley's end stood a shop without a sign. Its windows were dark, its door unmarked, as though anonymity itself were part of its design.\r\n",

            "Inside, the air was cool and still. Shelves lined the walls, filled not with merchandise but with sealed envelopes arranged in precise rows. Each one hummed faintly, like a contained storm waiting for release. Behind a narrow counter sat an elderly clerk, his posture composed, his hands moving methodically as he sorted the envelopes by size and shade.\r\n\"Do you wish to trade?\" he asked without lifting his eyes.\r\nLeora placed her envelope on the counter. It felt heavier than paper should. \"I have something of value,\" she replied carefully.\r\nThe clerk rested his fingertips on the envelope and tilted it slightly, as if listening to a distant echo within. His expression did not change, yet the room seemed to grow quieter.\r\n\"Rare,\" he murmured at last. \"Untouched by confession. Untested by exposure.\" He looked up then, his gaze sharp despite his age. \"But it comes with a condition. Do you understand?\"\r\n\"I understand,\" she said, though uncertainty tightened her voice.\r\n\"No trade is without consequence,\" he continued. \"To give a secret is to alter the balance. To receive one is to accept its gravity.\"\r\nShe nodded. There was no space here for hesitation.\r\n",

            "The clerk slid a smaller envelope across the counter. Unlike the others, it did not hum. It felt still, almost weightless. \"This is what you are owed,\" he said. \"Open it only when you are prepared.\"\r\nOutside, the city carried on, indifferent to the quiet exchanges shaping its invisible hierarchy. Cars moved in steady lines. Conversations drifted from café terraces. Above it all, the glass towers reflected the night sky, their windows glowing like watchful eyes. For every secret surrendered, another was claimed. For every truth buried, another surfaced elsewhere. The system maintained itself with quiet precision.\r\n",

            "Leora walked several blocks before stopping beneath a flickering streetlamp. She turned the envelope over in her hands, aware that whatever lay inside would not simply grant advantage—it would demand choice.\r\nWhen she finally opened it, she found no written confession, no recorded evidence. Instead, a single line of ink stretched across the paper: You are not the only one who remembers.\r\n",

            "The words struck deeper than accusation. The secret she had given away had not disappeared; it had merely shifted position within the city's unseen network. Someone else now possessed the power to reveal or withhold it. And in exchange, she had been granted knowledge of another concealed truth—one that implicated a figure whose influence reached the highest towers.\r\nShe understood then that the envelope contained not an object, but a decision. She could expose what she now knew and unravel a powerful life, perhaps securing her own safety in the process. Or she could remain silent, preserving a fragile balance that kept greater harm at bay.\r\n",

            "Power in this city was never absolute. It existed in tension, suspended between revelation and restraint. To act was to invite retaliation. To remain silent was to accept complicity.\r\nLeora folded the paper and slipped it back into the envelope. As she resumed walking, neon signs flickered above her like distant, unstable stars. She felt neither triumph nor relief—only clarity. Survival here was not about acquiring the most secrets, nor about destroying rivals with hidden truths. It was about understanding the weight of what one carried and recognizing when silence held more strength than exposure.\r\n",

            "By the time she reached her apartment, the city had quieted into its midnight rhythm. Windows darkened one by one, but the towers remained lit, guardians of countless unspoken exchanges.\r\nLeora stood at her window, looking out at the skyline. In a place built on silent bargains, she realized that true power was not in possession. It was in the discipline to decide which truths to trade—and which to let fade, unspoken, into the vast and watchful dark.\r\n"
        };

        private BoxView[] _dots;

        public StoryPage7()
        {
            InitializeComponent();
            _dots = new BoxView[] { Dot1, Dot2, Dot3, Dot4, Dot5, Dot6, Dot7, Dot8 };
            UpdateUI();

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetNavBarHasShadow(this, false);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsVisible = false,
                IsEnabled = false
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void UpdateUI()
        {
            int total = _storyPages.Length;
            int current = _currentPage;

            StoryLabel.Text = _storyPages[current];

            double readingProgress = (current + 1) / (double)total * 80.0;
            int pct = (int)readingProgress;
            ProgressPercent.Text = $"{pct}%";

            double maxWidth = 640;
            ProgressFill.WidthRequest = maxWidth * readingProgress / 100.0;

            for (int i = 0; i < _dots.Length; i++)
            {
                _dots[i].BackgroundColor = (i == current)
                    ? Color.FromArgb("#0F2D4A")
                    : Color.FromArgb("#CBDCEB");
                _dots[i].WidthRequest = (i == current) ? 10 : 8;
                _dots[i].HeightRequest = (i == current) ? 10 : 8;
            }

            NextBtn.Opacity = (current == total - 1) ? 0.35 : 1.0;
            NextBtn.IsEnabled = (current < total - 1);

            PrevBtn.Opacity = (current == 0) ? 0.35 : 1.0;
            PrevBtn.IsEnabled = (current > 0);
            PrevBtnBorder.Opacity = (current == 0) ? 0.35 : 1.0;

            QuizBanner.IsVisible = (current == total - 1);

            if (current == total - 1 && !_progressSaved)
            {
                _progressSaved = true;
                _ = QuizFunc.SaveProgressAsync(storyNumber: 7, progress: pct);
            }
        }

        private void OnPrevClicked(object sender, EventArgs e)
        {
            if (_currentPage > 0) { _currentPage--; UpdateUI(); }
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            if (_currentPage < _storyPages.Length - 1) { _currentPage++; UpdateUI(); }
        }

        private async void OnGoToQuizClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QuizPage7(new QuizPage7ViewModel()));
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

        private async void OnDictionaryClicked(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new DictionaryPage7());
        }
    }
}
