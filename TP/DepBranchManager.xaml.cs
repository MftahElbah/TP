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
        // Check if the selection has changed
        if (SelectionController != null && e.AddedItems.Count > 0)
        {
            // Get the first added item (the selected department)
            var selectedDepartment = e.AddedItems[0] as DepTable;

            if (selectedDepartment != null)
            {
                // Navigate to the EditDepBranch page and pass the selected department for editing
                await Navigation.PushAsync(new EditDepBranch(selectedDepartment));
            }
        }

        // Optionally clear the selection (to allow re-selection of the same item)
        // If you want to clear the selection, you may need to handle it differently based on your UI logic
    }
}