using Google.Android.Material.Snackbar;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Methods.actions;

namespace TP.Pages.Teacher;

public partial class SubjectCenterTeacher : ContentPage
{
    public ObservableCollection<SubjectPosts> Posts { get; set; }
    private ObservableCollection<DegreeTable> DegreeTableGetter;
    Database database = Database.SelectedDatabase;

    public ObservableCollection<DegreeTable> DegreeTableSetter
    {
        get => DegreeTableGetter;
        set
        {
            DegreeTableGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }
    }
    public ObservableCollection<SubjectBooks> Books { get; set; }
    public int SSubId;
    private FileResult result;
    public string namevar;
    public string LinkUrl;
    public bool[] Emptys = new bool[3];//used to show empty massage


    public SubjectCenterTeacher(int subid)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        SSubId = subid;
        Books = new ObservableCollection<SubjectBooks>();
        Posts = new ObservableCollection<SubjectPosts>();
        DegreeTableGetter = new ObservableCollection<DegreeTable>();
        BindingContext = this;


        HideContentViewMethod.HideContentView(PopupEditDegreeWindow, PopupEditDegreeBorder);
        HideContentViewMethod.HideContentView(PopupEditBookNameWindow, PopupEditBookBorder);
        HideContentViewMethod.HideContentView(EditPostPopupWindow, EditPostPopupBorder);
        HideContentViewMethod.HideContentView(MenuPopupWindow, MenuPopupBorder);
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        /*
        await LoadPosts();
        await LoadData();
        await LoadBooks();
        */
        PageShowStatus(1);

        MenuPopupWindow.IsVisible = false;
    }
    private async Task LoadPosts(){
        var data = await database.getSubjectPostsBySubId(SSubId);
        var posts = data.OrderByDescending(p => p.PostDate).ToList();
        Posts.Clear();
            if (posts.Count == 0) {
            Emptys[0] = true;
            return;
        }
        foreach (var post in posts) {
            Posts.Add(post);
        }
        Emptys[0] = false;
    }
    private async Task LoadData(){
        var degreeTableData = await database.getDegreeTablesBySubId(SSubId);
        DegreeTableSetter = new ObservableCollection<DegreeTable>(degreeTableData);
        if (degreeTableData.Count == 0)
        {
            Emptys[1] = true;
            return;
        }
        Emptys[1] = false;
    }
    private async Task LoadBooks(){
        var books = await database.getSubjectBooksBySubId(SSubId);

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
    //NavBar
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
    //MenuPopup
    private void CloseMenuClicked(object sender, EventArgs e)
    {
        MenuPopupWindow.IsVisible = false;
    }
    private async void AddPostClicked(object sender, EventArgs e){
        await Navigation.PushAsync(new EditPostPage(SSubId, null , null , null,null , null)); // Navigate to Add Post page
        EditPostPopupWindow.IsVisible = false;
    }
    private void AddBookClicked(object sender, EventArgs e){
        UploadBook(1); // Navigate to Add Book page
        MenuPopupWindow.IsVisible = false;
    }
    private async void RequestsMangmentClicked(object sender, EventArgs e){
        await Navigation.PushAsync(new RequestMangment(SSubId)); // Navigate to Requests page
    }
    //Selection View Bar
    private void PostsShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(1);
    }
    private void DegreesShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(2);
    }
    private async void BooksShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(3);
    }
    private async void PageShowStatus(int status){
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
        Books.Clear();
        Posts.Clear();
        DegreeTableSetter.Clear();

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
                await LoadPosts();
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

                await LoadData();
                NoExistTitle.Text = "لا يوجد طالب مشترك";
                NoExistSubTitle.Text = "تأكد من صفحة \"طلبات الانضمام\" الموجودة في القائمة";
                EmptyMessage.IsVisible = Emptys[1];

                //Add btn icon change
                break;
                
            case 3: // Show books
                BooksShower.TextColor = Color.FromArgb("#D9D9D9");
                if (BooksShower.ImageSource is FontImageSource booksIconSource){
                    booksIconSource.Color = Color.FromArgb("#D9D9D9"); // Active icon color
                }
                BooksShower.BackgroundColor = Color.FromArgb("#1A1A1A");
                PdfListView.IsVisible = true;
                await LoadBooks();
                NoExistTitle.Text = "لا يوجد كتب";
                NoExistSubTitle.Text = "يمكنك اضافته عن طريق القائمة";
                EmptyMessage.IsVisible = Emptys[2];
                //Add btn icon change
                break;
        }
    }

   
    //Books Section
    public async void UploadBook(int step){
        switch (step){
            case 1:
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
            break;

            case 2:
                //var fileContent = await File.ReadAllBytesAsync(result.FullPath);

                var pdfFile = new SubjectBooks
                {
                    SubId = SSubId,
                    BookName = BookNameEntry.Text,
                    BookFile = result.FullPath,
                    UploadDate = DateTime.Now,
                };

                await database.insertSubjectBook(pdfFile);

                //auto post if new book add
                var pdfPost = new SubjectPosts
                {
                    SubId = SSubId,
                    PostTitle = "تم اضافة كتاب جديد",
                    PostDes = $"تم اضافة كتاب \"{BookNameEntry.Text}\" في قسم الكتب",
                    PostDate = DateTime.Now,
                    DeadLineTime = null,
                    PostFileLink = null,
                };
                await database.insertSubjectPost(pdfPost);
                PopupEditBookNameWindow.IsVisible = false;
                BookNameEntry.Text = "";
                //to reload data
                await LoadBooks();
                await LoadPosts();
                PageShowStatus(3);
                break;
        }
        /*if (step == 1) {
            
        }
        if (step == 2) { 
            
        }*/
    }
    private async void SaveBookClicked(object sender, EventArgs e){
        if(BookNameEntry == null)
        {
            
            Snackbar.ShowSnackbar(2, "يجب ان يكون حقل الاسم غير فارغ");

            return;
        }
        UploadBook(2);
    }
    private void CancelBookClicked(object sender, EventArgs e){
        PopupEditBookNameWindow.IsVisible = false;
    }
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

    private async void LongBookTapped(object sender, Syncfusion.Maui.ListView.ItemLongPressEventArgs e){
        var delbook = e.DataItem as SubjectBooks;
            //if (!confirm){return;}
            var yesNoPopup = new YesNoContentView();

            // Add the popup to the current page's layout (assuming a Grid or StackLayout named 'MainLayout')
            MainLayout.Children.Add(yesNoPopup);

            // Show the popup and wait for the user's response
            bool isConfirmed = await yesNoPopup.ShowAsync();

            // Remove the popup after the response
            MainLayout.Children.Remove(yesNoPopup);

            // If user clicked "No", exit the method
            if (!isConfirmed)
            {
                return;
        }

        await database.deleteSubjectBook(delbook);
            Books.Remove(delbook);
            Snackbar.ShowSnackbar(1, "تم حذف الكتاب بنجاح");

    }


    //Degree Section
    private void DegreeTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
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
            Snackbar.ShowSnackbar(2, "يجب ملئ جميع الحقول");
            
            
            return;
        }
        if (float.Parse(DegreeEntry.Text) < 0 || float.Parse(MidDegreeEntry.Text) < 0)
        {
            Snackbar.ShowSnackbar(2, "يجب الا يكون الدرجة اصغر من الصفر");
            
            return;
        }
        float total = float.Parse(DegreeEntry.Text) + float.Parse(MidDegreeEntry.Text);
        if(total > 40){
            Snackbar.ShowSnackbar(2, "يجب ان يكون مجموع درجة الطالب اقل او تساوي 40");
            
            return;
        }

        var deg = await database.getDegreeByStdNameAndSubId(namevar, SSubId);
        deg.Deg = float.Parse(DegreeEntry.Text);
        deg.MiddelDeg = float.Parse(MidDegreeEntry.Text);
        await database.updateDegree(deg);
        PopupEditDegreeWindow.IsVisible = false;
        await LoadData();
    }
    private void CancelDegreeClicked(object sender, EventArgs e)
    {
        // Hide popup
        PopupEditDegreeWindow.IsVisible = false;
    }
    private async void DeleteDegreeClicked(object sender, EventArgs e) {


        // Initialize the YesNoContentView
        var yesNoPopup = new YesNoContentView();

        // Add the popup to the current page's layout (assuming a Grid or StackLayout named 'MainLayout')
        MainLayout.Children.Add(yesNoPopup);

        // Show the popup and wait for the user's response
        bool isConfirmed = await yesNoPopup.ShowAsync();

        // Remove the popup after the response
        MainLayout.Children.Remove(yesNoPopup);

        // If user clicked "No", exit the method
        if (!isConfirmed)
        {
            return;
        }
        var DelDeg = await database.getDegreeByStdNameAndSubId(namevar, SSubId);
        await database.deleteDegree(DelDeg);
        PopupEditDegreeWindow.IsVisible = false;
        await LoadData();
    }

    //Post Section
    private void SelectionPostChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e){
        ShowAssignments.IsVisible = false;
        ShowDesFileBtn.IsVisible = false;
        //OpenLinkBtn.IsVisible = false;
        var SelectedPost = Postslistview.SelectedItem as SubjectPosts;

        IdLblPopup.Text = SelectedPost.PostId.ToString();
        TitleLblPopup.Text = SelectedPost.PostTitle;
        DesLblPopup.Text = SelectedPost.PostDes;
        LinkUrl = SelectedPost.PostFileLink;
        /*if (!string.IsNullOrEmpty(LinkUrl)) {
        //OpenLinkBtn.IsVisible = true;
        }*/
        DeadLineTimeLblPopup.Text = SelectedPost.DeadLineTime.ToString();
        if(!String.IsNullOrEmpty(DeadLineTimeLblPopup.Text))
        {
            ShowAssignments.IsVisible = true;
        }
        if (SelectedPost.PostFileLink != null) {
            ShowDesFileBtn.IsVisible = true;
        }
        EditPostPopupWindow.IsVisible = true;
        Postslistview.SelectedItem = null;
    }
    private async void ShowAssignmentsClicked(object sender, EventArgs e) { 
        await Navigation.PushAsync(new AssignmentsPage(int.Parse(IdLblPopup.Text)));
    }
    private async void EditPostClicked(object sender, EventArgs e) {   
        await Navigation.PushAsync(new EditPostPage(SSubId, IdLblPopup.Text.ToString(), TitleLblPopup.Text, DesLblPopup.Text , LinkUrl,DeadLineTimeLblPopup.Text)); // i want here to take data from selected list view
        EditPostPopupWindow.IsVisible = false;
    }
    private async void OpenLinkBtnClicked(object sender, EventArgs e){
        if (Uri.IsWellFormedUriString(LinkUrl, UriKind.Absolute)){
            await Launcher.OpenAsync(LinkUrl);
        }
    }
    private async void ShowDesFileBtnClicked(object sender, EventArgs e)
    {
        try
        {
            int pid = int.Parse(IdLblPopup.Text);

            // Get the post details, assuming it includes the file path
            var desFile = await database.getSubjectPost(pid);

            // Use the file path stored in the database (assuming it's in desFile.PostFilePath)
            string filePath = desFile.PostFileLink;

            // Check if the file path is valid and the file exists
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {

                
                Snackbar.ShowSnackbar(2, "لا يوجد في الجهاز الخاص بك");

                return;
            }

            // Open the file directly
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

    private void CancelPostClicked(object sender, EventArgs e) {
        EditPostPopupWindow.IsVisible = false;
    }

}