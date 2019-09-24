' 
'   STORE: 1.7.1.0, 2018.09.03


Imports Windows.Data.Xml.Dom
Imports System.Windows

Public NotInheritable Class MainPage
    Inherits Page

    Dim mEvents As String = ""
    Dim mHolid As String = ""
    Dim mBirths As String = ""
    Dim mDeaths As String = ""
    Dim mObceMiesiace As String = ""
    ' zmienne w Setup dostepne
    Dim mDate As DateTimeOffset
    'Dim mObceJezyki As String = "pl de fr es ru"
    Dim mPreferredLang As String = "pl"
    Dim mCurrLang As String = ""
    Dim mCurrPart As String = ""

    Private Sub bSetup_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Setup))
    End Sub

    Private Sub bInfo_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(InfoAbout))
    End Sub
    ' nazwa, odpowiednio Case dla Wikipedii - gdy poza zakresem, zwroci styczen
    Public Shared Function MonthNo2EnName(iMonth As Integer)
        Select Case iMonth
            Case 1
                MonthNo2EnName = "January"
            Case 2
                MonthNo2EnName = "February"
            Case 3
                MonthNo2EnName = "March"
            Case 4
                MonthNo2EnName = "April"
            Case 5
                MonthNo2EnName = "May"
            Case 6
                MonthNo2EnName = "June"
            Case 7
                MonthNo2EnName = "July"
            Case 8
                MonthNo2EnName = "August"
            Case 9
                MonthNo2EnName = "September"
            Case 10
                MonthNo2EnName = "October"
            Case 11
                MonthNo2EnName = "November"
            Case 12
                MonthNo2EnName = "December"
            Case Else
                MonthNo2EnName = "January"
        End Select

    End Function
    Private Function MonthNo2PlName(iMonth As Integer)
        Select Case iMonth
            Case 1
                MonthNo2PlName = "stycznia"
            Case 2
                MonthNo2PlName = "lutego"
            Case 3
                MonthNo2PlName = "marca"
            Case 4
                MonthNo2PlName = "kwietnia"
            Case 5
                MonthNo2PlName = "maja"
            Case 6
                MonthNo2PlName = "czerwca"
            Case 7
                MonthNo2PlName = "lipca"
            Case 8
                MonthNo2PlName = "sierpnia"
            Case 9
                MonthNo2PlName = "września"
            Case 10
                MonthNo2PlName = "października"
            Case 11
                MonthNo2PlName = "listopada"
            Case 12
                MonthNo2PlName = "grudnia"
            Case Else
                MonthNo2PlName = "stycznia"
        End Select

    End Function

    ''' <summary>
    ''' Wycięcie z _sPage_ tekstu od _sFrom_ do początku następnego h2
    ''' </summary>
    Private Function WyciagnijDane(sPage As String, sFrom As String) As String
        WyciagnijDane = ""

        mCurrPart = sFrom

        Dim iInd As Integer
        Dim sTxt As String

        iInd = sPage.IndexOf(sFrom)
        If iInd < 10 Then Exit Function
        sTxt = sPage.Substring(iInd)
        iInd = sTxt.IndexOf("<ul>")
        If iInd < 10 Then Exit Function
        sTxt = sTxt.Substring(iInd)
        iInd = sTxt.IndexOf("<h2")
        If iInd < 10 Then Exit Function
        sTxt = sTxt.Substring(0, iInd)
        iInd = sTxt.LastIndexOf("</ul")
        If iInd < 10 Then Exit Function
        WyciagnijDane = sTxt.Substring(0, iInd + 5)
    End Function

    Private Function WytnijObrazkiDE(sPage As String) As String
        ' wikipedia.de ma obrazki wklejone - ale przerywaja one <ul>, wiec latwo wyrzucic
        Dim iInd As Integer
        Dim sTmp As String = ""

        ' pozbywam sie potrzebnego </ul>
        iInd = sPage.LastIndexOf("</ul>")
        If iInd > 0 Then sPage = sPage.Substring(0, iInd - 1)

        iInd = sPage.IndexOf("</ul>")
        'ale jesli "</ul></li> to jest ok... - wersja FR tej funkcji!
        While iInd > 0
            sTmp = sTmp & sPage.Substring(0, iInd - 1)
            sPage = sPage.Substring(iInd)
            iInd = sPage.IndexOf("<ul>")
            If iInd > 0 Then
                sPage = sPage.Substring(iInd + 4)
                iInd = sPage.IndexOf("</ul>")
            End If
        End While

        sTmp = sTmp & sPage

        WytnijObrazkiDE = sTmp & "</ul>"
    End Function

    Private Function WytnijObrazkiFR(sPage As String) As String
        ' bardziej skomplikowane niz DE, bo są sublisty (znaczy runtime bardziej skomplikowany)
        If sPage = "" Then Return ""    ' żeby nie było <u></ul>
        Dim sResult As String = ""
        Dim oDom1 As XmlDocument = New XmlDocument
        sPage = "<root>" & sPage & "</root>"  ' bo inaczej error ze tylko jeden root element moze byc
        Try
            oDom1.LoadXml(sPage)
        Catch ex As Exception
            oDom1.LoadXml("<ul><li>0 ERROR loading sPage, WytnijObrazkiFR</li></ul>")
        End Try
        Dim oRoot1 As XmlElement = oDom1.DocumentElement
        Dim oNodes1 As XmlNodeList = oRoot1.SelectNodes("/root/ul/li")

        ' moze byc: <root><ul><li><ul><li> - tego glebiej nie ruszac!
        For Each oNode As IXmlNode In oNodes1
            sResult = sResult & vbCrLf & oNode.GetXml()
        Next

        WytnijObrazkiFR = "<ul>" & sResult & "</ul>"
    End Function

    ''' <summary>
    ''' rok wedle Wikipedii -> rok do sortowania
    ''' </summary>
    Private Function Li2Rok(sTxt As String) As Integer
        Dim iRok As Integer = 0
        Dim iInd, iInd1, iInd2, iInd3 As Integer
        sTxt = sTxt.Trim    ' " 422 -" wydarzenia na swiecie pl.wikipedia

        iInd1 = sTxt.IndexOf(" ")
        iInd2 = sTxt.IndexOf(":") ' przy <li>rok:<ul> (wersja PL)
        iInd3 = sTxt.IndexOf(ChrW(160)) ' rosyjskojezyczna ma ROK<160><kreska><spacja>
        iInd = 0

        If iInd1 = -1 Then iInd1 = 99
        If iInd2 = -1 Then iInd2 = 99
        If iInd3 = -1 Then iInd3 = 99
        iInd = Math.Min(Math.Min(iInd1, iInd2), iInd3)

        If iInd > 0 And iInd < 6 Then
            Try
                iRok = CInt(sTxt.Substring(0, iInd))
            Catch ex As Exception
                Debug.Write("Error CInt(" & sTxt.Substring(0, iInd) & ")")
            End Try
            sTxt = sTxt.Substring(iInd)
            If sTxt.Length() > 10 Then sTxt = sTxt.Substring(0, 10)  ' 20180115, bo jakis XBOX mial w tej funkcji out-of-range
            If sTxt.IndexOf(" BC") = 0 Then iRok = -iRok    ' en
            If sTxt.IndexOf(" p.n.e") = 0 Then iRok = -iRok ' pl
            If sTxt.IndexOf(" v. Chr") = 0 Then iRok = -iRok 'de
            If sTxt.IndexOf(" a. C") = 0 Then iRok = -iRok  ' es
            ' fr.wiki podaje MINUS :) (bez spacji) 
            If sTxt.IndexOf(" до н.") = 0 Then iRok = -iRok  ' ru
        End If
        Li2Rok = iRok
    End Function

    ''' <summary>
    ''' podziel _sPage_ na kawałki wedle H3, i scal je potem w jeden - od H3 do H3 jest juz posortowane
    ''' </summary>
    Private Function PrepareAndSplitSort(sPage As String) As String
        ' wejscie: <ul> ... </ul>, po drodze </ul><h3><ul>, moga byc obrazki..

        Dim sResult As String = ""
        Dim oDom1 As XmlDocument = New XmlDocument

        Try
            oDom1.LoadXml("<root>" & sPage & "</root>")
        Catch ex As Exception
            oDom1.LoadXml("<ul><li>0 ERROR loading sPage, SplitAndSort</li></ul>")
        End Try

        Dim oRoot1 As XmlElement = oDom1.DocumentElement
        ' dowolny na poziomie 1
        Dim oNodes1 As XmlNodeList = oRoot1.SelectNodes("/root/*")
        ' moze byc: <root><ul><li><ul><li> ALBO H3 ALBO <div> (obrazek)
        For Each oNode As IXmlNode In oNodes1
            If (oNode.NodeName = "ul" AndAlso (oNode.Attributes.Count < 1 OrElse oNode.Attributes.Item(0).NodeName <> "class")) _
                                Or oNode.NodeName = "h3" Then
                sResult = sResult & oNode.GetXml.Trim
            End If
        Next

        sResult = sResult.Replace("</ul><ul>", "")   ' miejsca po obrazkach
        PrepareAndSplitSort = SplitAndSort(sResult)
    End Function

    Private Function SplitAndSort(sTxtIn As String) As String
        ' podziel na kawalki wedle H3, i scal je potem w jeden - od H3 do H3 jest juz posortowane
        ' wejscie: <ul> ... </ul>, po drodze </ul><h3><ul>, moga byc obrazki..

        Dim iInd As Integer = 0
        Dim sTxtOut As String = ""
        Dim sTmp As String

        iInd = sTxtIn.IndexOf("<h3")
        While iInd > 0
            sTmp = sTxtIn.Substring(0, iInd)
            sTxtIn = sTxtIn.Substring(iInd)

            iInd = sTmp.LastIndexOf("</ul>")
            If iInd > 0 Then sTmp = sTmp.Substring(0, iInd + 5)

            iInd = sTxtIn.IndexOf("<ul>")
            If iInd > 0 Then sTxtIn = sTxtIn.Substring(iInd)

            sTxtOut = MergeSorted(sTxtOut, sTmp)
            iInd = sTxtIn.IndexOf("<h3")
        End While

        iInd = sTxtIn.LastIndexOf("</ul>")
        If iInd > 0 Then sTxtIn = sTxtIn.Substring(0, iInd + 5)
        SplitAndSort = MergeSorted(sTxtOut, sTxtIn)

    End Function

    Private Function PoprawRok(sTxt As String) As String
        'en: <li><a href="/wiki/301" title="301">301</a> &#8211;
        'de: <li><span style="visibility:hidden;">0</span><a href="/wiki/301" title="301">301</a>: 
        'fr: <li><a href="/wiki/301" title="301">301</a>&#160;:
        'es: <li><a href="/wiki/301" title="301">301</a>: 
        'pl: <li>&#160; <a href="/wiki/301" title="301">301</a> – 
        'ru: <li><a title="863 год" href="/wiki/863_%D0%B3%D0%BE%D0%B4">863</a>&nbsp;— 
        ' &#8211 = endash
        Dim sOut As String = sTxt
        sOut = sOut.Replace("<span style=""visibility:hidden;"">0</span>", "&#160;")    ' DE wyrównanie
        sOut = sOut.Replace("—", "–")   ' RU emdash na endash
        sOut = sOut.Replace("</a>&#160;:", "</a> –")    ' FR
        sOut = sOut.Replace("<li>&#160; &#160;", "<li>&#160;&#160;")    ' PL dla <100
        sOut = sOut.Replace("<li>&#160; ", "<li>&#160;")    ' PL dla <1000

        sOut = sOut.Replace("<li>&#160;", "<li>")    ' wyrownanie do lewej
        sOut = sOut.Replace("<li>&#160;", "<li>")


        Return sOut
    End Function

    Private Function MergeSorted(sTxt1 As String, sTxt2 As String) As String
        ' wsortowanie sTxt2 do sTxt1

        If sTxt1 = "" Then Return sTxt2 ' pierwsza strona - bez sortowania
        If sTxt2 = "" Then Return sTxt1 ' symetrycznie niezdarzalnie

        'EN: <ul>\n<li><a href = "/wiki/214" title="214">214</a> (czasem nie ma linka, ale to chyba przy powtorkach?)
        'PL: <ul>\n<li>&#160; <a href= "/wiki/214" title="214">214<
        ' tyle ze teraz linki sa juz pelne, tzn. https://pl.wikipedia.org/wiki/214

        Dim sResult As String = ""
        Dim sTmp As String = ""

        Dim oDom1 As XmlDocument = New XmlDocument
        Dim oDom2 As XmlDocument = New XmlDocument
        Try
            oDom1.LoadXml(sTxt1)
        Catch ex As Exception
            oDom1.LoadXml("<ul><li>0 ERROR loading sTxt1, lang: " & mCurrLang & ", part: " & mCurrPart & "</li></ul>")
        End Try

        Try
            oDom2.LoadXml(sTxt2)
        Catch ex As Exception
            oDom2.LoadXml("<ul><li>0 ERROR loading sTxt2, lang: " & mCurrLang & ", part: " & mCurrPart & "</li></ul>")
        End Try
        ' </ul><ul>
        Dim oRoot1 As XmlElement = oDom1.DocumentElement
        Dim oRoot2 As XmlElement = oDom2.DocumentElement
        Dim oNodes1 As XmlNodeList = oRoot1.SelectNodes("li")
        Dim oNodes2 As XmlNodeList = oRoot2.SelectNodes("li")

        ' gdy jest wczesniej błąd, to faktycznie moze byc count=0
        If oNodes1.Count = 0 Or oNodes2.Count = 0 Then
            '    Dim msg As ContentDialog
            '    msg = New ContentDialog With {
            '        .Title = "ERROR",
            '        .Content = "MergeSorted error, Count=0?",
            '        .CloseButtonText = "Pa"
            '    }
            '    msg.ShowAsync()
            Return ""
        End If

        Dim i1 As Integer = 0
        Dim i2 As Integer = 0

        Dim oNode1 As IXmlNode = oNodes1.ElementAt(i1)  ' SDK 1803 - nie zna typu? dopiero po rebuild zna
        Dim oNode2 As IXmlNode = oNodes2.ElementAt(i2)  ' SDK 1803 - nie zna typu? j.w.
        Dim iRok1 As Integer = Li2Rok(oNode1.InnerText)
        Dim iRok2 As Integer = Li2Rok(oNode2.InnerText)


        While i1 < oNodes1.Count And i2 < oNodes2.Count
            If iRok1 < iRok2 Then
                sResult = sResult & vbCrLf & PoprawRok(oNode1.GetXml())
                i1 = i1 + 1
                If i1 < oNodes1.Count Then
                    oNode1 = oNodes1.ElementAt(i1)
                    iRok1 = Li2Rok(oNode1.InnerText)
                End If
            Else
                sResult = sResult & vbCrLf & PoprawRok(oNode2.GetXml())
                i2 = i2 + 1
                If i2 < oNodes2.Count Then
                    oNode2 = oNodes2.ElementAt(i2)
                    iRok2 = Li2Rok(oNode2.InnerText)
                End If
            End If
        End While

        While i1 < oNodes1.Count
            oNode1 = oNodes1.ElementAt(i1)
            sResult = sResult & vbCrLf & PoprawRok(oNode1.GetXml())
            i1 = i1 + 1
        End While

        While i2 < oNodes2.Count
            oNode2 = oNodes2.ElementAt(i2)
            sResult = sResult & vbCrLf & PoprawRok(oNode2.GetXml())
            i2 = i2 + 1
        End While

        If sResult = "" Then Return ""
        Return "<ul>" & sResult & "</ul>"

    End Function

    ''' <summary>
    ''' Zamiana linku _sPage_ dla jezyka _sLang_ na pelny link
    ''' </summary>
    Private Function DodajPelnyLink(sPage As String, sLang As String) As String
        Dim sTmp As String = sPage
        sTmp = sTmp.Replace("""/wiki/", """https://" & sLang & ".wikipedia.org/wiki/")
        DodajPelnyLink = sTmp
    End Function

    Private Async Function ReadOneLang(sUrl As String, sPrefLang As String) As Task(Of String)
        Dim sTxt As String
        sTxt = Await GetHtmlPage(sUrl)

        Dim iInd As Integer
        sUrl = sUrl.Replace("https://", "")
        iInd = sUrl.IndexOf(".")
        sUrl = sUrl.Substring(0, iInd)

        sTxt = DodajPelnyLink(sTxt, sUrl)
        mCurrLang = sUrl

        Dim sTabs As String
        sTabs = App.GetSettingsString("EnabledTabs", "EBD")

        Select Case sUrl
            Case "en"
                If sTabs.IndexOf("E") > -1 Then mEvents = MergeSorted(mEvents, WyciagnijDane(sTxt, "id=""Events"">Events"))
                If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WyciagnijDane(sTxt, "id=""Births"">Births"))
                If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(sTxt, "id=""Deaths"">Deaths"))
                If sTabs.IndexOf("H") > -1 Then mHolid = mHolid & WyciagnijDane(sTxt, "observances"">Holidays")
            Case "pl"
                If sTabs.IndexOf("E") > -1 Then
                    mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Wydarzenia_w_Pols")))
                    mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Wydarzenia_na_świ")))
                End If
                If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WyciagnijDane(sTxt, "id=""Urodzili_się"))
                If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(sTxt, "id=""Zmarli"))
                If sTabs.IndexOf("H") > -1 Then mHolid = mHolid & WyciagnijDane(sTxt, "id=""Święta")
            Case "de"
                If sTabs.IndexOf("E") > -1 Then mEvents = MergeSorted(mEvents, PrepareAndSplitSort(WyciagnijDane(sTxt, "id=""Ereignisse")))
                'If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WytnijObrazkiDE(WyciagnijDane(sTxt, "id=""Geboren")))
                'If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WytnijObrazkiDE(WyciagnijDane(sTxt, "id=""Gestorben")))
                If sTabs.IndexOf("B") > -1 Then
                    Dim sTmp As String = PrepareAndSplitSort(WyciagnijDane(sTxt, "id=""Geboren"))
                    mBirths = MergeSorted(mBirths, sTmp)
                End If
                If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, PrepareAndSplitSort(WyciagnijDane(sTxt, "id=""Gestorben")))
                If sTabs.IndexOf("H") > -1 Then mHolid = mHolid & WyciagnijDane(sTxt, "id=""Feier")
            Case "fr"
                If sTabs.IndexOf("E") > -1 Then
                    mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Événements")))
                    mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Arts,_culture")))
                    mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Sciences_et")))
                    mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Économie")))
                End If
                If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WytnijObrazkiFR(WyciagnijDane(sTxt, "es"">Naissances")))
                If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WytnijObrazkiFR(WyciagnijDane(sTxt, "s"">Décès")))
                If sTabs.IndexOf("H") > -1 Then mHolid = mHolid & WyciagnijDane(sTxt, "ns"">Célébrations")
            Case "es"
                'If sTabs.IndexOf("E") > -1 Then mEvents = MergeSorted(mEvents, WytnijObrazkiDE(WyciagnijDane(sTxt, "s"">Acontecimientos")))
                'If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WytnijObrazkiDE(WyciagnijDane(sTxt, "s"">Nacimientos")))
                'If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WytnijObrazkiDE(WyciagnijDane(sTxt, "s"">Fallecimientos")))
                If sTabs.IndexOf("E") > -1 Then mEvents = MergeSorted(mEvents, PrepareAndSplitSort(WyciagnijDane(sTxt, "s"">Acontecimientos")))
                If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, PrepareAndSplitSort(WyciagnijDane(sTxt, "s"">Nacimientos")))
                If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, PrepareAndSplitSort(WyciagnijDane(sTxt, "s"">Fallecimientos")))
                If sTabs.IndexOf("H") > -1 Then mHolid = mHolid & WyciagnijDane(sTxt, "s"">Celebraciones")
            Case "ru"
                If sTabs.IndexOf("E") > -1 Then mEvents = MergeSorted(mEvents, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""События")))
                If sTabs.IndexOf("B") > -1 Then mBirths = MergeSorted(mBirths, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Родились")))
                If sTabs.IndexOf("D") > -1 Then mDeaths = MergeSorted(mDeaths, WytnijObrazkiFR(WyciagnijDane(sTxt, "id=""Скончались")))
                If sTabs.IndexOf("H") > -1 Then mHolid = mHolid & WyciagnijDane(sTxt, "id=""Праздники")
            Case Else
                mEvents = mEvents & "<h2>Unsupported lang: " & sUrl & "</h2>"
                mBirths = mBirths & "<h2>Unsupported lang: " & sUrl & "</h2>"
                mDeaths = mDeaths & "<h2>Unsupported lang: " & sUrl & "</h2>"
                mHolid = mHolid & "<h2>Unsupported lang: " & sUrl & "</h2>"

        End Select

        Return sTxt
    End Function

    Private Function ExtractLangLinks(sForLangs As String, sPage As String) As List(Of String)
        Dim lList As List(Of String) = New List(Of String)     ' SDK 1803 - problem z typem?

        Dim iInd As Integer
        Dim sTmp As String
        Dim aArr As String()   ' było Array, SDK 1803 - nie zna typu dla sLang, zmieniam na String()
        aArr = sForLangs.Split(" ")
        For Each sLang As String In aArr
            sLang = "https://" & sLang & ".wikipedia"
            iInd = sPage.IndexOf(sLang)
            If iInd > 100 Then
                sTmp = sPage.Substring(iInd)
                iInd = sTmp.IndexOf("""")
                sTmp = sTmp.Substring(0, iInd)
                lList.Add(sTmp)
            End If
        Next

        ExtractLangLinks = lList
    End Function

    Private Async Function GetHtmlPage(sUrl As String) As Task(Of String)
        Dim oHttp As Windows.Web.Http.HttpClient = New Windows.Web.Http.HttpClient()
        Dim oUri As Uri = New Uri(sUrl)
        Dim oResp As New Windows.Web.Http.HttpResponseMessage
        oResp = Await oHttp.GetAsync(oUri)
        If Not oResp.IsSuccessStatusCode Then
            Dim msg As ContentDialog
            msg = New ContentDialog With {
                .Title = "ERROR",
                .Content = "GetHtmlPage error, URL=" & sUrl,
                .CloseButtonText = "Pa"
            }
            Await msg.ShowAsync()
            Return ""
        End If
        Dim sTxt As String
        sTxt = Await oResp.Content.ReadAsStringAsync()
        Return sTxt
    End Function

    Private Async Sub bRead_Click(sender As Object, e As RoutedEventArgs)
        Dim sUrl As String = ""

        sUrl = "https://en.wikipedia.org/wiki/" & MonthNo2EnName(mDate.Month) & "_" & mDate.Day.ToString
        mEvents = ""
        mBirths = ""
        mDeaths = ""
        mHolid = ""
        tbDzien.Text = "Reading EN..."
        Dim sTxt As String = Await ReadOneLang(sUrl, mPreferredLang)

        sUrl = App.GetSettingsString("EnabledLanguages", "pl de fr es ru")
        Dim lList As List(Of String) = ExtractLangLinks(sUrl, sTxt)
        For Each sUri As String In lList
            tbDzien.Text = "Reading " & sUri.Substring(8, 2).ToUpperInvariant & "..."
            Await ReadOneLang(sUri, mPreferredLang)
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

        tbDzien.Text = mDate.ToString("d MMMM")  ' .Day.ToString & " " & MonthNo2PlName(mDate.Month)

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
    Private Sub bEvent_Click(sender As Object, e As RoutedEventArgs)
        SetWebView(mEvents, "") ' "<base href=""https://en.wikipedia.org/"">")
        bEvent.IsChecked = True
        bHolid.IsChecked = False
        bBirth.IsChecked = False
        bDeath.IsChecked = False
    End Sub
    Private Sub bHolid_Click(sender As Object, e As RoutedEventArgs)
        SetWebView(mHolid, "") '  "<base href=""https://en.wikipedia.org/"">")
        bEvent.IsChecked = False
        bHolid.IsChecked = True
        bBirth.IsChecked = False
        bDeath.IsChecked = False
    End Sub
    Private Sub bBirth_Click(sender As Object, e As RoutedEventArgs)
        SetWebView(mBirths, "") ' "<base href=""https://en.wikipedia.org/"">"
        bEvent.IsChecked = False
        bHolid.IsChecked = False
        bBirth.IsChecked = True
        bDeath.IsChecked = False
    End Sub
    Private Sub bDeath_Click(sender As Object, e As RoutedEventArgs)
        SetWebView(mDeaths, "") ' "<base href=""https://en.wikipedia.org/"">")
        bEvent.IsChecked = False
        bHolid.IsChecked = False
        bBirth.IsChecked = False
        bDeath.IsChecked = True
    End Sub

    Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs)

        Dim sTmp As String
        sTmp = App.GetSettingsString("EnabledTabs", "EBD")

        bEvent.IsEnabled = (sTmp.IndexOf("E") > -1)
        bBirth.IsEnabled = (sTmp.IndexOf("B") > -1)
        bDeath.IsEnabled = (sTmp.IndexOf("D") > -1)
        bHolid.IsEnabled = (sTmp.IndexOf("H") > -1)

        mDate = Date.Now
        uiDay.Date = mDate

    End Sub

    Private Sub wbViewer_NavigationStarting(sender As WebView, args As WebViewNavigationStartingEventArgs) Handles wbViewer.NavigationStarting

        If args.Uri Is Nothing Then Exit Sub

        args.Cancel = True

        If Not App.GetSettingsBool("LinksActive") Then Exit Sub

        Windows.System.Launcher.LaunchUriAsync(args.Uri)

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
