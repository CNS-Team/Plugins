using Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace ServerTools.AdminExtensionX;
internal class Main
{
    private static readonly bool[] PvPForced = new bool[255];

    private static readonly bool[] ghostMode = new bool[255];

    private static readonly List<TSPlayer> autoKilledPlayers = new List<TSPlayer>();

    private static readonly List<TSPlayer> autoHealedPlayers = new List<TSPlayer>();

    public static void Update(EventArgs args)
    {
        var players = TShock.Players;
        var array = players;
        var array2 = array;
        foreach (var val in array2)
        {
            if (val != null)
            {
                if (autoKilledPlayers.Contains(val))
                {
                    val.DamagePlayer(15000);
                }
                if (autoHealedPlayers.Contains(val))
                {
                    val.Heal(600);
                }
                if (PvPForced[val.Index] && !val.TPlayer.hostile)
                {
                    val.TPlayer.hostile = true;
                    NetMessage.SendData(30, -1, -1, NetworkText.Empty, val.Index, 0f, 0f, 0f, 0, 0, 0);
                }
                if (ghostMode[val.Index])
                {
                    val.TPlayer.position.X = 0f;
                    val.TPlayer.position.Y = 0f;
                    val.TPlayer.team = 0;
                    NetMessage.SendData(13, -1, -1, NetworkText.Empty, val.Index, 0f, 0f, 0f, 0, 0, 0);
                    NetMessage.SendData(45, -1, -1, NetworkText.Empty, val.Index, 0f, 0f, 0f, 0, 0, 0);
                }
            }
        }
    }

    internal static void ACmd()
    {
        Commands.ChatCommands.Add(new Command("ae.killall", KillAll, "killall", "击杀全部" ));
        Commands.ChatCommands.Add(new Command("ae.autokill", AutoKill, "autokill", "自动击杀" ));
        Commands.ChatCommands.Add(new Command("ae.healall", HealAll, "healall", "恢复全部" ));
        Commands.ChatCommands.Add(new Command("ae.autoheal", AutoHeal, "autoheal", "自动恢复" ));
        Commands.ChatCommands.Add(new Command("ae.forcepvp", ForcePvP, "forcepvp", "强制对战" ));
        Commands.ChatCommands.Add(new Command("ae.butcher.npc", ButcherNPC, "butchernpc", "清除NPC" ));
        Commands.ChatCommands.Add(new Command("ae.butcher.friendly", ButcherFriendly,  "butcherfriendly", "清除友好NPC", "butcherf" ));
    }

    internal static void KillAll(CommandArgs args)
    {
        var players = TShock.Players;
        var array = players;
        var array2 = array;
        foreach (var val in array2)
        {
            if (val != null && !val.Group.HasPermission("ae.killall.bypass") && val != args.Player && TShock.Utils.GetActivePlayerCount() > 1)
            {
                val.DamagePlayer(15000);
            }
        }
        args.Player.SendSuccessMessage("你已击杀所有人!");
        if (!args.Silent)
        {
            TSPlayer.All.SendErrorMessage(args.Player.Name + "使用指令击杀了所有人.");
        }
    }


    internal static void AutoKill(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "自动击杀 <用户名/列表>");
            return;
        }
        if (args.Parameters[0].ToLower() == "list" || args.Parameters[0].ToLower() == "列表")
        {
            var enumerable = from p in autoKilledPlayers
                             orderby p.Name
                             select p.Name;
            if (enumerable.Count() == 0)
            {
                args.Player.SendErrorMessage("自动击杀列表里没有用户.");
            }
            else
            {
                args.Player.SendSuccessMessage(string.Format("正在自动击杀的用户: {0}", string.Join(", ", enumerable)));
            }
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count == 0)
        {
            args.Player.SendErrorMessage("无效的用户.");
        }
        else if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError((IEnumerable<object>) list.Select((TSPlayer p) => p.Name));
        }
        else if (list[0].Group.HasPermission("ae.autokill.bypass"))
        {
            args.Player.SendErrorMessage("你不能将该用户添加进自动击杀列表.");
        }
        else if (!autoKilledPlayers.Contains(list[0]))
        {
            autoKilledPlayers.Add(list[0]);
            args.Player.SendSuccessMessage("已将" + list[0].Name + "添加进自动击杀列表,再次输入可以移除效果.");
            list[0].SendInfoMessage("你已被自动击杀.");
        }
        else
        {
            autoKilledPlayers.Remove(list[0]);
            args.Player.SendSuccessMessage("已将" + list[0].Name + "移除出自动击杀列表.");
            list[0].SendInfoMessage("已移除自动击杀效果.");
        }
    }

    internal static void HealAll(CommandArgs args)
    {
        var players = TShock.Players;
        foreach (var obj in players)
        {
            if (obj != null)
            {
                obj.Heal(600);
            }
        }
        args.Player.SendSuccessMessage("你已治疗所有人!");
        if (!args.Silent)
        {
            TSPlayer.All.SendInfoMessage(args.Player.Name + "治疗了所有人.");
        }
    }

    internal static void AutoHeal(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "自动恢复 <用户名/列表>");
            return;
        }
        if (args.Parameters[0].ToLower() == "list" || args.Parameters[0].ToLower() == "列表")
        {
            var enumerable = from p in autoHealedPlayers
                             orderby p.Name
                             select p.Name;
            if (enumerable.Count() == 0)
            {
                args.Player.SendErrorMessage("自动恢复列表里没有用户.");
            }
            else
            {
                args.Player.SendSuccessMessage(string.Format("正在自动恢复的用户: {0}", string.Join(", ", enumerable)));
            }
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count == 0)
        {
            args.Player.SendErrorMessage("无效的用户.");
        }
        else if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError((IEnumerable<object>) list.Select((TSPlayer p) => p.Name));
        }
        else if (!autoHealedPlayers.Contains(list[0]))
        {
            autoHealedPlayers.Add(list[0]);
            args.Player.SendSuccessMessage("已将" + list[0].Name + "添加进自动恢复列表,再次输入可以移除效果.");
            list[0].SendInfoMessage("你现在正处于自动恢复状态.");
        }
        else
        {
            autoHealedPlayers.Remove(list[0]);
            args.Player.SendSuccessMessage("已将" + list[0].Name + "移除出自动恢复列表.");
            list[0].SendInfoMessage("已移除自动恢复效果.");
        }
    }

    internal static void ForcePvP(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "强制对战 <用户名>");
            return;
        }
        var list = TSPlayer.FindByNameOrID(args.Parameters[0]);
        if (list.Count == 0)
        {
            args.Player.SendErrorMessage("无效的用户.");
        }
        else if (list.Count > 1)
        {
            args.Player.SendMultipleMatchError((IEnumerable<object>) list.Select((TSPlayer p) => p.Name));
        }
        else if (list[0].Group.HasPermission("ae.forcepvp.bypass"))
        {
            args.Player.SendErrorMessage("你不能强制开启该玩家的对战状态.");
        }
        else
        {
            PvPForced[list[0].Index] = !PvPForced[list[0].Index];
            args.Player.SendSuccessMessage(string.Format("{0}的对战状态已{1}.", list[0].Name, PvPForced[list[0].Index] ? "强制开启并锁定" : "不再锁定"));
            list[0].SendInfoMessage(string.Format("你的对战状态已{0}.", PvPForced[list[0].Index] ? "强制开启并锁定" : "不再锁定"));
        }
    }



    internal static void ButcherNPC(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.SendErrorMessage("错误的命令,正确的命令: " + TShock.Config.Settings.CommandSpecifier + "清除NPC <npc ID/名字>");
            return;
        }
        var num = 0;
        var nPCByIdOrName = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
        if (nPCByIdOrName.Count == 0)
        {
            args.Player.SendErrorMessage("无效的NPC.");
            return;
        }
        for (var i = 0; i < Terraria.Main.npc.Length; i++)
        {
            if (Terraria.Main.npc[i].friendly && Terraria.Main.npc[i].type == nPCByIdOrName[0].type)
            {
                num++;
                TSPlayer.Server.StrikeNPC(i, 9999, 0f, 0);
            }
        }
        TSPlayer.All.SendSuccessMessage(string.Format("{0}清除了{1}个{2}{3}.", args.Player.Name, num.ToString(), nPCByIdOrName[0].GivenOrTypeName, (num > 1) ? "" : ""));
    }

    internal static void ButcherFriendly(CommandArgs args)
    {
        var num = 0;
        for (var i = 0; i < Terraria.Main.npc.Length; i++)
        {
            if (Terraria.Main.npc[i].active && Terraria.Main.npc[i].townNPC)
            {
                num++;
                TSPlayer.Server.StrikeNPC(i, 9999, 0f, 0);
            }
        }
        TSPlayer.All.SendInfoMessage(string.Format("{0}已清除{1}个友好NPC{2}.", args.Player.Name, num.ToString(), (num > 1) ? "" : ""));
    }
}
