using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

// to co nie może być w Shared, bo jest unimplemented in Uno


namespace Anniversaries
{
    public static partial class pkar
    {


        
        // -- CLIPBOARD ---------------------------------------------



        public async static System.Threading.Tasks.Task<bool> NetWiFiOffOn()
        {

            // https://social.msdn.microsoft.com/Forums/ie/en-US/60c4a813-dc66-4af5-bf43-e632c5f85593/uwpbluetoothhow-to-turn-onoff-wifi-bluetooth-programmatically?forum=wpdevelop
            var result222 = await Windows.Devices.Radios.Radio.RequestAccessAsync();
            IReadOnlyList<Windows.Devices.Radios.Radio> radios = await Windows.Devices.Radios.Radio.GetRadiosAsync();

            foreach (var oRadio in radios)
            {
                if (oRadio.Kind == Windows.Devices.Radios.RadioKind.WiFi)
                {
                    Windows.Devices.Radios.RadioAccessStatus oStat = await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.Off);
                    if (oStat != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                        return false;
                    await System.Threading.Tasks.Task.Delay(3 * 1000);
                    oStat = await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.On);
                    if (oStat != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                        return false;
                }
            }

            return true;
        }

        public async static System.Threading.Tasks.Task<string> DialogBoxInput(string sMsgResId, string sDefaultResId = "", string sYesResId = "resDlgContinue", string sNoResId = "resDlgCancel")
        {
            string sMsg, sYes, sNo, sDefault;

            sDefault = "";

            {
                var withBlock = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                sMsg = withBlock.GetString(sMsgResId);
                sYes = withBlock.GetString(sYesResId);
                sNo = withBlock.GetString(sNoResId);
                if (!string.IsNullOrEmpty(sDefaultResId))
                    sDefault = withBlock.GetString(sDefaultResId);
            }

            if (string.IsNullOrEmpty(sMsg))
                sMsg = sMsgResId;  // zabezpieczenie na brak string w resource
            if (string.IsNullOrEmpty(sYes))
                sYes = sYesResId;
            if (string.IsNullOrEmpty(sNo))
                sNo = sNoResId;
            if (string.IsNullOrEmpty(sDefault))
                sDefault = sDefaultResId;

            var oInputTextBox = new Windows.UI.Xaml.Controls.TextBox();
            oInputTextBox.AcceptsReturn = false;
            oInputTextBox.Text = sDefault;
            Windows.UI.Xaml.Controls.ContentDialog oDlg = new Windows.UI.Xaml.Controls.ContentDialog();
            oDlg.Content = oInputTextBox;
            oDlg.PrimaryButtonText = sYes;
            oDlg.SecondaryButtonText = sNo;
            oDlg.Title = sMsg;

            var oCmd = await oDlg.ShowAsync();
            if (oCmd != Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                return "";

            return oInputTextBox.Text;
        }

        public static void SetBadgeNo(int iInt)
        {
            // https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/tiles-and-notifications-badges

            Windows.Data.Xml.Dom.XmlDocument oXmlBadge;
            oXmlBadge = Windows.UI.Notifications.BadgeUpdateManager.GetTemplateContent(Windows.UI.Notifications.BadgeTemplateType.BadgeNumber);

            Windows.Data.Xml.Dom.XmlElement oXmlNum;
            oXmlNum = (Windows.Data.Xml.Dom.XmlElement)oXmlBadge.SelectSingleNode("/badge");
            oXmlNum.SetAttribute("value", iInt.ToString(System.Globalization.CultureInfo.InvariantCulture));

            Windows.UI.Notifications.BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(new Windows.UI.Notifications.BadgeNotification(oXmlBadge));
        }


        public static int WinVer()
        {
            // Unknown = 0,
            // Threshold1 = 1507,   // 10240
            // Threshold2 = 1511,   // 10586
            // Anniversary = 1607,  // 14393 Redstone 1
            // Creators = 1703,     // 15063 Redstone 2
            // FallCreators = 1709 // 16299 Redstone 3
            // April = 1803		// 17134
            // October = 1809		// 17763
            // ? = 190?		// 18???

            // April  1803, 17134, RS5

            ulong u = ulong.Parse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion, System.Globalization.CultureInfo.InvariantCulture);
            u = (u & 0xFFFF0000L) >> 16;
            return (int)u;
        }

        private static Windows.Web.Http.HttpClient moHttp = new Windows.Web.Http.HttpClient();

        public async static System.Threading.Tasks.Task<string> HttpPageAsync(string sUrl, string sErrMsg, string sData = "")
        {
            try
            {
                if (!NetIsIPavailable(true))
                    return "";
                if (string.IsNullOrEmpty(sUrl))
                    return "";

                if (sUrl.Substring(0, 4) != "http")
                    sUrl = "http://beskid.geo.uj.edu.pl/p/dysk" + sUrl;

                if (moHttp == null)
                {
                    moHttp = new Windows.Web.Http.HttpClient();
                    moHttp.DefaultRequestHeaders.UserAgent.TryParseAdd("GrajCyganie");
                }

                var sError = "";
                Windows.Web.Http.HttpResponseMessage oResp = null;

                try
                {
                    if (!string.IsNullOrEmpty(sData))
                    {
                        var oHttpCont = new Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
                        oResp = await moHttp.PostAsync(new Uri(sUrl), oHttpCont);
                        oHttpCont.Dispose();
                    }
                    else
                        oResp = await moHttp.GetAsync(new Uri(sUrl));
                }
                catch (Exception ex)
                {
                    sError = ex.Message;
                }

                if (!string.IsNullOrEmpty(sError))
                {
                    await DialogBox("error " + sError + " at " + sErrMsg + " page");
                    return "";
                }

                //if ((oResp.StatusCode == 303) | (oResp.StatusCode == 302) | (oResp.StatusCode == 301))
                if ((oResp.StatusCode == Windows.Web.Http.HttpStatusCode.SeeOther ) ||
                    (oResp.StatusCode ==  Windows.Web.Http.HttpStatusCode.Found) ||
                    (oResp.StatusCode == Windows.Web.Http.HttpStatusCode.MovedPermanently))
                {
                    // redirect
                    sUrl = oResp.Headers.Location.ToString();
                    // If sUrl.ToLower.Substring(0, 4) <> "http" Then
                    // sUrl = "https://sympatia.onet.pl/" & sUrl   ' potrzebne przy szukaniu
                    // End If

                    if (!string.IsNullOrEmpty(sData))
                    {
                        // Dim oHttpCont = New HttpStringContent(sData, Text.Encoding.UTF8, "application/x-www-form-urlencoded")
                        var oHttpCont = new Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
                        oResp = await moHttp.PostAsync(new Uri(sUrl), oHttpCont);
                        oHttpCont.Dispose();
                    }
                    else
                        oResp = await moHttp.GetAsync(new Uri(sUrl));
                }

                if ((int)oResp.StatusCode > 290)
                {
                    await DialogBox("ERROR " + oResp.StatusCode + " getting " + sErrMsg + " page");
                    return "";
                }

                string sResp = "";
                try
                {
                    sResp = await oResp.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    sError = ex.Message;
                }

                if (!string.IsNullOrEmpty(sError))
                {
                    await DialogBox("error " + sError + " at ReadAsStringAsync " + sErrMsg + " page");
                    return "";
                }

                return sResp;
            }
            catch (Exception ex)
            {
                CrashMessageExit("@HttpPageAsync", ex.Message);
            }

            return "";
        }

        public static void OpenBrowser(Uri oUri, bool bForceEdge)
        {
            if (bForceEdge)
            {
                Windows.System.LauncherOptions options = new Windows.System.LauncherOptions();
                options.TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe";
                /* TODO ERROR: Skipped WarningDirectiveTrivia */
                Windows.System.Launcher.LaunchUriAsync(oUri, options);
            }
            else
                Windows.System.Launcher.LaunchUriAsync(oUri);
        }

        public static void OpenBrowser(string sUri, bool bForceEdge)
        {
            Uri oUri = new Uri(sUri);
            OpenBrowser(oUri, bForceEdge);
        }

    }
}
