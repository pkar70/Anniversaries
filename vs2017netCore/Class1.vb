' (...)
'            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed
'
'            ' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
'            AddHandler rootFrame.Navigated, AddressOf OnNavigatedAddBackButton
'            AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf OnBackButtonPressed
' (...)


' PLIK DOŁĄCZANY
' mklink pkarModule.vb ..\..\_mojeSuby\pkarModule.vb
' PLIK DOŁACZANY

' public async function AppServiceLocalCommand(sCommand as string) as string
' return ""
' end function

' 2021.12.14: XmlSafeString po nowemu, MakeToast zamiast XmlSafeString korzysta z XmlSafeStringQt
' 2021.10.21: Long.ToStringDHMS
' 2021.10.19: Int2BigNum (ze spacjami) jako int.ToStringWithSpaces, tak samo long.
' 2021.10.18: ReadText/WriteText: narzucanie encoding UTF8
' 2021.10.02: ProgRingShow: gdy jest Show w Show, to wtedy nie robi reset progBar.
' 2021.09.30: DebugBT przerobione jako Extension Function As String (bo chcę mieć w app wynik, nie w VStudio w oknie debug
' 2021.09.30: SerializeToJsonAsync (ale jest zakomentowane - niby uproszczenie nowych klas obsługujących typy)
' 2021.09.26: GetLangString: gdy nie ma ResourceMap, to później już nie próbuje i zwraca default/name (by było mniej zatrzymań podczas Debug-StepByStep)

' 2021.08.22: GetLogFileYearlyAsync
' 2021.08.10: GetLogFileMonthlyAsync jedno korzysta z drugiego


' 2021.07.10: *etSettingsLong
' 2021.06.29: SetSettString jako funkcja, ret=false gdy się nie udało
'               DebugOut - zapis do pliku gdy 2048 (a nie 8192), lub gdy za długa zmienna
' 2021.05.25: DialogBoxInputResAsync z dotychczasowego DialogBoxInputAsync, zas DialogBoxInputDirectAsync - dwa pierwsze bez Resource
' 2021.05.12: DebugOut z zapisem do pliku debug, GetSettingsInt("debugLogLevel",0), pkarDebug.Command "debug loglevel x" (takze z command line), za to usuwam DebugLogFileOutAsync

' 2021.05.05 Triggers: z StartingWith na Contains, oraz dodawanie USerPresent
' 2021.05.05: DistanceTo(ByVal oGeocoord0 As Windows.Devices.Geolocation.Geocoordinate (dla DailyIti)

' 2021.05.02: RemoveScheduledToasts
' 2021.05.02: SetSettingsDate(settName)
' 2021.05.01: podstawowe rzeczy do AppService (w App), do wykorzystania: RemSysInit, do dodania w kazdej App: AppServiceLocalCommand, GetSettingsBool("remoteSystemDisabled")


' 2021.04.28: UnregisterTriggers("") kasuje wszystkie (także te z innym początkiem)
' 2021.04.28: USUNIETE POTEM IsTriggerAppService(args) oraz 
' 2021.04.28: AppServiceStdCmd(sCommand As String) As String


' 2021.04.20: CrashMessageAdd(string) - ze jeden tylko
' 2021.04.19: bezparametrowe UnregisterTriggers i isregistered
' 2021.04.15: IsFamilyMobile - ze form dawalo "unknown"? przełączam na DeviceFamily (tu tak bylo, C# mialo inaczej!)
' 2021.04.15: katalog dla DataLogs - nazwa app, ale z wyrzucaniem spacji i apostrofów
' 2021.04.15: DodajTriggerPolnocny
' 2021.04.08: string.TrimBefore, .TrimBeforeLast, .TrimAfter, .TrimAfterLast, RemoveBetween
' 2021.04.08: ClipPutHtml
' 2021.04.07: int.MinMax
' 2021.03.20: IsFullVersion (DEBUG jako Full; reszta - wedle licencji z Store)
' 2021.03.20: Geoposition.DistanceTo(Geoposition), i to samo dla BasicGeoposition i Double; GetDomekGeopos(precision)
' 2021.03.15: GetAppVers(oTB): w wersji DEBUG dopisuje "(debug)"
' 2021.03.02: ToHexBytesString: poprawka, poprawnie zrobione 0 na początku
' 2021.03.12: GetLogFileMonthlyAsync

' 2021.02.25: oFold.FileExistsAsync
' 2021.02.02: ProgRingShow ma try/catch, bo jak jest wywoływane przy Page.UnLoad to może już nie być obiektów

' 2021.01.28: SYNC do .cs (bez Bluetooth)
' 2021.01.28: RegisterTimerTrigger(.., condition), dla SmogoMetra (condition: internet)
' 2021.01.16: DebugLogFileOutAsync
' 2021.01.08: IsFamilyDesktop, IsFromCmdLine
' 2021.01.05: Sett/GetSettingsBool(toggleButton), settingsName = toggleButton.Name
' 2020.12.29: GetLogFolderRootAsync
' 2020.12.29: Sett/GetSettingsBool(toggleswitch) - jesli nie ma podanej nazwy, to uzywa nazwy toggleSwitcha
' 2020.12.11: NetIsBTavailableAsync bez sprawdzania RequestAccess, bo to wymaga devCap=radios
' 2020.12.05: ToastOrDialogAsync
' 2020.11.29: NetTrySwitchBTOnAsync, HttpPageSetAgent, HttpPageReset
' 2020.11.28: double.MinMax
' 2020.11.25: oFold.OpenExplorer, NetIsMobile->IsFamilyMobile, 
' 2020.11.24: GetAppVers(null) - sam sobie robi
' 2020.11.20: dump urządzenia Bluetooth
' 2020.11.19: NetIsBTavailableAsync, oraz <summary> do niektórych funkcji
' 2020.11.12: GetSDcardFolderAsync, GetLogFolderAsync, GetLogFileAsync
' 2020.11.06: triggers: IsTriggersRegistered, UnregisterTriggers, CanRegisterTriggersAsync, RegisterTimerTrigger, RegisterToastTrigger
' 2020.11.04: Get/SetSettingsBool dla AppBarToggleButton (analogicznie jak CheckBox)
' 2020.10.29: wziety z RandkaMulti, dodałem "Async", etc., "sync" ze wszystkimi (głównie zestaw ProgRing/Bar, oraz Extensions do czytania z pliku)


Partial Public Class App

#Region "Back button"

    ' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
    Private Sub OnNavigatedAddBackButton(sender As Object, e As NavigationEventArgs)
        Try
            Dim oFrame As Frame = TryCast(sender, Frame)
            If oFrame Is Nothing Then Exit Sub

            Dim oNavig As Windows.UI.Core.SystemNavigationManager = Windows.UI.Core.SystemNavigationManager.GetForCurrentView

            If oFrame.CanGoBack Then
                oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible
            Else
                oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed
            End If

            Return

        Catch ex As Exception
            pkar.CrashMessageExit("@OnNavigatedAddBackButton", ex.Message)
        End Try

    End Sub

    Private Sub OnBackButtonPressed(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        Try
            TryCast(Window.Current.Content, Frame)?.GoBack()
            e.Handled = True
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "RemoteSystem/Background"
    Private moTaskDeferal As Windows.ApplicationModel.Background.BackgroundTaskDeferral = Nothing
    Private moAppConn As Windows.ApplicationModel.AppService.AppServiceConnection
    Private msLocalCmdsHelp As String = ""

    Private Sub RemSysOnServiceClosed(appCon As Windows.ApplicationModel.AppService.AppServiceConnection, args As Windows.ApplicationModel.AppService.AppServiceClosedEventArgs)
        If appCon IsNot Nothing Then appCon.Dispose()
        If moTaskDeferal IsNot Nothing Then
            moTaskDeferal.Complete()
            moTaskDeferal = Nothing
        End If
    End Sub

    Private Sub RemSysOnTaskCanceled(sender As Windows.ApplicationModel.Background.IBackgroundTaskInstance, reason As Windows.ApplicationModel.Background.BackgroundTaskCancellationReason)
        If moTaskDeferal IsNot Nothing Then
            moTaskDeferal.Complete()
            moTaskDeferal = Nothing
        End If
    End Sub

    ''' <summary>
    ''' do sprawdzania w OnBackgroundActivated
    ''' jak zwróci True, to znaczy że nie wolno zwalniać moTaskDeferal !
    ''' sLocalCmdsHelp: tekst do odesłania na HELP
    ''' </summary>
    Public Function RemSysInit(args As BackgroundActivatedEventArgs, sLocalCmdsHelp As String) As Boolean
        Dim oDetails As Windows.ApplicationModel.AppService.AppServiceTriggerDetails =
                TryCast(args.TaskInstance.TriggerDetails, Windows.ApplicationModel.AppService.AppServiceTriggerDetails)
        If oDetails Is Nothing Then Return False

        msLocalCmdsHelp = sLocalCmdsHelp

        AddHandler args.TaskInstance.Canceled, AddressOf RemSysOnTaskCanceled
        moAppConn = oDetails.AppServiceConnection
        AddHandler moAppConn.RequestReceived, AddressOf RemSysOnRequestReceived
        AddHandler moAppConn.ServiceClosed, AddressOf RemSysOnServiceClosed
        Return True

    End Function

    Public Async Function CmdLineOrRemSys(sCommand As String) As Task(Of String)
        Dim sResult As String = ""

        sResult = AppServiceStdCmd(sCommand, msLocalCmdsHelp)
        If String.IsNullOrEmpty(sResult) Then
            sResult = Await AppServiceLocalCommand(sCommand)
        End If

        Return sResult
    End Function

    Public Async Function ObsluzCommandLine(sCommand As String) As Task

        Dim oFold As Windows.Storage.StorageFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder
        If oFold Is Nothing Then Return

        Dim oLock As Windows.Storage.StorageFile = Await oFold.CreateFileAsync("cmdline.lock", Windows.Storage.CreationCollisionOption.ReplaceExisting)
        If oLock Is Nothing Then Return

        Dim sResult = Await CmdLineOrRemSys(sCommand)
        If String.IsNullOrEmpty(sResult) Then
            sResult = "(empty - probably unrecognized command)"
        End If

        Dim oResFile As Windows.Storage.StorageFile = Await oFold.CreateFileAsync("stdout.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting)
        If oResFile Is Nothing Then Return

        Await oResFile.WriteAllTextAsync(sResult)

        Await oLock.DeleteAsync
    End Function

    Private Async Sub RemSysOnRequestReceived(sender As Windows.ApplicationModel.AppService.AppServiceConnection, args As Windows.ApplicationModel.AppService.AppServiceRequestReceivedEventArgs)
        '// 'Get a deferral so we can use an awaitable API to respond to the message 

        Dim sStatus As String
        Dim sResult As String = ""
        Dim messageDeferral As Windows.ApplicationModel.AppService.AppServiceDeferral = args.GetDeferral()

        If GetSettingsBool("remoteSystemDisabled") Then
            sStatus = "No permission"
        Else

            Dim oInputMsg As Windows.Foundation.Collections.ValueSet = args.Request.Message

            sStatus = "ERROR while processing command"

            If oInputMsg.ContainsKey("command") Then

                Dim sCommand As String = oInputMsg("command")
                sResult = Await CmdLineOrRemSys(sCommand)
            End If

            If sResult <> "" Then sStatus = "OK"

        End If

        Dim oResultMsg As Windows.Foundation.Collections.ValueSet = New Windows.Foundation.Collections.ValueSet()
        oResultMsg.Add("status", sStatus)
        oResultMsg.Add("result", sResult)

        Await args.Request.SendResponseAsync(oResultMsg)

        messageDeferral.Complete()
        moTaskDeferal.Complete()

    End Sub


#End Region

End Class

Public Module pkar
#Region "CrashMessage"
    Private mHostName As String = ""
    Private mAppName As String = ""

    ''' <summary>
    ''' Inicjalizacja modułu - nazwa app, oraz hostname
    ''' </summary>
    Public Sub CrashMessageInit()
        If String.IsNullOrEmpty(mHostName) Then mHostName = GetHostName()
        If String.IsNullOrEmpty(mAppName) Then mAppName = Package.Current.DisplayName
    End Sub

    ''' <summary>
    ''' DialogBox z dotychczasowym logiem i skasowanie logu
    ''' </summary>
    Public Async Function CrashMessageShowAsync() As Task
        CrashMessageInit()
        Dim sTxt As String = GetSettingsString("appFailData")
        If sTxt = "" Then Return
        Await DialogBoxAsync(mAppName & " FAIL message:" & vbCrLf & sTxt)
        SetSettingsString("appFailData", "")
    End Function

    ''' <summary>
    ''' Dodaj do logu,
    ''' gdy debug to pokaż toast i wyślij DebugOut,
    ''' gdy release to toast gdy GetSettingsBool("crashShowToast") 
    ''' </summary>
    Public Sub CrashMessageAdd(sTxt As String)
        CrashMessageInit()
        Dim sAdd As String = Date.Now.ToString("HH:mm:ss") & " " & sTxt & vbCrLf & sTxt & vbCrLf
#If DEBUG Then
        ' linia z MyCameras - Toast replikowany, więc powinien podać z którego telefonu :)
        MakeToast(mAppName & "@" & mHostName & ", " & Date.Now.ToString("HH:mm:ss"), sTxt)
        ' MakeToast(sAdd)
        DebugOut(sAdd)
#Else
        If GetSettingsBool("crashShowToast") Then MakeToast(sAdd)
#End If
        SetSettingsString("appFailData", GetSettingsString("appFailData") & sAdd)
    End Sub


    ''' <summary>
    ''' Dodaj do logu,
    ''' gdy debug to pokaż toast i wyślij DebugOut,
    ''' gdy release to toast gdy GetSettingsBool("crashShowToast") 
    ''' </summary>
    Public Sub CrashMessageAdd(sTxt As String, exMsg As String)
        CrashMessageAdd(sTxt & vbCrLf & exMsg)
    End Sub

    ''' <summary>
    ''' Dodaj do logu, ewentualnie toast, i zakończ App
    ''' </summary>
    Public Sub CrashMessageExit(sTxt As String, exMsg As String)
        CrashMessageAdd(sTxt, exMsg)
        TryCast(Application.Current, App)?.Exit()
    End Sub

    ' wersja w MyCameras nie miała optional ze stack
    ''' <summary>
    ''' Dodaj do logu,
    ''' gdy debug to pokaż toast i wyślij DebugOut,
    ''' gdy release to toast gdy GetSettingsBool("crashShowToast") 
    ''' </summary>
    Public Sub CrashMessageAdd(sTxt As String, ex As Exception, Optional bWithStack As Boolean = False)
        Dim sMsg As String = ex.ToString & ":" & ex.Message
        If bWithStack AndAlso ex.StackTrace IsNot Nothing Then
            sMsg = sMsg & vbCrLf & ex.StackTrace
        End If
        CrashMessageAdd(sTxt, sMsg)
    End Sub


    Private mDebugLogFile As Windows.Storage.StorageFile = Nothing


    ''' <summary>
    ''' DebugOut z nazwą aktualnej funkcji i sMsg, oraz odpowiednio głęboko cofnięte
    ''' </summary>
    Public Sub DumpCurrMethod(Optional sMsg As String = "")
        Dim sTrace As String = Environment.StackTrace
        If String.IsNullOrEmpty(sTrace) Then
            Debug.WriteLine("<stack is empty>")
            Return
        End If

        Dim subs As String() = sTrace.Split(vbCr)
        Dim iLen As Integer = subs.Length
        If iLen < 4 Then
            Debug.WriteLine("<stack is za mały>")
            Return
        End If

        Dim sPrefix As String = " "
        For i = 1 To iLen - 6
            sPrefix = sPrefix & "  "
        Next

        ' skrócenie bardzo długiego typu:
        ' BtWatchDump.MainPage.VB$StateMachine_13_BTwatch_Received.MoveNext() 
        Dim sCurrMethod As String = subs(3).Trim.Substring(3)
        sCurrMethod = sCurrMethod.Replace(".VB$StateMachine_", ".VB$")
        If sCurrMethod.EndsWith(".MoveNext()") Then sCurrMethod = sCurrMethod.Substring(0, sCurrMethod.Length - 11)

        DebugOut(iLen - 5, sPrefix & sCurrMethod & " " & sMsg)
        'DebugOut(iLen - 5, "(prev: " & sPrefix & subs(4).Trim.Substring(3))

    End Sub

    ''' <summary>
    ''' DebugOut z komunikatem, odpowiednio głęboko cofnięte wedle głębokości CallStack oraz iLevel
    ''' </summary>
    Public Sub DumpMessage(sMsg As String, Optional iLevel As Integer = 1)
        Dim sTrace As String = Environment.StackTrace
        If String.IsNullOrEmpty(sTrace) Then
            DebugOut(sMsg)
            Return
        End If

        Dim subs As String() = sTrace.Split(vbCr)
        Dim iLen As Integer = subs.Length
        If iLen < 4 Then
            DebugOut(sMsg)
            Return
        End If

        Dim sPrefix As String = ""
        For i = 1 To iLen - 2 + iLevel
            sPrefix = sPrefix & "  "
        Next

        DebugOut(iLen - 2, sPrefix & sMsg)

    End Sub

    ''' <summary>
    ''' Wyślij DebugOut dodając prefix --PKAR-- (do łatwiejszego znajdywania w logu), także do pliku/zmiennej
    ''' </summary>
    Public Sub DebugOut(logLevel As Integer, sMsg As String)
        Debug.WriteLine("--PKAR---:    " & sMsg)

        If GetSettingsInt("debugLogLevel") < logLevel Then Return

        Dim sTxt As String = GetSettingsString("DebugOutData")
        sTxt = sTxt & vbCrLf & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " " & sMsg
        Dim bVarOk As Boolean = SetSettingsString("DebugOutData", sTxt)

        If sTxt.Length < 2048 AndAlso bVarOk Then Return

        ' jest już dużo, to zapisujemy
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
        DebugOutFlush()
#Enable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
        SetSettingsString("DebugOutData", "")
    End Sub

    Public Async Function DebugOutFlush() As Task
        Dim sTxt As String = GetSettingsString("DebugOutData")
        Try
            If mDebugLogFile Is Nothing Then
                mDebugLogFile = Await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync("log.txt", Windows.Storage.CreationCollisionOption.OpenIfExists)
                If mDebugLogFile Is Nothing Then Return
                Await mDebugLogFile.AppendLineAsync(vbCrLf & "===========================================")
                Await mDebugLogFile.AppendLineAsync("Start @" & DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") & vbCrLf)
            End If

            Await mDebugLogFile.AppendStringAsync(sTxt)
        Catch ex As Exception

        End Try
        SetSettingsString("DebugOutData", "")
    End Function

    ''' <summary>
    ''' Wyślij DebugOut dodając prefix --PKAR-- (do łatwiejszego znajdywania w logu), także do pliku/zmiennej
    ''' </summary>
    Public Sub DebugOut(sMsg As String)
        DebugOut(1, sMsg)
    End Sub

    ''' <summary>
    ''' Wyślij via DebugOut, a takze albo pokaż dialog albo zrób toast
    ''' </summary>
    Public Sub ToastOrDialog(bDialog As Boolean, sMsg As String)
        DebugOut(sMsg)
        If bDialog Then
            DialogBox(sMsg)
        Else
            MakeToast(sMsg)
        End If
    End Sub
    ''' <summary>
    ''' Wyślij via DebugOut, a takze albo pokaż dialog albo zrób toast - z czekaniem
    ''' </summary>
    Public Async Function ToastOrDialogAsync(bDialog As Boolean, sMsg As String) As Task
        DebugOut(sMsg)
        If bDialog Then
            Await DialogBoxAsync(sMsg)
        Else
            MakeToast(sMsg)
        End If
    End Function
#End Region

    ' -- CLIPBOARD ---------------------------------------------

#Region "ClipBoard"
    Public Sub ClipPut(sTxt As String)
        Dim oClipCont As DataTransfer.DataPackage = New DataTransfer.DataPackage
        oClipCont.RequestedOperation = DataTransfer.DataPackageOperation.Copy
        oClipCont.SetText(sTxt)
        DataTransfer.Clipboard.SetContent(oClipCont)
    End Sub

    Public Sub ClipPutHtml(sHtml As String)
        Dim oClipCont As DataTransfer.DataPackage = New DataTransfer.DataPackage
        oClipCont.RequestedOperation = DataTransfer.DataPackageOperation.Copy
        oClipCont.SetHtmlFormat(sHtml)
        DataTransfer.Clipboard.SetContent(oClipCont)
    End Sub

    ''' <summary>
    ''' w razie Catch() zwraca ""
    ''' </summary>
    Public Async Function ClipGetAsync() As Task(Of String)
        Dim oClipCont As DataTransfer.DataPackageView = DataTransfer.Clipboard.GetContent
        Try
            Return Await oClipCont.GetTextAsync()
        Catch ex As Exception
            Return ""
        End Try
    End Function
#End Region


    ' -- Get/Set Settings ---------------------------------------------

#Region "Get/Set settings"

#Region "String"

    Public Function GetSettingsString(oTBox As TextBlock, sName As String, Optional sDefault As String = "") As String
        Dim sTmp As String = GetSettingsString(sName, sDefault)
        oTBox.Text = sTmp
        Return sTmp
    End Function

    Public Function GetSettingsString(oTBox As TextBox, sName As String, Optional sDefault As String = "") As String
        Dim sTmp As String = GetSettingsString(sName, sDefault)
        oTBox.Text = sTmp
        Return sTmp
    End Function


    Public Function GetSettingsString(sName As String, Optional sDefault As String = "") As String
        Dim sTmp As String

        sTmp = sDefault

        With Windows.Storage.ApplicationData.Current
            If .RoamingSettings.Values.ContainsKey(sName) Then
                sTmp = .RoamingSettings.Values(sName).ToString
            End If
            If .LocalSettings.Values.ContainsKey(sName) Then
                sTmp = .LocalSettings.Values(sName).ToString
            End If
        End With

        Return sTmp

    End Function

    Public Function SetSettingsString(sName As String, sValue As String) As Boolean
        Return SetSettingsString(sName, sValue, False)
    End Function

    Public Function SetSettingsString(sName As String, sValue As String, bRoam As Boolean) As Boolean
        Try
            If bRoam Then Windows.Storage.ApplicationData.Current.RoamingSettings.Values(sName) = sValue
            Windows.Storage.ApplicationData.Current.LocalSettings.Values(sName) = sValue
            Return True
        Catch ex As Exception
            ' jesli przepełniony bufor (za długa zmienna) - nie zapisuj dalszych błędów
            Return False
        End Try
    End Function


    Public Sub SetSettingsString(sName As String, sValue As TextBox, bRoam As Boolean)
        SetSettingsString(sName, sValue.Text, bRoam)
    End Sub

    Public Sub SetSettingsString(sName As String, sValue As TextBox)
        SetSettingsString(sName, sValue.Text, False)
    End Sub

    Public Sub SetSettingsString(sValue As TextBox, sName As String)
        SetSettingsString(sName, sValue.Text, False)
    End Sub

#End Region
#Region "Int"
    Public Function GetSettingsInt(sName As String, Optional iDefault As Integer = 0) As Integer
        Dim sTmp As Integer

        sTmp = iDefault

        With Windows.Storage.ApplicationData.Current
            If .RoamingSettings.Values.ContainsKey(sName) Then
                sTmp = CInt(.RoamingSettings.Values(sName).ToString)
            End If
            If .LocalSettings.Values.ContainsKey(sName) Then
                sTmp = CInt(.LocalSettings.Values(sName).ToString)
            End If
        End With

        Return sTmp

    End Function

    Public Sub SetSettingsInt(sName As String, sValue As Integer)
        SetSettingsInt(sName, sValue, False)
    End Sub

    Public Sub SetSettingsInt(sName As String, sValue As Integer, bRoam As Boolean)
        With Windows.Storage.ApplicationData.Current
            If bRoam Then .RoamingSettings.Values(sName) = sValue.ToString
            .LocalSettings.Values(sName) = sValue.ToString
        End With
    End Sub
#End Region
#Region "Long"
    Public Function GetSettingsLong(sName As String, Optional iDefault As Long = 0) As Long
        Dim sTmp As Long

        sTmp = iDefault

        With Windows.Storage.ApplicationData.Current
            If .RoamingSettings.Values.ContainsKey(sName) Then
                sTmp = CLng(.RoamingSettings.Values(sName).ToString)
            End If
            If .LocalSettings.Values.ContainsKey(sName) Then
                sTmp = CLng(.LocalSettings.Values(sName).ToString)
            End If
        End With

        Return sTmp

    End Function

    Public Sub SetSettingsLong(sName As String, sValue As Long)
        SetSettingsLong(sName, sValue, False)
    End Sub

    Public Sub SetSettingsLong(sName As String, sValue As Long, bRoam As Boolean)
        With Windows.Storage.ApplicationData.Current
            If bRoam Then .RoamingSettings.Values(sName) = sValue.ToString
            .LocalSettings.Values(sName) = sValue.ToString
        End With
    End Sub
#End Region
#Region "Bool"
    Public Function GetSettingsBool(sName As String, Optional iDefault As Boolean = False) As Boolean
        Dim sTmp As Boolean

        sTmp = iDefault
        With Windows.Storage.ApplicationData.Current
            If .RoamingSettings.Values.ContainsKey(sName) Then
                sTmp = CBool(.RoamingSettings.Values(sName).ToString)
            End If
            If .LocalSettings.Values.ContainsKey(sName) Then
                sTmp = CBool(.LocalSettings.Values(sName).ToString)
            End If
        End With

        Return sTmp

    End Function

    Public Function GetSettingsBool(oSwitch As ToggleSwitch, Optional iDefault As Boolean = False) As Boolean
        Dim sTmp As Boolean
        sTmp = GetSettingsBool(oSwitch.Name, iDefault)
        oSwitch.IsOn = sTmp
        Return sTmp

    End Function
    Public Function GetSettingsBool(oSwitch As ToggleButton, Optional iDefault As Boolean = False) As Boolean
        Dim sTmp As Boolean
        sTmp = GetSettingsBool(oSwitch.Name, iDefault)
        oSwitch.IsChecked = sTmp
        Return sTmp

    End Function

    Public Function GetSettingsBool(oSwitch As ToggleSwitch, sName As String, Optional iDefault As Boolean = False) As Boolean
        Dim sTmp As Boolean
        sTmp = GetSettingsBool(sName, iDefault)
        oSwitch.IsOn = sTmp
        Return sTmp

    End Function
    Public Function GetSettingsBool(oSwitch As AppBarToggleButton, sName As String, Optional iDefault As Boolean = False) As Boolean
        Dim sTmp As Boolean
        sTmp = GetSettingsBool(sName, iDefault)
        oSwitch.IsChecked = sTmp
        Return sTmp

    End Function

    Public Sub SetSettingsBool(sName As String, sValue As Boolean)
        SetSettingsBool(sName, sValue, False)
    End Sub

    Public Sub SetSettingsBool(sName As String, sValue As Boolean, bRoam As Boolean)
        With Windows.Storage.ApplicationData.Current
            If bRoam Then .RoamingSettings.Values(sName) = sValue.ToString
            .LocalSettings.Values(sName) = sValue.ToString
        End With
    End Sub

    Public Sub SetSettingsBool(sValue As AppBarToggleButton, sName As String, Optional bRoam As Boolean = False)
        SetSettingsBool(sName, sValue.IsChecked, bRoam)
    End Sub
    Public Sub SetSettingsBool(sValue As ToggleSwitch, sName As String, Optional bRoam As Boolean = False)
        SetSettingsBool(sName, sValue.IsOn, bRoam)
    End Sub
    Public Sub SetSettingsBool(sValue As ToggleSwitch, Optional bRoam As Boolean = False)
        SetSettingsBool(sValue.Name, sValue.IsOn, bRoam)
    End Sub
    Public Sub SetSettingsBool(sValue As ToggleButton, Optional bRoam As Boolean = False)
        SetSettingsBool(sValue.Name, sValue.IsChecked, bRoam)
    End Sub

    Public Sub SetSettingsBool(sName As String, sValue As ToggleSwitch, bRoam As Boolean)
        SetSettingsBool(sName, sValue.IsOn, bRoam)
    End Sub

    Public Sub SetSettingsBool(sName As String, sValue As ToggleSwitch)
        SetSettingsBool(sName, sValue.IsOn, False)
    End Sub

#End Region
#Region "Date"
    Public Sub SetSettingsDate(sName As String)
        Dim sValue As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        SetSettingsString(sName, sValue)
    End Sub


#End Region

#End Region


    'Public Function IsFromCmdLine(args As IActivatedEventArgs) As Boolean
    '    ' If Not IsFamilyDesktop() Then Return False
    '    If WinVer() < 16299 Then Return False

    '    ' 1021 = ActivationKind.CommandLineLaunch; ale to jest obecne dopiero od 16299 czyli = Win 1709
    '    If args.Kind <> 1021 Then Return False

    '    Return True
    'End Function


    ' -- Testy sieciowe ---------------------------------------------

#Region "testy sieciowe"

    Public Function IsFamilyMobile() As Boolean
        Return (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily = "Windows.Mobile")
    End Function

    Public Function IsFamilyDesktop() As Boolean
        Return (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily = "Windows.Desktop")
    End Function


    Public Function NetIsIPavailable(bMsg As Boolean) As Boolean
        If GetSettingsBool("offline") Then Return False

        If Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() Then Return True
        If bMsg Then
            DialogBox("ERROR: no IP network available")
        End If
        Return False
    End Function

    Public Function NetIsCellInet() As Boolean
        Return Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile().IsWwanConnectionProfile
    End Function


    Public Function GetHostName() As String
        Dim hostNames As IReadOnlyList(Of Windows.Networking.HostName) =
                Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
        For Each oItem As Windows.Networking.HostName In hostNames
            If oItem.DisplayName.Contains(".local") Then
                Return oItem.DisplayName.Replace(".local", "")
            End If
        Next
        Return ""
    End Function

    ''' <summary>
    ''' Ale to chyba przestało działać...
    ''' </summary>
    Public Function IsThisMoje() As Boolean
        Dim sTmp As String = GetHostName.ToLower
        If sTmp = "home-pkar" Then Return True
        If sTmp = "lumia_pkar" Then Return True
        If sTmp = "kuchnia_pk" Then Return True
        If sTmp = "ppok_pk" Then Return True
        'If sTmp.Contains("pkar") Then Return True
        'If sTmp.EndsWith("_pk") Then Return True
        Return False
    End Function

    ''' <summary>
    ''' w razie Catch() zwraca false
    ''' </summary>
    Public Async Function NetWiFiOffOnAsync() As Task(Of Boolean)

        Try
            ' https://social.msdn.microsoft.com/Forums/ie/en-US/60c4a813-dc66-4af5-bf43-e632c5f85593/uwpbluetoothhow-to-turn-onoff-wifi-bluetooth-programmatically?forum=wpdevelop
            Dim result222 As Windows.Devices.Radios.RadioAccessStatus = Await Windows.Devices.Radios.Radio.RequestAccessAsync()
            If result222 <> Windows.Devices.Radios.RadioAccessStatus.Allowed Then Return False

            Dim radios As IReadOnlyList(Of Windows.Devices.Radios.Radio) = Await Windows.Devices.Radios.Radio.GetRadiosAsync()

            For Each oRadio In radios
                If oRadio.Kind = Windows.Devices.Radios.RadioKind.WiFi Then
                    Dim oStat As Windows.Devices.Radios.RadioAccessStatus =
                    Await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.Off)
                    If oStat <> Windows.Devices.Radios.RadioAccessStatus.Allowed Then Return False
                    Await Task.Delay(3 * 1000)
                    oStat = Await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.On)
                    If oStat <> Windows.Devices.Radios.RadioAccessStatus.Allowed Then Return False
                End If
            Next

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

#Region "Bluetooth"
    ''' <summary>
    ''' Zwraca -1 (no radio), 0 (off), 1 (on), ale gdy bMsg to pokazuje dokładniej błąd (nie włączony, albo nie ma radia Bluetooth) - wedle stringów podanych, które mogą być jednak identyfikatorami w Resources
    ''' </summary>
    Public Async Function NetIsBTavailableAsync(bMsg As Boolean,
                                    Optional bRes As Boolean = False,
                                    Optional sBtDisabled As String = "ERROR: Bluetooth is not enabled",
                                    Optional sNoRadio As String = "ERROR: Bluetooth radio not found") As Task(Of Integer)


        'Dim result222 As Windows.Devices.Radios.RadioAccessStatus = Await Windows.Devices.Radios.Radio.RequestAccessAsync()
        'If result222 <> Windows.Devices.Radios.RadioAccessStatus.Allowed Then Return -1

        Dim oRadios As IReadOnlyList(Of Windows.Devices.Radios.Radio) = Await Windows.Devices.Radios.Radio.GetRadiosAsync()

#If DEBUG Then
        DumpCurrMethod(", count=" & oRadios.Count)
        For Each oRadio As Windows.Devices.Radios.Radio In oRadios
            DumpMessage("NEXT RADIO")
            DumpMessage("name=" & oRadio.Name)
            DumpMessage("kind=" & oRadio.Kind)
            DumpMessage("state=" & oRadio.State)
        Next
#End If

        Dim bHasBT As Boolean = False

        For Each oRadio As Windows.Devices.Radios.Radio In oRadios
            If oRadio.Kind = Windows.Devices.Radios.RadioKind.Bluetooth Then
                If oRadio.State = Windows.Devices.Radios.RadioState.On Then Return 1
                bHasBT = True
            End If
        Next

        If bHasBT Then
            If bMsg Then
                If bRes Then
                    Await DialogBoxResAsync(sBtDisabled)
                Else
                    Await DialogBoxAsync(sBtDisabled)
                End If
            End If
            Return 0
        Else
            If bMsg Then
                If bRes Then
                    Await DialogBoxResAsync(sNoRadio)
                Else
                    Await DialogBoxAsync(sNoRadio)
                End If
            End If
            Return -1
        End If


    End Function

    ''' <summary>
    ''' Zwraca true/false czy State (po call) jest taki jak bOn; wymaga devCap=radios
    ''' </summary>
    Public Async Function NetTrySwitchBTOnAsync(bOn As Boolean) As Task(Of Boolean)
        Dim iCurrState As Integer = Await NetIsBTavailableAsync(False)
        If iCurrState = -1 Then Return False

        ' jeśli nie trzeba przełączać... 
        If bOn AndAlso iCurrState = 1 Then Return True
        If Not bOn AndAlso iCurrState = 0 Then Return True

        ' czy mamy prawo przełączyć? (devCap=radios)
        Dim result222 As Windows.Devices.Radios.RadioAccessStatus = Await Windows.Devices.Radios.Radio.RequestAccessAsync()
        If result222 <> Windows.Devices.Radios.RadioAccessStatus.Allowed Then Return False


        Dim radios As IReadOnlyList(Of Windows.Devices.Radios.Radio) = Await Windows.Devices.Radios.Radio.GetRadiosAsync()

        For Each oRadio In radios
            If oRadio.Kind = Windows.Devices.Radios.RadioKind.Bluetooth Then
                Dim oStat As Windows.Devices.Radios.RadioAccessStatus
                If bOn Then
                    oStat = Await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.On)
                Else
                    oStat = Await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.Off)
                End If
                If oStat <> Windows.Devices.Radios.RadioAccessStatus.Allowed Then Return False
            End If
        Next

        Return True
    End Function

#End Region

#End Region


    ' -- DialogBoxy ---------------------------------------------

#Region "DialogBoxy"

    Public Async Function DialogBoxAsync(sMsg As String) As Task
        Dim oMsg As Windows.UI.Popups.MessageDialog = New Windows.UI.Popups.MessageDialog(sMsg)
        Await oMsg.ShowAsync
    End Function

    Public Sub DialogBox(sMsg As String)
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
        DialogBoxAsync(sMsg)
#Enable Warning BC42358
    End Sub

    Private mbNoResMap As Boolean = False
    Public Function GetLangString(sMsg As String, Optional sDefault As String = "") As String
        If sMsg = "" Then Return ""

        Dim sRet As String = sMsg
        If sDefault <> "" Then sRet = sDefault

        If mbNoResMap Then Return sRet

        Dim oResMap As Windows.ApplicationModel.Resources.ResourceLoader
        Try
            oResMap = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView()
        Catch
            mbNoResMap = True
            Return sRet
        End Try

        Try
            sRet = oResMap.GetString(sMsg)
        Catch
        End Try

        If sRet = "" Then Return sMsg
        Return sRet
    End Function

    Public Async Function DialogBoxResAsync(sMsg As String) As Task
        sMsg = GetLangString(sMsg)
        Await DialogBoxAsync(sMsg)
    End Function
    Public Sub DialogBoxRes(sMsg As String)
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
        DialogBoxResAsync(sMsg)
#Enable Warning BC42358
    End Sub


    Public Async Function DialogBoxErrorAsync(iNr As Integer, sMsg As String) As Task
        Dim sTxt As String = GetLangString("errAnyError")
        sTxt = sTxt & " (" & iNr & ")" & vbCrLf & sMsg
        Await DialogBoxAsync(sTxt)
    End Function
    Public Sub DialogBoxError(iNr As Integer, sMsg As String)
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
        DialogBoxErrorAsync(iNr, sMsg)
#Enable Warning BC42358
    End Sub

    Public Async Function DialogBoxYNAsync(sMsg As String, Optional sYes As String = "Tak", Optional sNo As String = "Nie") As Task(Of Boolean)
        Dim oMsg As Windows.UI.Popups.MessageDialog = New Windows.UI.Popups.MessageDialog(sMsg)
        Dim oYes As Windows.UI.Popups.UICommand = New Windows.UI.Popups.UICommand(sYes)
        Dim oNo As Windows.UI.Popups.UICommand = New Windows.UI.Popups.UICommand(sNo)
        oMsg.Commands.Add(oYes)
        oMsg.Commands.Add(oNo)
        oMsg.DefaultCommandIndex = 1    ' default: No
        oMsg.CancelCommandIndex = 1
        Dim oCmd As Windows.UI.Popups.IUICommand = Await oMsg.ShowAsync
        If oCmd Is Nothing Then Return False
        If oCmd.Label = sYes Then Return True

        Return False
    End Function

    Public Async Function DialogBoxResYNAsync(sMsgResId As String, Optional sYesResId As String = "resDlgYes", Optional sNoResId As String = "resDlgNo") As Task(Of Boolean)
        Dim sMsg, sYes, sNo As String

        With Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView()
            sMsg = .GetString(sMsgResId)
            sYes = .GetString(sYesResId)
            sNo = .GetString(sNoResId)
        End With

        If sMsg = "" Then sMsg = sMsgResId  ' zabezpieczenie na brak string w resource
        If sYes = "" Then sYes = sYesResId
        If sNo = "" Then sNo = sNoResId

        Return Await DialogBoxYNAsync(sMsg, sYes, sNo)
    End Function

    ''' <summary>
    ''' Dla Cancel zwraca ""
    ''' </summary>
    Public Async Function DialogBoxInputResAsync(sMsgResId As String, Optional sDefaultResId As String = "", Optional sYesResId As String = "resDlgContinue", Optional sNoResId As String = "resDlgCancel") As Task(Of String)
        Dim sMsg, sDefault As String
        sDefault = ""

        sMsg = GetLangString(sMsgResId)

        If sDefaultResId <> "" Then sDefault = GetLangString(sDefaultResId)

        Return Await DialogBoxInputDirectAsync(sMsg, sDefault, sYesResId, sNoResId)
    End Function

    Public Async Function DialogBoxInputDirectAsync(sMsg As String, Optional sDefault As String = "", Optional sYesResId As String = "resDlgContinue", Optional sNoResId As String = "resDlgCancel") As Task(Of String)
        Dim sYes, sNo As String

        'sDefault = ""

        'sMsg = GetLangString(sMsgResId)
        sYes = GetLangString(sYesResId)
        sNo = GetLangString(sNoResId)
        'If sDefaultResId <> "" Then sDefault = GetLangString(sDefaultResId)

        ' to już jest w GetLangString - wcześniej było samodzielne sięganie do Resources
        'If sMsg = "" Then sMsg = sMsgResId  ' zabezpieczenie na brak string w resource - 
        'If sYes = "" Then sYes = sYesResId
        'If sNo = "" Then sNo = sNoResId
        'If sDefault = "" Then sDefault = sDefaultResId

        Dim oInputTextBox = New TextBox
        oInputTextBox.AcceptsReturn = False
        oInputTextBox.Text = sDefault
        oInputTextBox.IsSpellCheckEnabled = False

        Dim oDlg As New ContentDialog
        oDlg.Content = oInputTextBox
        oDlg.PrimaryButtonText = sYes
        oDlg.SecondaryButtonText = sNo
        oDlg.Title = sMsg

        Dim oCmd = Await oDlg.ShowAsync
        If oCmd <> ContentDialogResult.Primary Then Return ""

        Return oInputTextBox.Text

    End Function

    Public Sub DialogOrToast(bDialog As Boolean, sMessage As String)
        DebugOut(sMessage)
        If bDialog Then
            DialogBox(sMessage)
        Else
            MakeToast(sMessage)
        End If
    End Sub


#End Region

    ' --- progring ------------------------

#Region "ProgressBar/Ring"
    ' dodałem 25 X 2020

    Private _mProgRing As ProgressRing = Nothing
    Private _mProgBar As ProgressBar = Nothing
    Private _mProgRingShowCnt As Integer = 0

    Public Sub ProgRingInit(bRing As Boolean, bBar As Boolean)

        ' 2020.11.24: dodaję force-off do ProgRing na Init
        _mProgRingShowCnt = 0   ' skoro inicjalizuje, to znaczy że na pewno trzeba wyłączyć

        Dim oFrame As Frame = Window.Current.Content
        Dim oPage As Page = oFrame.Content
        Dim oGrid As Grid = TryCast(oPage.Content, Grid)
        If oGrid Is Nothing Then
            ' skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
            Debug.WriteLine("ProgRingInit wymaga Grid jako podstawy Page")
            Throw New ArgumentException("ProgRingInit wymaga Grid jako podstawy Page")
        End If

        ' *TODO* sprawdz czy istnieje juz taki Control?

        Dim iCols As Integer = 0
        If oGrid.ColumnDefinitions IsNot Nothing Then iCols = oGrid.ColumnDefinitions.Count ' mo¿e byæ 0
        Dim iRows As Integer = 0
        If oGrid.RowDefinitions IsNot Nothing Then iRows = oGrid.RowDefinitions.Count ' mo¿e byæ 0

        If bRing Then
            _mProgRing = New ProgressRing
            _mProgRing.Name = "uiPkAutoProgRing"
            _mProgRing.VerticalAlignment = VerticalAlignment.Center
            _mProgRing.HorizontalAlignment = HorizontalAlignment.Center
            _mProgRing.Visibility = Visibility.Collapsed
            Canvas.SetZIndex(_mProgRing, 10000)
            If iRows > 1 Then
                Grid.SetRow(_mProgRing, 0)
                Grid.SetRowSpan(_mProgRing, iRows)
            End If
            If iCols > 1 Then
                Grid.SetColumn(_mProgRing, 0)
                Grid.SetColumnSpan(_mProgRing, iCols)
            End If
            oGrid.Children.Add(_mProgRing)
        End If

        If bBar Then
            _mProgBar = New ProgressBar
            _mProgBar.Name = "uiPkAutoProgBar"
            _mProgBar.VerticalAlignment = VerticalAlignment.Bottom
            _mProgBar.HorizontalAlignment = HorizontalAlignment.Stretch
            _mProgBar.Visibility = Visibility.Collapsed
            Canvas.SetZIndex(_mProgRing, 10000)
            If iRows > 1 Then Grid.SetRow(_mProgBar, iRows - 1)
            If iCols > 1 Then
                Grid.SetColumn(_mProgBar, 0)
                Grid.SetColumnSpan(_mProgBar, iCols)
            End If
            oGrid.Children.Add(_mProgBar)
        End If

    End Sub

    Public Sub ProgRingShow(bVisible As Boolean, Optional bForce As Boolean = False, Optional dMin As Double = 0, Optional dMax As Double = 100)

        '2021.10.02: tylko gdy jeszcze nie jest pokazywany
        '2021.10.13: gdy min<>max, oraz tylko gdy ma pokazać - inaczej nie zmieniaj zakresu!
        If bVisible And _mProgBar IsNot Nothing And _mProgRingShowCnt < 1 Then
            If dMin <> dMax Then
                Try
                    _mProgBar.Minimum = dMin
                    _mProgBar.Value = dMin
                    _mProgBar.Maximum = dMax
                Catch ex As Exception
                End Try
            End If
        End If

        If bForce Then
            If bVisible Then
                _mProgRingShowCnt = 1
            Else
                _mProgRingShowCnt = 0
            End If
        Else
            If bVisible Then
                _mProgRingShowCnt += 1
            Else
                _mProgRingShowCnt -= 1
            End If
        End If
        Debug.WriteLine("ProgRingShow(" & bVisible & ", " & bForce & "...), current ShowCnt=" & _mProgRingShowCnt)


        Try
            If _mProgRingShowCnt > 0 Then
                DebugOut("ProgRingShow - mam pokazac")
                If _mProgRing IsNot Nothing Then
                    Dim dSize As Double
                    dSize = (Math.Min(TryCast(_mProgRing.Parent, Grid).ActualHeight, TryCast(_mProgRing.Parent, Grid).ActualWidth)) / 2
                    dSize = Math.Max(dSize, 50) ' g³ównie na póŸniej, dla Android
                    _mProgRing.Width = dSize
                    _mProgRing.Height = dSize

                    _mProgRing.Visibility = Visibility.Visible
                    _mProgRing.IsActive = True
                End If
                If _mProgBar IsNot Nothing Then _mProgBar.Visibility = Visibility.Visible
            Else
                DebugOut("ProgRingShow - mam ukryc")
                If _mProgRing IsNot Nothing Then
                    _mProgRing.Visibility = Visibility.Collapsed
                    _mProgRing.IsActive = False
                End If
                If _mProgBar IsNot Nothing Then _mProgBar.Visibility = Visibility.Collapsed
            End If

        Catch ex As Exception
        End Try
    End Sub

    Public Sub ProgRingMaxVal(dMaxValue As Double)
        If _mProgBar Is Nothing Then
            ' skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
            Debug.WriteLine("ProgRing(double) wymaga wczesniej ProgRingInit")
            Throw New ArgumentException("ProgRing(double) wymaga wczesniej ProgRingInit")
        End If

        _mProgBar.Maximum = dMaxValue

    End Sub

    Public Sub ProgRingVal(dValue As Double)
        If _mProgBar Is Nothing Then
            ' skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
            Debug.WriteLine("ProgRing(double) wymaga wczesniej ProgRingInit")
            Throw New ArgumentException("ProgRing(double) wymaga wczesniej ProgRingInit")
        End If

        _mProgBar.Value = dValue

    End Sub

    Public Sub ProgRingInc()
        If _mProgBar Is Nothing Then
            ' skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
            Debug.WriteLine("ProgRing(double) wymaga wczesniej ProgRingInit")
            Throw New ArgumentException("ProgRing(double) wymaga wczesniej ProgRingInit")
        End If

        Dim dVal As Double = _mProgBar.Value + 1
        If dVal > _mProgBar.Maximum Then
            Debug.WriteLine("ProgRingInc na wiecej niz Maximum?")
            _mProgBar.Value = _mProgBar.Maximum
        Else
            _mProgBar.Value = dVal
        End If

    End Sub




#End Region


    ' --- INNE FUNKCJE ------------------------
#Region "Toasty itp"
    Public Sub SetBadgeNo(iInt As Integer)
        ' https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/tiles-and-notifications-badges

        Dim oXmlBadge As Windows.Data.Xml.Dom.XmlDocument
        oXmlBadge = Windows.UI.Notifications.BadgeUpdateManager.GetTemplateContent(
                Windows.UI.Notifications.BadgeTemplateType.BadgeNumber)

        Dim oXmlNum As Windows.Data.Xml.Dom.XmlElement
        oXmlNum = CType(oXmlBadge.SelectSingleNode("/badge"), Windows.Data.Xml.Dom.XmlElement)
        oXmlNum.SetAttribute("value", iInt.ToString)

        Windows.UI.Notifications.BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(
                New Windows.UI.Notifications.BadgeNotification(oXmlBadge))
    End Sub


    Public Function XmlSafeString(sInput As String) As String
        Return New XText(sInput).ToString()

        'Dim sTmp As String
        'sTmp = sInput.Replace("&", "&amp;")
        'sTmp = sTmp.Replace("<", "&lt;")
        'sTmp = sTmp.Replace(">", "&gt;")
        'Return sTmp
    End Function

    Public Function XmlSafeStringQt(sInput As String) As String
        Dim sTmp As String
        sTmp = XmlSafeString(sInput)
        sTmp = sTmp.Replace("""", "&quote;")
        Return sTmp
    End Function

    Public Function ToastAction(sAType As String, sAct As String, sGuid As String, sContent As String) As String
        Dim sTmp As String = sContent
        If sTmp <> "" Then sTmp = GetSettingsString(sTmp, sTmp)

        Dim sTxt As String = "<action " &
            "activationType=""" & sAType & """ " &
            "arguments=""" & sAct & sGuid & """ " &
            "content=""" & sTmp & """/> "
        Return sTxt
    End Function

    ''' <summary>
    ''' dwa kolejne teksty, sMsg oraz sMsg1
    ''' </summary>
    Public Sub MakeToast(sMsg As String, Optional sMsg1 As String = "")
        Dim sXml = "<visual><binding template='ToastGeneric'><text>" & XmlSafeStringQt(sMsg)
        If sMsg1 <> "" Then sXml = sXml & "</text><text>" & XmlSafeStringQt(sMsg1)
        sXml = sXml & "</text></binding></visual>"
        Dim oXml = New Windows.Data.Xml.Dom.XmlDocument
        oXml.LoadXml("<toast>" & sXml & "</toast>")
        Dim oToast = New Windows.UI.Notifications.ToastNotification(oXml)
        Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().Show(oToast)
    End Sub
    Public Sub MakeToast(oDate As DateTime, sMsg As String, Optional sMsg1 As String = "")
        Dim sXml = "<visual><binding template='ToastGeneric'><text>" & XmlSafeStringQt(sMsg)
        If sMsg1 <> "" Then sXml = sXml & "</text><text>" & XmlSafeStringQt(sMsg1)
        sXml = sXml & "</text></binding></visual>"
        Dim oXml = New Windows.Data.Xml.Dom.XmlDocument
        oXml.LoadXml("<toast>" & sXml & "</toast>")
        Try
            ' Dim oToast = New Windows.UI.Notifications.ScheduledToastNotification(oXml, oDate, TimeSpan.FromHours(1), 10)
            Dim oToast = New Windows.UI.Notifications.ScheduledToastNotification(oXml, oDate)
            Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().AddToSchedule(oToast)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub RemoveScheduledToasts()

        Try
            While Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications().Count > 0
                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications().Item(0))
            End While
        Catch ex As Exception
            ' ponoc na desktopm nie dziala
        End Try

    End Sub
#End Region

#Region "WinVer, AppVer"


    Public Function WinVer() As Integer
        'Unknown = 0,
        'Threshold1 = 1507,   // 10240
        'Threshold2 = 1511,   // 10586
        'Anniversary = 1607,  // 14393 Redstone 1
        'Creators = 1703,     // 15063 Redstone 2
        'FallCreators = 1709 // 16299 Redstone 3
        'April = 1803		// 17134
        'October = 1809		// 17763
        '? = 190?		// 18???

        'April  1803, 17134, RS5

        Dim u As ULong = ULong.Parse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion)
        u = (u And &HFFFF0000L) >> 16
        Return u
        'For i As Integer = 5 To 1 Step -1
        '    If Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", i) Then Return i
        'Next

        'Return 0
    End Function

    Public Function GetAppVers() As String

        Return Windows.ApplicationModel.Package.Current.Id.Version.Major & "." &
        Windows.ApplicationModel.Package.Current.Id.Version.Minor & "." &
        Windows.ApplicationModel.Package.Current.Id.Version.Build

    End Function

    ''' <summary>
    ''' jeśli oTB=null, to wtedy dodaje textblock w grid.row=1, grid.colspan=max
    ''' </summary>
    Public Sub GetAppVers(oTB As TextBlock)

        If oTB Is Nothing Then
            Dim oFrame As Frame = Window.Current.Content
            Dim oPage As Page = oFrame.Content
            Dim oGrid As Grid = TryCast(oPage.Content, Grid)
            If oGrid Is Nothing Then
                ' skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
                Debug.WriteLine("GetAppVers(null) wymaga Grid jako podstawy Page")
                Throw New ArgumentException("GetAppVers(null) wymaga Grid jako podstawy Page")
            End If

            Dim iCols As Integer = 0
            If oGrid.ColumnDefinitions IsNot Nothing Then iCols = oGrid.ColumnDefinitions.Count ' może być 0
            Dim iRows As Integer = 0
            If oGrid.RowDefinitions IsNot Nothing Then iRows = oGrid.RowDefinitions.Count ' może być 0

            oTB = New TextBlock
            oTB.Name = "uiPkAutoVersion"
            oTB.VerticalAlignment = VerticalAlignment.Center
            oTB.HorizontalAlignment = HorizontalAlignment.Center
            oTB.FontSize = 10

            If iRows > 2 Then Grid.SetRow(oTB, 1)
            If iCols > 1 Then
                Grid.SetColumn(oTB, 0)
                Grid.SetColumnSpan(oTB, iCols)
            End If
            oGrid.Children.Add(oTB)
        End If

        Dim sTxt As String = GetAppVers()
#If DEBUG Then
        sTxt &= " (debug)"
#End If
        oTB.Text = sTxt
    End Sub


#End Region

#Region "GetWebPage + pomocnicze"


    Private moHttp As Windows.Web.Http.HttpClient = New Windows.Web.Http.HttpClient

    ' 2020.11.29
    Private msAgent As String = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4321.0 Safari/537.36 Edg/88.0.702.0"
    Public Sub HttpPageSetAgent(sAgent As String)
        msAgent = sAgent
    End Sub

    Public Sub HttpPageReset()
        moHttp = Nothing
    End Sub

    ' uwaga: jest wersja bez bMsg!
    Public Async Function HttpPageAsync(sUrl As String, sErrMsg As String, bMsg As Boolean, Optional sData As String = "", Optional bReset As Boolean = False) As Task(Of String)
        Try
            If Not NetIsIPavailable(True) Then Return ""
            If sUrl = "" Then Return ""

            If sUrl.Substring(0, 4) <> "http" Then sUrl = "http://beskid.geo.uj.edu.pl/p/dysk" & sUrl

            If moHttp Is Nothing Or bReset Then
                moHttp = New Windows.Web.Http.HttpClient
                moHttp.DefaultRequestHeaders.UserAgent.TryParseAdd(msAgent)
            End If

            Dim sError = ""
            Dim oResp As Windows.Web.Http.HttpResponseMessage = Nothing

            Try
                If sData <> "" Then
                    Dim oHttpCont = New Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded")
                    oResp = Await moHttp.PostAsync(New Uri(sUrl), oHttpCont)
                Else
                    oResp = Await moHttp.GetAsync(New Uri(sUrl))
                End If
            Catch ex As Exception

                ' 2021.12.10: wykorzystanie  CrashMessageAdd(exception)
                CrashMessageAdd("@HttpPageAsync get/post " & sUrl, ex, True)
                Return ""

                'sError = ex.Message
            End Try

            'If sError <> "" Then
            '    sError = "error " & sError & " Get/Post at " & sErrMsg & " page"
            '    If bMsg Then
            '        Await DialogBoxAsync(sError)
            '    Else
            '        CrashMessageAdd("@HttpPageAsync get/post " & sUrl, sError)
            '    End If
            '    Return ""
            'End If

            If oResp.StatusCode = 303 Or oResp.StatusCode = 302 Or oResp.StatusCode = 301 Then
                ' redirect
                sUrl = oResp.Headers.Location.ToString
                'If sUrl.ToLower.Substring(0, 4) <> "http" Then
                '    sUrl = "https://sympatia.onet.pl/" & sUrl   ' potrzebne przy szukaniu
                'End If

                If sData <> "" Then
                    ' Dim oHttpCont = New HttpStringContent(sData, Text.Encoding.UTF8, "application/x-www-form-urlencoded")
                    Dim oHttpCont = New Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded")
                    oResp = Await moHttp.PostAsync(New Uri(sUrl), oHttpCont)
                Else
                    oResp = Await moHttp.GetAsync(New Uri(sUrl))
                End If
            End If

            If oResp.StatusCode > 290 Then
                sError = "ERROR " & oResp.StatusCode & " getting redirected " & sErrMsg & " page"
                If bMsg Then
                    Await DialogBoxAsync(sError)
                Else
                    CrashMessageAdd("@HttpPageAsync2", sError)
                End If
                Return ""
            End If

            Dim sResp As String = ""
            Try
                sResp = Await oResp.Content.ReadAsStringAsync
            Catch ex As Exception
                sError = ex.Message
            End Try

            If sError <> "" Then
                sError = "error " & sError & " at ReadAsStringAsync " & sErrMsg & " page"
                If bMsg Then
                    Await DialogBoxAsync(sError)
                Else
                    CrashMessageAdd("@HttpPageAsync3", sError)
                End If
                Return ""
            End If

            Return sResp

        Catch ex As Exception
            CrashMessageExit("@HttpPageAsync catch", ex.Message)
        End Try

        Return ""
    End Function

    Public Function RemoveHtmlTags(sHtml As String) As String
        Dim iInd0, iInd1 As Integer

        iInd0 = sHtml.IndexOf("<script")
        If iInd0 > 0 Then
            iInd1 = sHtml.IndexOf("</script>", iInd0)
            If iInd1 > 0 Then
                sHtml = sHtml.Remove(iInd0, iInd1 - iInd0 + 9)
            End If
        End If

        iInd0 = sHtml.IndexOf("<")
        iInd1 = sHtml.IndexOf(">")
        While iInd0 > -1
            If iInd1 > -1 Then
                sHtml = sHtml.Remove(iInd0, iInd1 - iInd0 + 1)
            Else
                sHtml = sHtml.Substring(0, iInd0)
            End If
            sHtml = sHtml.Trim

            iInd0 = sHtml.IndexOf("<")
            iInd1 = sHtml.IndexOf(">")
        End While

        sHtml = sHtml.Replace("&nbsp;", " ")
        sHtml = sHtml.Replace(vbLf, vbCrLf)
        sHtml = sHtml.Replace(vbCrLf & vbCrLf, vbCrLf)
        sHtml = sHtml.Replace(vbCrLf & vbCrLf, vbCrLf)
        sHtml = sHtml.Replace(vbCrLf & vbCrLf, vbCrLf)

        Return sHtml.Trim

    End Function


    Public Sub OpenBrowser(oUri As Uri, bForceEdge As Boolean)
        If bForceEdge Then
            Dim options As Windows.System.LauncherOptions = New Windows.System.LauncherOptions()
            options.TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
#Disable Warning BC42358
            Windows.System.Launcher.LaunchUriAsync(oUri, options)
        Else
            Windows.System.Launcher.LaunchUriAsync(oUri)
#Enable Warning BC42358
        End If

    End Sub

    Public Sub OpenBrowser(sUri As String, Optional bForceEdge As Boolean = False)
        Dim oUri As Uri = New Uri(sUri)
        OpenBrowser(oUri, bForceEdge)
    End Sub

#End Region

    Public Function FileLen2string(iBytes As Long) As String
        If iBytes = 1 Then Return "1 byte"
        If iBytes < 10000 Then Return iBytes & " bytes"
        iBytes = iBytes \ 1024
        If iBytes = 1 Then Return "1 kibibyte"
        If iBytes < 2000 Then Return iBytes & " kibibytes"
        iBytes = iBytes \ 1024
        If iBytes = 1 Then Return "1 mebibyte"
        If iBytes < 2000 Then Return iBytes & " mebibytes"
        iBytes = iBytes \ 1024
        If iBytes = 1 Then Return "1 gibibyte"
        Return iBytes & " gibibytes"
    End Function

    'Public Function UnixTimeToTime(lTime As Long) As DateTime
    '    '1509993360
    '    Dim dtDateTime As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0)
    '    dtDateTime = dtDateTime.AddSeconds(lTime)   ' UTC
    '    ' dtDateTime.Kind = DateTimeKind.Utc
    '    Return dtDateTime.ToLocalTime
    'End Function

#Region "DebugLogFileOut"

    'Private mDebugLogFile As Windows.Storage.StorageFile = Nothing

    'Public Async Function DebugLogFileOutAsync(sMsg As String, bFile As Boolean) As Task
    '    ' jakby mialo wypasc, ze "nie ten wątek"...
    '    If bFile Then
    '        Try
    '            If mDebugLogFile Is Nothing Then
    '                mDebugLogFile = Await Windows.Storage.ApplicationData.Current.LocalCacheFolder.CreateFileAsync("log.txt", Windows.Storage.CreationCollisionOption.OpenIfExists)
    '                If mDebugLogFile Is Nothing Then Return
    '                Await mDebugLogFile.AppendLineAsync(vbCrLf & "===========================================")
    '                Await mDebugLogFile.AppendLineAsync("Start @" & DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") & vbCrLf)
    '            End If

    '            Await mDebugLogFile.AppendLineAsync(sMsg)
    '        Catch ex As Exception

    '        End Try
    '    End If

    '    DebugOut(sMsg)
    'End Function


#End Region

#Region "triggers"
#Region "zwykłe"
    Public Function IsTriggersRegistered(sNameMask As String) As Boolean
        sNameMask = sNameMask.Replace(" ", "").Replace("'", "")

        Try
            For Each oTask As KeyValuePair(Of Guid, Background.IBackgroundTaskRegistration) In Background.BackgroundTaskRegistration.AllTasks
                If oTask.Value.Name.ToLower.Contains(sNameMask.ToLower) Then Return True
            Next
        Catch ex As Exception
            ' np. gdy nie ma permissions, to może być FAIL
        End Try

        Return False
    End Function

    ''' <summary>
    ''' jakikolwiek z prefixem Package.Current.DisplayName
    ''' </summary>
    Public Function IsTriggersRegistered() As Boolean
        Return IsTriggersRegistered(Windows.ApplicationModel.Package.Current.DisplayName)
    End Function

    ''' <summary>
    ''' wszystkie z prefixem Package.Current.DisplayName
    ''' </summary>
    Public Sub UnregisterTriggers()
        UnregisterTriggers(Windows.ApplicationModel.Package.Current.DisplayName)
    End Sub



    Public Sub UnregisterTriggers(sNamePrefix As String)
        sNamePrefix = sNamePrefix.Replace(" ", "").Replace("'", "")

        Try
            For Each oTask As KeyValuePair(Of Guid, Background.IBackgroundTaskRegistration) In Background.BackgroundTaskRegistration.AllTasks
                If String.IsNullOrEmpty(sNamePrefix) OrElse oTask.Value.Name.ToLower.Contains(sNamePrefix.ToLower) Then oTask.Value.Unregister(True)
            Next
        Catch ex As Exception
            ' np. gdy nie ma permissions, to może być FAIL
        End Try

        ' z innego wyszlo, ze RemoveAccess z wnetrza daje Exception
        ' If bAll Then BackgroundExecutionManager.RemoveAccess()

    End Sub

    Public Async Function CanRegisterTriggersAsync() As Task(Of Boolean)

        Dim oBAS As Background.BackgroundAccessStatus
        oBAS = Await Background.BackgroundExecutionManager.RequestAccessAsync()

        If oBAS = Windows.ApplicationModel.Background.BackgroundAccessStatus.AlwaysAllowed Then Return True
        If oBAS = Windows.ApplicationModel.Background.BackgroundAccessStatus.AllowedSubjectToSystemPolicy Then Return True

        Return False

    End Function

    Public Function RegisterTimerTrigger(sName As String, iMinutes As Integer, Optional bOneShot As Boolean = False, Optional oCondition As Windows.ApplicationModel.Background.SystemCondition = Nothing) As Windows.ApplicationModel.Background.BackgroundTaskRegistration

        Try
            Dim builder As Background.BackgroundTaskBuilder = New Background.BackgroundTaskBuilder
            Dim oRet As Background.BackgroundTaskRegistration

            builder.SetTrigger(New Windows.ApplicationModel.Background.TimeTrigger(iMinutes, bOneShot))
            builder.Name = sName
            If oCondition IsNot Nothing Then builder.AddCondition(oCondition)
            oRet = builder.Register()
            Return oRet
        Catch ex As Exception
            ' brak możliwości rejestracji (na przykład)
        End Try

        Return Nothing
    End Function

    Public Function RegisterUserPresentTrigger(Optional sName As String = "", Optional bOneShot As Boolean = False) As Windows.ApplicationModel.Background.BackgroundTaskRegistration

        Try
            Dim builder As Background.BackgroundTaskBuilder = New Background.BackgroundTaskBuilder
            Dim oRet As Background.BackgroundTaskRegistration

            Dim oTrigger As Windows.ApplicationModel.Background.SystemTrigger
            oTrigger = New Background.SystemTrigger(Background.SystemTriggerType.UserPresent, bOneShot)

            builder.SetTrigger(oTrigger)
            builder.Name = sName
            If String.IsNullOrEmpty(sName) Then builder.Name = GetTriggerNamePrefix() & "_userpresent"

            oRet = builder.Register()
            Return oRet
        Catch ex As Exception
            ' brak możliwości rejestracji (na przykład)
        End Try

        Return Nothing
    End Function

    Private Function GetTriggerNamePrefix() As String
        Dim sName As String = Windows.ApplicationModel.Package.Current.DisplayName
        sName = sName.Replace(" ", "").Replace("'", "")
        Return sName
    End Function

    Private Function GetTriggerPolnocnyName() As String
        Return GetTriggerNamePrefix() & "_polnocny"
    End Function


    ''' <summary>
    ''' Tak naprawdę powtarzalny - w OnBackgroundActivated wywołaj IsThisTriggerPolnocny
    ''' </summary>
    Public Async Function DodajTriggerPolnocny() As System.Threading.Tasks.Task
        If Not Await CanRegisterTriggersAsync() Then Return

        Dim oDateNew As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 40, 0)
        If DateTime.Now.Hour > 21 Then oDateNew = oDateNew.AddDays(1)

        Dim iMin As Integer = (oDateNew - DateTime.Now).TotalMinutes
        Dim sName As String = GetTriggerPolnocnyName()

        RegisterTimerTrigger(sName, iMin, False)
    End Function

    ''' <summary>
    ''' para z DodajTriggerPolnocny
    ''' </summary>
    Public Function IsThisTriggerPolnocny(args As Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs) As Boolean

        Dim sName As String = GetTriggerPolnocnyName()
        If args.TaskInstance.Task.Name <> sName Then Return False

        Dim sCurrDate As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        SetSettingsString("lastPolnocnyTry", sCurrDate)

        Dim bRet As Boolean '= False
        Dim oDateNew As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 40, 0)

        If (DateTime.Now.Hour = 23 AndAlso DateTime.Now.Minute > 20) Then
            ' tak, to jest północny o północy
            bRet = True
            oDateNew = oDateNew.AddDays(1)
            SetSettingsString("lastPolnocnyOk", sCurrDate)
        Else
            ' północny, ale nie o północy
            bRet = False
        End If
        Dim iMin As Integer = (oDateNew - DateTime.Now).TotalMinutes

        ' Usuwamy istniejący, robimy nowy
        UnregisterTriggers(sName)
        RegisterTimerTrigger(sName, iMin, False)

        Return bRet

    End Function



    Public Function RegisterServicingCompletedTrigger(sName As String) As Background.BackgroundTaskRegistration

        Try
            Dim builder As Background.BackgroundTaskBuilder = New Background.BackgroundTaskBuilder
            Dim oRet As Windows.ApplicationModel.Background.BackgroundTaskRegistration

            builder.SetTrigger(New Background.SystemTrigger(Background.SystemTriggerType.ServicingComplete, True))
            builder.Name = sName
            oRet = builder.Register()
            Return oRet
        Catch ex As Exception
            ' brak możliwości rejestracji (na przykład)
        End Try

        Return Nothing
    End Function

    Public Function RegisterToastTrigger(sName As String) As Background.BackgroundTaskRegistration

        Try
            Dim builder As Background.BackgroundTaskBuilder = New Background.BackgroundTaskBuilder
            Dim oRet As Windows.ApplicationModel.Background.BackgroundTaskRegistration

            builder.SetTrigger(New Windows.ApplicationModel.Background.ToastNotificationActionTrigger)
            builder.Name = sName
            oRet = builder.Register()
            Return oRet
        Catch ex As Exception
            ' brak możliwości rejestracji (na przykład)
        End Try

        Return Nothing
    End Function

#End Region
#Region "RemoteSystem"

    ''' <summary>
    ''' jeśli na wejściu jest jakaś standardowa komenda, to na wyjściu będzie jej rezultat. Else = ""
    ''' </summary>
    Public Function AppServiceStdCmd(sCommand As String, sLocalCmds As String) As String
        Dim sTmp As String

        If sCommand.StartsWith("debug loglevel") Then
            Dim sRetVal As String = "Previous loglevel: " & GetSettingsInt("debugLogLevel") & vbCrLf
            sCommand = sCommand.Replace("debug loglevel", "").Trim
            Dim iTemp As Integer = 0
            If Not Integer.TryParse(sCommand, iTemp) Then
                Return sRetVal & "Not changed - bad loglevel value"
            End If

            SetSettingsInt("debugLogLevel", iTemp)
            Return sRetVal & "Current loglevel: " & iTemp
        End If


        Select Case sCommand.ToLower()
            Case "ping"
                Return "pong"

            Case "ver"
                Return GetAppVers()
            Case "localdir"
                Return Windows.Storage.ApplicationData.Current.LocalFolder.Path
            Case "appdir"
                Return Windows.ApplicationModel.Package.Current.InstalledLocation.Path
            Case "installeddate"
                Return Windows.ApplicationModel.Package.Current.InstalledDate.ToString("yyyy.MM.dd HH:mm:ss")
            Case "help"
                Return "App specific commands:" & vbCrLf & sLocalCmds


            Case "debug vars"
                Return DumpSettings()
            Case "debug triggers"
                Return DumpTriggers()
            Case "debug toasts"
                Return DumpToasts()
            Case "debug memsize"
                Return Windows.System.MemoryManager.AppMemoryUsage.ToString() & "/" & Windows.System.MemoryManager.AppMemoryUsageLimit.ToString()
            Case "debug rungc"
                sTmp = "Memory usage before Global Collector call: " & Windows.System.MemoryManager.AppMemoryUsage.ToString() & vbCrLf
                GC.Collect()
                GC.WaitForPendingFinalizers()
                sTmp = sTmp & "After: " & Windows.System.MemoryManager.AppMemoryUsage.ToString() & "/" & Windows.System.MemoryManager.AppMemoryUsageLimit.ToString()
                Return sTmp
            Case "debug crashmsg"
                sTmp = GetSettingsString("appFailData", "")
                If sTmp = "" Then sTmp = "No saved crash info"
                Return sTmp
            Case "debug crashmsg clear"
                sTmp = GetSettingsString("appFailData", "")
                If sTmp = "" Then sTmp = "No saved crash info"
                SetSettingsString("appFailData", "")
                Return sTmp

            Case "lib unregistertriggers"
                sTmp = DumpTriggers()
                UnregisterTriggers("") ' // całkiem wszystkie
                Return sTmp
            Case "lib isfamilymobile"
                Return IsFamilyMobile().ToString()
            Case "lib isfamilydesktop"
                Return IsFamilyDesktop().ToString()
            Case "lib netisipavailable"
                Return NetIsIPavailable(False).ToString()
            Case "lib netiscellinet"
                Return NetIsCellInet().ToString()
            Case "lib gethostname"
                Return GetHostName()
            Case "lib isthismoje"
                Return IsThisMoje().ToString()
            Case "lib istriggersregistered"
                Return IsTriggersRegistered().ToString()

            Case "lib pkarmode 1"
                SetSettingsBool("pkarMode", True)
                Return "DONE"
            Case "lib pkarmode 0"
                SetSettingsBool("pkarMode", False)
                Return "DONE"
            Case "lib pkarmode"
                Return GetSettingsBool("pkarMode").ToString()
        End Select

        Return ""  ' oznacza: to nie jest standardowa komenda
    End Function


    Private Function DumpSettings() As String
        Dim sRoam As String = ""
        Try
            For Each oVal In Windows.Storage.ApplicationData.Current.RoamingSettings.Values
                sRoam = sRoam & oVal.Key & vbTab & oVal.Value.ToString() & vbCrLf
            Next
        Catch
        End Try

        Dim sLocal As String = ""
        Try
            For Each oVal In Windows.Storage.ApplicationData.Current.LocalSettings.Values
                sLocal = sLocal & oVal.Key & vbTab & oVal.Value.ToString() & vbCrLf
            Next
        Catch
        End Try

        Dim sRet As String = "Dumping Settings" & vbCrLf
        If sRoam <> "" Then
            sRet = sRet & vbCrLf & "Roaming:" & vbCrLf & sRoam
        Else
            sRet = sRet & "(no roaming settings)" & vbCrLf
        End If

        If sLocal <> "" Then
            sRet = sRet & vbCrLf & "Local:" & vbCrLf & sLocal
        Else
            sRet = sRet & "(no local settings)" & vbCrLf
        End If

        Return sRet
    End Function


    Private Function DumpTriggers() As String
        Dim sRet As String = "Dumping Triggers" & vbCrLf & vbCrLf

        Try
            For Each oTask In Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks
                sRet &= oTask.Value.Name & vbCrLf ' //GetType niestety nie daje rzeczywistego typu
            Next
        Catch
        End Try


        Return sRet
    End Function

    Private Function DumpToasts() As String

        Dim sResult As String = ""
        For Each oToast As Windows.UI.Notifications.ScheduledToastNotification
            In Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications()

            sResult = sResult & oToast.DeliveryTime.ToString("yyyy-MM-dd HH:mm:ss") & vbCrLf
        Next

        If sResult = "" Then
            sResult = "(no toasts scheduled)"
        Else
            sResult = "Toasts scheduled for dates: " & vbCrLf & sResult
        End If

        Return sResult
    End Function

#End Region


#End Region

#Region "DataLog folder support"

    Private Async Function GetSDcardFolderAsync() As Task(Of Windows.Storage.StorageFolder)
        ' uwaga: musi być w Manifest RemoteStorage oraz fileext!

        Dim oRootDir As Windows.Storage.StorageFolder

        Try
            oRootDir = Windows.Storage.KnownFolders.RemovableDevices
        Catch ex As Exception
            Return Nothing ' brak uprawnień, może być także THROW
        End Try

        Try
            Dim oCards As IReadOnlyList(Of Windows.Storage.StorageFolder) = Await oRootDir.GetFoldersAsync()
            Return oCards.FirstOrDefault()
        Catch ex As Exception
            ' nie udało się folderu SD
        End Try

        Return Nothing


    End Function

    Public Async Function GetLogFolderRootAsync(Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFolder)
        Dim oSdCard As Windows.Storage.StorageFolder = Nothing
        Dim oFold As Windows.Storage.StorageFolder

        If IsFamilyMobile() Then
            oSdCard = Await GetSDcardFolderAsync()

            If oSdCard IsNot Nothing Then
                oFold = Await oSdCard.CreateFolderAsync("DataLogs", Windows.Storage.CreationCollisionOption.OpenIfExists)
                If oFold Is Nothing Then Return Nothing

                Dim sAppName As String = Package.Current.DisplayName
                sAppName = sAppName.Replace(" ", "").Replace("'", "")

                oFold = Await oFold.CreateFolderAsync(sAppName, Windows.Storage.CreationCollisionOption.OpenIfExists)
                If oFold Is Nothing Then Return Nothing
            Else
                If Not bUseOwnFolderIfNotSD Then Return Nothing
                oSdCard = Windows.Storage.ApplicationData.Current.LocalFolder
                oFold = Await oSdCard.CreateFolderAsync("DataLogs", Windows.Storage.CreationCollisionOption.OpenIfExists)
                If oFold Is Nothing Then Return Nothing
            End If
        Else
            oSdCard = Windows.Storage.ApplicationData.Current.LocalFolder
            oFold = Await oSdCard.CreateFolderAsync("DataLogs", Windows.Storage.CreationCollisionOption.OpenIfExists)
            If oFold Is Nothing Then Return Nothing
        End If

        Return oFold
    End Function


    Public Async Function GetLogFolderYearAsync(Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFolder)
        Dim oFold As Windows.Storage.StorageFolder = Await GetLogFolderRootAsync(bUseOwnFolderIfNotSD)
        If oFold Is Nothing Then Return Nothing
        oFold = Await oFold.CreateFolderAsync(Date.Now.ToString("yyyy"), Windows.Storage.CreationCollisionOption.OpenIfExists)
        Return oFold
    End Function

    Public Async Function GetLogFolderMonthAsync(Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFolder)
        Dim oFold As Windows.Storage.StorageFolder = Await GetLogFolderYearAsync(bUseOwnFolderIfNotSD)
        If oFold Is Nothing Then Return Nothing

        oFold = Await oFold.CreateFolderAsync(Date.Now.ToString("MM"), Windows.Storage.CreationCollisionOption.OpenIfExists)
        Return oFold

    End Function

    Public Async Function GetLogFileDailyAsync(sBaseName As String, sExtension As String, Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFile)
        Dim oFold As Windows.Storage.StorageFolder = Await GetLogFolderMonthAsync(bUseOwnFolderIfNotSD)
        If oFold Is Nothing Then Return Nothing

        If Not sExtension.StartsWith(".") Then sExtension = "." & sExtension

        Dim sFile As String = sBaseName & " " & Date.Now.ToString("yyyy.MM.dd") & sExtension
        Return Await oFold.CreateFileAsync(sFile, Windows.Storage.CreationCollisionOption.OpenIfExists)
    End Function

    Public Async Function GetLogFileDailyAsync(sFileName As String, Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFile)
        Dim oFold As Windows.Storage.StorageFolder = Await GetLogFolderMonthAsync(bUseOwnFolderIfNotSD)
        If oFold Is Nothing Then Return Nothing

        Return Await oFold.CreateFileAsync(sFileName, Windows.Storage.CreationCollisionOption.OpenIfExists)
    End Function

    Public Async Function GetLogFileMonthlyAsync(sBaseName As String, sExtension As String, Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFile)
        ' 2021.08.20: połączone z tym niżej, tu tylko ustalenie nazwy
        If String.IsNullOrEmpty(sExtension) Then sExtension = ".txt"
        If Not sExtension.StartsWith(".") Then sExtension = "." & sExtension

        Dim sFile As String
        If String.IsNullOrEmpty(sBaseName) Then
            sFile = Date.Now.ToString("yyyy.MM") & sExtension
        Else
            sFile = sBaseName & " " & Date.Now.ToString("yyyy.MM") & sExtension
        End If

        Return Await GetLogFileMonthlyAsync(sFile, bUseOwnFolderIfNotSD)
    End Function

    Public Async Function GetLogFileMonthlyAsync(sFileName As String, Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFile)
        Dim oFold As Windows.Storage.StorageFolder = Await GetLogFolderYearAsync(bUseOwnFolderIfNotSD)
        If oFold Is Nothing Then Return Nothing

        Return Await oFold.CreateFileAsync(sFileName, Windows.Storage.CreationCollisionOption.OpenIfExists)
    End Function

    Public Async Function GetLogFileYearlyAsync(sBaseName As String, sExtension As String, Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFile)
        If String.IsNullOrEmpty(sExtension) Then sExtension = ".txt"
        If Not sExtension.StartsWith(".") Then sExtension = "." & sExtension

        Dim sFile As String
        If String.IsNullOrEmpty(sBaseName) Then
            sFile = Date.Now.ToString("yyyy") & sExtension
        Else
            sFile = sBaseName & " " & Date.Now.ToString("yyyy") & sExtension
        End If

        Return Await GetLogFileYearlyAsync(sFile, bUseOwnFolderIfNotSD)
    End Function

    Public Async Function GetLogFileYearlyAsync(sFileName As String, Optional bUseOwnFolderIfNotSD As Boolean = True) As Task(Of Windows.Storage.StorageFile)
        Dim oFold As Windows.Storage.StorageFolder = Await GetLogFolderRootAsync(bUseOwnFolderIfNotSD)
        If oFold Is Nothing Then Return Nothing

        Return Await oFold.CreateFileAsync(sFileName, Windows.Storage.CreationCollisionOption.OpenIfExists)
    End Function


#End Region


#Region "Bluetooth debugs"

#Region "pomocnicze"

#If False Then
    ' przeniesione jako <extension> do Byte()

    Private Sub DebugPrintArray(aArr As Byte(), iSpaces As Integer)
        ' jakby nic tego nie chciało, to można Sub skasować
        Debug.WriteLine(DebugStringArray(aArr, iSpaces))
    End Sub
    Public Function DebugStringArray(aArr As Byte(), iSpaces As Integer) As String

        Dim sPrefix As String = ""
        For i As Integer = 1 To iSpaces
            sPrefix &= " "
        Next

        If aArr.Length > 6 Then Debug.WriteLine(sPrefix & "length: " & aArr.Length)

        Dim sBytes As String = ""
        Dim sAscii As String = sBytes

        For i As Integer = 0 To Math.Min(aArr.Length - 1, 32) ' bylo oVal

            Dim cBajt As Byte = aArr.ElementAt(i)

            ' hex: tylko 16 bajtow
            If i < 16 Then
                Try
                    sBytes = sBytes & " 0x" & String.Format("{0:X}", cBajt)
                Catch ex As Exception
                    sBytes = sBytes & " ??"
                End Try
            End If

            ' ascii: do 32 bajtow
            If cBajt > 31 And cBajt < 160 Then
                sAscii = sAscii & ChrW(cBajt)
            Else
                sAscii = sAscii & "?"
            End If
        Next

        If aArr.Length - 1 > 16 Then sBytes = sBytes & " ..."
        If aArr.Length - 1 > 32 Then sAscii = sAscii & " ..."

        Return sPrefix & "binary: " & sBytes & vbCrLf &
            sPrefix & "ascii:  " & sAscii
        ' mamy ibuffer
        ' 0x2901 - string utf-8, Characteristic User Description
        ' 0x2902 - Client Characteristic Configuration

    End Function
        Public Function DebugIBuffer(oBuf As Windows.Storage.Streams.IBuffer, iMaxLen As Integer)
        Dim sRet As String = oBuf.Length & ": "
        Dim oArr As Byte() = oBuf.ToArray

        For i As Integer = 0 To Math.Min(oBuf.Length - 1, iMaxLen)
            sRet = sRet & oArr.ElementAt(i).ToString("X2") & " "
        Next

        Return sRet
    End Function
#End If

    ' na podstawie: https://gist.github.com/sam016/4abe921b5a9ee27f67b3686910293026
    'Public Function DebugBTreservedServiceName(oServGuid As Guid)
    '    Dim sServ As String = oServGuid.ToString

    '    Select Case sServ
    '        Case "00001800-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Generic Access"
    '        Case "00001801-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Generic Attribute"
    '        Case "00001802-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Immediate Alert"
    '        Case "00001803-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Link Loss"
    '        Case "00001804-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Tx Power"
    '        Case "00001805-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Current Time Service"
    '        Case "00001806-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Reference Time Update Service"
    '        Case "00001807-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Next DST Change Service"
    '        Case "00001808-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Glucose"
    '        Case "00001809-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Health Thermometer"
    '        Case "0000180a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Device Information"
    '        Case "0000180d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Heart Rate"
    '        Case "0000180e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Phone Alert Status Service"
    '        Case "0000180f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Battery Service"
    '        Case "00001810-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Blood Pressure"
    '        Case "00001811-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Alert Notification Service"
    '        Case "00001812-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Human Interface Device"
    '        Case "00001813-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Scan Parameters"
    '        Case "00001814-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Running Speed and Cadence"
    '        Case "00001815-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Automation IO"
    '        Case "00001816-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Cycling Speed and Cadence"
    '        Case "00001818-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Cycling Power"
    '        Case "00001819-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Location and Navigation"
    '        Case "0000181a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Environmental Sensing"
    '        Case "0000181b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Body Composition"
    '        Case "0000181c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: User Data"
    '        Case "0000181d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Weight Scale"
    '        Case "0000181e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Bond Management Service"
    '        Case "0000181f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Continuous Glucose Monitoring"
    '        Case "00001820-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Internet Protocol Support Service"
    '        Case "00001821-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Indoor Positioning"
    '        Case "00001822-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Pulse Oximeter Service"
    '        Case "00001823-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HTTP Proxy"
    '        Case "00001824-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Transport Discovery"
    '        Case "00001825-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Transfer Service"
    '        Case "00001826-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Fitness Machine"
    '        Case "00001827-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Mesh Provisioning Service"
    '        Case "00001828-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Mesh Proxy Service"
    '        Case "00001829-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Reconnection Configuration"
    '    End Select

    '    Return ""
    'End Function

    'Public Function DebugBTreservedDescrName(oDescrGuid As Guid)
    '    Dim sGuid As String = oDescrGuid.ToString
    '    Select Case sGuid
    '        Case "00002900-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Characteristic Extended Properties"
    '        Case "00002901-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Characteristic User Description"
    '        Case "00002902-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Client Characteristic Configuration"
    '        Case "00002903-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Server Characteristic Configuration"
    '        Case "00002904-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Characteristic Presentation Format"
    '        Case "00002905-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Characteristic Aggregate Format"
    '        Case "00002906-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Valid Range"
    '        Case "00002907-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: External Report Reference"
    '        Case "00002908-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Report Reference"
    '        Case "00002909-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Number of Digitals"
    '        Case "0000290a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Value Trigger Setting"
    '        Case "0000290b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Environmental Sensing Configuration"
    '        Case "0000290c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Environmental Sensing Measurement"
    '        Case "0000290d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Environmental Sensing Trigger Setting"
    '        Case "0000290e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time Trigger Setting"
    '    End Select
    '    Return ""
    'End Function

    'Public Function DebugBTreservedCharName(oCharGuid As Guid) As String
    '    Dim sChar As String = oCharGuid.ToString

    '    Select Case sChar
    '        Case "00002a00-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Device Name"
    '        Case "00002a01-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Appearance"
    '        Case "00002a02-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Peripheral Privacy Flag"
    '        Case "00002a03-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Reconnection Address"
    '        Case "00002a04-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Peripheral Preferred Connection Parameters"
    '        Case "00002a05-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Service Changed"
    '        Case "00002a06-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Alert Level"
    '        Case "00002a07-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Tx Power Level"
    '        Case "00002a08-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Date Time"
    '        Case "00002a09-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Day of Week"
    '        Case "00002a0a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Day Date Time"
    '        Case "00002a0b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Exact Time 100"
    '        Case "00002a0c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Exact Time 256"
    '        Case "00002a0d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: DST Offset"
    '        Case "00002a0e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time Zone"
    '        Case "00002a0f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Local Time Information"
    '        Case "00002a10-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Secondary Time Zone"
    '        Case "00002a11-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time with DST"
    '        Case "00002a12-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time Accuracy"
    '        Case "00002a13-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time Source"
    '        Case "00002a14-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Reference Time Information"
    '        Case "00002a15-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time Broadcast"
    '        Case "00002a16-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time Update Control Point"
    '        Case "00002a17-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Time Update State"
    '        Case "00002a18-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Glucose Measurement"
    '        Case "00002a19-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Battery Level"
    '        Case "00002a1a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Battery Power State"
    '        Case "00002a1b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Battery Level State"
    '        Case "00002a1c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Temperature Measurement"
    '        Case "00002a1d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Temperature Type"
    '        Case "00002a1e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Intermediate Temperature"
    '        Case "00002a1f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Temperature Celsius"
    '        Case "00002a20-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Temperature Fahrenheit"
    '        Case "00002a21-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Measurement Interval"
    '        Case "00002a22-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Boot Keyboard Input Report"
    '        Case "00002a23-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: System ID"
    '        Case "00002a24-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Model Number String"
    '        Case "00002a25-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Serial Number String"
    '        Case "00002a26-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Firmware Revision String"
    '        Case "00002a27-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Hardware Revision String"
    '        Case "00002a28-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Software Revision String"
    '        Case "00002a29-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Manufacturer Name String"
    '        Case "00002a2a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: IEEE 11073-20601 Regulatory Certification Data List"
    '        Case "00002a2b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Current Time"
    '        Case "00002a2c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Magnetic Declination"
    '        Case "00002a2f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Position 2D"
    '        Case "00002a30-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Position 3D"
    '        Case "00002a31-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Scan Refresh"
    '        Case "00002a32-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Boot Keyboard Output Report"
    '        Case "00002a33-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Boot Mouse Input Report"
    '        Case "00002a34-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Glucose Measurement Context"
    '        Case "00002a35-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Blood Pressure Measurement"
    '        Case "00002a36-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Intermediate Cuff Pressure"
    '        Case "00002a37-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Heart Rate Measurement"
    '        Case "00002a38-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Body Sensor Location"
    '        Case "00002a39-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Heart Rate Control Point"
    '        Case "00002a3a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Removable"
    '        Case "00002a3b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Service Required"
    '        Case "00002a3c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Scientific Temperature Celsius"
    '        Case "00002a3d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: String"
    '        Case "00002a3e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Network Availability"
    '        Case "00002a3f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Alert Status"
    '        Case "00002a40-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Ringer Control point"
    '        Case "00002a41-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Ringer Setting"
    '        Case "00002a42-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Alert Category ID Bit Mask"
    '        Case "00002a43-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Alert Category ID"
    '        Case "00002a44-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Alert Notification Control Point"
    '        Case "00002a45-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Unread Alert Status"
    '        Case "00002a46-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: New Alert"
    '        Case "00002a47-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Supported New Alert Category"
    '        Case "00002a48-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Supported Unread Alert Category"
    '        Case "00002a49-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Blood Pressure Feature"
    '        Case "00002a4a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HID Information"
    '        Case "00002a4b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Report Map"
    '        Case "00002a4c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HID Control Point"
    '        Case "00002a4d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Report"
    '        Case "00002a4e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Protocol Mode"
    '        Case "00002a4f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Scan Interval Window"
    '        Case "00002a50-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: PnP ID"
    '        Case "00002a51-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Glucose Feature"
    '        Case "00002a52-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Record Access Control Point"
    '        Case "00002a53-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: RSC Measurement"
    '        Case "00002a54-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: RSC Feature"
    '        Case "00002a55-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: SC Control Point"
    '        Case "00002a56-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Digital"
    '        Case "00002a57-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Digital Output"
    '        Case "00002a58-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Analog"
    '        Case "00002a59-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Analog Output"
    '        Case "00002a5a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Aggregate"
    '        Case "00002a5b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CSC Measurement"
    '        Case "00002a5c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CSC Feature"
    '        Case "00002a5d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Sensor Location"
    '        Case "00002a5e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: PLX Spot-Check Measurement"
    '        Case "00002a5f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: PLX Continuous Measurement Characteristic"
    '        Case "00002a60-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: PLX Features"
    '        Case "00002a62-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Pulse Oximetry Control Point"
    '        Case "00002a63-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Cycling Power Measurement"
    '        Case "00002a64-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Cycling Power Vector"
    '        Case "00002a65-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Cycling Power Feature"
    '        Case "00002a66-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Cycling Power Control Point"
    '        Case "00002a67-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Location and Speed Characteristic"
    '        Case "00002a68-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Navigation"
    '        Case "00002a69-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Position Quality"
    '        Case "00002a6a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: LN Feature"
    '        Case "00002a6b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: LN Control Point"
    '        Case "00002a6c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Elevation"
    '        Case "00002a6d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Pressure"
    '        Case "00002a6e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Temperature"
    '        Case "00002a6f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Humidity"
    '        Case "00002a70-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: True Wind Speed"
    '        Case "00002a71-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: True Wind Direction"
    '        Case "00002a72-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Apparent Wind Speed"
    '        Case "00002a73-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Apparent Wind Direction"
    '        Case "00002a74-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Gust Factor"
    '        Case "00002a75-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Pollen Concentration"
    '        Case "00002a76-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: UV Index"
    '        Case "00002a77-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Irradiance"
    '        Case "00002a78-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Rainfall"
    '        Case "00002a79-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Wind Chill"
    '        Case "00002a7a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Heat Index"
    '        Case "00002a7b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Dew Point"
    '        Case "00002a7d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Descriptor Value Changed"
    '        Case "00002a7e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Aerobic Heart Rate Lower Limit"
    '        Case "00002a7f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Aerobic Threshold"
    '        Case "00002a80-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Age"
    '        Case "00002a81-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Anaerobic Heart Rate Lower Limit"
    '        Case "00002a82-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Anaerobic Heart Rate Upper Limit"
    '        Case "00002a83-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Anaerobic Threshold"
    '        Case "00002a84-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Aerobic Heart Rate Upper Limit"
    '        Case "00002a85-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Date of Birth"
    '        Case "00002a86-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Date of Threshold Assessment"
    '        Case "00002a87-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Email Address"
    '        Case "00002a88-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Fat Burn Heart Rate Lower Limit"
    '        Case "00002a89-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Fat Burn Heart Rate Upper Limit"
    '        Case "00002a8a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: First Name"
    '        Case "00002a8b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Five Zone Heart Rate Limits"
    '        Case "00002a8c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Gender"
    '        Case "00002a8d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Heart Rate Max"
    '        Case "00002a8e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Height"
    '        Case "00002a8f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Hip Circumference"
    '        Case "00002a90-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Last Name"
    '        Case "00002a91-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Maximum Recommended Heart Rate"
    '        Case "00002a92-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Resting Heart Rate"
    '        Case "00002a93-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Sport Type for Aerobic and Anaerobic Thresholds"
    '        Case "00002a94-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Three Zone Heart Rate Limits"
    '        Case "00002a95-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Two Zone Heart Rate Limit"
    '        Case "00002a96-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: VO2 Max"
    '        Case "00002a97-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Waist Circumference"
    '        Case "00002a98-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Weight"
    '        Case "00002a99-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Database Change Increment"
    '        Case "00002a9a-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: User Index"
    '        Case "00002a9b-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Body Composition Feature"
    '        Case "00002a9c-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Body Composition Measurement"
    '        Case "00002a9d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Weight Measurement"
    '        Case "00002a9e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Weight Scale Feature"
    '        Case "00002a9f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: User Control Point"
    '        Case "00002aa0-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Magnetic Flux Density - 2D"
    '        Case "00002aa1-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Magnetic Flux Density - 3D"
    '        Case "00002aa2-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Language"
    '        Case "00002aa3-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Barometric Pressure Trend"
    '        Case "00002aa4-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Bond Management Control Point"
    '        Case "00002aa5-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Bond Management Features"
    '        Case "00002aa6-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Central Address Resolution"
    '        Case "00002aa7-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CGM Measurement"
    '        Case "00002aa8-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CGM Feature"
    '        Case "00002aa9-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CGM Status"
    '        Case "00002aaa-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CGM Session Start Time"
    '        Case "00002aab-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CGM Session Run Time"
    '        Case "00002aac-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: CGM Specific Ops Control Point"
    '        Case "00002aad-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Indoor Positioning Configuration"
    '        Case "00002aae-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Latitude"
    '        Case "00002aaf-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Longitude"
    '        Case "00002ab0-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Local North Coordinate"
    '        Case "00002ab1-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Local East Coordinate"
    '        Case "00002ab2-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Floor Number"
    '        Case "00002ab3-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Altitude"
    '        Case "00002ab4-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Uncertainty"
    '        Case "00002ab5-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Location Name"
    '        Case "00002ab6-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: URI"
    '        Case "00002ab7-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HTTP Headers"
    '        Case "00002ab8-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HTTP Status Code"
    '        Case "00002ab9-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HTTP Entity Body"
    '        Case "00002aba-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HTTP Control Point"
    '        Case "00002abb-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: HTTPS Security"
    '        Case "00002abc-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: TDS Control Point"
    '        Case "00002abd-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: OTS Feature"
    '        Case "00002abe-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Name"
    '        Case "00002abf-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Type"
    '        Case "00002ac0-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Size"
    '        Case "00002ac1-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object First-Created"
    '        Case "00002ac2-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Last-Modified"
    '        Case "00002ac3-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object ID"
    '        Case "00002ac4-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Properties"
    '        Case "00002ac5-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Action Control Point"
    '        Case "00002ac6-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object List Control Point"
    '        Case "00002ac7-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object List Filter"
    '        Case "00002ac8-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Object Changed"
    '        Case "00002ac9-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Resolvable Private Address Only"
    '        Case "00002acc-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Fitness Machine Feature"
    '        Case "00002acd-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Treadmill Data"
    '        Case "00002ace-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Cross Trainer Data"
    '        Case "00002acf-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Step Climber Data"
    '        Case "00002ad0-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Stair Climber Data"
    '        Case "00002ad1-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Rower Data"
    '        Case "00002ad2-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Indoor Bike Data"
    '        Case "00002ad3-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Training Status"
    '        Case "00002ad4-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Supported Speed Range"
    '        Case "00002ad5-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Supported Inclination Range"
    '        Case "00002ad6-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Supported Resistance Level Range"
    '        Case "00002ad7-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Supported Heart Rate Range"
    '        Case "00002ad8-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Supported Power Range"
    '        Case "00002ad9-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Fitness Machine Control Point"
    '        Case "00002ada-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Fitness Machine Status"
    '        Case "00002aed-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Date UTC"
    '        Case "00002b1d-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: RC Feature"
    '        Case "00002b1e-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: RC Settings"
    '        Case "00002b1f-0000-1000-8000-00805f9b34fb"
    '            Return vbTab & "known as: Reconnection Configuration Control Point"
    '    End Select

    '    Return ""
    'End Function



#End Region

    'Public Async Function DebugBTcharacteristicAsync(oChar As Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic) As Task

    '    If oChar Is Nothing Then Return

    '    Dim sProp As String = oChar.CharacteristicProperties.ToDebugString
    '    Dim bCanRead As Boolean = False
    '    If sProp.Contains("[read]") Then bCanRead = True
    '    ' ewentualnie wygaszenie gdy:
    '    'sProp &= "[indicate] "
    '    ' bCanRead = False
    '    '   sProp &= "[notify] "
    '    ' bCanRead = False

    '    Debug.WriteLine("      CharacteristicProperties: " & sProp)

    '    Dim oDescriptors = Await oChar.GetDescriptorsAsync
    '    If oDescriptors Is Nothing Then Return

    '    If oDescriptors.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
    '        Debug.WriteLine("      GetDescriptorsAsync.Status = " & oDescriptors.Status.ToString)
    '        Return
    '    End If


    '    For Each oDescr In oDescriptors.Descriptors
    '        DebugOut(Await oDescr.ToDebugStringAsync)
    '        'Debug.WriteLine("      descriptor: " & oDescr.Uuid.ToString & vbTab & DebugBTreservedDescrName(oDescr.Uuid))
    '        'Dim oRdVal = Await oDescr.ReadValueAsync
    '        'If oRdVal.Status = Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
    '        '    Dim oVal = oRdVal.Value
    '        '    Debug.WriteLine(oVal.ToArray.ToDebugString(8))
    '        'Else
    '        '    Debug.WriteLine("      ReadValueAsync status = " & oRdVal.Status.ToString)
    '        'End If

    '    Next

    '    If bCanRead Then
    '        Dim oRd = Await oChar.ReadValueAsync()
    '        If oRd.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
    '            DebugOut("ReadValueAsync.Status=" & oRd.Status)
    '        Else
    '            Debug.WriteLine("      characteristic data (read):")
    '            Debug.WriteLine(oRd.Value.ToArray.ToDebugString(8))
    '        End If

    '    End If

    'End Function


    'Public Async Function DebugBTserviceAsync(oServ As Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService) As Task

    '    If oServ Is Nothing Then Return

    '    Dim oChars = Await oServ.GetCharacteristicsAsync
    '    If oChars Is Nothing Then Return
    '    If oChars.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
    '        Debug.WriteLine("    GetCharacteristicsAsync.Status = " & oChars.Status.ToString)
    '        Return
    '    End If

    '    For Each oChr In oChars.Characteristics
    '        Debug.WriteLine(vbCrLf & "    characteristic: " & oChr.Uuid.ToString & DebugBTreservedCharName(oChr.Uuid))
    '        DebugOut(Await oChr.ToDebugStringAsync)
    '    Next

    'End Function

    Private mLastBTdeviceDumped As ULong = 0

    'Public Async Function DebugBTdeviceAsync(oDevice As Windows.Devices.Bluetooth.BluetoothLEDevice) As Task

    '    If oDevice Is Nothing Then
    '        DebugOut("DebugBTdevice with NULL parameter!")
    '        Return
    '    End If

    '    If oDevice.BluetoothAddress = mLastBTdeviceDumped Then
    '        DebugOut("DebugBTdevice, but MAC same as previous - skipping")
    '        Return
    '    End If

    '    mLastBTdeviceDumped = oDevice.BluetoothAddress

    '    DebugOut("DebugBTdevice, data dump:")
    '    Debug.WriteLine("Device name: " & oDevice.Name)
    '    Debug.WriteLine("MAC address: " & oDevice.BluetoothAddress.ToHexBytesString)
    '    Debug.WriteLine("Connection status: " & oDevice.ConnectionStatus.ToString)

    '    Dim oDAI = oDevice.DeviceAccessInformation
    '    Debug.WriteLine(vbCrLf & "DeviceAccessInformation:")
    '    Debug.WriteLine("  CurrentStatus: " & oDAI.CurrentStatus.ToString)

    '    Dim oDApperr = oDevice.Appearance
    '    Debug.WriteLine(vbCrLf & "Appearance:")
    '    Debug.WriteLine("  Category: " & oDApperr.Category)
    '    Debug.WriteLine("  Subcategory: " & oDApperr.SubCategory)


    '    Debug.WriteLine("Services: " & oDApperr.SubCategory)

    '    Dim oSrv = Await oDevice.GetGattServicesAsync
    '    If oSrv.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
    '        Debug.WriteLine("  GetGattServicesAsync.Status = " & oSrv.Status.ToString)
    '        Return
    '    End If

    '    For Each oSv In oSrv.Services
    '        Debug.WriteLine(vbCrLf & "  service: " & oSv.Uuid.ToString & vbTab & vbTab & DebugBTreservedServiceName(oSv.Uuid))
    '        Await DebugBTserviceAsync(oSv)
    '    Next

    'End Function

    '''' <summary>
    '''' Wersja BTwatch_Received , używana jako druga przy tworzeniu nowych app - gdy już znany jest adres
    '''' UWAGA: Manifest:Capabilities:Bluetooth musi być włączony!
    '''' </summary>
    'Public Sub DebugBTadvertisement(oAdv As Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisement)

    '    If oAdv Is Nothing Then
    '        DebugOut("Advertisement is Nothing, unmoglich!")
    '        Return
    '    End If

    '    If oAdv.DataSections IsNot Nothing Then
    '        DebugOut("BT advertisement, DataSections count = " & oAdv.DataSections.Count)
    '        For Each oItem As Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementDataSection In oAdv.DataSections
    '            DebugOut(" DataSection: " & oItem.Data.ToDebugString(32))
    '        Next
    '    End If

    '    If oAdv.Flags IsNot Nothing Then DebugOut("Adv.Flags: " & CInt(oAdv.Flags))

    '    DebugOut("Adv local name: " & oAdv.LocalName)

    '    If oAdv.ManufacturerData IsNot Nothing Then
    '        For Each oItem As Windows.Devices.Bluetooth.Advertisement.BluetoothLEManufacturerData In oAdv.ManufacturerData
    '            DebugOut(" ManufacturerData.Company: " & oItem.CompanyId)
    '            DebugOut(" ManufacturerData.Data: " & oItem.Data.ToDebugString(32))
    '        Next
    '    End If

    '    If oAdv.ServiceUuids IsNot Nothing Then
    '        For Each oItem As Guid In oAdv.ServiceUuids
    '            DebugOut(" service " & oItem.ToString)
    '        Next
    '    End If

    'End Sub

    Public Async Function DebugBTGetServChar(uMAC As ULong,
                                      sService As String, sCharacteristic As String) As Task(Of Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic)
        Dim oDev As Windows.Devices.Bluetooth.BluetoothLEDevice
        oDev = Await Windows.Devices.Bluetooth.BluetoothLEDevice.FromBluetoothAddressAsync(uMAC)
        If oDev Is Nothing Then
            DebugOut("DebugBTGetServChar called, cannot get device for uMAC = " & uMAC.ToHexBytesString)
            Return Nothing
        End If

        Return Await DebugBTGetServChar(oDev, sService, sCharacteristic)
    End Function

    Public Async Function DebugBTGetServChar(oDevice As Windows.Devices.Bluetooth.BluetoothLEDevice,
                                      sService As String, sCharacteristic As String) As Task(Of Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic)

        If oDevice Is Nothing Then
            DebugOut("DebugBTGetServChar called with oDevice = null")
            Return Nothing
        End If

        Dim oSrv = Await oDevice.GetGattServicesAsync
        If oSrv.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
            DebugOut("DebugBTGetServChar:GetGattServicesAsync.Status = " & oSrv.Status.ToString)
            Return Nothing
        End If

        Dim oSvc As Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService = Nothing
        For Each oSv In oSrv.Services
            If oSv.Uuid.ToString = sService.ToLower Then
                oSvc = oSv
            End If
        Next
        If oSvc Is Nothing Then
            DebugOut("DebugBTGetServChar: cannot find service " & sService)
            Return Nothing
        End If

        Dim oChars = Await oSvc.GetCharacteristicsAsync
        If oChars Is Nothing Then
            DebugOut("DebugBTGetServChar:GetCharacteristicsAsync = null")
            Return Nothing
        End If

        If oChars.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
            Debug.WriteLine("DebugBTGetServChar:GetCharacteristicsAsync.Status = " & oChars.Status.ToString)
            Return Nothing
        End If

        For Each oChr In oChars.Characteristics
            If oChr.Uuid.ToString = sCharacteristic.ToLower Then Return oChr
        Next

        Return Nothing
    End Function

#End Region

    Public Function MacStringToULong(sStr As String) As ULong
        If String.IsNullOrEmpty(sStr) Then Throw New ArgumentNullException(NameOf(sStr), "MacStringToULong powinno miec parametr")
        If Not sStr.Contains(":") Then Throw New ArgumentException("MacStringToULong - nie ma dwukropków w sStr")

        sStr = sStr.Replace(":", "")
        Dim uLng As ULong = ULong.Parse(sStr, System.Globalization.NumberStyles.HexNumber)

        Return uLng
    End Function


    Public Function GetDomekGeopos(Optional iDecimalDigits As UInt16 = 0) As Windows.Devices.Geolocation.BasicGeoposition
        Dim oTestPoint As Windows.Devices.Geolocation.BasicGeoposition = New Windows.Devices.Geolocation.BasicGeoposition()
        ' 50.01985 
        ' 19.97872

        Select Case iDecimalDigits
            Case 1
                oTestPoint.Latitude = 50.0
                oTestPoint.Longitude = 19.9
            Case 2
                oTestPoint.Latitude = 50.01
                oTestPoint.Longitude = 19.97
            Case 3
                oTestPoint.Latitude = 50.019
                oTestPoint.Longitude = 19.978
            Case 4
                oTestPoint.Latitude = 50.0198
                oTestPoint.Longitude = 19.9787
            Case 5
                oTestPoint.Latitude = 50.01985
                oTestPoint.Longitude = 19.97872
            Case Else
                oTestPoint.Latitude = 50.0
                oTestPoint.Longitude = 20.0
        End Select

        Return oTestPoint
    End Function

    <CodeAnalysis.SuppressMessage("Safety", "UWP001:Platform-specific", Justification:="MinTarget jest OK")>
    Public Async Function IsFullVersion() As Task(Of Boolean)
#If DEBUG Then
        Return True
#End If

        If IsThisMoje() Then Return True

        ' Windows.Services.Store.StoreContext: min 14393 (1607)
        Dim oLicencja = Await Windows.Services.Store.StoreContext.GetDefault().GetAppLicenseAsync()
        If Not oLicencja.IsActive Then Return False ' bez licencji? jakżeż to możliwe?

        If oLicencja.IsTrial Then Return False

        Return True

    End Function


End Module

Module Extensions

    <Extension()>
    Public Function MinMax(ByVal dDouble As Double, dMin As Double, dMax As Double) As Double
        Dim dTmp As Double = dDouble
        dTmp = Math.Max(dMin, dTmp)
        dTmp = Math.Min(dTmp, dMax)
        Return dTmp
    End Function
    <Extension()>
    Public Function MinMax(ByVal iValue As Integer, iMin As Integer, iMax As Integer) As Integer
        Dim dTmp As Integer = iValue
        dTmp = Math.Max(iMin, dTmp)
        dTmp = Math.Min(dTmp, iMax)
        Return dTmp
    End Function
#Region "Read/Write text"

    <Extension()>
    Public Async Function WriteAllTextAsync(ByVal oFile As Windows.Storage.StorageFile, sTxt As String) As Task
        Dim oStream As Stream = Await oFile.OpenStreamForWriteAsync
        Dim oWriter As Windows.Storage.Streams.DataWriter = New Windows.Storage.Streams.DataWriter(oStream.AsOutputStream)
        oWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8
        oWriter.WriteString(sTxt)
        Await oWriter.FlushAsync()
        Await oWriter.StoreAsync()
        oWriter.Dispose()
        'oStream.Flush()
        'oStream.Dispose()
    End Function

    ''' <summary>
    ''' appenduje string, i dodaje vbCrLf
    ''' </summary>
    <Extension()>
    Public Async Function AppendLineAsync(ByVal oFile As Windows.Storage.StorageFile, sTxt As String) As Task
        Await oFile.AppendStringAsync(sTxt & vbCrLf)
    End Function

    ''' <summary>
    ''' appenduje string, nic nie dodając. Zwraca FALSE gdy nie udało się otworzyć pliku.
    ''' </summary>
    <Extension()>
    Public Async Function AppendStringAsync(ByVal oFile As Windows.Storage.StorageFile, sTxt As String) As Task(Of Boolean)

        Dim oStream As Stream = Nothing

        Try
            oStream = Await oFile.OpenStreamForWriteAsync
        Catch ex As Exception
            Return False ' mamy błąd otwarcia pliku
        End Try

        oStream.Seek(0, SeekOrigin.End)
        Dim oWriter As Windows.Storage.Streams.DataWriter = New Windows.Storage.Streams.DataWriter(oStream.AsOutputStream)
        oWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8
        oWriter.WriteString(sTxt)
        Await oWriter.FlushAsync()
        Await oWriter.StoreAsync()
        oWriter.Dispose()

        Return True
        'oStream.Flush()
        'oStream.Dispose()
    End Function

    <Extension()>
    Public Async Function WriteAllTextToFileAsync(ByVal oFold As Windows.Storage.StorageFolder, sFileName As String, sTxt As String, Optional oOption As Windows.Storage.CreationCollisionOption = Windows.Storage.CreationCollisionOption.FailIfExists) As Task
        Dim oFile As Windows.Storage.StorageFile = Await oFold.CreateFileAsync(sFileName, oOption)
        If oFile Is Nothing Then Return

        Await oFile.WriteAllTextAsync(sTxt)
    End Function

#If False Then

    <Extension()>
    Public Async Function SerializeToJSONAsync(ByVal oFold As Windows.Storage.StorageFolder, sFileName As String, mItems As Object) As Task

        Dim sTxt As String = Newtonsoft.Json.JsonConvert.SerializeObject(mItems, Newtonsoft.Json.Formatting.Indented)
        Await oFold.WriteAllTextToFileAsync(sFileName, sTxt, Windows.Storage.CreationCollisionOption.ReplaceExisting)

    End Function
#End If

    <Extension()>
    Public Async Function ReadAllTextAsync(ByVal oFile As Windows.Storage.StorageFile) As Task(Of String)
        ' zamiast File.ReadAllText(oFile.Path)
        Dim oStream As Stream = Await oFile.OpenStreamForReadAsync
        Dim oReader As Windows.Storage.Streams.DataReader = New Windows.Storage.Streams.DataReader(oStream.AsInputStream)
        oReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8
        Dim iSize As Integer = oStream.Length
        Await oReader.LoadAsync(iSize)
        Dim sTxt As String = oReader.ReadString(iSize)
        oReader.Dispose()
        oStream.Dispose()
        Return sTxt
    End Function

    ''' <summary>
    ''' Uwaga: zwraca NULL gdy nie ma pliku, lub tresc pliku
    ''' </summary>
    <Extension()>
    Public Async Function ReadAllTextFromFileAsync(ByVal oFold As Windows.Storage.StorageFolder, sFileName As String) As Task(Of String)
        Dim oFile As Windows.Storage.StorageFile = Await oFold.TryGetItemAsync(sFileName)
        If oFile Is Nothing Then Return Nothing
        Return Await oFile.ReadAllTextAsync
    End Function

#End Region

    <Extension()>
    Public Sub OpenExplorer(ByVal oFold As Windows.Storage.StorageFolder)
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed
        Windows.System.Launcher.LaunchFolderAsync(oFold)
#Enable Warning BC42358
    End Sub

    <Extension()>
    Public Async Function FileExistsAsync(ByVal oFold As Windows.Storage.StorageFolder, sFileName As String) As Task(Of Boolean)
        Try
            Dim oTemp As Windows.Storage.StorageFile = Await oFold.TryGetItemAsync(sFileName)
            If oTemp Is Nothing Then Return False
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    <Extension()>
    Public Function ToHexBytesString(ByVal iVal As ULong) As String
        Dim sTmp As String = String.Format("{0:X}", iVal)
        If sTmp.Length Mod 2 <> 0 Then sTmp = "0" & sTmp

        Dim sRet As String = ""
        Dim bDwukrop As Boolean = False

        While sTmp.Length > 0
            If bDwukrop Then sRet &= ":"
            bDwukrop = True
            sRet = sRet & sTmp.Substring(0, 2)
            sTmp = sTmp.Substring(2)
        End While

        ' gniazdko BT18, daje 15:A6:00:E8:07 (bez 00:)
        ' 71:0A:22:CD:4F:20
        ' 12345678901234567
        If sRet.Length < 17 Then sRet = "00:" & sRet
        If sRet.Length < 17 Then sRet = "00:" & sRet


        Return sRet
    End Function

    <Extension()>
    Public Async Function GetDocumentHtml(ByVal uiWebView As WebView) As Task(Of String)
        Try
            Return Await uiWebView.InvokeScriptAsync("eval", New String() {"document.documentElement.outerHTML;"})
        Catch ex As Exception
            Return "" ' jesli strona jest pusta, jest Exception
        End Try
    End Function

#Region "GPS odleglosci"

    <Extension()>
    Public Function DistanceTo(ByVal oGeocoord0 As Windows.Devices.Geolocation.Geocoordinate, oGeocoord1 As Windows.Devices.Geolocation.Geocoordinate) As Integer
        Return oGeocoord0.Point.Position.DistanceTo(oGeocoord1.Point.Position)
    End Function

    <Extension()>
    Public Function DistanceTo(ByVal oGeopos0 As Windows.Devices.Geolocation.Geoposition, oGeopos1 As Windows.Devices.Geolocation.Geoposition) As Integer
        Return oGeopos0.Coordinate.DistanceTo(oGeopos1.Coordinate)

    End Function

    <Extension()>
    Public Function DistanceTo(ByVal oGeopos0 As Windows.Devices.Geolocation.BasicGeoposition, oGeopos1 As Windows.Devices.Geolocation.BasicGeoposition) As Integer
        ' https://stackoverflow.com/questions/28569246/how-to-get-distance-between-two-locations-in-windows-phone-8-1

        Try
            Dim iRadix As Integer = 6371000
            Dim tLat As Double = (oGeopos1.Latitude - oGeopos0.Latitude) * Math.PI / 180
            Dim tLon As Double = (oGeopos1.Longitude - oGeopos0.Longitude) * Math.PI / 180
            Dim a As Double = Math.Sin(tLat / 2) * Math.Sin(tLat / 2) +
            Math.Cos(Math.PI / 180 * oGeopos0.Latitude) * Math.Cos(Math.PI / 180 * oGeopos1.Latitude) *
            Math.Sin(tLon / 2) * Math.Sin(tLon / 2)
            Dim c As Double = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)))
            Dim d As Double = iRadix * c

            Return d

        Catch ex As Exception
            Return 0    ' nie powinno sie nigdy zdarzyc, ale na wszelki wypadek...
        End Try

    End Function

    <Extension()>
    Public Function DistanceTo(ByVal oGeopos0 As Windows.Devices.Geolocation.BasicGeoposition, dLat As Double, dLong As Double) As Integer

        Dim oPkt As Windows.Devices.Geolocation.BasicGeoposition = New Windows.Devices.Geolocation.BasicGeoposition
        oPkt.Latitude = dLat
        oPkt.Longitude = dLong
        Return oGeopos0.DistanceTo(oPkt)

    End Function

#End Region

    ''' <summary>
    ''' Zwraca od sStart do końca (lub bez zmian, gdy nie ma sStart)
    ''' </summary>
    <Extension()>
    Public Function TrimBefore(ByVal baseString As String, sStart As String) As String
        Dim iInd As Integer = baseString.IndexOf(sStart)
        If iInd < 0 Then Return baseString
        Return baseString.Substring(iInd)
    End Function

    ''' <summary>
    ''' Zwraca od początku do sEnd do końca (lub bez zmian, gdy nie ma sEnd)
    ''' </summary>
    <Extension()>
    Public Function TrimAfter(ByVal baseString As String, sEnd As String) As String
        Dim iInd As Integer = baseString.IndexOf(sEnd)
        If iInd < 0 Then Return baseString
        Return baseString.Substring(0, iInd + sEnd.Length)
    End Function

    ''' <summary>
    ''' Zwraca od sStart do końca (lub bez zmian, gdy nie ma sStart) - szuka od końca
    ''' </summary>
    <Extension()>
    Public Function TrimBeforeLast(ByVal baseString As String, sStart As String) As String
        Dim iInd As Integer = baseString.LastIndexOf(sStart)
        If iInd < 0 Then Return baseString
        Return baseString.Substring(iInd)
    End Function

    ''' <summary>
    ''' Zwraca od początku do sEnd do końca (lub bez zmian, gdy nie ma sEnd) - szuka od końca
    ''' </summary>
    <Extension()>
    Public Function TrimAfterLast(ByVal baseString As String, sEnd As String) As String
        Dim iInd As Integer = baseString.LastIndexOf(sEnd)
        If iInd < 0 Then Return baseString
        Return baseString.Substring(0, iInd + sEnd.Length)
    End Function

    ''' <summary>
    ''' Wycina fragment od sStart do sEnd, jeśli któregoś nie ma - nie tyka
    ''' </summary>
    <Extension()>
    Public Function RemoveBetween(ByVal baseString As String, sStart As String, sEnd As String) As String
        Dim iIndS As Integer = baseString.IndexOf(sStart)
        If iIndS < 0 Then Return baseString
        Dim iIndE As Integer = baseString.IndexOf(sEnd)
        If iIndE < 0 Then Return baseString
        Return baseString.Remove(iIndS, iIndE - iIndS + 1)
    End Function

    <Extension()>
    Public Function ToDebugString(ByVal aArr As Byte(), iSpaces As Integer) As String

        Dim sPrefix As String = ""
        For i As Integer = 1 To iSpaces
            sPrefix &= " "
        Next

        Dim sBytes As String = ""
        Dim sAscii As String = sBytes

        For i As Integer = 0 To Math.Min(aArr.Length - 1, 32) ' bylo oVal

            Dim cBajt As Byte = aArr.ElementAt(i)

            ' hex: tylko 16 bajtow
            If i < 16 Then
                Try
                    sBytes = sBytes & " 0x" & String.Format("{0:X}", cBajt)
                Catch ex As Exception
                    sBytes = sBytes & " ??"
                End Try
            End If

            ' ascii: do 32 bajtow
            If cBajt > 31 And cBajt < 160 Then
                sAscii = sAscii & ChrW(cBajt)
            Else
                sAscii = sAscii & "?"
            End If
        Next

        If aArr.Length - 1 > 16 Then sBytes = sBytes & " ..."
        If aArr.Length - 1 > 32 Then sAscii = sAscii & " ..."

        Dim sRet As String = ""
        If aArr.Length > 6 Then sRet = sPrefix & "length: " & aArr.Length
        sRet = sRet & sPrefix & "binary: " & sBytes & vbCrLf &
            sPrefix & "ascii:  " & sAscii

        Return sRet & vbCrLf

    End Function

    <Extension()>
    Public Function ToDebugString(ByVal oBuf As Windows.Storage.Streams.IBuffer, iMaxLen As Integer) As String
        Dim sRet As String = oBuf.Length & ": "
        Dim oArr As Byte() = oBuf.ToArray

        For i As Integer = 0 To Math.Min(oBuf.Length - 1, iMaxLen)
            sRet = sRet & oArr.ElementAt(i).ToString("X2") & " "
        Next

        Return sRet & vbCrLf
    End Function

    <Extension()>
    Public Function ToStringWithSpaces(ByVal iInteger As Integer) As String
        Dim nfi As System.Globalization.NumberFormatInfo
        nfi = System.Globalization.NumberFormatInfo.InvariantInfo.Clone
        nfi.NumberGroupSeparator = " "
        Return iInteger.ToString(nfi)
    End Function

    <Extension()>
    Public Function ToStringWithSpaces(ByVal iLong As Long) As String
        Dim nfi As System.Globalization.NumberFormatInfo
        nfi = System.Globalization.NumberFormatInfo.InvariantInfo.Clone
        nfi.NumberGroupSeparator = " "
        Return iLong.ToString(nfi)
    End Function

    <Extension()>
    Public Function ToStringDHMS(ByVal iSecs As Long) As String
        Dim sTmp As String = ""
        If iSecs > 60 * 60 * 24 Then
            sTmp = sTmp & iSecs \ (60 * 60 * 24) & "d "
            iSecs = iSecs Mod (60 * 60 * 24)
        End If
        If iSecs > 60 * 60 Then
            sTmp = sTmp & iSecs \ (60 * 60) & ":"
            iSecs = iSecs Mod (60 * 60)
        End If
        If iSecs \ 60 < 10 And sTmp.Length > 1 Then sTmp = sTmp & "0"
        sTmp = sTmp & iSecs \ 60 & ":"
        iSecs = iSecs Mod 60
        If iSecs < 10 And sTmp.Length > 1 Then sTmp = sTmp & "0"
        sTmp &= iSecs.ToString

        Return sTmp
    End Function


#Region "Bluetooth debug strings"

    <Extension()>
    Public Function ToDebugString(ByVal oAdv As Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisement) As String

        If oAdv Is Nothing Then
            Return "ERROR: Advertisement is Nothing, unmoglich!"
        End If

        Dim sRet As String = ""

        If oAdv.DataSections IsNot Nothing Then
            sRet = sRet & "Adverisement, number of data sections: " & oAdv.DataSections.Count & vbCrLf
            For Each oItem As Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementDataSection In oAdv.DataSections
                sRet = sRet & " DataSection: " & oItem.Data.ToDebugString(32)
            Next
        End If

        If oAdv.Flags IsNot Nothing Then sRet = sRet & "Adv.Flags: " & CInt(oAdv.Flags) & vbCrLf

        sRet = sRet & "Adv local name: " & oAdv.LocalName & vbCrLf

        If oAdv.ManufacturerData IsNot Nothing Then
            For Each oItem As Windows.Devices.Bluetooth.Advertisement.BluetoothLEManufacturerData In oAdv.ManufacturerData
                sRet = sRet & " ManufacturerData.Company: " & oItem.CompanyId & vbCrLf
                sRet = sRet & " ManufacturerData.Data: " & oItem.Data.ToDebugString(32) & vbCrLf
            Next
        End If

        If oAdv.ServiceUuids IsNot Nothing Then
            For Each oItem As Guid In oAdv.ServiceUuids
                sRet = sRet & " service " & oItem.ToString & vbCrLf
            Next
        End If

        Return sRet
    End Function

    <Extension()>
    Public Async Function ToDebugStringAsync(ByVal oDescriptor As Windows.Devices.Bluetooth.GenericAttributeProfile.GattDescriptor) As Task(Of String)
        Dim sRet As String

        sRet = "      descriptor: " & oDescriptor.Uuid.ToString & vbTab & oDescriptor.Uuid.AsGattReservedDescriptorName & vbCrLf
        Dim oRdVal = Await oDescriptor.ReadValueAsync
        If oRdVal.Status = Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
            Dim oVal = oRdVal.Value
            sRet = sRet & oVal.ToArray.ToDebugString(8) & vbCrLf
        Else
            sRet = sRet & "      ReadValueAsync status = " & oRdVal.Status.ToString & vbCrLf
        End If
        Return sRet
    End Function

    <Extension()>
    Public Function AsGattReservedDescriptorName(ByVal oGUID As Guid) As String
        Dim sGuid As String = oGUID.ToString
        Select Case sGuid
            Case "00002900-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Characteristic Extended Properties"
            Case "00002901-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Characteristic User Description"
            Case "00002902-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Client Characteristic Configuration"
            Case "00002903-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Server Characteristic Configuration"
            Case "00002904-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Characteristic Presentation Format"
            Case "00002905-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Characteristic Aggregate Format"
            Case "00002906-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Valid Range"
            Case "00002907-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: External Report Reference"
            Case "00002908-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Report Reference"
            Case "00002909-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Number of Digitals"
            Case "0000290a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Value Trigger Setting"
            Case "0000290b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Environmental Sensing Configuration"
            Case "0000290c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Environmental Sensing Measurement"
            Case "0000290d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Environmental Sensing Trigger Setting"
            Case "0000290e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time Trigger Setting"
        End Select
        Return ""
    End Function

    <Extension()>
    Public Function AsGattReservedServiceName(ByVal oGUID As Guid) As String
        Dim sServ As String = oGUID.ToString

        Select Case sServ
            Case "00001800-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Generic Access"
            Case "00001801-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Generic Attribute"
            Case "00001802-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Immediate Alert"
            Case "00001803-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Link Loss"
            Case "00001804-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Tx Power"
            Case "00001805-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Current Time Service"
            Case "00001806-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Reference Time Update Service"
            Case "00001807-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Next DST Change Service"
            Case "00001808-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Glucose"
            Case "00001809-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Health Thermometer"
            Case "0000180a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Device Information"
            Case "0000180d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Heart Rate"
            Case "0000180e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Phone Alert Status Service"
            Case "0000180f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Battery Service"
            Case "00001810-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Blood Pressure"
            Case "00001811-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Alert Notification Service"
            Case "00001812-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Human Interface Device"
            Case "00001813-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Scan Parameters"
            Case "00001814-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Running Speed and Cadence"
            Case "00001815-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Automation IO"
            Case "00001816-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Cycling Speed and Cadence"
            Case "00001818-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Cycling Power"
            Case "00001819-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Location and Navigation"
            Case "0000181a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Environmental Sensing"
            Case "0000181b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Body Composition"
            Case "0000181c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: User Data"
            Case "0000181d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Weight Scale"
            Case "0000181e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Bond Management Service"
            Case "0000181f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Continuous Glucose Monitoring"
            Case "00001820-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Internet Protocol Support Service"
            Case "00001821-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Indoor Positioning"
            Case "00001822-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Pulse Oximeter Service"
            Case "00001823-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HTTP Proxy"
            Case "00001824-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Transport Discovery"
            Case "00001825-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Transfer Service"
            Case "00001826-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Fitness Machine"
            Case "00001827-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Mesh Provisioning Service"
            Case "00001828-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Mesh Proxy Service"
            Case "00001829-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Reconnection Configuration"
        End Select

        Return ""
    End Function


    <Extension()>
    Public Function AsGattReservedCharacteristicName(ByVal oGUID As Guid) As String
        Dim sChar As String = oGUID.ToString

        Select Case sChar
            Case "00002a00-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Device Name"
            Case "00002a01-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Appearance"
            Case "00002a02-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Peripheral Privacy Flag"
            Case "00002a03-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Reconnection Address"
            Case "00002a04-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Peripheral Preferred Connection Parameters"
            Case "00002a05-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Service Changed"
            Case "00002a06-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Alert Level"
            Case "00002a07-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Tx Power Level"
            Case "00002a08-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Date Time"
            Case "00002a09-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Day of Week"
            Case "00002a0a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Day Date Time"
            Case "00002a0b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Exact Time 100"
            Case "00002a0c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Exact Time 256"
            Case "00002a0d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: DST Offset"
            Case "00002a0e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time Zone"
            Case "00002a0f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Local Time Information"
            Case "00002a10-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Secondary Time Zone"
            Case "00002a11-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time with DST"
            Case "00002a12-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time Accuracy"
            Case "00002a13-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time Source"
            Case "00002a14-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Reference Time Information"
            Case "00002a15-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time Broadcast"
            Case "00002a16-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time Update Control Point"
            Case "00002a17-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Time Update State"
            Case "00002a18-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Glucose Measurement"
            Case "00002a19-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Battery Level"
            Case "00002a1a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Battery Power State"
            Case "00002a1b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Battery Level State"
            Case "00002a1c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Temperature Measurement"
            Case "00002a1d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Temperature Type"
            Case "00002a1e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Intermediate Temperature"
            Case "00002a1f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Temperature Celsius"
            Case "00002a20-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Temperature Fahrenheit"
            Case "00002a21-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Measurement Interval"
            Case "00002a22-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Boot Keyboard Input Report"
            Case "00002a23-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: System ID"
            Case "00002a24-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Model Number String"
            Case "00002a25-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Serial Number String"
            Case "00002a26-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Firmware Revision String"
            Case "00002a27-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Hardware Revision String"
            Case "00002a28-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Software Revision String"
            Case "00002a29-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Manufacturer Name String"
            Case "00002a2a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: IEEE 11073-20601 Regulatory Certification Data List"
            Case "00002a2b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Current Time"
            Case "00002a2c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Magnetic Declination"
            Case "00002a2f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Position 2D"
            Case "00002a30-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Position 3D"
            Case "00002a31-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Scan Refresh"
            Case "00002a32-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Boot Keyboard Output Report"
            Case "00002a33-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Boot Mouse Input Report"
            Case "00002a34-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Glucose Measurement Context"
            Case "00002a35-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Blood Pressure Measurement"
            Case "00002a36-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Intermediate Cuff Pressure"
            Case "00002a37-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Heart Rate Measurement"
            Case "00002a38-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Body Sensor Location"
            Case "00002a39-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Heart Rate Control Point"
            Case "00002a3a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Removable"
            Case "00002a3b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Service Required"
            Case "00002a3c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Scientific Temperature Celsius"
            Case "00002a3d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: String"
            Case "00002a3e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Network Availability"
            Case "00002a3f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Alert Status"
            Case "00002a40-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Ringer Control point"
            Case "00002a41-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Ringer Setting"
            Case "00002a42-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Alert Category ID Bit Mask"
            Case "00002a43-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Alert Category ID"
            Case "00002a44-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Alert Notification Control Point"
            Case "00002a45-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Unread Alert Status"
            Case "00002a46-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: New Alert"
            Case "00002a47-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Supported New Alert Category"
            Case "00002a48-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Supported Unread Alert Category"
            Case "00002a49-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Blood Pressure Feature"
            Case "00002a4a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HID Information"
            Case "00002a4b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Report Map"
            Case "00002a4c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HID Control Point"
            Case "00002a4d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Report"
            Case "00002a4e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Protocol Mode"
            Case "00002a4f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Scan Interval Window"
            Case "00002a50-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: PnP ID"
            Case "00002a51-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Glucose Feature"
            Case "00002a52-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Record Access Control Point"
            Case "00002a53-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: RSC Measurement"
            Case "00002a54-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: RSC Feature"
            Case "00002a55-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: SC Control Point"
            Case "00002a56-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Digital"
            Case "00002a57-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Digital Output"
            Case "00002a58-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Analog"
            Case "00002a59-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Analog Output"
            Case "00002a5a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Aggregate"
            Case "00002a5b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CSC Measurement"
            Case "00002a5c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CSC Feature"
            Case "00002a5d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Sensor Location"
            Case "00002a5e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: PLX Spot-Check Measurement"
            Case "00002a5f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: PLX Continuous Measurement Characteristic"
            Case "00002a60-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: PLX Features"
            Case "00002a62-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Pulse Oximetry Control Point"
            Case "00002a63-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Cycling Power Measurement"
            Case "00002a64-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Cycling Power Vector"
            Case "00002a65-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Cycling Power Feature"
            Case "00002a66-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Cycling Power Control Point"
            Case "00002a67-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Location and Speed Characteristic"
            Case "00002a68-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Navigation"
            Case "00002a69-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Position Quality"
            Case "00002a6a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: LN Feature"
            Case "00002a6b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: LN Control Point"
            Case "00002a6c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Elevation"
            Case "00002a6d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Pressure"
            Case "00002a6e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Temperature"
            Case "00002a6f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Humidity"
            Case "00002a70-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: True Wind Speed"
            Case "00002a71-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: True Wind Direction"
            Case "00002a72-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Apparent Wind Speed"
            Case "00002a73-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Apparent Wind Direction"
            Case "00002a74-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Gust Factor"
            Case "00002a75-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Pollen Concentration"
            Case "00002a76-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: UV Index"
            Case "00002a77-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Irradiance"
            Case "00002a78-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Rainfall"
            Case "00002a79-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Wind Chill"
            Case "00002a7a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Heat Index"
            Case "00002a7b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Dew Point"
            Case "00002a7d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Descriptor Value Changed"
            Case "00002a7e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Aerobic Heart Rate Lower Limit"
            Case "00002a7f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Aerobic Threshold"
            Case "00002a80-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Age"
            Case "00002a81-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Anaerobic Heart Rate Lower Limit"
            Case "00002a82-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Anaerobic Heart Rate Upper Limit"
            Case "00002a83-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Anaerobic Threshold"
            Case "00002a84-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Aerobic Heart Rate Upper Limit"
            Case "00002a85-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Date of Birth"
            Case "00002a86-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Date of Threshold Assessment"
            Case "00002a87-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Email Address"
            Case "00002a88-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Fat Burn Heart Rate Lower Limit"
            Case "00002a89-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Fat Burn Heart Rate Upper Limit"
            Case "00002a8a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: First Name"
            Case "00002a8b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Five Zone Heart Rate Limits"
            Case "00002a8c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Gender"
            Case "00002a8d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Heart Rate Max"
            Case "00002a8e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Height"
            Case "00002a8f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Hip Circumference"
            Case "00002a90-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Last Name"
            Case "00002a91-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Maximum Recommended Heart Rate"
            Case "00002a92-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Resting Heart Rate"
            Case "00002a93-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Sport Type for Aerobic and Anaerobic Thresholds"
            Case "00002a94-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Three Zone Heart Rate Limits"
            Case "00002a95-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Two Zone Heart Rate Limit"
            Case "00002a96-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: VO2 Max"
            Case "00002a97-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Waist Circumference"
            Case "00002a98-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Weight"
            Case "00002a99-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Database Change Increment"
            Case "00002a9a-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: User Index"
            Case "00002a9b-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Body Composition Feature"
            Case "00002a9c-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Body Composition Measurement"
            Case "00002a9d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Weight Measurement"
            Case "00002a9e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Weight Scale Feature"
            Case "00002a9f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: User Control Point"
            Case "00002aa0-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Magnetic Flux Density - 2D"
            Case "00002aa1-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Magnetic Flux Density - 3D"
            Case "00002aa2-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Language"
            Case "00002aa3-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Barometric Pressure Trend"
            Case "00002aa4-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Bond Management Control Point"
            Case "00002aa5-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Bond Management Features"
            Case "00002aa6-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Central Address Resolution"
            Case "00002aa7-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CGM Measurement"
            Case "00002aa8-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CGM Feature"
            Case "00002aa9-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CGM Status"
            Case "00002aaa-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CGM Session Start Time"
            Case "00002aab-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CGM Session Run Time"
            Case "00002aac-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: CGM Specific Ops Control Point"
            Case "00002aad-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Indoor Positioning Configuration"
            Case "00002aae-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Latitude"
            Case "00002aaf-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Longitude"
            Case "00002ab0-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Local North Coordinate"
            Case "00002ab1-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Local East Coordinate"
            Case "00002ab2-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Floor Number"
            Case "00002ab3-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Altitude"
            Case "00002ab4-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Uncertainty"
            Case "00002ab5-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Location Name"
            Case "00002ab6-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: URI"
            Case "00002ab7-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HTTP Headers"
            Case "00002ab8-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HTTP Status Code"
            Case "00002ab9-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HTTP Entity Body"
            Case "00002aba-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HTTP Control Point"
            Case "00002abb-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: HTTPS Security"
            Case "00002abc-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: TDS Control Point"
            Case "00002abd-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: OTS Feature"
            Case "00002abe-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Name"
            Case "00002abf-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Type"
            Case "00002ac0-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Size"
            Case "00002ac1-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object First-Created"
            Case "00002ac2-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Last-Modified"
            Case "00002ac3-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object ID"
            Case "00002ac4-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Properties"
            Case "00002ac5-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Action Control Point"
            Case "00002ac6-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object List Control Point"
            Case "00002ac7-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object List Filter"
            Case "00002ac8-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Object Changed"
            Case "00002ac9-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Resolvable Private Address Only"
            Case "00002acc-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Fitness Machine Feature"
            Case "00002acd-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Treadmill Data"
            Case "00002ace-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Cross Trainer Data"
            Case "00002acf-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Step Climber Data"
            Case "00002ad0-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Stair Climber Data"
            Case "00002ad1-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Rower Data"
            Case "00002ad2-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Indoor Bike Data"
            Case "00002ad3-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Training Status"
            Case "00002ad4-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Supported Speed Range"
            Case "00002ad5-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Supported Inclination Range"
            Case "00002ad6-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Supported Resistance Level Range"
            Case "00002ad7-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Supported Heart Rate Range"
            Case "00002ad8-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Supported Power Range"
            Case "00002ad9-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Fitness Machine Control Point"
            Case "00002ada-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Fitness Machine Status"
            Case "00002aed-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Date UTC"
            Case "00002b1d-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: RC Feature"
            Case "00002b1e-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: RC Settings"
            Case "00002b1f-0000-1000-8000-00805f9b34fb"
                Return vbTab & "known as: Reconnection Configuration Control Point"
        End Select

        Return ""
    End Function


    <Extension()>
    Public Function ToDebugString(ByVal oProp As Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties) As String

        Dim sRet As String = "      CharacteristicProperties: "

        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Read) Then
            sRet &= "[read] "
        End If

        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.AuthenticatedSignedWrites) Then
            sRet &= "[AuthenticatedSignedWrites] "
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Broadcast) Then
            sRet &= "[broadcast] "
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Indicate) Then
            sRet &= "[indicate] "
            ' bCanRead = False
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.None) Then
            sRet &= "[NONE] "
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Notify) Then
            sRet &= "[notify] "
            ' bCanRead = False
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.ReliableWrites) Then
            sRet &= "[reliableWrite] "
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Write) Then
            sRet &= "[write] "
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.WritableAuxiliaries) Then
            sRet &= "[WritableAuxiliaries] "
        End If
        If oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.WriteWithoutResponse) Then
            sRet &= "[writeNoResponse] "
        End If

        Return sRet
    End Function

    <Extension()>
    Public Async Function ToDebugStringAsync(ByVal oChar As Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic) As Task(Of String)

        Dim sRet As String = "      CharacteristicProperties: " & oChar.CharacteristicProperties.ToDebugString & vbCrLf
        Dim bCanRead As Boolean = False
        If sRet.Contains("[read]") Then bCanRead = True
        ' ewentualnie wygaszenie gdy:
        'sProp &= "[indicate] "
        ' bCanRead = False
        '   sProp &= "[notify] "
        ' bCanRead = False


        Dim oDescriptors = Await oChar.GetDescriptorsAsync
        If oDescriptors Is Nothing Then Return sRet

        If oDescriptors.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
            sRet = sRet & "      GetDescriptorsAsync.Status = " & oDescriptors.Status.ToString & vbCrLf
            Return sRet
        End If


        For Each oDescr In oDescriptors.Descriptors
            sRet = sRet & Await oDescr.ToDebugStringAsync & vbCrLf
        Next

        If bCanRead Then
            Dim oRd = Await oChar.ReadValueAsync()
            If oRd.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
                sRet = sRet & "ReadValueAsync.Status=" & oRd.Status & vbCrLf
            Else
                sRet = sRet & "      characteristic data (read):" & vbCrLf
                sRet = sRet & oRd.Value.ToArray.ToDebugString(8) & vbCrLf
            End If

        End If

        Return sRet
    End Function

    <Extension()>
    Public Async Function ToDebusStringAsync(ByVal oServ As Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService) As Task(Of String)

        If oServ Is Nothing Then Return ""

        Dim oChars = Await oServ.GetCharacteristicsAsync
        If oChars Is Nothing Then Return ""
        If oChars.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
            Return "    GetCharacteristicsAsync.Status = " & oChars.Status.ToString
        End If

        Dim sRet As String = ""
        For Each oChr In oChars.Characteristics
            sRet = sRet & vbCrLf & "    characteristic: " & oChr.Uuid.ToString & oChr.Uuid.AsGattReservedCharacteristicName & vbCrLf
            sRet = sRet & Await oChr.ToDebugStringAsync & vbCrLf
        Next

        Return sRet
    End Function

    <Extension()>
    Public Async Function ToDebusStringAsync(ByVal oDevice As Windows.Devices.Bluetooth.BluetoothLEDevice) As Task(Of String)

        'If oDevice.BluetoothAddress = mLastBTdeviceDumped Then
        '    DebugOut("DebugBTdevice, but MAC same as previous - skipping")
        '    Return
        'End If
        'mLastBTdeviceDumped = oDevice.BluetoothAddress

        Dim sRet As String = ""

        sRet = sRet & "DebugBTdevice, data dump:" & vbCrLf
        sRet = sRet & "Device name: " & oDevice.Name & vbCrLf
        sRet = sRet & "MAC address: " & oDevice.BluetoothAddress.ToHexBytesString & vbCrLf
        sRet = sRet & "Connection status: " & oDevice.ConnectionStatus.ToString & vbCrLf

        Dim oDAI = oDevice.DeviceAccessInformation
        sRet = sRet & vbCrLf & "DeviceAccessInformation:" & vbCrLf
        sRet = sRet & "  CurrentStatus: " & oDAI.CurrentStatus.ToString & vbCrLf

        Dim oDApperr = oDevice.Appearance
        sRet = sRet & vbCrLf & "Appearance:" & vbCrLf
        sRet = sRet & "  Category: " & oDApperr.Category & vbCrLf
        sRet = sRet & "  Subcategory: " & oDApperr.SubCategory & vbCrLf

        sRet = sRet & "Services: " & oDApperr.SubCategory & vbCrLf

        Dim oSrv = Await oDevice.GetGattServicesAsync
        If oSrv.Status <> Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success Then
            sRet = sRet & "  GetGattServicesAsync.Status = " & oSrv.Status.ToString & vbCrLf
            Return sRet
        End If

        For Each oSv In oSrv.Services
            sRet = sRet & vbCrLf & "  service: " & oSv.Uuid.ToString & vbTab & vbTab & oSv.Uuid.AsGattReservedServiceName & vbCrLf
            sRet = sRet & Await oSv.ToDebusStringAsync & vbCrLf
        Next

        Return sRet

    End Function


#End Region
End Module
