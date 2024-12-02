using Syncfusion.Maui.Data;

namespace TP;

public partial class EditDepBranch : ContentPage
{
    private DepTable _department; // The department being edited (null for new)
    private readonly DatabaseHelper _databaseHelper;
    public string _DepName;
    public EditDepBranch(string depName)
	{
		InitializeComponent();
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath);
        _DepName = depName;
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
        if (string.IsNullOrWhiteSpace(departmentName))
        {
            await DisplayAlert("Error", "Please enter a valid department name.", "OK");
            return;
        }
        if (_DepName == null)
        {
            // Add a new department
            var newDepartment = new DepTable { DepName = departmentName };
            await _databaseHelper._database.InsertAsync(newDepartment);
            await DisplayAlert("Success", "Department added successfully!", "OK");
        }
        else
        {
            // Update an existing department
            _department.DepName = departmentName;
            await _databaseHelper._database.UpdateAsync(_department);
            await DisplayAlert("Success", "Department updated successfully!", "OK");
        }
        // Navigate back to the previous page
        await Navigation.PopAsync();
    }

    /*private async void ClosePopup(object sender, EventArgs e)
    {
        // Close the popup
        SuccessPopup.IsOpen = false;
        // Navigate back to the previous page
        await Navigation.PopAsync();
    }
*/
}