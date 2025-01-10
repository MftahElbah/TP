using System.Threading.Tasks;

namespace TP.Pages
{
    public partial class YesNoContentView : ContentView
    {
        private TaskCompletionSource<bool> _taskCompletionSource;

        public YesNoContentView()
        {
            InitializeComponent();
        }

        // Show the popup and wait for the user's response
        public Task<bool> ShowAsync()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            this.IsVisible = true;  // Show the popup
            return _taskCompletionSource.Task;  // Wait for user action
        }

        // Handle Yes button click
        private void YesClicked(object sender, EventArgs e)
        {
            _taskCompletionSource?.SetResult(true);  // Return true
            this.IsVisible = false;  // Hide the popup
        }

        // Handle No button click
        private void NoClicked(object sender, EventArgs e)
        {
            _taskCompletionSource?.SetResult(false);  // Return false
            this.IsVisible = false;  // Hide the popup
        }
    }
}
