using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Anniversaries
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application // Windows.UI.Xaml.
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
           // ConfigureFilters(Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);

            this.InitializeComponent();
            // this.Suspending += OnSuspending; // komentuje, bo OnSuspending jest i tak puste
        }


        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

            Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                //' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
                rootFrame.Navigated += OnNavigatedAddBackButton;
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += OnBackButtonPressed;

            }

#if NETFX_CORE
            if (e != null && e.PrelaunchActivated == true) return;
#endif

            if (rootFrame.Content == null)
                {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Windows.UI.Xaml.Window.Current.Activate();
        }


        private void OnNavigatedAddBackButton(object sender, NavigationEventArgs e)
        {
            try
            {
                Frame oFrame = sender as Frame;
                if (oFrame == null)
                    return;

                Windows.UI.Core.SystemNavigationManager oNavig = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();

                if (oFrame.CanGoBack)
                    oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
                else
                    oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;

                return;
            }
            catch (Exception ex)
            {
                pkar.CrashMessageExit("@OnNavigatedAddBackButton", ex.Message);
            }
        }


        private void OnBackButtonPressed(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            try
            {
                (Window.Current.Content as Frame).GoBack();
                e.Handled = true;
            }
            catch { }
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

    }
}
