using SQLite;
using Syncfusion.Maui.Data;
using System;
using System.Collections.ObjectModel;
using TP.Methods.actions;

namespace TP.Pages.Teacher;

public partial class RequestMangment : ContentPage{
    private MineSQLite _sqlite = new MineSQLite();

    public ObservableCollection<RequestJoinSubject> RequestsColl { get; set; }
    public int SubIds;
    public RequestMangment(int subid)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable nAavigation bar for this page


        SubIds = subid;
        
        RequestsColl = new ObservableCollection<RequestJoinSubject>();
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRequests();
    }
    private async void BackClicked(object sender, EventArgs e){
        await Navigation.PopAsync();
    }
    private async Task LoadRequests(){
        RequestsColl.Clear();
        var requests =  await _sqlite.getRequestJoinSubjectsBySubId(SubIds);
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
            await _sqlite.insertDegree(std);
        }
        
        await Task.Delay(500);

        await _sqlite.deleteRequestJoin(swipedItem.ReqId);
        RequestsColl.Remove(swipedItem);
      

    }
}