using SQLite;
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
        if (e.Offset < 250)
        {
            return;
        }
        var swipedItem = e.DataItem as RequestJoinSubject;
        if (swipedItem != null) { return; }


        if (e.Direction == SwipeDirection.Right) {
            var std = new SubForStdTable
            {
                SubId = SubIds,
                StdId = swipedItem.UserId,
                Deg = 0,
                MiddelDeg = 0,

            };
            await _database.InsertAsync(std);
        }
        if (e.Direction == SwipeDirection.Left) {
            var req = await _database.Table<RequestJoinSubject>().FirstOrDefaultAsync(d => d.ReqId == swipedItem.ReqId);
            if (req == null)
            {
                await DisplayAlert("Success", "حدث خطاء", "OK");
                return;
            }
            await _database.DeleteAsync(req);
        }

    }
}