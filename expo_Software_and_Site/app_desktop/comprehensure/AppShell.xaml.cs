using comprehensure.DASHBOARD;
using comprehensure.DASHBOARD.MiniGames;
using comprehensure.DASHBOARD.StoryPage;
using comprehensure.Models;
using Microsoft.Maui.Controls.Shapes;

namespace comprehensure
{
    public partial class AppShell : Shell
    {
        private Grid? _logoutOverlay;

        public AppShell()
        {
            BindingContext = new AppShellViewModel();
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(MainDashboard), typeof(MainDashboard));
            Routing.RegisterRoute(nameof(ModulesDashboard), typeof(ModulesDashboard));
            Routing.RegisterRoute(nameof(AboutUs), typeof(AboutUs));
            Routing.RegisterRoute(nameof(StoryPage1), typeof(StoryPage1));
            Routing.RegisterRoute(nameof(StoryPage2), typeof(StoryPage2));
            Routing.RegisterRoute(nameof(StoryPage3), typeof(StoryPage3));
            Routing.RegisterRoute(nameof(StoryPage4), typeof(StoryPage4));
            Routing.RegisterRoute(nameof(StoryPage5), typeof(StoryPage5));
            Routing.RegisterRoute(nameof(StoryPage6), typeof(StoryPage6));
            Routing.RegisterRoute(nameof(StoryPage7), typeof(StoryPage7));
            Routing.RegisterRoute(nameof(StoryPage8), typeof(StoryPage8));
            Routing.RegisterRoute("SynonymGamePage", typeof(SynonymHuntPage));
            Routing.RegisterRoute("OneThemeGamePage", typeof(OneThemePage));
            Routing.RegisterRoute("ProfileDashboard", typeof(ProfileDashboard));
            Routing.RegisterRoute(nameof(QuizPage1), typeof(QuizPage1));
            Routing.RegisterRoute(nameof(QuizPage2), typeof(QuizPage2));
            Routing.RegisterRoute(nameof(QuizPage3), typeof(QuizPage3));
            Routing.RegisterRoute(nameof(QuizPage4), typeof(QuizPage4));
            Routing.RegisterRoute(nameof(QuizPage5), typeof(QuizPage5));
            Routing.RegisterRoute(nameof(QuizPage6), typeof(QuizPage6));
            Routing.RegisterRoute(nameof(QuizPage7), typeof(QuizPage7));
            Routing.RegisterRoute(nameof(QuizPage8), typeof(QuizPage8));
            Routing.RegisterRoute("HelpPage", typeof(HelpPage));
            Routing.RegisterRoute("ChangePassword", typeof(ChangePassword));

            _logoutOverlay = BuildLogoutOverlay();
        }

        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await Task.Delay(300);

            ContentPage? page = null;
            var stack = Shell.Current?.Navigation?.NavigationStack;
            if (stack != null)
                for (int i = stack.Count - 1; i >= 0; i--)
                    if (stack[i] is ContentPage cp) { page = cp; break; }
            page ??= Shell.Current?.CurrentPage as ContentPage;

            if (page == null || _logoutOverlay == null) return;

            Grid root;
            if (page.Content is Grid g)
            {
                root = g;
            }
            else
            {
                root = new Grid();
                if (page.Content != null)
                    root.Children.Add(page.Content);
                page.Content = root;
            }

            if (_logoutOverlay.Parent is Grid oldParent)
                oldParent.Children.Remove(_logoutOverlay);

            root.Children.Add(_logoutOverlay);
            _logoutOverlay.IsVisible = true;
        }

        private void HideOverlay()
        {
            if (_logoutOverlay == null) return;
            _logoutOverlay.IsVisible = false;
            if (_logoutOverlay.Parent is Grid parent)
                parent.Children.Remove(_logoutOverlay);
        }

        private async void OnLogoutConfirm(object sender, EventArgs e)
        {
            HideOverlay();
            Preferences.Default.Clear();
            await GoToAsync("///MainPage");
        }

        private void OnLogoutCancel(object sender, EventArgs e)
        {
            HideOverlay();
        }

        private Grid BuildLogoutOverlay()
        {
            var confirmBtn = new Button
            {
                Text = "Yes, log out",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.White,
                BorderWidth = 0,
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                HeightRequest = 52,
            };
            confirmBtn.Clicked += OnLogoutConfirm;

            var confirmBorder = new Border
            {
                BackgroundColor = Color.FromArgb("#0F2D4A"),
                Stroke = Colors.Transparent,
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(26) },
                Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#0F2D4A")), Offset = new Point(0, 6), Radius = 16, Opacity = 0.22f },
                Content = confirmBtn,
            };

            var cancelBtn = new Button
            {
                Text = "Cancel",
                BackgroundColor = Colors.Transparent,
                TextColor = Color.FromArgb("#0F2D4A"),
                BorderWidth = 0,
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                HeightRequest = 52,
            };
            cancelBtn.Clicked += OnLogoutCancel;

            var cancelBorder = new Border
            {
                BackgroundColor = Colors.Transparent,
                Stroke = Color.FromArgb("#CBDCEB"),
                StrokeThickness = 1.5,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(26) },
                Content = cancelBtn,
            };

            var gradientBar = new Border
            {
                HeightRequest = 6,
                Stroke = Colors.Transparent,
                StrokeThickness = 0,
                Background = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb("#0F2D4A"), 0f),
                        new GradientStop(Color.FromArgb("#4A7FA8"), 1f),
                    },
                    new Point(0, 0), new Point(1, 0)
                ),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(36, 36, 0, 0) },
            };

            var iconChip = new Border
            {
                BackgroundColor = Color.FromArgb("#D6EAFF"),
                Stroke = Colors.Transparent,
                StrokeThickness = 0,
                HeightRequest = 64,
                WidthRequest = 64,
                HorizontalOptions = LayoutOptions.Center,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(32) },
                Content = new Label
                {
                    Text = "\U0001F6AA",
                    FontSize = 30,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
            };

            var card = new Border
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 360,
                BackgroundColor = Colors.White,
                Stroke = Color.FromArgb("#CBDCEB"),
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(36) },
                Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#0F2D4A")), Offset = new Point(0, 16), Radius = 48, Opacity = 0.15f },
                Content = new VerticalStackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        gradientBar,
                        new VerticalStackLayout
                        {
                            Spacing = 20,
                            Padding = new Thickness(36, 28, 36, 32),
                            Children =
                            {
                                iconChip,
                                new Label
                                {
                                    Text = "Log Out?",
                                    FontSize = 22,
                                    FontAttributes = FontAttributes.Bold,
                                    TextColor = Color.FromArgb("#0F2D4A"),
                                    HorizontalOptions = LayoutOptions.Center,
                                },
                                new Label
                                {
                                    Text = "Are you sure you want to log out of your account?",
                                    FontSize = 14,
                                    TextColor = Color.FromArgb("#6B8CAE"),
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    LineHeight = 1.6,
                                    LineBreakMode = LineBreakMode.WordWrap,
                                },
                                new BoxView
                                {
                                    HeightRequest = 1,
                                    BackgroundColor = Color.FromArgb("#D6EAFF"),
                                    Margin = new Thickness(0, 4),
                                },
                                new VerticalStackLayout
                                {
                                    Spacing = 12,
                                    Children = { confirmBorder, cancelBorder }
                                },
                            },
                        },
                    },
                },
            };

            var overlay = new Grid
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                IsVisible = false,
            };
            overlay.Children.Add(card);

            return overlay;
        }
    }
}
