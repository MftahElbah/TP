using System.Collections.ObjectModel;
namespace TP;
///<summary>
///ComboBoxFeatures class
///</summary>
public partial class ComboBoxFeatures : ContentPage
{
    ///<summary>
    ///ComboBoxFeatures constructor
    ///</summary>
	public ComboBoxFeatures()
	{
		InitializeComponent();
	}
}
///<summary>
///ComboBoxFeatures Model class
///</summary>
public class SocialMediaClass
{
    public string Name { get; set; }
    public int ID { get; set; }
}
///<summary>
///ComboBoxFeatures ViewModel class
///</summary>
public class SocialMediaViewModels
{
    public ObservableCollection<SocialMediaClass> SocialMedias { get; set; }
    ///<summary>
    ///ComboBoxFeatures ViewModel class constructor
    ///</summary>
    public SocialMediaViewModels()
    {
        this.SocialMedias = new ObservableCollection<SocialMediaClass>();
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Facebook", ID = 0 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Google Plus", ID = 1 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Instagram", ID = 2 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "LinkedIn", ID = 3 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Skype", ID = 4 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Telegram", ID = 5 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Televzr", ID = 6 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Tik Tok", ID = 7 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Tout", ID = 8 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Tumblr", ID = 9 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Twitter", ID = 10 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "Vimeo", ID = 11 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "WhatsApp", ID = 12 });
        this.SocialMedias.Add(new SocialMediaClass() { Name = "YouTube", ID = 13 });
    }
}
