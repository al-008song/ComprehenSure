using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage1 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;          // prevent duplicate writes

        public int StoryProgress => _currentPage;

        private readonly string[] _storyPages = new string[]
        {
            "In a quiet town surrounded by green hills, there stood an old library that most people had forgotten. It was built many years ago, long before modern buildings and computers became common. The paint on its walls was fading, and the wooden doors made a soft creaking sound when opened. Few visitors entered anymore. Most people preferred new bookstores or searched for information online. Because of this, the library felt lonely. Inside, tall shelves were filled with dusty books. Some books looked so old that their pages had turned yellow. The smell of old paper and wood filled the air. Sunlight entered through narrow windows, lighting up floating dust in the room. It felt like stepping into the past.",

            "People in town often said that the library held forgotten knowledge, stories and letters that no one had read for years. Some even believed it contained secret writings from explorers who had traveled to faraway lands long ago. One afternoon, a curious student named Leo decided to visit. Leo loved history. While other students saw history as boring lists of dates, he saw it as stories of real people who made brave choices. When he heard rumors about the old library, his interest grew stronger. He wanted to see if the stories were true. As he stepped inside, the wooden floor creaked under his shoes. The room was quiet—so quiet that he could hear the turning of his own pages. He slowly walked between the shelves, reading the titles. Many books had not been touched for years.",

            "While exploring the back corner of the library, Leo noticed something unusual. One bookshelf looked slightly out of place. Curious, he gently pushed it. To his surprise, it moved just enough to reveal a narrow passage behind it. His heart beat faster. He stepped inside the hidden space. It was small and dim, with a single desk and several old journals stacked on top. The covers were worn, and some pages were tied together with thin string. Leo carefully opened one journal. The writing inside was strange. It was written in shorthand—a quick writing style used long ago to save time and space. At first, he could not understand it. But instead of giving up, he copied parts of the text and began studying the symbols.",

            "For days after school, Leo returned to the library. He sat at the small desk and worked patiently. He compared letters, searched for patterns, and slowly decoded the words. It was not easy. Sometimes he felt frustrated. Sometimes he made mistakes and had to start again. But little by little, the stories began to make sense. The journals were written by explorers from many years ago. They described long journeys across unknown lands. There were detailed drawings of rivers, mountains, and paths that were not on modern maps. Some pages told of dangerous storms and difficult choices. Others spoke about teamwork, courage, and fear.",

            "One journal described an expedition where a group of travelers had disappeared. The writer carefully recorded their last known location and the choices they made before they vanished. Leo realized that these were not just adventure stories. They were real accounts of human decisions—good ones and bad ones. As he continued reading, Leo began to understand something important. History was not only about big events. It was about small details. It was about writing things down carefully. It was about observing and recording what people did, why they did it, and what happened after.",

            "He learned that one small decision could change everything. A wrong turn on a map. A choice to continue during bad weather. A moment of doubt or courage. After a month of hard work, Leo had translated several journals. He organized the information into clear notes and simple summaries. He created maps based on the old drawings and compared them with modern ones. Excited by what he discovered, Leo shared his findings with the local history club. At first, some members were surprised that such important stories had been hidden for so long. But as Leo explained the journals, everyone became interested.",

            "The club decided to preserve the writings and inform the town about the forgotten explorers. Soon, more people began visiting the library. The once-quiet building slowly became alive again. Standing inside the old library one evening, Leo looked around at the shelves. He realized something meaningful. The most valuable knowledge is not always the most popular. Sometimes, important truths are hidden in quiet places. Sometimes, they wait patiently for someone curious enough to search for them."
        };

        private BoxView[] _dots;

        public StoryPage1()
        {
            InitializeComponent();
            _dots = new BoxView[] { Dot1, Dot2, Dot3, Dot4, Dot5, Dot6, Dot7 };
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
                _ = QuizFunc.SaveProgressAsync(storyNumber: 1, progress: pct);
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
            await Navigation.PushAsync(new QuizPage1(new QuizPage1ViewModel(StoryProgress)));
        }

        private async void OnDictionaryClicked(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new DictionaryPage1());
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
    }
}
