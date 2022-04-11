
Imports Windows.Storage
Imports VBlib.Extensions

Public NotInheritable Class Setup
    Inherits Page

    Private Sub bSetupOk(sender As Object, e As RoutedEventArgs)
        Dim sTmp As String

        sTmp = ""
        If uiSetLangPl.IsOn Then sTmp &= "pl "
        If uiSetLangFr.IsOn Then sTmp &= "fr "
        If uiSetLangEs.IsOn Then sTmp &= "es "
        If uiSetLangRu.IsOn Then sTmp &= "ru "
        If uiSetLangDe.IsOn Then sTmp &= "de "
        App.SetSettingsString("EnabledLanguages", sTmp)

        sTmp = ""
        If uiSetTabE.IsOn Then sTmp &= "E"
        If uiSetTabB.IsOn Then sTmp &= "B"
        If uiSetTabD.IsOn Then sTmp &= "D"
        If uiSetTabH.IsOn Then sTmp &= "H"
        App.SetSettingsString("EnabledTabs", sTmp)

        App.SetSettingsBool("LinksActive", uiSetLinksActive.IsOn)

        Me.Frame.Navigate(GetType(MainPage))
        'Me.Frame.GoBack()
    End Sub

    Private Sub SetupPage_Loaded(sender As Object, e As RoutedEventArgs)
        Dim sTmp As String
        sTmp = App.GetSettingsString("EnabledLanguages", "pl de fr es ru")

        uiSetLangEn.IsOn = True
        uiSetLangPl.IsOn = (sTmp.IndexOfOrdinal("pl") > -1)
        uiSetLangFr.IsOn = (sTmp.IndexOfOrdinal("fr") > -1)
        uiSetLangEs.IsOn = (sTmp.IndexOfOrdinal("es") > -1)
        uiSetLangRu.IsOn = (sTmp.IndexOfOrdinal("ru") > -1)
        uiSetLangDe.IsOn = (sTmp.IndexOfOrdinal("de") > -1)

        sTmp = App.GetSettingsString("EnabledTabs", "EBD")

        uiSetTabE.IsOn = (sTmp.IndexOfOrdinal("E") > -1)
        uiSetTabB.IsOn = (sTmp.IndexOfOrdinal("B") > -1)
        uiSetTabD.IsOn = (sTmp.IndexOfOrdinal("D") > -1)
        uiSetTabH.IsOn = (sTmp.IndexOfOrdinal("H") > -1)

        uiSetLinksActive.IsOn = App.GetSettingsBool("LinksActive")

        ' włoski nie, bo ma podstrony!
    End Sub
End Class
