Imports System.Reflection
Imports System.Timers
Imports Microsoft.Xna.Framework

Imports Terraria
Imports Terraria.Chat
Imports Terraria.GameContent.NetModules
Imports Terraria.Localization
Imports Terraria.Net
Imports Terraria.UI.Chat
Imports TerrariaApi.Server

Imports TShockAPI
Imports TShockAPI.DB
Imports TShockAPI.Hooks

Imports MySql.Data.MySqlClient

Imports BUtils = VBY.Basic.Utils
Imports VBY.Basic.Command
Imports VBY.Basic.Extension

'<Assembly: AssemblyVersion(CustomPlayerPlugin.VersionText)>
<ApiVersion(2, 1)>
Partial Public Class CustomPlayerPlugin
	Inherits TerrariaPlugin
	Public Const VersionText As String = "1.0.0.2"
	Public Overrides ReadOnly Property Name As String = "CustomPlayerPlugin"
	Public Overrides ReadOnly Property Author As String = "yu"
	Public Overrides ReadOnly Property Version As Version = Version.Parse(VersionText)
	Public Shared ReadOnly ReadConfig As New Config(TShock.SavePath, "CustomPlayer.json")
	Private Shared TestObject As (Prefix As String, Suffix As String, Time As TimeSpan?)
	Private ReadOnly MainCommand, MainCtlCommand As SubCmdRoot
	Private ReadOnly TimeOutTimer As New Timer(1000 * 60 * 5)
	Private ReadOnly AddCommands As Command()
	Public Sub New(game As Main)
		MyBase.New(game)
		Order = 10

		MainCommand = New SubCmdRoot("Bear")
		MainCtlCommand = New SubCmdRoot("Custom")

		With MainCommand
			With .Add("Permission", "权限", "perm")
				.Add(AddressOf CmdPermissionList, "List", "权限列表").AllowServer = False
			End With
			With .Add("Prefix", "前缀")
				.Add(Sub(args) TitleList(args, "前缀"), "List", "前缀列表").AllowServer = False
				.AddA(Sub(args) TitleWear(args, "Prefix"), "Wear", "前缀佩戴", "<前缀ID>", "为-1时设置为空").AllowServer = False
			End With
			With .Add("Suffix", "后缀")
				.Add(Sub(args) TitleList(args, "后缀"), "List", "后缀列表").AllowServer = False
				.AddA(Sub(args) TitleWear(args, "Suffix"), "Wear", "后缀佩戴", "<后缀ID>", "为-1时设置为空").AllowServer = False
			End With
			.Add(AddressOf CmdReload, "Reload", "数据修改后重载").AllowServer = False
		End With

		With MainCtlCommand
			.Add(Sub(args As SubCmdArgs)
					 args.commandArgs.Parameters.RemoveAt(0)
					 Group(args.commandArgs)
				 End Sub, "Group", "ts组管理")
			.AddA(AddressOf CmdCtlGroupSet, "GroupSet", "设置组", "<玩家名> <组名>", "<组名>可写null,表示清除")
			With .Add("Permission", "权限管理", "perm")
				.AddA(AddressOf CmdCtlPermissionAdd, "Add", "添加权限", "<玩家名> <权限名> <时限>", "<时限>为-1时为永久,为get时会获取Time.Test命令的设置")
				.AddA(AddressOf CmdCtlPermissionDel, "Del", "删除权限", "<玩家名> <权限名>")
				.AddA(AddressOf CmdCtlPermissionList, "List", "权限列表", "<玩家名>")
			End With
			With .Add("Prefix", "前缀管理")
				.AddA(Sub(args) CtlTitleAdd(args, "Prefix"), "Add", "添加前缀", "<玩家名> <前缀头衔> <时限>", "<时限>为-1时为永久,为get时会获取Time.Test命令的设置" & vbLf & "<前缀头衔>为get时会获取Prefix.Test命令的设置")
				.AddA(Sub(args) CtlTitleDel(args, "Prefix"), "Del", "删除前缀", "<玩家名> <前缀ID/前缀头衔>")
				.AddA(Sub(args) CtlTitleList(args, "Prefix"), "List", "前缀列表", "<玩家名>")
				.AddA(Sub(args) CtlTitleTest(args, "Prefix"), "Test", "前缀测试", "<前缀头衔>", "会把输入的前缀设置为当前玩家的前缀").AllowServer = False
				.AddA(Sub(args) CtlTitleWear(args, "Prefix"), "Wear", "佩戴前缀", "<玩家名> <前缀ID> <服务器ID>")
			End With
			With .Add("Suffix", "后缀管理")
				.AddA(Sub(args) CtlTitleAdd(args, "Suffix"), "Add", "添加后缀", "<玩家名> <后缀头衔> <时限>", "<时限>为-1时为永久,为get时会获取Time.Test命令的设置" & vbLf & "<后缀头衔>为get时会获取Suffix.Test命令的设置")
				.AddA(Sub(args) CtlTitleDel(args, "Suffix"), "Del", "删除后缀", "<玩家名> <后缀ID/后缀头衔>")
				.AddA(Sub(args) CtlTitleList(args, "Suffix"), "List", "后缀列表", "<玩家名>")
				.AddA(Sub(args) CtlTitleTest(args, "Suffix"), "Test", "后缀测试", "<后缀头衔>", "会把输入的后缀设置为当前玩家的后缀").AllowServer = False
				.AddA(Sub(args) CtlTitleWear(args, "Suffix"), "Wear", "佩戴后缀", "<玩家名> <后缀ID> <服务器ID>")
			End With
			With .Add("Time", "时间相关")
				.Add(Sub(args)
						 OnTimer(Nothing, Nothing)
						 args.commandArgs.Player.SendSuccessMessage("时间检查完成")
					 End Sub, "Check", "立刻进行时间检查")
				.AddA(Sub(args)
						  If Not TestObject.Time.HasValue Then TestObject.Time = New TimeSpan()
						  Dim time As TimeSpan
						  If TimeSpan.TryParse(args.Parameters(0), time) Then
							  args.commandArgs.Player.SendSuccessMessage("转换成功: " & time.ToString())
							  TestObject.Time = time
						  Else
							  args.commandArgs.Player.SendInfoMessage("转换失败")
						  End If
					  End Sub, "Test", "时间测试", "<时限>")
			End With
			.Add(AddressOf CmdCtlReload, "Reload", "重载")
		End With

		PluginInit()
		AddCommands = ReadConfig.Root.Commands.GetCommands(AddressOf Cmd, AddressOf CmdCtl)
	End Sub
	Private Sub PluginInit(Optional player As TSPlayer = Nothing)
		If ReadConfig.Read() Then
			CustomPlayerPluginHelpers.DB = New MySqlConnection(New MySqlConnectionStringBuilder() With {
									 .Server = ReadConfig.Root.MysqlHost,
									 .Port = ReadConfig.Root.MysqlPort,
									 .Database = ReadConfig.Root.MysqlDatabase,
									 .UserID = ReadConfig.Root.MysqlUser,
									 .Password = ReadConfig.Root.MysqlPass
									 }.ConnectionString)
			Try
				Dim sqlcreator = New SqlTableCreator(CustomPlayerPluginHelpers.DB, New MysqlQueryCreator())
				Dim createTitleTable = New SqlTable(NameOf(TableInfo.Prefix),
										New SqlColumn(NameOf(TableInfo.Prefix.Name), MySqlDbType.Text, 32),
										New SqlColumn(NameOf(TableInfo.Prefix.Id), MySqlDbType.Int32),
										New SqlColumn(NameOf(TableInfo.Prefix.Value), MySqlDbType.Text),
										New SqlColumn(NameOf(TableInfo.Prefix.StartTime), MySqlDbType.DateTime),
										New SqlColumn(NameOf(TableInfo.Prefix.EndTime), MySqlDbType.DateTime),
										New SqlColumn(NameOf(TableInfo.Prefix.DurationText), MySqlDbType.Text) With {.NotNull = True, .DefaultValue = ""})
				sqlcreator.EnsureTableStructure(createTitleTable)
				createTitleTable = New SqlTable(NameOf(TableInfo.Suffix),
										New SqlColumn(NameOf(TableInfo.Suffix.Name), MySqlDbType.Text, 32),
										New SqlColumn(NameOf(TableInfo.Suffix.Id), MySqlDbType.Int32),
										New SqlColumn(NameOf(TableInfo.Suffix.Value), MySqlDbType.Text),
										New SqlColumn(NameOf(TableInfo.Suffix.StartTime), MySqlDbType.DateTime),
										New SqlColumn(NameOf(TableInfo.Suffix.EndTime), MySqlDbType.DateTime),
										New SqlColumn(NameOf(TableInfo.Suffix.DurationText), MySqlDbType.Text) With {.NotNull = True, .DefaultValue = ""})
				sqlcreator.EnsureTableStructure(createTitleTable)
				sqlcreator.EnsureTableStructure(New SqlTable(NameOf(TableInfo.Useing),
												 New SqlColumn(NameOf(TableInfo.Useing.Name), MySqlDbType.Text, 32),
												 New SqlColumn(NameOf(TableInfo.Useing.ServerId), MySqlDbType.Int32),
												 New SqlColumn(NameOf(TableInfo.Useing.PrefixId), MySqlDbType.Int32) With {.DefaultValue = "-1"},
												 New SqlColumn(NameOf(TableInfo.Useing.SuffixId), MySqlDbType.Int32) With {.DefaultValue = "-1"}))
				sqlcreator.EnsureTableStructure(New SqlTable(NameOf(TableInfo.PlayerList),
												 New SqlColumn(NameOf(TableInfo.PlayerList.Name), MySqlDbType.Text, 32),
												 New SqlColumn(NameOf(TableInfo.PlayerList.Commands), MySqlDbType.Text),
												 New SqlColumn(NameOf(TableInfo.PlayerList.Group), MySqlDbType.Text),
												 New SqlColumn(NameOf(TableInfo.PlayerList.ChatColor), MySqlDbType.Text)))
				sqlcreator.EnsureTableStructure(New SqlTable(NameOf(TableInfo.GroupList),
												 New SqlColumn(NameOf(TableInfo.GroupList.GroupName), MySqlDbType.Text, 32),
												 New SqlColumn(NameOf(TableInfo.GroupList.Parent), MySqlDbType.Text, 32),
												 New SqlColumn(NameOf(TableInfo.GroupList.Commands), MySqlDbType.Text),
												 New SqlColumn(NameOf(TableInfo.GroupList.ChatColor), MySqlDbType.Text),
												 New SqlColumn(NameOf(TableInfo.GroupList.Prefix), MySqlDbType.Text),
												 New SqlColumn(NameOf(TableInfo.GroupList.Suffix), MySqlDbType.Text)))
				sqlcreator.EnsureTableStructure(New SqlTable(NameOf(TableInfo.ExpirationInfo),
												 New SqlColumn(NameOf(TableInfo.ExpirationInfo.Name), MySqlDbType.Text, 32),
												 New SqlColumn(NameOf(TableInfo.ExpirationInfo.Value), MySqlDbType.Text),
												 New SqlColumn(NameOf(TableInfo.ExpirationInfo.Type), MySqlDbType.Text),
												 New SqlColumn(NameOf(TableInfo.ExpirationInfo.StartTime), MySqlDbType.DateTime),
												 New SqlColumn(NameOf(TableInfo.ExpirationInfo.EndTime), MySqlDbType.DateTime),
												 New SqlColumn(NameOf(TableInfo.ExpirationInfo.DurationText), MySqlDbType.Text) With {.NotNull = True, .DefaultValue = ""}))

			Catch ex As MySqlException
				If player Is Nothing Then
					BUtils.WriteColorLine("数据库初始化失败")
				Else
					player.SendErrorMessage("数据库初始化失败")
				End If
			End Try
		End If
	End Sub
	Public Overrides Sub Initialize()
		Commands.ChatCommands.AddRange(AddCommands)
		AddHandler TimeOutTimer.Elapsed, AddressOf OnTimer
		AddHandler PlayerHooks.PlayerPostLogin, AddressOf OnPlayerPostLogin
		AddHandler PlayerHooks.PlayerPermission, AddressOf OnPlayerPermission
		AddHandler PlayerHooks.PlayerLogout, AddressOf OnPlayerLogout

		Dim handlers = ServerApi.Hooks.ServerChat
		Dim searchFlags = BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic
		Dim registrations = CType(handlers, IEnumerable).GetType().GetField("registrations", searchFlags).GetValue(handlers)
		Dim registratorfield = registrations.GetType().GenericTypeArguments(0).GetProperty("Registrator", searchFlags)
		Dim handlerfield = registrations.GetType().GenericTypeArguments(0).GetProperty("Handler", searchFlags)
		For Each registration In registrations
			Dim handler = CType(handlerfield.GetValue(registration), HookHandler(Of ServerChatEventArgs))
			Dim plugin = CType(registratorfield.GetValue(registration), TerrariaPlugin)
			If TypeOf plugin Is TShock Then
				TShock.Log.ConsoleInfo("TShock server chat handled forced To de-register")
				ServerApi.Hooks.ServerChat.Deregister(plugin, handler)
				ServerApi.Hooks.ServerChat.Register(plugin, AddressOf OnChat)
			End If
		Next

		For Each method In GetType(TShock).Assembly.GetType("TShockAPI.I18n").GetMethods()
			If method.Name = "GetString" Then
				Select Case method.GetParameters().Length
					'Case 1
					'	Utils.GetStringMethod1 = [Delegate].CreateDelegate(GetType(Func(Of FormattableString, String)), method)
					Case 2
						Utils.GetStringMethod = [Delegate].CreateDelegate(GetType(Func(Of GetText.FormattableStringAdapter, Object(), String)), method)
				End Select
			End If
		Next

		CustomPlayerPluginHelpers.Groups = New GroupManager(CustomPlayerPluginHelpers.DB)
	End Sub
	Protected Overrides Sub Dispose(disposing As Boolean)
		If disposing Then
			Commands.ChatCommands.RemoveRange(AddCommands)
			RemoveHandler TimeOutTimer.Elapsed, AddressOf OnTimer
			RemoveHandler PlayerHooks.PlayerPostLogin, AddressOf OnPlayerPostLogin
			RemoveHandler PlayerHooks.PlayerPermission, AddressOf OnPlayerPermission
			RemoveHandler PlayerHooks.PlayerLogout, AddressOf OnPlayerLogout

			CustomPlayerPluginHelpers.DB.Dispose()
			TimeOutTimer.Dispose()
		End If
		MyBase.Dispose(disposing)
	End Sub
	Private Sub OnTimer(sender As Object, e As ElapsedEventArgs)
		Dim now = Date.Now
		TSPlayer.Server.SendInfoMessage("time checking")
		For Each obj In CustomPlayerPluginHelpers.TimeOutList
			TSPlayer.Server.SendInfoMessage($"name:{obj.Name} type:{obj.Type} value:{obj.Value} outed:{obj.TimeOuted}")
			If Not obj.TimeOuted Then
				If obj.DurationText = "-1" Then Continue For
				TSPlayer.Server.SendInfoMessage("not -1")
				If obj.EndTime < now Then
					obj.TimeOuted = True
					Dim fply = FindPlayer(obj.Name)
					If fply IsNot Nothing Then
						Select Case obj.Type
							Case NameOf(Permission)
								fply.Permissions.Remove(obj.Value)
								fply.NegatedPermissions.Remove(obj.Value)
								fply.Player.SendInfoMessage($"权限:{obj.Value} 已过期")
							Case NameOf(CustomPlayer.Prefix)
								If obj.Value = fply.Prefix Then
									fply.Prefix = Nothing
									Query("update Useing set Prefix = -1 where Name = @0", fply.Name)
								End If
								fply.Player.SendInfoMessage($"前缀:{obj.Value} 已过期")
							Case NameOf(CustomPlayer.Suffix)
								If obj.Value = fply.Suffix Then
									fply.Suffix = Nothing
									Query("update Useing set Suffix = -1 where Name = @0", fply.Name)
								End If
								fply.Player.SendInfoMessage($"后缀:{obj.Value} 已过期")
						End Select
					End If
				End If
			End If
		Next
		Dim alltimeout = CustomPlayerPluginHelpers.TimeOutList.FindAll(Function(x) x.TimeOuted)
		alltimeout.ForEach(Sub(x) x.Delete())
		CustomPlayerPluginHelpers.TimeOutList.RemoveRange(alltimeout)
	End Sub
	Private Sub OnChat(args As ServerChatEventArgs)
		If args.Handled Then Return
		Dim tsplayer = TShock.Players(args.Who)
		If tsplayer Is Nothing Then
			args.Handled = True
			Return
		End If
		If args.Text.Length > 500 Then
			tsplayer.Kick("Crash attempt via Long chat packet.", True)
			args.Handled = True
			Return
		End If
		Dim text = args.Text
		For Each item In ChatManager.Commands._localizedCommands
			If item.Value._name = args.CommandId._name Then
				text = If(String.IsNullOrEmpty(text), item.Key.Value, item.Key.Value & " " & text)
				Exit For
			End If
		Next

		If (text.StartsWith(TShock.Config.Settings.CommandSpecifier) OrElse text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)) _
			AndAlso Not String.IsNullOrEmpty(text.Substring(1)) Then
			Try
				args.Handled = True
				If Not Commands.HandleCommand(tsplayer, text) Then
					tsplayer.SendErrorMessage("Unable To parse command. Please contact an administrator For assistance.")
					TShock.Log.ConsoleError("Unable To parse command '{0}' from player {1}.", text, tsplayer.Name)
				End If
			Catch ex As Exception
				TShock.Log.ConsoleError("An exception occurred executing a command.")
				TShock.Log.Error(ex.ToString())
			End Try
		Else
			If Not tsplayer.HasPermission(Permissions.canchat) Then
				args.Handled = True
				Return
			ElseIf tsplayer.mute Then
				tsplayer.SendErrorMessage("You are muted!")
				args.Handled = True
				Return
			End If

			'add 
			Dim prefix As String = Nothing, suffix As String = Nothing
			Dim color As Color?
			If tsplayer.IsLoggedIn Then
				Dim ply = CustomPlayerPluginHelpers.Players(args.Who)
				If ply IsNot Nothing Then
					prefix = ply.Prefix
					suffix = ply.Suffix
					color = ply.ChatColor
				End If
				'get data
			End If

			color = If(color, New Color(tsplayer.Group.R, tsplayer.Group.G, tsplayer.Group.B))

			If TShock.Config.Settings.EnableChatAboveHeads Then
				Dim player = Main.player(args.Who)
				Dim name = player.name
				player.name = String.Format(TShock.Config.Settings.ChatAboveHeadsFormat,
									  tsplayer.Group.Name,
									  prefix.NullOrEmptyReturn(tsplayer.Group.Prefix),
									  tsplayer.Name,
									  suffix.NullOrEmptyReturn(tsplayer.Group.Suffix))
				NetMessage.SendData(PacketTypes.PlayerInfo, -1, -1, NetworkText.FromLiteral(player.name), args.Who)
				player.name = name
				If PlayerHooks.OnPlayerChat(tsplayer, args.Text, text) Then
					args.Handled = True
					Return
				End If
				Dim packet = NetTextModule.SerializeServerMessage(NetworkText.FromLiteral(name), color, CByte(args.Who))
				NetManager.Instance.Broadcast(packet, args.Who)
				NetMessage.SendData(PacketTypes.PlayerInfo, -1, -1, NetworkText.FromLiteral(name), args.Who)

				'modify1
				Dim msg = $"<{String.Format(TShock.Config.Settings.ChatAboveHeadsFormat, tsplayer.Group.Name, prefix.NullOrEmptyReturn(tsplayer.Group.Prefix), tsplayer.Name, suffix.NullOrEmptyReturn(tsplayer.Group.Suffix)) }> {text}"
				tsplayer.SendMessage(msg, color)
				TSPlayer.Server.SendMessage(msg, color)

				TShock.Log.Info("Broadcast: {0}", msg)
				args.Handled = True
			Else
				text = String.Format(TShock.Config.Settings.ChatFormat, tsplayer.Group.Name,
						prefix.NullOrEmptyReturn(tsplayer.Group.Prefix),
						tsplayer.Name,
						suffix.NullOrEmptyReturn(tsplayer.Group.Suffix),
						args.Text)
				Dim cancelChat = PlayerHooks.OnPlayerChat(tsplayer, args.Text, text)
				args.Handled = True
				If cancelChat Then Return

				'modify2
				TShock.Utils.Broadcast(text, color)
			End If
		End If
	End Sub
	Private Sub OnPlayerPermission(args As PlayerPermissionEventArgs)
		If args.Result <> PermissionHookResult.Unhandled Then Return
		If args.Player.Index < 0 OrElse args.Player.Index > Byte.MaxValue - 1 Then Return
		Dim ply = CustomPlayerPluginHelpers.Players(args.Player.Index)
		If ply Is Nothing Then Return
		If ply.NegatedPermissions.Contains(args.Permission) Then
			args.Result = PermissionHookResult.Denied
		Else
			If ply.Permissions.Contains(args.Permission) Then args.Result = PermissionHookResult.Granted
			If If(ply.Group?.HasPermission(args.Permission), False) Then args.Result = PermissionHookResult.Granted
		End If
	End Sub
	Private Sub OnPlayerPostLogin(args As PlayerPostLoginEventArgs)
		'TSPlayer.Server.SendInfoMessage($"[CustomPlayer]{args.Player.Name} data load")
		CustomPlayerPluginHelpers.Players(args.Player.Index) = CustomPlayer.Read(args.Player.Name, args.Player)
	End Sub
	Private Sub OnPlayerLogout(args As PlayerLogoutEventArgs)
		CustomPlayerPluginHelpers.Players(args.Player.Index) = Nothing
		CustomPlayerPluginHelpers.TimeOutList.RemoveAll(Function(x) x.Name = args.Player.Name)
	End Sub
End Class