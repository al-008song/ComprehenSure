using comprehensure.DataBaseControl.Models;

namespace comprehensure.DASHBOARD
{
    public partial class AboutUs : ContentPage
    {
     
        private record MemberInfo(string ImageSource, string Position, string Description);

        private static readonly Dictionary<string, MemberInfo> Members = new()
        {
            ["Fren"]   = new("member_fren.png", "Leader",
                             "Led the team in planning, ensured tasks were completed on time, and guided members throughout the development process. Frenchelle also contributed to the different aspects of the application."),

            ["Franz"]  = new("member_franz.png",  "Backend & Database Developer",
                             "Franz developed the core functionality and logic of the application; he also managed the systems data and database."),

            ["Abbey"]  = new("member_abbey.png",  "UI/UX Designer",
                             "Abbey designed the layout and user interface of the application, ensuring it is user-friendly, visually appealing, and easy to navigate."),

            ["Japlyn"] = new("member_Japlyn.png", "Content Developer",
                             "Japlyn handles the reading materials and quizzes, and developed one of the mini games. She also gave recommendations to improve the overall quality of the application."),

            ["York"]   = new("member_york.png",   "Content Developer",
                             "Stephen prepared the reading materials and comprehension quizzes, contributed to one of the mini games, and suggested ideas to imprpove the application."),

            ["Abram"]  = new("member_abram.png",  "Content Developer",
                             "Abram prepared the reading materials and comprehension questions."),

            ["Charly"] = new("member_charly.png", "Researcher",
                             "Charly handles research, gathered data, prepared documentation, and contributed to some quiz contents."),

            ["Daniel"] = new("member_daniel.png", "Researcher",
                             "Daniel conducted research, gathered data, prepared documentation, and provided suggestions to improve the overall quality of the app."),

            ["Megan"]  = new("member_megan.png",  "Researcher",
                             "Megan assisted in conducting research and gathering data."),
        };

        
        private string? _openMember = null;

       
        private Dictionary<string, (Border panel, Label label)> _descPanels = new();

        public AboutUs()
        {
            InitializeComponent();
            BindingContext = new AboutUsViewModel();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetNavBarHasShadow(this, false);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsVisible = false,
                IsEnabled = false
            });

            _descPanels = new Dictionary<string, (Border, Label)>
            {
                ["Fren"]   = (Desc_Fren,   DescLabel_Fren),
                ["Franz"]  = (Desc_Franz,  DescLabel_Franz),
                ["Abbey"]  = (Desc_Abbey,  DescLabel_Abbey),
                ["Japlyn"] = (Desc_Japlyn, DescLabel_Japlyn),
                ["York"]   = (Desc_York,   DescLabel_York),
                ["Abram"]  = (Desc_Abram,  DescLabel_Abram),
                ["Charly"] = (Desc_Charly, DescLabel_Charly),
                ["Daniel"] = (Desc_Daniel, DescLabel_Daniel),
                ["Megan"]  = (Desc_Megan,  DescLabel_Megan),
            };
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("MainDashboard");
        }


        private async void OnMemberTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not string name) return;
            if (!Members.TryGetValue(name, out var info)) return;
            if (!_descPanels.TryGetValue(name, out var ui)) return;

            var (panel, label) = ui;

            if (_openMember == name && panel.IsVisible)
            {
                await CollapsePanel(panel);
                _openMember = null;
                return;
            }

            if (_openMember != null && _descPanels.TryGetValue(_openMember, out var prev))
                await CollapsePanel(prev.panel);

            label.Text = info.Description;
            panel.Opacity = 0;
            panel.TranslationY = -6;
            panel.IsVisible = true;

            await Task.WhenAll(
                panel.FadeTo(1, 200, Easing.CubicOut),
                panel.TranslateTo(0, 0, 200, Easing.CubicOut)
            );

            _openMember = name;
        }

        private static async Task CollapsePanel(Border panel)
        {
            await Task.WhenAll(
                panel.FadeTo(0, 150, Easing.CubicIn),
                panel.TranslateTo(0, -6, 150, Easing.CubicIn)
            );
            panel.IsVisible = false;
        }
    }
}
