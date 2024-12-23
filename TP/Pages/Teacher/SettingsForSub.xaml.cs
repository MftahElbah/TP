using SQLite;
using TP.Methods;
namespace TP.Pages.Teacher;

public partial class SettingsForSub : ContentPage{
    public int SubId;
    public readonly SQLiteAsyncConnection _database;
    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    

    public SettingsForSub(int id){
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
        _database = new SQLiteAsyncConnection(dbPath);
        SubId = id;


        HideContentViewMethod.HideContentView(PasswordPopup);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSelectedSub();
    }
    
    private async Task LoadSelectedSub(){
        var sub = await _database.Table<SubTable>()
                                           .Where(b => b.SubId == SubId)
                                           .FirstOrDefaultAsync();
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
            var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == SubId);
            if (Sub != null){
                Sub.SubName = Name;
                Sub.ShowDeg = ShowDegSwitch.IsToggled;
                await _database.UpdateAsync(Sub);
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
        var agree = await _database.Table<UsersAccountTable>()
                                           .Where(b => b.UserId == UserSession.UserId && b.Password == password)
                                           .FirstOrDefaultAsync();
        if (agree == null || string.IsNullOrEmpty(PasswordEntry.Text)) { return; }

        // Deletes all branches associated with the department.
        var SubStdToDelete = await _database.Table<DegreeTable>().Where(b => b.SubId == SubId).ToListAsync();
        var booksToDelete = await _database.Table<SubjectBooks>().Where(b => b.SubId == SubId).ToListAsync();
        var postsToDelete = await _database.Table<SubjectPosts>().Where(b => b.SubId == SubId).ToListAsync();
        var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == SubId);
        foreach (var delete in SubStdToDelete)
        {
            await _database.DeleteAsync(delete); // Deletes the branches from the database.
        }
        foreach (var book in booksToDelete)
        {
            await _database.DeleteAsync(book); // Deletes the branches from the database.
        }
        foreach (var post in postsToDelete)
        {
            await _database.DeleteAsync(post); // Deletes the branches from the database.
        }
        await _database.DeleteAsync(Sub);
        await DisplayAlert("Success", "تمت الحذف بنجاح", "OK");

        if (Navigation?.NavigationStack?.Count > 2)
        {
            var secondLastPage = Navigation.NavigationStack[^2];
            Navigation.RemovePage(secondLastPage); // Removes the second last page
            await Navigation.PopAsync(); // Pops the last page
        }   
    }
}