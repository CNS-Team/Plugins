using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ServerTool;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    private readonly HashSet<string> hash = new HashSet<string>();

    private bool freezeAll = false;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public override string Author => "少司命整合(棱镜，cjx，Leader)";

    public override string Description => "服务器工具";

    private Config Config = new();

    private readonly string PATH = Path.Combine(TShock.SavePath, "ServerTool.json");

    public bool[] PvPForced = new bool[255];

    public bool[] ghostMode = new bool[255];

    public List<TSPlayer> autoKilledPlayers = new List<TSPlayer>();

    public List<TSPlayer> autoHealedPlayers = new List<TSPlayer>();

    private int updater;

    public static string Tag => TShock.Utils.ColorTag("ServerTool:", Color.Teal);

    public Plugin(Main game)
        : base(game)
    {
    }


    public override void Initialize()
    {
        this.LoadConfig();
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.GamePostUpdate.Register(this, this.PostGameUpdate);
        ServerApi.Hooks.NetGetData.Register(this, this.OnNetGetData);
        GetDataHandlers.ReadNetModule.Register(this.Teleport, HandlerPriority.Normal, false);
        Commands.ChatCommands.Add(new Command(Permissions.kick, this.FreezeCmd, "freeze"));
        Commands.ChatCommands.Add(new Command(Permissions.kick, this.FreezeAllCmd,"freezeall" ));
        Commands.ChatCommands.Add(new Command("server.tool.killall", this.KillAll, "killall", "击杀全部"));
        Commands.ChatCommands.Add(new Command("server.tool.autokill", this.AutoKill, "autokill", "自动击杀"));
        Commands.ChatCommands.Add(new Command("server.tool.healall", this.HealAll, "healall", "恢复全部"));
        Commands.ChatCommands.Add(new Command("server.tool.autoheal", this.AutoHeal, "autoheal", "自动恢复"));
        Commands.ChatCommands.Add(new Command("server.tool.forcepvp", this.ForcePvP, "forcepvp", "强制对战"));
        Commands.ChatCommands.Add(new Command("server.tool.ghost", this.Ghost, "ghost", "隐身上下线"));
        Commands.ChatCommands.Add(new Command("server.tool.butcher.npc", this.ButcherNPC, "butchernpc", "清除NPC"));
        Commands.ChatCommands.Add(new Command("server.tool.butcher.friendly", this.ButcherFriendly, "butcherfriendly", "清除友好NPC", "butcherf"));
        Commands.ChatCommands.Add(new Command("server.tool.findpermission", this.FindPermission, "findpermission", "查权限"));
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += (e) =>
        {
            this.LoadConfig();
            e.Player.SendSuccessMessage("[ServerTool]配置已重读");
        };

    }

    private void OnUpdate(EventArgs e)
    {
        this.updater++;
        if (this.updater % 60 != 0)
        {
            return;
        }
        foreach (var tSPlayer in TShock.Players)
        {
            if (tSPlayer != null)
            {
                if (this.autoKilledPlayers.Contains(tSPlayer))
                {
                    tSPlayer.DamagePlayer(15000);
                }
                if (this.autoHealedPlayers.Contains(tSPlayer))
                {
                    tSPlayer.Heal();
                }
                if (this.PvPForced[tSPlayer.Index] && !tSPlayer.TPlayer.hostile)
                {
                    tSPlayer.TPlayer.hostile = true;
                    NetMessage.SendData(30, -1, -1, null, tSPlayer.Index);
                }
                if (this.ghostMode[tSPlayer.Index])
                {
                    tSPlayer.TPlayer.position.X = 0f;
                    tSPlayer.TPlayer.position.Y = 0f;
                    tSPlayer.TPlayer.team = 0;
                    NetMessage.SendData(13, -1, -1, null, tSPlayer.Index);
                    NetMessage.SendData(45, -1, -1, null, tSPlayer.Index);
                }
            }
        }
    }


    private void LoadConfig()
    {
        try
        {
            if (File.Exists(this.PATH))
            {
                this.Config = Config.Read(this.PATH);
            }
            else
            {
                
            }
            this.Config.Write(this.PATH);
        
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("ServerTool.json读取错误:" + ex.ToString());
        }
    }

    private void OnNetGetData(GetDataEventArgs args)
    {
        if (!this.Config.NoVisualLimit)
        {
            return;
        }
        if (args.MsgID != PacketTypes.ConnectRequest)
        {
            return;
        }
        args.Handled = true;
        if (Main.netMode != 2)
        {
            return;
        }
        if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[args.Msg.whoAmI].Socket.GetRemoteAddress()))
        {
            NetMessage.TrySendData(2, args.Msg.whoAmI, -1, Lang.mp[3].ToNetworkText(), 0, 0f, 0f, 0f, 0, 0, 0);
        }
        else if (Netplay.Clients[args.Msg.whoAmI].State == 0)
        {
            if (string.IsNullOrEmpty(Netplay.ServerPassword))
            {
                Netplay.Clients[args.Msg.whoAmI].State = 1;
                NetMessage.TrySendData(3, args.Msg.whoAmI, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
            else
            {
                Netplay.Clients[args.Msg.whoAmI].State = -1;
                NetMessage.TrySendData(37, args.Msg.whoAmI, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
        }
    }

    private void Teleport(object? sender, GetDataHandlers.ReadNetModuleEventArgs e)
    {
        if (e.Player.HasPermission("server.tool.maptp") && e.ModuleType == GetDataHandlers.NetModuleType.Ping)
        {
            using var binaryReader = new BinaryReader(e.Data);
            var val = Terraria.Utils.ReadVector2(binaryReader);
            e.Player.Teleport(val.X * 16f, val.Y * 16f, 1);
        }
    }

    private void FreezeAllCmd(CommandArgs args)
    {
        this.freezeAll = !this.freezeAll;
        args.Player.SendSuccessMessage("全服冻结已" + (this.freezeAll ? "开启" : "关闭"));
        TSPlayer.All.SendInfoMessage(args.Player.Name + (this.freezeAll ? "开启" : "关闭") + "了全服冻结");
        if (args.Parameters.Count > 0)
        {
            TSPlayer.All.SendInfoMessage(args.Parameters[0]);
        }
    }

    private void PostGameUpdate(EventArgs args)
    {
        foreach (var val in TShock.Players)
        {
            if (val != null && (this.hash.Contains(val.Name) || this.freezeAll) && !val.HasPermission("tshock.admin.kick"))
            {
                val.Disable("", 0);
                val.TPlayer.Bottom = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
                val.SendData(PacketTypes.PlayerUpdate, "", val.Index, 0f, 0f, 0f, 0);
            }
        }
    }

    private void FreezeCmd(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage("用法:/freeze <玩家名>");
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError((IEnumerable<object>) list);
            return;
        }
        if (list.Count == 0)
        {
            args.Player.SendErrorMessage("找不到玩家");
            return;
        }
        var val = list[0];
        if (this.hash.Add(val.Name))
        {
            val.Disable("被管理员冻结", 0);
            args.Player.SendSuccessMessage("已冻结" + val.Name);
        }
        else
        {
            args.Player.SendSuccessMessage("已解除冻结" + val.Name);
            this.hash.Remove(val.Name);
        }
    }

    private void FindPermission(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: /查权限 <命令名>");
            return;
        }
        var text = args.Parameters[0].ToLowerInvariant();
        if (text.StartsWith(TShock.Config.Settings.CommandSpecifier))
        {
            text = text[1..];
        }
        var result = Commands.ChatCommands.Find(f => f.Names.Contains(text));
        if (result == null)
        {
            args.Player.SendErrorMessage("没有查询到相关指令!");
            return;
        }

        args.Player.PluginInfoMessage(string.Format("所查询命令{2}{1}的权限是{0}", string.Join(",", result.Permissions), result.Name, TShock.Config.Settings.CommandSpecifier));
        return;

    }

    public void KillAll(CommandArgs args)
    {
        foreach (var tSPlayer in TShock.Players)
        {
            if (tSPlayer != null && !tSPlayer.Group.HasPermission("ae.killall.bypass") && tSPlayer != args.Player && TShock.Utils.GetActivePlayerCount() > 1)
            {
                tSPlayer.DamagePlayer(15000);
            }
        }
        args.Player.PluginSuccessMessage("你已击杀所有人!");
        if (!args.Silent)
        {
            TSPlayer.All.PluginErrorMessage(args.Player.Name + "使用指令击杀了所有人.");
        }
    }

    public void AutoKill(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "自动击杀 <用户名/列表>");
            return;
        }
        if (args.Parameters[0].ToLower() == "list" || args.Parameters[0].ToLower() == "列表")
        {
            var enumerable = from p in this.autoKilledPlayers
                             orderby p.Name
                             select p.Name;
            if (enumerable.Count() == 0)
            {
                args.Player.PluginErrorMessage("自动击杀列表里没有用户.");
            }
            else
            {
                args.Player.PluginSuccessMessage(string.Format("正在自动击杀的用户: {0}", string.Join(", ", enumerable)));
            }
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count == 0)
        {
            args.Player.PluginErrorMessage("无效的用户.");
        }
        else if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError(list.Select((TSPlayer p) => p.Name));
        }
        else if (list[0].Group.HasPermission("ae.autokill.bypass"))
        {
            args.Player.PluginErrorMessage("你不能将该用户添加进自动击杀列表.");
        }
        else if (!this.autoKilledPlayers.Contains(list[0]))
        {
            this.autoKilledPlayers.Add(list[0]);
            args.Player.PluginSuccessMessage("已将" + list[0].Name + "添加进自动击杀列表,再次输入可以移除效果.");
            list[0].PluginInfoMessage("你已被自动击杀.");
        }
        else
        {
            this.autoKilledPlayers.Remove(list[0]);
            args.Player.PluginSuccessMessage("已将" + list[0].Name + "移除出自动击杀列表.");
            list[0].PluginInfoMessage("已移除自动击杀效果.");
        }
    }

    public void HealAll(CommandArgs args)
    {
        foreach (var player in TShock.Players)
        {
            if (player != null && player.Active)
            {
                player.Heal();
            }
        }
        args.Player.PluginSuccessMessage("你已治疗所有人!");
        if (!args.Silent)
        {
            TSPlayer.All.PluginInfoMessage(args.Player.Name + "治疗了所有人.");
        }
    }

    public void AutoHeal(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "自动恢复 <用户名/列表>");
            return;
        }
        if (args.Parameters[0].ToLower() == "list" || args.Parameters[0].ToLower() == "列表")
        {
            var enumerable = from p in this.autoHealedPlayers
                             orderby p.Name
                             select p.Name;
            if (enumerable.Count() == 0)
            {
                args.Player.PluginErrorMessage("自动恢复列表里没有用户.");
            }
            else
            {
                args.Player.PluginSuccessMessage(string.Format("正在自动恢复的用户: {0}", string.Join(", ", enumerable)));
            }
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count == 0)
        {
            args.Player.PluginErrorMessage("无效的用户.");
        }
        else if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError(list.Select((TSPlayer p) => p.Name));
        }
        else if (!this.autoHealedPlayers.Contains(list[0]))
        {
            this.autoHealedPlayers.Add(list[0]);
            args.Player.PluginSuccessMessage("已将" + list[0].Name + "添加进自动恢复列表,再次输入可以移除效果.");
            list[0].PluginInfoMessage("你现在正处于自动恢复状态.");
        }
        else
        {
            this.autoHealedPlayers.Remove(list[0]);
            args.Player.PluginSuccessMessage("已将" + list[0].Name + "移除出自动恢复列表.");
            list[0].PluginInfoMessage("已移除自动恢复效果.");
        }
    }

    public void ForcePvP(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "强制对战 <用户名>");
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count == 0)
        {
            args.Player.PluginErrorMessage("无效的用户.");
        }
        else if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError(list.Select((TSPlayer p) => p.Name));
        }
        else if (list[0].Group.HasPermission("ae.forcepvp.bypass"))
        {
            args.Player.PluginErrorMessage("你不能强制开启该玩家的对战状态.");
        }
        else
        {
            this.PvPForced[list[0].Index] = !this.PvPForced[list[0].Index];
            args.Player.PluginSuccessMessage(string.Format("{0}的对战状态已{1}.", list[0].Name, this.PvPForced[list[0].Index] ? "强制开启并锁定" : "不再锁定"));
            list[0].PluginInfoMessage(string.Format("你的对战状态已{0}.", this.PvPForced[list[0].Index] ? "强制开启并锁定" : "不再锁定"));
        }
    }

    public void Ghost(CommandArgs args)
    {
        if (args.Parameters.Count == 1)
        {
            var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (list.Count == 0)
            {
                args.Player.PluginErrorMessage("无效的用户.");
                return;
            }
            if (list.Count > 1)
            {
                args.Player.SendMultipleMatchError(list.Select((TSPlayer p) => p.Name));
                return;
            }
            this.ghostMode[list[0].Index] = !this.ghostMode[list[0].Index];
            args.Player.PluginSuccessMessage(string.Format("{0}了{1}的隐身上下线模式.", this.ghostMode[list[0].Index] ? "已开启" : "已关闭", list[0].Name));
            list[0].PluginInfoMessage(string.Format("{0} {1}了隐身上下线模式.", args.Player.Name, this.ghostMode[list[0].Index] ? "已为你开启" : "已关闭"));
            TSPlayer.All.SendInfoMessage(string.Format("{0} {1}.", list[0].Name, this.ghostMode[list[0].Index] ? "离开游戏" : "加入游戏"));
        }
        else if (args.Parameters.Count == 0)
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendErrorMessage("你必须在游戏中使用.");
                return;
            }
            this.ghostMode[args.Player.Index] = !this.ghostMode[args.Player.Index];
            args.Player.PluginSuccessMessage(string.Format("{0}隐身上下线模式.", this.ghostMode[args.Player.Index] ? "已开启" : "已关闭"));
            TSPlayer.All.SendInfoMessage(string.Format("{0} {1}.", args.Player.Name, this.ghostMode[args.Player.Index] ? "离开游戏" : "加入游戏"));
        }
    }

    public void ButcherNPC(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "清除NPC <npc ID/名字>");
            return;
        }
        var num = 0;
        var nPCByIdOrName = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
        if (nPCByIdOrName.Count == 0)
        {
            args.Player.PluginErrorMessage("无效的NPC.");
            return;
        }
        for (var i = 0; i < Main.npc.Length; i++)
        {
            if (Main.npc[i].friendly && Main.npc[i].type == nPCByIdOrName[0].type)
            {
                num++;
                TSPlayer.Server.StrikeNPC(i, 9999, 0f, 0);
            }
        }
        TSPlayer.All.PluginSuccessMessage(string.Format("{0}清除了{1}个{2}{3}.", args.Player.Name, num.ToString(), nPCByIdOrName[0].GivenOrTypeName, (num > 1) ? "" : ""));
    }

    public void ButcherFriendly(CommandArgs args)
    {
        var num = 0;
        for (var i = 0; i < Main.npc.Length; i++)
        {
            if (Main.npc[i].active && Main.npc[i].townNPC)
            {
                num++;
                TSPlayer.Server.StrikeNPC(i, 9999, 0f, 0);
            }
        }
        TSPlayer.All.PluginInfoMessage(string.Format("{0}已清除{1}个友好NPC{2}.", args.Player.Name, num.ToString(), (num > 1) ? "" : ""));
    }
}