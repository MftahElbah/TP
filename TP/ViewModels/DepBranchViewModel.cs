using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TP.ViewModels
{

    public class DepBranchViewModel : INotifyPropertyChanged
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        private readonly DatabaseHelper _databaseHelper;
        private ObservableCollection<DepTable> _departments;
        private ObservableCollection<BranchTable> _branches;

        public DepBranchViewModel()
        {
            // Initialize the database helper with the path to your SQLite database
            _databaseHelper = new DatabaseHelper(dbPath);
            AddCommand = new Command(async () => await AddNewItem());
            RefreshCommand = new Command(async () => await RefreshData()); // Initialize RefreshCommand
            LoadData();
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

        private async Task AddNewItem()
        {
            // Logic to add a new department or branch
            // You can show a dialog or navigate to another page for input
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
