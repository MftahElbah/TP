using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TP.Methods
{
    public class EditStdViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseHelper _databaseHelper;
        private ObservableCollection<DepTable> _departments;
        private ObservableCollection<BranchTable> _branches;
        private ObservableCollection<int> _classes;

        public EditStdViewModel()
        {
            _databaseHelper = new DatabaseHelper("YourDatabaseName.db");
            LoadData();
        }

        public ObservableCollection<DepTable> Departments
        {
            get => _departments;
            set { _departments = value; OnPropertyChanged(nameof(Departments)); }
        }

        public ObservableCollection<BranchTable> Branches
        {
            get => _branches;
            set { _branches = value; OnPropertyChanged(nameof(Branches)); }
        }

        public ObservableCollection<int> Classes
        {
            get => _classes;
            set { _classes = value; OnPropertyChanged(nameof(Classes)); }
        }

        private StdTable _currentStudent;
        public StdTable CurrentStudent
        {
            get => _currentStudent;
            set { _currentStudent = value; OnPropertyChanged(nameof(CurrentStudent)); }
        }

        public async Task LoadData()
        {
            Departments = new ObservableCollection<DepTable>(await _databaseHelper.GetDepartmentsAsync());
            Branches = new ObservableCollection<BranchTable>(await _databaseHelper.GetBranchesAsync());
            Classes = new ObservableCollection<int>(new[] { 1, 2, 3, 4 ,5 ,6 ,7 ,8}); // Example class levels
        }

        public async Task SaveStudentAsync()
        {
            if (CurrentStudent.StdId == 0)
            {
                await _databaseHelper.AddStudentAsync(CurrentStudent);
            }
            else
            {
                await _databaseHelper.UpdateStudentAsync(CurrentStudent);
            }
        }

        public async Task DeleteStudentAsync()
        {
            if (CurrentStudent != null)
            {
                await _databaseHelper.DeleteStudentAsync(CurrentStudent.StdId);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
