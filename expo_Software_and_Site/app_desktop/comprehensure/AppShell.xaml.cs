using comprehensure.DASHBOARD;
using comprehensure.DASHBOARD.MiniGames;
using comprehensure.DASHBOARD.StoryPage;
using comprehensure.Models;

namespace comprehensure
{
    public partial class AppShell : Shell
    {
    
        private Grid? _popupOverlay;
        private TaskCompletionSource<bool>? _popupTcs;


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

            _popupOverlay = BuildPopupOverlay();
        }

       
        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await Task.Delay(250); // wait for flyout to close it weird gliches occur when u remove this

            bool confirmed = await ShowLogoutPopup();
            if (confirmed)
            {
                Preferences.Default.Remove("SavedUserUid");
                Preferences.Default.Remove("SavedUserEmail");
                await GoToAsync("///MainPage");
            }
        }
      
        private Task<bool> ShowLogoutPopup()
        {
            _popupTcs = new TaskCompletionSource<bool>();

           
            ContentPage? page = GetCurrentPage();
            if (page == null)
            {
                _popupTcs.SetResult(false);
                return _popupTcs.Task;
            }

         
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

            if (_popupOverlay!.Parent != null)
                root.Children.Remove(_popupOverlay);

            root.Children.Add(_popupOverlay);
            _popupOverlay.IsVisible = true;

            return _popupTcs.Task;
        }

        private void HidePopup(bool result)
        {
            if (_popupOverlay != null)
                _popupOverlay.IsVisible = false;

            _popupTcs?.TrySetResult(result);
        }

       
        private void OnLogoutConfirm(object sender, EventArgs e) => HidePopup(true);
        private void OnLogoutCancel(object sender, EventArgs e) => HidePopup(false);

      
        private static ContentPage? GetCurrentPage()
        {
          
            var stack = Shell.Current?.Navigation?.NavigationStack;
            if (stack != null)
                for (int i = stack.Count - 1; i >= 0; i--)
                    if (stack[i] is ContentPage cp) return cp;

      
            if (Shell.Current?.CurrentPage is ContentPage sp)
                return sp;

            return null;
        }

      
        private Grid BuildPopupOverlay()
        
        {
         
            var yesBtn = new Button
            {
                Text = "Yes, log out",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.White,
                BorderWidth = 0,
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                HeightRequest = 52,
            };
            yesBtn.Clicked += OnLogoutConfirm;

            var yesBorder = new Border
            {
                BackgroundColor = Color.FromArgb("#0F2D4A"),
                Stroke = Colors.Transparent,
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(26) },
                Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#0F2D4A")), Offset = new Point(0, 6), Radius = 16, Opacity = 0.22f },
                Content = yesBtn,
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
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(26) },
                Content = cancelBtn,
            };

         
            var gradientBand = new Border
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
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(36, 36, 0, 0) },
            };

      
            var iconChip = new Border
            {
                BackgroundColor = Color.FromArgb("#D6EAFF"),
                Stroke = Colors.Transparent,
                StrokeThickness = 0,
                HeightRequest = 64,
                WidthRequest = 64,
                HorizontalOptions = LayoutOptions.Center,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(32) },
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
                WidthRequest = 380,
                BackgroundColor = Colors.White,
                Stroke = Color.FromArgb("#CBDCEB"),
                StrokeThickness = 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(36) },
                Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#0F2D4A")), Offset = new Point(0, 16), Radius = 48, Opacity = 0.15f },
                Content = new VerticalStackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        gradientBand,
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
                                new BoxView { HeightRequest = 1, BackgroundColor = Color.FromArgb("#D6EAFF"), Margin = new Thickness(0, 4) },
                                new VerticalStackLayout { Spacing = 12, Children = { yesBorder, cancelBorder } },
                            },
                        },
                    },
                },
            };

        
            var overlay = new Grid
            {
                BackgroundColor = Color.FromArgb("#60000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                IsVisible = false,
            };
            overlay.Children.Add(card);

            return overlay;
        }
    }
}