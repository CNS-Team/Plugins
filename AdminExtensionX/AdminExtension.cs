using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AdminExtension;

[ApiVersion(2, 1)]
public class AdminExtension : TerrariaPlugin
{

    public bool[] PvPForced = new bool[255];

    public bool[] ghostMode = new bool[255];

    public List<TSPlayer> autoKilledPlayers = new List<TSPlayer>();

    public List<TSPlayer> autoHealedPlayers = new List<TSPlayer>();

    private int updater;

    public static string Tag => TShock.Utils.ColorTag("AdminExtensionX:", Color.Teal);

    public override string Name => "AdminExtensionX汉化版";

    public override string Author => "ProfessorX,Ghasty制作,nnt汉化";

    public override string Description => "添加一些有用的命令.";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public AdminExtension(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
    }



    private void OnUpdate(EventArgs e)
    {
        this.updater++;
        if (this.updater % 60 != 0)
            return;
        TSPlayer[] players = TShock.Players;
        TSPlayer[] array = players;
        TSPlayer[] array2 = array;
        foreach (TSPlayer tSPlayer in array2)
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



    public void OnInitialize(EventArgs args)
    {
        Commands.ChatCommands.Add(new Command("ae.killall", this.KillAll, "killall", "击杀全部"));
        Commands.ChatCommands.Add(new Command("ae.autokill", this.AutoKill, "autokill", "自动击杀"));
        Commands.ChatCommands.Add(new Command("ae.healall", this.HealAll, "healall", "恢复全部"));
        Commands.ChatCommands.Add(new Command("ae.autoheal", this.AutoHeal, "autoheal", "自动恢复"));
        Commands.ChatCommands.Add(new Command("ae.forcepvp", this.ForcePvP, "forcepvp", "强制对战"));
        Commands.ChatCommands.Add(new Command("ae.ghost", this.Ghost, "ghost", "隐身上下线"));
        Commands.ChatCommands.Add(new Command("ae.butcher.npc", this.ButcherNPC, "butchernpc", "清除NPC"));
        Commands.ChatCommands.Add(new Command("ae.butcher.friendly", this.ButcherFriendly, "butcherfriendly", "清除友好NPC", "butcherf"));
        Commands.ChatCommands.Add(new Command("ae.findpermission", this.FindPermission, "findpermission", "查权限"));
    }

    private void FindPermission(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: /查权限 <命令名>");
            return;
        }
        string text = args.Parameters[0].ToLowerInvariant();
        if (text.StartsWith(TShock.Config.Settings.CommandSpecifier))
        {
            text = text.Substring(1);
        }
        int num = 0;
        while (true)
        {
            if (num < Commands.ChatCommands.Count)
            {
                if (Commands.ChatCommands[num].Names.Contains(text))
                {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        if (Commands.ChatCommands[num].Permissions.Count == 1)
        {
            args.Player.PluginInfoMessage(string.Format("所查询命令{2}{1}的权限是{0}", Commands.ChatCommands[num].Permissions[0], Commands.ChatCommands[num].Name, TShock.Config.Settings.CommandSpecifier));
            return;
        }
        args.Player.PluginInfoMessage("所查询命令" + TShock.Config.Settings.CommandSpecifier + Commands.ChatCommands[num].Name + "的权限是:");
        for (int i = 0; i < Commands.ChatCommands[num].Permissions.Count; i++)
        {
            args.Player.PluginInfoMessage(Commands.ChatCommands[num].Permissions[i]);
        }
    }

    public void KillAll(CommandArgs args)
    {
        TSPlayer[] players = TShock.Players;
        TSPlayer[] array = players;
        TSPlayer[] array2 = array;
        foreach (TSPlayer tSPlayer in array2)
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
            IEnumerable<string> enumerable = from p in this.autoKilledPlayers
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
        List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
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
        TSPlayer[] players = TShock.Players;
        for (int i = 0; i < players.Length; i++)
        {
            players[i]?.Heal();
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
            IEnumerable<string> enumerable = from p in this.autoHealedPlayers
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
        List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
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
        List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
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
            List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
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
        int num = 0;
        List<NPC> nPCByIdOrName = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
        if (nPCByIdOrName.Count == 0)
        {
            args.Player.PluginErrorMessage("无效的NPC.");
            return;
        }
        for (int i = 0; i < Main.npc.Length; i++)
        {
            if (Main.npc[i].friendly && Main.npc[i].type == nPCByIdOrName[0].type)
            {
                num++;
                TSPlayer.Server.StrikeNPC(i, 9999, 0f, 0);
            }
        }
        TSPlayer.All.PluginSuccessMessage(string.Format("{0}清除了{1}个{2}{3}.", args.Player.Name, num.ToString(), nPCByIdOrName[0].GivenOrTypeName, (num > 1) ? "" : ""));
    }

    private void CreateConfig()
    {

    }

    public void ButcherFriendly(CommandArgs args)
    {
        int num = 0;
        for (int i = 0; i < Main.npc.Length; i++)
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
