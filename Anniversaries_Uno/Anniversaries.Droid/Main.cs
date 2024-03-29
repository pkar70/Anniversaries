using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;
using Windows.UI.Xaml.Media;

namespace Anniversaries.Droid
{
    [global::Android.App.ApplicationAttribute(
        Label = "@string/ApplicationName",
        LargeHeap = true,
        HardwareAccelerated = true,
        Theme = "@style/AppTheme"
    )]
    public class Application : Windows.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
           //  : base(new App(), javaReference, transfer)    // linia dla starszej wersji
           : base(() => new App(), javaReference, transfer) // linia dla nowszej wersji
        {
            ConfigureUniversalImageLoader();
            //Uno.UI.FeatureConfiguration.Style.UseUWPDefaultStyles = false; // PROBA DODANIA - nieudana; 

            // wymagane dla dzia�ania CalendarDatePicker, obecnego w Uno 3.8.6
            Uno.UI.FeatureConfiguration.Popup.UseNativePopup = false;
        }

        private void ConfigureUniversalImageLoader()
        {
            // Create global configuration and initialize ImageLoader with this config
            ImageLoaderConfiguration config = new ImageLoaderConfiguration
                .Builder(Context)
                .Build();

            ImageLoader.Instance.Init(config);

            ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
        }
    }
}
