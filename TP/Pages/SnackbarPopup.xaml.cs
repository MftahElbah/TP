using TP.Methods;

namespace TP.Pages;

public partial class SnackbarPopup : ContentView
{
    


    public SnackbarPopup()
    {
        InitializeComponent();
        ConBoarder.IsVisible = !UserSession.internet;
    }

}