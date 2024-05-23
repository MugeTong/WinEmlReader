using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using MimeKit;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
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
            // By this step, the file must be an EML file (class: Windows.Storage.StorageFile)
            var message = await ReadEmlFile(e.Parameter as Windows.Storage.StorageFile);
            if (message == null)
            {
                Frame.GoBack();  // If the file is not a valid EML file, go back to the previous page
                return;
            }

            // Display the email file subject in the WebView
            ApplicationView.GetForCurrentView().Title = message.Subject;

            RenderMimeMessage(message);
        }

        // Read the EML file
        private static async Task<MimeMessage> ReadEmlFile(Windows.Storage.StorageFile emlFile)
        {
            try
            {
                // read the stream and use mimekit to parse the message
                var stream = await emlFile.OpenStreamForReadAsync();
                var options = new ParserOptions
                {
                    CharsetEncoding = Encoding.GetEncoding("gb2312")
                };
                return await MimeMessage.LoadAsync(options, stream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                DisplayError("Format Exception", "[0x0002]Empty email file or invalid file.", "OK");
            }

            return null;
        }

        // Render the MimeMessage in the WebView
        private void RenderMimeMessage(MimeMessage message)
        {
            // Create a new StringBuilder
            var builder = new StringBuilder();
            // Append the HTML header
            builder.Append("<html><head><meta charset='utf-8'></head><body>");
            // Append the HTML body
            builder.Append(message.HtmlBody);
            // Append the HTML footer
            builder.Append("</body></html>");
            // Display the HTML content in the WebView
            EmlBodyWebView.NavigateToString(builder.ToString());

        }

        // Display the error message
        private static async void DisplayError(string title, string content, string closeButtonText)
        {
            var dialog = new ContentDialog { Title = title, Content = content, CloseButtonText = closeButtonText };
            await dialog.ShowAsync();
        }

        // Accept the dragged item
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;
        }

        // Drop the dragged item, and navigate to the EmlPage
        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            // Check if the dragged item is a StorageItem
            if (!e.DataView.Contains(StandardDataFormats.StorageItems)) return;

            // Get the dragged items
            var items = await e.DataView.GetStorageItemsAsync();
            var index = 0; // Initialize the index of the dragged item
            foreach (var item in items)
            {
                // Check if the file is an EML file
                if (item is Windows.Storage.StorageFile file && file.FileType.ToLower() == ".eml")
                {
                    if (index == 0)
                    {
                        // If the file is the first file, navigate to the EmlPage with the EML file
                        Frame.Navigate(typeof(EmlPage), file);
                    }
                    else
                    {
                        // If the file is not the first file, open the file on this app with a new instance
                        // However, without tests, we closed the ablity to open multiple instances of the app
                        var options = new Windows.System.LauncherOptions
                        {
                            TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName
                        };
                        await Windows.System.Launcher.LaunchFileAsync(file, options);
                    }
                }

                index++;
            }
        }
    }
}
