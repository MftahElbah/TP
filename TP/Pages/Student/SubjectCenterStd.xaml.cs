﻿using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Student;

public partial class SubjectCenterStd : ContentPage
{
    private DatabaseHelper _databaseHelper;
    public ObservableCollection<SubjectBooks> Books { get; set; }
    public ObservableCollection<SubjectPosts> Posts { get; set; }
    private System.Timers.Timer _countdownTimer;
    public int SubId;
    public readonly SQLiteAsyncConnection _database;
    
    public SubjectCenterStd(int subid,bool showdeg)
	{
		InitializeComponent();
        SubId = subid;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath); // Pass your database path
        _database = new SQLiteAsyncConnection(dbPath);
        Books = new ObservableCollection<SubjectBooks>();
        Posts = new ObservableCollection<SubjectPosts>();
        if (!showdeg)
        { ShowDegree.IsVisible = false; }
        BindingContext = this;
    }
    protected override async void OnAppearing(){
        base.OnAppearing();
        await LoadBooks();
        await LoadPosts();
        PageShowStatus(1);
    }

   private async Task LoadBooks()
    {
        var books = await _database.Table<SubjectBooks>()
            .Where(b => b.SubId == SubId)
            .OrderByDescending(b => b.UploadDate)
            .ToListAsync();

        Books.Clear();
        foreach (var book in books) { 
            Books.Add(book);
        } 
    }
    private async Task LoadPosts()
    {
        Posts.Clear();
        var posts = await _database.Table<SubjectPosts>()
            .Where(b => b.SubId == SubId)
            .OrderByDescending(b => b.PostDate)
            .ToListAsync();

        foreach (var post in posts) {
            Posts.Add(post);
        } 
    }

    private async void ShowDegreeClicked(object sender, EventArgs e) {
        var deg = await _database.Table<DegreeTable>().Where(s => s.SubId == SubId && s.StdName == UserSession.Name).FirstOrDefaultAsync();
        await DisplayAlert("درجات", $"الأعمال:{deg.Deg}\n الجزئي:{deg.MiddelDeg} \n المجموع:{deg.Total}","حسنا");
    }

    private void PostsShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(1);
    }
    private void BooksShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(2);
    }
    public void PageShowStatus(int Status)
    {
        //to show posts
        if(Status == 1) {
            PostsShower.TextColor = Color.FromArgb("#DCDCDC");
            PostsShower.Background = Color.FromArgb("#2374AB");
            Postslistview.IsVisible = true;

            BooksShower.TextColor= Color.FromArgb("#1A1A1A");
            BooksShower.Background = Colors.Transparent;
            PdfListView.IsVisible = false;
        }

        //to show To show books
        if(Status == 2)
        {
            PostsShower.TextColor = Color.FromArgb("#1A1A1A");
            PostsShower.Background = Colors.Transparent;
            Postslistview.IsVisible = false;

            BooksShower.TextColor = Color.FromArgb("#DCDCDC");
            BooksShower.Background = Color.FromArgb("#2374AB");
            PdfListView.IsVisible = true;
        }
    }



    private async void SelectionPostChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {
        if (Postslistview.SelectedItem == null)
        {
            return;
        }
        ShowAssignments.IsVisible = false;
        ShowDesFileBtn.IsVisible = false;
        CountdownLabel.IsVisible = false;
        var SelectedPost = Postslistview.SelectedItem as SubjectPosts;

        IdLblPopup.Text = SelectedPost.PostId.ToString();
        TitleLblPopup.Text = SelectedPost.PostTitle;
        DesLblPopup.Text = SelectedPost.PostDes;
        DeadLineTimeLblPopup.Text = SelectedPost.DeadLineTime.ToString();
        PostPopupWindow.IsVisible = true;
        Postslistview.SelectedItem = null;
        if (SelectedPost.PostDesFile != null)
        {
            ShowDesFileBtn.IsVisible = true;
        }
        if (String.IsNullOrEmpty(DeadLineTimeLblPopup.Text))
        {
            return ;
        }
        CountdownLabel.IsVisible = true;
        ShowAssignments.IsVisible = true;
        Countdown(SelectedPost.DeadLineTime.Value);
        if (DateTime.Now > SelectedPost.DeadLineTime) { 

        ShowAssignments.IsEnabled = false;
        ShowAssignments.Text = "غير متوفر";
        ShowAssignments.BackgroundColor = Colors.Gray;
            return ;
        }
        var isUploaded = await _database.Table<SubjectAssignments>()
            .FirstOrDefaultAsync(a => a.PostId == SelectedPost.PostId && a.StdId == UserSession.UserId);
        if (isUploaded != null) { 
        ShowAssignments.IsEnabled = false;
        ShowAssignments.Text = "تم الرفع";
        ShowAssignments.BackgroundColor = Colors.Gray;
        }

    }
    private void Countdown(DateTime deadlineTime)
    {
        if (_countdownTimer != null)
        {
            _countdownTimer.Stop();
            _countdownTimer.Dispose();
            _countdownTimer = null;
        }

        _countdownTimer = new System.Timers.Timer(1000); // Tick every 1 second
        _countdownTimer.Elapsed += (s, e) =>
        {
            var remainingTime = deadlineTime - DateTime.Now;

            if (remainingTime <= TimeSpan.Zero)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CountdownLabel.Text = "00:00:00";
                    ShowAssignments.IsEnabled = false;
                    ShowAssignments.Text = "غير متوفر";
                    ShowAssignments.BackgroundColor = Colors.Gray;
                });

                _countdownTimer.Stop();
                _countdownTimer.Dispose();
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                CountdownLabel.Text = $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            });
        };

        _countdownTimer.Start();
    }
    private async void ShowDesFileBtnClicked(object sender, EventArgs e)
    {
        int pid = int.Parse(IdLblPopup.Text);
        var desFile = await _database.Table<SubjectPosts>()
           .FirstOrDefaultAsync(a => a.PostId == pid);


        // Write the file to the temporary directory
        byte[] fileBytes = desFile.PostDesFile;
        var tempPath = Path.Combine(FileSystem.CacheDirectory, $"{desFile.PostTitle}.pdf");
        await File.WriteAllBytesAsync(tempPath, fileBytes);


        // Open the file
        await Launcher.Default.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(tempPath)
        });
    }
    private async void UploadAssignmentsClicked(object sender, EventArgs e)
    {
        string filetypename;
        try
        {
            // Define custom file types
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
            //{ DevicePlatform.iOS, new[] { "public.composite-content", "public.data", "public.text", "public.zip-archive", "com.adobe.pdf" } }, if anyone want to convert this app to Iphone
            { DevicePlatform.Android, new[] { "application/pdf", "application/zip", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } },
            });

            // Use FilePicker to allow user to select a file
            var result = await FilePicker.PickAsync(new PickOptions{
                PickerTitle = "Select a file to upload",
                FileTypes = customFileType // Apply the custom file type
            });

            if (result == null){return; }


            filetypename = Path.GetExtension(result.FileName)?.ToLower();
            // Read the file data
            var stream = await result.OpenReadAsync();

                // Convert file to byte array for storage
                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                // Store file data in the database or process it as needed
                var assignment = new SubjectAssignments
                {
                    PostId = int.Parse(IdLblPopup.Text),
                    StdId = UserSession.UserId,
                    StdName = UserSession.Name,
                    FileType = filetypename,
                    AssignmentFile = fileData // Save the file data

                };

                await _database.InsertAsync(assignment);

                await DisplayAlert("تمت", "تم رفع العملية بنجاح", "حسنا");
            PostPopupWindow.IsVisible = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
    private void CancelPostClicked(object sender, EventArgs e)
    {
        PostPopupWindow.IsVisible = false;
    }
    
    private async void BookTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (e.DataItem is SubjectBooks selectedPdf)
        {
            // Save the file temporarily
            var tempPath = Path.Combine(FileSystem.CacheDirectory, selectedPdf.BookName);
            await File.WriteAllBytesAsync(tempPath, selectedPdf.BookFile);

            // Open the file
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(tempPath)
            });
        }
    }

}