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
}
public class SubCmdNodeList : SubCmdNode
{
    public List<SubCmdNode> SubCmds = new();
    internal SubCmdNodeList(string cmdName, string description, params string[] names) : base(cmdName, description, names)
    { DescCmd = true; }
    public SubCmdNode? this[string name]
    {
        get { return SubCmds.Find(x => x.CmdName == name); }
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
    internal void Add(SubCmdNode addNode)
    {
        addNode.CmdIndex = CmdIndex + 1;
        addNode.FullCmdName = FullCmdName + "." + addNode.CmdName;
        addNode.Parent = this;
        var nodeRun = addNode as SubCmdNodeRun;
        if (nodeRun is not null)
            nodeRun.MinArgsCount += nodeRun.CmdIndex;
        SubCmds.Add(addNode);
    }
    public SubCmdNodeList Add(string cmdName, string description, params string[] names)
    {
        var addNode = new SubCmdNodeList(cmdName, description, names);
        Add(addNode);
        return addNode;
    }
    public SubCmdNodeList Add(string cmdName, string description) =>
        Add(cmdName, description, cmdName.ToLower());
    public SubCmdNodeRun Add(SubCmdD runCmd, string cmdName, string description, string[] names, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names,argsHelpText, helpText, minArgsCount);
        Add(addNode);
        return addNode;
    }
    public SubCmdNodeRun Add(SubCmdD runCmd, string cmdName, string description, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0) =>
        Add(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText, minArgsCount);
    public SubCmdNodeRun AddA(SubCmdD runCmd, string cmdName, string description, string[] names, string argsHelpText, string? helpText = null)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names, argsHelpText, helpText, argsHelpText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
        Add(addNode);
        return addNode;
    }
    public SubCmdNodeRun AddA(SubCmdD runCmd, string cmdName, string description, string argsHelpText, string? helpText = null) =>
        AddA(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText);
}
public class SubCmdNodeRun : SubCmdNode
{
    internal SubCmdD RunCmd;
    internal bool DireRun = false;
    internal int MinArgsCount;
    public string? ArgsHelpText, HelpText;
    internal SubCmdNodeRun(SubCmdD runCmd, string cmdName, string description, string[] names, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0) : base(cmdName, description, names)
    {
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
}
public class SubCmdRoot : SubCmdNodeList
{
    public SubCmdRoot(string cmdName) : base(cmdName, "", Array.Empty<string>()) { }
}
public readonly struct SubCmdArgs
{
    public readonly string CmdName;
    public readonly List<string> Parameters;
    public readonly CommandArgs commandArgs;
    internal SubCmdArgs(string cmdName, CommandArgs args, List<string> parameters)
    { CmdName = cmdName; commandArgs = args; Parameters = parameters; }

}
public delegate void SubCmdD(SubCmdArgs args);