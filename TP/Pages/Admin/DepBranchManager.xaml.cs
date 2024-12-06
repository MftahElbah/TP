using Syncfusion.Maui.Buttons; // Provides button components for Syncfusion MAUI.
using Syncfusion.Maui.Data; // Enables data-handling features, such as grids.
using Syncfusion.Maui.ListView; // Provides functionality for list views.
using System.Xml.Linq; // Used for XML operations (not explicitly used here).
using TP.Methods; // Includes custom methods or utilities from the project.

namespace TP.Pages.Level1;

public partial class DepBranchManager : ContentPage // A page that manages departments and branches.
{
    private DepBranchViewModel _viewModel; // The ViewModel for managing UI data and business logic.
    public int CheckerNum = 1; // Tracks which grid is displayed: 1 for departments, 2 for branches.

    public DepBranchManager()
    {
        InitializeComponent(); // Initializes components defined in the XAML file.
        _viewModel = new DepBranchViewModel(); // Instantiates the ViewModel for data binding.
        BindingContext = _viewModel; // Sets the BindingContext to connect UI with the ViewModel.
        ChickWhichTableShow(CheckerNum); // Determines which grid to show on page load
    }

    // Event called when the page appears on the screen.
    protected override async void OnAppearing()
    {
        base.OnAppearing(); // Calls the base class implementation.
        await _viewModel.LoadData(); // Asynchronously loads data into the ViewModel.
    }

    // Event called when the "Add" button is clicked.
    private async void AddClicked(object sender, EventArgs e)
    {
        // Navigates to the EditDepBranch page to add a new department or branch.
        await Navigation.PushAsync(new EditDepBranch(null, null, null, CheckerNum));
    }

    // Event called when a row in the Department grid is selected.
    private async void DepartmentGrid_SelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (DepartmentGrid.SelectedRow == null) // Checks if a row is selected.
        {
            return;
        }

            var rowData = DepartmentGrid.SelectedRow; // Gets the selected row's data.

            // Retrieves properties using reflection.
            var depId = rowData?.GetType().GetProperty("DepId")?.GetValue(rowData)?.ToString();
            var depName = rowData?.GetType().GetProperty("DepName")?.GetValue(rowData)?.ToString();
            CheckerNum = 1; // Indicates department view.

            if (depId != null) // Checks if the ID is valid.
            {
                // Navigates to the EditDepBranch page to edit the selected department.
                await Navigation.PushAsync(new EditDepBranch(depId, depName, null, CheckerNum));
            }

            DepartmentGrid.SelectedRow = null; // Clears the grid's selection.
    }

    // Event called when a row in the Branch grid is selected.
    private async void BranchGrid_SelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (BranchGrid.SelectedRow == null) // Checks if a row is selected.
        {
            return;
        }
            var rowData = BranchGrid.SelectedRow; // Gets the selected row's data.

            // Retrieves properties using reflection.
            var branchId = rowData?.GetType().GetProperty("BranchId")?.GetValue(rowData)?.ToString();
            var branchName = rowData?.GetType().GetProperty("BranchName")?.GetValue(rowData)?.ToString();
            var depName = rowData?.GetType().GetProperty("DepName")?.GetValue(rowData)?.ToString();
            CheckerNum = 2; // Indicates branch view.

            if (branchId != null) // Checks if the ID is valid.
            {
                // Navigates to the EditDepBranch page to edit the selected branch.
                await Navigation.PushAsync(new EditDepBranch(branchId, branchName, depName, CheckerNum));
            }

            BranchGrid.SelectedRow = null; // Clears the grid's selection.
    }

    // Event called when the "Show Departments" button is clicked.
    private void DepDataGridShowerClicked(object sender, EventArgs e)
    {
        CheckerNum = 1; // Sets view to department grid.
        ChickWhichTableShow(CheckerNum); // Updates the UI to show the department grid.
    }

    // Event called when the "Show Branches" button is clicked.
    private void BranchDataGridShowerClicked(object sender, EventArgs e)
    {
        CheckerNum = 2; // Sets view to branch grid.
        ChickWhichTableShow(CheckerNum); // Updates the UI to show the branch grid.
    }

    // Updates UI to show either the department grid or branch grid.
    private void ChickWhichTableShow(int Ch)
    {
        if (Ch == 1) // If showing departments.
        {
            DepDataGridShower.Background = Color.FromArgb("#2374AB"); // Highlights the department button.
            DepDataGridShower.TextColor = Color.FromArgb("#DCDCDC"); // Sets department button text color.

            BranchDataGridShower.Background = Colors.Transparent; // Clears branch button highlight.
            BranchDataGridShower.TextColor = Color.FromArgb("#1A1A1A"); // Resets branch button text color.

            DepartmentGrid.IsVisible = true; // Displays the department grid.
            BranchGrid.IsVisible = false; // Hides the branch grid.
        }
        else if (Ch == 2) // If showing branches.
        {
            BranchDataGridShower.Background = Color.FromArgb("#2374AB"); // Highlights the branch button.
            BranchDataGridShower.TextColor = Color.FromArgb("#DCDCDC"); // Sets branch button text color.

            DepDataGridShower.Background = Colors.Transparent; // Clears department button highlight.
            DepDataGridShower.TextColor = Color.FromArgb("#1A1A1A"); // Resets department button text color.

            DepartmentGrid.IsVisible = false; // Hides the department grid.
            BranchGrid.IsVisible = true; // Displays the branch grid.
        }
    }
}
