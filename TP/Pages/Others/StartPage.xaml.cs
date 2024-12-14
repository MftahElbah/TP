using SQLite;

namespace TP.Pages.Others;

public partial class StartPage : ContentPage
{
    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    public readonly SQLiteAsyncConnection _database;
    public StartPage()
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

    private async void OpenLoginPageBtnClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new LoginPage());
    }
	private async void OpenSignupPageBtnClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new SignupPage());
    }
}