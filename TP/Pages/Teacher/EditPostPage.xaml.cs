using SQLite;

namespace TP.Pages.Teacher;

public partial class EditPostPage : ContentPage
{
    public readonly SQLiteAsyncConnection _database;

    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

    public EditPostPage(int subid)
	{
		InitializeComponent();
		SubId = subid;
		_database = new SQLiteAsyncConnection(dbPath);
	}

    private void TitleEntryChanged(object sender, TextChangedEventArgs e)
	{
		CheckEmpty();
    }
	private void DesEditorChanged(object sender, TextChangedEventArgs e)
	{
		CheckEmpty();
    }

    private async void SaveClicked(object sender, EventArgs e) {
		var post = new SubjectPosts
		{
			PostTitle = TitleEntry.Text,
			PostDes = DesEditor.Text,
			SubId = SubId,
            PostDate = DateTime.Now,
		};
        await _database.InsertAsync(post);
        await DisplayAlert("تمت", "تم اضافة منشور", "حسنا");
		await Navigation.PopAsync();
    }

    public void CheckEmpty()
	{
		if (string.IsNullOrEmpty(TitleEntry.Text) || string.IsNullOrEmpty(DesEditor.Text))
		{
			SaveBtn.IsEnabled = false;
			SaveBtn.Background = Colors.Gray;
		}
		else {
            SaveBtn.IsEnabled = true;
            SaveBtn.Background = Colors.Gold;
        }
	}


}