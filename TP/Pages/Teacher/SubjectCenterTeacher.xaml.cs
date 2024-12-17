using SQLite;
using Syncfusion.Maui.ListView;
using System.Collections.ObjectModel;
using TP.Methods;


namespace TP.Pages.Teacher;

public partial class SubjectCenterTeacher : ContentPage
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
    public int SSubId;
    public readonly SQLiteAsyncConnection _database;
    private FileResult result;
    public bool[] Emptys = new bool[3];
    public SubjectCenterTeacher(int subid)
	{
		InitializeComponent();
        SSubId = subid;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath); // Pass your database path
        _database = new SQLiteAsyncConnection(dbPath);
        Books = new ObservableCollection<SubjectBooks>();
        Posts = new ObservableCollection<SubjectPosts>();
        DegreeTableGetter = new ObservableCollection<DegreeTable>();
        /*suby = new ObservableCollection<SubForStdTable>();*/
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPosts();
        await LoadData();
        await LoadBooks();
        PageShowStatus(1);
    }
    private async Task LoadPosts()
    {
        Posts.Clear();
        var posts = await _database.Table<SubjectPosts>()
            .Where(b => b.SubId == SSubId)
            .OrderByDescending(b => b.PostDate)
            .ToListAsync();
        if (posts.Count == 0 ) {
            Emptys[0] = true;
            return;
        }
        foreach (var post in posts) {
            Posts.Add(post);
        }
        Emptys[0] = false;
    }

    private async Task LoadData()
    {
        var degreeTableData = await _database.Table<DegreeTable>().Where(s => s.SubId == SSubId).ToListAsync();
            DegreeTableSetter = new ObservableCollection<DegreeTable>(degreeTableData);
        if (degreeTableData.Count == 0)
        {
            Emptys[1] = true;
            return;
        }
        Emptys[1] = false;
    }
    private async Task LoadBooks()
    {
        var books = await _database.Table<SubjectBooks>()
            .Where(b => b.SubId == SSubId)
            .OrderByDescending(b => b.UploadDate)
            .ToListAsync();

        Books.Clear();
        
        //to check if its empty or not to load message
        if(books.Count == 0) { 
            Emptys[2]=true;
            return;
        }
        foreach (var book in books) { 
            Books.Add(book);
        }
        Emptys[2] = false;
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("قائمة المادة", null, null, "اضف منشور", "أضف كتاب", "الأعدادات", "طلبات الانضمام");

        switch (action)
        {
            case "اضف منشور":
                await Navigation.PushAsync(new EditPostPage(SSubId, null , null , null)); // Navigate to Add Post page
                break;
            case "أضف كتاب":
                UploadBook(1); // Navigate to Add Book page
                break;
            case "طلبات الانضمام":
                await Navigation.PushAsync(new RequestMangment(SSubId)); // Navigate to Requests page
                break;
            case "الأعدادات":
                await Navigation.PushAsync(new SettingsForSub(SSubId)); // Navigate to Settings page
                break;
        }
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

    private async void LongBookTapped(object sender, Syncfusion.Maui.ListView.ItemLongPressEventArgs e)
    {
        var delbook = e.DataItem as SubjectBooks;
        if (delbook!=null)
        {
            bool confirm = await DisplayAlert("تأكيد الحذف", $"هل تريد حذف الكتاب: {delbook.BookName}؟", "نعم", "لا");
            if (confirm)
            {
                await _database.DeleteAsync(delbook);
                Books.Remove(delbook);
                await DisplayAlert("تم الحذف", "تم حذف الكتاب بنجاح", "حسنا");
            }
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
                SubId = SSubId,
                BookName = BookNameEntry.Text,
                BookFile = fileContent,
                UploadDate = DateTime.Now,
            };

            await _database.InsertAsync(pdfFile);
            var pdfPost = new SubjectPosts{
                SubId = SSubId,
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
    private void DegreeTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (DegreeTableDataGrid.SelectedRow == null)
        {
            return;
        }

        var DataRow = DegreeTableDataGrid.SelectedRow;
        StdNameEntry.Text = DataRow?.GetType().GetProperty("StdName")?.GetValue(DataRow)?.ToString();
        DegreeEntry.Text = DataRow?.GetType().GetProperty("Deg")?.GetValue(DataRow)?.ToString();
        MidDegreeEntry.Text = DataRow?.GetType().GetProperty("MiddelDeg")?.GetValue(DataRow)?.ToString();

        PopupEditDegreeWindow.IsVisible = true;
        DegreeTableDataGrid.SelectedRow = null;
    }
    private async void SelectionPostChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {
        if (Postslistview.SelectedItem == null)
        {
            return;
        }
        var SelectedPost = Postslistview.SelectedItem as SubjectPosts;

        await Navigation.PushAsync(new EditPostPage(SSubId, SelectedPost.PostId.ToString(), SelectedPost.PostTitle, SelectedPost.PostDes)); // i want here to take data from selected list view

        Postslistview.SelectedItem = null;
    }

    private async void DeleteDegreeClicked(object sender, EventArgs e) {
        bool isConfirmed =await DisplayAlert("تأكيد", "هل انت متأكد", "متأكد", "الغاء");
        if (!isConfirmed) { return; }
        string StdNameFromLbl = StdNameEntry.Text;
        var DelDeg = await _database.Table<DegreeTable>().Where(d => d.StdName == StdNameFromLbl).FirstOrDefaultAsync();
        await _database.DeleteAsync(DelDeg);
        PopupEditDegreeWindow.IsVisible = false;
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
        await LoadData();
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
            PostsShower.BackgroundColor = Color.FromArgb("#2374AB");
            Postslistview.IsVisible = true;

            DegreesShower.TextColor= Color.FromArgb("#1A1A1A");
            DegreesShower.BackgroundColor = Colors.Transparent;
            DegreeTableDataGrid.IsVisible = false;

            BooksShower.TextColor= Color.FromArgb("#1A1A1A");
            BooksShower.BackgroundColor = Colors.Transparent;
            PdfListView.IsVisible = false;

            NoExistTitle.Text = "لا يوجد منشورات";
            NoExistSubTitle.Text = "يمكنك اضافته عن طريق القائمة";
            EmptyMessage.IsVisible = Emptys[0];
        }
        //to show Degrees Table
        if(Status == 2) {
            PostsShower.TextColor = Color.FromArgb("#1A1A1A");
            PostsShower.BackgroundColor = Colors.Transparent;
            Postslistview.IsVisible = false;

            DegreesShower.TextColor = Color.FromArgb("#DCDCDC");
            DegreesShower.BackgroundColor = Color.FromArgb("#2374AB");
            DegreeTableDataGrid.IsVisible = true;

            BooksShower.TextColor = Color.FromArgb("#1A1A1A");
            BooksShower.BackgroundColor = Colors.Transparent;
            PdfListView.IsVisible = false;

            
            NoExistTitle.Text = "لا يوجد طالب مشترك";
            NoExistSubTitle.Text = "تأكد من صفحة \"طلبات الانضمام\" الموجودة في القائمة";            
            EmptyMessage.IsVisible = Emptys[1];
        }
        //to show To show books
        if(Status == 3)
        {
            PostsShower.TextColor = Color.FromArgb("#1A1A1A");
            PostsShower.BackgroundColor = Colors.Transparent;
            Postslistview.IsVisible = false;

            DegreesShower.TextColor = Color.FromArgb("#1A1A1A");
            DegreesShower.BackgroundColor = Colors.Transparent;
            DegreeTableDataGrid.IsVisible = false;

            BooksShower.TextColor = Color.FromArgb("#DCDCDC");
            BooksShower.BackgroundColor = Color.FromArgb("#2374AB");
            PdfListView.IsVisible = true;


            NoExistTitle.Text = "لا يوجد كتب";
            NoExistSubTitle.Text = "يمكنك اضافته عن طريق القائمة";
            EmptyMessage.IsVisible = Emptys[2];
        }
    }
}