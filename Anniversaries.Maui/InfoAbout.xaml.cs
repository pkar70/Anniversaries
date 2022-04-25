using System;


using Microsoft.Maui;
using Microsoft.Maui.Controls;

using static ExtensionsUWPMAUI;

#region "UWP-MAUI conversions"
//using RoutedEventArgs = System.EventArgs;
//using WebViewNavigationStartingEventArgs = Microsoft.Maui.Controls.WebNavigatingEventArgs;
//using Page = Microsoft.Maui.Controls.ContentPage;
//using StackPanel = Microsoft.Maui.Controls.StackLayout;

// this.Frame.GoBack();  => this.GoBack(); // Frame jest niestety used do innych celów, nie mo¿na go aliasowaæ

#endregion


namespace Anniversaries
{

    public sealed partial class InfoAbout : ContentPage
    {
        public InfoAbout()
        {
            this.InitializeComponent();
        }


        private void bInfoOk(object sender, EventArgs e) // RoutedEventArgs e)
        {
            this.GoBack(); // .Frame.GoBack(); 
        }

        private void Page_Load(object sender, EventArgs e) // RoutedEventArgs e)
        {

            this.ShowAppVers();

            string sTmp;

            sTmp = "https://en.wikipedia.org/wiki/";
            sTmp += VBlib.MainPage.MonthNo2EnName(DateTime.Now.Month);
            sTmp = sTmp + "_" + DateTime.Now.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);

            uiWikiLink.Content = sTmp;
            uiWikiLink.NavigateUri = new Uri(sTmp);

        }

#if WINDOWS10_0_17763_0_OR_GREATER 
        private async void bRateIt_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {// nie wejdzie tu przy !UWP, bo wygasza wtedy Button; #if ¿eby nie by³o warninga
            Uri sUri = new Uri("ms-windows-store://review/?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            await Windows.System.Launcher.LaunchUriAsync(sUri);
    }
#endif
    }


}

