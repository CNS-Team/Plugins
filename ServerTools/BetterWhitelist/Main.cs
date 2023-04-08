using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;

namespace ServerTools.BetterWhitelist;
public class Main
{
    internal static void OnJoin(JoinEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }
        else
        {
            var ply = TShock.Players[args.Who];
            if (ply != null)
            {
                if (!Plugin.Config.BwlList.Contains(ply.Name))
                {
                    ply.Disconnect(Plugin.Config.BwlPrompt);
                }
            }
        }
    }

    internal static void Acmd()
    {
        Commands.ChatCommands.Add(new Command("bwl.use", Bwl, "bwl"));
    }

    private static void Bwl(CommandArgs args)
    {
        if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "add")
        {
            if (Plugin.Config.BwlList.Contains(args.Parameters[1]))
            {
                args.Player.SendErrorMessage($"用户名 {args.Parameters[1]} 已经被添加过了");
            }
            else
            {
                Plugin.Config.BwlList.Add(args.Parameters[1]);
                args.Player.SendSuccessMessage("添加成功!");
            }
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "del")
        {
            if (Plugin.Config.BwlList.Contains(args.Parameters[1]))
            {
                Plugin.Config.BwlList.Remove(args.Parameters[1]);
                args.Player.SendErrorMessage($"用户名名 {args.Parameters[1]} 成功移出白名单!");
            }
            else
            {
                args.Player.SendSuccessMessage("用户名不存在");
            }
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
        {
            if (Plugin.Config.BwlList.Count == 0)
            {
                args.Player.SendInfoMessage("白名单列表空空如也");
            }
            else
            {
                Plugin.Config.BwlList.ForEach(x => args.Player.SendInfoMessage(x));
            }
        }
        else if (args.Parameters.Count == 0 && args.Parameters[0].ToLower() == "reset")
        {
            Plugin.Config.BwlList.Clear();
            args.Player.SendSuccessMessage("白名单已重置!");
        }
        else
        {
            args.Player.SendInfoMessage("/bwl add <Name>");
            args.Player.SendInfoMessage("/bwl del <Name>");
            args.Player.SendInfoMessage("/bwl list");
            args.Player.SendInfoMessage("/bwl reset");
        }

        Plugin.Config.Write(Plugin.PATH);
    }
}
