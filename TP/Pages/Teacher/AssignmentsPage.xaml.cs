using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Methods.actions;

namespace TP.Pages.Teacher;

public partial class AssignmentsPage : ContentPage
{
    
    Database database = Database.SelectedDatabase;
    private MineSQLite sqLite = new MineSQLite() ;
    public ObservableCollection<SubjectAssignments> AssignmentsForListView { get; set; }
    public int postid;
    public AssignmentsPage(int pid)
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
        postid = pid;
        AssignmentsForListView = new ObservableCollection<SubjectAssignments>();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fetch all subjects from the SubTable
        await LoadAvailableAssignments();
    }

    private async void BackClicked(object sender, EventArgs e){
        await Navigation.PopAsync();
    }

    private async Task LoadAvailableAssignments()
    {
        var assignments =  await database.getSubjectAssignmentsByPost(postid);

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
                // Use the file path stored in the database
                string filePath = assignment.AssignmentFile;

                // Check if the file exists
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    await DisplayAlert("File Not Found", "The file does not exist at the specified path.", "OK");
                    return;
                }

                // Open the file directly
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to open file: {ex.Message}", "OK");
            }
        }
    }



    /*    private async Task<bool> CheckAndRequestWritePermissionAsync()
        {
    #if ANDROID
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q) // Before Android 10
            {
                var permissionStatus = AndroidX.Core.Content.ContextCompat.CheckSelfPermission(Android.App.Application.Context, Android.Manifest.Permission.WriteExternalStorage);
                if (permissionStatus != Android.Content.PM.Permission.Granted)
                {
                    AndroidX.Core.App.ActivityCompat.RequestPermissions(MainActivity.Instance, new[] { Android.Manifest.Permission.WriteExternalStorage }, 1);
                    return await Task.FromResult(false); // Assume not granted for simplicity
                }
            }
            return true;
    #else
            return await Task.FromResult(true);

    #endif
        } */



    /*private async void DownloadClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is SubjectAssignments assignment)
        {
            try
            {
                // Check and request write permission
                var hasPermission = await CheckAndRequestWritePermissionAsync();
                if (!hasPermission)
                {
                    await DisplayAlert("Permission Denied", "Write permission is required to download the file.", "OK");
                    return;
                }

                // File content and name
                byte[] fileData = assignment.AssignmentFile; // Ensure this contains the file's byte[] content
                if (fileData == null || fileData.Length == 0)
                {
                    await DisplayAlert("Error", "No file data found to download.", "OK");
                    return;
                }

                string fileName = $"Assignment_{assignment.StdName}{assignment.FileType}"; // File name format
                string localPath = PlatformFileHelper.GetPublicDownloadsPath(fileName);

                // Save the file
                File.WriteAllBytes(localPath, fileData);

                await DisplayAlert("Download Complete", $"File saved at {localPath}", "OK");

                // Open the file
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(localPath)
                });
            }
            catch (UnauthorizedAccessException uaEx)
            {
                await DisplayAlert("Permission Denied", uaEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to download file: {ex.Message}", "OK");
            }
        }
    }*/






    /*async Task RequestReadAndRight
    private async void DownloadClicked(object sender, EventArgs e) {
        var button = sender as Button;

        bool[] ch = new bool[2];
        var status1 = PermissionStatus.Unknown;
        var status2 = PermissionStatus.Unknown;
        status1 = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        status2 = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

        if (status1 == PermissionStatus.Granted) {
            ch[0] = true;
        }
        else
        {
            ch[0] = false;
        }
        if (status2 == PermissionStatus.Granted) {
            ch[1] = true;
        }
        else
        {
            ch[1] = false;
        }
        await DisplayAlert("yes", $"Status1:{ch[0]}\n Status2:{ch[1]}", "yes");

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
*/

}