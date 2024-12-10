using SQLite;
using Syncfusion.Maui.ListView;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Pages.Teacher;

namespace TP.Pages;

public partial class SubjectCenter : ContentPage
{
    private DatabaseHelper _databaseHelper;
    public ObservableCollection<SubjectBooks> Books { get; set; }
    private ObservableCollection<DegreeTable> DegreeTableGetter;
    public ObservableCollection<DegreeTable> DegreeTableSetter
    {
        get => DegreeTableGetter;
        set
        {
            DegreeTableGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }
    }
    public ObservableCollection<SubjectPosts> Posts { get; set; }
    public int SubId;
    public readonly SQLiteAsyncConnection _database;
    private FileResult result;
    public SubjectCenter(int subid)
	{
		InitializeComponent();
        SubId = subid;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath); // Pass your database path
        _database = new SQLiteAsyncConnection(dbPath);
        Books = new ObservableCollection<SubjectBooks>();
        Posts = new ObservableCollection<SubjectPosts>();
        DegreeTableGetter = new ObservableCollection<DegreeTable>();
        /*suby = new ObservableCollection<SubForStdTable>();*/
        BindingContext = this;
        PageShowStatus(1);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
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
    private async Task LoadData()
    {
        var degreeTableData = await _database.Table<DegreeTable>().Where(s => s.SubId == SubId).ToListAsync();
        if (degreeTableData != null)
        {
            DegreeTableSetter = new ObservableCollection<DegreeTable>(degreeTableData);
        }
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("قائمة المادة", null, null, "اضف منشور", "أضف كتاب", "طلبات الانضمام", "الأعدادات");

        switch (action)
        {
            case "اضف منشور":
                await Navigation.PushAsync(new EditPostPage(SubId)); // Navigate to Add Post page
                break;
            case "أضف كتاب":
                UploadBook(1); // Navigate to Add Book page
                break;
            case "طلبات الانضمام":
                await Navigation.PushAsync(new RequestMangment(SubId)); // Navigate to Requests page
                break;
            case "الأعدادات":
                /*await Navigation.PushAsync(new SettingsPage());*/ // Navigate to Settings page
                break;
        }
    }

    private async void OnBookTapped(object sender, Microsoft.Maui.Controls.ItemTappedEventArgs e)
    {
        if (e.Item is SubjectBooks selectedPdf)
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
    private async void SaveBookClicked(object sender, EventArgs e)
    {
        if(BookNameEntry == null)
        {
            await DisplayAlert("خطا", "يجب ان يكون حقل الاسم غير فارغ", "حسنا");
            return;
        }
        UploadBook(2);
    }
    private void CancelBookClicked(object sender, EventArgs e)
    {
        PopupEditBookNameWindow.IsVisible = false;
        return;
    }
    public async void UploadBook(int step)
    {
        if (step == 1) {
            result = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf,
                PickerTitle = "Select a PDF"
            });
            if (result != null)
            {
                PopupEditBookNameWindow.IsVisible = true;
                BookNameEntry.Text = result.FileName;
            }
        }
        if (step == 2) { 
            var fileContent = await File.ReadAllBytesAsync(result.FullPath);

            var pdfFile = new SubjectBooks{
                SubId = SubId,
                BookName = BookNameEntry.Text,
                BookFile = fileContent,
                UploadDate = DateTime.Now,
            };

            await _database.InsertAsync(pdfFile);
            var pdfPost = new SubjectPosts{
                SubId = SubId,
                PostTitle = "تم اضافة كتاب جديد",
                PostDes = $"تم اضافة كتاب \"{BookNameEntry.Text}\" في قسم الكتب",
                PostDate = DateTime.Now,
            };
            await _database.InsertAsync(pdfPost);
            PopupEditBookNameWindow.IsVisible = false;
            await LoadBooks();
            await LoadPosts();
        }
    }
    private async void DegreeTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (DegreeTableDataGrid.SelectedRow == null)
        {
            return;
        }
        var DataRow = DegreeTableDataGrid.SelectedRow;

        /*var stdName = DataRow?.GetType().GetProperty("StdName")?.GetValue(DataRow)?.ToString();
        var degree = DataRow?.GetType().GetProperty("Deg")?.GetValue(DataRow)?.ToString();
        var midDegree = DataRow?.GetType().GetProperty("MidDeg")?.GetValue(DataRow)?.ToString();
        var total = DataRow?.GetType().GetProperty("Total")?.GetValue(DataRow)?.ToString();*/
        StdNameEntry.Text = DataRow?.GetType().GetProperty("StdName")?.GetValue(DataRow)?.ToString();
        DegreeEntry.Text = DataRow?.GetType().GetProperty("Deg")?.GetValue(DataRow)?.ToString();
        MidDegreeEntry.Text = DataRow?.GetType().GetProperty("MiddelDeg")?.GetValue(DataRow)?.ToString();

        PopupEditDegreeWindow.IsVisible = true;
        DegreeTableDataGrid.SelectedRow = null;
        await LoadData();
    }

    private async void SaveDegreeClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(DegreeEntry.Text) || string.IsNullOrEmpty(MidDegreeEntry.Text)) { 
            return;
        }
        float total = float.Parse(DegreeEntry.Text) + float.Parse(MidDegreeEntry.Text);
        if(total > 40)
        {
            await DisplayAlert("خطا", "يجب ان يكون مجموع درجة الطالب اقل او تساوي 40", "حسنا");
            return;
        }

        var deg = await _database.Table<DegreeTable>().FirstOrDefaultAsync(d => d.StdName == StdNameEntry.Text);

        deg.Deg = float.Parse(DegreeEntry.Text);
        deg.MiddelDeg = float.Parse(MidDegreeEntry.Text);
        
        await _database.UpdateAsync(deg);
        PopupEditDegreeWindow.IsVisible = false;
    }

    
    private void CancelDegreeClicked(object sender, EventArgs e)
    {
        // Hide popup
        PopupEditDegreeWindow.IsVisible = false;
    }

    private void PostsShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(1);
    }
    private void DegreesShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(2);
    }
    private void BooksShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(3);
    }

    public void PageShowStatus(int Status)
    {
        //to show posts
        if(Status == 1) {
            PostsShower.TextColor = Color.FromArgb("#DCDCDC");
            PostsShower.Background = Color.FromArgb("#2374AB");
            Postslistview.IsVisible = true;

            DegreesShower.TextColor= Color.FromArgb("#1A1A1A");
            DegreesShower.Background = Colors.Transparent;
            DegreeTableDataGrid.IsVisible = false;

            BooksShower.TextColor= Color.FromArgb("#1A1A1A");
            BooksShower.Background = Colors.Transparent;
            PdfListView.IsVisible = false;
        }
        //to show Degrees Table
        if(Status == 2) {
            PostsShower.TextColor = Color.FromArgb("#1A1A1A");
            PostsShower.Background = Colors.Transparent;
            Postslistview.IsVisible = false;

            DegreesShower.TextColor = Color.FromArgb("#DCDCDC");
            DegreesShower.Background = Color.FromArgb("#2374AB");
            DegreeTableDataGrid.IsVisible = true;

            BooksShower.TextColor = Color.FromArgb("#1A1A1A");
            BooksShower.Background = Colors.Transparent;
            PdfListView.IsVisible = false;
        }
        //to show To show books
        if(Status == 3)
        {
            PostsShower.TextColor = Color.FromArgb("#1A1A1A");
            PostsShower.Background = Colors.Transparent;
            Postslistview.IsVisible = false;

            DegreesShower.TextColor = Color.FromArgb("#1A1A1A");
            DegreesShower.Background = Colors.Transparent;
            DegreeTableDataGrid.IsVisible = false;

            BooksShower.TextColor = Color.FromArgb("#DCDCDC");
            BooksShower.Background = Color.FromArgb("#2374AB");
            PdfListView.IsVisible = true;
        }
    }
}