using SQLite;
using Syncfusion.Maui.Data;
using Syncfusion.Maui.ProgressBar;
using System.Collections.ObjectModel;
using TP.Methods;

namespace TP.Pages.Others;

public partial class SignupPage : ContentPage
{
    private ObservableCollection<StepProgressBarItem> StepItems { get; set; }
    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    public readonly SQLiteAsyncConnection _database;

    public int StepStatus;
    public SignupPage()
    {
        InitializeComponent();

        StepItems = new ObservableCollection<StepProgressBarItem>
        {
            new StepProgressBarItem { PrimaryText = "معلومات المستخدم" },
            new StepProgressBarItem { PrimaryText = "معلومات الحساب" }
        };
        stepProgress.ItemsSource = StepItems;
        StepStatus = 0;
        UpdateStepView(StepStatus);
        _database = new SQLiteAsyncConnection(dbPath);
    }
    private void UNEChanged(object sender, TextChangedEventArgs e)
    {
        // Use regex to allow only English letters
        string filteredText = new string(e.NewTextValue.Where(char.IsLetter).ToArray());

        // If the text contains invalid characters, revert to the filtered text
        if (e.NewTextValue != filteredText)
        {
            UsernameEntry.Text = filteredText;
        }
    }

    private async void NextSaveBtnClicked(object sender, EventArgs e){
        if (StepStatus == 0)
        {
            if (String.IsNullOrEmpty(IdEntry.Text) || String.IsNullOrEmpty(FirstNameEntry.Text) || String.IsNullOrEmpty(LastNameEntry.Text))
            {
                await DisplayAlert("خطاء", "يجب ملئ جميع الحقول","حسنا");
                return;
            }
            int id = int.Parse(IdEntry.Text);
            var userid= await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId== id);
            if (userid != null) { 
                await DisplayAlert("خطاء", "رقم الدراسي المكتوب موجود بالفعل","حسنا");
                return;
            }
            if (IdEntry.Text.Length != 9)
            {
                await DisplayAlert("خطاء", "يجب ان يكون الرقم الدراسي يتكون من 9 ارقام","حسنا");
                return;
            }
            StepStatus++;
            UpdateStepView(StepStatus);
            return;
        }
        if (String.IsNullOrEmpty(UsernameEntry.Text) || String.IsNullOrEmpty(PasswordEntry.Text) || String.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("خطاء", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }
        string us = UsernameEntry.Text.ToLower();
        var username= await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username== us);
        if (username != null) { 
            await DisplayAlert("خطاء", "اسم المستخدم المكتوب موجود بالفعل","حسنا");
            return;
        }
        if (PasswordEntry.Text.Length < 8) {
            await DisplayAlert("خطاء", "يجب ان يكون كلمة السر يتكون من 8 حروف على الأقل", "حسنا");
            return;
        }
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("خطاء", "كلمة السر غير متشابهة", "حسنا");
            return;
        }

        var newuser = new UsersAccountTable
        {
            UserId = int.Parse(IdEntry.Text),
            Name = $"{FirstNameEntry.Text} {LastNameEntry.Text}",
            Username = UsernameEntry.Text.ToLower(),
            Password = PasswordEntry.Text,
            UserType = 3
        };
        await _database.InsertAsync(newuser);
        await DisplayAlert("نجحت", "تم التسجيل بنجاح", "حسنا");

    }
    private void PreCancelBtnClicked(object sender, EventArgs e){
        if (StepStatus == 1)
        {
            StepStatus--;
            UpdateStepView(StepStatus);
            return;
        }
    }

    private void UpdateStepView(int step)
    {
        if (step == 0)
        {
            IdTxtInL.IsVisible = true;
            FirstNameTxtInL.IsVisible = true;
            LastNameTxtInL.IsVisible = true;

            UsernameTxtInL.IsVisible = false;
            PasswordTxtInL.IsVisible = false;
            ConfirmPasswordTxtInL.IsVisible = false;
            NextSaveBtn.Text = "التالي";
            PreCancelBtn.Text = "الغاء";
            stepProgress.ActiveStepIndex = 0;
        }
        else if (step == 1)
        {
            IdTxtInL.IsVisible = false;
            FirstNameTxtInL.IsVisible = false;
            LastNameTxtInL.IsVisible = false;

            UsernameTxtInL.IsVisible = true;
            PasswordTxtInL.IsVisible = true;
            ConfirmPasswordTxtInL.IsVisible = true;
            NextSaveBtn.Text = "تسجيل";
            PreCancelBtn.Text = "السابق";
            stepProgress.ActiveStepIndex = 1;
        }
    }
}