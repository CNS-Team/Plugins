Imports TShockAPI

Imports VBY.Basic.Command
Imports VBY.Basic.Extension

Imports CustomPlayer.TableInfo
Partial Public Class CustomPlayerPlugin
    Private Shared Function TimeParse(forever As Boolean, time As String, ByRef startTime As Date, ByRef endTime As Date, ByRef addTime As TimeSpan, player As TSPlayer) As Boolean
        If forever Then
            endTime = startTime
        Else
            If time = "get" Then
                If TestObject.Time.HasValue Then
                    addTime = TestObject.Time.Value
                    endTime = startTime.Add(TestObject.Time.Value)
                Else
                    player.SendErrorMessage($"Time.Test 未设置过值")
                    Return True
                End If
            Else
                If TimeSpan.TryParse(time, addTime) Then
                    endTime = startTime.Add(addTime)
                Else
                    player.SendErrorMessage($"转换 {time} 为 TimeSpan 失败")
                    Return True
                End If
            End If
        End If
        Return False
    End Function
    Private Shared Function TitleParse(type As String, ByRef title As String, player As TSPlayer) As Boolean
        If title = "get" Then
            Select Case type
                Case NameOf(CustomPlayer.Prefix)
                    If String.IsNullOrEmpty(TestObject.Prefix) Then
                        player.SendErrorMessage("Prefix.Test 未设置有效值")
                        Return True
                    Else
                        title = TestObject.Prefix
                    End If
                Case NameOf(CustomPlayer.Suffix)
                    If String.IsNullOrEmpty(TestObject.Suffix) Then
                        player.SendErrorMessage("Suffix.Test 未设置有效值")
                        Return True
                    Else
                        title = TestObject.Suffix
                    End If
            End Select
        End If
        Return False
    End Function
    Private Shared Sub TitleList(ByRef args As SubCmdArgs, type As String)
        Dim player = args.commandArgs.Player
        Dim cply = player.GetPlayer()
        Dim list = If(type = "前缀", cply.PrefixList, cply.SuffixList)
        player.SendInfoMessage($"你的所有{type}({list.Count})")
        For Each title In list
            With title
                player.SendInfoMessage($"ID:{ .Id} 内容:{ .Value} 剩余时间:{If(.DurationText = "-1", "永久", (Now - .EndTime).ToString())}")
            End With
        Next
    End Sub
    Private Shared Sub TitleWear(ByRef args As SubCmdArgs, type As String)
        Dim player = args.commandArgs.Player
        Dim id As Integer
        If Not Integer.TryParse(args.Parameters(0), id) Then
            player.SendErrorMessage("转换Id失败")
            Return
        End If

        Dim isPrefix = type = NameOf(CustomPlayer.Prefix)
        Dim typeChinese = If(isPrefix, "前缀", "后缀")
        Dim clear = id = -1
        Dim cply = player.GetPlayer()
        Dim list = If(isPrefix, cply.PrefixList, cply.SuffixList)
        If clear Then
            If isPrefix Then
                cply.Prefix = Nothing
            Else
                cply.Suffix = Nothing
            End If
            player.SendSuccessMessage($"佩戴{typeChinese}已清除")
        Else
            Dim fTitle = list.Find(Function(x) x.Id = id)
            If fTitle Is Nothing Then
                player.SendInfoMessage($"{typeChinese}ID:{id} 未找到")
                Return
            Else
                If isPrefix Then
                    cply.Prefix = fTitle.Value
                Else
                    cply.Suffix = fTitle.Value
                End If
                player.SendSuccessMessage($"{typeChinese}ID:{id} 已佩戴")
            End If
        End If
        Query($"update Useing set {type}Id = @0 where ServerId = @1", id, ReadConfig.Root.ServerId)
    End Sub
    Private Shared Sub CtlTitleList(ByRef args As SubCmdArgs, type As String)
        Dim player = args.commandArgs.Player
        Dim name = args.Parameters(0)
        Dim isPrefix As Boolean = type = NameOf(CustomPlayer.Prefix)
        Dim typeChinese = If(isPrefix, "前缀", "后缀")
        Using reader = If(isPrefix, PrefixQuery(name), SuffixQuery(name))
            If reader.Read() Then
                player.SendInfoMessage($"玩家:{name} 的{typeChinese}")
                reader.Reader.DoForEach(Sub(x) player.SendInfoMessage($"{typeChinese}:{x.GetString("Value")} Id:{x.GetInt32("Id")} 剩余时间:{If(x.GetString("DurationText") = "-1", "永久", (x.GetDateTime("EndTime") - Now).ToString("d\.hh\:mm\:ss"))}"))
            Else
                player.SendInfoMessage($"没有找到此玩家的{typeChinese}")
            End If
        End Using
    End Sub
    Private Shared Sub CtlTitleAdd(ByRef args As SubCmdArgs, type As String)
        Dim name = args.Parameters(0), title = args.Parameters(1).Replace(";"c, ""), time = args.Parameters(2)
        Dim player = args.commandArgs.Player
        Dim startTime = Now, endTime As Date, addTime As TimeSpan
        Dim forever = time = "-1"
        Dim addId = 0
        Dim isPrefix = type = NameOf(CustomPlayer.Prefix)
        Dim typeChinese = If(isPrefix, "前缀", "后缀")

        If TimeParse(forever, time, startTime, endTime, addTime, player) Then Return
        If TitleParse(type, title, player) Then Return

        Dim cply = FindPlayer(name)
        If cply IsNot Nothing Then
            If forever Then
                cply.Player.SendInfoMessage($"你已获得永久{typeChinese}:{title}")
            Else
                cply.Player.SendInfoMessage($"你获得{typeChinese}:{title}")
            End If

            CustomPlayerPluginHelpers.TimeOutList.Add(New TimeOutObject(name, title, type, startTime, endTime, time))
            Dim list = If(isPrefix, cply.PrefixList, cply.SuffixList)
            list.Add(New Prefix(name, addId, title, startTime, startTime, time))
        End If

        Using maxReader = QueryReader($"select max(Id) from {type} where Name = @0", name)
            If maxReader.Read() AndAlso Not maxReader.Reader.IsDBNull(0) Then addId = maxReader.Reader.GetInt32(0) + 1
        End Using
        Query($"insert into {type}(Name,Id,Value,StartTime,EndTime,DurationText) values(@0,@1,@2,@3,@4,@5)", name, addId, title, startTime, endTime, time)
        If forever Then
            player.SendInfoMessage($"添加成功 玩家:{name} {typeChinese}:{title} Id:{addId} 持续时间:永久")
        Else
            player.SendInfoMessage($"添加成功 玩家:{name} {typeChinese}:{title} Id:{addId} 起始时间:{startTime} 结束时间:{endTime} 持续时间:{addTime}")
        End If
    End Sub
    Private Shared Sub CtlTitleDel(ByRef args As SubCmdArgs, type As String)
        Dim name = args.Parameters(0), delValue = args.Parameters(1)
        Dim player = args.commandArgs.Player
        Dim isPrefix = type = NameOf(CustomPlayer.Prefix)
        Dim typeChinese = If(isPrefix, "前缀", "后缀")
        Dim delId As Integer
        If Integer.TryParse(delValue, delId) AndAlso Query($"delete from {type} where Name = @0 AND Id = @1", name, delId) > 0 Then
            player.SendSuccessMessage($"删除玩家[{name}] {typeChinese}Id:{delId} 成功")
        Else
            If Query($"delete from {type} where Name = @0 AND Value = @1", name, delValue) > 0 Then
                player.SendSuccessMessage($"删除玩家:[{name}] {typeChinese}Value:{delValue} 成功")
            Else
                player.SendErrorMessage("删除失败,没有找到")
            End If
        End If
    End Sub
    Private Shared Sub CtlTitleTest(ByRef args As SubCmdArgs, type As String)
        Dim cply = args.commandArgs.Player.GetPlayer()
        If type = "Prefix" Then
            cply.Prefix = args.Parameters(0)
            TestObject.Prefix = args.Parameters(0)
        Else
            cply.Suffix = args.Parameters(0)
            TestObject.Suffix = args.Parameters(0)
        End If
    End Sub
    Private Shared Sub CtlTitleWear(ByRef args As SubCmdArgs, type As String)
        Dim player = args.commandArgs.Player
        Dim name = args.Parameters(0)
        Dim isPrefix = type = NameOf(CustomPlayer.Prefix)
        Dim typeChinese = If(isPrefix, "前缀", "后缀")
        Dim titleId, serverId As Integer
        If Not Integer.TryParse(args.Parameters(1), titleId) Then
            player.SendErrorMessage($"转换{typeChinese}Id失败")
            Return
        End If
        If Not Integer.TryParse(args.Parameters(2), serverId) Then
            player.SendErrorMessage("转换ServerId失败")
            Return
        End If

        Using reader = TitleQuery(type, name, titleId)
            If reader.Read() Then
                Using usingReader = QueryReader("select PrefixId,SuffixId from Useing where Name = @0 and ServerId = @1", name, serverId)
                    If usingReader.Read() Then
                        Query($"update Useing set {type}Id = @0 where ServerId = @1", titleId, serverId)
                    Else
                        Query($"insert into Useing(Name,ServerId,{type}Id) values(@0,@1,@2)", name, serverId, titleId)
                    End If
                End Using
                player.SendSuccessMessage($"已为玩家:{name} 佩戴{typeChinese}Id:{titleId} 到服务器:{serverId}")
            Else
                player.SendErrorMessage($"玩家:{name} {typeChinese}Id:{titleId} 未找到")
            End If
        End Using
    End Sub
End Class