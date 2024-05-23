using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace WinEmlReader
{
    /// <summary>
    /// the main page of the app, which accepts the dragged EML file and navigates to the EMlPage
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
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

        // open new file with `FileOpen` button
        private async void FileOpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the file picker
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List, // Set the view mode of the file picker
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary // Consider to change it adaptively
            };

            // Add the filter of the file picker
            picker.FileTypeFilter.Add(".eml");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Frame.Navigate(typeof(EmlPage), file);
            }
        }

        // Open the donation page with the `Donate` button
        private async void DonateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Just Kidding",
                Content = "In development.",
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();

            // // Open the donation page
            // Frame.Navigate(typeof(DonationPage));
        }
    }
}
