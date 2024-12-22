using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Pages.Teacher;
using TP.Pages.Student;
using TP.Pages.Others;




namespace TP.Pages;

public partial class SubjectSelectionPage : ContentPage
{
    private readonly SQLiteAsyncConnection _database;
    public ObservableCollection<SubTable> Subjects { get; set; }
    public SubjectSelectionPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _database = new SQLiteAsyncConnection(dbPath);
        Subjects = new ObservableCollection<SubTable>();
        BindingContext = this;
        if(UserSession.UserType == 1){
        
        return;
        }
        if(UserSession.UserType == 2){
        return;
        }
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

        switch (UserSession.UserType)
        {
            case 1: // Teacher
                AddBtn.IsVisible = true;
                NoSubExist.IsVisible = false;
                var teacherSubjects = await _database.Table<SubTable>()
                                                     .Where(s => s.UserId == UserSession.UserId)
                                                     .ToListAsync();
                if (teacherSubjects.Count == 0)
                {
                    NoSubExist.IsVisible = true;
                    NoSubExistSubTitle.Text = " يمكنك اضافة مواد عن طريق الزر الموجود بالاعلى يمين";
                    return;
                }
                foreach (var subject in teacherSubjects)
                {
                    Subjects.Add(subject);
                }
                break;

            case 2: // Student
                SearchBtn.IsVisible = true;
                var stdInSub = await _database.Table<DegreeTable>()
                                               .Where(s => s.StdName == UserSession.Name)
                                               .ToListAsync();
                var allSubjects = await _database.Table<SubTable>().ToListAsync();

                foreach (var studentSubject in stdInSub)
                {
                    foreach (var subject in allSubjects)
                    {
                        if (studentSubject.SubId == subject.SubId)
                        {
                            Subjects.Add(subject);
                        }
                    }
                }

                if (Subjects.Count == 0)
                {
                    NoSubExist.IsVisible = true;
                    NoSubExistSubTitle.Text = "يمكنك انضمام للمواد عن طريق الزر الموجود بالاعلى يمين";
                }
                
                break;
        }

    }
    private async Task CheckSession()
    {
        if (UserSession.sessionyn) { 
            return;
        }
        SaveSession.IsVisible = false;
        var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();

        /*SaveSession.IsVisible = session == null;*/
        if (session == null)
        {
            SaveSession.IsVisible = true;
            UserSession.sessionyn = true;
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
    private void CancelSessionClicked(object sender, EventArgs e)
    {
        SaveSession.IsVisible = false;
    }

    private void AddClicked(object sender, EventArgs e){
        AddSubPopupWindow.IsVisible = true;
    }
    private async void SearchBtnClicked(object sender, EventArgs e){
        await Navigation.PushAsync(new RequestSubjectPage());
    }
    private void LogoutClicked(object sender, EventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
        }
    }

    private async void CreateSubClick(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SubNameEntry.Text)){
            await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }
        try{
           var Sub = new SubTable{
               SubName = SubNameEntry.Text,
               UserId = UserSession.UserId,
               SubTeacherName = UserSession.Name,
               ShowDeg = false,
               };
           await _database.InsertAsync(Sub);
           await DisplayAlert("تمت", "تم انشاء المادة بنجاح", "حسنا"); 
        }
        catch (Exception ex){
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        AddSubPopupWindow.IsVisible = false;
        await LoadSubjects();
    }
    private void CancelSubClick(object sender, EventArgs e)
    {
        
        AddSubPopupWindow.IsVisible = false;
    }
    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        // Get the selected item
        var selectedItem = e.CurrentSelection.FirstOrDefault() as SubTable;

        if (selectedItem != null)
        {
            if(UserSession.UserType == 1) {
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