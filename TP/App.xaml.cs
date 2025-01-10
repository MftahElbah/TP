using SQLite;
using SQLitePCL;
using TP.Methods;
using TP.Methods.actions;
using TP.Pages;

namespace TP
{
    public partial class App : Application
    {
        Database database = Database.SelectedDatabase;
        //private MineSQLite _sqlite = new MineSQLite();
        public static async Task<bool> IsInternetAvailable()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://clients3.google.com/generate_204");
                    Console.WriteLine($"Internet check response: {response.StatusCode}");
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internet check failed: {ex.Message}");
                return false;
            }
        }

        public App(){
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzYzMTM0MEAzMjM4MmUzMDJlMzBIUEF2a3E1ZzlTN3I3VXJDOHRKNDd3NlIyd0crTTd0TTBibml6Unl6SFl3PQ==");
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new ContentPage());
        }



        protected override async void OnStart()
        {
            try
            {
                UserSession.internet = await IsInternetAvailable();
                if (UserSession.internet)
                {
                    Database.SelectedDatabase = new firebaseDB();
                    UserSession.internet = true;
                    Console.WriteLine("Using Firebase Database.");
                }
                else
                {
                    Database.SelectedDatabase = new MineSQLite();
                    
                    UserSession.internet = false;
                    Console.WriteLine("Using SQLite Database.");
                }

                database = Database.SelectedDatabase;

                database.DatabaseStarted();
                await Task.Delay(1000);
                await InitializeApp();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error: {ex.Message}");
                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new ContentPage
                    {
                        Content = new Label
                        {
                            Text = $"Critical Error: {ex.Message}",
                            TextColor = Colors.Red,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                    };
                }
            }
        }

        private async Task InitializeApp()
        {
            try
            {
                var session = await database.UserSessionChecker();
                if (session == null)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
                    }
                    return;
                }

                var user = await database.loginSecction(session.Password, session.UserId);
                                

                if (user == null || !user.IsActive)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
                    }
                    return;
                }

                UserSession.UserId = user.UserId;
                UserSession.Name = user.Name;
                UserSession.UserType = user.UserType;
                UserSession.Password = user.Password;
                UserSession.SessionYesNo = true;

                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new NavigationPage(new SubjectSelectionPage());
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., logging)
                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new NavigationPage(new ContentPage { Content = new Label { Text = $"Error: {ex.Message}" } });
                }
                /*MainPage = new ContentPage { Content = new Label { Text = $"Error: {ex.Message}" } };*/
            }
        }

    }
}
