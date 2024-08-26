
' wzięte z Uno.Shared:MainPage.xaml.cs 
' 2022.01.26

#Disable Warning CA1707 ' Identifiers should not contain underscores
#Disable Warning CA2007 'Consider calling ConfigureAwait On the awaited task

Public Module MainPage

    Private mEvents As New List(Of OneRocznica)
    Private mBirths As New List(Of OneRocznica)
    Private mDeaths As New List(Of OneRocznica)

    ' nazwa, odpowiednio Case dla Wikipedii - gdy poza zakresem, zwroci styczen
    Public Function MonthNo2EnName(ByVal iMonth As Integer) As String ' static, bo wywoływane z  InfoAbout

        Select Case iMonth
            Case 1
                Return "January"
            Case 2
                Return "February"
            Case 3
                Return "March"
            Case 4
                Return "April"
            Case 5
                Return "May"
            Case 6
                Return "June"
            Case 7
                Return "July"
            Case 8
                Return "August"
            Case 9
                Return "September"
            Case 10
                Return "October"
            Case 11
                Return "November"
            Case 12
                Return "December"
            Case Else
                Return "January"
        End Select
    End Function

    Public Sub bRead_Click_Reset()
        ' fragment bRead_Click, żeby nie trzeba było Agility dodawać do samej App - wystarczy w VBlib
        mEvents = New List(Of OneRocznica)
        mBirths = New List(Of OneRocznica)
        mDeaths = New List(Of OneRocznica)
    End Sub

    Public Function GetContentForWebview(sTab As String) As String
        Select Case sTab
            Case "E"
                Return Dict2Web(mEvents)
            Case "B"
                Return Dict2Web(mBirths)
            Case "D"
                Return Dict2Web(mDeaths)
            Case Else
                DumpMessage("unrecognized sTab (" & sTab & ") in GetContentForWebview, using Events")
                Return Dict2Web(mEvents)
        End Select

    End Function

    Private Function Dict2Web(dict As List(Of OneRocznica)) As String
        If dict Is Nothing Then Return ""
        Dim ret As String = ""
        For Each oItem In dict.OrderBy(Of Integer)(Function(x) x.rok)
            ret = ret & vbCrLf & "<li>" & oItem.html
        Next

        Return "<html><body><ul>" & ret & "</ul></body></html>"
    End Function


    ''' <summary>
    ''' Wycięcie z htmlDoc fragmentu od h2 zawierającego _sFrom_ (np. "id=wydarzenia") do początku następnego h2
    ''' </summary>
    Private Function WyciagnijSekcjeH2(htmlDoc As HtmlAgilityPack.HtmlDocument, sFrom As String, sLang As String) As HtmlAgilityPack.HtmlDocument
        DumpCurrMethod(sFrom)

        Dim oRetXml = New HtmlAgilityPack.HtmlDocument()

        ' ze względu na zmianę struktury ( <h2> jest pod div, więc trzeba byłoby szukać DIVa w którym jest H2, i iterować siblingi DIVa..
        Dim sPage As String = htmlDoc.DocumentNode.InnerHtml
        Dim iInd As Integer = sPage.IndexOf("<h2 id=""" & sFrom)
        If iInd < 10 Then Return oRetXml

        sPage = sPage.Substring(iInd)
        iInd = sPage.IndexOf("<h2", 10)
        sPage = sPage.Substring(0, iInd)

        oRetXml.LoadHtml(sPage)
        Return oRetXml

    End Function



    Private Sub UsunElementy(oDoc As HtmlAgilityPack.HtmlDocument, sTagName As String, Optional sTagAttr As String = "")
        For iGuard = 100 To 1 Step -1
            Dim bBreak = True
            Dim oNodes = oDoc.DocumentNode.SelectNodes("//" & sTagName)

            If oNodes IsNot Nothing Then
                For Each oNode As HtmlAgilityPack.HtmlNode In oNodes
                    If sTagAttr = "" OrElse oNode.OuterHtml.Contains(sTagAttr) Then oNode.ParentNode.RemoveChild(oNode)
                    bBreak = False
                    Exit For
                Next
            End If

            If bBreak Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' Wycięcie z htmlDoc fragmentu od h2 zawierającego _sFrom_ (np. "id=wydarzenia") do początku następnego h2
    ''' Ma usunąć wszystkie niepotrzebne rzeczy.
    ''' jest to robione dla języka sLang (jakby były jakieś różnice)
    ''' </summary>
    Private Function WyciagnijDane(htmlDoc As HtmlAgilityPack.HtmlDocument, sFrom As String, sLang As String) As List(Of OneRocznica)
        DumpCurrMethod($"WyciagnijDane(htmlDoc, '{sFrom}', '{sLang}'")
        Dim oH2 = WyciagnijSekcjeH2(htmlDoc, sFrom, sLang)
        If oH2 Is Nothing Then Return Nothing

        UsunElementy(oH2, "h2")
        UsunElementy(oH2, "span", "mw-edit")

        ' usuwamy obrazki: <div class="thumb tright"> oraz tleft
        ' dla: DE, FR, ES
        UsunElementy(oH2, "div", "div class=""thumb")

        ' dla UA
        UsunElementy(oH2, "figure")

        ' usuwamy link do multimedia
        ' dla: PL
        UsunElementy(oH2, "table", "infobox")

        ' nic nie robi dla:
        ' PL: (ale za to MergeSorted h2 świat i PL)
        ' dla EL: nic
        ' mogłoby być wcześniej, ale żaden problem spróbować usunąć
        'If sLang = "pl" OrElse sLang = "el" Then Return oH2

        ' dla DE:
        '  w wydarzeniach, H3 do MergeSorted, w pozostałych - do usunięcia
        'If sLang = "de" Then
        '    oH2 = SplitAndSort(oH2, "h3")
        'End If

        ' usuwamy podrozdziały (h3)
        ' dla: EN, RU, UK
        UsunElementy(oH2, "h3", "")

        ' pod-nagłówki h4 (DE)
        UsunElementy(oH2, "h4", "")

        ' usuwamy DL (RU)
        UsunElementy(oH2, "dl", "")
        ' dziwne coś, także tylko dla RU
        UsunElementy(oH2, "div", "hatnote")

        ' dla FR, PL w wydarzeniach sklejaj H2 - ale to się robi "piętro wyżej"

        ' usuń podział na <ul></ul><ul> (po tych podziałach h3)
        Dim sXml As String = oH2.DocumentNode.OuterHtml
        sXml = sXml.Replace(vbLf, " ")
        sXml = sXml.Replace(vbCr, " ")
        'sXml = sXml.Replace("  ", " ")
        'sXml = sXml.Replace("  ", " ")
        'sXml = sXml.Replace("  ", " ")
        'sXml = sXml.Replace("</ul> <ul>", "")
        'sXml = sXml.Replace("</ul><ul>", "")

        oH2.LoadHtml(sXml)

        ' teraz bierzemy tylko LI
        Dim retDict As New List(Of OneRocznica)
        For Each oNode In oH2.DocumentNode.SelectNodes("//li")
            Dim rok As Integer = Li2Rok(oNode.InnerText)
            ' DE: li ma podpunkty, bez powtórzenia roku - bez tego zabezpieczenia by było jako rok 0
            If rok > 2340 Then Continue For
            retDict.Add(New OneRocznica(rok, oNode.InnerHtml))
        Next

        Return retDict
    End Function

    Private Function IndexOfOr99(ByVal sTxt As String, ByVal sSubstring As String) As Integer
        Dim iRet = sTxt.IndexOf(sSubstring, StringComparison.Ordinal)
        If iRet = -1 Then Return 99
        Return iRet
    End Function

    ''' <summary>
    ''' rok wedle Wikipedii -> rok do sortowania
    ''' </summary>
    Private Function Li2Rok(ByVal sTxt As String) As Integer
        Dim iRok = 2345 ' nieudana konwersja
        Dim iInd As Integer
        sTxt = sTxt.Trim()    ' " 422 -" wydarzenia na swiecie pl.wikipedia
        iInd = sTxt.IndexOf(Microsoft.VisualBasic.ChrW(160)) ' rosyjskojezyczna ma ROK<160><kreska><spacja>
        If iInd = -1 Then iInd = 99
        iInd = Math.Min(iInd, IndexOfOr99(sTxt, " "))
        iInd = Math.Min(iInd, IndexOfOr99(sTxt, ":"))
        iInd = Math.Min(iInd, IndexOfOr99(sTxt, "#"))
        iInd = Math.Min(iInd, IndexOfOr99(sTxt, "&"))
        iInd = Math.Min(iInd, IndexOfOr99(sTxt, "年"))  ' japonski
        iInd = Math.Min(iInd, IndexOfOr99(sTxt, "년"))  ' koreanski
        iInd = Math.Min(iInd, IndexOfOr99(sTxt, "年"))  ' chinski

        If iInd > 0 And iInd < 6 Then
            If Not Integer.TryParse(sTxt.Substring(0, iInd), iRok) Then
                Debug.Write("Error CInt(" & sTxt.Substring(0, iInd) & ")")
                Return 2345
            End If
            sTxt = sTxt.Substring(iInd)
            If sTxt.Length > 10 Then sTxt = sTxt.Substring(0, 10)  ' 20180115, bo jakis XBOX mial w tej funkcji out-of-range
            If sTxt.IndexOf(" BC", StringComparison.Ordinal) = 0 Then iRok = -iRok    ' en
            If sTxt.IndexOf(" p.n.e", StringComparison.Ordinal) = 0 Then iRok = -iRok ' pl
            If sTxt.IndexOf(" v. Chr", StringComparison.Ordinal) = 0 Then iRok = -iRok ' de
            If sTxt.IndexOf(" a. C", StringComparison.Ordinal) = 0 Then iRok = -iRok  ' es
            ' fr.wiki podaje MINUS :) (bez spacji) 
            If sTxt.IndexOf(" до н.", StringComparison.Ordinal) = 0 Then iRok = -iRok  ' ru
        End If

        Return iRok
    End Function

    ''' <summary>
    ''' podziel _sPage_ na kawałki wedle H3, i scal je potem w jeden - od H3 do H3 jest juz posortowane
    ''' </summary>
    Private Function SplitAndSort(oDom As HtmlAgilityPack.HtmlDocument, sSplitTag As String) As HtmlAgilityPack.HtmlDocument
        Dim oRetDoc = New HtmlAgilityPack.HtmlDocument()

        For Each oNode In oDom.DocumentNode.SelectNodes("//" & sSplitTag)
            Dim sTmpDoc = ""
            Dim oEntry = oNode.NextSibling

            While oEntry IsNot Nothing
                If oEntry.Name = "h2" OrElse oEntry.Name = sSplitTag Then Exit While

                If oEntry.Name = "ul" Then
                    For Each oItem In oEntry.ChildNodes
                        sTmpDoc += oItem.OuterHtml
                    Next
                End If

                oEntry = oEntry.NextSibling
            End While

            Dim oTmpDoc = New HtmlAgilityPack.HtmlDocument()
            oTmpDoc.LoadHtml("<root><ul>" & sTmpDoc & "</ul></root>")
            oRetDoc = MergeSorted(oRetDoc, oTmpDoc)
        Next

        Return oRetDoc
    End Function

    Private Function PoprawRok(sTxt As String) As String
        ' en: <li><a href="/wiki/301" title="301">301</a> &#8211;
        ' de: <li><span style="visibility:hidden;">0</span><a href="/wiki/301" title="301">301</a>: 
        ' fr: <li><a href="/wiki/301" title="301">301</a>&#160;:
        ' es: <li><a href="/wiki/301" title="301">301</a>: 
        ' pl: <li>&#160; <a href="/wiki/301" title="301">301</a> – 
        ' ru: <li><a title="863 год" href="/wiki/863_%D0%B3%D0%BE%D0%B4">863</a>&nbsp;— 
        ' &#8211 = endash
        Dim sOut = sTxt
        sOut = sOut.Replace("<span style=""visibility:hidden;"">0</span>", "&#160;")    ' DE wyrównanie
        sOut = sOut.Replace("—", "–")   ' RU emdash na endash
        sOut = sOut.Replace("</a>&#160;:", "</a> –")    ' FR
        sOut = sOut.Replace("<li>&#160; &#160;", "<li>&#160;&#160;")    ' PL dla <100
        sOut = sOut.Replace("<li>&#160; ", "<li>&#160;")    ' PL dla <1000
        sOut = sOut.Replace("<li>&#160;", "<li>")    ' wyrownanie do lewej
        Return sOut
    End Function

    Private Function MergeSorted(oDom1 As HtmlAgilityPack.HtmlDocument, oDom2 As HtmlAgilityPack.HtmlDocument) As HtmlAgilityPack.HtmlDocument
        DumpCurrMethod()
        ' wsortowanie sTxt2 do sTxt1

        If oDom1 Is Nothing Then Return oDom2 ' pierwsza strona - bez sortowania
        If oDom2 Is Nothing Then Return oDom1 ' symetrycznie niezdarzalnie

        ' EN: <ul>\n<li><a href = "/wiki/214" title="214">214</a> (czasem nie ma linka, ale to chyba przy powtorkach?)
        ' PL: <ul>\n<li>&#160; <a href= "/wiki/214" title="214">214<
        ' tyle ze teraz linki sa juz pelne, tzn. https://pl.wikipedia.org/wiki/214

        ' </ul><ul>
        Dim oRoot1 As HtmlAgilityPack.HtmlNode = oDom1.DocumentNode ' <root>...
        If oRoot1 Is Nothing Then Return oDom2
        Dim oRoot2 As HtmlAgilityPack.HtmlNode = oDom2.DocumentNode
        If oRoot2 Is Nothing Then Return oDom1

        Dim oNodes1 As HtmlAgilityPack.HtmlNodeCollection = oRoot1.SelectNodes("//li") ' wewnątrz #document powinien być tylko <root>
        Dim oNodes2 As HtmlAgilityPack.HtmlNodeCollection = oRoot2.SelectNodes("//li")

        'If oNodes1.Count < 1 Then Return oDom2
        'If oNodes2.Count < 1 Then Return oDom1
        'If oNodes1.Count > 1 Or oNodes2.Count > 1 Then
        '    ' coś jest nie tak, powinno być tylko jedno
        '    DumpMessage("Something is wrong - should be only one item inside #document!")
        '    Return oDom1
        'End If

        'oNodes1 = oNodes1.ElementAt(0).ChildNodes ' czyli <root> 
        'oNodes2 = oNodes2.ElementAt(0).ChildNodes

        'If oNodes1.Count < 1 Then Return oDom2
        'If oNodes2.Count < 1 Then Return oDom1
        'If oNodes1.Count > 1 Or oNodes2.Count > 1 Then
        '    ' coś jest nie tak, powinno być tylko jedno 
        '    DumpMessage("Something is wrong - should be only one item inside <root>!")

        '    DumpMessage("oNodes1:")
        '    For Each oItem In oNodes1
        '        DumpMessage(oItem.Name)
        '    Next

        '    DumpMessage("oNodes2:")
        '    For Each oItem In oNodes2
        '        DumpMessage(oItem.Name)
        '    Next

        '    Return oDom1
        'End If

        'oNodes1 = oNodes1.ElementAt(0).SelectNodes("//li")   ' a w <root><ul> interesują nas <li>
        'oNodes2 = oNodes2.ElementAt(0).SelectNodes("//li")

        'DumpMessage($"MergeSorted, count1={oNodes1.Count}, count2={oNodes2.Count}")

        ' gdy jest wczesniej błąd, to faktycznie moze byc count=0
        'if ((oNodes1.Count == 0) || (oNodes2.Count == 0))
        '    return "";

        Dim sResult = ""
        Dim i1 = 0
        Dim i2 = 0
        Dim oNode1 As HtmlAgilityPack.HtmlNode = oNodes1.ElementAt(i1)
        Dim oNode2 As HtmlAgilityPack.HtmlNode = oNodes2.ElementAt(i2)
        Dim iRok1 = Li2Rok(oNode1.InnerText)
        Dim iRok2 = Li2Rok(oNode2.InnerText)

        While i1 < oNodes1.Count And i2 < oNodes2.Count

            If iRok1 < iRok2 Then
                sResult = sResult & vbLf & PoprawRok(oNode1.OuterHtml)
                i1 += 1

                If i1 < oNodes1.Count Then
                    oNode1 = oNodes1.ElementAt(i1)
                    iRok1 = Li2Rok(oNode1.InnerText)
                End If
            Else
                sResult = sResult & vbLf & PoprawRok(oNode2.OuterHtml)
                i2 += 1

                If i2 < oNodes2.Count Then
                    oNode2 = oNodes2.ElementAt(i2)
                    iRok2 = Li2Rok(oNode2.InnerText)
                End If
            End If
        End While

        While i1 < oNodes1.Count
            oNode1 = oNodes1.ElementAt(i1)
            sResult = sResult & vbLf & PoprawRok(oNode1.OuterHtml)
            i1 += 1
        End While

        While i2 < oNodes2.Count
            oNode2 = oNodes2.ElementAt(i2)
            sResult = sResult & vbLf & PoprawRok(oNode2.OuterHtml)
            i2 += 1
        End While

        Dim oRetDoc = New HtmlAgilityPack.HtmlDocument()
        If String.IsNullOrEmpty(sResult) Then Return oRetDoc
        oRetDoc.LoadHtml($"<root><ul>{sResult}</ul></root>")
        Return oRetDoc
    End Function

    ''' <summary>
    ''' Zamiana linku _sPage_ dla jezyka _sLang_ na pelny link
    ''' </summary>
    Private Function DodajPelnyLink(sPage As String, sLang As String) As String
        Dim sTmp = sPage
        sTmp = sTmp.Replace("""/wiki/", $"""https://{sLang}.wikipedia.org/wiki/")
        Return sTmp
    End Function

    ''' <summary>
    ''' wczytaj dane z sUrl, dodawaj do mEvents, mBirths, mDeaths i mHolid
    ''' </summary>
    Public Async Function ReadOneLang(oUri As Uri) As Task(Of String)
        If oUri Is Nothing Then Return ""   ' dla CA1062

        DumpCurrMethod(oUri.ToString)
        Dim sTxt As String
        sTxt = Await HttpPageAsync(oUri)
        If sTxt = "" Then Return ""

        Dim iInd As Integer
        Dim sLang = oUri.Host
        iInd = sLang.IndexOf(".", StringComparison.Ordinal)
        sLang = sLang.Substring(0, iInd)
        sTxt = DodajPelnyLink(sTxt, sLang)
        If sTxt.StartsWith("<!DOCTYPE html>", StringComparison.Ordinal) Then sTxt = sTxt.Replace("<!DOCTYPE html>", "")
        Dim htmlDoc = New HtmlAgilityPack.HtmlDocument()
        htmlDoc.LoadHtml(sTxt)
        Dim sTabs As String
        sTabs = GetSettingsString("EnabledTabs")

        Select Case sLang
            Case "en"
                DodajEventy(sTabs, htmlDoc, sLang, "Events", "Births", "Deaths")
            Case "de"
                DodajEventy(sTabs, htmlDoc, sLang, "Ereignisse", "Geboren", "Gestorben")
            Case "pl"
                If sTabs.Contains("E") Then
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "Wydarzenia_w_Pols", sLang)).ToList
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "Wydarzenia_na_świ", sLang)).ToList
                End If
                DodajEventy(sTabs, htmlDoc, sLang, "", "Urodzili", "Zmarli")
            Case "fr"
                If sTabs.Contains("E") Then
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "Événements", sLang)).ToList
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "Arts,_culture", sLang)).ToList
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "Sciences_et", sLang)).ToList
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "Économie", sLang)).ToList
                End If
                DodajEventy(sTabs, htmlDoc, sLang, "", "Naissances", "Décès")
            Case "es"
                DodajEventy(sTabs, htmlDoc, sLang, "Acontecimientos", "Nacimientos", "Fallecimientos")
            Case "ru"
                DodajEventy(sTabs, htmlDoc, sLang, "События", "Родились", "Скончались")
            Case "uk"
                DodajEventy(sTabs, htmlDoc, sLang, "Події", "Народились", "Померли")
            Case "el"
                DodajEventy(sTabs, htmlDoc, sLang, "Γεγονότα", "Γεννήσεις", "Θάνατοι")
            Case "he"
                DodajEventy(sTabs, htmlDoc, sLang, "אירועים", "נולדו", "נפטרו")
            Case "ja"
                DodajEventy(sTabs, htmlDoc, sLang, "できごと", "誕生日", "忌日")
            Case "ar"
                DodajEventy(sTabs, htmlDoc, sLang, "أحداث", "مواليد", "وفيات")
            Case "ka"
                DodajEventy(sTabs, htmlDoc, sLang, "დღის_მოვლენები", "ამ_დღეს_დაბადებულნი", "ამ_დღეს_გარდაცვლილნი")
            Case "ko"
                DodajEventy(sTabs, htmlDoc, sLang, "", "탄생", "사망")

                If sTabs.Contains("E") Then
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "사건", sLang)).ToList
                    mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, "문화", sLang)).ToList
                End If
            Case "zh"
                DodajEventy(sTabs, htmlDoc, sLang, "大事记", "出生", "逝世")
            Case Else
                Dim oUnsupported As New OneRocznica(10, $"<h2>Unsupported lang: {sLang}</h2>")
                mEvents.Add(oUnsupported)
                mBirths.Add(oUnsupported)
                mDeaths.Add(oUnsupported)
        End Select

        Return sTxt
    End Function

    Private Sub DodajEventy(sTabs As String, htmlDoc As HtmlAgilityPack.HtmlDocument, sLang As String, eventsHdr As String, birthsHdr As String, deathsHdr As String)
        If sTabs.Contains("E") AndAlso eventsHdr <> "" Then mEvents = mEvents.Concat(WyciagnijDane(htmlDoc, eventsHdr, sLang)).ToList
        If sTabs.Contains("B") AndAlso birthsHdr <> "" Then mBirths = mBirths.Concat(WyciagnijDane(htmlDoc, birthsHdr, sLang)).ToList
        If sTabs.Contains("D") AndAlso deathsHdr <> "" Then mDeaths = mDeaths.Concat(WyciagnijDane(htmlDoc, deathsHdr, sLang)).ToList
    End Sub

    Public Function ExtractLangLinks(sForLangs As String, sPage As String) As IReadOnlyList(Of String)
        Dim lList As New List(Of String)
        If sForLangs Is Nothing Then Return lList
        If String.IsNullOrEmpty(sPage) Then Return lList

        Dim iInd As Integer
        Dim sTmp As String
        Dim aArr As String()   ' było Array, SDK 1803 - nie zna typu dla sLang, zmieniam na String()
        aArr = sForLangs.Split(" "c)

        For Each sLang In aArr
            Dim sLang1 = "https://" & sLang & ".wikipedia"
            iInd = sPage.IndexOf(sLang1, StringComparison.OrdinalIgnoreCase)

            If iInd > 100 Then
                sTmp = sPage.Substring(iInd)
                iInd = sTmp.IndexOf("""", StringComparison.Ordinal)
                sTmp = sTmp.Substring(0, iInd)
                lList.Add(sTmp)
            End If
        Next

        Return lList
    End Function


    Public Class OneRocznica
        Public Property rok As Integer
        Public Property html As String

        Public Sub New(_rok As Integer, _html As String)
            rok = _rok
            html = _html
        End Sub
    End Class

End Module

#Enable Warning CA2007 'Consider calling ConfigureAwait On the awaited task
#Enable Warning CA1707 ' Identifiers should not contain underscores


