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
    { CmdName = cmdName; FullCmdName = cmdName; Names = names; Description = description; }
    internal virtual bool NoCanRun(TSPlayer player)
    {
        return
            AllowCheck(player, NeedLoggedIn && !player.IsLoggedIn, $"[{FullCmdName}]请登陆使用此命令") ||
            AllowCheck(player, !AllowServer && !player.RealPlayer, $"[{FullCmdName}]服务器不允许执行此命令") ||
            AllowCheck(player, !(string.IsNullOrEmpty(Permission) || player.HasPermission(Permission)), $"[{FullCmdName}]权限不足");
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
    internal void OutCmdRun(CommandArgs args) => OutCmdRun(args, CmdIndex);
    internal abstract void OutCmdRun(CommandArgs args, int outCount);
    public void SetAllowInfo(AllowInfo info)
    {
        if (info.Permission is not null) Permission = info.Permission;
        if (info.AllowServer.HasValue) AllowServer = info.AllowServer.Value;
        if (info.NeedLoggedIn.HasValue) NeedLoggedIn = info.NeedLoggedIn.Value;
    }
    public virtual TShockAPI.Command GetCommand() => new(Permission, OutCmdRun, Names);
    public virtual TShockAPI.Command GetCommand(params string[] names) => new(Permission, OutCmdRun, names);
    public virtual TShockAPI.Command GetCommand(string permission, string[] names) => new(permission, OutCmdRun, names);
}
public class SubCmdNodeList : SubCmdNode
{
    public List<SubCmdNode> SubCmds = new();
    internal SubCmdNodeList(string cmdName, string description, params string[] names) : base(cmdName, description, names)
    { DescCmd = true; NodeType = NodeType.List; }
    public SubCmdNode? this[string name]
    {
        get
        {
            if (name.Contains('.'))
            {
                int index = name.IndexOf('.');
                var find = SubCmds.Find(x => x.CmdName == name[..index]);
                if (find is not null && find.NodeType == NodeType.List)
                    return ((SubCmdNodeList) find)[name[(index + 1)..]];
                else
                    return null;
            }
            else
                return SubCmds.Find(x => x.CmdName == name);
        }
    }
    public override void Run(CommandArgs args)
    {
        if (NoCanRun(args.Player)) return;
        if (args.Parameters.Count == CmdIndex)
        {
            args.Player.SendInfoMessage($"/{args.Message.Trim()} 的子命令");
            bool have = false;
            foreach (var cmd in SubCmds)
            {
                if (cmd.Enabled)
                {
                    args.Player.SendInfoMessage($"{cmd.Names[0]} {cmd.Description}");
                    have = true;
                }
            }
            if (!have) args.Player.SendInfoMessage($"好像没有可用子命令,问问腐竹是不是配错了");
        }
        else
        {
            string findText = args.Parameters[CmdIndex];
            bool found = false;
            foreach (var cmd in SubCmds)
            {
                if (cmd.Enabled && cmd.Names.Contains(findText))
                {
                    cmd.Run(args);
                    found = true;
                }
            }
            if (!found) args.Player.SendInfoMessage($"未知参数 {findText}");
        }
    }
    internal override void OutCmdRun(CommandArgs args, int outCount)
    {
        if (NoCanRun(args.Player)) return;
        if (args.Parameters.Count == CmdIndex - outCount)
        {
            args.Player.SendInfoMessage($"/{args.Message.Trim()} 的子命令");
            bool have = false;
            foreach (var cmd in SubCmds)
            {
                if (cmd.Enabled)
                {
                    args.Player.SendInfoMessage($"{cmd.Names[0]} {cmd.Description}");
                    have = true;
                }
            }
            if (!have) args.Player.SendInfoMessage($"好像没有可用子命令,问问腐竹是不是配错了");
        }
        else
        {
            string findText = args.Parameters[CmdIndex - outCount];
            bool found = false;
            foreach (var cmd in SubCmds)
            {
                if (cmd.Enabled && cmd.Names.Contains(findText))
                {
                    cmd.OutCmdRun(args, outCount);
                    found = true;
                }
            }
            if (!found) args.Player.SendInfoMessage($"未知参数 {findText}");
        }
    }
    internal void AddNode(SubCmdNode addNode)
    {
        addNode.CmdIndex = CmdIndex + 1;
        addNode.FullCmdName = FullCmdName + "." + addNode.CmdName;
        addNode.Parent = this;
        if (addNode.NodeType == NodeType.Run)
            ((SubCmdNodeRun) addNode).MinArgsCount += addNode.CmdIndex;
        SubCmds.Add(addNode);
    }
    public SubCmdNodeList AddList(string cmdName, string description, params string[] names)
    {
        var addNode = new SubCmdNodeList(cmdName, description, names);
        AddNode(addNode);
        return addNode;
    }
    public SubCmdNodeList AddList(string cmdName, string description) =>
        AddList(cmdName, description, cmdName.ToLower());
    public SubCmdNodeRun AddCmd(SubCmdD runCmd, string cmdName, string description, string[] names, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names, argsHelpText, helpText, minArgsCount);
        AddNode(addNode);
        return addNode;
    }
    public SubCmdNodeRun AddCmd(SubCmdD runCmd, string cmdName, string description, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0) =>
        AddCmd(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText, minArgsCount);
    public SubCmdNodeRun AddCmdA(SubCmdD runCmd, string cmdName, string description, string[] names, string argsHelpText, string? helpText = null)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names, argsHelpText, helpText, argsHelpText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
        AddNode(addNode);
        return addNode;
    }
    public SubCmdNodeRun AddCmdA(SubCmdD runCmd, string cmdName, string description, string argsHelpText, string? helpText = null) =>
        AddCmdA(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText);
    public SubCmdNodeRun AddCmdAA(SubCmdD runCmd, string description, string argsHelpText, string? helpText = null) =>
        AddCmdA(runCmd, runCmd.Method.Name.LastWord(), description, new string[] { runCmd.Method.Name.LastWord().ToLower() }, argsHelpText, helpText);
    public void SetAllNodeRun(AllowInfo info)
    {
        foreach (var node in SubCmds)
        {
            if (node.NodeType == NodeType.List)
                ((SubCmdNodeList) node).SetAllNodeRun(info);
            else if (node.NodeType == NodeType.Run)
                node.SetAllowInfo(info);
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
        NodeType = NodeType.Run;
        RunCmd = runCmd;
        if (argsHelpText is null && helpText is null)
            DireRun = true;
        else
        {
            ArgsHelpText = argsHelpText;
            HelpText = helpText;
        }
        MinArgsCount = minArgsCount;
    }
    public override void Run(CommandArgs args)
    {
        if (NoCanRun(args.Player)) return;
        if (DireRun || args.Parameters.Count >= MinArgsCount)
            RunCmd.Invoke(new SubCmdArgs(CmdName, args, args.Parameters.Skip(CmdIndex).ToList()));
        else
        {
            args.Player.SendInfoMessage($"参数不足,最少需要{MinArgsCount - CmdIndex}个参数");
            bool flag1 = !string.IsNullOrEmpty(ArgsHelpText);
            if (flag1) args.Player.SendInfoMessage($"/{string.Join(' ', args.Message[..args.Message.IndexOf(' ')], string.Join(' ', args.Parameters.GetRange(0, CmdIndex)))} {ArgsHelpText}");
            bool flag2 = !string.IsNullOrEmpty(HelpText);
            if (flag2) args.Player.SendInfoMessage(HelpText);
            if (!(flag1 || flag2)) args.Player.SendErrorMessage("此命令没有帮助文本!");
        }
    }
    internal override void OutCmdRun(CommandArgs args, int outCount)
    {
        if (DireRun || args.Parameters.Count >= (MinArgsCount - outCount))
            RunCmd.Invoke(new SubCmdArgs(CmdName, args, args.Parameters.Skip(CmdIndex - outCount).ToList()));
        else
        {
            args.Player.SendInfoMessage($"参数不足,最少需要{MinArgsCount - CmdIndex}个参数");
            bool flag1 = !string.IsNullOrEmpty(ArgsHelpText);
            if (flag1) args.Player.SendInfoMessage(CmdIndex == outCount ? $"/{args.Message.Trim()} {ArgsHelpText}" : $"/{string.Join(' ', args.Message[..args.Message.IndexOf(' ')], string.Join(' ', args.Parameters.GetRange(0, CmdIndex - outCount)))} {ArgsHelpText}");
            bool flag2 = !string.IsNullOrEmpty(HelpText);
            if (flag2) args.Player.SendInfoMessage(HelpText);
            if (!(flag1 || flag2)) args.Player.SendErrorMessage("此命令没有帮助文本!");
        }
    }
}
public class SubCmdRoot : SubCmdNodeList
{
    public SubCmdRoot(string cmdName) : base(cmdName, "", cmdName.ToLower()) { }
    public override TShockAPI.Command GetCommand() => new(Permission, Run, Names) { HelpText = Description };
    public override TShockAPI.Command GetCommand(params string[] names) => new(Permission, Run, names) { HelpText = Description };
    public override TShockAPI.Command GetCommand(string permission, string[] names) => new(Permission, Run, names) { HelpText = Description };
}
public readonly struct SubCmdArgs
{
    public readonly string CmdName;
    public readonly List<string> Parameters;
    public readonly CommandArgs commandArgs;
    public TSPlayer Player => commandArgs.Player;
    public SubCmdArgs(string cmdName, CommandArgs args, List<string> parameters)
    { CmdName = cmdName; commandArgs = args; Parameters = parameters; }

}
public delegate void SubCmdD(SubCmdArgs args);
public class AllowInfo
{
    public bool? NeedLoggedIn { get; set; }
    public bool? AllowServer { get; set; }
    public string? Permission { get; set; }
    public AllowInfo() { }
    public AllowInfo(bool? needLoggedIn, bool? allowServer, string? permission)
    { NeedLoggedIn = needLoggedIn; AllowServer = allowServer; Permission = permission; }
}
public enum NodeType
{
    List, Run
}