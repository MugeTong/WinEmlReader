using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinEmlReader
{
    public partial class EmlPage
    {
        public EmlPage()
        {
            InitializeComponent();
        }

        // Load the EML file
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Get the EML file from the navigation parameter
            var file = e.Parameter as Windows.Storage.StorageFile;
            // Read the EML file
            var eml = await Windows.Storage.FileIO.ReadTextAsync(file);
            // Display the EML file in the WebView
            var dlg = new ContentDialog
                { Title = "Unsupported File", Content = "Can NOT open this file.", CloseButtonText = "OK" };
            await dlg.ShowAsync();
        }
    }
}
