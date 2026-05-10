using System;
using Microsoft.Maui.Controls;
using comprehensure.Models;

namespace comprehensure.DASHBOARD.StoryPage
{
    public partial class StoryPage8 : ContentPage
    {
        private int _currentPage = 0;
        private bool _progressSaved = false;

        private readonly string[] _storyPages = new string[]
        {
            "The forest had grown unnaturally dense, its trees fused with strands of blackened iron that twisted through bark and branch like hardened veins. Travelers spoke of it in lowered voices, claiming the wind carried whispers—neither human nor animal, but something suspended uneasily between the two. Maren arrived at its edge not out of curiosity, but necessity.\r\nA message had been delivered to her by a courier who spoke only once, his lips trembling as though the words themselves frightened him. “The forest answers,” he said. “But it asks a price.” She had not asked what kind of price. Some questions, she understood, revealed themselves only when the time demanded it.\r\n",

            "Inside, the forest defied reason. Leaves clanged softly against one another like distant chains stirred by invisible hands. Roots rose from the ground in arching patterns, resembling the vaulted ceilings of a cathedral forged from metal and shadow.\r\nThe air carried the scent of rain and rust. Even light seemed altered; shadows shifted against the direction of the sun, stretching toward her instead of away. Every step Maren took felt measured, as though the ground anticipated her movement. The deeper she walked, the more the familiar sky narrowed into slivers of pale gray between iron-laced branches. Soon, even those fragments vanished, replaced by a metallic haze that blurred distance and distorted sound. \r\n",

            "Time lost its meaning. Hours—or perhaps days—passed without distinction. Hunger dulled, fatigue drifted at the edges of her awareness, yet she did not stop. The whispers grew clearer. At first they were fragments of indistinct murmurs, like conversations overheard through walls. Then they sharpened into recognizable tones. She heard her own voice among them, repeating sentences she did not remember speaking. Promises she had never made echoed alongside fears she had never admitted aloud.",

            "Each whisper felt deliberate. They tested her resolve, tempting her with alternate versions of herself—bolder choices she might have taken, harsher words she might have spoken, gentler paths she might have followed. In the silence between the murmurs, she realized something unsettling. The forest did not merely echo what it found within her. It observed. It adjusted. It learned.",

            "It knew her. Eventually, she reached a clearing where the air seemed heavier, as if saturated with unseen weight. At its center stood an iron tree stretching impossibly high, its trunk engraved with symbols that pulsed faintly like a slow, steady heartbeat. The markings were unfamiliar, yet they felt strangely comprehensible, as though meaning hovered just beyond her understanding. At the base of the tree lay a still pool of water. When Maren stepped closer, she saw that it did not reflect her face. Instead, it shimmered with scenes from her life. Childhood laughter beneath a summer sky.",

            "A night of bitter loss beneath cold rain. A quiet moment of triumph she had never shared with anyone. Some images were sharp with detail; others wavered, uncertain, as though memory itself questioned their accuracy. A voice emerged from the iron tree, deep and resonant, carrying the scrape of metal beneath its tone. “Choose one memory to leave behind. All else may follow you, but this must be relinquished.” The words settled heavily in the clearing. Maren did not answer at once. She knelt beside the pool, studying the shifting images. Each memory carried weight.",

            "Pain had shaped her caution. Regret had sharpened her judgment. Joy had reminded her of what was worth protecting. Even sorrow had carved depth into her understanding of others. To surrender one was not a simple exchange; it was an alteration of identity. She considered abandoning a moment of humiliation, freeing herself from the sting of failure. She considered releasing a memory of grief that still tightened her chest when recalled. Yet she hesitated. Without those memories, who would she become? Would relief also erase the lessons they carried? The whispers circled the clearing, growing restless. The iron bark trembled faintly, as if impatient.",

            "At last, Maren spoke. She chose a memory rooted not only in pain, but in self-doubt—a moment when fear had defined her more than circumstance. It was a memory that had quietly guided her choices for years, narrowing her path without her fully realizing it. When she whispered its name, the symbols along the iron trunk flared softly. The pool shimmered, and the chosen memory rose like mist from the water. For a fleeting instant, she felt it slip from her grasp—an ache both sharp and hollow. The forest exhaled, a long metallic sigh that resonated through branch and root.",

            "Silence followed. The clearing brightened. The iron tree stood still, its pulse fading to a faint glow. Maren touched her forehead, searching for the absence. The details of the surrendered moment blurred beyond reach. She remembered that something significant had once occupied that space, but its edges were indistinct, softened like a faded photograph.",

            "When she turned back, the path behind her had cleared. The forest no longer pressed inward. The sky reappeared above, pale and open. As she stepped beyond the tree line, the metallic haze dissolved, and the ordinary world resumed its quiet rhythm. Yet something within her had shifted. The absence of that single memory did not weaken her as she had feared. Instead, it created space—room for interpretation unshaped by old doubt. Decisions that once felt constrained now seemed wider, less burdened.",

            "The whispers did not disappear entirely. At times, in moments of uncertainty, she still sensed them at the edges of her thoughts. But they no longer controlled the narrative. She understood now that the forest had not sought to punish her. It had demanded awareness. Some forests, she realized, are not obstacles meant to be conquered. They are reflections—vast and unsettling—revealing the parts of ourselves we hesitate to examine. And sometimes, growth requires not the gathering of more, but the courage to let something go.\t"


        };

        private BoxView[] _dots;

        public StoryPage8()
        {
            InitializeComponent();
            _dots = new BoxView[] { Dot1, Dot2, Dot3, Dot4, Dot5, Dot6, Dot7, Dot8, Dot9, Dot10, Dot11 };
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
                _ = QuizFunc.SaveProgressAsync(storyNumber: 8, progress: pct);
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
            await Navigation.PushAsync(new QuizPage8(new QuizPage8ViewModel()));
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
            await Navigation.PushAsync(new DictionaryPage8());
        }
    }
}
