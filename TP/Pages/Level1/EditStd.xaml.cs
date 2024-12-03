using TP.Methods;

namespace TP.Pages.Level1;

public partial class EditStd : ContentPage
{
    private EditStdViewModel _viewModel;

    public EditStd()
    {
        InitializeComponent();
        _viewModel = new EditStdViewModel();
        BindingContext = _viewModel;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.SaveStudentAsync();
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.DeleteStudentAsync();
    }
}
