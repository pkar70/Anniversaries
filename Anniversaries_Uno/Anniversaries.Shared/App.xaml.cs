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
    partial class App : p.PkApplication 
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
        internal async override System.Threading.Tasks.Task<string> AppServiceLocalCommand(string sCommand)
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
