using TShockAPI;

namespace VBY.Basic.Command;

public abstract class SubCmdNode
{
    public int CmdIndex { get; internal set; } = 0;
    public bool AllowServer = true;
    public bool DescCmd;
    public string CmdName, FullCmdName;
    public bool NeedLoggedIn;
    public string Permission = string.Empty;
    public bool Enabled = true;
    public string[] Names;
    public string Description;
    public NodeType NodeType { get; protected set; }
    public SubCmdNodeList? Parent { get; internal set; } = null;
    internal SubCmdNode(string cmdName, string description, params string[] names)
    { this.CmdName = cmdName; this.FullCmdName = cmdName; this.Names = names; this.Description = description; }
    internal virtual bool NoCanRun(TSPlayer player)
    {
        return
            AllowCheck(player, this.NeedLoggedIn && !player.IsLoggedIn, $"[{this.FullCmdName}]请登陆使用此命令") ||
            AllowCheck(player, !this.AllowServer && !player.RealPlayer, $"[{this.FullCmdName}]服务器不允许执行此命令") ||
            AllowCheck(player, !(string.IsNullOrEmpty(this.Permission) || player.HasPermission(this.Permission)), $"[{this.FullCmdName}]权限不足");
    }
    internal static bool AllowCheck(TSPlayer player, bool noAllow, string error)
    {
        if (noAllow)
        {
            player.SendErrorMessage(error);
            return true;
        }
        return false;
    }
    public abstract void Run(CommandArgs args);
    internal void OutCmdRun(CommandArgs args)
    {
        this.OutCmdRun(args, this.CmdIndex);
    }

    internal abstract void OutCmdRun(CommandArgs args, int outCount);
    public void SetAllowInfo(AllowInfo info)
    {
        if (info.Permission is not null)
        {
            this.Permission = info.Permission;
        }

        if (info.AllowServer.HasValue)
        {
            this.AllowServer = info.AllowServer.Value;
        }

        if (info.NeedLoggedIn.HasValue)
        {
            this.NeedLoggedIn = info.NeedLoggedIn.Value;
        }
    }
    public virtual TShockAPI.Command GetCommand()
    {
        return new(this.Permission, this.OutCmdRun, this.Names);
    }

    public virtual TShockAPI.Command GetCommand(params string[] names)
    {
        return new(this.Permission, this.OutCmdRun, names);
    }

    public virtual TShockAPI.Command GetCommand(string permission, string[] names)
    {
        return new(permission, this.OutCmdRun, names);
    }
}
public class SubCmdNodeList : SubCmdNode
{
    public List<SubCmdNode> SubCmds = new();
    internal SubCmdNodeList(string cmdName, string description, params string[] names) : base(cmdName, description, names)
    { this.DescCmd = true; this.NodeType = NodeType.List; }
    public SubCmdNode this[string name]
    {
        get
        {
            SubCmdNode result = this;
            if (name.Contains('.'))
            {
                var names = name.Split('.', StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < names.Length; i++)
                {
                    result = ((SubCmdNodeList)result)[names[i]];
                }
            }
            else
            {
                result = this.SubCmds.Find(x => x.CmdName == name)!;
                if (result is null)
                {
                    throw new Exception($"{this.FullCmdName}'s SubCmd '{name}' not find");
                }
            }
            return result;
        }
    }
    public override void Run(CommandArgs args)
    {
        if (this.NoCanRun(args.Player))
        {
            return;
        }

        if (args.Parameters.Count == this.CmdIndex)
        {
            args.Player.SendInfoMessage($"/{args.Message.Trim()} 的子命令");
            var have = false;
            foreach (var cmd in this.SubCmds)
            {
                if (cmd.Enabled)
                {
                    args.Player.SendInfoMessage($"{cmd.Names[0]} {cmd.Description}");
                    have = true;
                }
            }
            if (!have)
            {
                args.Player.SendInfoMessage($"好像没有可用子命令,问问腐竹是不是配错了");
            }
        }
        else
        {
            var findText = args.Parameters[this.CmdIndex];
            var found = false;
            foreach (var cmd in this.SubCmds)
            {
                if (cmd.Enabled && cmd.Names.Contains(findText))
                {
                    cmd.Run(args);
                    found = true;
                }
            }
            if (!found)
            {
                args.Player.SendInfoMessage($"未知参数 {findText}");
            }
        }
    }
    internal override void OutCmdRun(CommandArgs args, int outCount)
    {
        if (this.NoCanRun(args.Player))
        {
            return;
        }

        if (args.Parameters.Count == this.CmdIndex - outCount)
        {
            args.Player.SendInfoMessage($"/{args.Message.Trim()} 的子命令");
            var have = false;
            foreach (var cmd in this.SubCmds)
            {
                if (cmd.Enabled)
                {
                    args.Player.SendInfoMessage($"{cmd.Names[0]} {cmd.Description}");
                    have = true;
                }
            }
            if (!have)
            {
                args.Player.SendInfoMessage($"好像没有可用子命令,问问腐竹是不是配错了");
            }
        }
        else
        {
            var findText = args.Parameters[this.CmdIndex - outCount];
            var found = false;
            foreach (var cmd in this.SubCmds)
            {
                if (cmd.Enabled && cmd.Names.Contains(findText))
                {
                    cmd.OutCmdRun(args, outCount);
                    found = true;
                }
            }
            if (!found)
            {
                args.Player.SendInfoMessage($"未知参数 {findText}");
            }
        }
    }
    internal void AddNode(SubCmdNode addNode)
    {
        addNode.CmdIndex = this.CmdIndex + 1;
        addNode.FullCmdName = this.FullCmdName + "." + addNode.CmdName;
        addNode.Parent = this;
        if (addNode.NodeType == NodeType.Run)
        {
            ((SubCmdNodeRun) addNode).MinArgsCount += addNode.CmdIndex;
        }

        this.SubCmds.Add(addNode);
    }
    public SubCmdNodeList AddList(string cmdName, string description, params string[] names)
    {
        var addNode = new SubCmdNodeList(cmdName, description, names);
        this.AddNode(addNode);
        return addNode;
    }
    public SubCmdNodeList AddList(string cmdName, string description)
    {
        return this.AddList(cmdName, description, cmdName.ToLower());
    }

    public SubCmdNodeRun AddCmd(SubCmdD runCmd, string cmdName, string description, string[] names, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names, argsHelpText, helpText, minArgsCount);
        this.AddNode(addNode);
        return addNode;
    }
    public SubCmdNodeRun AddCmd(SubCmdD runCmd, string cmdName, string description, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0)
    {
        return this.AddCmd(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText, minArgsCount);
    }

    public SubCmdNodeRun AddCmdA(SubCmdD runCmd, string cmdName, string description, string[] names, string argsHelpText, string? helpText = null)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names, argsHelpText, helpText, argsHelpText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
        this.AddNode(addNode);
        return addNode;
    }
    public SubCmdNodeRun AddCmdA(SubCmdD runCmd, string cmdName, string description, string argsHelpText, string? helpText = null)
    {
        return this.AddCmdA(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText);
    }

    public SubCmdNodeRun AddCmdAA(SubCmdD runCmd, string description, string argsHelpText, string? helpText = null)
    {
        return this.AddCmdA(runCmd, runCmd.Method.Name.LastWord(), description, new string[] { runCmd.Method.Name.LastWord().ToLower() }, argsHelpText, helpText);
    }

    public void SetAllNodeRun(AllowInfo info)
    {
        foreach (var node in this.SubCmds)
        {
            if (node.NodeType == NodeType.List)
            {
                ((SubCmdNodeList) node).SetAllNodeRun(info);
            }
            else if (node.NodeType == NodeType.Run)
            {
                node.SetAllowInfo(info);
            }
        }
    }
}
public class SubCmdNodeRun : SubCmdNode
{
    internal SubCmdD RunCmd;
    internal bool DireRun = false;
    internal int MinArgsCount;
    public string? ArgsHelpText, HelpText;
    internal SubCmdNodeRun(SubCmdD runCmd, string cmdName, string description, string[] names, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0) : base(cmdName, description, names)
    {
        this.NodeType = NodeType.Run;
        this.RunCmd = runCmd;
        if (argsHelpText is null && helpText is null)
        {
            this.DireRun = true;
        }
        else
        {
            this.ArgsHelpText = argsHelpText;
            this.HelpText = helpText;
        }
        this.MinArgsCount = minArgsCount;
    }
    public override void Run(CommandArgs args)
    {
        if (this.NoCanRun(args.Player))
        {
            return;
        }

        if (this.DireRun || args.Parameters.Count >= this.MinArgsCount)
        {
            this.RunCmd.Invoke(new SubCmdArgs(this.CmdName, args, args.Parameters.Skip(this.CmdIndex).ToList()));
        }
        else
        {
            args.Player.SendInfoMessage($"参数不足,最少需要{this.MinArgsCount - this.CmdIndex}个参数");
            var flag1 = !string.IsNullOrEmpty(this.ArgsHelpText);
            if (flag1)
            {
                args.Player.SendInfoMessage($"/{string.Join(' ', args.Message[..args.Message.IndexOf(' ')], string.Join(' ', args.Parameters.GetRange(0, this.CmdIndex)))} {this.ArgsHelpText}");
            }

            var flag2 = !string.IsNullOrEmpty(this.HelpText);
            if (flag2)
            {
                args.Player.SendInfoMessage(this.HelpText);
            }

            if (!(flag1 || flag2))
            {
                args.Player.SendErrorMessage("此命令没有帮助文本!");
            }
        }
    }
    internal override void OutCmdRun(CommandArgs args, int outCount)
    {
        if (this.DireRun || args.Parameters.Count >= (this.MinArgsCount - outCount))
        {
            this.RunCmd.Invoke(new SubCmdArgs(this.CmdName, args, args.Parameters.Skip(this.CmdIndex - outCount).ToList()));
        }
        else
        {
            args.Player.SendInfoMessage($"参数不足,最少需要{this.MinArgsCount - this.CmdIndex}个参数");
            var flag1 = !string.IsNullOrEmpty(this.ArgsHelpText);
            if (flag1)
            {
                args.Player.SendInfoMessage(this.CmdIndex == outCount ? $"/{args.Message.Trim()} {this.ArgsHelpText}" : $"/{string.Join(' ', args.Message[..args.Message.IndexOf(' ')], string.Join(' ', args.Parameters.GetRange(0, this.CmdIndex - outCount)))} {this.ArgsHelpText}");
            }

            var flag2 = !string.IsNullOrEmpty(this.HelpText);
            if (flag2)
            {
                args.Player.SendInfoMessage(this.HelpText);
            }

            if (!(flag1 || flag2))
            {
                args.Player.SendErrorMessage("此命令没有帮助文本!");
            }
        }
    }
}
public class SubCmdRoot : SubCmdNodeList
{
    public SubCmdRoot(string cmdName) : base(cmdName, "", cmdName.ToLower()) { }
    public override TShockAPI.Command GetCommand()
    {
        return new(this.Permission, this.Run, this.Names) { HelpText = Description };
    }

    public override TShockAPI.Command GetCommand(params string[] names)
    {
        return new(this.Permission, this.Run, names) { HelpText = Description };
    }

    public override TShockAPI.Command GetCommand(string permission, string[] names)
    {
        return new(this.Permission, this.Run, names) { HelpText = Description };
    }
}
public readonly struct SubCmdArgs
{
    public readonly string CmdName;
    public readonly List<string> Parameters;
    public readonly CommandArgs commandArgs;
    public TSPlayer Player => this.commandArgs.Player;
    public SubCmdArgs(string cmdName, CommandArgs args, List<string> parameters)
    { this.CmdName = cmdName; this.commandArgs = args; this.Parameters = parameters; }

}
public delegate void SubCmdD(SubCmdArgs args);
public class AllowInfo
{
    public bool? NeedLoggedIn { get; set; }
    public bool? AllowServer { get; set; }
    public string? Permission { get; set; }
    public AllowInfo() { }
    public AllowInfo(bool? needLoggedIn, bool? allowServer, string? permission)
    { this.NeedLoggedIn = needLoggedIn; this.AllowServer = allowServer; this.Permission = permission; }
}
public enum NodeType
{
    List, Run
}