using Syncfusion.Maui.Buttons;
using Syncfusion.Maui.Data;
using Syncfusion.Maui.ListView;
using System.Xml.Linq;
using TP.ViewModels;

namespace TP;

public partial class DepBranchManager : ContentPage
{
    private DepBranchViewModel _viewModel;
    public int CheckerNum = 1;

    public DepBranchManager()
    {
        InitializeComponent();
        _viewModel = new DepBranchViewModel();
        BindingContext = _viewModel;
        ChickWhichTableShow(CheckerNum);
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
        await Navigation.PushAsync(new EditDepBranch(null, null,null, 0)); // Pass null for a new department
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
            var depId = rowData?.GetType().GetProperty("DepId")?.GetValue(rowData)?.ToString();
            var depName = rowData?.GetType().GetProperty("DepName")?.GetValue(rowData)?.ToString();
            CheckerNum = 1;
            if (depId != null)
            {
                // Navigate to the EditDepBranch page, passing DepName as a parameter
                await Navigation.PushAsync(new EditDepBranch(depId, depName,null, CheckerNum));
            }

            // Clear the selection
            DepartmentGrid.SelectedRow = null;
        }
    }
    private async void BranchGrid_SelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (BranchGrid.SelectedRow != null)
        {
            // Access the selected row data
            var rowData = BranchGrid.SelectedRow;

            // Extract the DepName property using reflection or dynamic binding
            var depId = rowData?.GetType().GetProperty("BranchId")?.GetValue(rowData)?.ToString();
            var branchName = rowData?.GetType().GetProperty("BranchName")?.GetValue(rowData)?.ToString();
            var depName = rowData?.GetType().GetProperty("DepName")?.GetValue(rowData)?.ToString();
            CheckerNum = 2;

            if (depId != null)
            {
                // Navigate to the EditDepBranch page, passing DepName as a parameter
                await Navigation.PushAsync(new EditDepBranch(depId, depName, branchName, CheckerNum));
            }

            // Clear the selection
            DepartmentGrid.SelectedRow = null;
        }
    }

    private async void DepDataGridShowerClicked(object sender, EventArgs e)
    {
        CheckerNum = 1;
        ChickWhichTableShow(CheckerNum);
    }
    private async void BranchDataGridShowerClicked(object sender, EventArgs e)
    {
        CheckerNum= 2;
        ChickWhichTableShow(CheckerNum);
    }

    private void ChickWhichTableShow(int Ch)
    {
        
        if (Ch == 1)
        {
            DepDataGridShower.Background = Color.FromArgb("#2374AB");
            DepDataGridShower.TextColor = Color.FromArgb("#DCDCDC");


            BranchDataGridShower.Background = Colors.Transparent;
            BranchDataGridShower.TextColor = Color.FromArgb("#1A1A1A");

            DepartmentGrid.IsVisible = true;
            BranchGrid.IsVisible = false;
        }
        else if (Ch == 2)
        {
            BranchDataGridShower.Background = Color.FromArgb("#2374AB");
            BranchDataGridShower.TextColor = Color.FromArgb("#DCDCDC");


            DepDataGridShower.Background = Colors.Transparent;
            DepDataGridShower.TextColor = Color.FromArgb("#1A1A1A");

            DepartmentGrid.IsVisible = false;
            BranchGrid.IsVisible = true;
        }
    }
}