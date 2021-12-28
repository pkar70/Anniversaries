/*

ANDRO UNO BUG:
* BottomAppBar: w ogóle nie działa
* AppBar: zepsute ikonki [workaround], niepotrzebny button rozwijania ze złą ikonką (a i tak nie działa)
* CommandBar: pokazuje tylko SecondaryCommands (brak obsługi innych niż BitmapIcon)

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


        private static HtmlAgilityPack.HtmlDocument mEvents = new HtmlAgilityPack.HtmlDocument();
        private static HtmlAgilityPack.HtmlDocument mBirths = new HtmlAgilityPack.HtmlDocument();
        private static HtmlAgilityPack.HtmlDocument mDeaths = new HtmlAgilityPack.HtmlDocument();

        private static DateTimeOffset mDate;

        private void bSetup_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Setup));
        }

        private void bInfo_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InfoAbout));
        }

        // nazwa, odpowiednio Case dla Wikipedii - gdy poza zakresem, zwroci styczen
        public static string MonthNo2EnName(int iMonth) // static, bo wywoływane z  InfoAbout
        {
            switch (iMonth)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return "January";
            }

        }



        /// <summary>
        /// Wycięcie z htmlDoc fragmentu od <h2> zawierającego _sFrom_ (np. "id=wydarzenia") do początku następnego h2
        /// </summary>
        private HtmlAgilityPack.HtmlDocument WyciagnijSekcjeH2(HtmlAgilityPack.HtmlDocument htmlDoc, string sFrom, string sLang)
        {
            p.k.DebugOut("WyciagnijSekcjeH2('" + sFrom);
            // wszystkie H2 proszę przeiterować
            // w każdym z nich, w inner html, sprawdzam istnienie sFrom

            foreach(HtmlAgilityPack.HtmlNode oH2 in htmlDoc.DocumentNode.SelectNodes("//h2"))
            {
                if(oH2.InnerText.Contains(sFrom))
                {
                    // mam to! znaczy odpowiedni H2
                    // sklejaj wszystkie outer aż do następnego H2
                    string sRet = "";
                    HtmlAgilityPack.HtmlNode oEntry = oH2.NextSibling;
                    while(oEntry != null)
                    {
                        if (oEntry.Name == "h2")
                        {
                            var oRetXml = new HtmlAgilityPack.HtmlDocument();
                            oRetXml.LoadHtml("<root>" + sRet + "</root>");
                            //oRetXml.LoadXml(sRet);
                            return oRetXml;
                        }


                        bool bSkip = false;

                        // pomijam puste
                        if (oEntry.OuterHtml.Trim() == "") bSkip = true;

                        // dla Ukrainy takie coś - mają pierwsze <p>, *TODO* tylko pierwsze <p> pomijać
                        if (sLang =="uk" && oEntry.Name == "p") bSkip = true;

                        if(oEntry.Name=="h4") bSkip = true; // dla DE
                        if (oEntry.Name == "dl") bSkip = true; // dla RU

                        if (!bSkip) sRet += oEntry.OuterHtml.Trim();
                        
                        oEntry = oEntry.NextSibling;
                    }
                }
            }

            return null;  // nie było takiego
        }


        private void UsunElementy(HtmlAgilityPack.HtmlDocument oDoc, string sTagName, string sTagAttr = "")
        {
            for (int iGuard = 100; iGuard > 0; iGuard--)
            {
                bool bBreak = true;
                var oNodes = oDoc.DocumentNode.SelectNodes("//" + sTagName);
                if (oNodes != null)
                {
                    foreach (HtmlAgilityPack.HtmlNode oNode in oNodes)
                    {
                        if (sTagAttr == "" || oNode.OuterHtml.Contains(sTagAttr))
                            oNode.ParentNode.RemoveChild(oNode);
                        bBreak = false;
                        break;
                    }
                }
                if (bBreak) break;
            }
        }

        /// <summary>
        /// Wycięcie z htmlDoc fragmentu od <h2> zawierającego _sFrom_ (np. "id=wydarzenia") do początku następnego h2
        /// Ma usunąć wszystkie niepotrzebne rzeczy.
        /// jest to robione dla języka sLang (jakby były jakieś różnice)
        /// </summary>
        private HtmlAgilityPack.HtmlDocument WyciagnijDane(HtmlAgilityPack.HtmlDocument htmlDoc, string sFrom, string sLang)
        {
            //private TYPXML.XmlDocument WyciagnijDane(TYPXML.XmlDocument htmlDoc, string sFrom, string sLang)
            p.k.DebugOut("WyciagnijDane(htmlDoc, '" + sFrom + "', '" + sLang);
            var oH2 = WyciagnijSekcjeH2(htmlDoc, sFrom, sLang);
            if (oH2 is null) return null;

            // usuwamy obrazki: <div class="thumb tright"> oraz tleft
            // dla: DE, FR, ES
            UsunElementy(oH2, "div", "div class=\"thumb");

            // usuwamy link do multimedia
            // dla: PL
            UsunElementy(oH2, "table", "infobox");


            // nic nie robi dla:
            // PL: (ale za to MergeSorted h2 świat i PL)
            // dla EL: nic
            // mogłoby być wcześniej, ale żaden problem spróbować usunąć
            if (sLang == "pl" || sLang == "el")
                return oH2;

            // dla DE:
            //  w wydarzeniach, H3 do MergeSorted, w pozostałych - do usunięcia
            if (sLang == "de")
            {
                oH2 = SplitAndSort(oH2, "h3");
            }


            // usuwamy podrozdziały (h3)
            // dla: EN, RU, UK
            UsunElementy(oH2, "h3", "");
            
            // pod-nagłówki h4 (DE)
            UsunElementy(oH2, "h4", "");

            // usuwamy DL (RU)
            UsunElementy(oH2, "dl", "");
            // dziwne coś, także tylko dla RU
            UsunElementy(oH2, "div", "hatnote");

            // dla FR, PL w wydarzeniach sklejaj H2 - ale to się robi "piętro wyżej"

            // usuń podział na <ul></ul><ul> (po tych podziałach h3)
            string sXml = oH2.DocumentNode.OuterHtml;
            sXml = sXml.Replace("\n", " ");
            sXml = sXml.Replace("\r", " ");
            sXml = sXml.Replace("  ", " ");
            sXml = sXml.Replace("  ", " ");
            sXml = sXml.Replace("  ", " ");
            sXml = sXml.Replace("</ul> <ul>", "");
            sXml = sXml.Replace("</ul><ul>", "");
            oH2.LoadHtml(sXml);

            return oH2;
        }
    


        private static int IndexOfOr99(string sTxt, string sSubstring)
        {
            int iRet = sTxt.IndexOf(sSubstring, StringComparison.Ordinal);
            if (iRet == -1) return 99;
            return iRet;
        }

        /// <summary>
        /// rok wedle Wikipedii -> rok do sortowania
        /// </summary>
        private static int Li2Rok(string sTxt)
        {
            int iRok = 0;
            int iInd;
            sTxt = sTxt.Trim();    // " 422 -" wydarzenia na swiecie pl.wikipedia

            iInd = sTxt.IndexOf((char)160); // rosyjskojezyczna ma ROK<160><kreska><spacja>
            if(iInd == -1) iInd = 99;
            iInd = Math.Min(iInd, IndexOfOr99(sTxt, " "));
            iInd = Math.Min(iInd, IndexOfOr99(sTxt, ":"));
            iInd = Math.Min(iInd, IndexOfOr99(sTxt, "#"));
            iInd = Math.Min(iInd, IndexOfOr99(sTxt, "&"));
            iInd = Math.Min(iInd, IndexOfOr99(sTxt, "年"));  // japonski
            iInd = Math.Min(iInd, IndexOfOr99(sTxt, "년"));  // koreanski
            iInd = Math.Min(iInd, IndexOfOr99(sTxt, "年"));  // chinski

            if ((iInd > 0) & (iInd < 6))
            {
                if(!int.TryParse(sTxt.Substring(0, iInd), out iRok))
                    System.Diagnostics.Debug.Write("Error CInt(" + sTxt.Substring(0, iInd) + ")");

                sTxt = sTxt.Substring(iInd);
                if (sTxt.Length > 10)
                    sTxt = sTxt.Substring(0, 10);  // 20180115, bo jakis XBOX mial w tej funkcji out-of-range
                if (sTxt.IndexOf(" BC", StringComparison.Ordinal) == 0)
                    iRok = -iRok;    // en
                if (sTxt.IndexOf(" p.n.e", StringComparison.Ordinal) == 0)
                    iRok = -iRok; // pl
                if (sTxt.IndexOf(" v. Chr", StringComparison.Ordinal) == 0)
                    iRok = -iRok; // de
                if (sTxt.IndexOf(" a. C", StringComparison.Ordinal) == 0)
                    iRok = -iRok;  // es
                                   // fr.wiki podaje MINUS :) (bez spacji) 
                if (sTxt.IndexOf(" до н.", StringComparison.Ordinal) == 0)
                    iRok = -iRok;  // ru
            }
            return iRok;
        }

        /// <summary>
        /// podziel _sPage_ na kawałki wedle H3, i scal je potem w jeden - od H3 do H3 jest juz posortowane
        /// </summary>
        private HtmlAgilityPack.HtmlDocument SplitAndSort(HtmlAgilityPack.HtmlDocument oDom, string sSplitTag)
        {
            var oRetDoc = new HtmlAgilityPack.HtmlDocument();

            foreach ( var oNode in oDom.DocumentNode.SelectNodes("//" + sSplitTag))
            {
                string sTmpDoc = ""; 
                var oEntry = oNode.NextSibling;
                while (oEntry != null)
                {
                    if (oEntry.Name == "h2" || oEntry.Name == sSplitTag) break;

                    if(oEntry.Name == "ul")
                    {
                        foreach (var oItem in oEntry.ChildNodes)
                            sTmpDoc += oItem.OuterHtml;
                    }

                    oEntry = oEntry.NextSibling;
                }

                var oTmpDoc = new HtmlAgilityPack.HtmlDocument();
                oTmpDoc.LoadHtml("<root><ul>" + sTmpDoc + "</ul></root>");
                oRetDoc = MergeSorted(oRetDoc, oTmpDoc);

            }

            return oRetDoc;
        }

        private static string PoprawRok(string sTxt)
        {
            // en: <li><a href="/wiki/301" title="301">301</a> &#8211;
            // de: <li><span style="visibility:hidden;">0</span><a href="/wiki/301" title="301">301</a>: 
            // fr: <li><a href="/wiki/301" title="301">301</a>&#160;:
            // es: <li><a href="/wiki/301" title="301">301</a>: 
            // pl: <li>&#160; <a href="/wiki/301" title="301">301</a> – 
            // ru: <li><a title="863 год" href="/wiki/863_%D0%B3%D0%BE%D0%B4">863</a>&nbsp;— 
            // &#8211 = endash
            string sOut = sTxt;
            sOut = sOut.Replace("<span style=\"visibility:hidden;\">0</span>", "&#160;");    // DE wyrównanie
            sOut = sOut.Replace("—", "–");   // RU emdash na endash
            sOut = sOut.Replace("</a>&#160;:", "</a> –");    // FR
            sOut = sOut.Replace("<li>&#160; &#160;", "<li>&#160;&#160;");    // PL dla <100
            sOut = sOut.Replace("<li>&#160; ", "<li>&#160;");    // PL dla <1000

            sOut = sOut.Replace("<li>&#160;", "<li>");    // wyrownanie do lewej

            return sOut;
        }

        private HtmlAgilityPack.HtmlDocument MergeSorted(HtmlAgilityPack.HtmlDocument oDom1, HtmlAgilityPack.HtmlDocument oDom2)
        {
            p.k.DebugOut("MergeSorted");
            // wsortowanie sTxt2 do sTxt1

            if (oDom1 is null)
                return oDom2; // pierwsza strona - bez sortowania
            if (oDom2 is null)
                return oDom1; // symetrycznie niezdarzalnie

            // EN: <ul>\n<li><a href = "/wiki/214" title="214">214</a> (czasem nie ma linka, ale to chyba przy powtorkach?)
            // PL: <ul>\n<li>&#160; <a href= "/wiki/214" title="214">214<
            // tyle ze teraz linki sa juz pelne, tzn. https://pl.wikipedia.org/wiki/214

            // </ul><ul>
            HtmlAgilityPack.HtmlNode oRoot1 = oDom1.DocumentNode; // <root>...
            if (oRoot1 is null) return oDom2;
            HtmlAgilityPack.HtmlNode oRoot2 = oDom2.DocumentNode;
            if(oRoot2 is null) return oDom1;

            HtmlAgilityPack.HtmlNodeCollection oNodes1 = oRoot1.ChildNodes; // wewnątrz #document powinien być tylko <root>
            HtmlAgilityPack.HtmlNodeCollection oNodes2 = oRoot2.ChildNodes;

            if (oNodes1.Count < 1) return oDom2;
            if (oNodes2.Count < 1) return oDom1;
            if (oNodes1.Count > 1 || oNodes2.Count > 1)
            {
                // coś jest nie tak, powinno być tylko jedno
                p.k.DebugOut("Something is wrong - should be only one item inside #document!");
                return oDom1;
            }

            oNodes1 = oNodes1.ElementAt(0).ChildNodes; // czyli <root> 
            oNodes2 = oNodes2.ElementAt(0).ChildNodes;

            if (oNodes1.Count <1) return oDom2;
            if (oNodes2.Count < 1) return oDom1;
            if (oNodes1.Count > 1 || oNodes2.Count > 1)
            {
                // coś jest nie tak, powinno być tylko jedno 
                p.k.DebugOut("Something is wrong - should be only one item inside <root>!");

                p.k.DebugOut("oNodes1:");
                foreach(var oItem in oNodes1)
                {
                    p.k.DebugOut(oItem.Name);
                }

                p.k.DebugOut("oNodes2:");
                foreach (var oItem in oNodes2)
                {
                    p.k.DebugOut(oItem.Name);
                }

                return oDom1;
            }

            oNodes1 = oNodes1.ElementAt(0).SelectNodes("li");   // a w <root><ul> interesują nas <li>
            oNodes2 = oNodes2.ElementAt(0).SelectNodes("li");


            p.k.DebugOut("MergeSorted, count1= " + oNodes1.Count + ", count2=" + oNodes2.Count);

            //// gdy jest wczesniej błąd, to faktycznie moze byc count=0
            //if ((oNodes1.Count == 0) || (oNodes2.Count == 0))
            //    return "";

            string sResult = "";

            int i1 = 0;
            int i2 = 0;

            HtmlAgilityPack.HtmlNode oNode1 = oNodes1.ElementAt(i1);
            HtmlAgilityPack.HtmlNode oNode2 = oNodes2.ElementAt(i2);

            int iRok1 = Li2Rok(oNode1.InnerText);
            int iRok2 = Li2Rok(oNode2.InnerText);

            while ((i1 < oNodes1.Count) & (i2 < oNodes2.Count))
            {
                if (iRok1 < iRok2)
                {
                    sResult = sResult + "\n" + PoprawRok(oNode1.OuterHtml);
                    i1 = i1 + 1;
                    if (i1 < oNodes1.Count)
                    {
                        oNode1 = oNodes1.ElementAt(i1);
                        iRok1 = Li2Rok(oNode1.InnerText);
                    }
                }
                else
                {
                    sResult = sResult + "\n" + PoprawRok(oNode2.OuterHtml);
                    i2 = i2 + 1;
                    if (i2 < oNodes2.Count)
                    {
                        oNode2 = oNodes2.ElementAt(i2);
                        iRok2 = Li2Rok(oNode2.InnerText);
                    }
                }
            }

            while (i1 < oNodes1.Count)
            {
                oNode1 = oNodes1.ElementAt(i1);
                sResult = sResult + "\n" + PoprawRok(oNode1.OuterHtml);
                i1 = i1 + 1;
            }

            while (i2 < oNodes2.Count)
            {
                oNode2 = oNodes2.ElementAt(i2);
                sResult = sResult + "\n" + PoprawRok(oNode2.OuterHtml);
                i2 = i2 + 1;
            }

            var oRetDoc = new HtmlAgilityPack.HtmlDocument();

            if (string.IsNullOrEmpty(sResult))
                return oRetDoc;

            oRetDoc.LoadHtml("<root><ul>" + sResult + "</ul></root>");
            return oRetDoc;
        }

        /// <summary>
        /// Zamiana linku _sPage_ dla jezyka _sLang_ na pelny link
        /// </summary>
        private static string DodajPelnyLink(string sPage, string sLang)
        {
            string DodajPelnyLinkRet;
            string sTmp = sPage;
            sTmp = sTmp.Replace("\"/wiki/", "\"https://" + sLang + ".wikipedia.org/wiki/");
            DodajPelnyLinkRet = sTmp;
            return DodajPelnyLinkRet;
        }

        /// <summary>
        /// wczytaj dane z sUrl, dodawaj do mEvents, mBirths, mDeaths i mHolid
        /// </summary>
        private async System.Threading.Tasks.Task<string> ReadOneLang(string sUrl)
        {
            p.k.DebugOut("ReadOneLang(" + sUrl);

            string sTxt;
            sTxt = await GetHtmlPage(sUrl).ConfigureAwait(true);

            int iInd;
            sUrl = sUrl.Replace("https://", "");
            iInd = sUrl.IndexOf(".", StringComparison.Ordinal);
            sUrl = sUrl.Substring(0, iInd);

            sTxt = DodajPelnyLink(sTxt, sUrl);

            if (sTxt.StartsWith("<!DOCTYPE html>"))
                sTxt = sTxt.Replace("<!DOCTYPE html>", "");

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(sTxt);


            string sTabs;
            sTabs = p.k.GetSettingsString("EnabledTabs", "EBD");

            switch (sUrl)
            {
                case "en":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Events", sUrl));
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Births", sUrl));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Deaths", sUrl));
                        //if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                        //    mHolid = mHolid + WyciagnijDane(sTxt, "observances\">Holidays", sUrl);
                        break;
                    }


                case "de":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Ereignisse", sUrl));
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = WyciagnijDane(htmlDoc, "Geboren", sUrl);
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Gestorben", sUrl));
                        //if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                        //    mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Feier");
                        break;
                    }

                case "pl":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                        {
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Wydarzenia w Pols", sUrl));
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Wydarzenia na świ", sUrl));
                        }
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Urodzili", sUrl));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Zmarli", sUrl));
                        //if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                        //    mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Święta");
                        break;
                    }

                case "fr":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                        {
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Événements", sUrl));
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Arts, culture", sUrl));
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Sciences_et", sUrl));
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Économie", sUrl));
                        }
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Naissances", sUrl));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Décès", sUrl));
                        //if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                        //    mHolid = mHolid + WyciagnijDane(sTxt, "ns\">Célébrations");
                        break;
                    }

                case "es":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Acontecimientos", sUrl));
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Nacimientos", sUrl));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Fallecimientos", sUrl));
                        //if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                        //    mHolid = mHolid + WyciagnijDane(sTxt, "s\">Celebraciones");
                        break;
                    }

                case "ru":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "События", sUrl));
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Родились", sUrl));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Скончались", sUrl));
                        //if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                        //    mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Праздники");
                        break;
                    }

                case "uk":
                    {
                        if (sTabs.Contains("E"))
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Події", sUrl));
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Народились", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Померли", sUrl));
                        break;
                    }
                case "el":
                    {
                        if (sTabs.Contains("E"))
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Γεγονότα", sUrl));
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Γεννήσεις", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Θάνατοι", sUrl));
                        break;
                    }

                case "he":
                    {
                        if (sTabs.Contains("E"))
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "אירועים", sUrl));
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "נולדו", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "נפטרו", sUrl));
                        break;
                    }

                case "ja":
                    {
                        if (sTabs.Contains("E"))
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "できごと", sUrl));
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "誕生日", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "忌日", sUrl));
                        break;
                    }
                case "ar":
                    {
                        if (sTabs.Contains("E"))
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "أحداث", sUrl));
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "مواليد", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "وفيات", sUrl));
                        break;
                    }
                case "ka":
                    {
                        if (sTabs.Contains("E"))
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "მოვლენები", sUrl));
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "დაბადებულნი", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "გარდაცვლილნი", sUrl));
                        break;
                    }

                case "ko":
                    {
                        if (sTabs.Contains("E"))
                        {
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "사건", sUrl));
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "문화", sUrl));
                        }
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "탄생", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "사망", sUrl));
                        break;
                    }

                case "zh":
                    {
                        if (sTabs.Contains("E"))
                            mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "大事记", sUrl));
                        if (sTabs.Contains("B"))
                            mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "出生", sUrl));
                        if (sTabs.Contains("D"))
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "逝世", sUrl));
                        break;
                    }

                default:
                    {
                        var oUnsupported = HtmlAgilityPack.HtmlNode.CreateNode("<h2>Unsupported lang: " + sUrl + "</h2>");
                        mEvents.DocumentNode.AppendChild(oUnsupported);
                        mBirths.DocumentNode.AppendChild(oUnsupported);
                        mDeaths.DocumentNode.AppendChild(oUnsupported);

                        break;
                    }
            }

            return sTxt;
        }

        private static List<string> ExtractLangLinks(string sForLangs, string sPage)
        {
            List<string> lList = new List<string>();     // SDK 1803 - problem z typem?

            int iInd;
            string sTmp;
            string[] aArr;   // było Array, SDK 1803 - nie zna typu dla sLang, zmieniam na String()
            aArr = sForLangs.Split(' ');
            foreach (string sLang in aArr)
            {
                string sLang1 = "https://" + sLang + ".wikipedia";
                iInd = sPage.IndexOf(sLang1, StringComparison.Ordinal);
                if (iInd > 100)
                {
                    sTmp = sPage.Substring(iInd);
                    iInd = sTmp.IndexOf("\"", StringComparison.Ordinal);
                    sTmp = sTmp.Substring(0, iInd);
                    lList.Add(sTmp);
                }
            }

            return lList;
        }

        private static async System.Threading.Tasks.Task<string> GetHtmlPage(string sUrl)
        { // UNO NIE MA windows.web - migracja do System.Net.Http
            p.k.DebugOut("Reading page: " + sUrl);

            using (System.Net.Http.HttpClient oHttp = new System.Net.Http.HttpClient())
            {
                Uri oUri = new Uri(sUrl);
                using (System.Net.Http.HttpResponseMessage oResp = await oHttp.GetAsync(oUri).ConfigureAwait(true))
                {
                    if (!oResp.IsSuccessStatusCode)
                    {
                        await p.k.DialogBoxAsync("GetHtmlPage error, URL=" + sUrl).ConfigureAwait(true);
                        return "";
                    }
                    return await oResp.Content.ReadAsStringAsync().ConfigureAwait(true);
                }
            }
        }

        private async void bRead_Click(object sender, RoutedEventArgs e)
        {
            p.k.DebugOut("bRead_Click");

            // Uno bug override
            //if (pkar.GetPlatform("android"))
            //{
            //    if (uiDayAndroBar.Date != null)
            //        mDate = uiDayAndroBar.Date;
            //}


            string sUrl;

            sUrl = "https://en.wikipedia.org/wiki/" + MonthNo2EnName(mDate.Month) + "_" + mDate.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);
            mEvents = new HtmlAgilityPack.HtmlDocument();
            mBirths = new HtmlAgilityPack.HtmlDocument();
            mDeaths = new HtmlAgilityPack.HtmlDocument();
            // mHolid = "";
            tbDzien.Text = "Reading EN...";
            
            string sTxt = await ReadOneLang(sUrl).ConfigureAwait(true);

            sUrl = p.k.GetSettingsString("EnabledLanguages", "pl de fr es ru");
            List<string> lList = ExtractLangLinks(sUrl, sTxt);

            uiProgBar.Maximum = 1 + lList.Count;
            uiProgBar.Visibility = Visibility.Visible;
            uiProgBar.Value = 1;

            foreach (string sUri in lList)
            {
                tbDzien.Text = "Reading " + sUri.Substring(8, 2).ToUpperInvariant() + "...";
                await ReadOneLang(sUri).ConfigureAwait(true);
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
            if (bBirth.IsChecked.HasValue && bBirth.IsChecked.Value )
            {
                bZaden = false;
                bBirth_Click(sender, e);
            }
            if (bDeath.IsChecked.HasValue && bDeath.IsChecked.Value )
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
                SetWebView(mEvents.DocumentNode, ""); // "<base href=""https://en.wikipedia.org/"">")
            ToggleButtony(true, false, false);
        }
        //private void bHolid_Click(object sender, RoutedEventArgs e)
        //{
        //    SetWebView(mHolid, ""); // "<base href=""https://en.wikipedia.org/"">")
        //    ToggleButtony(false, false, false);
        //}
        private void bBirth_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(mBirths.DocumentNode, ""); // "<base href=""https://en.wikipedia.org/"">"
            ToggleButtony(false, true, false);
        }
        private void bDeath_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(mDeaths.DocumentNode, ""); // "<base href=""https://en.wikipedia.org/"">")
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

        private void DopasowanieCmdBar_GoPages(bool bAsPrimaryCmds)
        {
            Visibility bVis = (bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiBarSeparat3.Visibility = bVis;
            uiGoSett.Visibility = bVis;
            uiGoInfo.Visibility = bVis;

            bVis = (!bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiGoSettSec.Visibility = bVis;
            uiGoInfoSec.Visibility = bVis;
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

        }

        private void DopasowanieCmdBar_Kalendarz(bool bAsPrimaryCmds, bool bAndroSec)
        {
            Visibility bVis = (bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiKalend.Visibility = bVis;

            bVis = (!bAsPrimaryCmds) ? Visibility.Visible : Visibility.Collapsed;
            uiKalendSec.Visibility = bVis;
#if !NETFX_CORE
            bVis = (!bAndroSec) ? Visibility.Visible : Visibility.Collapsed;
            uiAndroSec.Visibility = bVis;
#endif
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

