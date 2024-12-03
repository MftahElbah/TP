using TP.Methods;
namespace TP;

public partial class testpage : ContentPage
{
	public testpage()
	{
	
        InitializeComponent();

        BindingContext = new DepBranchViewModel();

    }
}