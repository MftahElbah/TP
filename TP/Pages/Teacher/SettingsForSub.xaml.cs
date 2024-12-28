using SQLite;
using TP.Methods;
using TP.Methods.actions;
namespace TP.Pages.Teacher;

public partial class SettingsForSub : ContentPage{
    public int SubId;
    private MineSQLite _sqlite = new MineSQLite();


    public SettingsForSub(int id){
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
        SubId = id;


        HideContentViewMethod.HideContentView(PasswordPopup);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSelectedSub();
    }
    
    private async Task LoadSelectedSub(){
        var sub = _sqlite.getSubBySubId(SubId).Result;
            NameEntry.Text = sub.SubName;
            ShowDegSwitch.IsToggled = sub.ShowDeg;
    }
    private async void BackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();

    }
    
    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        string Name = NameEntry.Text.ToLower();

        if (string.IsNullOrWhiteSpace(Name))
        {
            await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }        
        try
        {
            var Sub = _sqlite.getSubBySubId(SubId).Result;
            if (Sub != null){
                Sub.SubName = Name;
                Sub.ShowDeg = ShowDegSwitch.IsToggled;
                await _sqlite.updateSubBySubId(Sub);
            }
            await DisplayAlert("تم التعديل", "تم التعديل بنجاح", "حسنا");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private void DeleteButtonClicked(object sender, EventArgs e){
        PasswordPopup.IsVisible = true;
    }
    private void CancelDeleteClicked(object sender, EventArgs e)
    {
        PasswordPopup.IsVisible = false; // Hide the popup
    }
    private async void AgreeDeleteClicked(object sender, EventArgs e)
    {
        string password = PasswordEntry.Text; // Retrieve entered password
        var agree = _sqlite.getUserAccountById(UserSession.UserId).Result;
        if (agree == null || string.IsNullOrEmpty(PasswordEntry.Text)) { return; }

        // Deletes all branches associated with the department.
       
        var SubStdToDelete = _sqlite.getDegreeTablesBySubId(SubId).Result;
        var booksToDelete = _sqlite.getSubjectBooksBySubId(SubId).Result;
        var postsToDelete = _sqlite.getSubjectPostsBySubId(SubId).Result;
        var Sub = _sqlite.getSubBySubId(SubId).Result;
        foreach (var delete in SubStdToDelete)
        {
            await _sqlite.deleteDegree(delete); // Deletes the branches from the
                                                //
                                                //
                                                //
                                                // .
        }
        foreach (var book in booksToDelete)
        {
            await _sqlite.deleteSubjectBook(book); // Deletes the branches from the
                                                   // .
        }
        foreach (var post in postsToDelete)
        {
            await _sqlite.deletePost(post); // Deletes the branches from the database.
        }
        await _sqlite.deleteSub(Sub); // Deletes the department from the database.
        await DisplayAlert("Success", "تمت الحذف بنجاح", "OK");

        if (Navigation?.NavigationStack?.Count > 2)
        {
            var secondLastPage = Navigation.NavigationStack[^2];
            Navigation.RemovePage(secondLastPage); // Removes the second last page
            await Navigation.PopAsync(); // Pops the last page
        }   
    }
}