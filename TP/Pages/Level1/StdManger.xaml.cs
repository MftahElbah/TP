using Syncfusion.Maui.Buttons;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.Data;

using TP.Methods;

namespace TP.Pages.Level1;

public partial class StdManger : ContentPage
{
    public EditStdViewModel _viewModel;
    public StdManger()
	{
		InitializeComponent();
        _viewModel = new EditStdViewModel();
        BindingContext = _viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadStudentsAsync();
    }

    private async void StdGrid_SelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {


        if (StdGrid.SelectedRow != null)
        {
            // Access the selected row data
            var rowData = StdGrid.SelectedRow;

            // Extract the DepName property using reflection or dynamic binding
            var stdId = rowData?.GetType().GetProperty("StdhId")?.GetValue(rowData)?.ToString();
            var stdName = rowData?.GetType().GetProperty("StdName")?.GetValue(rowData)?.ToString();
            var stdBranch = rowData?.GetType().GetProperty("StdBranch")?.GetValue(rowData)?.ToString();
            var stdDep = rowData?.GetType().GetProperty("StdDep")?.GetValue(rowData)?.ToString();
            var stdClass = rowData?.GetType().GetProperty("StdClass")?.GetValue(rowData)?.ToString();
            

            if (stdId != null)
            {
                // Navigate to the EditDepBranch page, passing DepName as a parameter
                await Navigation.PushAsync(new EditStd(stdId, stdName, stdBranch, stdDep, stdClass, 2));
            }

            // Clear the selection
            StdGrid.SelectedRow = null;
        }
    }



    private async void AddClicked(object sender, EventArgs e)
    {
        // Navigate to the EditDepBranch page to add a new department
        await Navigation.PushAsync(new EditStd(null, null, null, null ,null,1)); // Pass null for a new department
    }
}