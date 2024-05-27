using System;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinEmlReader
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            // Register the encoding provider to decode with different encodings.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            // if the app is prelaunched, do nothing
            if (e.PrelaunchActivated) return;

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // Register the back requested event
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            rootFrame.Navigated += RootFrame_Navigated;
        }

        // change the back button visibility after navigation
        private static void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame)) return;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        // Handle the back requested event
        private static void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            // Check that no one has already handled this
            if (e.Handled) return;

            // Default is to navigate back within the Frame
            if (!(Window.Current.Content is Frame frame && frame.CanGoBack)) return;

            // Navigate back
            frame.GoBack();
            // Signal handled so the system doesn't navigate back through the app stack
            e.Handled = true;
        }


        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
