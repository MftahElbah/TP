using SQLite;
using Syncfusion.Maui.PullToRefresh;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Methods.actions;

namespace TP.Pages.Student;

public partial class RequestSubjectPage : ContentPage
{
    private MineSQLite _sqlite = new MineSQLite();

    public ObservableCollection<SubTable> Subjects { get; set; }
    public RequestSubjectPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable nAavigation bar for this page


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
        var subjects = await _sqlite.getSubTable();
        
        Subjects.Clear();
        foreach (var subject in subjects)
        { 
            var SubInReq = await _sqlite.getRequestJoinBySubIdAndUserId(subject.SubId);
            var StdInTable = await _sqlite.getDegreeTableBySubIdAndStdName(subject.SubId);
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
            var subjects = await _sqlite.getSubTable();
            Subjects.Clear();

            foreach (var subject in subjects)       
            {
                var subInReq = await _sqlite.getRequestJoinBySubIdAndUserId(subject.SubId);
                var stdInTable = await _sqlite.getDegreeTableBySubIdAndStdName(subject.SubId);  

                if (subInReq.Count == 0 && stdInTable.Count == 0)
                {
                    Subjects.Add(subject);
                }
            }
            EmptyMessage.IsVisible = false;
            return;
        }

        // Search for subjects with names matching the search text
        var filteredSubjects = await _sqlite.searchSubTable(searchText);
        Subjects.Clear();
        if (filteredSubjects.Count == 0) {
            EmptyMessage.IsVisible = true;
            return;
        }
        EmptyMessage.IsVisible = false;
        foreach (var subject in filteredSubjects)
        {
            var subInReq = await _sqlite.getRequestJoinBySubIdAndUserId(subject.SubId);
            var stdInTable = await _sqlite.getDegreeTableBySubIdAndStdName(subject.SubId);


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
            // Create a new request and insert it into the
            // 




            var request = new RequestJoinSubject
            {
                UserId = UserSession.UserId, // Assuming UserSession.UserId is set
                SubId = subject.SubId,
                Name = UserSession.Name,
                RequestDate = DateTime.Now,
            };
            await _sqlite.insertRequestJoin(request);
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