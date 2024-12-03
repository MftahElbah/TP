using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Provides a dynamic collection that updates UI when data changes.
using System.ComponentModel; // Supports data binding and notification of property changes.
using System.Linq; // Enables LINQ queries.
using System.Text;
using System.Threading.Tasks; // Supports asynchronous programming.
using System.Windows.Input; // Provides command functionality for MVVM.

namespace TP.Methods
{
    public class DepBranchViewModel : INotifyPropertyChanged // Implements INotifyPropertyChanged for data binding.
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        // Defines the database file path in the local application data directory.

        private readonly DatabaseHelper _databaseHelper; // Database helper for handling SQLite operations.
        private ObservableCollection<DepTable> _departments; // Observable collection of departments.
        private ObservableCollection<BranchTable> _branches; // Observable collection of branches.

        public DepBranchViewModel()
        {
            _databaseHelper = new DatabaseHelper(dbPath); // Initializes the database helper with the database path.

            InitializeAsync(); // Ensures database setup or additional initialization tasks.

            // Asynchronously initialize the database and load data in a background task.
            Task.Run(async () =>
            {
                await _databaseHelper.InitializeAsync(); // Initializes the database (tables, etc.).
                await LoadData(); // Loads data into the ViewModel properties.
            });

            RefreshCommand = new Command(async () => await RefreshData()); // Initializes RefreshCommand with an action to reload data.
        }

        private async Task InitializeAsync()
        {
            await _databaseHelper.InitializeDatabaseAsync(); // Ensures the database schema is ready.
            // Additional initialization tasks, if needed.
        }

        // Observable collection for departments with notification for UI updates.
        public ObservableCollection<DepTable> Departments
        {
            get => _departments;
            set
            {
                _departments = value; // Sets the new value.
                OnPropertyChanged(nameof(Departments)); // Notifies the UI of changes.
            }
        }

        // Observable collection for branches with notification for UI updates.
        public ObservableCollection<BranchTable> Branches
        {
            get => _branches;
            set
            {
                _branches = value; // Sets the new value.
                OnPropertyChanged(nameof(Branches)); // Notifies the UI of changes.
            }
        }

        public ICommand AddCommand { get; } // Command for adding items (not implemented here).
        public ICommand RefreshCommand { get; } // Command to reload data.

        // Loads data into the ViewModel, typically called on initialization.
        public async Task LoadData()
        {
            await RefreshData(); // Calls RefreshData to populate collections.
        }

        // Refreshes data from the database and updates collections.
        public async Task RefreshData()
        {
            var departments = await _databaseHelper.GetDepartmentsAsync(); // Fetches department data.
            var branches = await _databaseHelper.GetBranchesAsync(); // Fetches branch data.

            // Updates observable collections with new data.
            Departments = new ObservableCollection<DepTable>(departments);
            Branches = new ObservableCollection<BranchTable>(branches);
        }

        public event PropertyChangedEventHandler PropertyChanged; // Event for property change notifications.

        // Notifies the UI when a property value changes.
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
