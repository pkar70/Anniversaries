﻿/*


 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using pkar.UI.Extensions;
using vb14 = VBlib.pkarlibmodule14;
using static p.Extensions;


namespace Anniversaries
{

    public partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.ProgRingInit(false,true);
            VBlib.pkarlibmodule14.LangEnsureInit(false);
        }

        private static DateTimeOffset mDate;

        private void bSetup_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate(typeof(Setup));
        }

        private void bInfo_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate(typeof(InfoAbout));
        }


        private async void bRead_Click(object sender, RoutedEventArgs e)
        {
            vb14.DumpCurrMethod();

            string sUrl;

            sUrl = "https://en.wikipedia.org/wiki/" + VBlib.MainPage.MonthNo2EnName(mDate.Month) + "_" + mDate.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);
            VBlib.MainPage.bRead_Click_Reset();
            // mHolid = "";
            tbDzien.Text = "Reading EN...";

            // p.k.GetSettingsString("EnabledTabs", "EBD")'
            string sTxt = await VBlib.MainPage.ReadOneLang(new Uri(sUrl));
            if(sTxt =="")
            {
                // nie udalo sie wczytac strony - i to tej glownej, ktora nam daje linki do pozostałych
                vb14.DialogBox($"ERROR getting page\n{sUrl}");
                return; // skoro i tak nie przejdziemy do pozostałych, to możemy zrezygnować
            }
            sUrl = vb14.GetSettingsString("EnabledLanguages");
            IReadOnlyList<string> lList = VBlib.MainPage.ExtractLangLinks(sUrl, sTxt);

            this.ProgRingShow(true, false, 0, 1 + lList.Count);
            this.ProgRingInc();
            //uiProgBar.Maximum = 1 + lList.Count;
            //uiProgBar.Visibility = Visibility.Visible;
            //uiProgBar.Value = 1;

            foreach (string sUri in lList)
            {
                tbDzien.Text = "Reading " + sUri.Substring(8, 2).ToUpperInvariant() + "...";
                // p.k.GetSettingsString("EnabledTabs", "EBD")'
                await VBlib.MainPage.ReadOneLang(new Uri(sUri));
                //uiProgBar.Value++;
                this.ProgRingInc();
            }
            this.ProgRingShow(false);

            bool bZaden = true;
            if (bEvent.IsChecked.HasValue && bEvent.IsChecked.Value)
            {
                bZaden = false;
                bEvent_Click(sender, e);
            }
            //if (bHolid.IsChecked)
            //{
            //    bZaden = false;
            //    bHolid_Click(sender, e);
            //}
            if (bBirth.IsChecked.HasValue && bBirth.IsChecked.Value)
            {
                bZaden = false;
                bBirth_Click(sender, e);
            }
            if (bDeath.IsChecked.HasValue && bDeath.IsChecked.Value)
            {
                bZaden = false;
                bDeath_Click(sender, e);
            }

            if (bZaden)
                bEvent_Click(sender, e);

            tbDzien.Text = mDate.ToString("d MMMM", System.Globalization.CultureInfo.CurrentCulture);  // .Day.ToString & " " & MonthNo2PlName(mDate.Month)
        }

        //private static string MetaViewport()
        //{// kopia z Brewiarz
        //    double dScale = p.k.GetSettingsInt("fontSize", 100);   // skalowanie - w Brewiarz jest, a tu nie
        //    string sScale = "initial-scale=" + dScale.ToString("0.##");
        //    return "<meta name=\"viewport\" content=\"width=device-width, " + sScale + "\">";
        //}

        private void SetWebView(string sHtml, string sHead)
        {
            wbViewer.Height = naView.ActualHeight - 10;
            wbViewer.Width = naView.ActualWidth - 10;
            // if (sHead == "") sHead = MetaViewport(); - jest jeszcze gorzej :)
            sHtml = $"<html><head>{sHead}</head><body>{sHtml}</body></html>";
            wbViewer.NavigateToString(sHtml);
        }

        private void SetWebView(string sDoc)
        { // head (kiedyś): "<base href=""https://en.wikipedia.org/"">"
            if (string.IsNullOrEmpty(sDoc))
                vb14.DialogBoxRes("errNoData");
            else
                SetWebView(sDoc, "");
        }

        private void ToggleButtony(bool bEv, bool bBir, bool bDea)
        {
            // primary
            bEvent.IsChecked = bEv;
            //bHolid.IsChecked = false;
            bBirth.IsChecked = bBir;
            bDeath.IsChecked = bDea;
            //bEventAndroBar.IsChecked = bEv;
            ////bHolid.IsChecked = false;
            //bBirthAndroBar.IsChecked = bBir;
            //bDeathAndroBar.IsChecked = bDea;

            // oraz z menu
            //#if NETFX_CORE
            uiSelEvent.IsChecked = bEv;
            uiSelBirth.IsChecked = bBir;
            uiSelDeath.IsChecked = bDea;
            //#endif 
        }

        private void bEvent_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(VBlib.MainPage.GetContentForWebview("E")); 
            ToggleButtony(true, false, false);
        }
        //private void bHolid_Click(object sender, RoutedEventArgs e)
        //{
        //    SetWebView(mHolid, ""); // "<base href=""https://en.wikipedia.org/"">")
        //    ToggleButtony(false, false, false);
        //}
        private void bBirth_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(VBlib.MainPage.GetContentForWebview("B")); 
            ToggleButtony(false, true, false);
        }
        private void bDeath_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(VBlib.MainPage.GetContentForWebview("D")); 
            ToggleButtony(false, false, true);
        }

        private void UwpAndro()
        { // przełączanie aktywnego w Android (AppBar) i w UWP (BottomAppBar-CommandBar)
          // pierwotna wersja miała #if, ale tak chyba jest lepiej
            uiDay.Date = mDate;

#if NETFX_CORE
            // nie wiem czemu pokazywane jako nieistniejące dla Droid - skoro powinno być?
            uiDaySec.Date = mDate;
#endif
        }

        private int CmdBarWidth()
        {

            int iIconWidth, iGridWidth;
            bool bUwp = p.k.GetPlatform("uwp");

            //if (pkar.GetPlatform("uwp"))
            iIconWidth = (int)bRefresh.ActualWidth; // zakładam że to będzie zawsze widoczne, czyli dobrze policzy
            //else
            //    iIconWidth = 80;    // *TODO* jest na sztywno, bo w Android się trudno połapać :) [bo nie wiadomo co będzie pokazane]
            iGridWidth = (int)uiGrid.ActualWidth;

            System.Diagnostics.Debug.WriteLine("width: grid=" + iGridWidth.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", icon=" + iIconWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

            if (bUwp)
                iGridWidth -= (int)(2.7 * iIconWidth); // miejsce na tekst oraz "..." (UWP: 48 px)
            else
                iGridWidth -= 120;

            int iIcons = (int)Math.Floor((double)iGridWidth / iIconWidth);
            // Lumia532: 320 - 48 / 68 = 4: zgadza się :)
            return iIcons;
        }

        private void DopasowanieCmdBar_JestSecondary(bool bShowSecondaryCommands)
        {
            // guzik wejścia do Secondary menu
            Visibility bVis = (bShowSecondaryCommands) ? Visibility.Visible : Visibility.Collapsed;
#if !NETFX_CORE
            uiAndroSec.Visibility = bVis;
#endif
        }

        private void DopasowanieCmdBar_GoPages(bool bAsPrimaryCmds)
        {
            Visibility bVis = (bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiBarSeparat3.Visibility = bVis;
            uiGoSett.Visibility = bVis;
            uiGoInfo.Visibility = bVis;

            bVis = (!bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiGoSettSec.Visibility = bVis;
            uiGoInfoSec.Visibility = bVis;

            DopasowanieCmdBar_JestSecondary(!bAsPrimaryCmds);
        }

        private void DopasowanieCmdBar_SelektorStrony(bool bAsPrimaryCmds)
        {
            Visibility bVis = (bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiBarSeparat2.Visibility = bVis;
            bEvent.Visibility = bVis;
            // bHolid.Visibility = bVis;
            bBirth.Visibility = bVis;
            bDeath.Visibility = bVis;

            bVis = (!bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiSelektorStrony.Visibility = bVis;

            DopasowanieCmdBar_JestSecondary(!bAsPrimaryCmds);
        }

        private void DopasowanieCmdBar_Kalendarz(bool bAsPrimaryCmds)
        {
            Visibility bVis = (bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiKalend.Visibility = bVis;

            bVis = (!bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiKalendSec.Visibility = bVis;

            DopasowanieCmdBar_JestSecondary(!bAsPrimaryCmds);
        }

        private void DopasowanieCmdBar()
        {
            int iIcons = CmdBarWidth();

            // ustawienie w zależności od szerokości ekranu
            // Lumia 532 ma 480 px, i to są 4 ikonki + wielokropek
            // separator ma szerokość połowy, "..." trochę więcej (jakieś 3/4)


            // pomysły:
            // text + 10 + ... : sep, 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
            // text + 9 + ... : 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
            // text + 5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
            // text + 2 + ... : text, submenu przelacznika, submenu komend, wielokropek
            // (czyli migracje miedzy secondary a primarycommand)
            //System.Diagnostics.Debug.WriteLine("ikonek ma być niby " + iIcons.ToString());

            // zawsze:
            tbDzien.Visibility = Visibility.Visible;
            bRefresh.Visibility = Visibility.Visible;


            if (iIcons > 8)
            {
                // text + 7butt + 3sep = 8.5 + ... : sep, 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
                uiBarSeparat1.Visibility = Visibility.Visible;
                DopasowanieCmdBar_Kalendarz(true);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(true);

                return; // bo 9 jest także > 4 :)
            }


            if (iIcons > 7)
            {
                // text + + 7butt + 2sep = 8 + ... : 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                DopasowanieCmdBar_Kalendarz(true);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(true);

                return;
            }

            if (iIcons > 5)
            {
                // text + 5.5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                DopasowanieCmdBar_Kalendarz(true);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(false);

                return;
            }


            if (iIcons > 4)
            {
                // text + 4.5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                DopasowanieCmdBar_Kalendarz(false);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(false);

                return;
            }

            // najmniejsze

            // primary commands
            uiBarSeparat1.Visibility = Visibility.Collapsed;
            DopasowanieCmdBar_Kalendarz(false);
            DopasowanieCmdBar_SelektorStrony(false);
            DopasowanieCmdBar_GoPages(false);

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string sTmp;

            sTmp = vb14.GetSettingsString("EnabledTabs");

            bEvent.IsEnabled = sTmp.Contains('E');
            bBirth.IsEnabled = sTmp.Contains('B');
            bDeath.IsEnabled = sTmp.Contains('D');
            //bHolid.IsEnabled = (sTmp.IndexOf("H",StringComparison.Ordinal) > -1);

            mDate = DateTime.Now;

            UwpAndro();             // Uno bug override - własny AppBar
            DopasowanieCmdBar();    // liczba ikonek a szerokość 

            //vb14.InitDump(0, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path); - jest już w App.ctor

            if (vb14.GetSettingsBool("AutoLoad")) bRead_Click(null, null);

        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void wbViewer_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri == null)
                return;

            args.Cancel = true;

            if (!vb14.GetSettingsBool("LinksActive"))
                return;

            args.Uri.OpenBrowser();
        }
#pragma warning restore IDE0079 // Remove unnecessary suppression

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void uiDay_Changed(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            // 11/27/2017 9:11:28 PM  1.5.1.6  Console     Microsoft-Xbox One  10.0.16299.4037 
            // System::Nullable$1_System::DateTimeOffset_.get_Value
            // Anniversaries::MainPage.uiDay_Changed

            if (sender.Date != null)
                if (sender.Date.HasValue)
                    mDate = sender.Date.Value;
        }

        private void uiGrid_Resized(object sender, SizeChangedEventArgs e)
        {
            DopasowanieCmdBar();
        }
    }
}

