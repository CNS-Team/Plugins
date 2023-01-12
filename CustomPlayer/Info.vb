Imports Microsoft.Xna.Framework

Imports TShockAPI
Imports TShockAPI.DB

Imports VBY.Basic.Extension

Imports CP = CustomPlayer.CustomPlayerPlugin

Public Class CustomPlayer
    Public Name As String
    Public Player As TSPlayer
    Public Group As Group
    Public Permissions, NegatedPermissions As New List(Of String)
    Public Prefix, Suffix As String
    Public ChatColor As Color?
    Public PrefixList, SuffixList As New List(Of TableInfo.Prefix)
    Public Sub New(name As String, player As TSPlayer)
        Me.Name = name
        Me.Player = player
    End Sub
    Public Sub AddPermission(permission As String)
        If permission.StartsWith("!"c) Then
            NegatedPermissions.Add(permission.Substring(1))
        Else
            Permissions.Add(permission)
        End If
    End Sub
    Public Function DelPermission(permission As String) As Boolean
        If permission.StartsWith("!"c) Then
            Return NegatedPermissions.Remove(permission.Substring(1))
        Else
            Return Permissions.Remove(permission)
        End If
    End Function
    Public Function GetTitle(type As String) As String
        If type = "Prefix" Then
            Return Prefix
        Else
            Return Suffix
        End If
    End Function
    Public Sub SetTitle(type As String, value As String)
        If type = "Prefix" Then
            Prefix = value
        Else
            Suffix = value
        End If
    End Sub
    Public Shared Function Read(name As String, player As TSPlayer) As CustomPlayer
        Dim cply As New CustomPlayer(name, player)

        Using playerReader = QueryPlayer(name)
            If playerReader.Read() Then
                If CP.ReadConfig.Root.EnablePermission Then
                    For Each x In playerReader.Reader.GetString(NameOf(TableInfo.PlayerList.Commands)).Split(","c, StringSplitOptions.RemoveEmptyEntries)
                        cply.AddPermission(x)
                    Next
                End If

                If CP.ReadConfig.Root.EnableGroup Then
                    Dim groupName = playerReader.Reader.GetString(NameOf(TableInfo.PlayerList.Group))
                    Dim fgroup As Group = Nothing
                    For Each g In CustomPlayerPluginHelpers.Groups
                        If g.Name = groupName Then
                            fgroup = g
                            Exit For
                        End If
                    Next
                    If fgroup Is Nothing Then
                        player.SendInfoMessage("custom组未找到")
                        TShock.Log.Error($"custom组{groupName}未找到")
                    Else
                        cply.Group = fgroup
                    End If
                End If

                If Not playerReader.Reader.IsDBNull(2) Then
                    Dim colorStrs = playerReader.Reader.GetString(NameOf(TableInfo.PlayerList.ChatColor)).Split(","c)
                    cply.ChatColor = New Color(Integer.Parse(colorStrs(0)), Integer.Parse(colorStrs(1)), Integer.Parse(colorStrs(3)))
                End If
            End If
        End Using
        If CP.ReadConfig.Root.EnablePermission Then
            Using timeoutReader = QueryReader("select Value,Type,StartTime,EndTime,DurationText from ExpirationInfo where Name = @0", name)
                timeoutReader.Reader.ForEach(
                    Sub(x)
                        With x
                            Dim tobj = New TimeOutObject(name,
                                                         .GetString(NameOf(TableInfo.ExpirationInfo.Value)),
                                                         .GetString(NameOf(TableInfo.ExpirationInfo.Type)),
                                                         .GetDateTime(NameOf(TableInfo.ExpirationInfo.StartTime)),
                                                         .GetDateTime(NameOf(TableInfo.ExpirationInfo.EndTime)),
                                                         .GetString(NameOf(TableInfo.ExpirationInfo.DurationText)))
                            'TSPlayer.Server.SendInfoMessage("tobj create")
                            If tobj.DurationText = "-1" OrElse tobj.EndTime > Now Then
                                'TSPlayer.Server.SendInfoMessage("tobj not timeout,type is " & tobj.Type)
                                If tobj.Type = NameOf(Permission) Then
                                    CustomPlayerPluginHelpers.TimeOutList.Add(tobj)
                                    'TSPlayer.Server.SendInfoMessage("tobj is permission")
                                    cply.AddPermission(tobj.Value)
                                End If
                            Else
                                player.SendInfoMessage($"权限:{tobj.Value} 已过期")
                                tobj.Delete()
                            End If
                        End With
                    End Sub)
            End Using
        End If

        If CP.ReadConfig.Root.EnablePrefix Then
            Using prefixReader = PrefixQuery(name)
                prefixReader.Reader.ForEach(
                Sub(x)
                    With x
                        Dim tobj = New TimeOutObject(name,
                                                     .GetString(NameOf(TableInfo.Prefix.Value)),
                                                     NameOf(Prefix),
                                                     .GetDateTime(NameOf(TableInfo.Prefix.StartTime)),
                                                     .GetDateTime(NameOf(TableInfo.Prefix.EndTime)),
                                                     .GetString(NameOf(TableInfo.ExpirationInfo.DurationText)),
                                                     .GetInt32(NameOf(TableInfo.Prefix.Id)))
                        If tobj.DurationText = "-1" OrElse tobj.EndTime > Now Then
                            cply.PrefixList.Add(New TableInfo.Prefix(name, tobj.Id, tobj.Value, tobj.StartTime, tobj.EndTime, tobj.DurationText))
                            CustomPlayerPluginHelpers.TimeOutList.Add(tobj)
                        Else
                            player.SendInfoMessage($"前缀:{tobj.Value} 已过期")
                            tobj.Delete()
                        End If
                    End With
                End Sub)
            End Using
        End If

        If CP.ReadConfig.Root.EnableSuffix Then
            Using suffixReader = SuffixQuery(name)
                suffixReader.Reader.ForEach(
                Sub(x)
                    With x
                        Dim tobj = New TimeOutObject(name,
                                                     .GetString(NameOf(TableInfo.Suffix.Value)),
                                                     NameOf(Suffix),
                                                     .GetDateTime(NameOf(TableInfo.Suffix.StartTime)),
                                                     .GetDateTime(NameOf(TableInfo.Suffix.EndTime)),
                                                     .GetString(NameOf(TableInfo.Suffix.DurationText)),
                                                     .GetInt32(NameOf(TableInfo.Suffix.Id)))
                        If tobj.DurationText = "-1" OrElse tobj.EndTime > Now Then
                            cply.SuffixList.Add(New TableInfo.Prefix(name, tobj.Id, tobj.Value, tobj.StartTime, tobj.EndTime, tobj.DurationText))
                            CustomPlayerPluginHelpers.TimeOutList.Add(tobj)
                        Else
                            player.SendInfoMessage($"后缀:{tobj.Value} 已过期")
                            tobj.Delete()
                        End If
                    End With
                End Sub)
            End Using
        End If

        Using usingReader = QueryReader("select PrefixId,SuffixId from Useing where Name = @0 and ServerId = @1", name, CP.ReadConfig.Root.ServerId)
            If usingReader.Read() Then
                cply.Prefix = If(cply.PrefixList.Find(Function(x) x.Id = usingReader.Reader.GetInt32(NameOf(TableInfo.Useing.PrefixId)))?.Value, Nothing)
                cply.Suffix = If(cply.SuffixList.Find(Function(x) x.Id = usingReader.Reader.GetInt32(NameOf(TableInfo.Useing.SuffixId)))?.Value, Nothing)
            Else
                Query("insert into Useing(Name,ServerId,PrefixId,SuffixId) values(@0,@1,-1,-1)", name, CP.ReadConfig.Root.ServerId)
                TSPlayer.Server.SendInfoMessage("insert data")
            End If
        End Using
        'TSPlayer.Server.SendInfoMessage("return customplayer")
        Return cply
    End Function
    'Public Overloads Shared Narrowing Operator CType(value As CustomPlayer) As Boolean
    '   Return value Is Nothing
    'End Operator
End Class
Public Class TimeOutObject
    Public Name, Value, Type, DurationText As String
    Public StartTime, EndTime As Date
    Public TimeOuted As Boolean = False
    Public Id As Integer
    Public Sub New(name As String, value As String, type As String, startTime As Date, endTime As Date, durationText As String, Optional id As Integer = -1)
        Me.Name = name
        Me.Value = value
        Me.Type = type
        Me.StartTime = startTime
        Me.EndTime = endTime
        Me.DurationText = durationText
        Me.Id = id
    End Sub
    Public ReadOnly Property RemainTime As String
        Get
            If DurationText = "-1" Then
                Return "无限"
            Else
                Dim remainingTime = EndTime - Now
                If remainingTime < TimeSpan.Zero Then
                    Return "已过期"
                Else
                    Return remainingTime.ToString("d\.hh\:mm\:ss")
                End If
            End If
        End Get
    End Property
    Public Sub Delete()
        Select Case Type
            Case NameOf(Permission)
                Query("delete from ExpirationInfo where Name = @0 AND Value = @1 AND Type = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", Name, Value, Type, StartTime, EndTime, DurationText)
                TSPlayer.Server.SendInfoMessage($"delete permission:{Value}")
            Case NameOf(CustomPlayer.Prefix)
                Query("delete from Prefix where Name = @0 AND Value = @1 AND ID = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", Name, Value, Id, StartTime, EndTime, DurationText)
                TSPlayer.Server.SendInfoMessage($"delete prefix:{Value}")
            Case NameOf(CustomPlayer.Suffix)
                Query("delete from Suffix where Name = @0 AND Value = @1 AND ID = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", Name, Value, Id, StartTime, EndTime, DurationText)
                TSPlayer.Server.SendInfoMessage($"delete suffix:{Value}")
        End Select
    End Sub
End Class