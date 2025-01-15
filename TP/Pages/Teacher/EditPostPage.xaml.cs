using SQLite;
using System;
using TP.Methods;
using TP.Methods.actions;

namespace TP.Pages.Teacher;

public partial class EditPostPage : ContentPage
{
    Database database = Database.SelectedDatabase;

    public int SubId; // Subject Id
    public int PTNum;   // Post Type Number
    public string PostId;
    private string fl;
    private FileResult result;
    

    public EditPostPage(int subid, string postid, string posttitel, string postdes, string fileloacaion, string DLTime)
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"XAML Error: {ex.Message}");
        }
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        TimeChecker.IsChecked = true;
        TimeChecker.IsChecked = false;
        SubId = subid;
        PostId = postid;
        //PostRadio.IsChecked = true;
        if (PostId == null) { return; }

        DeleteBtn.IsVisible = true;
        TitleEntry.Text = posttitel;
        DesEditor.Text = postdes;
        fl = fileloacaion;
        //LinkEntry.Text = Link;
        if (string.IsNullOrEmpty(DLTime))
        {
            return;
        }
        DeadLinePicker.SelectedDate = DateTime.Parse(DLTime);
        TimeChecker.IsChecked = true;
        DeadLinePicker.SelectedDate = DateTime.Parse(DLTime);
        //AssignmentRadio.IsChecked = true;

    }

    private async void BackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void DeleteClicked(object sender, EventArgs e)
    {
        int pid = int.Parse(PostId);

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
        // Perform delete operation
        await database.deleteSubjectPost(pid);
        Snackbar.ShowSnackbar(1, "تم حذف المنشور بنجاح");
        await Navigation.PopAsync();
    }

    private void TitleEntryChanged(object sender, TextChangedEventArgs e)
    {
        CheckEmpty();
    }
    private void DesEditorChanged(object sender, TextChangedEventArgs e)
    {
        CheckEmpty();
    }
    private void CheckEmpty()
    {
        if (string.IsNullOrEmpty(TitleEntry.Text) || string.IsNullOrEmpty(DesEditor.Text))
        {
            SaveBtn.IsEnabled = false;
            SaveBtn.BackgroundColor = Color.FromArgb("#D9D9D9");
        }
        else
        {
            SaveBtn.IsEnabled = true;
            SaveBtn.BackgroundColor = Color.FromArgb("#D3B05F");
        }
    }

    private void timercheckedchanged(object sender, CheckedChangedEventArgs e)
    {
        var checkedtime = sender as CheckBox;
        DeadLineBtn.IsEnabled = false;
        DeadLineBtn.BackgroundColor = Color.FromArgb("#D9D9D9");
        if (checkedtime.IsChecked)
        {
            DeadLineBtn.IsEnabled = true;
            DeadLineBtn.BackgroundColor = Color.FromArgb("#1A1A1A");
        }
    }

    /*private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var selectedRadioButton = sender as RadioButton;

        // Update the UI or show something based on the selected radio button
        if (selectedRadioButton == PostRadio)
        {
            DeadLineBtn.IsVisible = false;
            UploadDesBtn.IsVisible = false;
            PTNum = 1;
            return;
        }
        UploadDesBtn.IsVisible = true;
        DeadLineBtn.IsVisible = true;
        PTNum = 2;
    }*/
    private void DeadLineBtnClicked(object sender, EventArgs e)
    {
        DeadLinePicker.IsOpen = true;
        DeadLinePicker.SelectionView.Background = Color.FromRgba("#1a1a1a");
    }
    private async void UploadDesBtnClicked(object sender, EventArgs e)
    {
        result = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Pdf,
            PickerTitle = "Select a PDF"
        });

        if (result != null)
        {
            UploadDesBtn.BackgroundColor = Color.FromArgb("#D3B05F");
            UploadDesBtn.TextColor = Color.FromArgb("#1A1A1A");

            // Change the icon color to #1A1A1A
                //UploadDesIcon.Color = Color.FromArgb("#1A1A1A");
            
        }
    }


    private async void SaveClicked(object sender, EventArgs e)
    {

        DateTime? STime = null;
        if (TimeChecker.IsChecked)
        {
            STime = DeadLinePicker?.SelectedDate.Value;
        }
        if (DeadLinePicker.SelectedDate < DateTime.Now && TimeChecker.IsChecked)
        {
            await Task.Delay(500);
            Snackbar.ShowSnackbar(2, "يجب الا يكون اخر موعد قبل الوقت الحالي");
            //await DisplayAlert("خطا", "يجب الا يكون اخر موعد قبل الوقت الحالي", "حسنا");
            DeadLinePicker.SelectedDate = DateTime.Now.AddMinutes(1).Date;// + minute

            return;
        }


        if (string.IsNullOrEmpty(PostId))
        {
            var post = new SubjectPosts
            {
                PostTitle = TitleEntry.Text,
                PostDes = DesEditor.Text,
                SubId = SubId,
                PostDate = DateTime.Now,
                DeadLineTime = STime,
                PostFileLink = result?.FullPath,

            };
            await database.insertSubjectPost(post);
            //await DisplayAlert("تمت", "تم اضافة منشور", "حسنا");
            Snackbar.ShowSnackbar(1, "تم اضافة المنشور");
        }
        else
        {
            int pid = int.Parse(PostId);
            var existingPost = await database.getSubjectPost(pid);
            if (existingPost != null)
            {
                existingPost.PostTitle = TitleEntry.Text;
                existingPost.PostDes = DesEditor.Text;
                existingPost.DeadLineTime = STime;
                /*
				if(fileContent != null){
				existingPost.PostDesFile = fileContent;*/

                if (result != null)
                {
                    existingPost.PostFileLink = result.FullPath;

                }
                else
                    existingPost.PostFileLink = fl;
                await database.updateSubjectPost(existingPost);
                //await DisplayAlert("تمت", "تم تعديل المنشور", "حسنا");
                Snackbar.ShowSnackbar(1, "تم تعديل المنشور");
            }
        }

        await Navigation.PopAsync();
    }

    /*private async void SaveClicked(object sender, EventArgs e)
    {

        if (!string.IsNullOrEmpty(LinkEntry.Text) && !Uri.IsWellFormedUriString(LinkEntry.Text, UriKind.Absolute))
        {
            Snackbar.ShowSnackbar(2, "الرابط الذي أدخلته غير صالح");
            //await DisplayAlert("خطأ", "الرابط الذي أدخلته غير صحيح. يرجى إدخال رابط صالح.", "حسنا");
            return;
        }

        if (string.IsNullOrEmpty(PostId))
        {
            var post = new SubjectPosts
            {
                PostTitle = TitleEntry.Text,
                PostDes = DesEditor.Text,
                SubId = SubId,
                PostDate = DateTime.Now,
                PostFileLink = LinkEntry.Text,
            };
            await database.insertSubjectPost(post);
            //await DisplayAlert("تمت", "تم اضافة منشور", "حسنا");
            Snackbar.ShowSnackbar(1, "تم اضافة المنشور");
        }
        else
        {
            int pid = int.Parse(PostId);
            var existingPost = await database.getSubjectPost(pid);
            if (existingPost != null)
            {
                existingPost.PostTitle = TitleEntry.Text;
                existingPost.PostDes = DesEditor.Text;
                existingPost.PostFileLink = LinkEntry.Text;
                await database.updateSubjectPost(existingPost);
                Snackbar.ShowSnackbar(1, "تم تعديل المنشور");
                //await DisplayAlert("تمت", "تم تعديل المنشور", "حسنا");
            }
        }

        await Navigation.PopAsync();
    }*/
}