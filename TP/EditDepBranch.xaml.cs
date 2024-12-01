using Syncfusion.Maui.Data;

namespace TP;

public partial class EditDepBranch : ContentPage
{
    private DepTable _department; // The department being edited (null for new)
    private readonly DatabaseHelper _databaseHelper;
    public EditDepBranch(DepTable department)
	{
		InitializeComponent();
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath);
        // Set up the page based on whether it's adding or editing
        if (_department != null)
        {
            // Editing an existing department
            Title = "Edit Department";
            NameEntry.Text = _department.DepName;
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
        if (_department == null)
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

    private async void ClosePopup(object sender, EventArgs e)
    {
        // Close the popup
        SuccessPopup.IsOpen = false;
        // Navigate back to the previous page
        await Navigation.PopAsync();
    }

}