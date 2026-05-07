using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage2 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;

        private readonly string[] _storyPages = new string[]
        {
            "In the quiet town of Maplewood, a narrow street was lined with small shops that had stood for many years. Between a warm bakery that smelled of fresh bread and a cozy bookstore filled with novels stood an old clock shop with a wooden sign that read, Alden's Timepieces. The gold paint on the letters had faded, and the windows were slightly fogged from years of dust and oil, yet the shop remained open every day without fail.\r\nInside worked Mr. Alden, the town's clockmaker. He was an elderly man with silver hair and round glasses that rested gently on his nose. His hands were steady and careful, even after decades of repairing tiny gears and springs. The shop was always filled with the steady ticking of clocks. Tall grandfather clocks stood proudly against the walls, small pocket watches rested in glass cases, and cuckoo clocks hung near the ceiling. Though each clock was different, they somehow ticked in perfect harmony, creating a calm and steady rhythm that filled the room.\r\n",

            "People in Maplewood respected Mr. Alden not only because he was skilled, but because of his unusual way of selling clocks. Whenever a customer pointed to a clock they liked, he rarely handed it to them right away. Instead, he would ask questions about their daily lives. He wanted to know when they woke up, how they spent their afternoons, and whether they preferred quiet or soft sound while working. After listening carefully, he would often suggest a different clock—one that seemed to fit them better than the one they first chose. Some people found this strange, but many later admitted that he was always right.\r\nOne rainy afternoon, a young woman named Clara stepped into the shop. She had been preparing for important exams and wanted a small clock for her desk to help her manage her time. The rain tapped softly against the window as she looked around. Her eyes quickly landed on a simple silver clock. It was modern, neat, and quiet. She decided it would be perfect.\r\n",

            "\"I would like this one, please,\" Clara said politely.\r\nMr. Alden did not immediately reach for it. Instead, he studied her expression. He noticed the slight tension in her shoulders and the determined look in her eyes. After a moment, he walked toward a shelf at the back of the shop and picked up a wooden clock. It was not shiny or modern. Its face had a strange design carved into it, a maze made of thin winding lines that crossed and curved in careful patterns.\r\n\"This clock would suit you better,\" he said gently.\r\nClara looked at it with curiosity. It seemed unusual and slightly old-fashioned. Mr. Alden explained that the clock was not just for telling time. He said it would teach patience, attention, and observation. Clara did not fully understand what he meant, but something about his calm voice made her trust him. After a short pause, she bought the wooden clock and carried it home.\r\nThat evening, she placed the clock on her desk. As she reviewed her notes, she noticed something different about it. Every hour, a small hand inside the clock moved along the carved maze instead of simply circling the edge like a normal clock. It followed the winding path carefully and paused at certain points before continuing. Curious, Clara leaned closer to observe it.\r\n",

            "The next hour, she watched again. The tiny hand stopped at a different place on the maze. Clara decided to draw the path in her notebook. Over the next few days, she continued observing and sketching the movement. She began to notice patterns in where the hand paused. Some of the stopping points formed shapes that looked like letters. Others seemed to represent numbers.\r\nAfter carefully comparing her notes one evening, Clara realized that the maze was forming short hidden messages. The first message she discovered encouraged her to observe closely. A few days later, another message appeared, reminding her to find patterns and pay attention to details. The clock was giving her small puzzles, and each one required careful thinking.\r\n",

            "As weeks passed, Clara found herself becoming more patient. She no longer rushed through her work. Instead, she read each question slowly and carefully. She double-checked her answers and thought about problems step by step. Even outside of her studies, she noticed simple patterns in daily life, such as the regular opening of shops, the rhythm of footsteps on the street, and the repeated habits of people around her.\r\nThe clock did not simply measure time. It quietly trained her mind. It reminded her that clear thinking requires focus and patience. By the end of the month, Clara felt more confident in her abilities. She was not only working harder; she was thinking more clearly.\r\n",

            "One afternoon, she returned to Mr. Alden's shop to thank him. The familiar ticking sound welcomed her as she stepped inside. Mr. Alden was polishing a large golden clock in the corner. Without looking up, he smiled and said that she had learned well. Clara told him about the hidden messages and how they had helped her grow.\r\nMr. Alden adjusted his glasses and gently explained that the lessons were never only in the clock itself. They were in the patience she practiced, the attention she gave, and the curiosity she chose to keep. The clock had simply guided her to develop those qualities on her own.\r\nClara left the shop feeling grateful. As she walked through the quiet streets of Maplewood, she realized she carried more than just a wooden clock. She carried a new way of thinking. She understood that time is not only something to measure, but something to use wisely and used carefully.\r\n"
        };

        private BoxView[] _dots;

        public StoryPage2()
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
                _ = QuizFunc.SaveProgressAsync(storyNumber: 2, progress: pct);
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
            await Navigation.PushAsync(new QuizPage2(new QuizPage2ViewModel()));
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
            await Navigation.PushAsync(new DictionaryPage2());
        }
    }
}
