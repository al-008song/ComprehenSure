using CommunityToolkit.Maui;
using comprehensure.DASHBOARD;
using comprehensure.DASHBOARD.StoryPage;
using comprehensure.DataBaseControl.Models;
using comprehensure.Models;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Extensions.Logging;

namespace comprehensure
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var apiKey = LoadApiKey();

            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = apiKey,
                AuthDomain = "comprehensuredb-f9f7c.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            }));

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<MainDashboardViewModel>();
            builder.Services.AddTransient<UsernameReqViewModel>();
            builder.Services.AddTransient<ProfileDashboard>();
            builder.Services.AddTransient<ProfileDashboardViewModel>();
            builder.Services.AddTransient<UsernameReq>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddTransient<MainDashboard>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ModulesDashboardViewModel>();
            builder.Services.AddTransient<ModulesDashboard>();

            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<ChangePassword>();

            builder.Services.AddTransient<StoryPage1>();
            builder.Services.AddTransient<StoryPage2>();
            builder.Services.AddTransient<StoryPage3>();
            builder.Services.AddTransient<StoryPage4>();
            builder.Services.AddTransient<StoryPage5>();
            builder.Services.AddTransient<StoryPage6>();
            builder.Services.AddTransient<StoryPage7>();
            builder.Services.AddTransient<StoryPage8>();

            builder.Services.AddTransient<StoryPage2ViewModel>();
            builder.Services.AddTransient<StoryPage4ViewModel>();

            builder.Services.AddTransient<QuizPage1ViewModel>(); // ADDED
            builder.Services.AddTransient<QuizPage2ViewModel>();
            builder.Services.AddTransient<QuizPage3ViewModel>();
            builder.Services.AddTransient<QuizPage4ViewModel>(); // ADDED
            builder.Services.AddTransient<QuizPage5ViewModel>();
            builder.Services.AddTransient<QuizPage6ViewModel>();
            builder.Services.AddTransient<QuizPage7ViewModel>();
            builder.Services.AddTransient<QuizPage8ViewModel>();

            builder.Services.AddTransient<QuizPage1>(); // ADDED
            builder.Services.AddTransient<QuizPage2>();
            builder.Services.AddTransient<QuizPage3>(); // ADDED
            builder.Services.AddTransient<QuizPage4>(); // ADDED
            builder.Services.AddTransient<QuizPage5>(); // ADDED
            builder.Services.AddTransient<QuizPage6>(); // ADDED
            builder.Services.AddTransient<QuizPage7>(); // ADDED
            builder.Services.AddTransient<QuizPage8>(); // ADDED

            return builder.Build();
        }

        private static string LoadApiKey()
        {
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("secrets.json").Result;
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("FirebaseApiKey").GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load API key: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
