using Syncfusion.Maui.Data;

namespace TP;

public partial class EditDepBranch : ContentPage
{
    private DepTable _department; // The department being edited (null for new)
    private readonly DatabaseHelper _databaseHelper;
    public string _DepName;
    public string _DepId;
    public EditDepBranch(string depId,string depName)
	{
		InitializeComponent();
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath);
        _DepName = depName;
        _DepId = depId;
        // Set up the page based on whether it's adding or editing
        if (_DepName != null)
        {
            // Editing an existing department
            Title = "Edit Department";
            NameEntry.Text = depName;

        }
        else
        {
            // Adding a new department
            Title = "Add Department";
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        string departmentName = NameEntry.Text;

        // Validate the input
        if (string.IsNullOrWhiteSpace(departmentName))
        {
            await DisplayAlert("Error", "Please enter a valid department name.", "OK");
            return;
        }

        try
        {
            if (_DepId == null)
            {
                // Add a new department
                var newDepartment = new DepTable { DepName = departmentName };
                await _databaseHelper._database.InsertAsync(newDepartment);
                await DisplayAlert("Success", "Department added successfully!", "OK");
            }
            else
            {
                // Update an existing department using UpdateDepartmentAsync
                await _databaseHelper.UpdateDepartmentAsync(int.Parse(_DepId), departmentName);
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
}