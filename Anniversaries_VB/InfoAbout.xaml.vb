' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class InfoAbout
    Inherits Page

    Private Sub bInfoOk(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
        'Me.Frame.GoBack()  ' niby lepsze, ale jak zmienia default na nie-pamietanie, to wylecialoby na error
    End Sub

    Private Sub uiInfoLoaded(sender As Object, e As RoutedEventArgs)
        Dim sTmp As String

        sTmp = "https://en.wikipedia.org/wiki/"
        sTmp = sTmp & VBlibekStd.MainPage.MonthNo2EnName(Date.Now.Month)
        sTmp = sTmp & "_" & Date.Now.Day

        uiWikiLink.Content = sTmp
        uiWikiLink.NavigateUri = New Uri(sTmp)

    End Sub

    Private Async Sub bRateIt_Click(sender As Object, e As RoutedEventArgs)
        Dim sUri As New Uri("ms-windows-store://review/?PFN=" & Package.Current.Id.FamilyName)
        Await Windows.System.Launcher.LaunchUriAsync(sUri)

    End Sub
End Class
