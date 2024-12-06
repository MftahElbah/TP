using SQLite;

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

    private async void LoginBtnClicked(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlert("خطا", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        var IfUserExist = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username == username && d.Password == password);
        if (IfUserExist == null)
        {
            await DisplayAlert("خطاء", "هناك خطاء في اسم المستخدم أو كلمة المرور", "حسنا");
            return;
        }
        await DisplayAlert("suc", "im dead", "حسنا");
    }

}