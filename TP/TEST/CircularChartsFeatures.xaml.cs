namespace TP;
///<summary>
///CircularChartsFeatures class
///</summary>
public partial class CircularChartsFeatures : ContentPage
{
	///<summary>
    ///CircularChartsFeatures constructor
    ///</summary>
	public CircularChartsFeatures()
	{
		InitializeComponent();
	}
}
///<summary>
///CircularChartsFeatures Model class
///</summary>
public class Sales
{
	public string Product { get; set; }
	public double SalesRate { get; set; }
}
///<summary>
///CircularChartsFeatures ViewModel class
///</summary>
public class CircularChartViewModel
{
	public List<Sales> Data { get; set; }
	///<summary>
    ///CircularChartsFeatures ViewModel class constructor
    ///</summary>
	public CircularChartViewModel()
	{
		Data = new List<Sales>()
		{
			new Sales(){Product = "iPad", SalesRate = 25},
			new Sales(){Product = "iPhone", SalesRate = 35},
			new Sales(){Product = "MacBook", SalesRate = 15},
			new Sales(){Product = "Mac", SalesRate = 5},
			new Sales(){Product = "Others", SalesRate = 10},
		};
	}
}
