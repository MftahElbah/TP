using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Level1;
public partial class EditSubject : ContentPage
{
    public ObservableCollection<int> Classes { get; set; }
    public ObservableCollection<string> DepNames { get; set; }
    public ObservableCollection<string> BranchesName { get; set; }
    
    /*public EditStdViewModel _viewModel = new EditStdViewModel ();*/

    public string ids;

    public readonly SQLiteAsyncConnection _database;

    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

    public EditSubject(string id,string subname , string dep , string branch , string classnum,int gt)
    {
        InitializeComponent();
        ids = id;
        _database = new SQLiteAsyncConnection(dbPath);

        
        Classes = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        //BindingContext = this;
        ClassComboBox.ItemsSource = Classes;
        if (string.IsNullOrEmpty(id)) {
            return;
        }
            NameEntry.Text = subname;
            DepartmentComboBox.SelectedItem = dep;
            LoadBranchData(dep);
            BranchComboBox.SelectedItem = branch;
            ClassComboBox.SelectedItem = classnum;
            ChickWhichViewShow(gt);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDepartmentsAsync();
        
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

            if (ids == null)
            {

                var Sub = new SubTable
                {
                    SubName = NameEntry.Text,
                    SubDep = dep.DepId,
                    SubBranch = branch.BranchId,
                    SubClass = selectedClass,
                    ShowDeg = false,
                    UserId = UserSession.UserId,
                    SubTeacher = UserSession.Name,
                };
                await _database.InsertAsync(Sub);
                await DisplayAlert("Success", "Subject added successfully.", "OK");
            }
            else
            {
                int idnum = int.Parse(ids);
                var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == idnum);
                if (Sub != null)
                {
                    Sub.SubName = NameEntry.Text;
                    Sub.SubDep = dep.DepId;
                    Sub.SubBranch = branch.BranchId;
                    Sub.SubClass = selectedClass;
                    await _database.UpdateAsync(Sub);
                }
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        int subid = int.Parse(ids);
        var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == subid);
        if (Sub == null)
        {
            await DisplayAlert("Success", "حدث خطاء", "OK");
            return;
        }
            await _database.DeleteAsync(Sub);
            await DisplayAlert("Success", "تمت الحذف بنجاح", "OK");
            await Navigation.PopAsync();
    }

    private void DepComboBoxSelectionChanged(object sender, EventArgs e)
    {
        LoadBranchData(DepartmentComboBox.Text);
    }

    private async void LoadBranchData(string depname)
    {
        if (string.IsNullOrWhiteSpace(depname)){return;}
        try{
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
    private void ChickWhichViewShow(int Ch)
    {
        //1 to insert , 2 to update
        if (Ch == 1)
        {
            DeleteButton.IsVisible = false;
        }
        else if (Ch == 2)
        {
            DeleteButton.IsVisible = true;  
        }
    }

}
