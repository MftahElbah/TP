using SQLite;
using Syncfusion.Maui.PullToRefresh;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Student;

public partial class RequestSubjectPage : ContentPage
{
    public ObservableCollection<SubTable> Subjects { get; set; }
    private readonly SQLiteAsyncConnection _database;
    public RequestSubjectPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable nAavigation bar for this page


        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _database = new SQLiteAsyncConnection(dbPath);
        Subjects = new ObservableCollection<SubTable>();
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fetch all subjects from the SubTable
        await LoadAvailableSubjects();
    }

    private async Task LoadAvailableSubjects()
    {
        var subjects = await _database.Table<SubTable>().ToListAsync();
        
        Subjects.Clear();
        foreach (var subject in subjects)
        { 
            var SubInReq = await _database.Table<RequestJoinSubject>().Where(s => s.SubId == subject.SubId && s.UserId == UserSession.UserId).ToListAsync();
            var StdInTable = await _database.Table<DegreeTable>().Where(s => s.SubId == subject.SubId && s.StdName == UserSession.Name).ToListAsync();
            if (SubInReq.Count == 0 && StdInTable.Count == 0) {
                Subjects.Add(subject);
            }
        }
    }
    private async void BackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    
    private async void SearchEntryChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.Trim().ToLower();

        if (string.IsNullOrEmpty(searchText))
        {
            // Load all subjects with filtering logic
            var subjects = await _database.Table<SubTable>().ToListAsync();
            Subjects.Clear();

            foreach (var subject in subjects)
            {
                var subInReq = await _database.Table<RequestJoinSubject>()
                                              .Where(s => s.SubId == subject.SubId && s.UserId == UserSession.UserId)
                                              .ToListAsync();
                var stdInTable = await _database.Table<DegreeTable>()
                                                .Where(s => s.SubId == subject.SubId && s.StdName == UserSession.Name)
                                                .ToListAsync();

                if (subInReq.Count == 0 && stdInTable.Count == 0)
                {
                    Subjects.Add(subject);
                }
            }
            EmptyMessage.IsVisible = false;
            return;
        }
        
        // Search for subjects with names matching the search text
        var filteredSubjects = await _database.Table<SubTable>()
                                              .Where(s => s.SubName.Contains(searchText) || s.SubTeacherName.Contains(searchText))
                                              .ToListAsync();
        Subjects.Clear();
        if (filteredSubjects.Count == 0) {
            EmptyMessage.IsVisible = true;
            return;
        }
        EmptyMessage.IsVisible = false;
        foreach (var subject in filteredSubjects)
        {
            var subInReq = await _database.Table<RequestJoinSubject>()
                                              .Where(s => s.SubId == subject.SubId && s.UserId == UserSession.UserId)
                                              .ToListAsync();
            var stdInTable = await _database.Table<DegreeTable>()
                                            .Where(s => s.SubId == subject.SubId && s.StdName == UserSession.Name)
                                            .ToListAsync();

            if (subInReq.Count == 0 && stdInTable.Count == 0)
            {
                Subjects.Add(subject);
            }
        }
        
    }

    private async void OnSendRequestClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var subject = button.BindingContext as SubTable;
        if(button.Text == "تم الأرسال")
        {
            return;
        }
        if (subject != null)
        {
            // Create a new request and insert it into the database
            var request = new RequestJoinSubject
            {
                UserId = UserSession.UserId, // Assuming UserSession.UserId is set
                SubId = subject.SubId,
                Name = UserSession.Name,
                RequestDate = DateTime.Now,
            };
            await _database.InsertAsync(request);
            button.Text = "تم الأرسال";
            button.BackgroundColor = Colors.Gray;
        }
    }
    private async void OnPullToRefreshRefreshing(object sender, EventArgs args)
    {
        pulltorefresh.IsRefreshing = true;
        await Task.Delay(2000);
        await LoadAvailableSubjects();
        pulltorefresh.IsRefreshing = false;
    }
}