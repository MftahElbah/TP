using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Student;

public partial class SubjectCenterStd : ContentPage
{
    private DatabaseHelper _databaseHelper;
    public ObservableCollection<SubjectBooks> Books { get; set; }
    public ObservableCollection<SubjectPosts> Posts { get; set; }
    public int SubId;
    public readonly SQLiteAsyncConnection _database;
    private FileResult result;
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
        PageShowStatus(1);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadBooks();
        await LoadPosts();
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
 private async Task LoadBtn()
    {
        Posts.Clear();
        var lb = await _database.Table<SubTable>()
            .Where(b => b.SubId == SubId)
            .FirstOrDefaultAsync();

        if (lb.ShowDeg)
        {
            ShowDegree.IsVisible = true;
            return;
        }
        ShowDegree.IsVisible = false;
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
}