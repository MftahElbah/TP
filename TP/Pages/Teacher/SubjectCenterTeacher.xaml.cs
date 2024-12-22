using Dalvik.SystemInterop;
using SQLite;
using Syncfusion.Maui.ListView;
using System.Collections.ObjectModel;
using TP.Methods;


namespace TP.Pages.Teacher;

public partial class SubjectCenterTeacher : ContentPage
{
    private DatabaseHelper _databaseHelper;
    public ObservableCollection<SubjectBooks> Books { get; set; }
    public string CCC { get; set; }
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
    public string namevar;
    public bool[] Emptys = new bool[3];
    public SubjectCenterTeacher(int subid)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

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
        if (posts.Count == 0) {
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



    private async void BackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void SettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsForSub(SSubId)); // Navigate to Settings page
    }
    private void AddMenuClicked(object sender, EventArgs e)
    {
        MenuPopupWindow.IsVisible = true;
    }
    private void CloseMenuClicked(object sender, EventArgs e)
    {
        MenuPopupWindow.IsVisible = false;
    }
    private async void AddPostClicked(object sender, EventArgs e)
    {
                await Navigation.PushAsync(new EditPostPage(SSubId, null , null , null,null)); // Navigate to Add Post page
        MenuPopupWindow.IsVisible = false;
    }
    private void AddBookClicked(object sender, EventArgs e)
    {
                UploadBook(1); // Navigate to Add Book page
                MenuPopupWindow.IsVisible = false;
    }
    private async void RequestsMangmentClicked(object sender, EventArgs e)
    {
                await Navigation.PushAsync(new RequestMangment(SSubId)); // Navigate to Requests page
                MenuPopupWindow.IsVisible = false;
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
    private void PageShowStatus(int status){
        // Reset all controls to the default state
        PostsShower.TextColor = Color.FromArgb("#1A1A1A");
        if (PostsShower.ImageSource is FontImageSource postFontImageSource)
        {
            postFontImageSource.Color = Color.FromArgb("#1A1A1A"); // Reset icon color
        }
        PostsShower.BackgroundColor = Colors.Transparent;
        Postslistview.IsVisible = false;

        DegreesShower.TextColor = Color.FromArgb("#1A1A1A");
        if (DegreesShower.ImageSource is FontImageSource degreesFontImageSource)
        {
            degreesFontImageSource.Color = Color.FromArgb("#1A1A1A"); // Reset icon color
        }
        DegreesShower.BackgroundColor = Colors.Transparent;
        DegreeTableDataGrid.IsVisible = false;

        BooksShower.TextColor = Color.FromArgb("#1A1A1A");
        if (BooksShower.ImageSource is FontImageSource booksFontImageSource)
        {
            booksFontImageSource.Color = Color.FromArgb("#1A1A1A"); // Reset icon color
        }
        BooksShower.BackgroundColor = Colors.Transparent;
        PdfListView.IsVisible = false;

        // Change styles based on the status
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

            case 2: // Show degrees table
                DegreesShower.TextColor = Color.FromArgb("#D9D9D9");
                if (DegreesShower.ImageSource is FontImageSource degreesIconSource)
                {
                    degreesIconSource.Color = Color.FromArgb("#D9D9D9"); // Active icon color
                }
                DegreesShower.BackgroundColor = Color.FromArgb("#1A1A1A");
                DegreeTableDataGrid.IsVisible = true;

                NoExistTitle.Text = "لا يوجد طالب مشترك";
                NoExistSubTitle.Text = "تأكد من صفحة \"طلبات الانضمام\" الموجودة في القائمة";
                EmptyMessage.IsVisible = Emptys[1];

                //Add btn icon change
                break;

            case 3: // Show books
                BooksShower.TextColor = Color.FromArgb("#D9D9D9");
                if (BooksShower.ImageSource is FontImageSource booksIconSource)
                {
                    booksIconSource.Color = Color.FromArgb("#D9D9D9"); // Active icon color
                }
                BooksShower.BackgroundColor = Color.FromArgb("#1A1A1A");
                PdfListView.IsVisible = true;

                NoExistTitle.Text = "لا يوجد كتب";
                NoExistSubTitle.Text = "يمكنك اضافته عن طريق القائمة";
                EmptyMessage.IsVisible = Emptys[2];
                //Add btn icon change
                break;
        }
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
                DeadLineTime = null,
            };
            await _database.InsertAsync(pdfPost);
            PopupEditBookNameWindow.IsVisible = false;
            await LoadBooks();
            await LoadPosts();
            PageShowStatus(3);
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


    private void DegreeTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (DegreeTableDataGrid.SelectedRow == null)
        {
            return;
        }

        var DataRow = DegreeTableDataGrid.SelectedRow;
        namevar = DataRow?.GetType().GetProperty("StdName")?.GetValue(DataRow)?.ToString();
        StdNameEntry.Text = $"اسم: {namevar}";
        DegreeEntry.Text = DataRow?.GetType().GetProperty("Deg")?.GetValue(DataRow)?.ToString();
        MidDegreeEntry.Text = DataRow?.GetType().GetProperty("MiddelDeg")?.GetValue(DataRow)?.ToString();

        PopupEditDegreeWindow.IsVisible = true;
        DegreeTableDataGrid.SelectedRow = null;
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

        var deg = await _database.Table<DegreeTable>().FirstOrDefaultAsync(d => d.StdName == namevar);

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
    private async void DeleteDegreeClicked(object sender, EventArgs e) {
        bool isConfirmed =await DisplayAlert("تأكيد", "هل انت متأكد", "متأكد", "الغاء");
        if (!isConfirmed) { return; }
        
        var DelDeg = await _database.Table<DegreeTable>().Where(d => d.StdName == namevar).FirstOrDefaultAsync();
        await _database.DeleteAsync(DelDeg);
        PopupEditDegreeWindow.IsVisible = false;
        await LoadData();
    }


    private void SelectionPostChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {
        if (Postslistview.SelectedItem == null)
        {
            return;
        }
        ShowAssignments.IsVisible = false;
        ShowDesFileBtn.IsVisible = false;
        var SelectedPost = Postslistview.SelectedItem as SubjectPosts;

        IdLblPopup.Text = SelectedPost.PostId.ToString();
        TitleLblPopup.Text = SelectedPost.PostTitle;
        DesLblPopup.Text = SelectedPost.PostDes;
        DeadLineTimeLblPopup.Text = SelectedPost.DeadLineTime.ToString();
        if(!String.IsNullOrEmpty(DeadLineTimeLblPopup.Text))
        {
            ShowAssignments.IsVisible = true;
        }
        if (SelectedPost.PostDesFile != null) {
            ShowDesFileBtn.IsVisible = true;
        }
        EditPostPopupWindow.IsVisible = true;
        Postslistview.SelectedItem = null;
    }
    private async void ShowAssignmentsClicked(object sender, EventArgs e) { 
        await Navigation.PushAsync(new AssignmentsPage(int.Parse(IdLblPopup.Text)));
    }
    private async void EditPostClicked(object sender, EventArgs e) { 
        
        await Navigation.PushAsync(new EditPostPage(SSubId, IdLblPopup.Text.ToString(), TitleLblPopup.Text, DesLblPopup.Text,DeadLineTimeLblPopup.Text)); // i want here to take data from selected list view
    }
    private async void ShowDesFileBtnClicked(object sender, EventArgs e) {
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
    private void CancelPostClicked(object sender, EventArgs e) {
        EditPostPopupWindow.IsVisible = false;
    }


}