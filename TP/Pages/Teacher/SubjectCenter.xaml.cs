using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Teacher;

public partial class SubjectCenter : ContentPage
{
    private DatabaseHelper _databaseHelper;
    /*private ObservableCollection<SubForStdTable> suby;*/
    private ObservableCollection<DegreeTable> DegreeTableGetter;
    public ObservableCollection<DegreeTable> DegreeTableSetter
    {
        get => DegreeTableGetter;
        set
        {
            DegreeTableGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }
    }
    public int SubId;
    public readonly SQLiteAsyncConnection _database;
    public SubjectCenter(int subid)
	{
		InitializeComponent();
        SubId = subid;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath); // Pass your database path
        _database = new SQLiteAsyncConnection(dbPath);
        DegreeTableGetter = new ObservableCollection<DegreeTable>();
        /*suby = new ObservableCollection<SubForStdTable>();*/
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
    }

    private async Task LoadData()
    {
        var degreeTableData = await _database.Table<DegreeTable>().Where(s => s.SubId == SubId).ToListAsync();
        if (degreeTableData != null)
        {
            DegreeTableSetter = new ObservableCollection<DegreeTable>(degreeTableData);
        }
    }

    /*private async Task LoadData()
    {
        var degreeTableViews = await _databaseHelper.GetDegreeTableViewAsync();
        if (degreeTableViews != null && degreeTableViews.Any())
        {
            var filteredData = degreeTableViews.Where(d => d.StdName == SubNames).ToList();
            if (filteredData != null && filteredData.Any())
            {
                DegreeTableViewSetter = new ObservableCollection<DegreeTableView>(degreeTableViews);
            }
            else
            {
                DegreeTableViewSetter.Clear(); // Clear the collection if no relevant data is found
            }
        }
        else
        {
            DegreeTableViewSetter.Clear(); // Clear the collection if no data is available
        }
    }*/

    private async void DegreeTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (DegreeTableDataGrid.SelectedRow == null)
        {
            return;
        }
        var DataRow = DegreeTableDataGrid.SelectedRow;

        var stdName = DataRow?.GetType().GetProperty("StdName")?.GetValue(DataRow)?.ToString();
        var degree = DataRow?.GetType().GetProperty("Deg")?.GetValue(DataRow)?.ToString();
        var midDegree = DataRow?.GetType().GetProperty("MidDeg")?.GetValue(DataRow)?.ToString();
        var total = DataRow?.GetType().GetProperty("Total")?.GetValue(DataRow)?.ToString();
        if (stdName != null)
        {
            // Navigate to the EditDepBranch page, passing DepName as a parameter
            /*await Navigation.PushAsync(new EditSubject(subId, subName, depName, branchName, SubClass, 2));*/
        }
        DegreeTableDataGrid.SelectedRow = null;
    }
}