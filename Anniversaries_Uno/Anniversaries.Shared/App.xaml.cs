using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using vb14 = VBlib.pkarlibmodule14;

namespace Anniversaries
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    partial class App : Application
    {

        private Window _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
#if __ANDROID__
            // ConfigureFilters(Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
            InitializeLogging(); // nowsze Uno
            
#endif 
            this.InitializeComponent();
            // this.Suspending += OnSuspending; // komentuje, bo OnSuspending jest i tak puste
        }




        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

#if NET5_0 && WINDOWS
            _window = new Window();
            _window.Activate();
#else
            _window = Windows.UI.Xaml.Window.Current;
#endif

            Frame rootFrame = OnLaunchFragment(_window);

#if !(NET5_0 && WINDOWS)
            if (args.PrelaunchActivated == false)
#endif
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), args.Arguments);
                }
                // Ensure the current window is active
                _window.Activate();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal async System.Threading.Tasks.Task<string> AppServiceLocalCommand(string sCommand)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return "NO";
        }

        //' CommandLine, Toasts
        protected override async void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            if (await OnActivatedFragment(args)) return;    // command line już załatwiona w MyApp

            //' jesli nie cmdline (a np. toast), albo cmdline bez parametrow, to pokazujemy okno
            Windows.UI.Xaml.Controls.Frame rootFrame = OnLaunchFragment(Windows.UI.Xaml.Window.Current);

            if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.ToastNotification)
                rootFrame.Navigate(typeof(MainPage));

            Windows.UI.Xaml.Window.Current.Activate();
        }

        protected Windows.UI.Xaml.Controls.Frame OnLaunchFragment(Windows.UI.Xaml.Window win)
        {
            Windows.UI.Xaml.Controls.Frame mRootFrame = win.Content as Windows.UI.Xaml.Controls.Frame;

            //' Do not repeat app initialization when the Window already has content,
            //' just ensure that the window is active

            if (mRootFrame is null)
            {
                //' Create a Frame to act as the navigation context and navigate to the first page
                mRootFrame = new Windows.UI.Xaml.Controls.Frame();

                mRootFrame.NavigationFailed += OnNavigationFailed;

                //' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
                mRootFrame.Navigated += OnNavigatedAddBackButton;
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += OnBackButtonPressed;

                //' Place the frame in the current Window
                win.Content = mRootFrame;

                p.k.InitLib(null);
            }

            return mRootFrame;
        }

        void OnNavigationFailed(object sender, Windows.UI.Xaml.Navigation.NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        #region "Back button"

        private void OnNavigatedAddBackButton(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            try
            {
                Windows.UI.Xaml.Controls.Frame oFrame = sender as Windows.UI.Xaml.Controls.Frame;
                if (oFrame is null) return;

                Windows.UI.Core.SystemNavigationManager oNavig = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();


                if (oFrame.CanGoBack)
                    oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
                else
                    oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;

                return;
            }
            catch (Exception ex)
            {
                p.k.CrashMessageExit("@OnNavigatedAddBackButton", ex.Message);
            }
        }

        private void OnBackButtonPressed(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            try
            {
                (Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame)?.GoBack();
                e.Handled = true;
            }
            catch { }
        }

        #endregion

        //  RemoteSystems, Timer
        protected override async void OnBackgroundActivated(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args)
        {
            moTaskDeferal = args.TaskInstance.GetDeferral(); // w pkarmodule.App


            bool bNoComplete = false;
            bool bObsluzone = false;

            //' lista komend danej aplikacji
            string sLocalCmds = "";

            //' zwroci false gdy to nie jest RemoteSystem; gdy true, to zainicjalizowało odbieranie
            if (!bObsluzone) bNoComplete = RemSysInit(args, sLocalCmds);

            if (!bNoComplete) moTaskDeferal.Complete();
        }


        //' CommandLine, Toasts
        protected async System.Threading.Tasks.Task<bool> OnActivatedFragment(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            //' to jest m.in. dla Toast i tak dalej?

            //' próba czy to commandline
            if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.CommandLineLaunch)
            {

                Windows.ApplicationModel.Activation.CommandLineActivatedEventArgs commandLine = args as Windows.ApplicationModel.Activation.CommandLineActivatedEventArgs;
                Windows.ApplicationModel.Activation.CommandLineActivationOperation operation = commandLine?.Operation;
                string strArgs = operation?.Arguments;


                p.k.InitLib(strArgs.Split(' ').ToList<string>()); // mamy command line, próbujemy zrobić z tego string() (.Net Standard 1.4)

                if (!string.IsNullOrEmpty(strArgs))
                {
                    await ObsluzCommandLineAsync(strArgs);
                    Windows.UI.Xaml.Window.Current.Close();
                }
                return true;
            }


            return false;   // to Toast, proszę przejść do odpowiedniej strony
        }

        #region "RemoteSystem/Background"

        private Windows.ApplicationModel.Background.BackgroundTaskDeferral moTaskDeferal = null;
        private Windows.ApplicationModel.AppService.AppServiceConnection moAppConn;
        private string msLocalCmdsHelp = "";

        private void RemSysOnServiceClosed(Windows.ApplicationModel.AppService.AppServiceConnection appCon, Windows.ApplicationModel.AppService.AppServiceClosedEventArgs args)
        {
            if (appCon != null) appCon.Dispose();
            if (moTaskDeferal != null)
            {
                moTaskDeferal.Complete();
                moTaskDeferal = null;
            }
        }

        private void RemSysOnTaskCanceled(Windows.ApplicationModel.Background.IBackgroundTaskInstance sender, Windows.ApplicationModel.Background.BackgroundTaskCancellationReason reason)
        {
            if (moTaskDeferal != null)
            {
                moTaskDeferal.Complete();
                moTaskDeferal = null;
            }
        }

        ///<summary>
        ///do sprawdzania w OnBackgroundActivated
        ///jak zwróci True, to znaczy że nie wolno zwalniać moTaskDeferal !
        ///sLocalCmdsHelp: tekst do odesłania na HELP
        ///</summary>
        public bool RemSysInit(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args, string sLocalCmdsHelp)
        {
            Windows.ApplicationModel.AppService.AppServiceTriggerDetails oDetails =
             args.TaskInstance.TriggerDetails as Windows.ApplicationModel.AppService.AppServiceTriggerDetails;
            if (oDetails is null) return false;

            msLocalCmdsHelp = sLocalCmdsHelp;

            args.TaskInstance.Canceled += RemSysOnTaskCanceled;
            moAppConn = oDetails.AppServiceConnection;
            moAppConn.RequestReceived += RemSysOnRequestReceived;
            moAppConn.ServiceClosed += RemSysOnServiceClosed;
            return true;
        }

        public async System.Threading.Tasks.Task<string> CmdLineOrRemSysAsync(string sCommand)
        {
            string sResult = p.k.AppServiceStdCmd(sCommand, msLocalCmdsHelp);
            if (string.IsNullOrEmpty(sResult))
                sResult = await AppServiceLocalCommand(sCommand);

            return sResult;
        }

        public async System.Threading.Tasks.Task ObsluzCommandLineAsync(string sCommand)

        {
            Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.TemporaryFolder;
            if (oFold is null) return;

            string sLockFilepathname = System.IO.Path.Combine(oFold.Path, "cmdline.lock");
            string sResultFilepathname = System.IO.Path.Combine(oFold.Path, "stdout.txt");

            try
            {
                System.IO.File.WriteAllText(sLockFilepathname, "lock");
            }
            catch
            {
                return;
            }

            string sResult = await CmdLineOrRemSysAsync(sCommand);
            if (string.IsNullOrEmpty(sResult))
                sResult = "(empty - probably unrecognized command)";

            System.IO.File.WriteAllText(sResultFilepathname, sResult);

            System.IO.File.Delete(sLockFilepathname);
        }

        private async void RemSysOnRequestReceived(Windows.ApplicationModel.AppService.AppServiceConnection sender, Windows.ApplicationModel.AppService.AppServiceRequestReceivedEventArgs args)
        {
            // 'Get a deferral so we can use an awaitable API to respond to the message

            string sStatus;
            string sResult = "";
            Windows.ApplicationModel.AppService.AppServiceDeferral messageDeferral = args.GetDeferral();

            if (vb14.GetSettingsBool("remoteSystemDisabled"))
            {
                sStatus = "No permission";
            }
            else
            {
                Windows.Foundation.Collections.ValueSet oInputMsg = args.Request.Message;

                sStatus = "ERROR while processing command";

                if (oInputMsg.ContainsKey("command"))
                {

                    String sCommand = (string)oInputMsg["command"];
                    sResult = await CmdLineOrRemSysAsync(sCommand);
                }

                if (sResult != "") sStatus = "OK";
            }

            Windows.Foundation.Collections.ValueSet oResultMsg = new Windows.Foundation.Collections.ValueSet();
            oResultMsg.Add("status", sStatus);
            oResultMsg.Add("result", sResult);

            await args.Request.SendResponseAsync(oResultMsg);

            messageDeferral.Complete();
            moTaskDeferal.Complete();
        }


        #endregion



        #region "logging"

#if !NETFX_CORE
        // nowsze Uno
        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {

            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

//#if HAS_UNO
//            global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
//#endif
        }
#endif

        #endregion

    }
}
