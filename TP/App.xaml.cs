using SQLite;
using TP.ViewModels;

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
            MainPage = new NavigationPage(new DepBranchManager());
        }
    }
}
