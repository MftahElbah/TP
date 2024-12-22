using Android.App;
using Android.Content.PM;
using Android.OS;

namespace TP
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Change the status bar color
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#1a1a1a")); // Replace with your color
        }
    }

}
