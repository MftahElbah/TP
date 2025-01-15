using Google.Android.Material.Snackbar;
using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Methods.actions;
using static Java.Util.Jar.Attributes;

namespace TP.Pages.Student;

public partial class SubjectCenterStd : ContentPage
{
    Database database = Database.SelectedDatabase;

    public ObservableCollection<SubjectPosts> Posts { get; set; }
    public ObservableCollection<SubjectBooks> Books { get; set; }
    private System.Timers.Timer _countdownTimer;
    public int SubId;
    public bool[] Emptys = new bool[2];
    public string LinkUrl;


    public SubjectCenterStd(int subid,bool showdeg)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        SubId = subid;
        Books = new ObservableCollection<SubjectBooks>();
        Posts = new ObservableCollection<SubjectPosts>();
        if (!showdeg)
        { ShowDegree.IsVisible = false; }
        BindingContext = this;
        HideContentViewMethod.HideContentView(PostPopupWindow , PostBorder);
        HideContentViewMethod.HideContentView(PopupShowDegreeWindow, PopupShowDegreeBorder);
    }
    protected override async void OnAppearing(){
        base.OnAppearing();
        await LoadBooks();
        await LoadPosts();
        Postslistview.IsVisible = true;

        PageShowStatus(1);
    }
    //LoadDataSection
    private async Task LoadPosts()
    {
        EmptyMessage.IsVisible = false;
        Posts.Clear();
        var data = await database.getSubjectPostsBySubId(SubId);
        var posts = data
            .OrderByDescending(b => b.PostDate)
            .ToList();
        if (posts.Count == 0)
        {
        EmptyMessage.IsVisible = true;
            Emptys[0] = true;
            return;
        }
        foreach (var post in posts)
        {
            Posts.Add(post);
        }
        Emptys[0] = false;
    }
    private async Task LoadBooks()
    {
        var data = await database.getSubjectBooksBySubId(SubId);
       var books = data     
            .OrderByDescending(b => b.UploadDate)
            .ToList();

        Books.Clear();

        //to check if its empty or not to load message
        if (books.Count == 0)
        {
            Emptys[1] = true;
            return;
        }
        foreach (var book in books)
        {
            Books.Add(book);
        }
        Emptys[1] = false;
    }
    //Nav Bar
    private async void ShowDegreeClicked(object sender, EventArgs e) {

        var data = await database.getDegreeTableBySubIdAndStdName(SubId);
        var deg = data.FirstOrDefault();
        DegreeLbl.Text = $"الأعمال:{deg.Deg}";
        MidDegreeLbl.Text = $"الجزئي:{deg.MiddelDeg}";
        TotalDegreeLbl.Text = $"المجموع:{deg.Total}";
        PopupShowDegreeWindow.IsVisible = true;
    }

    private async void BackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    //Selection View Bar
    private void PostsShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(1);
    }
    private void BooksShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(2);
    }
    public void PageShowStatus(int status)
    {
        PostsShower.TextColor = Color.FromArgb("#1A1A1A");
        if (PostsShower.ImageSource is FontImageSource postFontImageSource)
        {
            postFontImageSource.Color = Color.FromArgb("#1A1A1A"); // Reset icon color
        }
        PostsShower.BackgroundColor = Colors.Transparent;
        Postslistview.IsVisible = false;

        BooksShower.TextColor = Color.FromArgb("#1A1A1A");
        if (BooksShower.ImageSource is FontImageSource booksFontImageSource)
        {
            booksFontImageSource.Color = Color.FromArgb("#1A1A1A"); // Reset icon color
        }
        BooksShower.BackgroundColor = Colors.Transparent;
        PdfListView.IsVisible = false;
        //to show posts
        switch (status)
        {
            case 1: // Show posts
                PostsShower.TextColor = Color.FromArgb("#D9D9D9");
                if (PostsShower.ImageSource is FontImageSource postIconSource)
                {
                    postIconSource.Color = Color.FromArgb("#D9D9D9"); // Active icon color
                }
                PostsShower.BackgroundColor = Color.FromArgb("#1A1A1A");
                Postslistview.IsVisible = true;

                NoExistTitle.Text = "لا يوجد منشورات";
                NoExistSubTitle.Text = "يمكنك اضافته عن طريق القائمة";
                EmptyMessage.IsVisible = Emptys[0];
                //Add btn icon change
                break;


            case 2: // Show books
                BooksShower.TextColor = Color.FromArgb("#D9D9D9");
                if (BooksShower.ImageSource is FontImageSource booksIconSource)
                {
                    booksIconSource.Color = Color.FromArgb("#D9D9D9"); // Active icon color
                }
                BooksShower.BackgroundColor = Color.FromArgb("#1A1A1A");
                PdfListView.IsVisible = true;

                NoExistTitle.Text = "لا يوجد كتب";
                NoExistSubTitle.Text = "يمكنك اضافته عن طريق القائمة";
                EmptyMessage.IsVisible = Emptys[1];
                //Add btn icon change
                break;
        }
    }
    //Post Section
    private async void SelectionPostChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {
        ShowAssignments.IsVisible = false;
        ShowDesFileBtn.IsVisible = false;
        CountdownLabel.IsVisible = false;
        //OpenLinkBtn.IsVisible = false;
        ShowAssignments.IsEnabled = true;
        ShowAssignments.Text = "الرفع";
        ShowAssignments.BackgroundColor = Color.FromArgb("#1A1A1A");

        var SelectedPost = Postslistview.SelectedItem as SubjectPosts;

        IdLblPopup.Text = SelectedPost.PostId.ToString();
        TitleLblPopup.Text = SelectedPost.PostTitle;
        DesLblPopup.Text = SelectedPost.PostDes;
        DeadLineTimeLblPopup.Text = SelectedPost.DeadLineTime.ToString();
        PostPopupWindow.IsVisible = true;
        Postslistview.SelectedItem = null;
        /*LinkUrl = SelectedPost.PostFileLink;
        if (!string.IsNullOrEmpty(LinkUrl))
        {
            OpenLinkBtn.IsVisible = true;
        }*/
        if (SelectedPost.PostFileLink != null)
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
        bool isUploaded = await database.getSubjectAssignmentByPostIdAndStdId(SelectedPost.PostId);
            
        if (isUploaded) { 
        ShowAssignments.IsEnabled = false;
        ShowAssignments.Text = "تم الرفع";
        ShowAssignments.BackgroundColor = Colors.Gray;
        }

    }
    private async void OpenLinkBtnClicked(object sender, EventArgs e)
    {
        if (Uri.IsWellFormedUriString(LinkUrl, UriKind.Absolute))
        {
            await Launcher.OpenAsync(LinkUrl);
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
                    CountdownLabel.Text = "00:00:00:00";
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
                CountdownLabel.Text = $"{remainingTime.Days:D2}:{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            });
        };

        _countdownTimer.Start();
    }
    private async void ShowDesFileBtnClicked(object sender, EventArgs e)
    {
        try
        {
            int pid = int.Parse(IdLblPopup.Text);

            // Fetch the post data with the file path
            var desFile = await database.getSubjectPost(pid);

            // Assuming desFile.PostDesFilePath contains the full file path on the device
            string filePath = desFile.PostFileLink;

            // Check if the file exists
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                
                Snackbar.ShowSnackbar(2, "لا يوجد في الجهاز الخاص بك");
                return;
            }

            // Open the file directly from the stored location
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            Snackbar.ShowSnackbar(2, $"Failed to open file: {ex.Message}");

        }
    }

    private async void UploadAssignmentsClicked(object sender, EventArgs e)
    {
        string fileTypeName;
        try
        {
            // Define custom file types for Android
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.Android, new[]
                {
                    "application/pdf",
                    "application/zip",
                    "application/msword",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                }
            },
        });

            // Use FilePicker to select a file
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a file to upload",
                FileTypes = customFileType
            });

            if (result == null) return; // No file selected

            // Get file extension
            fileTypeName = Path.GetExtension(result.FileName)?.ToLower();

            // Get the file's full path
            string filePath = result.FullPath;

            if (string.IsNullOrEmpty(filePath))
            {

                Snackbar.ShowSnackbar(2, "لا يوجد في الجهاز الخاص بك");
                return;
            }

            // Store file path instead of file data
            var assignment = new SubjectAssignments
            {
                PostId = int.Parse(IdLblPopup.Text),
                StdId = UserSession.UserId,
                StdName = UserSession.Name,
                AssignmentFile = filePath // Save the file path instead of byte array
            };

            await database.insertSubjectAssignment(assignment);

            Snackbar.ShowSnackbar(1, "تم الرفع بنجاح");

            PostPopupWindow.IsVisible = false;
        }
        catch (Exception ex)
        {
            Snackbar.ShowSnackbar(2, $"An error occurred: {ex.Message}");

        }
    }

    private void CancelDegreeClicked(object sender, EventArgs e)
    {
        PopupShowDegreeWindow.IsVisible = false;
    }
    private void CancelPostClicked(object sender, EventArgs e)
    {
        PostPopupWindow.IsVisible = false;
    }

    //Book Section
    private async void BookTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (e.DataItem is SubjectBooks selectedPdf)
        {
            try
            {
                // Assuming selectedPdf.BookPath holds the file location on the device
                string filePath = selectedPdf.BookFile;

                // Validate if the file path exists
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {

                    Snackbar.ShowSnackbar(2, "لا يوجد في الجهاز الخاص بك");

                    return;
                }

                // Open the file from the stored location
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {

                Snackbar.ShowSnackbar(2, $"Failed to open file: {ex.Message}");

            }
        }
    }

}