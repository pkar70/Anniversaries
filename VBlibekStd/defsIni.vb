﻿
' ponieważ NIE DZIAŁA pod Uno.Android wczytywanie pliku (apk nie jest rozpakowany?),
' to w ten sposób przekazywanie zawartości pliku INI
' wychodzi na to samo, edycja pliku defaults.ini albo defsIni.lib.vb

Public Class IniLikeDefaults

    Public Const sIniContent As String = "
[main]
# Event, Births, Deaths
EnabledTabs=EBD
# languages - two letters as in wikipedia
EnabledLanguages=pl de fr es ru

[debug]
key=value # remark

[app]
; lista z app (bez ustawiania)


[libs]
; lista z pkarmodule
remoteSystemDisabled=false
appFailData=
offline=false
lastPolnocnyTry=
lastPolnocnyOk=

"

End Class
