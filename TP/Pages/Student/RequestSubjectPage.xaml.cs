﻿using SQLite;
using Syncfusion.Maui.Buttons;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Student;

public partial class RequestSubjectPage : ContentPage
{
    public ObservableCollection<SubTable> Subjects { get; set; }
    private readonly SQLiteAsyncConnection _database;
    public RequestSubjectPage()
	{
		InitializeComponent();
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _database = new SQLiteAsyncConnection(dbPath);
        Subjects = new ObservableCollection<SubTable>();
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fetch all subjects from the SubTable
        await LoadAvailableSubjects();
    }

    private async Task LoadAvailableSubjects()
    {
        var subjects = await _database.Table<SubTable>().ToListAsync();
        /*int SubIdForSearch;
        string UserNameForSearch;*/
        Subjects.Clear();
        foreach (var subject in subjects)
        {
            /*SubIdForSearch = subject.SubId;
            UserIdForSearch = UserSession.Name;*/

            var SubInReq = await _database.Table<RequestJoinSubject>().Where(s => s.SubId == subject.SubId && s.UserId == UserSession.UserId).ToListAsync();
            var StdInTable = await _database.Table<DegreeTable>().Where(s => s.SubId == subject.SubId && s.StdName == UserSession.Name).ToListAsync();
            if (SubInReq.Count == 0 && StdInTable.Count == 0) {
                Subjects.Add(subject);
            }
        }
    }
    private async void OnSendRequestClicked(object sender, EventArgs e)
    {
        var button = sender as SfButton;
        var subject = button.BindingContext as SubTable;
        if(button.Text == "تم الأرسال")
        {
            return;
        }
        if (subject != null)
        {
            // Create a new request and insert it into the database
            var request = new RequestJoinSubject
            {
                UserId = UserSession.UserId, // Assuming UserSession.UserId is set
                SubId = subject.SubId,
                Name = UserSession.Name,
                RequestDate = DateTime.Now,
            };
            await _database.InsertAsync(request);
            button.Text = "تم الأرسال";
            button.Background = Colors.Gray;
        }
    }
}