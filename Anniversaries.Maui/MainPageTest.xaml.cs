//using Microsoft.Maui.Controls;
//using Microsoft.Maui.Essentials;
//using System;
//using System.Linq;  // daje ElementAt(0)
//using System.Collections.Generic;   // List?
//using vb14 = VBlib.pkarlibmodule14;

//namespace Anniversaries
//{
//    public partial class MainPage : ContentPage
//    {

//        public MainPage()
//        {
//            InitializeComponent();
//            // NONMAUI this.ProgRingInit(false, true);
//        }

//        private static DateTimeOffset mDate;

//        private void bSetup_Click(object sender, EventArgs e)
//        {
//            // NONMAUI this.Frame.Navigate(typeof(Setup));
//        }

//        private void bInfo_Click(object sender, EventArgs e)
//        {
//            // NONMAUI this.Frame.Navigate(typeof(InfoAbout));
//        }


//        private async void bRead_Click(object sender, EventArgs e)
//        {
//            vb14.DumpCurrMethod();

//            string sUrl;

//            sUrl = "https://en.wikipedia.org/wiki/" + VBlib.MainPage.MonthNo2EnName(mDate.Month) + "_" + mDate.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);
//            VBlib.MainPage.bRead_Click_Reset();
//            // mHolid = "";
//            tbDzien.Text = "Reading EN...";

//            // p.k.GetSettingsString("EnabledTabs", "EBD")'
//            string sTxt = await VBlib.MainPage.ReadOneLang(new Uri(sUrl));
//            if (sTxt == "")
//            {
//                // nie udalo sie wczytac strony - i to tej glownej, ktora nam daje linki do pozostałych
//                vb14.DialogBox("ERROR getting page\n" + sUrl);
//                return; // skoro i tak nie przejdziemy do pozostałych, to możemy zrezygnować
//            }
//            sUrl = vb14.GetSettingsString("EnabledLanguages");
//            IReadOnlyList<string> lList = VBlib.MainPage.ExtractLangLinks(sUrl, sTxt);

//            // NONMAUI this.ProgRingShow(true, false, 0, 1 + lList.Count);
//            // NONMAUI this.ProgRingInc();

//            foreach (string sUri in lList)
//            {
//                tbDzien.Text = "Reading " + sUri.Substring(8, 2).ToUpperInvariant() + "...";
//                // p.k.GetSettingsString("EnabledTabs", "EBD")'
//                await VBlib.MainPage.ReadOneLang(new Uri(sUri));
//                //uiProgBar.Value++;
//                // NONMAUI this.ProgRingInc();
//            }
//            // NONMAUI this.ProgRingShow(false);

//            bool bZaden = true;
//            // NOMAUI if (bEvent.IsChecked.HasValue && bEvent.IsChecked.Value)
//            {
//                bZaden = false;
//                bEvent_Click(sender, e);
//            }

//            // NOMAUI if (bBirth.IsChecked.HasValue && bBirth.IsChecked.Value)
//            {
//                bZaden = false;
//                bBirth_Click(sender, e);
//            }
//            // NOMAUI if (bDeath.IsChecked.HasValue && bDeath.IsChecked.Value)
//            {
//                bZaden = false;
//                bDeath_Click(sender, e);
//            }

//            if (bZaden)
//                bEvent_Click(sender, e);

//            tbDzien.Text = mDate.ToString("d MMMM", System.Globalization.CultureInfo.CurrentCulture);  // .Day.ToString & " " & MonthNo2PlName(mDate.Month)
//        }

//        //private static string MetaViewport()
//        //{// kopia z Brewiarz
//        //    double dScale = p.k.GetSettingsInt("fontSize", 100);   // skalowanie - w Brewiarz jest, a tu nie
//        //    string sScale = "initial-scale=" + dScale.ToString("0.##");
//        //    return "<meta name=\"viewport\" content=\"width=device-width, " + sScale + "\">";
//        //}

//        private void SetWebView(string sHtml, string sHead)
//        {
//            // NOMAUI wbViewer.Height = naView.ActualHeight - 10;
//            // NOMAUI wbViewer.Width = naView.ActualWidth - 10;
//            // if (sHead == "") sHead = MetaViewport(); - jest jeszcze gorzej :)
//            sHtml = "<html><head>" + sHead + "</head><body>" + sHtml + "</body></html>";
//            var oSource = new HtmlWebViewSource()
//            {
//                Html = sHtml
//            };
//            wbViewer.Source = oSource;

//            wbViewer.Navigating += WbViewer_Navigating;
//        }

//        private void WbViewer_Navigating(object sender, WebNavigatingEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        private void SetWebView(string sDoc)
//        { // head (kiedyś): "<base href=""https://en.wikipedia.org/"">"
//            if (string.IsNullOrEmpty(sDoc))
//                vb14.DialogBoxRes("errNoData");
//            else
//                SetWebView(sDoc, "");
//        }

//        private void ToggleButtony(bool bEv, bool bBir, bool bDea)
//        {
//            // primary
//            // NOMAUI bEvent.IsChecked = bEv;
//            //bHolid.IsChecked = false;
//            // NOMAUI bBirth.IsChecked = bBir;
//            // NOMAUI bDeath.IsChecked = bDea;
//            //bEventAndroBar.IsChecked = bEv;
//            ////bHolid.IsChecked = false;
//            //bBirthAndroBar.IsChecked = bBir;
//            //bDeathAndroBar.IsChecked = bDea;

//            // oraz z menu
//            // NOMAUI uiSelEvent.IsChecked = bEv;
//            // NOMAUI uiSelBirth.IsChecked = bBir;
//            // NOMAUI uiSelDeath.IsChecked = bDea;
//            //#endif 
//        }

//        private void bEvent_Click(object sender, EventArgs e)
//        {
//            SetWebView(VBlib.MainPage.GetContentForWebview("E"));
//            ToggleButtony(true, false, false);
//        }
//        //private void bHolid_Click(object sender, RoutedEventArgs e)
//        //{
//        //    SetWebView(mHolid, ""); // "<base href=""https://en.wikipedia.org/"">")
//        //    ToggleButtony(false, false, false);
//        //}
//        private void bBirth_Click(object sender, EventArgs e)
//        {
//            SetWebView(VBlib.MainPage.GetContentForWebview("B"));
//            ToggleButtony(false, true, false);
//        }
//        private void bDeath_Click(object sender, EventArgs e)
//        {
//            SetWebView(VBlib.MainPage.GetContentForWebview("D"));
//            ToggleButtony(false, false, true);
//        }

//        private void UwpAndro()
//        { // przełączanie aktywnego w Android (AppBar) i w UWP (BottomAppBar-CommandBar)
//          // pierwotna wersja miała #if, ale tak chyba jest lepiej
//          // NOMAUI uiDay.Date = mDate;

//            // nie wiem czemu pokazywane jako nieistniejące dla Droid - skoro powinno być?
//            // NOMAUI uiDaySec.Date = mDate;
//        }

//        private int CmdBarWidth()
//        {

//            int iIconWidth, iGridWidth;
//            bool bUwp = p.k.GetPlatform("uwp");

//            //if (pkar.GetPlatform("uwp"))
//            // NOMAUI iIconWidth = (int)bRefresh.ActualWidth; // zakładam że to będzie zawsze widoczne, czyli dobrze policzy
//            //else
//            //    iIconWidth = 80;    // *TODO* jest na sztywno, bo w Android się trudno połapać :) [bo nie wiadomo co będzie pokazane]
//            // NOMAUI iGridWidth = (int)uiGrid.ActualWidth;

//            // NOMAUI System.Diagnostics.Debug.WriteLine("width: grid=" + iGridWidth.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", icon=" + iIconWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

//            // NOMAUI if (bUwp)
//            // NOMAUI     iGridWidth -= (int)(2.7 * iIconWidth); // miejsce na tekst oraz "..." (UWP: 48 px)
//            // NOMAUI else
//            // NOMAUI             iGridWidth -= 120;

//            // NOMAUI int iIcons = (int)Math.Floor((double)iGridWidth / iIconWidth);
//            // Lumia532: 320 - 48 / 68 = 4: zgadza się :)
//            // NOMAUI return iIcons;
//            return 0;
//        }

//        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "bo to dla Android jest, w UWP jest warning")]
//        private void DopasowanieCmdBar_JestSecondary(bool bShowSecondaryCommands)
//        {
//            // guzik wejścia do Secondary menu
//            // NOMAUI uiAndroSec.Visibility = bShowSecondaryCommands;
//        }

//        private void DopasowanieCmdBar_GoPages(bool bAsPrimaryCmds)
//        {
//            // NOMAUI uiBarSeparat3.IsVisible = bAsPrimaryCmds;
//            // NOMAUI uiGoSett.IsVisible = bAsPrimaryCmds;
//            // NOMAUI uiGoInfo.IsVisible = bAsPrimaryCmds;

//            // NOMAUI uiGoSettSec.IsVisible = !bAsPrimaryCmds;
//            // NOMAUI uiGoInfoSec.IsVisible = !bAsPrimaryCmds;

//            DopasowanieCmdBar_JestSecondary(!bAsPrimaryCmds);
//        }

//        private void DopasowanieCmdBar_SelektorStrony(bool bAsPrimaryCmds)
//        {
//            // NOMAUI uiBarSeparat2.Visibility = bAsPrimaryCmds;
//            bEvent.IsVisible = bAsPrimaryCmds;
//            // bHolid.Visibility = bVis;
//            bBirth.IsVisible = bAsPrimaryCmds;
//            bDeath.IsVisible = bAsPrimaryCmds;

//            // NOMAUI uiSelektorStrony.Visibility = !bAsPrimaryCmds;

//            DopasowanieCmdBar_JestSecondary(!bAsPrimaryCmds);
//        }

//        private void DopasowanieCmdBar_Kalendarz(bool bAsPrimaryCmds)
//        {
//            // NOMAUI uiKalend.Visibility = bAsPrimaryCmds;
//            // NOMAUI uiKalendSec.Visibility = !bAsPrimaryCmds;

//            DopasowanieCmdBar_JestSecondary(!bAsPrimaryCmds);
//        }

//        private void DopasowanieCmdBar()
//        {
//            int iIcons = CmdBarWidth();

//            // ustawienie w zależności od szerokości ekranu
//            // Lumia 532 ma 480 px, i to są 4 ikonki + wielokropek
//            // separator ma szerokość połowy, "..." trochę więcej (jakieś 3/4)


//            // pomysły:
//            // text + 10 + ... : sep, 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
//            // text + 9 + ... : 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
//            // text + 5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
//            // text + 2 + ... : text, submenu przelacznika, submenu komend, wielokropek
//            // (czyli migracje miedzy secondary a primarycommand)
//            //System.Diagnostics.Debug.WriteLine("ikonek ma być niby " + iIcons.ToString());

//            // zawsze:
//            tbDzien.IsVisible = true;
//            bRefresh.IsVisible = true;


//            if (iIcons > 8)
//            {
//                // text + 7butt + 3sep = 8.5 + ... : sep, 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
//                // NONMAUI uiBarSeparat1.IsVisible = true;
//                DopasowanieCmdBar_Kalendarz(true);
//                DopasowanieCmdBar_SelektorStrony(true);
//                DopasowanieCmdBar_GoPages(true);

//                return; // bo 9 jest także > 4 :)
//            }


//            if (iIcons > 7)
//            {
//                // text + + 7butt + 2sep = 8 + ... : 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
//                // NONMAUI uiBarSeparat1.IsVisible = false;
//                DopasowanieCmdBar_Kalendarz(true);
//                DopasowanieCmdBar_SelektorStrony(true);
//                DopasowanieCmdBar_GoPages(true);

//                return;
//            }

//            if (iIcons > 5)
//            {
//                // text + 5.5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
//                // NONMAUI uiBarSeparat1.IsVisible = false;
//                DopasowanieCmdBar_Kalendarz(true);
//                DopasowanieCmdBar_SelektorStrony(true);
//                DopasowanieCmdBar_GoPages(false);

//                return;
//            }


//            if (iIcons > 4)
//            {
//                // text + 4.5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
//                // NONMAUI uiBarSeparat1.IsVisible = false;
//                DopasowanieCmdBar_Kalendarz(false);
//                DopasowanieCmdBar_SelektorStrony(true);
//                DopasowanieCmdBar_GoPages(false);

//                return;
//            }

//            // najmniejsze

//            // primary commands
//            // NONMAUI uiBarSeparat1.IsVisible = false;
//            DopasowanieCmdBar_Kalendarz(false);
//            DopasowanieCmdBar_SelektorStrony(false);
//            DopasowanieCmdBar_GoPages(false);

//        }

//        private void Page_Loaded(object sender, EventArgs e)
//        {
//            string sTmp;

//            sTmp = vb14.GetSettingsString("EnabledTabs");

//            bEvent.IsEnabled = sTmp.Contains('E');
//            bBirth.IsEnabled = sTmp.Contains('B');
//            bDeath.IsEnabled = sTmp.Contains('D');
//            //bHolid.IsEnabled = (sTmp.IndexOf("H",StringComparison.Ordinal) > -1);

//            mDate = DateTime.Now;

//            UwpAndro();             // Uno bug override - własny AppBar
//            DopasowanieCmdBar();    // liczba ikonek a szerokość 

//            //vb14.InitDump(0, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path); - jest już w App.ctor

//            if (vb14.GetSettingsBool("AutoLoad")) bRead_Click(null, null);

//        }

//#pragma warning disable IDE0079 // Remove unnecessary suppression
        
//        private void wbViewer_NavigationStarting(object sender, WebNavigatingEventArgs args)
//        {
//        if (args.Url == null)
//        return;

//        args.Cancel = true;

//        if (!vb14.GetSettingsBool("LinksActive"))
//        return;
//#pragma warning disable CS4014
//        Launcher.OpenAsync(args.Url);
//#pragma warning restore
//        }
//#pragma warning restore IDE0079 // Remove unnecessary suppression

//        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
//        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
//        // NOMAUI private void uiDay_Changed(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
//        //{
//        //    // 11/27/2017 9:11:28 PM  1.5.1.6  Console     Microsoft-Xbox One  10.0.16299.4037 
//        //    // System::Nullable$1_System::DateTimeOffset_.get_Value
//        //    // Anniversaries::MainPage.uiDay_Changed

//        //    if (sender.Date != null)
//        //        if (sender.Date.HasValue)
//        //            mDate = sender.Date.Value;
//        //}

//        // NOMAUI private void uiGrid_Resized(object sender, SizeChangedEventArgs e)
//        //{
//        //    DopasowanieCmdBar();
//        //}


//    }
//}

