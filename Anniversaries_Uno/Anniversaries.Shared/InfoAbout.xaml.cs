using System;


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
            this.GoBack(); // .Navigate(typeof(MainPage));
        }

        private void Page_Load(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            this.ShowAppVers();

            string sTmp;

            sTmp = "https://en.wikipedia.org/wiki/";
            sTmp += VBlib.MainPage.MonthNo2EnName(DateTime.Now.Month);
            sTmp = sTmp + "_" + DateTime.Now.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);

            uiWikiLink.Content = sTmp;
            uiWikiLink.NavigateUri = new Uri(sTmp);

        }

#if NETFX_CORE
        private async void bRateIt_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {// nie wejdzie tu przy !UWP, bo wygasza wtedy Button; #if żeby nie było warninga
            Uri sUri = new Uri("ms-windows-store://review/?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            await Windows.System.Launcher.LaunchUriAsync(sUri);
    }
#endif
    }


}

