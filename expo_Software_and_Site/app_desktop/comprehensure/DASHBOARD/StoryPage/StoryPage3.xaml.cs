using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage3 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;

        private readonly string[] _storyPages = new string[]
        {
            "At the highest point of Briarwood stood an old stone observatory that many people had forgotten. It had been built decades ago by a small group of scientists who believed that understanding the sky could help people better understand themselves. Over time, newer buildings replaced it, and brighter city lights made it harder to see the stars. Eventually, the observatory became quiet, visited only by a few curious minds.\r\nOne evening, Adrian decided to climb the hill and see it for himself. He had always been fascinated by the night sky. While others admired the stars for their beauty, he often wondered how they moved so precisely, how they could appear scattered yet follow invisible paths. The observatory door was heavy, but it opened with a slow push. Inside, dust covered the wooden floor, and old charts of constellations hung on the walls. At the center of the room stood a large telescope, still pointed toward the sky as if waiting to be used again.\r\n",

            "An older woman stood beside it, adjusting a notebook filled with handwritten calculations. She introduced herself as Dr. Vale, the caretaker of the observatory. Though most people assumed the building was abandoned, she continued to maintain it, recording the movement of stars each night. \"You're here because you're curious,\" she said, noticing Adrian's careful gaze. He nodded. \"I've always wondered how the stars seem random but still follow patterns.\" Dr. Vale smiled slightly. \"They are not random. Their motion follows laws—predictable, measurable laws. It only seems chaotic from a distance.\" She invited him to look through the telescope. Adrian adjusted his eyes and saw a cluster of stars glowing faintly against the dark sky. Dr. Vale handed him an old star chart and asked him to compare what he saw with the markings on the page.",

            "At first, the chart looked confusing. Lines connected distant stars, forming shapes that did not seem obvious. But as he looked back and forth between the chart and the sky, he began to notice alignment. Certain stars always rose together. Others followed steady paths across the horizon. Over the next several evenings, Adrian returned to the observatory. Dr. Vale taught him how to track movement over time rather than in a single moment. She explained that understanding required patience. A single glance at the sky could not reveal its structure. Only careful observation, repeated night after night, could show the pattern.",

            "He began recording positions of stars at the same hour each evening. Slowly, he saw how they shifted in small, consistent ways. What once looked scattered now appeared organized. The sky moved like a vast clock, each star playing its role.\r\nOne cloudy night, when no stars were visible, Adrian felt disappointed. Dr. Vale, however, handed him a different task. She asked him to review his notes and predict where a certain star would appear when the sky cleared. Using his past observations, he calculated its expected position.\r\nWhen the clouds finally parted two nights later, he looked through the telescope with quiet anticipation. The star shone exactly where he had predicted.\r\n",

            "The moment filled him with a deep sense of clarity. It was not simply about being correct. It was about understanding that even the most distant lights in the sky moved with purpose and order.\r\nAs the weeks passed, Adrian began to notice patterns beyond astronomy. He observed how seasons changed gradually, how shadows lengthened and shortened at predictable times, and how daily routines formed quiet cycles. The sky had trained him to look for consistency within change.\r\nOn his final evening at the observatory before leaving town for new opportunities, Adrian stood beside Dr. Vale and looked at the horizon. The stars began to appear one by one.\r\n",

            "\"Most people think discovery is about finding something new,\" Dr. Vale said softly. \"But often, it is about seeing what has always been there.\"\r\nAdrian understood. The observatory had not shown him new stars. It had shown him how to observe carefully, think patiently, and trust patterns supported by evidence.\r\nAs he walked down the hill, the lights of Briarwood glowing below, he carried with him more than knowledge of constellations. He carried a disciplined way of thinking—a reminder that beneath confusion, there is often structure, and beneath randomness, there is design waiting to be understood.\r\n"
        };

        private BoxView[] _dots;

        public StoryPage3()
        {
            InitializeComponent();
            _dots = new BoxView[] { Dot1, Dot2, Dot3, Dot4, Dot5, Dot6 };
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
                _ = QuizFunc.SaveProgressAsync(storyNumber: 3, progress: pct);
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
            await Navigation.PushAsync(new QuizPage3(new QuizPage3ViewModel()));
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
            await Navigation.PushAsync(new DictionaryPage3());
        }
    }
}
