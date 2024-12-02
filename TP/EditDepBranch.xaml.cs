using Syncfusion.Maui.Data;
using System.Collections.ObjectModel;

namespace TP;

public partial class EditDepBranch : ContentPage
{
    
    private readonly DatabaseHelper _databaseHelper;
    public string _Id;
    public string _Name1;
    public string _Name2;
    public int _TypeDataGrid;
    public ObservableCollection<string> Departments { get; set; }
    public string SelectedDepName { get; set; }


    public EditDepBranch(string Id,string Name1,string Name2, int TypeDataGrid)
	{
		InitializeComponent();

        BindingContext = this;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath);

        Departments = new ObservableCollection<string>();
        LoadDepartments();

        _Id = Id;
        _Name1 = Name1;
        _Name2 = Name2;
        _TypeDataGrid = TypeDataGrid;

        ChickWhichViewShow(TypeDataGrid);

        // Set up the page based on whether it's adding or editing
        if (_Id != null)
        {
            // Editing an existing department
            Title = "تعديل";
            NameEntry.Text = Name1;
            EnableButtons(false);

        }
        else
        {
            // Adding a new department
            Title = "اضافة";
            EnableButtons(true);
        }


    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        string departmentName = NameEntry.Text;

        // Validate the input
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert("Error", "Please enter a valid department name.", "OK");
            return;
        }

        try
        {
            if (_Id == null)
            {
                // Add a new department
                var newDepartment = new DepTable { DepName = NameEntry.Text };
                await _databaseHelper._database.InsertAsync(newDepartment);
                await DisplayAlert("Success", "Department added successfully!", "OK");
            }
            else
            {
                // Update an existing department using UpdateDepartmentAsync
                await _databaseHelper.UpdateDepartmentAsync(int.Parse(_Id), NameEntry.Text);
                await DisplayAlert("Success", "Department updated successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

        // Navigate back to the previous page
        await Navigation.PopAsync();
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_Id))
        {
            await DisplayAlert("Error", "Please enter a valid department name.", "OK");
            
        }
        else
        {
            await _databaseHelper.DeleteDepartmentAsync(int.Parse(_Id));
            await DisplayAlert("Success", "Deleted successfully!", "OK");
            await Navigation.PopAsync();
        }
    }

    private async void DepViewShowerClicked(object sender, EventArgs e)
    {
        _TypeDataGrid = 1;
        ChickWhichViewShow(_TypeDataGrid);
    }
    private async void BranchViewShowerClicked(object sender, EventArgs e)
    {
        _TypeDataGrid = 2;
        ChickWhichViewShow(_TypeDataGrid);
    }

    private void EnableButtons(bool enable) 
    {
        if (!enable)
        {
            DepViewShower.IsEnabled = false;
            BranchViewShower.IsEnabled = false;
        }
        else
        {
            DepViewShower.IsEnabled = true;
            BranchViewShower.IsEnabled = true;
        }
    }
    private void ChickWhichViewShow(int Ch)
    {

       if (Ch == 1)
        {
            DepViewShower.Background = Color.FromArgb("#2374AB");
            DepViewShower.TextColor = Color.FromArgb("#DCDCDC");


            BranchViewShower.Background = Colors.Transparent;
            BranchViewShower.TextColor = Color.FromArgb("#1A1A1A");

        }
        else if (Ch == 2)
        {
            BranchViewShower.Background = Color.FromArgb("#2374AB");
            BranchViewShower.TextColor = Color.FromArgb("#DCDCDC");


            DepViewShower.Background = Colors.Transparent;
            DepViewShower.TextColor = Color.FromArgb("#1A1A1A");

        }
    }

    private async void LoadDepartments()
    {
        var departments = await _databaseHelper._database.Table<DepTable>().ToListAsync();
        foreach (var department in departments)
        {
            Departments.Add(department.DepName);
        }
    }

}