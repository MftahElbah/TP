using SQLite;
using TP.Methods;
using TP.Pages.Level1;

namespace TP
{
    public partial class App : Application
    {
        
        public App()
        {
            //Register Syncfusion license https://help.syncfusion.com/common/essential-studio/licensing/how-to-generate
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR LICENSE KEY");

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzYwMzI0N0AzMjM3MmUzMDJlMzBYMEdaSXZDSTFZR3Vwd2NoRkloaWNQeWVEMlRxMDgyMTRsQzhQdnR4THI0PQ==");
            
            InitializeComponent();
            //MainPage = new NavigationPage(new DepBranchManager());
            MainPage = new NavigationPage(new StdManger());
            //MainPage = new NavigationPage(new testpage());
        }

        protected override async void OnStart()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
            var dbHelper = new DatabaseHelper(dbPath);
            await dbHelper.InitializeDatabaseAsync();
            // Proceed with other initialization
        }

    }
}
