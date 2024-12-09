using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Teacher;

public partial class SubjectCenter : ContentPage
{
    private DatabaseHelper _databaseHelper;
    /*private ObservableCollection<SubForStdTable> suby;*/
    private ObservableCollection<DegreeTableView> DegreeTableViewGetter;
    public ObservableCollection<DegreeTableView> DegreeTableView
    {
        get => DegreeTableViewGetter;
        set
        {
            DegreeTableViewGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }
    }
    public string SubNames;
    public readonly SQLiteAsyncConnection _database;
    public SubjectCenter(string subname)
	{
		InitializeComponent();
        SubNames = subname;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath); // Pass your database path
        _database = new SQLiteAsyncConnection(dbPath);
        DegreeTableViewGetter = new ObservableCollection<DegreeTableView>();
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
        var degreeTableViews = await _databaseHelper.GetDegreeTableViewAsync(SubNames);
        if (degreeTableViews != null)
        {
            DegreeTableView = new ObservableCollection<DegreeTableView>(degreeTableViews);
        }

        /*var t = await _database.Table<SubForStdTable>().ToListAsync();
        foreach (var ts in t)
        {


            suby.Add(ts);
            
        }*/

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
        var degree = DataRow?.GetType().GetProperty("Degree")?.GetValue(DataRow)?.ToString();
        var midDegree = DataRow?.GetType().GetProperty("MidDegree")?.GetValue(DataRow)?.ToString();
        var total = DataRow?.GetType().GetProperty("Total")?.GetValue(DataRow)?.ToString();
        if (stdName != null)
        {
            // Navigate to the EditDepBranch page, passing DepName as a parameter
            /*await Navigation.PushAsync(new EditSubject(subId, subName, depName, branchName, SubClass, 2));*/
        }
        DegreeTableDataGrid.SelectedRow = null;
    }
}