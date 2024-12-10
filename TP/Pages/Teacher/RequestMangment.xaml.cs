using SQLite;
using Syncfusion.Maui.Data;
using System;
using System.Collections.ObjectModel;

namespace TP.Pages.Teacher;

public partial class RequestMangment : ContentPage
{
    private readonly SQLiteAsyncConnection _database;
    public ObservableCollection<RequestJoinSubject> RequestsColl { get; set; }
    public int SubIds;
    public RequestMangment(int subid)
    {
        InitializeComponent();
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _database = new SQLiteAsyncConnection(dbPath);
        SubIds = subid;
        
        RequestsColl = new ObservableCollection<RequestJoinSubject>();
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRequests();
    }
    private async Task LoadRequests()
    {
        var requests = await _database.Table<RequestJoinSubject>().Where(d => d.SubId == SubIds).ToListAsync();
        RequestsColl.Clear();
        foreach (var req in requests)
        {
            RequestsColl.Add(req);
        }
    }

    private async void LVSwipEnd(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.Offset < 200)
        {
            return;
        }
        
        var swipedItem = e.DataItem as RequestJoinSubject;
        uid.Text = swipedItem.UserId.ToString();
        sid.Text = SubIds.ToString();
        
        listview.SwipeOffset = listview.Width;

        if (e.Direction == SwipeDirection.Right)
        {
            var std = new DegreeTable
            {
                SubId = SubIds,
                StdName = swipedItem.Name,
                Deg = 0,
                MiddelDeg = 0,
            };
            await _database.InsertAsync(std);
        }
        
        await Task.Delay(2000);

        var req = await _database.Table<RequestJoinSubject>().FirstOrDefaultAsync(d => d.ReqId == swipedItem.ReqId);
        await _database.DeleteAsync(req);
        RequestsColl.Remove(swipedItem);
      

    }
}