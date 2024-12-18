using SQLite;
using TP.Methods;
using TP.Pages.Student;
using TP.Pages.Teacher;

namespace TP.Pages.Others;

public partial class LoginPage : ContentPage
{
    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    public readonly SQLiteAsyncConnection _database;
    public LoginPage()
	{
		InitializeComponent();
        _database = new SQLiteAsyncConnection(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DeleteSession();
    }

    private async Task DeleteSession()
    {
        var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
        if (session != null)
        {
            await _database.DeleteAsync(session);
        }
    }

    private async void LoginBtnClicked(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlert("خطا", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }
        string username = UsernameEntry.Text.ToLower();
        string password = PasswordEntry.Text;
        var IfUserExist = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username == username && d.Password == password);
        if (IfUserExist == null)
        {
            await DisplayAlert("خطاء", "هناك خطاء في اسم المستخدم أو كلمة المرور", "حسنا");
            return;
        }
        UserSession.UserId = IfUserExist.UserId;
        UserSession.Name = IfUserExist.Name;
        UserSession.Password = IfUserExist.Password;
        UserSession.UserType = IfUserExist.UserType;
        UserSession.sessionyn = false;
        if (UserSession.UserType == 3)
        {
            /*await Application.Current.MainPage.Navigation.PushAsync(new NavigationPage(new StudentShell()));
            Application.Current.MainPage = new NavigationPage(new StudentShell());
*/

            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new StudentShell());
            }

            return;
        }
        if (UserSession.UserType == 2)
        {
            /*await Application.Current.MainPage.Navigation.PushAsync(new NavigationPage(new TeacherAppShell()));*/

            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new TeacherAppShell());
            }
            return;
        }
        await DisplayAlert("suc", "im dead", "حسنا");
    }

}