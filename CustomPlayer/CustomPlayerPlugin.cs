﻿using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Reflection;
using System.Timers;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Net;
using Terraria.UI.Chat;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.DB.Queries;
using TShockAPI.Hooks;
using VBY.Basic.Command;
using VBY.Basic.Extension;
using BUtils = VBY.Basic.Utils;

namespace CustomPlayer;

[ApiVersion(2, 1)]
public partial class CustomPlayerPlugin : TerrariaPlugin
{
    public const string VersionText = "1.0.0.3";
    public override string Name { get; } = "CustomPlayerPlugin";
    public override string Author { get; } = "yu";
    public override Version Version { get; } = Version.Parse(VersionText);
    public static readonly Config ReadConfig = new(TShock.SavePath, "CustomPlayer.json");
    private static (string Prefix, string Suffix, TimeSpan? Time) TestObject;
    private readonly SubCmdRoot CmdCommand, CtlCommand;
    private readonly System.Timers.Timer TimeOutTimer = new(1000 * 60 * 5);
    private readonly Command[] AddCommands;
    public CustomPlayerPlugin(Main game) : base(game)
    {
        this.Order = 1;
        this.CmdCommand = new SubCmdRoot("Bear");
        this.CtlCommand = new SubCmdRoot("Custom");
        SubCmdNodeList curNode;
        {
            this.CmdCommand.AddList("Permission", "权限", "perm")
            .AddCmd(this.CmdPermissionList, "List", "权限列表");
        }
        {
            curNode = this.CmdCommand.AddList("Prefix", "前缀");
            curNode.AddCmd(args => TitleList(ref args, "前缀"), "List", "前缀列表");
            curNode.AddCmdA(args => TitleWear(ref args, "Prefix"), "Wear", "前缀佩戴", "<前缀ID>", "为-1时设置为空");
        }
        {
            curNode = this.CmdCommand.AddList("Suffix", "后缀");
            curNode.AddCmd(args => TitleList(ref args, "后缀"), "List", "后缀列表");
            curNode.AddCmdA(args => TitleWear(ref args, "Suffix"), "Wear", "后缀佩戴", "<后缀ID>", "为-1时设置为空");
        }
        this.CmdCommand.AddCmd(this.CmdReload, "Reload", "数据修改后重载");
        this.CmdCommand.SetAllNodeRun(new AllowInfo(null, false, null));

        this.CtlCommand.AddCmd((SubCmdArgs args) =>
        {
            args.commandArgs.Parameters.RemoveAt(0);
            Group(args.commandArgs);
        }, "Group", "ts组管理");
        {
            curNode = this.CtlCommand.AddList("GroupCtl", "组管理");
            curNode.AddCmdAA(this.CtlGroupCtlAdd, "添加组", "<玩家名> <组名> <时限>");
            curNode.AddCmdAA(this.CtlGroupCtlDel, "删除组", "<玩家名> <组名>");
            curNode.AddCmdAA(this.CtlGroupCtlList, "列出组", "<玩家名>");
        }
        {
            curNode = this.CtlCommand.AddList("Permission", "权限管理", "perm");
            curNode.AddCmdAA(this.CtlPermissionAdd, "添加权限", "<玩家名> <权限名> <时限>", " <时限>为-1时为永久,为get时会获取Time.Test命令的设置");
            curNode.AddCmdAA(this.CtlPermissionDel, "删除权限", "<玩家名> <权限名>");
            curNode.AddCmdAA(this.CtlPermissionList, "权限列表", "<玩家名>");
        }
        {
            curNode = this.CtlCommand.AddList("Prefix", "前缀管理");
            curNode.AddCmdAA(this.CtlPrefixAdd, "添加前缀", "<玩家名> <前缀头衔> <时限>", " <时限>为-1时为永久,为get时会获取Time.Test命令的设置\n <前缀头衔>为get时会获取Prefix.Test命令的设置");
            curNode.AddCmdAA(this.CtlPrefixDel, "删除前缀", "<玩家名> <前缀ID/前缀头衔>");
            curNode.AddCmdAA(this.CtlPrefixList, "前缀列表", "<玩家名>");
            curNode.AddCmdAA(this.CtlPrefixTest, "前缀测试", "<前缀头衔>", " 会把输入的前缀设置为当前玩家的前缀").AllowServer = false;
            curNode.AddCmdAA(this.CtlPrefixWear, "佩戴前缀", "<玩家名> <前缀ID> <服务器ID>");
        }
        {
            curNode = this.CtlCommand.AddList("Suffix", "后缀管理");
            curNode.AddCmdAA(this.CtlSuffixAdd, "添加后缀", "<玩家名> <后缀头衔> <时限>", " <时限>为-1时为永久,为get时会获取Time.Test命令的设置\n <后缀头衔>为get时会获取Suffix.Test命令的设置");
            curNode.AddCmdAA(this.CtlSuffixDel, "删除后缀", "<玩家名> <后缀ID/后缀头衔>");
            curNode.AddCmdAA(this.CtlSuffixList, "后缀列表", "<玩家名>");
            curNode.AddCmdAA(this.CtlSuffixTest, "后缀测试", "<后缀头衔>", " 会把输入的后缀设置为当前玩家的后缀").AllowServer = false;
            curNode.AddCmdAA(this.CtlSuffixWear, "佩戴后缀", "<玩家名> <后缀ID> <服务器ID>");
        }
        {
            curNode = this.CtlCommand.AddList("Time", "时间相关");
            curNode.AddCmd(args =>
            {
                this.OnTimer(null, null!);
                args.commandArgs.Player.SendSuccessMessage("时间检查完成");
            }, "Check", "立刻进行时间检查");
            curNode.AddCmdA(args =>
            {
                if (!TestObject.Time.HasValue)
                {
                    TestObject.Time = new TimeSpan();
                }

                if (TimeSpan.TryParse(args.Parameters[0], out var time))
                {
                    args.commandArgs.Player.SendSuccessMessage("转换成功: " + time.ToString());
                    TestObject.Time = time;
                }
                else
                {
                    args.commandArgs.Player.SendInfoMessage("转换失败");
                }
            }, "Test", "时间测试", "<时限>");
        }
        this.CtlCommand.AddCmd(this.CtlReload, "Reload", "重载");

        PluginInit();
        this.AddCommands = ReadConfig.Root.Commands.GetCommands(this.CmdCommand, this.CtlCommand);
    }
    private static void PluginInit(TSPlayer? player = default)
    {
        if (ReadConfig.Read())
        {
            CustomPlayer.对接称号插件 = ReadConfig.Root.对接称号插件;
            CustomPlayerPluginHelpers.DB = new MySqlConnection(new MySqlConnectionStringBuilder()
            {
                Server = ReadConfig.Root.MysqlHost,
                Port = ReadConfig.Root.MysqlPort,
                Database = ReadConfig.Root.MysqlDatabase,
                UserID = ReadConfig.Root.MysqlUser,
                Password = ReadConfig.Root.MysqlPass
            }.ConnectionString);
            try
            {
                var sqlcreator = new SqlTableCreator(CustomPlayerPluginHelpers.DB, new MysqlQueryBuilder());
                foreach (var createTable in typeof(TableInfo).GetNestedTypes())
                {
                    sqlcreator.EnsureTableStructure(Utils.SqlTableCreate(createTable));
                }
            }
            catch (MySqlException)
            {
                if (player == null)
                {
                    BUtils.WriteColorLine("数据库初始化失败");
                }
                else
                {
                    player.SendErrorMessage("数据库初始化失败");
                }
            }
        }
    }
    public override void Initialize()
    {
        this.RestInit();
        Commands.ChatCommands.AddRange(this.AddCommands);
        PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
        PlayerHooks.PlayerPermission += this.OnPlayerPermission;
        PlayerHooks.PlayerLogout += OnPlayerLogout;
        this.TimeOutTimer.Elapsed += this.OnTimer;

        if (!ReadConfig.Root.对接称号插件)
        {
            var handlers = ServerApi.Hooks.ServerChat;
            var registrations = (IEnumerable) handlers.GetType()
                .GetField("registrations", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(handlers)!;
            var registratorfield = registrations.GetType().GenericTypeArguments[0]
                .GetProperty("Registrator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var handlerfield = registrations.GetType().GenericTypeArguments[0]
                .GetProperty("Handler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var registration in registrations)
            {
                var handler = (HookHandler<ServerChatEventArgs>) handlerfield.GetValue(registration)!;
                var plugin = (TerrariaPlugin) registratorfield.GetValue(registration)!;
                if (plugin is TShock)
                {
                    TShock.Log.ConsoleInfo("TShock server chat handled forced to de-register");
                    ServerApi.Hooks.ServerChat.Deregister(plugin, handler);
                    ServerApi.Hooks.ServerChat.Register(plugin, this.OnChat);
                }
            }
        }

        foreach (var method in typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetMethods())
        {
            if (method.Name == "GetString" && method.GetParameters().Length == 2)
            {
                Utils.GetStringMethod = (Func<GetText.FormattableStringAdapter, object[], string>) Delegate.CreateDelegate(typeof(Func<GetText.FormattableStringAdapter, object[], string>), method)!;
            }
        }

        CustomPlayerPluginHelpers.Groups = new ModfiyGroup.GroupManager(CustomPlayerPluginHelpers.DB);
        CustomPlayerPluginHelpers.GroupLevelSet();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveRange(this.AddCommands);
            this.TimeOutTimer.Elapsed -= this.OnTimer;
            PlayerHooks.PlayerPostLogin -= OnPlayerPostLogin;
            PlayerHooks.PlayerPermission -= this.OnPlayerPermission;
            PlayerHooks.PlayerLogout -= OnPlayerLogout;

            CustomPlayerPluginHelpers.DB.Dispose();
            this.TimeOutTimer.Dispose();
        }
        base.Dispose(disposing);
    }
    private void OnTimer(object? sender, ElapsedEventArgs e)
    {
        var now = DateTime.Now;
        TSPlayer.Server.SendInfoMessage("time checking");
        foreach (var obj in CustomPlayerPluginHelpers.TimeOutList)
        {
            TSPlayer.Server.SendInfoMessage($"name:{obj.Name} type:{obj.Type} value:{obj.Value} outed:{obj.TimeOuted}");
            if (!obj.TimeOuted)
            {
                if (obj.DurationText == "-1")
                {
                    continue;
                }
                //TSPlayer.Server.SendInfoMessage("not -1");
                if (obj.EndTime < now)
                {
                    obj.TimeOuted = true;
                    var fply = Utils.FindPlayer(obj.Name);
                    if (fply != null)
                    {
                        switch (obj.Type)
                        {
                            case nameof(Permission):
                            {
                                fply.Permissions.Remove(obj.Value);
                                fply.NegatedPermissions.Remove(obj.Value);
                                fply.Player.SendInfoMessage($"权限:{obj.Value} 已过期");
                                break;
                            }

                            case nameof(CustomPlayer.Prefix):
                            {
                                if (obj.Value == fply.Prefix)
                                {
                                    fply.Prefix = null;
                                    Utils.Query("update Useing set Prefix = -1 where Name = @0", fply.Name);
                                }
                                fply.Player.SendInfoMessage($"前缀:{obj.Value} 已过期");
                                break;
                            }

                            case nameof(CustomPlayer.Suffix):
                            {
                                if (obj.Value == fply.Suffix)
                                {
                                    fply.Suffix = null;
                                    Utils.Query("update Useing set Suffix = -1 where Name = @0", fply.Name);
                                }
                                fply.Player.SendInfoMessage($"后缀:{obj.Value} 已过期");
                                break;
                            }

                            case nameof(CustomPlayer.Group):
                            {
                                var pgroup = fply.Group;
                                fply.Player.SendInfoMessage($"组:{obj.Value} 已过期");
                                fply.HaveGroupNames.Remove(obj.Value);
                                fply.Player.SendInfoMessage("因组过期,你附加组:{0} 已清除", obj.Value);
                                if (obj.Value == pgroup.Name)
                                {
                                    if (ReadConfig.Root.CoverGroup)
                                    {
                                        if (fply.HaveGroupNames.Count > 0)
                                        {
                                            fply.Player.Group = fply.HaveGroupNames.Select(x => (CustomPlayerPluginHelpers.Groups.GetGroupByName(x), CustomPlayerPluginHelpers.GroupGrade[x]))
                                            .MaxBy(x => x.Item2).Item1;
                                            fply.Player.SendInfoMessage("你的组切换为更低级的组:{0}", fply.Group.Name);
                                        }
                                        else
                                        {
                                            fply.Player.Group = fply.Group;
                                            fply.Player.SendInfoMessage("你的组切换为原始组:{0}", fply.Group.Name);
                                        }
                                    }
                                    else
                                    {
                                        if (fply.HaveGroupNames.Count > 0)
                                        {
                                            fply.Group = fply.HaveGroupNames.Select(x => (CustomPlayerPluginHelpers.Groups.GetGroupByName(x), CustomPlayerPluginHelpers.GroupGrade[x]))
                                                .MaxBy(x => x.Item2).Item1;
                                            fply.Player.SendInfoMessage("你的组切换为更低级的组:{0}", fply.Group.Name);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        var alltimeout = CustomPlayerPluginHelpers.TimeOutList.FindAll(x => x.TimeOuted);
        alltimeout.ForEach(x => x.Delete());
        CustomPlayerPluginHelpers.TimeOutList.RemoveRange(alltimeout);
    }
    private void OnChat(ServerChatEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        var tsplayer = TShock.Players[args.Who];
        if (tsplayer == null)
        {
            args.Handled = true;
            return;
        }
        if (args.Text.Length > 500)
        {
            tsplayer.Kick("Crash attempt via Long chat packet.", true);
            args.Handled = true;
            return;
        }
        var text = args.Text;
        foreach (var item in ChatManager.Commands._localizedCommands)
        {
            if (item.Value._name == args.CommandId._name)
            {
                text = string.IsNullOrEmpty(text) ? item.Key.Value : item.Key.Value + " " + text;
                break;
            }
        }

        if ((text.StartsWith(TShock.Config.Settings.CommandSpecifier) || text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)) && !string.IsNullOrEmpty(text[1..]))
        {
            try
            {
                args.Handled = true;
                if (!Commands.HandleCommand(tsplayer, text))
                {
                    tsplayer.SendErrorMessage("Unable To parse command. Please contact an administrator For assistance.");
                    TShock.Log.ConsoleError("Unable To parse command '{0}' from player {1}.", text, tsplayer.Name);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("An exception occurred executing a command.");
                TShock.Log.Error(ex.ToString());
            }
        }
        else
        {
            if (!tsplayer.HasPermission(Permissions.canchat))
            {
                args.Handled = true;
                return;
            }
            else if (tsplayer.mute)
            {
                tsplayer.SendErrorMessage("You are muted!");
                args.Handled = true;
                return;
            }


            // add 
            string? prefix = null;
            string? suffix = null;
            Color? color = null;
            if (tsplayer.IsLoggedIn)
            {
                var ply = CustomPlayerPluginHelpers.Players[args.Who];
                if (ply != null)
                {
                    prefix = ply.Prefix;
                    suffix = ply.Suffix;
                    color = ply.ChatColor;
                }
            }

            color ??= new Color(tsplayer.Group.R, tsplayer.Group.G, tsplayer.Group.B);

            if (TShock.Config.Settings.EnableChatAboveHeads)
            {
                var player = Main.player[args.Who];
                var name = player.name;
                player.name = string.Format(TShock.Config.Settings.ChatAboveHeadsFormat, tsplayer.Group.Name, prefix.NullOrEmptyReturn(tsplayer.Group.Prefix), tsplayer.Name, suffix.NullOrEmptyReturn(tsplayer.Group.Suffix));
                NetMessage.SendData((int) PacketTypes.PlayerInfo, -1, -1, NetworkText.FromLiteral(player.name), args.Who);
                player.name = name;
                if (PlayerHooks.OnPlayerChat(tsplayer, args.Text, ref text))
                {
                    args.Handled = true;
                    return;
                }
                var packet = NetTextModule.SerializeServerMessage(NetworkText.FromLiteral(name), color.Value, System.Convert.ToByte(args.Who));
                NetManager.Instance.Broadcast(packet, args.Who);
                NetMessage.SendData((int) PacketTypes.PlayerInfo, -1, -1, NetworkText.FromLiteral(name), args.Who);

                // modify1
                var msg = $"<{string.Format(TShock.Config.Settings.ChatAboveHeadsFormat, tsplayer.Group.Name, prefix.NullOrEmptyReturn(tsplayer.Group.Prefix), tsplayer.Name, suffix.NullOrEmptyReturn(tsplayer.Group.Suffix))}> {text}";
                tsplayer.SendMessage(msg, color.Value);
                TSPlayer.Server.SendMessage(msg, color.Value);

                TShock.Log.Info("Broadcast: {0}", msg);
                args.Handled = true;
            }
            else
            {
                text = string.Format(TShock.Config.Settings.ChatFormat, tsplayer.Group.Name, prefix.NullOrEmptyReturn(tsplayer.Group.Prefix), tsplayer.Name, suffix.NullOrEmptyReturn(tsplayer.Group.Suffix), args.Text);
                var cancelChat = PlayerHooks.OnPlayerChat(tsplayer, args.Text, ref text);
                args.Handled = true;
                if (cancelChat)
                {
                    return;
                }

                // modify2
                TShock.Utils.Broadcast(text, color.Value);
            }
        }
    }
    private void OnPlayerPermission(PlayerPermissionEventArgs args)
    {
        /*TSPlayer.Server.SendInfoMessage(args.Permission);
        if (args.Result != PermissionHookResult.Unhandled)
        {
            TSPlayer.Server.SendInfoMessage("权限已检查");
            return;
        }
        if (args.Player.Index < 0 || args.Player.Index > byte.MaxValue - 1)
        {
            TSPlayer.Server.SendInfoMessage("index超范围");
            return;
        }
        var cply = CustomPlayerPluginHelpers.Players[args.Player.Index];
        if (cply == null)
        {
            TSPlayer.Server.SendInfoMessage("cply is null");
            return;
        }
        if (cply.NegatedPermissions.Contains(args.Permission))
        {
            TSPlayer.Server.SendInfoMessage("权限已拒绝");
            args.Result = PermissionHookResult.Denied;
        }
        else
        {
            if (cply.Permissions.Contains(args.Permission))
            {
                TSPlayer.Server.SendInfoMessage("玩家拥有权限");
                args.Result = PermissionHookResult.Granted;
            }
            if(cply.Group is null)
            {
                TSPlayer.Server.SendInfoMessage("组为空");
            }
            else
            {
                if (cply.Group.HasPermission(args.Permission))
                {
                    TSPlayer.Server.SendInfoMessage("组有权限");
                    args.Result = PermissionHookResult.Granted;
                }
                else
                {
                    TSPlayer.Server.SendInfoMessage("组无权限");
                }
            }
        }
        return;*/
        if (args.Result != PermissionHookResult.Unhandled)
        {
            return;
        }

        if (args.Player.Index < 0 || args.Player.Index > byte.MaxValue - 1)
        {
            return;
        }

        var cply = CustomPlayerPluginHelpers.Players[args.Player.Index];
        if (cply == null)
        {
            return;
        }

        if (cply.NegatedPermissions.Contains(args.Permission))
        {
            args.Result = PermissionHookResult.Denied;
        }
        else
        {
            if (cply.Permissions.Contains(args.Permission))
            {
                args.Result = PermissionHookResult.Granted;
            }

            if (!ReadConfig.Root.CoverGroup && (cply.Group?.HasPermission(args.Permission) ?? false))
            {
                args.Result = PermissionHookResult.Granted;
            }
        }
    }
    public static void OnPlayerPostLogin(PlayerPostLoginEventArgs args)
    {
        CustomPlayerPluginHelpers.Players[args.Player.Index] = CustomPlayer.Read(args.Player.Name, args.Player);
    }
    public static void OnPlayerLogout(PlayerLogoutEventArgs args)
    {
        CustomPlayerPluginHelpers.Players[args.Player.Index] = null!;
        CustomPlayerPluginHelpers.TimeOutList.RemoveAll(x => x.Name == args.Player.Name);
    }
}