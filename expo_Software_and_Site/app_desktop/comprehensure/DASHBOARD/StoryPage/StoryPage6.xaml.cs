using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage6 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;

        private readonly string[] _storyPages = new string[]
        {
            "When the paper wings were first revealed to the public, they were met with disbelief rather than admiration. They appeared too delicate to hold a person’s weight—thin, almost translucent layers arranged with careful precision. Yet the engineers insisted that the material itself was not the true breakthrough. The wings did not rely on engines or fuel. Instead, they responded to the mental and emotional state of the person wearing them. This idea unsettled many people. ",

            "Flight had always depended on mechanical force, something measurable and predictable. Now it depended on emotional steadiness, something far less certain. If a flyer lost focus or allowed fear to take control, the wings would react immediately.\r\nThe first public demonstration transformed doubt into fascination. A volunteer climbed a high platform overlooking a crowded square. The silence was heavy as he stepped forward and jumped. For a brief second, he fell like anyone else would. Then the wings unfolded and caught the air. Slowly, almost cautiously, he began to rise above the city.\r\n",

            "The crowd erupted in applause as he floated higher, suspended between earth and sky. Within months, launch towers appeared across major cities. Training centers promised to teach discipline and mental focus. For many, the wings symbolized freedom—an escape from traffic, from physical limits, and perhaps from ordinary life itself.\r\nHowever, the excitement did not last untouched. The first widely broadcast fall shifted public opinion. An experienced flyer, steady and confident during previous flights, suddenly began to waver midair. Witnesses described a visible hesitation in her movements, as if doubt had interrupted her concentration. Within seconds, the wings failed to sustain her.\r\n\r\n",

            "Investigations found no mechanical damage. The conclusion was unsettling: the failure had come from within. More incidents followed, and a pattern became clear. It was not fear alone that caused descent. It was the loss of emotional balance—the moment when fear overwhelmed control—that made the wings unstable.",

            "Dr. Anya Serrano, one of the lead engineers behind the invention, watched the public response change from admiration to caution. She had believed the wings would demonstrate human potential. Instead, they revealed human vulnerability. Governments began requiring psychological screenings before granting flight permits. Critics argued that no technology should depend so heavily on something as unpredictable as emotion.",

            "Anya, however, was not convinced that fear was the true problem. She began quietly interviewing those who maintained long, stable flights. She expected to find people who were unusually confident or naturally fearless. Instead, she found individuals who admitted they were afraid each time they stepped off a platform. “I never stop feeling fear,” one flyer told her. “I just don’t let it control me.” Another explained that pretending to be fearless only made the wings unstable, while acknowledging fear helped restore balance.",

            "Despite her role in creating the wings, Anya had never flown herself. She told others that her work required observation, but privately she knew the reason was simpler. She was afraid of failing publicly. One evening, long after the training center had closed, she climbed the launch tower alone. The city lights stretched beneath her, steady and distant. She fastened the wings carefully to her shoulders and stepped toward the edge. Doubt surfaced immediately—memories of failed prototypes, harsh media criticism, and the weight of responsibility she carried for every accident. Her breathing quickened, and she felt the wings tremble slightly in response.",

            "For a moment, she nearly stepped back. Instead, she moved forward and jumped. The drop was sudden and disorienting. Wind rushed past her, and panic rose sharply in her chest. The wings shuddered as her fear intensified. She felt gravity pulling her downward and understood how easily she could lose control. Then she stopped trying to eliminate the fear. Instead of denying it, she acknowledged it. “I am afraid,” she said quietly. The admission did not remove the fear, but it steadied her. Her breathing slowed. The wings adjusted. Gradually, the descent softened, and she began to rise.",

            "From above the skyline, the city seemed smaller and less overwhelming. The buildings below appeared structured and solid, yet fragile from that height. In that moment, Anya realized that the wings were not designed to reward simple bravery. They responded to balance—the ability to hold fear and determination at the same time without allowing either to dominate.\r\nAfter that night, she revised the training programs. Instructors began teaching emotional awareness instead of forced confidence. Flyers were encouraged to recognize doubt rather than suppress it. Stability, they learned, came from understanding their internal reactions, not from pretending they did not exist.\r\n",

            "Over time, accidents became less frequent. Flying gradually lost its dramatic appeal and became more personal. Many chose to rise at sunrise or dusk, when the sky was quiet and there was no audience. The wings did not remove the invisible weight people carried within themselves. They revealed it. Those who learned to rise were not those who felt no fear, but those who understood its presence and remained steady despite it."

        };

        private BoxView[] _dots;

        public StoryPage6()
        {
            InitializeComponent();
            _dots = new BoxView[] { Dot1, Dot2, Dot3, Dot4, Dot5, Dot6, Dot7, Dot8, Dot9, Dot10 };
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

            PageIndicator.Text = $"Page {current + 1} of {total}";
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
                _ = QuizFunc.SaveProgressAsync(storyNumber: 6, progress: pct);
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
            await Navigation.PushAsync(new QuizPage6(new QuizPage6ViewModel()));
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
            await Navigation.PushAsync(new DictionaryPage6());
        }
    }
}
