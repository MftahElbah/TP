using SQLite;
using Syncfusion.Maui.PullToRefresh;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Methods.actions;

namespace TP.Pages.Student;

public partial class RequestSubjectPage : ContentPage
{
    Database database = Database.SelectedDatabase;

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
        try
        {
            var subjects = await database.getSubTable() ?? new List<SubTable>();

            // Log the count of subjects for debugging
            Console.WriteLine($"Total subjects fetched: {subjects.Count}");

            Subjects.Clear();

            // Filter out null entries in the subjects list
            foreach (var subject in subjects.Where(s => s != null))
            {
                // Check if there are any requests or degrees associated with the subject
                var SubInReq = await database.getRequestJoinBySubIdAndUserId(subject.SubId);
                var StdInTable = await database.getDegreeTableBySubIdAndStdName(subject.SubId);

                if ((SubInReq?.Count ?? 0) == 0 && (StdInTable?.Count ?? 0) == 0)
                {
                    Subjects.Add(subject);
                }
            }

            if (Subjects.Count == 0) {
                EmptyMessage.IsVisible = true;
                NoExistTitle.Text = "لا توجد مواد";
                NoExistSubTitle.Text = "لا توجد مواد في الوقت الحالي";
            }

            // Bind the filtered subjects to the UI
            list.ItemsSource = Subjects;

            // Log the count of available subjects
            Console.WriteLine($"Available subjects added: {Subjects.Count}");
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Error in LoadAvailableSubjects: {ex.Message}");
        }
    }

    private async void BackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    
    private async void SearchEntryChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower();

        if (string.IsNullOrEmpty(searchText))
        {
            // Load all subjects with filtering logic
            await LoadAvailableSubjects();
            EmptyMessage.IsVisible = false;

        }

        // Search for subjects with names matching the search text
        var filteredSubjects = await database.searchSubTable(searchText);
        Subjects.Clear();
        if (filteredSubjects.Count == 0) {
            EmptyMessage.IsVisible = true;
            NoExistTitle.Text = "غير متوفر";
            NoExistSubTitle.Text = "لالمادة او الاستاذ الذي تبحث عليه غير متوفر";
            return;
        }
        EmptyMessage.IsVisible = false;
        foreach (var subject in filteredSubjects)
        {
            var subInReq = await database.getRequestJoinBySubIdAndUserId(subject.SubId);
            var stdInTable = await database.getDegreeTableBySubIdAndStdName(subject.SubId);


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
            await database.insertRequestJoin(request);
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