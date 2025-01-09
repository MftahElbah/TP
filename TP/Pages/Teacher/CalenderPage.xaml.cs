using AndroidX.Core.Util;
using Firebase;
using Java.Lang.Ref;
using SQLite;
using Syncfusion.Maui.Picker;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using TP.Methods;
using TP.Methods.actions;

namespace TP.Pages.Teacher;

public partial class CalenderPage : ContentPage
{
    private ObservableCollection<SchedulerAppointment> _appointments;

    //private MineSQLite _sqlite = new MineSQLite();
    Database database = Database.SelectedDatabase;

    private int ViewChange = 0;
    int ColorChange = 0;
    bool IfStartOrEndTime; //false = start , true = end
    string TaskId;
    DateTime NowTime = DateTime.Now;
    private DateTime StartTime;
    private DateTime EndTime;


    
    public CalenderPage(){
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
        _appointments = new ObservableCollection<SchedulerAppointment>();
        HideContentViewMethod.HideContentView(TaskPopupWindow, TaskPopupBorder);
        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("ar-SA");

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("ar-SA");
        await LoadTasks();

        //PageShowStatus(1);
        Scheduler.AppointmentDragStarting += OnAppointmentDragStarting;
        Scheduler.AppointmentDrop += OnAppointmentDrop;
        Scheduler.Tapped += Scheduler_Tapped;
        Scheduler.LongPressed += Scheduler_LongPressed;
    }
    private async Task LoadTasks()
    {
        _appointments.Clear();
        var data = await database.getTaskTableByUserId();
        _appointments = new ObservableCollection<SchedulerAppointment>(
        data.Select(t => new SchedulerAppointment
        {
            Id = t.TaskId,
            StartTime = t.TaskStartTime,
            EndTime = t.TaskEndTime,
            Subject = t.TaskTitle,
            Notes = t.TaskDes,
            //Background = Color.FromRgba(t.TaskColor) // Use Background property for color
            Background = new SolidColorBrush(Color.FromRgba(t.TaskColor)) // Use Background property for color
        }));

        Scheduler.AppointmentsSource = _appointments;
    }
    private async void ChangeSchedulerViewClicked(object sender, EventArgs e)
    {
        ViewChange = (ViewChange + 1) % 3;
        // 0 = Week , 1 = Day , 2 = Moneth
        switch (ViewChange)
        {
            case 0:
                await Task.Delay(500);
                Scheduler.View = SchedulerView.Week;
            break;
            case 1:
                await Task.Delay(500);
                Scheduler.View = SchedulerView.Day;
                break;
            case 2:
                await Task.Delay(500);
                Scheduler.View = SchedulerView.Month;
                break;
        }
        
    }
    private void CleanTaskPopup()
    {
        CreateTaskBtn.IsVisible = true;
        EditTaskBtn.IsVisible = false;
        TaskId = string.Empty;
        TaskTitleEntry.Text = string.Empty;
        TaskDesEntry.Text = string.Empty;
        StartTime = new DateTime(NowTime.Year, NowTime.Month, NowTime.Day, NowTime.Hour + 1, 0, 0);
        EndTime = StartTime.AddHours(1);
        TaskColorBtn.BackgroundColor = Color.FromRgba("#213555");
        ColorChange = 0;
        PopupTitle.Text = "إضافة تذكير";

    }
    private async Task<bool> TaskValidation()
    {
        if (string.IsNullOrWhiteSpace(TaskTitleEntry.Text))
        {
            await DisplayAlert("خطا", "العنوان مطلوب.", "موافق");
            return true;
        }
        if (EndTime <= StartTime)
        {
            await DisplayAlert("Validation Error", "يجب أن يكون وقت الانتهاء بعد وقت البدء.", "موافق");
            return true;
        }
        bool checker = await database.TaskTimeConflict(StartTime, EndTime,TaskId);
        if (checker)
        {
            await DisplayAlert("Time Conflict", "لديك بالفعل مهمة مجدولة في هذا الوقت.", "موافق");
            return true;
        }
        return false;
    }
    public string ColorToHex(Color color)
    {
        int red = (int)(color.Red * 255);
        int green = (int)(color.Green * 255);
        int blue = (int)(color.Blue * 255);
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    private async void BackClicked(object sender, EventArgs e){
        await Navigation.PopAsync();
    }
    private void OnAddTaskClicked(object sender, EventArgs e)
    {
        CleanTaskPopup();
        TaskPopupWindow.IsVisible = true;
    }
    private async void Scheduler_Tapped(object sender, SchedulerTappedEventArgs e){

        if (!(e.Element == SchedulerElement.Appointment && e.Appointments != null && e.Appointments.Count > 0)) return;
        
        CleanTaskPopup();
        var selectedAppointment = (SchedulerAppointment)e.Appointments[0];
        int appointmentId = selectedAppointment.Id != null ? (int)selectedAppointment.Id : 0;
        var result = await database.getTaskByID(appointmentId);
        TaskId = Convert.ToString(result.TaskId);
        TaskTitleEntry.Text = result.TaskTitle;
        TaskDesEntry.Text = result.TaskDes;
        StartTime = result.TaskStartTime;
        EndTime = result.TaskEndTime;
        TaskColorBtn.BackgroundColor = Color.FromArgb(result.TaskColor);
        PopupTitle.Text = "تعديل تذكير";
        TaskPopupWindow.IsVisible= true;
        CreateTaskBtn.IsVisible = false;
        EditTaskBtn.IsVisible = true;
    }

    private void CancelTaskClick(object sender, EventArgs e) { 
        TaskPopupWindow.IsVisible = false;
        CleanTaskPopup();
    }
    private async void CreateTaskBtnClick(object sender, EventArgs e) {
        bool res = await TaskValidation();
        if (res)
        {
            return;
        }
        var inserted = new SchedulerTask
        {
            TaskTitle = TaskTitleEntry.Text,
            TaskDes = TaskDesEntry.Text,
            TaskStartTime = StartTime,
            TaskEndTime = EndTime,
            TaskColor = ColorToHex(TaskColorBtn.BackgroundColor),
            UserId = UserSession.UserId
        };
        await database.insertTask(inserted);

        await DisplayAlert("نجحت العملية", "نجحت اضافة التذكير", "حسنا");

        TaskPopupWindow.IsVisible = false;
        await LoadTasks();
        CleanTaskPopup();
    }
    private async void EditTaskBtnClick(object sender, EventArgs e) {
        bool res = await TaskValidation();
        if (res)
        {
            return;
        }
        var task = await database.getTaskByID(int.Parse(TaskId));
        task.TaskTitle = TaskTitleEntry.Text;
        task.TaskDes = TaskDesEntry.Text; 
        task.TaskStartTime = StartTime;
        task.TaskEndTime = EndTime;
        task.TaskColor = ColorToHex(TaskColorBtn.BackgroundColor);
        await database.updateTask(task);
        await DisplayAlert("نجحت العملية", "نجحت تعديل التذكير", "حسنا");
        TaskPopupWindow.IsVisible = false;
        await LoadTasks(); CleanTaskPopup();
    }
    private void StartTimeBtnClicked(object sender, EventArgs e)
    {
        TimePicker.IsOpen = true;
        IfStartOrEndTime = false;
        TimePicker.SelectedDate = StartTime;
        TimePicker.SelectionView.Background = Color.FromRgba("#1a1a1a");
    }
    private void EndTimeBtnClicked(object sender, EventArgs e)
    {
        TimePicker.IsOpen = true;
        IfStartOrEndTime = true;
        TimePicker.SelectedDate = EndTime;
        TimePicker.SelectionView.Background = Color.FromRgba("#1a1a1a");
    }
    private void TimePickerChanged(object sender, DateTimePickerSelectionChangedEventArgs e)
    {
        if (!IfStartOrEndTime)
        {
            StartTime = TimePicker.SelectedDate.Value ;
        }
        else
        {
            EndTime = TimePicker.SelectedDate.Value ;
        }
    }

    private async void TaskColorBtnClicked(object sender, EventArgs e){
        ColorChange = (ColorChange + 1) % 7;
        switch (ColorChange) { 
            case 0:
                await Task.Delay(500);
                TaskColorBtn.BackgroundColor = Color.FromRgba("#213555");
            break;
            case 1:
                await Task.Delay(500);
                TaskColorBtn.BackgroundColor = Color.FromRgba("#6A669D");
            break;
            case 2:
                await Task.Delay(500);
                TaskColorBtn.BackgroundColor = Color.FromRgba("#1F4529");
            break;
            case 3:
                await Task.Delay(500);
                TaskColorBtn.BackgroundColor = Color.FromRgba("#006A67");
            break;
            case 4:
                await Task.Delay(500);
                TaskColorBtn.BackgroundColor = Color.FromRgba("#EC8305");
            break;
            case 5:
                await Task.Delay(500);
                TaskColorBtn.BackgroundColor = Color.FromRgba("#5D0E41");
            break;
            case 6:
                await Task.Delay(500);
                TaskColorBtn.BackgroundColor = Color.FromRgba("#B8001F");
            break;
        }


    }


    /*public static Color GetColorFromName(string colorName)
    {
        if (string.IsNullOrEmpty(colorName))
        {
            Console.WriteLine("colorName is null or empty");
            return Colors.LightSkyBlue; // Default color if null
        }

        Console.WriteLine($"Getting color for name: {colorName}");

        // If the color name starts with a '#', it's a hex color code
        if (colorName.StartsWith("#"))
        {
            return Color.FromArgb(colorName);
        }

        // Look up the color name in the dictionary
        if (colorMapping.TryGetValue(colorName, out Color color))
        {
            return color;
        }

        // Return a default color if the name is not found in the dictionary
        return Colors.LightSkyBlue;
    }*/

    private async void Scheduler_LongPressed(object sender, SchedulerLongPressedEventArgs e)
    {
        var result = await DisplayAlert("متأكد ؟", "هل انت متأكد من الحذف", "نعم", "لا");
        if (!result)
        {
            return;
        }
        var selectedAppointment = (SchedulerAppointment)e.Appointments[0];
        int appointmentId = selectedAppointment.Id != null ? (int)selectedAppointment.Id : 0;

        await database.deleteTask(appointmentId);
        await LoadTasks();
    }
    private void OnAppointmentDragStarting(object sender, AppointmentDragStartingEventArgs e)
    {
        // Prevent drag
        e.Cancel = true;
    }
    private void OnAppointmentDrop(object sender, AppointmentDropEventArgs e)
    {
        // Prevent drop
        e.Cancel = true;
    }

}