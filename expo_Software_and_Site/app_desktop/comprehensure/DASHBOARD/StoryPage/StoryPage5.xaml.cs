using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage5 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;

        private readonly string[] _storyPages = new string[]
        {
            "In the center of Riverton stood an old town hall built of stone that had darkened with age. Most people walked past it without a second glance, focused instead on errands, conversations, or the movement of traffic in the square. Yet behind its tall windows and heavy wooden doors was a small archive room that preserved the town’s history in an unusual way. Instead of photographs or written records alone, the room held maps—carefully drawn documents that recorded how Riverton had changed over decades.",

            "These were not simple maps of roads and rivers. They showed where new buildings had replaced open fields, where trees had been planted along sidewalks, and how neighborhoods had gradually extended toward the hills. Each line of ink represented a decision made at a particular moment in time.",

            "Elias discovered the archive by chance while waiting for a community meeting to begin. With time to spare, he wandered down a quiet hallway and noticed a door slightly open. Curious, he stepped inside. The room smelled faintly of paper and dust. Shelves along the walls were filled with rolled documents tied neatly with string. At the center stood a wide wooden table covered with large sheets of parchment marked by careful handwriting and precise lines.",

            "An older man sat beneath a desk lamp, studying a faded map with steady attention. He introduced himself as Mr. Rowan, the town’s record keeper. His responsibility, he explained, was to maintain an accurate history of Riverton’s development.\r\n“Every street you walk on,” Mr. Rowan said, gesturing toward the maps, “exists because someone decided it should.”\r\n",

            "He explained that roads were extended when families grew and needed better access to schools and markets. Parks were created after community members asked for shared spaces. Even narrow footpaths revealed patterns of movement, showing where people preferred to walk long before pavement was added.",

            "Elias listened thoughtfully. He had always assumed that towns expanded naturally, almost automatically, as populations increased. He had never considered that each building, each bridge, and each row of trees reflected a deliberate choice.\r\nMr. Rowan handed him two maps for comparison—one drawn fifty years earlier and another from the present day. At first, they appeared unrelated. The older map showed fewer streets and large areas of empty land.\r\n",

            "The newer one was more complex, filled with intersecting roads and tightly arranged neighborhoods. However, when Elias studied them closely, he began to notice continuity. Many of the new streets followed the direction of older paths. Commercial areas had developed near intersections that were once simple crossroads. Houses had expanded outward from a central square that had always served as a gathering place.\r\n“Growth is rarely accidental,” Mr. Rowan remarked. “It reflects patterns of need, habit, and foresight.”\r\n",

            "Over the following weeks, Elias returned regularly to the archive. He traced the development of Riverton across decades, examining how specific decisions influenced future outcomes. A bridge built across the river had increased trade and encouraged businesses to open nearby. The placement of a school on the outskirts had drawn families to settle in that area, gradually transforming farmland into residential streets. Even the decision to plant trees along one avenue had changed how people used the space, making it a preferred route for walking and conversation.",

            "As he reviewed these changes, Elias recognized a consistent principle: significant transformation rarely occurs all at once. Instead, it emerges from a series of small, interconnected decisions. Each choice creates conditions that influence the next.\r\nOne afternoon, Mr. Rowan presented him with a practical challenge. A new neighborhood was being planned at the edge of Riverton. Before construction began, planners sought suggestions for road placement and public spaces. Using the archived maps as reference, Elias examined traffic patterns, population density, and natural pathways already formed by repeated use.\r\n",

            "He observed that people tended to gather near open water and shaded areas. Businesses thrived where roads intersected at visible and accessible points. Public spaces were most successful when located near both residential and commercial zones. Drawing upon these patterns, he proposed a layout that integrated the new neighborhood with the existing structure of the town rather than separating it.",

            "Months later, as construction began, Elias walked along the marked ground where roads would soon be paved. The design felt balanced and intentional. It did not disrupt the flow of Riverton but extended it logically.\r\nThrough the maps, he had gained more than knowledge of urban planning. He had developed an understanding of how deliberate reasoning shapes environments over time. The archive had revealed that communities are built not only through grand visions, but through careful attention to long-term consequences.\r\n",

            "Standing outside the town hall one evening, Elias viewed Riverton with renewed awareness. The streets were no longer just pathways for movement; they were visible records of earlier judgments. The parks represented shared values. The arrangement of buildings reflected priorities established years before.\r\nHe understood that the future of any place depends upon thoughtful decisions made in the present. Like the lines drawn on a map, each choice leaves a trace. Over time, those traces form patterns that define the character of a community.\r\n"

        };

        private BoxView[] _dots;

        public StoryPage5()
        {
            InitializeComponent();
            _dots = new BoxView[] { Dot1, Dot2, Dot3, Dot4, Dot5, Dot6, Dot7, Dot8, Dot9, Dot10, Dot11, Dot12 };
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
                _ = QuizFunc.SaveProgressAsync(storyNumber: 5, progress: pct);
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
            await Navigation.PushAsync(new QuizPage5(new QuizPage5ViewModel()));
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
            await Navigation.PushAsync(new DictionaryPage5());
        }
    }
}
