using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage4 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;

        private readonly string[] _storyPages = new string[]
        {
           "Mara had always been drawn to patterns. As a child, she noticed the repeating shapes in tiles, the rhythm in music, and the order in the way leaves grew along a stem. While others admired beauty without question, she often wondered why things were arranged the way they were. To her, patterns were not just decorations. They were clues.\r\nOne summer, while exploring the quieter edge of town, she heard about a private garden hidden behind tall stone walls. The garden belonged to Mr. Calder, a retired mathematician known for his quiet lifestyle and unusual ideas. Some people described him as eccentric, while others simply said he saw the world differently. The garden, according to rumor, was not designed for appearance alone. It was built on rules.\r\n",

           "Curiosity led Mara to the iron gate one warm afternoon. The gate was slightly open, as if inviting someone observant enough to enter. When she stepped inside, she immediately sensed that this was no ordinary place. The garden did not follow the common design of neat rows and random clusters of flowers. Instead, everything seemed deliberate.\r\nBright red flowers curved in spirals that widened as they stretched outward. Yellow blossoms appeared at exact intervals, each placed at a measured distance from the next. The hedges formed smooth arcs that seemed to echo one another, growing larger yet keeping the same shape. Even the stepping stones beneath her feet were arranged in a sequence, each stone slightly farther apart than the last.\r\n",

           "\"You're studying the spacing,\" a voice said calmly.\r\nMara turned to see a tall man with gray hair and thoughtful eyes standing near a line of lavender plants. His expression was not surprised, but approving.\r\n\"I was trying to understand it,\" Mara admitted. \"It doesn't look random.\"\r\n\"It isn't,\" Mr. Calder replied. \"This garden follows algorithms. Every plant grows according to a rule. Every path reflects a formula.\"\r\n",

           "Mara had heard the word before. An algorithm was a set of instructions, a sequence that guided actions step by step. She looked again at the spiral of flowers and wondered what invisible instructions had shaped them.\r\nMr. Calder did not explain everything at once. Instead, he asked her questions. How many petals did she count before the pattern repeated? How many steps did it take before the curve of the path mirrored itself? Where would the next row of flowers appear if the sequence continued?\r\n",

           "At first, Mara guessed incorrectly. Some patterns were simple, repeating every three or five spaces. Others followed more complex growth, expanding outward while doubling in number. The spirals reminded her of natural forms she had seen in seashells and sunflowers, though she could not yet explain why.\r\nShe returned to the garden often. Each visit became an exercise in careful observation. She carried a small notebook, sketching the layout of flower beds and marking distances between stones. She compared one section to another, looking for similarities hidden beneath differences. Gradually, she began to see connections that were not obvious at first glance.\r\n",

           "One afternoon, Mr. Calder pointed to a patch of soil where no flowers had yet bloomed. \"If the sequence continues,\" he said, \"where will the next cluster grow?\"\r\nMara studied the previous rows. She noticed that each new cluster added two more flowers than the one before and shifted slightly toward the east. After several minutes of quiet calculation, she pointed to a spot near the edge of the bed and predicted the number of blooms.\r\nDays later, the flowers appeared exactly where she had expected.\r\nThe success did not feel like luck. It felt like understanding.\r\n",

           "As summer continued, Mara's way of seeing began to change. She noticed patterns beyond the garden walls. The arrangement of bricks in old buildings followed repeated structures. The timing of traffic lights formed cycles. Even conversations between people had patterns—questions followed by responses, pauses followed by reactions.\r\nMusic, too, carried sequences. A melody often returned to its starting note after a series of changes. In nature, branches divided in predictable ways, and waves reached the shore in steady rhythms. The world was full of structure, even when it appeared chaotic.\r\n",

           "One evening, while walking through the garden at sunset, Mara realized that the paths were not only mathematical but balanced. For every expanding curve, there was a matching form elsewhere. The design held symmetry without being rigid. It allowed growth while maintaining order.\r\nMr. Calder eventually gave her a more difficult challenge. He asked her to design a new section of the garden that would blend with the existing structure. She was not to copy what was already there, but to continue its logic.\r\nMara walked through every path again, tracing spirals with her eyes and counting intervals between plants. She thought about how the patterns grew and how they maintained harmony. After careful planning, she sketched a design that extended one of the garden's sequences while introducing a subtle variation.\r\n",

           "When Mr. Calder reviewed her design, he nodded slowly. \"You've learned to see the rule beneath the surface,\" he said.\r\nMara understood then that the garden was never just about mathematics. It was about perception. It was about recognizing that order and beauty often work together. Logic did not remove wonder; it deepened it.\r\nBy the end of the summer, the garden no longer seemed mysterious. It felt like a conversation written in shapes and numbers. Mara had learned to read it. More importantly, she had learned to look at the world with sharper awareness. Patterns were not confined to equations or carefully planted flowers. They existed in movement, growth, and even human behavior.\r\n",

           "As she stepped out of the garden one final evening, the spirals of flowers glowing in the fading light, she understood something lasting. The world is filled with hidden structures waiting to be noticed. Once you learn how to search for them, ordinary places begin to reveal extraordinary design.\r\nAnd from that day forward, Mara carried with her not only knowledge of patterns, but a deeper appreciation for the quiet logic that shapes the world.\r\n"
        };

        private BoxView[] _dots;

        public StoryPage4()
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
                _ = QuizFunc.SaveProgressAsync(storyNumber: 4, progress: pct);
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
            await Navigation.PushAsync(new QuizPage4(new QuizPage4ViewModel()));
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
            await Navigation.PushAsync(new DictionaryPage4());
        }
    }
}
