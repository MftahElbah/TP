using Google.Android.Material.Snackbar;
using SQLite;
using TP.Methods;
using TP.Methods.actions;
namespace TP.Pages.Teacher;

public partial class SettingsForSub : ContentPage{
    public int SubId;
    Database database = Database.SelectedDatabase;


    public SettingsForSub(int id){
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
        SubId = id;


        HideContentViewMethod.HideContentView(PasswordPopup, PasswordPopupBorder);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSelectedSub();
    }
    
    private async Task LoadSelectedSub(){
        var sub = await database.getSubBySubId(SubId);
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
            Snackbar.ShowSnackbar(2, "يجب ملئ جميع الحقول");
            //await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }        
        try
        {
            var Sub = await database.getSubBySubId(SubId);
            if (Sub != null){
                Sub.SubName = Name;
                Sub.ShowDeg = ShowDegSwitch.IsToggled;
                await database.updateSubBySubId(Sub);
            }
            Snackbar.ShowSnackbar(1, "تم التعديل بنجاح");
            //await DisplayAlert("تم التعديل", "تم التعديل بنجاح", "حسنا");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Snackbar.ShowSnackbar(2, $"An error occurred: {ex.Message}");
            //await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private void DeleteButtonClicked(object sender, EventArgs e){
        PasswordPopup.IsVisible = true;
    }
    private void CancelDeleteClicked(object sender, EventArgs e)
    {
        PasswordPopup.IsVisible = false; // Hide the popup
    }
    /*private async void AgreeDeleteClicked(object sender, EventArgs e)
    {
        //string password = PasswordEntry.Text; // Retrieve entered password
        if (PasswordEntry.Text != UserSession.Password) { 
        await DisplayAlert("Success", "كلمة السر خطا", "OK");
            return;
        }
        await database.deleteSub(SubId); // Deletes the department from the database.
        *//*var agree = await database.getUserAccountById(UserSession.UserId);
        if (agree == null || string.IsNullOrEmpty(PasswordEntry.Text)) { return; }

        // Deletes all branches associated with the department.
       
        var SubStdToDelete = await database.getDegreeTablesBySubId(SubId);
        //var booksToDelete = await database.getSubjectBooksBySubId(SubId);
        var postsToDelete = await database.getSubjectPostsBySubId(SubId);
        var Sub = await database.getSubBySubId(SubId);
        foreach (var delete in SubStdToDelete)
        {
            await database.deleteDegree(delete); // Deletes the branches from the
                                                //
                                                //
                                                //
                                                // .
        }
        *//*foreach (var book in booksToDelete)
        {
            await database.deleteSubjectBook(book); // Deletes the branches from the
                                                   // .
        }*//*
        foreach (var post in postsToDelete)
        {
            await database.deletePost(post); // Deletes the branches from the database.
        }*//*
        await DisplayAlert("Success", "تمت الحذف بنجاح", "OK");

        if (Navigation?.NavigationStack?.Count > 2)
        {
            var secondLastPage = Navigation.NavigationStack[^2];
            Navigation.RemovePage(secondLastPage); // Removes the second last page
            await Navigation.PopAsync(); // Pops the last page
        }   
    }*/
    private async void AgreeDeleteClicked(object sender, EventArgs e)
    {
        //string password = PasswordEntry.Text; // Retrieve entered password
        if (PasswordEntry.Text != UserSession.Password) { 
        Snackbar.ShowSnackbar(2, "كلمة السر خطا");
        //await DisplayAlert("Success", "كلمة السر خطا", "OK");
            return;
        }
        await database.deleteSub(SubId); // Deletes the department from the database.
        
        Snackbar.ShowSnackbar(1, "تمت الحذف بنجاح");
        //await DisplayAlert("Success", "تمت الحذف بنجاح", "OK");

        if (Navigation?.NavigationStack?.Count > 2)
        {
            var secondLastPage = Navigation.NavigationStack[^2];
            Navigation.RemovePage(secondLastPage); // Removes the second last page
            await Navigation.PopAsync(); // Pops the last page
        }   
    }
}