
' wzięte z Uno.Shared:MainPage.xaml.cs 
' 2022.01.26

#Disable Warning CA1707 ' Identifiers should not contain underscores
#Disable Warning CA2007 'Consider calling ConfigureAwait On the awaited task

Public Module MainPage

    Private mEvents As New HtmlAgilityPack.HtmlDocument()
    Private mBirths As New HtmlAgilityPack.HtmlDocument()
    Private mDeaths As New HtmlAgilityPack.HtmlDocument()

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
        mEvents = New HtmlAgilityPack.HtmlDocument()
        mBirths = New HtmlAgilityPack.HtmlDocument()
        mDeaths = New HtmlAgilityPack.HtmlDocument()
    End Sub

    Public Function GetContentForWebview(sTab As String) As String
        Dim oDoc As HtmlAgilityPack.HtmlNode
        Select Case sTab
            Case "E"
                oDoc = mEvents.DocumentNode
            Case "B"
                oDoc = mBirths.DocumentNode
            Case "D"
                oDoc = mDeaths.DocumentNode
            Case Else
                DumpMessage("unrecognized sTab (" & sTab & ") in GetContentForWebview, using Events")
                oDoc = mEvents.DocumentNode
        End Select

        If oDoc.FirstChild Is Nothing Then Return Nothing

        Return oDoc.OuterHtml

    End Function

    ''' <summary>
    ''' Wycięcie z htmlDoc fragmentu od <h2> zawierającego _sFrom_ (np. "id=wydarzenia") do początku następnego h2
    ''' </summary>
    Private Function WyciagnijSekcjeH2(htmlDoc As HtmlAgilityPack.HtmlDocument, sFrom As String, sLang As String) As HtmlAgilityPack.HtmlDocument
        DumpCurrMethod(sFrom)
        ' wszystkie H2 proszę przeiterować
        ' w każdym z nich, w inner html, sprawdzam istnienie sFrom

        For Each oH2 As HtmlAgilityPack.HtmlNode In htmlDoc.DocumentNode.SelectNodes("//h2")

            If oH2.InnerText.Contains(sFrom) Then
                ' mam to! znaczy odpowiedni H2
                ' sklejaj wszystkie outer aż do następnego H2
                Dim sRet = ""
                Dim oEntry As HtmlAgilityPack.HtmlNode = oH2.NextSibling

                While oEntry IsNot Nothing

                    If oEntry.Name = "h2" Then
                        Dim oRetXml = New HtmlAgilityPack.HtmlDocument()
                        oRetXml.LoadHtml($"<root>{sRet}</root>")   ' oRetXml.LoadHtml("<root>" & sRet & "</root>")
                        'oRetXml.LoadXml(sRet);
                        Return oRetXml
                    End If

                    Dim bSkip = False

                    ' pomijam puste
                    If oEntry.OuterHtml.Trim() = "" Then bSkip = True

                    If sLang = "ru" AndAlso oEntry.InnerHtml.StartsWith("<i>См. также:") Then bSkip = True

                    ' dla Ukrainy takie coś - mają pierwsze <p>, *TODO* tylko pierwsze <p> pomijać
                    If sLang = "uk" AndAlso oEntry.Name = "p" Then bSkip = True
                    If oEntry.Name = "h4" Then bSkip = True ' dla DE
                    If oEntry.Name = "dl" Then bSkip = True ' dla RU
                    If Not bSkip Then sRet += oEntry.OuterHtml.Trim()
                    oEntry = oEntry.NextSibling
                End While
            End If
        Next

        Return Nothing  ' nie było takiego
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
    ''' Wycięcie z htmlDoc fragmentu od <h2> zawierającego _sFrom_ (np. "id=wydarzenia") do początku następnego h2
    ''' Ma usunąć wszystkie niepotrzebne rzeczy.
    ''' jest to robione dla języka sLang (jakby były jakieś różnice)
    ''' </summary></summary>
    Private Function WyciagnijDane(htmlDoc As HtmlAgilityPack.HtmlDocument, sFrom As String, sLang As String) As HtmlAgilityPack.HtmlDocument
        DumpCurrMethod($"WyciagnijDane(htmlDoc, '{sFrom}', '{sLang}'")
        Dim oH2 = WyciagnijSekcjeH2(htmlDoc, sFrom, sLang)
        If oH2 Is Nothing Then Return Nothing

        ' usuwamy obrazki: <div class="thumb tright"> oraz tleft
        ' dla: DE, FR, ES
        UsunElementy(oH2, "div", "div class=""thumb")

        ' usuwamy link do multimedia
        ' dla: PL
        UsunElementy(oH2, "table", "infobox")

        ' nic nie robi dla:
        ' PL: (ale za to MergeSorted h2 świat i PL)
        ' dla EL: nic
        ' mogłoby być wcześniej, ale żaden problem spróbować usunąć
        If sLang = "pl" OrElse sLang = "el" Then Return oH2

        ' dla DE:
        '  w wydarzeniach, H3 do MergeSorted, w pozostałych - do usunięcia
        If sLang = "de" Then
            oH2 = SplitAndSort(oH2, "h3")
        End If


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
        sXml = sXml.Replace("  ", " ")
        sXml = sXml.Replace("  ", " ")
        sXml = sXml.Replace("  ", " ")
        sXml = sXml.Replace("</ul> <ul>", "")
        sXml = sXml.Replace("</ul><ul>", "")
        oH2.LoadHtml(sXml)
        Return oH2
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
        Dim iRok = 0
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
            If Not Integer.TryParse(sTxt.Substring(0, iInd), iRok) Then Debug.Write("Error CInt(" & sTxt.Substring(0, iInd) & ")")
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

        Dim oNodes1 As HtmlAgilityPack.HtmlNodeCollection = oRoot1.ChildNodes ' wewnątrz #document powinien być tylko <root>
        Dim oNodes2 As HtmlAgilityPack.HtmlNodeCollection = oRoot2.ChildNodes

        If oNodes1.Count < 1 Then Return oDom2
        If oNodes2.Count < 1 Then Return oDom1
        If oNodes1.Count > 1 Or oNodes2.Count > 1 Then
            ' coś jest nie tak, powinno być tylko jedno
            DumpMessage("Something is wrong - should be only one item inside #document!")
            Return oDom1
        End If

        oNodes1 = oNodes1.ElementAt(0).ChildNodes ' czyli <root> 
        oNodes2 = oNodes2.ElementAt(0).ChildNodes

        If oNodes1.Count < 1 Then Return oDom2
        If oNodes2.Count < 1 Then Return oDom1
        If oNodes1.Count > 1 Or oNodes2.Count > 1 Then
            ' coś jest nie tak, powinno być tylko jedno 
            DumpMessage("Something is wrong - should be only one item inside <root>!")

            DumpMessage("oNodes1:")
            For Each oItem In oNodes1
                DumpMessage(oItem.Name)
            Next

            DumpMessage("oNodes2:")
            For Each oItem In oNodes2
                DumpMessage(oItem.Name)
            Next

            Return oDom1
        End If

        oNodes1 = oNodes1.ElementAt(0).SelectNodes("li")   ' a w <root><ul> interesują nas <li>
        oNodes2 = oNodes2.ElementAt(0).SelectNodes("li")

        DumpMessage($"MergeSorted, count1={oNodes1.Count}, count2={oNodes2.Count}")

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
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Events", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Births", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Deaths", sLang))
                    'if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                    '    mHolid = mHolid + WyciagnijDane(sTxt, "observances\">Holidays", sUrl);

            Case "de"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Ereignisse", sLang))
                If sTabs.Contains("B") Then mBirths = WyciagnijDane(htmlDoc, "Geboren", sLang)
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Gestorben", sLang))
                    'if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                    '    mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Feier");

            Case "pl"
                If sTabs.Contains("E") Then
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Wydarzenia w Pols", sLang))
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Wydarzenia na świ", sLang))
                End If

                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Urodzili", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Zmarli", sLang))
                    'if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                    '    mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Święta");

            Case "fr"
                If sTabs.Contains("E") Then
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Événements", sLang))
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Arts, culture", sLang))
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Sciences_et", sLang))
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Économie", sLang))
                End If

                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Naissances", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Décès", sLang))
                    'if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                    '    mHolid = mHolid + WyciagnijDane(sTxt, "ns\">Célébrations");

            Case "es"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Acontecimientos", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Nacimientos", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Fallecimientos", sLang))
                    'if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                    '    mHolid = mHolid + WyciagnijDane(sTxt, "s\">Celebraciones");

            Case "ru"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "События", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Родились", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Скончались", sLang))
                    'if (sTabs.IndexOf("H", StringComparison.Ordinal) > -1)
                    '    mHolid = mHolid + WyciagnijDane(sTxt, "id=\"Праздники");

            Case "uk"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Події", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Народились", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Померли", sLang))

            Case "el"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "Γεγονότα", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "Γεννήσεις", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "Θάνατοι", sLang))

            Case "he"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "אירועים", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "נולדו", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "נפטרו", sLang))

            Case "ja"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "できごと", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "誕生日", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "忌日", sLang))

            Case "ar"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "أحداث", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "مواليد", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "وفيات", sLang))

            Case "ka"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "მოვლენები", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "დაბადებულნი", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "გარდაცვლილნი", sLang))

            Case "ko"

                If sTabs.Contains("E") Then
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "사건", sLang))
                    mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "문화", sLang))
                End If

                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "탄생", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "사망", sLang))

            Case "zh"
                If sTabs.Contains("E") Then mEvents = MergeSorted(mEvents, WyciagnijDane(htmlDoc, "大事记", sLang))
                If sTabs.Contains("B") Then mBirths = MergeSorted(mBirths, WyciagnijDane(htmlDoc, "出生", sLang))
                If sTabs.Contains("D") Then mDeaths = MergeSorted(mDeaths, WyciagnijDane(htmlDoc, "逝世", sLang))

            Case Else
                Dim oUnsupported = HtmlAgilityPack.HtmlNode.CreateNode("<h2>Unsupported lang: " & sLang & "</h2>")
                mEvents.DocumentNode.AppendChild(oUnsupported)
                mBirths.DocumentNode.AppendChild(oUnsupported)
                mDeaths.DocumentNode.AppendChild(oUnsupported)

        End Select

        Return sTxt
    End Function

    Public Function ExtractLangLinks(sForLangs As String, sPage As String) As IReadOnlyList(Of String)
        Dim lList As New List(Of String)()     ' SDK 1803 - problem z typem?
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

End Module

#Enable Warning CA2007 'Consider calling ConfigureAwait On the awaited task
#Enable Warning CA1707 ' Identifiers should not contain underscores
