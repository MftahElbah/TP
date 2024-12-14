using TP.Pages.Others;

namespace TP.Pages.Student;

public partial class StudentShell : Shell
{
	public StudentShell()
	{
		InitializeComponent();
	}
    private void OnLogoutClicked(object sender, EventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new StartPage());
        }
    }
}