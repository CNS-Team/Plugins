Imports System.Runtime.CompilerServices

Imports TShockAPI
Imports TShockAPI.DB

Imports GetText

Imports VBY.Basic.Extension
Module Utils
	Public GetStringMethod As Func(Of FormattableStringAdapter, Object(), String)
	Public Function GetString(text As FormattableStringAdapter, ParamArray args As Object())
		Return GetStringMethod(text, args)
	End Function
	<Extension> Function NullOrEmptyReturn(args As String, value As String) As String
		Return If(String.IsNullOrEmpty(args), value, args)
	End Function
	<Extension> Function GetPlayer(player As TSPlayer) As CustomPlayer
		Return CustomPlayerPluginHelpers.Players(player.Index)
	End Function
	Public Function FindPlayer(name As String) As CustomPlayer
		Return CustomPlayerPluginHelpers.Players.Find(Function(x) x?.Name = name)
	End Function
	Function Query(commandText As String, ParamArray args As Object()) As Integer
		Return DbExt.Query(CustomPlayerPluginHelpers.DB, commandText, args)
	End Function
	Function QueryReader(commandText As String, ParamArray args As Object()) As QueryResult
		Return DbExt.QueryReader(CustomPlayerPluginHelpers.DB, commandText, args)
	End Function
	Function QueryPlayer(name As String) As QueryResult
		Return QueryReader("select Commands,`Group`,ChatColor from PlayerList where Name = @0", name)
	End Function
	Public Function TitleQuery(type As String, name As String) As QueryResult
		Return QueryReader($"select Id,Value,StartTime,EndTime,DurationText from {type} where Name  = @0", name)
	End Function
	Public Function TitleQuery(type As String, name As String, titleId As Integer) As QueryResult
		Return QueryReader($"select Value,StartTime,EndTime,DurationText from {type} where Name  = @0 AND Id = @1", name, titleId)
	End Function
	Public Function PrefixQuery(name As String) As QueryResult
		Return TitleQuery("Prefix", name)
	End Function
	Public Function SuffixQuery(name As String) As QueryResult
		Return TitleQuery("Suffix", name)
	End Function
End Module
