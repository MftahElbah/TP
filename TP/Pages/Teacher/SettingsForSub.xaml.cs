using SQLite;
using Syncfusion.Maui.Data;
using System.Collections.ObjectModel;
using TP.Methods;


namespace TP.Pages.Teacher;

public partial class SettingsForSub : ContentPage
{
	public int SubId;
    public ObservableCollection<string> DepNames { get; set; }
    public ObservableCollection<string> BranchesName { get; set; }
    public ObservableCollection<int> Classes { get; set; }
    public readonly SQLiteAsyncConnection _database;
    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

    public SettingsForSub(int id)
	{
		InitializeComponent();

        _database = new SQLiteAsyncConnection(dbPath);
        SubId = id;

        Classes = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        ClassComboBox.ItemsSource = Classes;
       
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDepartmentsAsync();
        await LoadSelectedSub();
    }
    private async Task LoadDepartmentsAsync()
    {
        // Fetch departments from the database
        var deps = await _database.Table<DepTable>().ToListAsync();

        // Populate DepNames with department names
        DepNames = new ObservableCollection<string>(deps.Select(dep => dep.DepName));

        // Bind it to the ComboBox
        DepartmentComboBox.ItemsSource = DepNames;
    }
    private async Task LoadSelectedSub()
    {
        var sub = await _database.Table<SubTable>()
                                           .Where(b => b.SubId == SubId)
                                           .FirstOrDefaultAsync();
        var dep = await _database.Table<DepTable>()
                                           .Where(b => b.DepId == sub.SubDep)
                                           .FirstOrDefaultAsync();
        var bran = await _database.Table<BranchTable>()
                                           .Where(b => b.BranchId == sub.SubBranch)
                                           .FirstOrDefaultAsync();
        if (sub != null)
        {
            NameEntry.Text = sub.SubName;
            DepartmentComboBox.SelectedItem = dep.DepName;
            BranchComboBox.SelectedItem = bran.BranchName;
            ClassComboBox.SelectedItem = sub.SubClass.ToString();
            ShowDegSwitch.IsOn = sub.ShowDeg;
        }
    }
    private void DepComboBoxSelectionChanged(object sender, EventArgs e)
    {
        LoadBranchData(DepartmentComboBox.Text);
    }
    private async void LoadBranchData(string depname)
    {
        if (string.IsNullOrWhiteSpace(depname)) { return; }
        try
        {
            //string DepNameToShearch = depname;
            // Fetch branches based on selected department
            var branches = await _database.Table<BranchTable>()
                                           .Where(b => b.DepName == depname)
                                           .ToListAsync();

            // Populate BranchesName with branch names
            BranchesName = new ObservableCollection<string>(
                branches.Select(branch => branch.BranchName)
            );

            // Bind it to the ComboBox
            BranchComboBox.ItemsSource = BranchesName;
        }
        catch (Exception ex)
        {
            // Log or handle any exceptions
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private void DeleteButtonClicked(object sender, EventArgs e){
        PasswordPopup.IsVisible = true;
        PasswordPopup.BackgroundColor = Color.FromRgba(0,0,0,0.5);
    }
    private void CancelButtonClicked(object sender, EventArgs e)
    {
        PasswordPopup.IsVisible = false; // Hide the popup
    }
    private async void AgreeButtonClicked(object sender, EventArgs e){
        string password = PasswordEntry.Text; // Retrieve entered password
        
        if (string.IsNullOrEmpty(PasswordEntry.Text)) { return; }
        string pass = PasswordEntry.Text;
        var agree = await _database.Table<UsersAccountTable>()
                                           .Where(b => b.UserId == UserSession.UserId && b.Password == pass)
                                           .FirstOrDefaultAsync();
        if (agree == null) { return; }

        // Deletes all branches associated with the department.
        var SubStdToDelete = await _database.Table<DegreeTable>().Where(b => b.SubId == SubId).ToListAsync();
        foreach (var delete in SubStdToDelete)
        {
            await _database.DeleteAsync(delete); // Deletes the branches from the database.
        }
        var booksToDelete = await _database.Table<SubjectBooks>().Where(b => b.SubId == SubId).ToListAsync();
        foreach (var book in booksToDelete)
        {
            await _database.DeleteAsync(book); // Deletes the branches from the database.
        }
        var postsToDelete = await _database.Table<SubjectPosts>().Where(b => b.SubId == SubId).ToListAsync();
        foreach (var post in postsToDelete)
        {
            await _database.DeleteAsync(post); // Deletes the branches from the database.
        }
        var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == SubId);
        await _database.DeleteAsync(Sub);
        await DisplayAlert("Success", "تمت الحذف بنجاح", "OK");
        await Navigation.PushAsync(new SubjectSelectionPage());
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        string depname = DepartmentComboBox.Text;
        string branchname = BranchComboBox.Text;

        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(depname) || string.IsNullOrWhiteSpace(branchname))
        {
            await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }

        if (!int.TryParse(ClassComboBox.Text, out int selectedClass))
        {
            await DisplayAlert("Error", "Invalid class number.", "OK");
            return;
        }

        try
        {
            var dep = await _database.Table<DepTable>().FirstOrDefaultAsync(d => d.DepName == depname);
            var branch = await _database.Table<BranchTable>().FirstOrDefaultAsync(b => b.BranchName == branchname);

            if (dep == null || branch == null)
            {
                await DisplayAlert("Error", "Invalid department or branch.", "OK");
                return;
            }

                var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == SubId);
                if (Sub != null)
                {
                    Sub.SubName = NameEntry.Text;
                    Sub.SubDep = dep.DepId;
                    Sub.SubBranch = branch.BranchId;
                    Sub.SubClass = selectedClass;
                    Sub.ShowDeg = ShowDegSwitch.IsOn.Value;
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
}