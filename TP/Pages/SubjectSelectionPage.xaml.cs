using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Pages.Teacher;
using TP.Pages.Student;
using TP.Methods.actions;

namespace TP.Pages;

public partial class SubjectSelectionPage : ContentPage
{

    Database database = Database.SelectedDatabase;

    public ObservableCollection<SubTable> Subjects { get; set; }
   
    public SubjectSelectionPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        Subjects = new ObservableCollection<SubTable>();
        HideContentViewMethod.HideContentView(SaveSession);
        HideContentViewMethod.HideContentView(AddSubPopupWindow);
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSubjects();
        await Task.Delay(1000);
        CheckSession();

    }

    //load data depends on UserType if he's Teacher or Student
    private async Task LoadSubjects()
    {

        Subjects.Clear();

        switch (UserSession.UserType)
        {
            case 1: // Teacher
                AddBtn.IsVisible = true;
                NoSubExist.IsVisible = false;
                var teacherSubjects = await database.getSubByUser();
                if (teacherSubjects.Count == 0)
                {
                    NoSubExist.IsVisible = true;
                    NoSubExistSubTitle.Text = " يمكنك اضافة مواد عن طريق الزر الموجود بالاعلى يمين";
                    return;
                }
                foreach (var subject in teacherSubjects)
                {
                    Subjects.Add(subject);
                }
                break;

            case 2: // Student
                SearchBtn.IsVisible = true;
                var stdInSub = await database.getDegreeBySessionName();
                var allSubjects = await database.getSubTable();

                foreach (var studentSubject in stdInSub)
                {
                    foreach (var subject in allSubjects)
                    {
                        if (studentSubject.SubId == subject.SubId)
                        {
                            Subjects.Add(subject);
                        }
                    }
                }

                if (Subjects.Count == 0)
                {
                    NoSubExist.IsVisible = true;
                    NoSubExistSubTitle.Text = "يمكنك انضمام للمواد عن طريق الزر الموجود بالاعلى يمين";
                }
                
                break;
        }

    }
    private void CheckSession()
    {
        //Check if it's already saved session
        if (UserSession.SessionYesNo) { 
            return;
        }
        //if he clicked Yes or No the message well not popup again
        SaveSession.IsVisible = true;
        UserSession.SessionYesNo = true;
    }
    private async void SaveSessionClicked(object sender, EventArgs e)
    {
            var session = new UserSessionTable
            {
                UserId = UserSession.UserId,
                Password = UserSession.Password,
            };
            await database.insertSession(session);

        SaveSession.IsVisible = false;
    }
    private void CancelSessionClicked(object sender, EventArgs e)
    {
        SaveSession.IsVisible = false;
    }
/*    private void BackgroundTapped(object sender, EventArgs e)
    {
        SaveSession.IsVisible = false; // Hide the modal
    }*/

    private void AddClicked(object sender, EventArgs e){
        AddSubPopupWindow.IsVisible = true;
    }
    private async void SearchBtnClicked(object sender, EventArgs e){
        await Navigation.PushAsync(new RequestSubjectPage());
    }
    private void LogoutClicked(object sender, EventArgs e)
    {
        //to not show navigation bar
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
        }
    }

    private async void CreateSubClick(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SubNameEntry.Text)){
            await DisplayAlert("خطا", "يجب وضع اسم للمادة", "حسنا");
            return;
        }
        try{
           var Sub = new SubTable{
               SubName = SubNameEntry.Text,
               UserId = UserSession.UserId,
               SubTeacherName = UserSession.Name,
               ShowDeg = false,
               };
           await database.insertSub(Sub);
           await DisplayAlert("تمت", "تم انشاء المادة بنجاح", "حسنا"); 
        }
        catch (Exception ex){
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        SubNameEntry.Text = "";
        AddSubPopupWindow.IsVisible = false;
        await LoadSubjects();
    }
    private void CancelSubClick(object sender, EventArgs e)
    {
        AddSubPopupWindow.IsVisible = false;
    }

    private async void OnItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        // Get the selected item
        if (e.DataItem is SubTable subtapped)
        {
            switch (UserSession.UserType){
                case 1:
                await Navigation.PushAsync(new SubjectCenterTeacher(subtapped.SubId));
                break;

                case 2:
                await Navigation.PushAsync(new SubjectCenterStd(subtapped.SubId, subtapped.ShowDeg));
                break;
            }
        }
    }

}