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


        private void bInfoOk(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.GoBack(); // .Navigate(typeof(MainPage));
        }

        private void Page_Load(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            string sTmp;

            sTmp = "https://en.wikipedia.org/wiki/";
            sTmp = sTmp + MainPage.MonthNo2EnName(DateTime.Now.Month);
            sTmp = sTmp + "_" + DateTime.Now.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);

            uiWikiLink.Content = sTmp;
            uiWikiLink.NavigateUri = new Uri(sTmp);

        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously - dla Android
        private async void bRateIt_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {// nie wejdzie tu przy !UWP, bo wygasza wtedy Button; #if żeby nie było warninga
#if NETFX_CORE
            Uri sUri = new Uri("ms-windows-store://review/?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            await Windows.System.Launcher.LaunchUriAsync(sUri);
#endif
        }
    }


}

