namespace TP.Pages.Others;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
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