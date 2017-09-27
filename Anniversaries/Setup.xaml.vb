' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.Storage
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Setup
    Inherits Page

    Private Sub bSetupOk(sender As Object, e As RoutedEventArgs)
        Dim sTmp As String

        sTmp = ""
        If uiSetLangPl.IsOn Then sTmp = sTmp & "pl "
        If uiSetLangFr.IsOn Then sTmp = sTmp & "fr "
        If uiSetLangEs.IsOn Then sTmp = sTmp & "es "
        If uiSetLangRu.IsOn Then sTmp = sTmp & "ru "
        ApplicationData.Current.LocalSettings.Values("EnabledLanguages") = sTmp

        sTmp = ""
        If uiSetTabE.IsOn Then sTmp = sTmp & "E"
        If uiSetTabB.IsOn Then sTmp = sTmp & "B"
        If uiSetTabD.IsOn Then sTmp = sTmp & "D"
        If uiSetTabH.IsOn Then sTmp = sTmp & "H"
        ApplicationData.Current.LocalSettings.Values("EnabledTabs") = sTmp

        ApplicationData.Current.LocalSettings.Values("LinksActive") = uiSetLinksActive.IsOn.ToString

        Me.Frame.Navigate(GetType(MainPage))
        'Me.Frame.GoBack()
    End Sub

    Private Sub SetupPage_Loaded(sender As Object, e As RoutedEventArgs)
        Dim sTmp As String
        If ApplicationData.Current.LocalSettings.Values.ContainsKey("EnabledLanguages") Then
            sTmp = ApplicationData.Current.LocalSettings.Values("EnabledLanguages").ToString
        Else
            sTmp = "pl de fr es ru"
        End If

        uiSetLangEn.IsOn = True
        uiSetLangPl.IsOn = (sTmp.IndexOf("pl") > -1)
        uiSetLangFr.IsOn = (sTmp.IndexOf("fr") > -1)
        uiSetLangEs.IsOn = (sTmp.IndexOf("es") > -1)
        uiSetLangRu.IsOn = (sTmp.IndexOf("ru") > -1)

        If ApplicationData.Current.LocalSettings.Values.ContainsKey("EnabledTabs") Then
            sTmp = ApplicationData.Current.LocalSettings.Values("EnabledTabs").ToString
        Else
            sTmp = "EBD"
        End If

        uiSetTabE.IsOn = (sTmp.IndexOf("E") > -1)
        uiSetTabB.IsOn = (sTmp.IndexOf("B") > -1)
        uiSetTabD.IsOn = (sTmp.IndexOf("D") > -1)
        uiSetTabH.IsOn = (sTmp.IndexOf("H") > -1)

        If ApplicationData.Current.LocalSettings.Values.ContainsKey("LinksActive") Then
            sTmp = ApplicationData.Current.LocalSettings.Values("LinksActive").ToString
        Else
            sTmp = "0"
        End If

        uiSetLinksActive.IsOn = CBool(sTmp)

    End Sub
End Class
