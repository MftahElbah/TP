using SQLite;
using Syncfusion.Maui.ListView;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages;

public partial class SubjectCenter : ContentPage
{
    private DatabaseHelper _databaseHelper;
    public ObservableCollection<SubjectPosts> Posts { get; set; }

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
        DegreeTableGetter = new ObservableCollection<DegreeTable>();
        Posts = new ObservableCollection<SubjectPosts>();
        /*suby = new ObservableCollection<SubForStdTable>();*/
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
        await LoadBooks();
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
        var posts = await _database.Table<SubjectPosts>()
            .Where(b => b.SubId == SubId)
            .OrderByDescending(b => b.PostDate)
            .ToListAsync();

        Posts.Clear();
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

    private async void OnUploadPdfClicked(object sender, EventArgs e)
    {

        UploadBook(1);
        
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
            await LoadBooks();
        }
    }
    private void DegreeTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
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
        MidDegreeEntry.Text = DataRow?.GetType().GetProperty("MidDeg")?.GetValue(DataRow)?.ToString();

        PopupEditDegreeWindow.IsVisible = true;
        DegreeTableDataGrid.SelectedRow = null;
    }

    private void SaveDegreeClicked(object sender, EventArgs e)
    {
        // Save data logic here
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
            AddBookBtn.IsVisible = false;
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
            AddBookBtn.IsVisible = false;
        }
        //to show To show books
        if(Status == 3)
        {
            PostsShower.TextColor = Color.FromArgb("#1A1A1A");
            PostsShower.Background = Colors.Transparent;
            Postslistview.IsVisible = false;

            DegreesShower.TextColor = Color.FromArgb("#1A1A1A");
            DegreesShower.Background = Colors.Transparent;
            DegreeTableDataGrid.IsVisible = true;

            BooksShower.TextColor = Color.FromArgb("#DCDCDC");
            BooksShower.Background = Color.FromArgb("#2374AB");
            PdfListView.IsVisible = false;
            AddBookBtn.IsVisible = false;
        }
    }
}