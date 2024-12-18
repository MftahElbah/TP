using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Teacher;

public partial class AssignmentsPage : ContentPage
{
    public ObservableCollection<SubjectAssignments> AssignmentsForListView { get; set; }
    private readonly SQLiteAsyncConnection _database;
    public int postid;
    public AssignmentsPage(int pid)
	{
        InitializeComponent();
        postid = pid;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _database = new SQLiteAsyncConnection(dbPath);
        AssignmentsForListView = new ObservableCollection<SubjectAssignments>();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fetch all subjects from the SubTable
        await LoadAvailableAssignments();
    }

    private async Task LoadAvailableAssignments()
    {
        var assignments = await _database.Table<SubjectAssignments>()
            .Where(a=> a.PostId == postid)
            .ToListAsync();

        AssignmentsForListView.Clear();
        foreach (var assignment in assignments)
        {
            AssignmentsForListView.Add(assignment);
        }
    }

    private async void DownloadClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is SubjectAssignments assignment)
        {
            try
            {
                // Assuming AssignmentFile contains the file content in binary or a path to the file
                byte[] fileData = assignment.AssignmentFile; // Replace with actual logic to get file content
                string fileName = $"Assignment_{assignment.StdName}{assignment.FileType}"; // Adjust extension as needed

                // Use FilePicker or local storage to save the file
                string localPath = PlatformFileHelper.GetDownloadsPath(fileName);
                // Save the file
                File.WriteAllBytes(localPath, fileData);

                await DisplayAlert("Download Complete", $"File saved at {localPath}", "OK");

                // Optionally, open the file after saving
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(localPath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to download file: {ex.Message}", "OK");
            }
        }
    }
}