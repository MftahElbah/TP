using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Level1;
public partial class EditStd : ContentPage
{
    public ObservableCollection<int> Classes { get; set; }
    public ObservableCollection<string> DepNames { get; set; }
    public ObservableCollection<string> BranchesName { get; set; }
    
    public EditStdViewModel _viewModel = new EditStdViewModel ();

    public readonly SQLiteAsyncConnection _database;


    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

    public EditStd(string id,string stdname , string dep , string branch , string classnum,int gt)
    {
        InitializeComponent();

        _database = new SQLiteAsyncConnection(dbPath);

        LoadDepartmentsAsync();
        Classes = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        //BindingContext = this;
        ClassComboBox.ItemsSource = Classes;

        if (!string.IsNullOrEmpty(id)) {
            IdEntry.Text = id;
            NameEntry.Text = stdname;
            DepartmentComboBox.SelectedItem = dep;
            LoadBranchData(dep);
            BranchComboBox.SelectedItem = branch;
            ClassComboBox.SelectedItem = classnum;
        }
            ChickWhichViewShow(gt);
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
        if (IdEntry.IsEnabled) { 
        var Std = new StdTable { StdId = int.Parse(IdEntry.Text),StdName = NameEntry.Text,StdDep=DepartmentComboBox.Text,StdBranch = BranchComboBox.Text , StdClass=int.Parse(ClassComboBox.Text)};
        await _database.InsertAsync(Std);
        await DisplayAlert("Success", "تمت اضافة الطالب بنجاح", "OK");
        }
        else {
            int stdid = int.Parse(IdEntry.Text);
            var Std = await _database.Table<StdTable>().FirstOrDefaultAsync(d => d.StdId == stdid);
            Std.StdName = NameEntry.Text;
            Std.StdDep = DepartmentComboBox.Text;
            Std.StdBranch = BranchComboBox.Text;
            Std.StdClass=int.Parse(ClassComboBox.Text);

            await _database.UpdateAsync(Std);
        }
        await Navigation.PopAsync();
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        int stdid = int.Parse(IdEntry.Text);
        var Std = await _database.Table<StdTable>().FirstOrDefaultAsync(d => d.StdId == stdid);
        if (Std != null)
        {
            await _database.DeleteAsync(Std);
        await DisplayAlert("Success", "نيك", "OK");
            await Navigation.PopAsync();
        }
        else
        {
        await DisplayAlert("Success", "موت", "OK");

        }
        
    }

    private async void DepComboBoxSelectionChanged(object sender, EventArgs e)
    {
        LoadBranchData(DepartmentComboBox.Text);
    }

    private async void LoadBranchData(string depname)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(depname))
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
            IdEntry.IsEnabled = true;
            DeleteButton.IsVisible = false;

        }
        else if (Ch == 2)
        {
            IdEntry.IsEnabled = false;
            DeleteButton.IsVisible = true;
           
        }
    }

}
