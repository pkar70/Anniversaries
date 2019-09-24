using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;



namespace Anniversaries
{
    public sealed partial class Setup : Page
    {
        public Setup()
        {
            this.InitializeComponent();
        }

        private void bSetupOk(object sender, RoutedEventArgs e)
        {
            string sTmp;

            sTmp = "";
            if (uiSetLangPl.IsOn) sTmp = sTmp + "pl ";
            if (uiSetLangFr.IsOn) sTmp = sTmp + "fr ";
            if (uiSetLangEs.IsOn) sTmp = sTmp + "es ";
            if (uiSetLangRu.IsOn) sTmp = sTmp + "ru ";
            if (uiSetLangDe.IsOn) sTmp = sTmp + "de ";
            pkar.SetSettingsString("EnabledLanguages", sTmp);

            sTmp = "";
            if (uiSetTabE.IsOn) sTmp = sTmp + "E";
            if (uiSetTabB.IsOn) sTmp = sTmp + "B";
            if (uiSetTabD.IsOn) sTmp = sTmp + "D";
            //if (uiSetTabH.IsOn) sTmp = sTmp + "H";
            pkar.SetSettingsString("EnabledTabs", sTmp);

            pkar.SetSettingsBool("LinksActive", uiSetLinksActive);
            pkar.SetSettingsBool("AutoLoad", uiAutoLoad);
            pkar.SetSettingsBool("localSetup", true);   // na wszelki wypadek - żeby nie zassał z OneDrive
            
            this.Frame.Navigate(typeof(MainPage));
        }

        private void SetupPage_Loaded(object sender, RoutedEventArgs e)
        {
            string sTmp;
            sTmp = pkar.GetSettingsString("EnabledLanguages", "pl de fr es ru");

            uiVersion.Text = "v. " + pkar.GetAppVers();

            uiSetLangEn.IsOn = true;
            uiSetLangPl.IsOn = (sTmp.IndexOf("pl",StringComparison.Ordinal) > -1);
            uiSetLangFr.IsOn = (sTmp.IndexOf("fr", StringComparison.Ordinal) > -1);
            uiSetLangEs.IsOn = (sTmp.IndexOf("es", StringComparison.Ordinal) > -1);
            uiSetLangRu.IsOn = (sTmp.IndexOf("ru", StringComparison.Ordinal) > -1);
            uiSetLangDe.IsOn = (sTmp.IndexOf("de", StringComparison.Ordinal) > -1);

            sTmp = pkar.GetSettingsString("EnabledTabs", "EBD");

            uiSetTabE.IsOn = (sTmp.IndexOf("E", StringComparison.Ordinal) > -1);
            uiSetTabB.IsOn = (sTmp.IndexOf("B", StringComparison.Ordinal) > -1);
            uiSetTabD.IsOn = (sTmp.IndexOf("D", StringComparison.Ordinal) > -1);
            //uiSetTabH.IsOn = (sTmp.IndexOf("H", StringComparison.Ordinal) > -1);

            uiSetLinksActive.IsOn = pkar.GetSettingsBool("LinksActive");
            pkar.GetSettingsBool(uiAutoLoad, "AutoLoad");
        }

    }
}
