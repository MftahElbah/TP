using SQLite;
using Syncfusion.Maui.Data;
using System.Collections.ObjectModel;
using TP.Methods;
namespace TP;

public partial class testpage : ContentPage
{

    public testpage()
    {
        InitializeComponent();

    }



}
/*using SQLite;
using Syncfusion.Maui.Data;
using System.Collections.ObjectModel;
using TP.Methods;
namespace TP;

public partial class testpage : ContentPage
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

    public testpage()
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
        var subTableData = await _databaseHelper.GetSubTableViewAsync();
        if (subTableData != null && subTableData.Any())
        {
            SubTableView = new ObservableCollection<SubTableView>(subTableData);
        }
        else
        {
            // Handle the case where no data is returned.
            lbl.Text = "error";
        }
    }

    private async void AddBtnClicked(object sender, EventArgs e)
    {

    }


}
*/

/*
//How combobox works

using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
namespace TP;

public partial class testpage : ContentPage
{
    //1.define connection string and sqldb var
    public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    public readonly SQLiteAsyncConnection _database;

    //public ObservableCollection<string> DepNames { get; set; }
    public testpage()
    {
        //2.make sqldb var = new connection (con string)
        _database = new SQLiteAsyncConnection(dbPath);
        InitializeComponent();
        //4.call this function
        LoadDepartmentsAsync();

        DepartmentComboBox.SelectedItem = "قسم الكهرباء";

    }

    //
    private async Task LoadDepartmentsAsync()
    {
        //3.then do this down

        // Fetch departments from the database
        var deps = await _database.Table<DepTable>().ToListAsync();

        // Populate DepNames with department names
        var DepNames = new ObservableCollection<string>(deps.Select(dep => dep.DepName));

        // Bind it to the ComboBox
        DepartmentComboBox.ItemsSource = DepNames;
    }
}*/


/*
 * ----------------------------------------------------------------------
 * thats for grid how it works
 * 
 * using TP.Methods;
namespace TP;

public partial class testpage : ContentPage
{
    private EditStdViewModel _viewModel;
    public testpage()
	{
	
        InitializeComponent();
        _viewModel = new EditStdViewModel();
        BindingContext = _viewModel;

    }
}
------------------------------------------------------------------------------------
 
 */