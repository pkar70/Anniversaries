/*

ANDRO UNO BUG:
* BottomAppBar: w ogóle nie działa
* AppBar: zepsute ikonki [workaround], niepotrzebny button rozwijania ze złą ikonką (a i tak nie działa)
* CommandBar: pokazuje tylko SecondaryCommands (brak obsługi innych niż BitmapIcon)

2022.01.26
* migracja z powrotem części kodu do VB (VB_Lib, .Net standard 1.4, widoczne w VB UWP, w Uno UWP i Droid, oraz w MAUI

2021.12.29
* aktualizacja dla Android 12, oraz aktualizacja Logging (see Uno.txt)
* błąd UNO: AppBarToggleButton.IsChecked nie działa
* błąd regression (Droid): nie włączał secondary commands - ale to był mój błąd, wygaszałem uiAndroidSec.
* InfoAbout - dodałem nazwę app oraz AppVersion, opis z pionowego środka na górę.

STORE 2112

2021.12.28
* zmiana kodu SetLang i SetTab - teraz zmiana potrzebna tylko w Setup.Xaml oraz w MainPage.ReadLang (nie trzeba zmnian w Setup.Xaml.vb)
* próba dodania HEbrew - udana :)
* więc jeszcze arabski oraz japoński, gruziński. Plus chiński i koreanski (koreanski ma takze urodziny postaci fikcyjnych :) ) 
* przeczyszczenie plików z tego, co było starą wersją (XML oraz ręczne cięcie HTMLa; i różne zaszłości)
* bugfix: nie przejmowało NavigationStarting, więc nie blokowało (pewnie odkąd przeszło z VB na C#)

2021.12.27
* Uno 4.0.11, tylko dla rekompilacji i kontroli, ale sprawdzam zachowanie także na UWP - są zmiany struktury stron...
* przerabiam na używanie XmlDocument - ale to nie potrafi zeżreć znaków z innych niż PL i EN, więc:
* dodaję HtmlAgilityPack, i mam HtmlDocument
* przeróbka struktury na bardziej uniwersalną (wygodniejsze usuwanie zbędnych <tag>)
* dodaję Ukraina oraz Grecja (hebrajski byłby fajny, ale on jest pisany od drugiej strony!)

2021.06.18
* Uno 3.8.6, już jest całość w Uno.Master
* dodaję Extensions dla XML, żeby zmniejszyć liczbę #if - e, lipa, niepotrzebne, bo wszak w 3.8.6 jest już Windows.XML

2021.04.03
* Uno 3.6.6

2021.02.27
* UNO 3.5.1, Android 11

STORE ANDROID 2010.1

2020.10.28
* LINK pkModuleShared.cs [..\..\..\_mojeSuby\pkarModule-Uno3-1-6.cs]
*   nie przenoszę ProgBar na modułowy, bo w Android musi być "piętro wyżej" (jest CommandBar najniżej)
*   nie da się przejść na CommandBar, bo nie ma SecondaryCommands, a tu jest tego dużo
*   dodałem brakujący font (dla Android)

2020.10.27
 * [Android] przechodze na Uno 3.1.6 (z dodatkami: 3.2.0-dev.265)

STORE ANDROID 2009.2

2020.08.28
 * podmieniam Uno na bazujące na 3.0.1515 (bo gogus wymusza kompilacje target Android 10)

STORE ANDROID 2002.1

2020.02.12
 * podmieniam Uno na bazująca na 945 - aktualizacja pkModuleShared (dużo już wprowadziłem do Uno), usuwam niepotrzebne Nugety
 * [Android] splashscreen

STORE ANDROID 1912.1

2019.12.22
* nowa kompilacja Uno, bazująca na 2.1.0-dev.408 (tylko CalendarDatePicker własny,  WebViewer.NavigateToString oraz MenuFlyoutitem.click już w base Uno)

2019.10.13
* dodałem do Uno.UI CalendarDatePicker, więc upraszczam kod - bez DatePicker

STORE 10.1910

2019.10.06
* [andro] przełączanie wedle szerokości (emulacja BottomCommandBar w AppBar)
* [!uwp] Setup: znika tekst o Feedback Hub

2019.10.05
* [xaml] warunkowa kompilacja XAML (prefix), co robi czytelniejszym kod

2019.09.13
* MainPage progress bar podczas ładowania (uiProgBar)

2019.09.12
* [andro] MainPage:OnDateChanged - wykorzystanie sender zamiast nazw obiektów XAML
* [uwp] MainPage:BottomAppBar: przełączanie wedle szerokości
* [andro] MainPage:BottomAppBar: przełączanie wedle UWP/Andro (gdy naprawią CommandBar)

2019.09.11
* [andro] emulacja BottomAppBar jako AppBar (początek) - bez uwzględniania szerokości ekranu
* [andro] workaround do zepsutych ikonek w AppBar (FontIcon zamiast SymbolIcon), Uno Pull Request

2019.09.10
* [all] opcja "autoload" danego dnia

STORE UWP 10.1909.1, 2019.09.02

2019.09.05
* w MainPage:XAML są dwa guziki, CalendarDatePicker i DatePicker
* funkcje które nie korzystają z this - dodaję im static

2019.09.02
 * Setup: usunięcie Toggle dla Holidays

2019.08.27
 * BackButton

2019.08.26
 * próba przeniesienia do Uno, bez pośredniego VC
 * MainPage: usunięcie guzika Holidays, bo i tak to nie działało
 * w Setup:Save SetSettings localConfig=true (że było lokalnie, i nie trzeba wczytywać z OneDrive - na przyszłość)
 * migracja do pkar.Dialog / pkar.*Settings* z app
 * zmiana lokalnego ContentDialog na pkar.DialogBox (ContentDialog nie ma w Uno?)
 * Windows.Data.Xml.Dom / System.Xml w zależności od UWP/iOS-Andro-WASM
 * Setup: pokazuje numer wersji
 * TODO zapis settings do OneDrive - ale może warto sprawdzić czy settingsy jednak nie przechodzą
 * TODO gdy to nie Windows, i nie ma localConfig, to odczyt Settingsow z OneDrive 

STORE: 1.7.1.0, 2018.09.03

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
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;

//using TYPXML = Windows.Data.Xml.Dom;


namespace Anniversaries
{

    public partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        //private static HtmlAgilityPack.HtmlDocument mEvents = new HtmlAgilityPack.HtmlDocument();
        //private static HtmlAgilityPack.HtmlDocument mBirths = new HtmlAgilityPack.HtmlDocument();
        //private static HtmlAgilityPack.HtmlDocument mDeaths = new HtmlAgilityPack.HtmlDocument();

        private static DateTimeOffset mDate;

        private void bSetup_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Setup));
        }

        private void bInfo_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InfoAbout));
        }

        // w VB Lib
        // public static string MonthNo2EnName(int iMonth) // static, bo wywoływane z  InfoAbout
        // private HtmlAgilityPack.HtmlDocument WyciagnijSekcjeH2(HtmlAgilityPack.HtmlDocument htmlDoc, string sFrom, string sLang)
        // private void UsunElementy(HtmlAgilityPack.HtmlDocument oDoc, string sTagName, string sTagAttr = "")
        // private HtmlAgilityPack.HtmlDocument WyciagnijDane(HtmlAgilityPack.HtmlDocument htmlDoc, string sFrom, string sLang)
        // private static int IndexOfOr99(string sTxt, string sSubstring)
        // private static int Li2Rok(string sTxt)
        // private HtmlAgilityPack.HtmlDocument SplitAndSort(HtmlAgilityPack.HtmlDocument oDom, string sSplitTag)
        // private static string PoprawRok(string sTxt)
        // private HtmlAgilityPack.HtmlDocument MergeSorted(HtmlAgilityPack.HtmlDocument oDom1, HtmlAgilityPack.HtmlDocument oDom2)
        // private static string DodajPelnyLink(string sPage, string sLang)
        // private async System.Threading.Tasks.Task<string> ReadOneLang(string sUrl)
        // private static List<string> ExtractLangLinks(string sForLangs, string sPage)
        // private static async System.Threading.Tasks.Task<string> GetHtmlPage(string sUrl)

        private async void bRead_Click(object sender, RoutedEventArgs e)
        {
            p.k.DebugOut("bRead_Click");

            string sUrl;

            sUrl = "https://en.wikipedia.org/wiki/" + VBlibekStd.MainPage.MonthNo2EnName(mDate.Month) + "_" + mDate.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);
            VBlibekStd.MainPage.mEvents = new HtmlAgilityPack.HtmlDocument();
            VBlibekStd.MainPage.mBirths = new HtmlAgilityPack.HtmlDocument();
            VBlibekStd.MainPage.mDeaths = new HtmlAgilityPack.HtmlDocument();
            // mHolid = "";
            tbDzien.Text = "Reading EN...";

            // p.k.GetSettingsString("EnabledTabs", "EBD")'
            string sTxt = await VBlibekStd.MainPage.ReadOneLang(sUrl, p.k.GetSettingsString("EnabledTabs", "EBD")).ConfigureAwait(true);
            if(sTxt =="")
            {
                // nie udalo sie wczytac strony - i to tej glownej, ktora nam daje linki do pozostałych
                p.k.DialogBox("ERROR getting page\n" + sUrl);
                return; // skoro i tak nie przejdziemy do pozostałych, to możemy zrezygnować
            }
            sUrl = p.k.GetSettingsString("EnabledLanguages", "pl de fr es ru");
            List<string> lList = VBlibekStd.MainPage.ExtractLangLinks(sUrl, sTxt);

            uiProgBar.Maximum = 1 + lList.Count;
            uiProgBar.Visibility = Visibility.Visible;
            uiProgBar.Value = 1;

            foreach (string sUri in lList)
            {
                tbDzien.Text = "Reading " + sUri.Substring(8, 2).ToUpperInvariant() + "...";
                // p.k.GetSettingsString("EnabledTabs", "EBD")'
                await VBlibekStd.MainPage.ReadOneLang(sUri, p.k.GetSettingsString("EnabledTabs", "EBD")).ConfigureAwait(true);
                uiProgBar.Value = uiProgBar.Value + 1;
            }
            uiProgBar.Visibility = Visibility.Collapsed;

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
            sHtml = "<html><head>" + sHead + "</head><body>" + sHtml + "</body></html>";
            wbViewer.NavigateToString(sHtml);
        }

        private void SetWebView(HtmlAgilityPack.HtmlNode oDoc, string sHead)
        {
            if (oDoc.FirstChild == null)
                p.k.DialogBoxRes("errNoData");
            else
                SetWebView(oDoc.OuterHtml, sHead);
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
            SetWebView(VBlibekStd.MainPage.mEvents.DocumentNode, ""); // "<base href=""https://en.wikipedia.org/"">")
            ToggleButtony(true, false, false);
        }
        //private void bHolid_Click(object sender, RoutedEventArgs e)
        //{
        //    SetWebView(mHolid, ""); // "<base href=""https://en.wikipedia.org/"">")
        //    ToggleButtony(false, false, false);
        //}
        private void bBirth_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(VBlibekStd.MainPage.mBirths.DocumentNode, ""); // "<base href=""https://en.wikipedia.org/"">"
            ToggleButtony(false, true, false);
        }
        private void bDeath_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(VBlibekStd.MainPage.mDeaths.DocumentNode, ""); // "<base href=""https://en.wikipedia.org/"">")
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

            System.Diagnostics.Debug.WriteLine("width: grid=" + iGridWidth.ToString() + ", icon=" + iIconWidth.ToString());

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

        private void DopasowanieCmdBar_Kalendarz(bool bAsPrimaryCmds, bool bAndroSec)
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
                DopasowanieCmdBar_Kalendarz(true, false);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(true);

                return; // bo 9 jest także > 4 :)
            }


            if (iIcons > 7)
            {
                // text + + 7butt + 2sep = 8 + ... : 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                DopasowanieCmdBar_Kalendarz(true, false);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(true);

                return;
            }

            if (iIcons > 5)
            {
                // text + 5.5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                DopasowanieCmdBar_Kalendarz(true, false);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(false);

                return;
            }


            if (iIcons > 4)
            {
                // text + 4.5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                DopasowanieCmdBar_Kalendarz(false, true);
                DopasowanieCmdBar_SelektorStrony(true);
                DopasowanieCmdBar_GoPages(false);

                return;
            }

            // najmniejsze

            // primary commands
            uiBarSeparat1.Visibility = Visibility.Collapsed;
            DopasowanieCmdBar_Kalendarz(false, true);
            DopasowanieCmdBar_SelektorStrony(false);
            DopasowanieCmdBar_GoPages(false);

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string sTmp;
            sTmp = p.k.GetSettingsString("EnabledTabs", "EBD");

            bEvent.IsEnabled = sTmp.Contains("E");
            bBirth.IsEnabled = sTmp.Contains("B");
            bDeath.IsEnabled = sTmp.Contains("D");
            //bHolid.IsEnabled = (sTmp.IndexOf("H",StringComparison.Ordinal) > -1);

            mDate = DateTime.Now;

            UwpAndro();             // Uno bug override - własny AppBar
            DopasowanieCmdBar();    // liczba ikonek a szerokość 

            VBlibekStd.pkarlibmodule.InitDump(0, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);

            if (p.k.GetSettingsBool("AutoLoad")) bRead_Click(null, null);

        }

        private void wbViewer_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri == null)
                return;

            args.Cancel = true;

            if (!p.k.GetSettingsBool("LinksActive"))
                return;
#pragma warning disable CS4014
            Windows.System.Launcher.LaunchUriAsync(args.Uri);
#pragma warning restore
        }


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

