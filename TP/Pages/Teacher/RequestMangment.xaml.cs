﻿using SQLite;
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
        if (e.Offset < 400)
        {
            return;
        }
        
        var swipedItem = e.DataItem as RequestJoinSubject;
        uid.Text = swipedItem.UserId.ToString();
        sid.Text = SubIds.ToString();
        
        listview.SwipeOffset = listview.Width;
        PopupMessageFrame.IsVisible = false;
        PopupMessageLbl.Text = "تم الرفض";

        if (e.Direction == SwipeDirection.Right) {
            var std = new DegreeTable
            {
                SubId = SubIds,
                StdName = swipedItem.Name,
                Deg = 0,
                MiddelDeg = 0,
            };
            await _database.InsertAsync(std);
            PopupMessageLbl.Text = "تم القبول";
        }
        e.Offset = listview.Width;
        PopupMessageFrame.IsVisible = true;
            await Task.Delay(1000);

        var req = await _database.Table<RequestJoinSubject>().FirstOrDefaultAsync(d => d.ReqId == swipedItem.ReqId);
        await _database.DeleteAsync(req);
        
        
        RequestsColl.Remove(swipedItem);

            await Task.Delay(1000);
        PopupMessageFrame.IsVisible = false;
    }
}