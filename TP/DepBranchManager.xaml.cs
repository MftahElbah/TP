using Syncfusion.Maui.Data;
using Syncfusion.Maui.ListView;
using TP.ViewModels;

namespace TP;

public partial class DepBranchManager : ContentPage
{
    private DepBranchViewModel _viewModel;

    public DepBranchManager()
    {
        InitializeComponent();
        _viewModel = new DepBranchViewModel();
        BindingContext = _viewModel;
    }

    // This method is called when the page appears
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadData(); // Ensure data is loaded before the UI is updated
    }

    // This method is called when the user clicks the floating action button (+)
    private async void AddClicked(object sender, EventArgs e)
    {
        // Navigate to the EditDepBranch page to add a new department
        await Navigation.PushAsync(new EditDepBranch(null)); // Pass null for a new department
    }

    // This method is called when the user selects a department in the DataGrid
    private async void DepartmentGrid_SelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        // Check if there is a selected item in the grid
        if (DepartmentGrid.SelectedRow != null)
        {
            // Access the selected row data
            var rowData = DepartmentGrid.SelectedRow;

            // Extract the DepName property using reflection or dynamic binding
            var depName = rowData?.GetType().GetProperty("DepName")?.GetValue(rowData)?.ToString();

            if (!string.IsNullOrEmpty(depName))
            {
                // Navigate to the EditDepBranch page, passing DepName as a parameter
                await Navigation.PushAsync(new EditDepBranch(depName));
            }

            // Clear the selection
            DepartmentGrid.SelectedRow = null;
        }
    }
    private async void BranchGrid_SelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {

    }


}