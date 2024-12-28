using SQLite;
using TP.Methods;
using TP.Methods.actions;
using TP.Pages;

namespace TP
{
    public partial class App : Application
    {
        private MineSQLite _sqlite = new MineSQLite();

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
                //to preinsert data
                var dbHelper = new DatabaseHelper(_sqlite.dbPath);
                await dbHelper.InitializeDatabaseAsync();
                await InitializeApp();
            }
            catch (Exception ex)
            {
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
                var session = await _sqlite.UserSessionChecker();
                if (session == null)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
                    }
                    return;
                }

                var user = await _sqlite.loginSecction(session.Password, session.UserId);
                                

                if (user == null)
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
