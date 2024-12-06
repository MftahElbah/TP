using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
using TP.Pages.Level1;

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
        Subjects = new ObservableCollection<SubTable>();
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSubjects();
    }
    private async Task LoadSubjects()
    {
        var subjects = await _database.Table<SubTable>().Where(s => s.UserId == UserSession.UserId).ToListAsync();
        Subjects.Clear();
        foreach (var subject in subjects)
        {
            Subjects.Add(subject);
        }
    }
    private async void AddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditSubject(null, null, null, null, null, 1));
    }
}