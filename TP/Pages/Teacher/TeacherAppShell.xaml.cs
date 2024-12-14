using TP.Pages.Others;

namespace TP.Pages.Teacher;

public partial class TeacherAppShell : Shell
{
	public TeacherAppShell()
	{
		InitializeComponent();
	}

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new StartPage());
        }
        /*Application.Current.MainPage = new NavigationPage(new StartPage());*/
    }
}