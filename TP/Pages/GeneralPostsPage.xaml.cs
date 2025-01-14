using System.Collections.ObjectModel;
using TP.Methods;
using TP.Methods.actions;

namespace TP.Pages;

public partial class GeneralPostsPage : ContentPage
{
    Database database = Database.SelectedDatabase;

    public ObservableCollection<SubjectPosts> Posts { get; set; }
    public string LinkUrl;

    public GeneralPostsPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        Posts = new ObservableCollection<SubjectPosts>();
        HideContentViewMethod.HideContentView(PostPopupWindow, PostBorder);

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPosts();
    }
    private async Task LoadPosts()
    {
        EmptyMessage.IsVisible = false;
        Posts.Clear();
        var data = await database.getGeneralPosts();
        if (data.Count == 0)
        {
            EmptyMessage.IsVisible = true;
            return;
        }
        var posts = data
            .OrderByDescending(b => b.PostDate)
            .ToList();
        foreach (var post in posts)
        {
            Posts.Add(post);
        }
        Postslistview.ItemsSource = Posts;
    }
    private async void BackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void SelectionPostChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {

        OpenLinkBtn.IsVisible = false;

        var SelectedPost = Postslistview.SelectedItem as SubjectPosts;

        IdLblPopup.Text = SelectedPost.PostId.ToString();
        TitleLblPopup.Text = SelectedPost.PostTitle;
        DesLblPopup.Text = SelectedPost.PostDes;
        //DeadLineTimeLblPopup.Text = SelectedPost.DeadLineTime.ToString();
        Postslistview.SelectedItem = null;
        LinkUrl = SelectedPost.PostFileLink;
        if (!string.IsNullOrEmpty(LinkUrl))
        {
            OpenLinkBtn.IsVisible = true;
        }

        PostPopupWindow.IsVisible = true;

    }

    private async void OpenLinkBtnClicked(object sender, EventArgs e)
    {
        if (Uri.IsWellFormedUriString(LinkUrl, UriKind.Absolute))
        {
            await Launcher.OpenAsync(LinkUrl);
        }
    }
    private void CancelPostClicked(object sender, EventArgs e)
    {
        PostPopupWindow.IsVisible = false;
    }
}