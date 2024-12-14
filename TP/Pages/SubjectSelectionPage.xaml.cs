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
        await Task.Delay(1000);
        await CheckSession();

    }
    private async Task LoadSubjects()
    {
            Subjects.Clear();
        if (UserSession.UserType == 2)
        {
            NoSubExist.IsVisible = false;
            var subjects = await _database.Table<SubTable>().Where(s => s.UserId == UserSession.UserId).ToListAsync();
            if(subjects.Count == 0)
            {
                NoSubExist.IsVisible = true;
                NoSubExistSubTitle.Text = " يمكنك اضافة مواد عن طريق علامة \"+\" ";
            }
            foreach (var subject in subjects)
            {
                Subjects.Add(subject);
            }
        }
        if (UserSession.UserType == 3)
        {
            AddBtn.IsVisible = false;
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
            // Check if the Subjects collection is empty
            if (Subjects.Count == 0)
            {
                // Show no-subjects message and update subtitle text
                NoSubExist.IsVisible = true;
                NoSubExistSubTitle.Text = "يمكنك اضافة مواد عن طريق \"انضمام لمادة جديدة\"";
            }
            else
            {
                // Hide the no-subjects message if subjects are available
                NoSubExist.IsVisible = false;
            }
        }
    }
    private async Task CheckSession()
    {
        SaveSession.IsVisible = false;
        var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();

        /*SaveSession.IsVisible = session == null;*/
        if (session == null)
        {
            SaveSession.IsVisible = true;
            return;
        }
    }
    private async void SaveSessionClicked(object sender, EventArgs e)
    {
        var existingSession = await _database.Table<UserSessionTable>()
            .FirstOrDefaultAsync(s => s.UserId == UserSession.UserId);

        if (existingSession == null)
        {
            var session = new UserSessionTable
            {
                UserId = UserSession.UserId,
                Password = UserSession.Password,
            };
            await _database.InsertAsync(session);
        }

        SaveSession.IsVisible = false;
    }
    private void CancelClicked(object sender, EventArgs e)
    {
        SaveSession.IsVisible = false;
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