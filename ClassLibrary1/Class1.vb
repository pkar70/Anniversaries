Public Class Class1

    Public Shared Function NetIsIPavailable() As Boolean

        ' .Net Core 1.0; .Net Framework 2.0; .Net Standard 2.0; UWP 10.
        If System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() Then Return True

        Return False
        End Function

End Class
