﻿using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Pages.Teacher;
using TP.Pages.Student;
using TP.Methods.actions;
using SQLitePCL;

namespace TP.Pages;

public partial class SubjectSelectionPage : ContentPage
{

    Database database = Database.SelectedDatabase;
    //private MineSQLite _sqlite = new MineSQLite();

    public ObservableCollection<SubTable> Subjects { get; set; }
   
    public SubjectSelectionPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        Subjects = new ObservableCollection<SubTable>();
        HideContentViewMethod.HideContentView(SaveSession, SessionBorder);
        HideContentViewMethod.HideContentView(AddSubPopupWindow, AddSubBorder);
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSubjects();
        await Task.Delay(1000);
        CheckSession();

    }
    private async void OnPullToRefreshRefreshing(object sender, EventArgs args)
    {
        pulltorefresh.IsRefreshing = true;
        await Task.Delay(2000);
        await LoadSubjects();
        pulltorefresh.IsRefreshing = false;
    }

    //load data depends on UserType if he's Teacher or Student
    private async Task LoadSubjects()
    {

        Subjects.Clear();

        NoSubExist.IsVisible = false;
        switch (UserSession.UserType)
        {
            case 1: // Teacher
                AddBtn.IsVisible = true;
                CalenderBtn.IsVisible = true;
                var teacherSubjects = await database.getSubByUser();
                if (teacherSubjects.Count == 0 )
                {
                    NoSubExist.IsVisible = true;
                    NoSubExistSubTitle.Text = " يمكنك اضافة مواد عن طريق الزر الموجود بالاعلى يمين";
                    return;
                }
                foreach (var subject in teacherSubjects)
                {
                    Subjects.Add(subject);
                }
                SubList.ItemsSource = Subjects;
                break;

            case 2: // Student
                SearchBtn.IsVisible = true;
                CalenderBtn.IsVisible = false;

                // Fetch data from the database
                var stdInSub = await database.getDegreeBySessionName() ?? new List<DegreeTable>();
                var allSubjects = await database.getSubTable() ?? new List<SubTable>();

                // Loop through and find matching subjects
                foreach (var studentSubject in stdInSub.Where(s => s != null)) // Ensure no null entries
                {
                    foreach (var subject in allSubjects.Where(s => s != null)) // Ensure no null entries
                    {
                        if (studentSubject.SubId == subject.SubId)
                        {
                            Subjects.Add(subject);
                        }
                    }
                }

                // Handle case where no subjects are found
                if (Subjects.Count == 0)
                {
                    NoSubExist.IsVisible = true;
                    NoSubExistSubTitle.Text = "يمكنك انضمام للمواد عن طريق الزر الموجود بالاعلى يمين";
                    return;
                }

                // Bind the subjects list to the UI
                SubList.ItemsSource = Subjects;
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
    private void AddClicked(object sender, EventArgs e){
        AddSubPopupWindow.IsVisible = true;
    }
    private async void SearchBtnClicked(object sender, EventArgs e){
        await Navigation.PushAsync(new RequestSubjectPage());
    }
    private async void NewsBtnClicked(object sender, EventArgs e){
        await Navigation.PushAsync(new GeneralPostsPage());
    }
    private async void CalenderClicked(object sender, EventArgs e)
    {
        //await _sqlite.CheckSchedulerExist();
        await Navigation.PushAsync(new CalenderPage());
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
            Snackbar.ShowSnackbar(2, "حدث خطأ أثناء الحذف!");
            //await DisplayAlert("خطا", "يجب وضع اسم للمادة", "حسنا");
            return;
        }
        try{
           var Sub = new SubTable{
               SubName = SubNameEntry.Text.ToLower(),
               UserId = UserSession.UserId,
               SubTeacherName = UserSession.Name,
               ShowDeg = false,
               };
           await database.insertSub(Sub);
           SubNameEntry.Text = "";
           AddSubPopupWindow.IsVisible = false;
           await LoadSubjects();
           Snackbar.ShowSnackbar(1, "تم انشاء المادة بنجاح");
           //await DisplayAlert("تمت", "تم انشاء المادة بنجاح", "حسنا"); 
        }
        catch (Exception ex){
            Snackbar.ShowSnackbar(2, $"{ex.Message}");
            //await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
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
