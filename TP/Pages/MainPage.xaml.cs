using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Pages.Level1;
using TP.Pages.Teacher;

namespace TP.Pages;

public partial class MainPage : ContentPage
{
    private readonly SQLiteAsyncConnection _database;
    public ObservableCollection<SubTable> Subjects { get; set; }
    public MainPage()
	{
		InitializeComponent();

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _database = new SQLiteAsyncConnection(dbPath);
        if(UserSession.UserType == 2){
        Subjects = new ObservableCollection<SubTable>();
        AddBtn.IsVisible = true;
        BindingContext = this;
            return;
        }
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSubjects();
    }
    private async Task LoadSubjects()
    {
        if (UserSession.UserType == 2)
        {
            var subjects = await _database.Table<SubTable>().Where(s => s.UserId == UserSession.UserId).ToListAsync();
            Subjects.Clear();
            foreach (var subject in subjects)
            {
                Subjects.Add(subject);
            }
            return;
        }
    }
    private async void AddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditSubject(null, null, null, null, null, 1));
    }
    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        // Get the selected item
        var selectedItem = e.CurrentSelection.FirstOrDefault() as SubTable;

        if (selectedItem != null)
        {
            // Navigate to the detail page, passing the selected item's ID
            await Navigation.PushAsync(new RequestMangment(selectedItem.SubId));

            // Clear the selection (optional)
            var collectionView = sender as CollectionView;
            if (collectionView != null)
            {
                collectionView.SelectedItem = null;
            }
        }
    }

}