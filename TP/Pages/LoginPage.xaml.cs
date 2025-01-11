using SQLite;
using TP.Methods;
using TP.Methods.actions;
namespace TP.Pages;

public partial class LoginPage : ContentPage
{   
    Database database = Database.SelectedDatabase;
    public LoginPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DeleteSession();
        if (!UserSession.internet)
        {
            NointernetSnackbar.ShowSnackbar(3, "لا يوجد اتصال بالإنترنت!");
        }
    }
    //delete session if user logout from the account


    private async Task DeleteSession()
    {
        await database.deleteSession();
    }

    private async void LoginBtnClicked(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            Snackbar.ShowSnackbar(2, "يجب ملئ جميع الحقول");
            //await DisplayAlert("خطا", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }
        string username = UsernameEntry.Text.ToLower();
        string password = PasswordEntry.Text;
        var IfUserExist = await database.UserLoginChecker(username, password);
        if (IfUserExist == null)
        {
            Snackbar.ShowSnackbar(2, "هناك خطاء في اسم المستخدم أو كلمة المرور");
            //await DisplayAlert("خطاء", "هناك خطاء في اسم المستخدم أو كلمة المرور", "حسنا");
            return;
        }
        if (!IfUserExist.IsActive)
        {
            Snackbar.ShowSnackbar(2, "هذا الحساب غير فعال, راجع قسم التسجيل");
            //await DisplayAlert("خطاء", "هذا الحساب غير فعال, راجع قسم التسجيل", "حسنا");
            return;
        }
        UserSession.UserId = IfUserExist.UserId;
        UserSession.Name = IfUserExist.Name;
        UserSession.Password = IfUserExist.Password;
        UserSession.UserType = IfUserExist.UserType;
        UserSession.SessionYesNo = false;
        if (Application.Current?.Windows.Count > 0){
            Application.Current.Windows[0].Page = new NavigationPage(new SubjectSelectionPage());
            }
    }
}