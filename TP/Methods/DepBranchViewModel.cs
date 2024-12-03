using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TP.Methods
{

    public class DepBranchViewModel : INotifyPropertyChanged
    {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        
        private readonly DatabaseHelper _databaseHelper;
        private ObservableCollection<DepTable> _departments;
        private ObservableCollection<BranchTable> _branches;

        public DepBranchViewModel()
        {
            _databaseHelper = new DatabaseHelper(dbPath);
         

            InitializeAsync();

            // Initialize the database helper with the path to your SQLite database
            Task.Run(async () =>
            {
                await _databaseHelper.InitializeAsync();
                await LoadData();
            });
            RefreshCommand = new Command(async () => await RefreshData()); // Initialize RefreshCommand
            
        }

        private async Task InitializeAsync()
        {
            await _databaseHelper.InitializeDatabaseAsync();
            // Load data or perform additional tasks
        }

        public ObservableCollection<DepTable> Departments
        {
            get => _departments;
            set
            {
                _departments = value;
                OnPropertyChanged(nameof(Departments));
            }
        }



        public ObservableCollection<BranchTable> Branches
        {
            get => _branches;
            set
            {
                _branches = value;
                OnPropertyChanged(nameof(Branches));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand RefreshCommand { get; } // Add RefreshCommand property
        public async Task LoadData()
        {
            await RefreshData(); // Load data initially
        }

        public async Task RefreshData()

        {

            var departments = await _databaseHelper.GetDepartmentsAsync();

            var branches = await _databaseHelper.GetBranchesAsync();


            Departments = new ObservableCollection<DepTable>(departments);

            Branches = new ObservableCollection<BranchTable>(branches);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*private DepTable _selectedDepartment;
        public DepTable SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));
            }
        }*/
    }
}
