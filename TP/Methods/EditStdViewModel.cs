using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TP.Methods
{
    public class EditStdViewModel : INotifyPropertyChanged
    {
        // Path to the SQLite database file.
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

        private readonly DatabaseHelper _databaseHelper; // Database helper for performing database operations.
        private ObservableCollection<DepTable> _departments; // Collection of departments.
        private ObservableCollection<BranchTable> _branches; // Collection of branches.
        private ObservableCollection<int> _classes; // Collection of class levels (e.g., 1st year, 2nd year).
        private ObservableCollection<StdTable> _students; // Collection of students.

        public EditStdViewModel()
        {
            _databaseHelper = new DatabaseHelper(dbPath); // Initialize the database helper.
            //LoadData(); // Loads departments, branches, and students data.
            Task.Run(async () =>
            {
                await InitializeAsync(); // Initializes the database (tables, etc.).
                await LoadData(); // Loads data into the ViewModel properties.
            });
        }

        // Asynchronously loads the list of students from the database.


        private async Task InitializeAsync()
        {
            await _databaseHelper.InitializeDatabaseAsync(); // Ensures the database schema is ready.
            // Additional initialization tasks, if needed.
        }

        // Observable collection for students.
        public ObservableCollection<StdTable> Students
        {
            get => _students;
            set { _students = value; OnPropertyChanged(nameof(Students)); }
        }

        // Observable collection for departments.
        public ObservableCollection<DepTable> Departments
        {
            get => _departments;
            set { _departments = value; OnPropertyChanged(nameof(Departments)); }
        }

        // Observable collection for branches.
        public ObservableCollection<BranchTable> Branches
        {
            get => _branches;
            set { _branches = value; OnPropertyChanged(nameof(Branches)); }
        }

        // Observable collection for class levels.
        public ObservableCollection<int> Classes
        {
            get => _classes;
            set { _classes = value; OnPropertyChanged(nameof(Classes)); }
        }

        // The currently selected student.
        private StdTable _currentStudent;
        public StdTable CurrentStudent
        {
            get => _currentStudent;
            set { _currentStudent = value; OnPropertyChanged(nameof(CurrentStudent)); }
        }

        // Loads departments, branches, and students from the database asynchronously.
        public async Task LoadData()
        {
            Students = new ObservableCollection<StdTable>(await _databaseHelper.GetStudentsAsync());
            Departments = new ObservableCollection<DepTable>(await _databaseHelper.GetDepartmentsAsync());
            Branches = new ObservableCollection<BranchTable>(await _databaseHelper.GetBranchesAsync());
        }


        // PropertyChanged event for notifying changes in the properties.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
