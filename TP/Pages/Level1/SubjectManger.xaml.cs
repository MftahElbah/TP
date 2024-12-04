using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Level1;

public partial class SubjectManger : ContentPage
{
    private DatabaseHelper _databaseHelper;
    private ObservableCollection<SubTableView> _subTableView;
    public ObservableCollection<SubTableView> SubTableView
    {
        get => _subTableView;
        set
        {
            _subTableView = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }
    }
    public readonly SQLiteAsyncConnection _database;

    public SubjectManger()
    {
        InitializeComponent();
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath); // Pass your database path
        _subTableView = new ObservableCollection<SubTableView>();
        _database = new SQLiteAsyncConnection(dbPath);
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ReloadData();
    }

    private async Task ReloadData()
    {
        var subTableData = await _databaseHelper.GetSubTableViewAsync();
        if (subTableData != null && subTableData.Any())
        {
            SubTableView = new ObservableCollection<SubTableView>(subTableData);
        }
        else
        {
            SubTableView.Clear(); // Clear the collection if no data is available
            lbl.Text = "No data available";
        }
    }

    private async void SubTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {


        if (SubTableDataGrid.SelectedRow != null)
        {
            // Access the selected row data
            var rowData = SubTableDataGrid.SelectedRow;

            // Extract the DepName property using reflection or dynamic binding
            var subId = rowData?.GetType().GetProperty("SubId")?.GetValue(rowData)?.ToString();
            var subName = rowData?.GetType().GetProperty("SubName")?.GetValue(rowData)?.ToString();
            var depName = rowData?.GetType().GetProperty("DepName")?.GetValue(rowData)?.ToString();
            var branchName = rowData?.GetType().GetProperty("BranchName")?.GetValue(rowData)?.ToString();
            var SubClass = rowData?.GetType().GetProperty("SubClass")?.GetValue(rowData)?.ToString();


            if (subId != null)
            {
                // Navigate to the EditDepBranch page, passing DepName as a parameter
                await Navigation.PushAsync(new EditSubject(subId, subName, depName, branchName, SubClass, 2));
            }

            // Clear the selection
            SubTableDataGrid.SelectedRow = null;
        }
    }

    private async void AddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditSubject(null, null, null, null,null,1));
    }
}