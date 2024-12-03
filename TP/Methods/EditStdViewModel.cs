using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TP.Methods
{
    public class EditStdViewModel : INotifyPropertyChanged
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

        private readonly DatabaseHelper _databaseHelper;
        private ObservableCollection<DepTable> _departments;
        private ObservableCollection<BranchTable> _branches;
        private ObservableCollection<int> _classes;
        private ObservableCollection<StdTable> _students;
        public EditStdViewModel()
        {
            _databaseHelper = new DatabaseHelper(dbPath);
            LoadData();
            LoadStudentsAsync();
        }

        public async Task LoadStudentsAsync()
        {
            var studentList = await _databaseHelper.GetStudentsAsync();
            Students = new ObservableCollection<StdTable>(studentList);
        }

        public ObservableCollection<StdTable> Students
        {
            get => _students;
            set { _students = value; OnPropertyChanged(nameof(Students)); }
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
            Students = new ObservableCollection<StdTable>(await _databaseHelper.GetStudentsAsync());
        }

        public async Task SaveStudentAsync(int GetType)
        {
            if (GetType == 1)
            {
                await _databaseHelper.AddStudentAsync(CurrentStudent);

            }
            else if (GetType == 2) 
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
