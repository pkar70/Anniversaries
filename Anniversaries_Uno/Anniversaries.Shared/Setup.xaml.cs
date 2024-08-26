using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//using static VBlib.Extensions;
using vb14 = VBlib.pkarlibmodule14;
using pkar.UI.Extensions;
using pkar.UI.Configs;
using static p.Extensions;
using static pkar.DotNetExtensions;

namespace Anniversaries
{
    public sealed partial class Setup : Page
    {
        public Setup()
        {
            this.InitializeComponent();
        }

        #region "ew class uwp"
        // VBuwp.Setup. , ale nie da się zrobić  reference from Uno

        private static string GetActiveTabs(StackPanel uiTabOnOff)
        {
            string sTmp = "";

            foreach (FrameworkElement oItem in uiTabOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWithOrdinal("uiSetTab"))
                {
                    if ((oItem as ToggleSwitch).IsOn)
                        sTmp += oItem.Name.Replace("uiSetTab", "").ToUpperInvariant();
                }
            }
            return sTmp;
        }

        private static string GetActiveLangs(StackPanel uiLangOnOff)
        {
            string sTmp = "";

            foreach (FrameworkElement oItem in uiLangOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWith("uiSetLang"))
                {
                    if ((oItem as ToggleSwitch).IsOn)
                        sTmp = sTmp + oItem.Name.Replace("uiSetLang", "").ToUpperInvariant() + " ";
                }
            }
            return sTmp;
        }

        private static void SetActiveTabs(StackPanel uiTabOnOff, string EnabledTabs)
        {
            foreach (FrameworkElement oItem in uiTabOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWith("uiSetTab"))
                {
                    if ((oItem as ToggleSwitch).IsOn)
                        (oItem as ToggleSwitch).IsOn = EnabledTabs.Contains(oItem.Name.Replace("uiSetTab", "").ToUpperInvariant());
                }
            }
        }

        private static void SetActiveLangs(StackPanel uiLangOnOff, string EnabledLanguages)
        {
            EnabledLanguages = EnabledLanguages.ToUpperInvariant();
            foreach (FrameworkElement oItem in uiLangOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWith("uiSetLang"))
                {
                    (oItem as ToggleSwitch).IsOn = EnabledLanguages.Contains(oItem.Name.Replace("uiSetLang", "").ToUpperInvariant() + " ");
                }
            }
        }

        #endregion 

        private void bSetupOk(object sender, RoutedEventArgs e)
        {
            string sTmp = GetActiveTabs(uiTabOnOff);

            if(sTmp == "")
            {
                vb14.DialogBoxRes("noTabSelected");
                return;
            }

            vb14.SetSettingsString("EnabledTabs", sTmp);

            // odczytanie wszystkich uiSetLang, żeby nie trzeba było tu zmieniać kodu - po ToUpper()
            sTmp = GetActiveLangs(uiLangOnOff);

            vb14.SetSettingsString("EnabledLanguages", sTmp);

            uiSetLinksActive.SetSettingsBool("LinksActive");
            uiAutoLoad.SetSettingsBool("AutoLoad");
            vb14.SetSettingsBool("localSetup", true);   // na wszelki wypadek - żeby nie zassał z OneDrive

            this.GoBack();    // Navigate(typeof(MainPage));
        }

        private void AddOneLang(string sLang, bool bPolish, string sOffEn, string sOn, string sOffPl)
        {
            ToggleSwitch ts = new ToggleSwitch()
            {
                OnContent = sOn,
                HorizontalAlignment = HorizontalAlignment.Center,

                OffContent = (bPolish) ? sOffPl : sOffEn
            };

            if (sLang.ToUpperInvariant() == "EN")
            {
                ts.Name = "uiSet" + sLang;  // krótsze dla En, żeby nie było przełączania
                ts.Header = pkar.Localize.TryGetResManString("uiSetLang_Hdr");
                ts.IsEnabled = false;
                ts.Margin = new Thickness(10, 10, 0, 0);
                ts.IsOn = true;
            }
            else
            { 
                ts.Name = "uiSetLang" + sLang;
                ts.Margin = new Thickness(10, 5, 0, 0);
            }

            uiLangOnOff.Children.Add(ts);
        }

        private void AddLangSwitches()
        {
            uiLangOnOff.Children.Clear();
            bool bPolish = (pkar.Localize.TryGetResManString("_lang").ToUpperInvariant() == "PL");

            AddOneLang("En", bPolish, "english", "English", "angielski");

            AddOneLang("De", bPolish, "german", "Deutsch", "niemiecki");
            AddOneLang("Es", bPolish, "spanish", "Español", "hiszpański");
            AddOneLang("Fr", bPolish, "french", "Français", "francuski");
            AddOneLang("Pl", bPolish, "polish", "Polski", "polski");
            AddOneLang("Ru", bPolish, "russian", "Русский", "rosyjski");
            AddOneLang("Uk", bPolish, "ukrainian", "Українська", "ukrainski");
            AddOneLang("El", bPolish, "greek", "Ελληνικά", "grecki");
            AddOneLang("He", bPolish, "hebrew", "עברית", "hebrajski");
            AddOneLang("Ja", bPolish, "japanese", "日本語", "japoński");
            AddOneLang("Ar", bPolish, "arabic", "العربية", "arabski");
            AddOneLang("Ka", bPolish, "georgian", "ქართული", "gruziński");
            AddOneLang("Ko", bPolish, "korean", "한국어", "koreański");
            AddOneLang("Zh", bPolish, "chineese", "中文", "chiński");

            if (p.k.GetPlatform("uwp"))
            {
                var tb = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10, 20, 0, 0),
                    Text = pkar.Localize.TryGetResManString("uiSetAddLang_Text")
                };

                uiLangOnOff.Children.Add(tb);
            }

        }


        private void SetupPage_Loaded(object sender, RoutedEventArgs e)
        {

            uiVersion.ShowAppVers();
            AddLangSwitches();

            SetActiveLangs(uiLangOnOff, vb14.GetSettingsString("EnabledLanguages"));

            SetActiveTabs(uiTabOnOff, vb14.GetSettingsString("EnabledTabs"));

            uiSetLinksActive.GetSettingsBool("LinksActive");
            uiAutoLoad.GetSettingsBool("AutoLoad");

        }

    }
}
