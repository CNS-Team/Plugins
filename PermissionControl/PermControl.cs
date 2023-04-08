// PermissionControl, Version=1.3.3.1, Culture=neutral, PublicKeyToken=null
// PermissionControl.PermControl
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PermissionControl;
[ApiVersion(2, 1)]
public class PermControl : TerrariaPlugin
{
    public static string Tag => TShock.Utils.ColorTag("PermControl:", Color.Teal);

    public override string Name => "PermControl";

    public override string Author => "Zaicon制作,nnt升级/汉化";

    public override string Description => "Searches for commands/permissions within groups.";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public PermControl(Main game)
        : base(game)
    {
        base.Order = 1;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
        }
        base.Dispose(Disposing);
    }

    private void OnInitialize(EventArgs args)
    {
        Commands.ChatCommands.Add(new Command("permcontrol", this.SCommand, "scommand", "查命令")
        {
            HelpText = "Searches for a specified command."
        });
        Commands.ChatCommands.Add(new Command("permcontrol", this.SearchPerm, "sperm", "查权限")
        {
            HelpText = "Searches for a specified permission."
        });
        Commands.ChatCommands.Add(new Command("permcontrol", this.SearchCommandInGroup, "sgcommand", "查组命令")
        {
            HelpText = "Provides a list of groups with a certain command."
        });
        Commands.ChatCommands.Add(new Command("permcontrol", this.SearchPermInGroup, "sgperm", "查组权限")
        {
            HelpText = "Provides a list of groups with a certain permission."
        });
        Commands.ChatCommands.Add(new Command("permcontrol", this.findPlugins, "pluginlist", "插件权限")
        {
            HelpText = "Provides a list of permissions from plugin-provided commands."
        });
    }

    public void SCommand(CommandArgs args)
    {
        if (args.Parameters.Count > 0)
        {
            List<string> list = new List<string>();
            foreach (Command chatCommand in Commands.ChatCommands)
            {
                for (int i = 0; i < chatCommand.Permissions.Count; i++)
                {
                    if (!args.Player.Group.HasPermission(chatCommand.Permissions[i]))
                    {
                        continue;
                    }
                    foreach (string name in chatCommand.Names)
                    {
                        bool flag = true;
                        foreach (string parameter in args.Parameters)
                        {
                            if (!name.Contains(parameter))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag && !list.Contains(name))
                        {
                            list.Add(chatCommand.Name);
                        }
                    }
                }
            }
            if (list.Count <= 0)
            {
                args.Player.PluginErrorMessage("没有找到匹配的命令.");
                return;
            }
            args.Player.PluginInfoMessage("已找到与搜索匹配的命令:");
            for (int j = 0; j < list.Count && j < 6; j++)
            {
                string text = "";
                for (int k = 0; k < list.Count - (j * 5) && k < 5; k++)
                {
                    text = ((j * 5) + k + 1 >= list.Count) ? (text + list[(j * 5) + k] + ".") : (text + list[(j * 5) + k] + ", ");
                }
                args.Player.SendInfoMessage(text);
            }
        }
        else
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier) + "scommand <命令>");
        }
    }

    public void SearchPerm(CommandArgs args)
    {
        if (args.Parameters.Count > 0)
        {
            foreach (Command chatCommand in Commands.ChatCommands)
            {
                if (chatCommand.Names.Contains(args.Parameters[0]))
                {
                    args.Player.PluginInfoMessage(string.Format("{0}命令对应的权限为{1}", chatCommand.Name, (chatCommand.Permissions.Count > 0) ? chatCommand.Permissions[0] : "无特殊权限"));
                    return;
                }
            }
            args.Player.PluginErrorMessage("命令没有找到.");
        }
        else
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier) + "sperm <命令>");
        }
    }

    public void SearchCommandInGroup(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier) + "sgcommand <命令>");
            return;
        }
        string text = string.Join(" ", args.Parameters);
        List<Command> therealcommand = new List<Command>();
        foreach (Command chatCommand in Commands.ChatCommands)
        {
            if (chatCommand.Name.Contains(text))
            {
                therealcommand.Add(chatCommand);
            }
            if (chatCommand.Name == text)
            {
                therealcommand.Clear();
                therealcommand.Add(chatCommand);
                break;
            }
        }
        if (therealcommand.Count < 1)
        {
            args.Player.PluginErrorMessage("命令没有找到.");
            return;
        }
        if (therealcommand.Count > 1)
        {
            args.Player.SendMultipleMatchError(therealcommand.Select((Command p) => p.Name));
            return;
        }
        IEnumerable<string> values = from thegroup in TShock.Groups
                                     where therealcommand[0].Permissions.Count <= 0 || thegroup.HasPermission(therealcommand[0].Permissions[0])
                                     select thegroup.Name;
        args.Player.PluginInfoMessage("以下组拥有使用" + therealcommand[0].Name + "命令的权限:");
        args.Player.SendInfoMessage(string.Join(", ", values));
        Command command = therealcommand[0];
        List<string> list = new List<string>();
        foreach (Group group in TShock.Groups)
        {
            try
            {
                string permission = command.Permissions[0];
                if (group.HasPermission(permission))
                {
                    list.Add(group.Name);
                }
            }
            catch
            {
                list.Add(group.Name);
            }
        }
        string text2 = "";
        for (int i = 0; i < list.Count; i++)
        {
            text2 += list[i];
            if (i + 1 < list.Count)
            {
                text2 += " ";
            }
        }
        args.Player.PluginInfoMessage("以下组拥有使用" + therealcommand[0].Name + "命令的权限:");
        args.Player.SendInfoMessage(text2);
    }

    private void SearchPermInGroup(CommandArgs args)
    {
        if (args.Parameters.Count != 1)
        {
            args.Player.PluginErrorMessage("错误的命令,正确的命令: " + (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier) + "sgperm <权限名>");
            return;
        }
        string perms = args.Parameters[0];
        IEnumerable<string> values = from thegroup in TShock.Groups
                                     where thegroup.HasPermission(perms)
                                     select thegroup.Name;
        args.Player.PluginInfoMessage("拥有" + perms + "权限的组有:");
        args.Player.SendInfoMessage(string.Join(", ", values));
    }

    private void findPlugins(CommandArgs args)
    {
        List<string> list = new List<string>();
        foreach (Command chatCommand in Commands.ChatCommands)
        {
            if (chatCommand.Permissions.Count > 0)
            {
                string[] array = chatCommand.Permissions[0].Split('.');
                if (array[0] != "tshock" && !list.Contains(array[0]))
                {
                    list.Add(array[0]);
                }
            }
        }
        args.Player.PluginInfoMessage("不以tshock开头的命令权限有:");
        args.Player.SendInfoMessage(string.Join(", ", list));
    }
}
