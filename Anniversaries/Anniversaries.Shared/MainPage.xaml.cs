/*

ANDRO UNO BUG:
* DatePicker, nie zmienia picker.Date , Uno Pull Request? [a może brakuje tylko guzika OK? i nowy Uno.Ui bedzie ok?]
* BottomAppBar: w ogóle nie działa
* AppBar: zepsute ikonki [workaround], niepotrzebny button rozwijania ze złą ikonką (a i tak nie działa)
* CalendarDatePicker: żeby nie trzeba było dwu wersji, DatePicker i CalendarDatePicker
* CommandBar: pokazuje tylko SecondaryCommands (brak obsługi innych niż BitmapIcon)

2019.09.13
* MainPage progress bar podczas ładowania (uiProgBar)

2019.09.12
* [andro] MainPage:OnDateChanged - wykorzystanie sender zamiast nazw obiektów XAML (dla Andro)
* [uwp] MainPage:BottomAppBar: przełączanie wedle szerokości
* [andro] MainPage:BottomAppBar: przełączanie wedle UWP/Andro (gdy naprawią CommandBar)

2019.09.11
* [andro] emulacja BottomAppBar jako AppBar (początek) - bez uwzględniania szerokości ekranu
* [andro] workaround do zepsutych ikonek w AppBar, Uno Pull Request

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

#if NETFX_CORE
using TYPXML = Windows.Data.Xml.Dom;
#else
using TYPXML = System.Xml;
#endif




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


        private static string mEvents = "";
        private static string mHolid = "";
        private static string mBirths = "";
        private static string mDeaths = "";
        // private string mObceMiesiace = "";
        // zmienne w Setup dostepne
        private static DateTimeOffset mDate;
        // Dim mObceJezyki As String = "pl de fr es ru"
        private static string mPreferredLang = "pl";
        private static string mCurrLang = "";
        private static string mCurrPart = "";

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

        private static string MonthNo2PlName(int iMonth)
        {
            switch (iMonth)
            {
                case 1:
                    return "stycznia";
                case 2:
                    return "lutego";
                case 3:
                    return "marca";
                case 4:
                    return "kwietnia";
                case 5:
                    return "maja";
                case 6:
                    return "czerwca";
                case 7:
                    return "lipca";
                case 8:
                    return "sierpnia";
                case 9:
                    return "września";
                case 10:
                    return "października";
                case 11:
                    return "listopada";
                case 12:
                    return "grudnia";
                default:
                    return "stycznia";
            }
        }

        /// <summary>
        /// Wycięcie z _sPage_ tekstu od _sFrom_ do początku następnego h2
        /// </summary>
        private string WyciagnijDane(string sPage, string sFrom)
        {
            mCurrPart = sFrom;

            int iInd;
            string sTxt;

            iInd = sPage.IndexOf(sFrom, StringComparison.Ordinal);
            if (iInd < 10)
                return "";
            sTxt = sPage.Substring(iInd);
            iInd = sTxt.IndexOf("<ul>", StringComparison.Ordinal);
            if (iInd < 10)
                return "";
            sTxt = sTxt.Substring(iInd);
            iInd = sTxt.IndexOf("<h2", StringComparison.Ordinal);
            if (iInd < 10)
                return "";
            sTxt = sTxt.Substring(0, iInd);
            iInd = sTxt.LastIndexOf("</ul", StringComparison.Ordinal);
            if (iInd < 10)
                return "";
            return sTxt.Substring(0, iInd + 5);
        }

        private string WytnijObrazkiDE(string sPage)
        {
            // wikipedia.de ma obrazki wklejone - ale przerywaja one <ul>, wiec latwo wyrzucic
            int iInd;
            string sTmp = "";

            // pozbywam sie potrzebnego </ul>
            iInd = sPage.LastIndexOf("</ul>", StringComparison.Ordinal);
            if (iInd > 0)
                sPage = sPage.Substring(0, iInd - 1);

            iInd = sPage.IndexOf("</ul>", StringComparison.Ordinal);
            // ale jesli "</ul></li> to jest ok... - wersja FR tej funkcji!
            while (iInd > 0)
            {
                sTmp = sTmp + sPage.Substring(0, iInd - 1);
                sPage = sPage.Substring(iInd);
                iInd = sPage.IndexOf("<ul>", StringComparison.Ordinal);
                if (iInd > 0)
                {
                    sPage = sPage.Substring(iInd + 4);
                    iInd = sPage.IndexOf("</ul>", StringComparison.Ordinal);
                }
            }

            sTmp = sTmp + sPage;

            return sTmp + "</ul>";
        }

        private static string WytnijObrazkiFR(string sPage)
        {
            // bardziej skomplikowane niz DE, bo są sublisty (znaczy runtime bardziej skomplikowany)
            if (string.IsNullOrEmpty(sPage))
                return "";    // żeby nie było <u></ul>
            string sResult = "";
            TYPXML.XmlDocument oDom1 = new TYPXML.XmlDocument();
            sPage = "<root>" + sPage + "</root>";  // bo inaczej error ze tylko jeden root element moze byc
            try
            {
                oDom1.LoadXml(sPage);
            }
            catch
            {
                return "<ul><li>0 ERROR loading sPage, WytnijObrazkiFR</li></ul>";
            }
            TYPXML.XmlElement oRoot1 = oDom1.DocumentElement;
            TYPXML.XmlNodeList oNodes1 = oRoot1.SelectNodes("/root/ul/li");

            // moze byc: <root><ul><li><ul><li> - tego glebiej nie ruszac!
#if NETFX_CORE
            foreach (TYPXML.IXmlNode oNode in oNodes1)
                sResult = sResult + "\n" + oNode.GetXml();
#else
            foreach (TYPXML.XmlNode oNode in oNodes1)
                sResult = sResult + "\n" + oNode.OuterXml.Trim();
#endif

            return "<ul>" + sResult + "</ul>";
        }

        /// <summary>
        /// rok wedle Wikipedii -> rok do sortowania
        /// </summary>
        private static int Li2Rok(string sTxt)
        {
            int Li2RokRet = default(int);
            int iRok = 0;
            int iInd, iInd1, iInd2, iInd3;
            sTxt = sTxt.Trim();    // " 422 -" wydarzenia na swiecie pl.wikipedia

            iInd1 = sTxt.IndexOf(" ", StringComparison.Ordinal);
            iInd2 = sTxt.IndexOf(":", StringComparison.Ordinal); // przy <li>rok:<ul> (wersja PL)
            iInd3 = sTxt.IndexOf((char)160); // rosyjskojezyczna ma ROK<160><kreska><spacja>
            iInd = 0;

            if (iInd1 == -1)
                iInd1 = 99;
            if (iInd2 == -1)
                iInd2 = 99;
            if (iInd3 == -1)
                iInd3 = 99;
            iInd = Math.Min(Math.Min(iInd1, iInd2), iInd3);

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
            Li2RokRet = iRok;
            return Li2RokRet;
        }

        /// <summary>
        /// podziel _sPage_ na kawałki wedle H3, i scal je potem w jeden - od H3 do H3 jest juz posortowane
        /// </summary>
        private string PrepareAndSplitSort(string sPage)
        {
            // wejscie: <ul> ... </ul>, po drodze </ul><h3><ul>, moga byc obrazki..

            string sResult = "";
            TYPXML.XmlDocument oDom1 = new TYPXML.XmlDocument();

            try
            {
                oDom1.LoadXml("<root>" + sPage + "</root>");
                //oDom2 = System.Xml.Linq.XDocument.Load("<root>" + sPage + "</root>");
            }
            catch
            {
                return "<ul><li>0 ERROR loading sPage, SplitAndSort</li></ul>";
            }

            TYPXML.XmlElement oRoot1 = oDom1.DocumentElement;

            // dowolny na poziomie 1
            TYPXML.XmlNodeList oNodes1 = oRoot1.SelectNodes("/root/*");

            // moze byc: <root><ul><li><ul><li> ALBO H3 ALBO <div> (obrazek)
#if NETFX_CORE
            foreach (TYPXML.IXmlNode oNode in oNodes1)
            {
                if ((oNode.NodeName == "ul" && (oNode.Attributes.Count < 1 || oNode.Attributes.Item(0).NodeName != "class")) | (oNode.NodeName == "h3"))
                    sResult = sResult + oNode.GetXml().Trim();
            }
#else
            foreach (TYPXML.XmlNode oNode in oNodes1)
            {
                if ((oNode.Name == "ul" && (oNode.Attributes.Count < 1 || oNode.Attributes.Item(0).Name != "class")) | (oNode.Name == "h3"))
                    sResult = sResult + oNode.OuterXml.Trim();
            }
#endif            

            sResult = sResult.Replace("</ul><ul>", "");   // miejsca po obrazkach
            return SplitAndSort(sResult);
        }

        private string SplitAndSort(string sTxtIn)
        {
            string SplitAndSortRet = default(string);
            // podziel na kawalki wedle H3, i scal je potem w jeden - od H3 do H3 jest juz posortowane
            // wejscie: <ul> ... </ul>, po drodze </ul><h3><ul>, moga byc obrazki..

            int iInd = 0;
            string sTxtOut = "";
            string sTmp = default(string);

            iInd = sTxtIn.IndexOf("<h3", StringComparison.Ordinal);
            while (iInd > 0)
            {
                sTmp = sTxtIn.Substring(0, iInd);
                sTxtIn = sTxtIn.Substring(iInd);

                iInd = sTmp.LastIndexOf("</ul>", StringComparison.Ordinal);
                if (iInd > 0)
                    sTmp = sTmp.Substring(0, iInd + 5);

                iInd = sTxtIn.IndexOf("<ul>", StringComparison.Ordinal);
                if (iInd > 0)
                    sTxtIn = sTxtIn.Substring(iInd);

                sTxtOut = MergeSorted(sTxtOut, sTmp);
                iInd = sTxtIn.IndexOf("<h3", StringComparison.Ordinal);
            }

            iInd = sTxtIn.LastIndexOf("</ul>", StringComparison.Ordinal);
            if (iInd > 0)
                sTxtIn = sTxtIn.Substring(0, iInd + 5);
            SplitAndSortRet = MergeSorted(sTxtOut, sTxtIn);
            return SplitAndSortRet;
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
            sOut = sOut.Replace("<li>&#160;", "<li>");


            return sOut;
        }

        private string MergeSorted(string sTxt1, string sTxt2)
        {
            // wsortowanie sTxt2 do sTxt1

            if (string.IsNullOrEmpty(sTxt1))
                return sTxt2; // pierwsza strona - bez sortowania
            if (string.IsNullOrEmpty(sTxt2))
                return sTxt1; // symetrycznie niezdarzalnie

            // EN: <ul>\n<li><a href = "/wiki/214" title="214">214</a> (czasem nie ma linka, ale to chyba przy powtorkach?)
            // PL: <ul>\n<li>&#160; <a href= "/wiki/214" title="214">214<
            // tyle ze teraz linki sa juz pelne, tzn. https://pl.wikipedia.org/wiki/214

            string sResult = "";

            TYPXML.XmlDocument oDom1 = new TYPXML.XmlDocument();
            TYPXML.XmlDocument oDom2 = new TYPXML.XmlDocument();
            try
            {
                oDom1.LoadXml(sTxt1);
            }
            catch 
            {
                oDom1.LoadXml("<ul><li>0 ERROR loading sTxt1, lang: " + mCurrLang + ", part: " + mCurrPart + "</li></ul>");
            }

            try
            {
                oDom2.LoadXml(sTxt2);
            }
            catch 
            {
                oDom2.LoadXml("<ul><li>0 ERROR loading sTxt2, lang: " + mCurrLang + ", part: " + mCurrPart + "</li></ul>");
            }
            // </ul><ul>
            TYPXML.XmlElement oRoot1 = oDom1.DocumentElement;
            TYPXML.XmlElement oRoot2 = oDom2.DocumentElement;
            TYPXML.XmlNodeList oNodes1 = oRoot1.SelectNodes("li");
            TYPXML.XmlNodeList oNodes2 = oRoot2.SelectNodes("li");

            // gdy jest wczesniej błąd, to faktycznie moze byc count=0
            if ((oNodes1.Count == 0) | (oNodes2.Count == 0))
                // Dim msg As ContentDialog
                // msg = New ContentDialog With {
                // .Title = "ERROR",
                // .Content = "MergeSorted error, Count=0?",
                // .CloseButtonText = "Pa"
                // }
                // msg.ShowAsync()
                return "";

            int i1 = 0;
            int i2 = 0;

#if NETFX_CORE
            TYPXML.IXmlNode oNode1 = oNodes1.ElementAt(i1);  // SDK 1803 - nie zna typu? dopiero po rebuild zna
            TYPXML.IXmlNode oNode2 = oNodes2.ElementAt(i2);  // SDK 1803 - nie zna typu? j.w.
#else
            TYPXML.XmlNode oNode1 = oNodes1.Item(i1);
            TYPXML.XmlNode oNode2 = oNodes2.Item(i2);
#endif            

            int iRok1 = Li2Rok(oNode1.InnerText);
            int iRok2 = Li2Rok(oNode2.InnerText);


            while ((i1 < oNodes1.Count) & (i2 < oNodes2.Count))
            {
                if (iRok1 < iRok2)
                {
#if NETFX_CORE
                    sResult = sResult + "\n" + PoprawRok(oNode1.GetXml());
#else
                    sResult = sResult + "\n" + PoprawRok(oNode1.OuterXml);
#endif            
                    i1 = i1 + 1;
                    if (i1 < oNodes1.Count)
                    {
#if NETFX_CORE
                        oNode1 = oNodes1.ElementAt(i1);
#else
                        oNode1 = oNodes1.Item(i1);
#endif            
                        iRok1 = Li2Rok(oNode1.InnerText);
                    }
                }
                else
                {
#if NETFX_CORE
                    sResult = sResult + "\n" + PoprawRok(oNode2.GetXml());
#else
                    sResult = sResult + "\n" + PoprawRok(oNode2.OuterXml);
#endif            
                    i2 = i2 + 1;
                    if (i2 < oNodes2.Count)
                    {
#if NETFX_CORE
                        oNode2 = oNodes2.ElementAt(i2);
#else
                        oNode2 = oNodes2.Item(i2);
#endif            
                        iRok2 = Li2Rok(oNode2.InnerText);
                    }
                }
            }

            while (i1 < oNodes1.Count)
            {
#if NETFX_CORE
                oNode1 = oNodes1.ElementAt(i1);
                sResult = sResult + "\n" + PoprawRok(oNode1.GetXml());
#else
                oNode1 = oNodes1.Item(i1);
                sResult = sResult + "\n" + PoprawRok(oNode1.OuterXml);
#endif            
                i1 = i1 + 1;
            }

            while (i2 < oNodes2.Count)
            {
#if NETFX_CORE
                oNode2 = oNodes2.ElementAt(i2);
                sResult = sResult + "\n" + PoprawRok(oNode2.GetXml());
#else
                oNode2 = oNodes2.Item(i2);
                sResult = sResult + "\n" + PoprawRok(oNode2.OuterXml);
#endif            
                i2 = i2 + 1;
            }

            if (string.IsNullOrEmpty(sResult))
                return "";
            return "<ul>" + sResult + "</ul>";
        }

        /// <summary>
        /// Zamiana linku _sPage_ dla jezyka _sLang_ na pelny link
        /// </summary>
        private static string DodajPelnyLink(string sPage, string sLang)
        {
            string DodajPelnyLinkRet = default(string);
            string sTmp = sPage;
            sTmp = sTmp.Replace("\"/wiki/", "\"https://" + sLang + ".wikipedia.org/wiki/");
            DodajPelnyLinkRet = sTmp;
            return DodajPelnyLinkRet;
        }

        private async System.Threading.Tasks.Task<string> ReadOneLang(string sUrl, string sPrefLang)
        {
            string sTxt;
            sTxt = await GetHtmlPage(sUrl).ConfigureAwait(true);

            int iInd;
            sUrl = sUrl.Replace("https://", "");
            iInd = sUrl.IndexOf(".", StringComparison.Ordinal);
            sUrl = sUrl.Substring(0, iInd);

            sTxt = DodajPelnyLink(sTxt, sUrl);
            mCurrLang = sUrl;

            string sTabs;
            sTabs = pkar.GetSettingsString("EnabledTabs", "EBD");

            switch (sUrl)
            {
                case "en":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, WyciagnijDane(sTxt, "id=\"Events\">Events"));
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WyciagnijDane(sTxt, "id=\"Births\">Births"));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(sTxt, "id=\"Deaths\">Deaths"));
                        if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                            mHolid = mHolid + WyciagnijDane(sTxt, "observances\">Holidays");
                        break;
                    }

                case "pl":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                        {
                            mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Wydarzenia_w_Pols")));
                            mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Wydarzenia_na_świ")));
                        }
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WyciagnijDane(sTxt, "id=\"Urodzili_się"));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WyciagnijDane(sTxt, "id=\"Zmarli"));
                        if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                            mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Święta");
                        break;
                    }

                case "de":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, PrepareAndSplitSort(WyciagnijDane(sTxt, "id=\"Ereignisse")));
                        // If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WytnijObrazkiDE(WyciagnijDane(sTxt, "id=""Geboren")))
                        // If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WytnijObrazkiDE(WyciagnijDane(sTxt, "id=""Gestorben")))
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                        {
                            string sTmp = PrepareAndSplitSort(WyciagnijDane(sTxt, "id=\"Geboren"));
                            mBirths = MergeSorted(mBirths, sTmp);
                        }
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, PrepareAndSplitSort(WyciagnijDane(sTxt, "id=\"Gestorben")));
                        if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                            mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Feier");
                        break;
                    }

                case "fr":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                        {
                            mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Événements")));
                            mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Arts,_culture")));
                            mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Sciences_et")));
                            mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Économie")));
                        }
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WytnijObrazkiFR(WyciagnijDane(sTxt, "es\">Naissances")));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WytnijObrazkiFR(WyciagnijDane(sTxt, "s\">Décès")));
                        if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                            mHolid = mHolid + WyciagnijDane(sTxt, "ns\">Célébrations");
                        break;
                    }

                case "es":
                    {
                        // If sTabs.IndexOf("E") > -1 Then mEvents = MergeSorted(mEvents, WytnijObrazkiDE(WyciagnijDane(sTxt, "s"">Acontecimientos")))
                        // If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WytnijObrazkiDE(WyciagnijDane(sTxt, "s"">Nacimientos")))
                        // If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WytnijObrazkiDE(WyciagnijDane(sTxt, "s"">Fallecimientos")))
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, PrepareAndSplitSort(WyciagnijDane(sTxt, "s\">Acontecimientos")));
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, PrepareAndSplitSort(WyciagnijDane(sTxt, "s\">Nacimientos")));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, PrepareAndSplitSort(WyciagnijDane(sTxt, "s\">Fallecimientos")));
                        if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                            mHolid = mHolid + WyciagnijDane(sTxt, "s\">Celebraciones");
                        break;
                    }

                case "ru":
                    {
                        if (sTabs.IndexOf("E", StringComparison.Ordinal) > -1)
                            mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"События")));
                        if (sTabs.IndexOf("B", StringComparison.Ordinal) > -1)
                            mBirths = MergeSorted(mBirths, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Родились")));
                        if (sTabs.IndexOf("D", StringComparison.Ordinal) > -1)
                            mDeaths = MergeSorted(mDeaths, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=\"Скончались")));
                        if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                            mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Праздники");
                        break;
                    }

                default:
                    {
                        mEvents = mEvents + "<h2>Unsupported lang: " + sUrl + "</h2>";
                        mBirths = mBirths + "<h2>Unsupported lang: " + sUrl + "</h2>";
                        mDeaths = mDeaths + "<h2>Unsupported lang: " + sUrl + "</h2>";
                        mHolid = mHolid + "<h2>Unsupported lang: " + sUrl + "</h2>";
                        break;
                    }
            }

            return sTxt;
        }

        private static List<string> ExtractLangLinks(string sForLangs, string sPage)
        {
            List<string> lList = new List<string>();     // SDK 1803 - problem z typem?

            int iInd = default(int);
            string sTmp = default(string);
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
            System.Net.Http.HttpClient oHttp = new System.Net.Http.HttpClient();
            Uri oUri = new Uri(sUrl);
            System.Net.Http.HttpResponseMessage oResp = new System.Net.Http.HttpResponseMessage();
            oResp = await oHttp.GetAsync(oUri).ConfigureAwait(true);
            if (!oResp.IsSuccessStatusCode)
            {
                await pkar.DialogBox("GetHtmlPage error, URL=" + sUrl).ConfigureAwait(true);
                //Windows.UI.Popups.MessageDialog oDlg;
                //oDlg = new Windows.UI.Popups.MessageDialog("GetHtmlPage error, URL=" + sUrl);
                //oDlg.Title = "ERROR";
                //oDlg.Commands.Add(new Windows.UI.Popups.UICommand("Pa"));
                //await oDlg.ShowAsync();

                //ContentDialog msg;
                //// UNO nie ma ContentDialog, choć piszą że ma...
                //msg = new ContentDialog()
                //{
                //    Title = "ERROR",
                //    Content = "GetHtmlPage error, URL=" + sUrl,
                //    CloseButtonText = "Pa"
                //};
                //await msg.ShowAsync();
                oResp.Dispose();
                oHttp.Dispose();
                return "";
            }
            string sTxt;
            sTxt = await oResp.Content.ReadAsStringAsync().ConfigureAwait(true);
            oResp.Dispose();
            oHttp.Dispose();
            return sTxt;
        }

        private async void bRead_Click(object sender, RoutedEventArgs e)
        {
            // Uno bug override
            if (pkar.GetPlatform("android"))
            {
                if (uiDayAndroBar.Date != null)
                    mDate = uiDayAndroBar.Date;
            }


            string sUrl = "";

            sUrl = "https://en.wikipedia.org/wiki/" + MonthNo2EnName(mDate.Month) + "_" + mDate.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);
            mEvents = "";
            mBirths = "";
            mDeaths = "";
            mHolid = "";
            tbDzien.Text = "Reading EN...";
            tbDzienAndroBar.Text = "Reading EN...";
            string sTxt = await ReadOneLang(sUrl, mPreferredLang).ConfigureAwait(true);

            sUrl = pkar.GetSettingsString("EnabledLanguages", "pl de fr es ru");
            List<string> lList = ExtractLangLinks(sUrl, sTxt);

            uiProgBar.Maximum = 1 + lList.Count;
            uiProgBar.Visibility = Visibility.Visible;
            uiProgBar.Value = 1;

            foreach (string sUri in lList)
            {
                tbDzien.Text = "Reading " + sUri.Substring(8, 2).ToUpperInvariant() + "...";
                tbDzienAndroBar.Text = "Reading " + sUri.Substring(8, 2).ToUpperInvariant() + "...";
                await ReadOneLang(sUri, mPreferredLang).ConfigureAwait(true);
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
            tbDzienAndroBar.Text = mDate.ToString("d MMMM", System.Globalization.CultureInfo.CurrentCulture);  // .Day.ToString & " " & MonthNo2PlName(mDate.Month)
        }

        private static string MetaViewport()
        {// kopia z Brewiarz
            double dScale = pkar.GetSettingsInt("fontSize", 100);   // skalowanie - w Brewiarz jest, a tu nie
            string sScale = "initial-scale=" + dScale.ToString("0.##");
            return "<meta name=\"viewport\" content=\"width=device-width, " + sScale + "\">";
        }

        private void SetWebView(string sHtml, string sHead)
        {
            wbViewer.Height = naView.ActualHeight - 10;
            wbViewer.Width = naView.ActualWidth - 10;
            // if (sHead == "") sHead = MetaViewport(); - jest jeszcze gorzej :)
            sHtml = "<html><head>" + sHead + "</head><body>" + sHtml + "</body></html>";
            wbViewer.NavigateToString(sHtml);
        }

        private void ToggleButtony(bool bEv, bool bBir, bool bDea)
        {
            // primary
            bEvent.IsChecked = bEv;
            //bHolid.IsChecked = false;
            bBirth.IsChecked = bBir;
            bDeath.IsChecked = bDea;
            bEventAndroBar.IsChecked = bEv;
            //bHolid.IsChecked = false;
            bBirthAndroBar.IsChecked = bBir;
            bDeathAndroBar.IsChecked = bDea;

            // oraz z menu
            uiSelEvent.IsChecked = bEv;
            uiSelBirth.IsChecked = bBir;
            uiSelDeath.IsChecked = bDea;

        }

        private void bEvent_Click(object sender, RoutedEventArgs e)
        {
            //if(pkar.GetPlatform("android"))
            //    SetWebView("<p>testowyparagraf</p>","");
            //else
                SetWebView(mEvents, ""); // "<base href=""https://en.wikipedia.org/"">")
            ToggleButtony(true, false, false);
        }
        private void bHolid_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(mHolid, ""); // "<base href=""https://en.wikipedia.org/"">")
            ToggleButtony(false, false, false);
        }
        private void bBirth_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(mBirths, ""); // "<base href=""https://en.wikipedia.org/"">"
            ToggleButtony(false, true, false);
        }
        private void bDeath_Click(object sender, RoutedEventArgs e)
        {
            SetWebView(mDeaths, ""); // "<base href=""https://en.wikipedia.org/"">")
            ToggleButtony(false, false, true);
        }

        private void UwpAndro()
        { // przełączanie aktywnego w Android (AppBar) i w UWP (BottomAppBar-CommandBar)
            // pierwotna wersja miała #if, ale tak chyba jest lepiej
            if (pkar.GetPlatform("uwp"))
            {
                uiKalendUWP.Visibility = Visibility.Visible;
                uiKalendAndro.Visibility = Visibility.Collapsed;
                uiDay.Date = mDate;
                uiAndroBottom.Visibility = Visibility.Collapsed;
            }
            else
            {
                uiKalendUWP.Visibility = Visibility.Collapsed;
                uiKalendAndro.Visibility = Visibility.Visible;
                // uiDayAndro.Date = mDate;
                uiDayAndroBar.Date = mDate; // emulowane w AndroidBar
                uiAndroBottom.Visibility = Visibility.Visible;
            }

        }

        private int CmdBarWidth()
        {

            int iIconWidth, iGridWidth;

            if (pkar.GetPlatform("uwp"))
                iIconWidth = (int)bRefresh.ActualWidth; // zakładam że to będzie zawsze widoczne, czyli dobrze policzy
            else
                iIconWidth = 80;    // *TODO* jest na sztywno, bo w Android się trudno połapać :) [bo nie wiadomo co będzie pokazane]
            iGridWidth = (int)uiGrid.ActualWidth;

            System.Diagnostics.Debug.WriteLine("width: grid=" + iGridWidth.ToString() + ", icon=" + iIconWidth.ToString());

            iGridWidth -= (int)(0.7 * iIconWidth); // miejsce na "...", UWP: 48

            int iIcons = (int)Math.Floor(uiGrid.ActualWidth / iIconWidth);  
            // Lumia532: 320 - 48 / 68 = 4: zgadza się :)
            iIcons -= 2;    // odliczam miejsce na tekst
            return iIcons;
        }

        private void DopasowanieCmdBar()
        {// ustawienie w zależności od szerokości ekranu
         // Lumia 532 ma 480 px, i to są 4 ikonki + wielokropek
         // separator ma szerokość połowy, "..." trochę więcej (jakieś 3/4)
            int iIcons = CmdBarWidth();

            // pomysły:
            // text + 10 + ... : sep, 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
            // text + 9 + ... : 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek
            // text + 5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)
            // text + 2 + ... : text, submenu przelacznika, submenu komend, wielokropek
            // (czyli migracje miedzy secondary a primarycommand)
            //System.Diagnostics.Debug.WriteLine("ikonek ma być niby " + iIcons.ToString());
            bool bUWP = pkar.GetPlatform("uwp");


            if (iIcons > 8 )
            {
                // text + 8.5 + ... : sep, 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek

                // zrob co trzeba i dalej nie idź
                // primary commands
                tbDzien.Visibility = Visibility.Visible;
                uiBarSeparat1.Visibility = Visibility.Visible;
                bRefresh.Visibility = Visibility.Visible;
                uiKalendUWP.Visibility = bUWP ? Visibility.Visible:Visibility.Collapsed;
                uiKalendAndro.Visibility = bUWP ? Visibility.Collapsed : Visibility.Visible;
                uiBarSeparat2.Visibility = Visibility.Visible;
                uiSelektorStrony.Visibility = Visibility.Collapsed;
                bEvent.Visibility = Visibility.Visible;
                // bHolid.Visibility = Visibility.Visible;
                bBirth.Visibility = Visibility.Visible; 
                bDeath.Visibility = Visibility.Visible;
                uiBarSeparat3.Visibility = Visibility.Visible;
                uiGoSett.Visibility = Visibility.Visible;
                uiGoInfo.Visibility = Visibility.Visible;
                
                // secondary commands
                uiKalendUWPSec.Visibility = Visibility.Collapsed;
                uiKalendAndroSec.Visibility = Visibility.Collapsed; 
                uiGoSettSec.Visibility = Visibility.Collapsed;
                uiGoInfoSec.Visibility = Visibility.Collapsed; 


                return;
            }


            if (iIcons > 7)
            {
                // text + 8 + ... : 2x (load, date), sep, 3x przelacznik zawartosci, sep, 2x (info, setup), wielokropek

                // zrob co trzeba i dalej nie idź
                // primary commands
                tbDzien.Visibility = Visibility.Visible;
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                bRefresh.Visibility = Visibility.Visible;
                uiKalendUWP.Visibility = bUWP ? Visibility.Visible : Visibility.Collapsed;
                uiKalendAndro.Visibility = bUWP ? Visibility.Collapsed : Visibility.Visible;
                uiBarSeparat2.Visibility = Visibility.Visible;
                uiSelektorStrony.Visibility = Visibility.Collapsed;
                bEvent.Visibility = Visibility.Visible;
                // bHolid.Visibility = Visibility.Visible;
                bBirth.Visibility = Visibility.Visible;
                bDeath.Visibility = Visibility.Visible;
                uiBarSeparat3.Visibility = Visibility.Visible;
                uiGoSett.Visibility = Visibility.Visible;
                uiGoInfo.Visibility = Visibility.Visible;

                // secondary commands
                uiKalendUWPSec.Visibility = Visibility.Collapsed;
                uiKalendAndroSec.Visibility = Visibility.Collapsed;
                uiGoSettSec.Visibility = Visibility.Collapsed;
                uiGoInfoSec.Visibility = Visibility.Collapsed;

                return;
            }

            if (iIcons > 4)
            {
                // text + 4.5 + ... : 1x load, sep, 3x przelacznik zawartosci, wielokropek (info, setup, date)

                // zrob co trzeba i dalej nie idź
                // primary commands
                tbDzien.Visibility = Visibility.Visible;
                uiBarSeparat1.Visibility = Visibility.Collapsed;
                bRefresh.Visibility = Visibility.Visible;
                uiKalendUWP.Visibility = Visibility.Collapsed;
                uiKalendAndro.Visibility = Visibility.Collapsed;
                uiBarSeparat2.Visibility = Visibility.Visible;
                uiSelektorStrony.Visibility = Visibility.Collapsed;
                bEvent.Visibility = Visibility.Visible;
                // bHolid.Visibility = Visibility.Visible;
                bBirth.Visibility = Visibility.Visible;
                bDeath.Visibility = Visibility.Visible;
                uiBarSeparat3.Visibility = Visibility.Collapsed;
                uiGoSett.Visibility = Visibility.Collapsed;
                uiGoInfo.Visibility = Visibility.Collapsed;

                // secondary commands
                uiKalendUWPSec.Visibility = bUWP ? Visibility.Visible : Visibility.Collapsed;
                uiKalendAndroSec.Visibility = bUWP ? Visibility.Collapsed : Visibility.Visible;
                uiGoSettSec.Visibility = Visibility.Visible;
                uiGoInfoSec.Visibility = Visibility.Visible;

                return;
            }

            // najmniejsze

            // primary commands
            tbDzien.Visibility = Visibility.Visible;
            uiBarSeparat1.Visibility = Visibility.Collapsed;
            bRefresh.Visibility = Visibility.Visible;
            uiKalendUWP.Visibility = Visibility.Collapsed;
            uiKalendAndro.Visibility = Visibility.Collapsed;
            uiBarSeparat2.Visibility = Visibility.Collapsed;
            uiSelektorStrony.Visibility = Visibility.Visible;
            bEvent.Visibility = Visibility.Collapsed;
            // bHolid.Visibility = Visibility.Collapsed;
            bBirth.Visibility = Visibility.Collapsed;
            bDeath.Visibility = Visibility.Collapsed;
            uiBarSeparat3.Visibility = Visibility.Collapsed;
            uiGoSett.Visibility = Visibility.Collapsed;
            uiGoInfo.Visibility = Visibility.Collapsed;

            // secondary commands
            uiKalendUWPSec.Visibility = bUWP ? Visibility.Visible : Visibility.Collapsed;
            uiKalendAndroSec.Visibility = bUWP ? Visibility.Collapsed : Visibility.Visible;
            uiGoSettSec.Visibility = Visibility.Visible;
            uiGoInfoSec.Visibility = Visibility.Visible;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string sTmp;
            sTmp = pkar.GetSettingsString("EnabledTabs", "EBD");

            bEvent.IsEnabled = (sTmp.IndexOf("E",StringComparison.Ordinal ) > -1);
            bBirth.IsEnabled = (sTmp.IndexOf("B", StringComparison.Ordinal) > -1);
            bDeath.IsEnabled = (sTmp.IndexOf("D", StringComparison.Ordinal) > -1);
            //bHolid.IsEnabled = (sTmp.IndexOf("H",StringComparison.Ordinal) > -1);

            mDate = DateTime.Now;

            
            UwpAndro();             // Uno bug override - własny AppBar
            DopasowanieCmdBar();    // liczba ikonek a szerokość 

            if (pkar.GetSettingsBool("AutoLoad")) bRead_Click(null, null);

            //uiCmdBar.Padding = new Thickness(0, 0, 0, 50);
            //uiCmdBar.MinHeight = 100;
            //uiCmdBar.Margin.Bottom = 40;
            // bRead_Click(null, null);    // do testowania Android - appBar nie dziala?
            //bSetup_Click(null, null);
            //bInfo_Click(null, null);
        }

        private void wbViewer_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri == null)
                return;

            args.Cancel = true;

            if (!pkar.GetSettingsBool("LinksActive"))
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

        // Uno bug override
        private void uiDayAndro_Changed(object sender, DatePickerValueChangedEventArgs args)
        {// wizard daje tu Object sender, a przy Calendar.. - dokładny typ sendera.
            DatePicker oPicker;
            oPicker = sender as DatePicker;
            if (oPicker is null) return;

            if (oPicker.Date != null)
                mDate = oPicker.Date;
        }

        private void uiGrid_Resized(object sender, SizeChangedEventArgs e)
        {
            DopasowanieCmdBar();
        }
    }
}
