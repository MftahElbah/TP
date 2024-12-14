using SQLite;
using TP.Methods;
using TP.Pages.Others;
using TP.Pages.Student;
using TP.Pages.Teacher;

namespace TP
{
    public partial class App : Application
    {
        public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        public readonly SQLiteAsyncConnection _database;
        public App(){
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf1FpR2NGfV5ycEVFal1WTnRbUiweQnxTdEFiW35dcHdUQWJYUUB3WQ==");
            
            InitializeComponent();
            _database=new SQLiteAsyncConnection(dbPath);
            MainPage = new ContentPage();
            /*
             * how to convert from MainPage = new ContentPage();
            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new StartPage());
            }
        */
        }
        protected override async void OnStart(){
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
            var dbHelper = new DatabaseHelper(dbPath);
            await dbHelper.InitializeDatabaseAsync();
            await InitializeApp();
            }
        private async Task InitializeApp()
        {
            try
            {
                
                var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();

                if (session == null)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new StartPage());
                    }
                    return;
                }

                var user = await _database.Table<UsersAccountTable>()
                                           .FirstOrDefaultAsync(u => u.UserId == session.UserId && u.Password == session.Password);

                if (user == null)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new StartPage());
                    }
                    return;
                }
                UserSession.UserId = user.UserId;
                UserSession.Name = user.Name;
                UserSession.UserType = user.UserType;
                UserSession.Password = user.Password;

                if (UserSession.UserType == 2)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new TeacherAppShell());
                    }
                }
                else
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new StudentShell());
                    }
                }
                
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., logging)
                MainPage = new ContentPage { Content = new Label { Text = $"Error: {ex.Message}" } };
            }
        }

    }
}
