using Syncfusion.Maui.Data;
using System.Collections.ObjectModel;
using TP.Methods;
namespace TP.Pages.Level1;

public partial class EditDepBranch : ContentPage
{
    
    private readonly DatabaseHelper _databaseHelper;
    public string _Id;
    public string _Name1;
    public string _Name2;
    public int _TypeDataGrid;


    public EditDepBranch(string Id,string Name1,string Name2, int TypeDataGrid)
	{
        BindingContext = new DepBranchViewModel();


		InitializeComponent();




        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        _databaseHelper = new DatabaseHelper(dbPath);

        

        _Id = Id;
        _Name1 = Name1;
        _Name2 = Name2;
        _TypeDataGrid = TypeDataGrid;
        ChickWhichViewShow(_TypeDataGrid);
        // Set up the page based on whether it's adding or editing
        if (_Id != null)
        {
            // Editing an existing department
            Title = "تعديل";
            NameEntry.Text = Name1;
            EnableButtons(false);
            DeleteButton.IsVisible = true;
            if(_Name2 != null)
            {
             DepartmentComboBox.Text = _Name2;
            }

        }
        else
        {
            // Adding a new department
            Title = "اضافة";
            EnableButtons(true);
            DeleteButton.IsVisible = false;
        }


    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        string departmentName = NameEntry.Text;

        // Validate the input
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {

            await DisplayAlert("Error", "Please enter a all valids.", "OK");
            return;
        }

        try
        {
            if (_Id == null)
            {
                if (_TypeDataGrid == 1){
                    // Add a new department
                    var newDepartment = new DepTable { DepName = NameEntry.Text };
                    await _databaseHelper._database.InsertAsync(newDepartment);
                    //the idea is when u add new department add branch automatcly
                    var newBranch = new BranchTable { BranchName = NameEntry.Text , DepName = NameEntry.Text };
                    await _databaseHelper._database.InsertAsync(newBranch);
                    await DisplayAlert("Success", "تمت اضافة القسم بنجاح", "OK");
                }
                else if (_TypeDataGrid == 2) {
                    if (string.IsNullOrWhiteSpace(DepartmentComboBox.Text))
                    {
                        await DisplayAlert("Error", "Please enter a all valids.", "OK");
                        return;
                    }
                    var newBranch = new BranchTable { BranchName = NameEntry.Text , DepName = DepartmentComboBox.Text };
                    await _databaseHelper._database.InsertAsync(newBranch);
                    await DisplayAlert("Success", "تمت اضافة الشعبة بنجاح", "OK");
                }
            }
            else
            {
                if (_TypeDataGrid == 1) { 
                // Update an existing department using UpdateDepartmentAsync
                await _databaseHelper.UpdateDepartmentAsync(int.Parse(_Id), NameEntry.Text);
                await DisplayAlert("Success", "تم تعديل القسم بنجاح", "OK");// Update an existing department using UpdateDepartmentAsync
                }

                else if (_TypeDataGrid == 2) { 
                
                await _databaseHelper.UpdateBranchAsync(int.Parse(_Id), NameEntry.Text,DepartmentComboBox.Text);
                await DisplayAlert("Success", "تم تعديل الشعبة بنجاح", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

        // Navigate back to the previous page
        await Navigation.PopAsync();
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_Id))
        {
            await DisplayAlert("Error", "Please enter a valid department name.", "OK");
            
        }
        else
        {
            if (_TypeDataGrid == 1)
            {
                await _databaseHelper.DeleteDepartmentAsync(int.Parse(_Id));
                await DisplayAlert("Success", "Deleted successfully!", "OK");
            }
            else if (_TypeDataGrid == 2) {
                await _databaseHelper.DeleteBranchAsync(int.Parse(_Id));
                await DisplayAlert("Success", "Deleted successfully!", "OK");
            }
            
            await Navigation.PopAsync();
        }



    }

    private async void DepViewShowerClicked(object sender, EventArgs e)
    {
        _TypeDataGrid = 1;
        ChickWhichViewShow(_TypeDataGrid);
    }
    private async void BranchViewShowerClicked(object sender, EventArgs e)
    {
        _TypeDataGrid = 2;
        ChickWhichViewShow(_TypeDataGrid);
    }

    private void EnableButtons(bool enable) 
    {
        if (!enable)
        {
            DepViewShower.IsEnabled = false;
            BranchViewShower.IsEnabled = false;
        }
        else
        {
            DepViewShower.IsEnabled = true;
            BranchViewShower.IsEnabled = true;
        }
    }
    private void ChickWhichViewShow(int Ch)
    {
        //1 to Departments , 2 to Branches
       if (Ch == 1)
        {
            DepViewShower.Background = Color.FromArgb("#2374AB");
            DepViewShower.TextColor = Color.FromArgb("#DCDCDC");


            BranchViewShower.Background = Colors.Transparent;
            BranchViewShower.TextColor = Color.FromArgb("#1A1A1A");

            DepartmentComboBox.IsVisible = false;

        }
        else if (Ch == 2)
        {
            BranchViewShower.Background = Color.FromArgb("#2374AB");
            BranchViewShower.TextColor = Color.FromArgb("#DCDCDC");


            DepViewShower.Background = Colors.Transparent;
            DepViewShower.TextColor = Color.FromArgb("#1A1A1A");

            DepartmentComboBox.IsVisible = true;
        }
    }


}