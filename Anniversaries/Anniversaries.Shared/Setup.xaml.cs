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
            string sTmp = "";
            foreach (FrameworkElement oItem in uiTabOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWith("uiSetTab"))
                {
                    if ((oItem as ToggleSwitch).IsOn)
                        sTmp = sTmp + oItem.Name.Replace("uiSetTab", "").ToUpper();
                }
            }

            if(sTmp == "")
            {
                p.k.DialogBoxRes("noTabSelected");
                return;
            }

            p.k.SetSettingsString("EnabledTabs", sTmp);


            sTmp ="";

            // odczytanie wszystkich uiSetLang, żeby nie trzeba było tu zmieniać kodu
            foreach (FrameworkElement oItem in uiLangOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWith("uiSetLang"))
                {
                    if ((oItem as ToggleSwitch).IsOn)
                        sTmp = sTmp + oItem.Name.Replace("uiSetLang", "").ToLower() + " ";
                }
            }

            p.k.SetSettingsString("EnabledLanguages", sTmp);


            p.k.SetSettingsBool("LinksActive", uiSetLinksActive);
            p.k.SetSettingsBool("AutoLoad", uiAutoLoad);
            p.k.SetSettingsBool("localSetup", true);   // na wszelki wypadek - żeby nie zassał z OneDrive

            this.Frame.GoBack();    // Navigate(typeof(MainPage));
        }

        private void SetupPage_Loaded(object sender, RoutedEventArgs e)
        {

            uiVersion.Text = "v. " + p.k.GetAppVers();

            string sTmp;
            sTmp = p.k.GetSettingsString("EnabledLanguages", "pl de fr es ru");

            foreach (FrameworkElement oItem in uiLangOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWith("uiSetLang"))
                    (oItem as ToggleSwitch).IsOn = sTmp.IndexOf(oItem.Name.Replace("uiSetLang", "").ToLower()) > -1;
            }


            sTmp = p.k.GetSettingsString("EnabledTabs", "EBD");

            foreach (FrameworkElement oItem in uiTabOnOff.Children)
            {
                if (oItem is ToggleSwitch && oItem.Name.StartsWith("uiSetTab"))
                {
                        (oItem as ToggleSwitch).IsOn = sTmp.IndexOf(oItem.Name.Replace("uiSetTab", "").ToUpper()) > -1;
                }
            }

            p.k.GetSettingsBool(uiSetLinksActive, "LinksActive");
            p.k.GetSettingsBool(uiAutoLoad, "AutoLoad");

        }

    }
}
