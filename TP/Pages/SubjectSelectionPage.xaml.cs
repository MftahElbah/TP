using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Pages.Level1;
using TP.Pages.Teacher;
using TP.Pages.Student;

namespace TP.Pages;

public partial class SubjectSelectionPage : ContentPage
{
    private readonly SQLiteAsyncConnection _database;
    public ObservableCollection<SubTable> Subjects { get; set; }
    public SubjectSelectionPage()
	{
		InitializeComponent();

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _database = new SQLiteAsyncConnection(dbPath);
        Subjects = new ObservableCollection<SubTable>();
        if(UserSession.UserType == 2){
        AddBtn.IsVisible = true;
        }
       
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSubjects();
    }
    private async Task LoadSubjects()
    {
            Subjects.Clear();
        if (UserSession.UserType == 2)
        {
            var subjects = await _database.Table<SubTable>().Where(s => s.UserId == UserSession.UserId).ToListAsync();
            foreach (var subject in subjects)
            {
                Subjects.Add(subject);
            }
        }
        if (UserSession.UserType == 3)
        {
            var stdinsub = await _database.Table<DegreeTable>().Where(s => s.StdName == UserSession.Name).ToListAsync();
            var subjects = await _database.Table<SubTable>().ToListAsync();
            foreach (var Stdinsub in stdinsub)
            {
                foreach (var subject in subjects)
                {
                    if(Stdinsub.SubId == subject.SubId)
                    Subjects.Add(subject);
                }
            }
        }
    }
    private async void AddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditSubject(null, null, null, null, null, 1));
    }
    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        // Get the selected item
        var selectedItem = e.CurrentSelection.FirstOrDefault() as SubTable;

        if (selectedItem != null)
        {
            if(UserSession.UserType == 2) {
                // Navigate to the detail page, passing the selected item's ID
                await Navigation.PushAsync(new SubjectCenterTeacher(selectedItem.SubId));
            }
            else { 
                await Navigation.PushAsync(new SubjectCenterStd(selectedItem.SubId, selectedItem.ShowDeg));
            }
            // Clear the selection (optional)
            var collectionView = sender as CollectionView;
            if (collectionView != null)
            {
                collectionView.SelectedItem = null;
            }
        }
    }

}