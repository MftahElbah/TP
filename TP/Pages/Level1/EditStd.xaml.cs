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

    public string _StdId;
    public string _StdName;
    public string _Dep;
    public string _Branch;
    public string _Class;
    public int _GetType;



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
            DepartmentComboBox.Text = dep;
            BranchComboBox.Text = branch;
            ClassComboBox.Text = classnum;
            ChickWhichViewShow(gt);
        }
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
        await _viewModel.SaveStudentAsync(1);
        await DisplayAlert("Success", "تمت اضافة الطالب بنجاح", "OK");
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.DeleteStudentAsync();
    }

    private async void DepComboBoxSelectionChanged(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(DepartmentComboBox.Text))
            {

                string DepNameToShearch = DepartmentComboBox.Text;
                // Fetch branches based on selected department
                var branches = await _database.Table<BranchTable>()
                                               .Where(b => b.DepName == DepNameToShearch)
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

        }
        else if (Ch == 2)
        {
            IdEntry.IsEnabled = false;
           
        }
    }

}
