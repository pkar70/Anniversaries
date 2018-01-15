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
        App.SetSettingsString("EnabledLanguages", sTmp)

        sTmp = ""
        If uiSetTabE.IsOn Then sTmp = sTmp & "E"
        If uiSetTabB.IsOn Then sTmp = sTmp & "B"
        If uiSetTabD.IsOn Then sTmp = sTmp & "D"
        If uiSetTabH.IsOn Then sTmp = sTmp & "H"
        App.SetSettingsString("EnabledTabs", sTmp)

        App.SetSettingsBool("LinksActive", uiSetLinksActive.IsOn)

        Me.Frame.Navigate(GetType(MainPage))
        'Me.Frame.GoBack()
    End Sub

    Private Sub SetupPage_Loaded(sender As Object, e As RoutedEventArgs)
        Dim sTmp As String
        sTmp = App.GetSettingsString("EnabledLanguages", "pl de fr es ru")

        uiSetLangEn.IsOn = True
        uiSetLangPl.IsOn = (sTmp.IndexOf("pl") > -1)
        uiSetLangFr.IsOn = (sTmp.IndexOf("fr") > -1)
        uiSetLangEs.IsOn = (sTmp.IndexOf("es") > -1)
        uiSetLangRu.IsOn = (sTmp.IndexOf("ru") > -1)

        sTmp = App.GetSettingsString("EnabledTabs", "EBD")

        uiSetTabE.IsOn = (sTmp.IndexOf("E") > -1)
        uiSetTabB.IsOn = (sTmp.IndexOf("B") > -1)
        uiSetTabD.IsOn = (sTmp.IndexOf("D") > -1)
        uiSetTabH.IsOn = (sTmp.IndexOf("H") > -1)

        uiSetLinksActive.IsOn = App.GetSettingsBool("LinksActive")

    End Sub
End Class
