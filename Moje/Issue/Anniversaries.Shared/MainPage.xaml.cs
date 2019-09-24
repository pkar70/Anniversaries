


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Anniversaries
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        private string mEvents = "";
        private string mHolid = "";
        private string mBirths = "";
        private string mDeaths = "";
        // private string mObceMiesiace = "";
        // zmienne w Setup dostepne
        private DateTimeOffset mDate;
        // Dim mObceJezyki As String = "pl de fr es ru"
        private string mPreferredLang = "pl";
        private string mCurrLang = "";
        private string mCurrPart = "";

        private void bSetup_Click(object sender, RoutedEventArgs e)
        {
//            this.Frame.Navigate(typeof(Setup));
        }

        private void bInfo_Click(object sender, RoutedEventArgs e)
        {
//            this.Frame.Navigate(typeof(InfoAbout));
        }


        private async void bRead_Click(object sender, RoutedEventArgs e)
        {
        }

        private void bEvent_Click(object sender, RoutedEventArgs e)
        {
        }
        private void bHolid_Click(object sender, RoutedEventArgs e)
        {
        }
        private void bBirth_Click(object sender, RoutedEventArgs e)
        {
        }
        private void bDeath_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string sTmp;
            sTmp = "EBD"; // pkar.GetSettingsString("EnabledTabs", "EBD");

            bEvent.IsEnabled = (sTmp.IndexOf("E",StringComparison.Ordinal ) > -1);
            bBirth.IsEnabled = (sTmp.IndexOf("B", StringComparison.Ordinal) > -1);
            bDeath.IsEnabled = (sTmp.IndexOf("D", StringComparison.Ordinal) > -1);

            mDate = DateTime.Now;
            uiDay.Date = mDate;
        }


    }
}
