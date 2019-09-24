using System;

//using Windows.UI.Xaml;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Anniversaries
{
    
    public sealed partial class InfoAbout : Windows.UI.Xaml.Controls.Page
    {
        public InfoAbout()
        {
            this.InitializeComponent();
        }

        // The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

        private void bInfoOk(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Page_Load(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if NETFX_CORE
#else
            uiBarRateIt.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            uiBarSeparat.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            uiInfoPort.Visibility = Windows.UI.Xaml.Visibility.Visible;
#endif

            string sTmp;

            sTmp = "https://en.wikipedia.org/wiki/";
            sTmp = sTmp + MainPage.MonthNo2EnName(DateTime.Now.Month);
            sTmp = sTmp + "_" + DateTime.Now.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);

            uiWikiLink.Content = sTmp;
            uiWikiLink.NavigateUri = new Uri(sTmp);

            
        }

        private async void bRateIt_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {// nie wejdzie tu przy !UWP, bo wygasza wtedy Button 
            Uri sUri = new Uri("ms-windows-store://review/?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            await Windows.System.Launcher.LaunchUriAsync(sUri);
        }
    }


}

