
' podłączenie VBLib, do testowania czy działa

' 
'   STORE: 1.7.1.0, 2018.09.03


Imports System.Windows
Imports VBlib.Extensions
Imports vb14 = VBlib.pkarlibmodule14
Imports inLib = VBlib.MainPage


Public NotInheritable Class MainPage
    Inherits Page

    ' zmienne w Setup dostepne
    Dim mDate As DateTimeOffset

    Private Sub bSetup_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Setup))
    End Sub

    Private Sub bInfo_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(InfoAbout))
    End Sub

    ' w VB libek
    ' Public Shared Function MonthNo2EnName(iMonth As Integer)
    ' Private Function MonthNo2PlName(iMonth As Integer)
    ' Private Function WyciagnijDane(sPage As String, sFrom As String) As String
    ' Private Function WytnijObrazkiDE(sPage As String) As String - to kompletnie przepisane
    ' Private Function WytnijObrazkiFR(sPage As String) As String
    ' Private Function Li2Rok(sTxt As String) As Integer
    ' Private Function PrepareAndSplitSort(sPage As String) As String
    ' Private Function SplitAndSort(sTxtIn As String) As String
    ' Private Function PoprawRok(sTxt As String) As String
    ' Private Function MergeSorted(sTxt1 As String, sTxt2 As String) As String
    ' Private Function DodajPelnyLink(sPage As String, sLang As String) As String
    ' Private Async Function ReadOneLang(sUrl As String, sPrefLang As String) As Task(Of String)
    ' Private Function ExtractLangLinks(sForLangs As String, sPage As String) As List(Of String)
    ' Private Async Function GetHtmlPage(sUrl As String) As Task(Of String)

    Private Async Sub bRead_Click(sender As Object, e As RoutedEventArgs)
        Dim sUrl As String = "https://en.wikipedia.org/wiki/" &
            inLib.MonthNo2EnName(mDate.Month) &
            "_" & mDate.Day.ToString(Globalization.CultureInfo.InvariantCulture)

        inLib.bRead_Click_Reset()
        tbDzien.Text = "Reading EN..."

        Dim sTxt As String = Await inLib.ReadOneLang(New Uri(sUrl), App.GetSettingsString("EnabledTabs", "EBD"))

        sUrl = App.GetSettingsString("EnabledLanguages", "pl de fr es ru")
        Dim lList As List(Of String) = inLib.ExtractLangLinks(sUrl, sTxt)
        For Each sUri As String In lList
            tbDzien.Text = "Reading " & sUri.Substring(8, 2).ToUpperInvariant & "..."
            Await inLib.ReadOneLang(New Uri(sUri), App.GetSettingsString("EnabledTabs", "EBD"))
        Next

        Dim bZaden As Boolean = True
        If bEvent.IsChecked Then
            bZaden = False
            bEvent_Click(sender, e)
        End If
        If bHolid.IsChecked Then
            bZaden = False
            bHolid_Click(sender, e)
        End If
        If bBirth.IsChecked Then
            bZaden = False
            bBirth_Click(sender, e)
        End If
        If bDeath.IsChecked Then
            bZaden = False
            bDeath_Click(sender, e)
        End If

        If bZaden Then bEvent_Click(sender, e)

        tbDzien.Text = mDate.ToString("d MMMM", Globalization.CultureInfo.InvariantCulture)  ' .Day.ToString & " " & MonthNo2PlName(mDate.Month)

        'jesli jest: http://brewiarz.pl/ix_17/1009/index.php3
        'else http://brewiarz.pl/ix_17/0909p/index.php3
        'Od "Dzisiejsi patroni:" do "</div", zamiana ";" na <li>? (ale czasem jest błąd, tzn. ";</a>")

    End Sub
    Private Sub SetWebView(sHtml As String, sHead As String)
        wbViewer.Height = naView.ActualHeight - 10
        wbViewer.Width = naView.ActualWidth - 10
        sHtml = "<html><head>" & sHead & "</head><body>" & sHtml & "</body></html>"
        wbViewer.NavigateToString(sHtml)
    End Sub

    Private Sub ToggleButtony(bEv As Boolean, bBir As Boolean, bDea As Boolean)
        '// primary
        bEvent.IsChecked = bEv
        bHolid.IsChecked = False
        bBirth.IsChecked = bBir
        bDeath.IsChecked = bDea
    End Sub


    Private Sub SetWebView(sDoc As String)
        ' head (kiedyś): "<base href=""https://en.wikipedia.org/"">"
        If String.IsNullOrEmpty(sDoc) Then
            vb14.DialogBoxRes("errNoData")
        Else
            SetWebView(sDoc, "")
        End If
    End Sub

    Private Sub bEvent_Click(sender As Object, e As RoutedEventArgs)
        SetWebView(inLib.GetContentForWebview("E"))
        ToggleButtony(True, False, False)
    End Sub
    Private Sub bHolid_Click(sender As Object, e As RoutedEventArgs)
        'SetWebView(mHolid, "") '  "<base href=""https://en.wikipedia.org/"">")
        'bEvent.IsChecked = False
        'bHolid.IsChecked = True
        'bBirth.IsChecked = False
        'bDeath.IsChecked = False
    End Sub
    Private Sub bBirth_Click(sender As Object, e As RoutedEventArgs)
        SetWebView(inLib.GetContentForWebview("B"))
        ToggleButtony(False, True, False)
    End Sub
    Private Sub bDeath_Click(sender As Object, e As RoutedEventArgs)
        SetWebView(inLib.GetContentForWebview("D"))
        ToggleButtony(False, False, True)
    End Sub

    Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs)

        Dim sTmp As String
        sTmp = App.GetSettingsString("EnabledTabs", "EBD")

        bEvent.IsEnabled = sTmp.Contains("E", StringComparison.Ordinal)
        bBirth.IsEnabled = sTmp.Contains("B", StringComparison.Ordinal)
        bDeath.IsEnabled = sTmp.Contains("D", StringComparison.Ordinal)
        bHolid.IsEnabled = sTmp.Contains("H", StringComparison.Ordinal)

        mDate = Date.Now
        uiDay.Date = mDate

    End Sub

    <CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification:="<Pending>")>
    Private Sub wbViewer_NavigationStarting(sender As WebView, args As WebViewNavigationStartingEventArgs) Handles wbViewer.NavigationStarting

        If args.Uri Is Nothing Then Exit Sub

        args.Cancel = True

        If Not App.GetSettingsBool("LinksActive") Then Exit Sub

#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
        Windows.System.Launcher.LaunchUriAsync(args.Uri)
#Enable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed

    End Sub

    Private Sub uiDay_Changed(sender As CalendarDatePicker, args As CalendarDatePickerDateChangedEventArgs) Handles uiDay.DateChanged
        ' 11/27/2017 9:11:28 PM  1.5.1.6  Console     Microsoft-Xbox One  10.0.16299.4037 
        ' System::Nullable$1_System::DateTimeOffset_.get_Value
        ' Anniversaries::MainPage.uiDay_Changed
        If uiDay.Date IsNot Nothing Then
            mDate = uiDay.Date
        End If
    End Sub
End Class
