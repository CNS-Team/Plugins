Imports TShockAPI
Imports TShockAPI.DB

Imports VBY.Basic.Command
Imports VBY.Basic.Extension

Partial Public Class CustomPlayerPlugin
#Region "Cmd"
    Private Sub Cmd(args As CommandArgs)
        If ReadConfig.Normal Then
            MainCommand.Run(args)
        Else
            args.Player.SendErrorMessage("配置读取有误,请配置正确再使用此命令")
        End If
    End Sub
    Private Sub CmdPermissionList(args As SubCmdArgs)
        Dim player = args.commandArgs.Player
        Dim permissions = CustomPlayerPluginHelpers.TimeOutList.Where(Function(x) x.Type = NameOf(Permission) AndAlso x.Name = player.Name)
        If Not permissions.Any Then player.SendInfoMessage("你当前没有权限")
        For Each permisssionInfo As TimeOutObject In permissions
            player.SendInfoMessage($"权限:{permisssionInfo.Value} 剩余时间:{ permisssionInfo.RemainTime}")
            'Console.WriteLine(New StackTrace())
        Next
    End Sub
    Private Sub CmdReload(args As SubCmdArgs)
        OnPlayerLogout(New Hooks.PlayerLogoutEventArgs(args.commandArgs.Player))
        OnPlayerPostLogin(New Hooks.PlayerPostLoginEventArgs(args.commandArgs.Player))
        args.commandArgs.Player.SendSuccessMessage("重载成功")
    End Sub
#End Region
#Region "CmdCtl"

    Private Sub CmdCtl(args As CommandArgs)
        MainCtlCommand.Run(args)
    End Sub
    Private Sub CmdCtlGroupSet(args As SubCmdArgs)
        Dim player = args.commandArgs.Player
        Dim name = args.Parameters(0)
        Dim groupName = args.Parameters(1)
        Dim clear = groupName = "null"
        Using reader = QueryPlayer(name)
            If clear Then
                If Not reader.Read() Then
                    player.SendInfoMessage($"玩家:{name} 的数据不存在,不用清除")
                    Return
                End If
            Else
                If Not CustomPlayerPluginHelpers.Groups.GroupExists(groupName) Then
                    player.SendErrorMessage($"组:{groupName} 不存在,如确定存在,请执行{Commands.Specifier}{AddCommands(1).Names} {MainCtlCommand("Reload").Names(0)}")
                    Return
                End If
            End If
            If Query("update PlayerList set `Group` = @0 where Name = @1", groupName, name) = 1 Then
                player.SendSuccessMessage($"已把玩家:{name} 的组设置为 {If(clear, "空", groupName)}")
                Dim cply = FindPlayer(name)
                If cply IsNot Nothing Then
                    cply.Group = If(CustomPlayerPluginHelpers.Groups.groups.Find(Function(x) x.Name = groupName), cply.Group)
                End If
            Else
                player.SendInfoMessage("似乎设置出错了")
            End If
        End Using
    End Sub
    Private Shared Sub Group(args As CommandArgs)
        Dim subCmd As String = If(args.Parameters.Count = 0, "help", args.Parameters(0).ToLower())

        Select Case subCmd
            Case "add"

                If args.Parameters.Count < 2 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group add <group name> [permissions].", Commands.Specifier))
                    Return
                End If

                Dim groupName As String = args.Parameters(1)
                args.Parameters.RemoveRange(0, 2)
                Dim permissions As String = String.Join(",", args.Parameters)

                Try
                    CustomPlayerPluginHelpers.Groups.AddGroup(groupName, Nothing, permissions, TShockAPI.Group.defaultChatColor)
                    args.Player.SendSuccessMessage(GetString($"Group {groupName} was added successfully."))
                Catch ex As GroupExistsException
                    args.Player.SendErrorMessage(GetString("A group with the same name already exists."))
                Catch ex As GroupManagerException
                    args.Player.SendErrorMessage(ex.ToString())
                End Try

                Return
            Case "addperm"

                If args.Parameters.Count < 3 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group addperm <group name> <permissions...>.", Commands.Specifier))
                    Return
                End If

                Dim groupName As String = args.Parameters(1)
                args.Parameters.RemoveRange(0, 2)

                If groupName = "*" Then
                    For Each g In CustomPlayerPluginHelpers.Groups
                        CustomPlayerPluginHelpers.Groups.AddPermissions(g.Name, args.Parameters)
                    Next
                    args.Player.SendSuccessMessage(GetString("The permissions have been added to all of the groups in the system."))
                    Return
                End If

                Try
                    Dim response As String = CustomPlayerPluginHelpers.Groups.AddPermissions(groupName, args.Parameters)

                    If response.Length > 0 Then
                        args.Player.SendSuccessMessage(response)
                    End If

                    Return
                Catch ex As GroupManagerException
                    args.Player.SendErrorMessage(ex.ToString())
                End Try

                Return
            Case "help"
                Dim pageNumber As Integer
                If Not PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, pageNumber) Then Return
                Dim lines = New List(Of String) From {
                GetString("add <name> <permissions...> - Adds a new group."),
                GetString("addperm <group> <permissions...> - Adds permissions to a group."),
                GetString("color <group> <rrr,ggg,bbb> - Changes a group's chat color."),
                GetString("rename <group> <new name> - Changes a group's name."),
                GetString("del <group> - Deletes a group."),
                GetString("delperm <group> <permissions...> - Removes permissions from a group."),
                GetString("list [page] - Lists groups."),
                GetString("listperm <group> [page] - Lists a group's permissions."),
                GetString("parent <group> <parent group> - Changes a group's parent group."),
                GetString("prefix <group> <prefix> - Changes a group's prefix."),
                GetString("suffix <group> <suffix> - Changes a group's suffix.")
            }
                PaginationTools.SendPage(args.Player, pageNumber, lines, New PaginationTools.Settings With {
                .HeaderFormat = GetString("Group Sub-Commands ({{0}}/{{1}}):"),
                .FooterFormat = GetString("Type {0}group help {{0}} for more sub-commands.", Commands.Specifier)
            })
                Return
            Case "parent"

                If args.Parameters.Count < 2 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group parent <group name> [new parent group name].", Commands.Specifier))
                    Return
                End If

                Dim groupName As String = args.Parameters(1)
                Dim group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName)
                If group Is Nothing Then
                    args.Player.SendErrorMessage(GetString("No such group ""{0}"".", groupName))
                    Return
                End If

                If args.Parameters.Count > 2 Then
                    Dim newParentGroupName As String = String.Join(" ", args.Parameters.Skip(2))

                    If Not String.IsNullOrWhiteSpace(newParentGroupName) AndAlso Not CustomPlayerPluginHelpers.Groups.GroupExists(newParentGroupName) Then
                        args.Player.SendErrorMessage(GetString("No such group ""{0}"".", newParentGroupName))
                        Return
                    End If

                    Try
                        CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, newParentGroupName, group.StrPermissions, group.ChatColor, group.Suffix, group.Prefix)

                        If Not String.IsNullOrWhiteSpace(newParentGroupName) Then
                            args.Player.SendSuccessMessage(GetString("Parent of group ""{0}"" set to ""{1}"".", groupName, newParentGroupName))
                        Else
                            args.Player.SendSuccessMessage(GetString("Removed parent of group ""{0}"".", groupName))
                        End If

                    Catch ex As GroupManagerException
                        args.Player.SendErrorMessage(ex.Message)
                    End Try
                Else

                    If group.Parent IsNot Nothing Then
                        args.Player.SendSuccessMessage(GetString("Parent of ""{0}"" is ""{1}"".", group.Name, group.Parent.Name))
                    Else
                        args.Player.SendSuccessMessage(GetString("Group ""{0}"" has no parent.", group.Name))
                    End If
                End If

                Return
            Case "suffix"

                If args.Parameters.Count < 2 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group suffix <group name> [new suffix].", Commands.Specifier))
                    Return
                End If

                Dim groupName As String = args.Parameters(1)
                Dim group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName)
                If group Is Nothing Then
                    args.Player.SendErrorMessage(GetString("No such group ""{0}"".", groupName))
                    Return
                End If

                If args.Parameters.Count > 2 Then
                    Dim newSuffix As String = String.Join(" ", args.Parameters.Skip(2))

                    Try
                        CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, group.ParentName, group.StrPermissions, group.ChatColor, newSuffix, group.Prefix)

                        If Not String.IsNullOrWhiteSpace(newSuffix) Then
                            args.Player.SendSuccessMessage(GetString("Suffix of group ""{0}"" set to ""{1}"".", groupName, newSuffix))
                        Else
                            args.Player.SendSuccessMessage(GetString("Removed suffix of group ""{0}"".", groupName))
                        End If

                    Catch ex As GroupManagerException
                        args.Player.SendErrorMessage(ex.Message)
                    End Try
                Else

                    If Not String.IsNullOrWhiteSpace(group.Suffix) Then
                        args.Player.SendSuccessMessage(GetString("Suffix of ""{0}"" is ""{1}"".", group.Name, group.Suffix))
                    Else
                        args.Player.SendSuccessMessage(GetString("Group ""{0}"" has no suffix.", group.Name))
                    End If
                End If

                Return
            Case "prefix"

                If args.Parameters.Count < 2 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group prefix <group name> [new prefix].", Commands.Specifier))
                    Return
                End If

                Dim groupName As String = args.Parameters(1)
                Dim group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName)
                If group Is Nothing Then
                    args.Player.SendErrorMessage(GetString("No such group ""{0}"".", groupName))
                    Return
                End If

                If args.Parameters.Count > 2 Then
                    Dim newPrefix As String = String.Join(" ", args.Parameters.Skip(2))

                    Try
                        CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, group.ParentName, group.StrPermissions, group.ChatColor, group.Suffix, newPrefix)

                        If Not String.IsNullOrWhiteSpace(newPrefix) Then
                            args.Player.SendSuccessMessage(GetString("Prefix of group ""{0}"" set to ""{1}"".", groupName, newPrefix))
                        Else
                            args.Player.SendSuccessMessage(GetString("Removed prefix of group ""{0}"".", groupName))
                        End If

                    Catch ex As GroupManagerException
                        args.Player.SendErrorMessage(ex.Message)
                    End Try
                Else

                    If Not String.IsNullOrWhiteSpace(group.Prefix) Then
                        args.Player.SendSuccessMessage(GetString("Prefix of ""{0}"" is ""{1}"".", group.Name, group.Prefix))
                    Else
                        args.Player.SendSuccessMessage(GetString("Group ""{0}"" has no prefix.", group.Name))
                    End If
                End If

                Return
            Case "color"

                If args.Parameters.Count < 2 OrElse args.Parameters.Count > 3 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group color <group name> [new color(000,000,000)].", Commands.Specifier))
                    Return
                End If

                Dim groupName As String = args.Parameters(1)
                Dim group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName)
                If group Is Nothing Then
                    args.Player.SendErrorMessage(GetString("No such group ""{0}"".", groupName))
                    Return
                End If

                If args.Parameters.Count = 3 Then
                    Dim newColor As String = args.Parameters(2)
                    Dim parts As String() = newColor.Split(","c)
                    Dim r As Byte
                    Dim g As Byte
                    Dim b As Byte

                    If parts.Length = 3 AndAlso Byte.TryParse(parts(0), r) AndAlso Byte.TryParse(parts(1), g) AndAlso Byte.TryParse(parts(2), b) Then

                        Try
                            CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, group.ParentName, group.StrPermissions, newColor, group.Suffix, group.Prefix)
                            args.Player.SendSuccessMessage(GetString("Chat color for group ""{0}"" set to ""{1}"".", groupName, newColor))
                        Catch ex As GroupManagerException
                            args.Player.SendErrorMessage(ex.Message)
                        End Try
                    Else
                        args.Player.SendErrorMessage(GetString("Invalid syntax for color, expected ""rrr,ggg,bbb""."))
                    End If
                Else
                    args.Player.SendSuccessMessage(GetString("Chat color for ""{0}"" is ""{1}"".", group.Name, group.ChatColor))
                End If

                Return
            Case "rename"

                If args.Parameters.Count <> 3 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group rename <group> <new name>.", Commands.Specifier))
                    Return
                End If

                Dim group As String = args.Parameters(1)
                Dim newName As String = args.Parameters(2)

                Try
                    Dim response As String = CustomPlayerPluginHelpers.Groups.RenameGroup(group, newName)
                    args.Player.SendSuccessMessage(response)
                Catch ex As GroupManagerException
                    args.Player.SendErrorMessage(ex.Message)
                End Try

                Return
            Case "del"

                If args.Parameters.Count <> 2 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group del <group name>.", Commands.Specifier))
                    Return
                End If

                Try
                    Dim response As String = CustomPlayerPluginHelpers.Groups.DeleteGroup(args.Parameters(1), True)

                    If response.Length > 0 Then
                        args.Player.SendSuccessMessage(response)
                    End If

                Catch ex As GroupManagerException
                    args.Player.SendErrorMessage(ex.Message)
                End Try

                Return
            Case "delperm"

                If args.Parameters.Count < 3 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group delperm <group name> <permissions...>.", Commands.Specifier))
                    Return
                End If

                Dim groupName As String = args.Parameters(1)
                args.Parameters.RemoveRange(0, 2)

                If groupName = "*" Then
                    For Each g In CustomPlayerPluginHelpers.Groups
                        CustomPlayerPluginHelpers.Groups.DeletePermissions(g.Name, args.Parameters)
                    Next
                    args.Player.SendSuccessMessage(GetString("The permissions have been removed from all of the groups in the system."))
                    Return
                End If

                Try
                    Dim response As String = CustomPlayerPluginHelpers.Groups.DeletePermissions(groupName, args.Parameters)

                    If response.Length > 0 Then
                        args.Player.SendSuccessMessage(response)
                    End If

                    Return
                Catch ex As GroupManagerException
                    args.Player.SendErrorMessage(ex.Message)
                End Try

                Return
            Case "list"
                Dim pageNumber As Integer
                If Not PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, pageNumber) Then Return
                Dim groupNames = From grp In CustomPlayerPluginHelpers.Groups.groups Select grp.Name
                PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(groupNames), New PaginationTools.Settings With {
                .HeaderFormat = GetString("Groups ({{0}}/{{1}}):"),
                .FooterFormat = GetString("Type {0}group list {{0}} for more.", Commands.Specifier)
            })
                Return
            Case "listperm"

                If args.Parameters.Count = 1 Then
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group listperm <group name> [page].", Commands.Specifier))
                    Return
                End If

                Dim pageNumber As Integer
                If Not PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, pageNumber) Then Return

                If Not CustomPlayerPluginHelpers.Groups.GroupExists(args.Parameters(1)) Then
                    args.Player.SendErrorMessage(GetString("Invalid group."))
                    Return
                End If

                Dim grp = CustomPlayerPluginHelpers.Groups.GetGroupByName(args.Parameters(1))
                Dim permissions As List(Of String) = grp.TotalPermissions
                PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(permissions), New PaginationTools.Settings With {
                .HeaderFormat = GetString("Permissions for {0} ({{0}}/{{1}}):", grp.Name),
                .FooterFormat = GetString("Type {0}group listperm {1} {{0}} for more.", Commands.Specifier, grp.Name),
                .NothingToDisplayString = GetString($"There are currently no permissions for {grp.Name}.")
            })
                Return
            Case Else
                args.Player.SendErrorMessage(GetString("Invalid subcommand! Type {0}group help for more information on valid commands.", Commands.Specifier))
                Return
        End Select
    End Sub
    Private Sub CmdCtlPermissionAdd(args As SubCmdArgs)
        Dim name = args.Parameters(0)
        Dim addPermission = args.Parameters(1)
        Dim time = args.Parameters(2)
        Dim player = args.commandArgs.Player
        Dim startTime = Now
        Dim endTime As Date
        Dim addTime As TimeSpan
        Dim forever = time = "-1"

        If TimeParse(forever, time, startTime, endTime, addTime, player) Then Return

        Dim cply = FindPlayer(name)
        If cply IsNot Nothing Then
            If forever Then
                cply.Player.SendInfoMessage($"你已获得永久权限:{addPermission}")
            Else
                cply.Player.SendInfoMessage("你获得权限:" & addPermission)
            End If

            CustomPlayerPluginHelpers.TimeOutList.Add(New TimeOutObject(name, addPermission, NameOf(Permission), startTime, endTime, time))
            cply.AddPermission(addPermission)
        End If

        Query("insert into ExpirationInfo(Name,Value,Type,StartTime,EndTime,DurationText) values(@0,@1,@2,@3,@4,@5)", name, addPermission, NameOf(Permission), startTime, endTime, time)
        If forever Then
            player.SendInfoMessage($"添加成功 玩家:{name} 权限:{addPermission} 持续时间:永久")
        Else
            player.SendInfoMessage($"添加成功 玩家:{name} 权限:{addPermission} 起始时间:{startTime} 结束时间:{endTime} 持续时间:{addTime}")
        End If
    End Sub
    Private Sub CmdCtlPermissionDel(args As SubCmdArgs)
        Dim player = args.commandArgs.Player
        Dim name = args.Parameters(0)
        Dim delPermission = args.Parameters(1)

        Dim delCount = Query("delete from ExpirationInfo where Name = @0 AND Value = @1", name, delPermission)
        If delCount = 0 Then
            player.SendInfoMessage($"未在数据库找到 玩家:{name} 的权限:{delPermission}")
        Else
            player.SendSuccessMessage($"删除成功")
            Dim cply = FindPlayer(name)
            If cply IsNot Nothing Then
                CustomPlayerPluginHelpers.TimeOutList.RemoveAll(Function(x) x.Name = name AndAlso x.Value = delPermission)
                cply.DelPermission(delPermission)
                cply.Player.SendInfoMessage($"你的权限:{delPermission} 已被管理员提前删除")
            End If
        End If
    End Sub
    Private Sub CmdCtlPermissionList(args As SubCmdArgs)
        Dim player = args.commandArgs.Player
        Dim name = args.Parameters(0)
        Using reader = QueryReader("select Value,EndTime,DurationText from ExpirationInfo where name = @0 AND Type = @1", name, NameOf(Permission))
            If reader.Read() Then
                player.SendInfoMessage($"玩家:{name} 的权限")
                reader.Reader.DoForEach(
                    Sub(x)
                        player.SendInfoMessage($"权限:{x.GetString("Value")} 剩余时间:{If(x.GetString("DurationText") = "-1", "永久", (x.GetDateTime("EndTime") - Now).ToString("d\.hh\:mm\:ss"))}")
                    End Sub)
            Else
                player.SendInfoMessage("没有找到此玩家的权限")
            End If
        End Using
        Using reader = QueryReader("select Commands from PlayerList where name = @0", name)
            If reader.Read() AndAlso Not reader.Reader.IsDBNull(0) Then
                player.SendInfoMessage("永久权限")
                For Each perm In reader.Reader.GetString(0).Split(",", StringSplitOptions.RemoveEmptyEntries)
                    player.SendInfoMessage($"权限:{perm} 剩余时间:永久")
                Next
            End If
        End Using
    End Sub
    Private Sub CmdCtlReload(args As SubCmdArgs)
        If ReadConfig.Read(args.commandArgs.Player, False, True, ReadConfig.Root) Then
            PluginInit()
            CustomPlayerPluginHelpers.Groups = New GroupManager(CustomPlayerPluginHelpers.DB)
            For i = 0 To CustomPlayerPluginHelpers.Players.Length - 1
                If CustomPlayerPluginHelpers.Players(i) IsNot Nothing Then OnPlayerLogout(New Hooks.PlayerLogoutEventArgs(CustomPlayerPluginHelpers.Players(i).Player))
            Next
            For Each player In TShock.Players.Where(Function(x) x IsNot Nothing AndAlso x.IsLoggedIn)
                OnPlayerLogout(New Hooks.PlayerLogoutEventArgs(player))
            Next
            args.commandArgs.Player.SendSuccessMessage("重载成功")
        Else
            args.commandArgs.Player.SendSuccessMessage("重载错误")
            args.commandArgs.Player.SendErrorMessage(ReadConfig.ErrorString)
        End If
    End Sub
#End Region
End Class