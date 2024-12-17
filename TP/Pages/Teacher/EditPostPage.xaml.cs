using SQLite;

namespace TP.Pages.Teacher;

public partial class EditPostPage : ContentPage
{
    public readonly SQLiteAsyncConnection _database;
    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

	public int SubId;
	public string PostId;
    public EditPostPage(int subid , string postid , string posttitel , string postdes)
	{
		InitializeComponent();
		SubId = subid;
		_database = new SQLiteAsyncConnection(dbPath);
		PostId = postid;

		if(PostId == null) {return;}

		DeleteBtn.IsVisible = true;
		TitleEntry.Text = posttitel;
		DesEditor.Text = postdes;
	}

    private void TitleEntryChanged(object sender, TextChangedEventArgs e)
	{
		CheckEmpty();
    }
	private void DesEditorChanged(object sender, TextChangedEventArgs e)
	{
		CheckEmpty();
    }

	
	private async void SaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(PostId))
		{
			var post = new SubjectPosts
			{
				PostTitle = TitleEntry.Text,
				PostDes = DesEditor.Text,
				SubId = SubId,
				PostDate = DateTime.Now,
			};
			await _database.InsertAsync(post);
			await DisplayAlert("تمت", "تم اضافة منشور", "حسنا");
		}
		else
		{
			int pid = int.Parse(PostId);
            var existingPost = await _database.Table<SubjectPosts>().FirstOrDefaultAsync(p => p.PostId == pid);
			if (existingPost != null)
			{
				existingPost.PostTitle = TitleEntry.Text;
				existingPost.PostDes = DesEditor.Text;

				await _database.UpdateAsync(existingPost);
				await DisplayAlert("تمت", "تم تعديل المنشور", "حسنا");
			}
		}
			await Navigation.PopAsync();
	}

    private async void DeleteClicked(object sender, EventArgs e)
    {
        int pid = int.Parse(PostId);
        bool confirm = await DisplayAlert("تأكيد الحذف", "هل أنت متأكد أنك تريد حذف هذا المنشور؟", "نعم", "لا");
        if (!confirm)
        {
			return;
        }
            // Perform delete operation
        var postToDelete = await _database.Table<SubjectPosts>().FirstOrDefaultAsync(p => p.PostId == pid);
        await _database.DeleteAsync(postToDelete);
        await DisplayAlert("تم الحذف", "تم حذف المنشور بنجاح", "حسنا");
        await Navigation.PopAsync();
    }

    public void CheckEmpty()
	{
		if (string.IsNullOrEmpty(TitleEntry.Text) || string.IsNullOrEmpty(DesEditor.Text))
		{
			SaveBtn.IsEnabled = false;
			SaveBtn.BackgroundColor = Color.FromArgb("#D9D9D9");
		}
		else {
            SaveBtn.IsEnabled = true;
            SaveBtn.BackgroundColor = Color.FromArgb("#D3B05F");
        }
	}


}