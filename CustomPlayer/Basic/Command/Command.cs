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
}
public class SubCmdNodeList : SubCmdNode
{
    public List<SubCmdNode> SubCmds = new();
    internal SubCmdNodeList(string cmdName, string description, params string[] names) : base(cmdName, description, names)
    { this.DescCmd = true; }
    public SubCmdNode? this[string name] => this.SubCmds.Find(x => x.CmdName == name);
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
    internal void Add(SubCmdNode addNode)
    {
        addNode.CmdIndex = this.CmdIndex + 1;
        addNode.FullCmdName = this.FullCmdName + "." + addNode.CmdName;
        addNode.Parent = this;
        var nodeRun = addNode as SubCmdNodeRun;
        if (nodeRun is not null)
        {
            nodeRun.MinArgsCount += nodeRun.CmdIndex;
        }

        this.SubCmds.Add(addNode);
    }
    public SubCmdNodeList Add(string cmdName, string description, params string[] names)
    {
        var addNode = new SubCmdNodeList(cmdName, description, names);
        this.Add(addNode);
        return addNode;
    }
    public SubCmdNodeList Add(string cmdName, string description)
    {
        return this.Add(cmdName, description, cmdName.ToLower());
    }

    public SubCmdNodeRun Add(SubCmdD runCmd, string cmdName, string description, string[] names, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names, argsHelpText, helpText, minArgsCount);
        this.Add(addNode);
        return addNode;
    }
    public SubCmdNodeRun Add(SubCmdD runCmd, string cmdName, string description, string? argsHelpText = null, string? helpText = null, int minArgsCount = 0)
    {
        return this.Add(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText, minArgsCount);
    }

    public SubCmdNodeRun AddA(SubCmdD runCmd, string cmdName, string description, string[] names, string argsHelpText, string? helpText = null)
    {
        var addNode = new SubCmdNodeRun(runCmd, cmdName, description, names, argsHelpText, helpText, argsHelpText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
        this.Add(addNode);
        return addNode;
    }
    public SubCmdNodeRun AddA(SubCmdD runCmd, string cmdName, string description, string argsHelpText, string? helpText = null)
    {
        return this.AddA(runCmd, cmdName, description, new string[] { cmdName.ToLower() }, argsHelpText, helpText);
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
    { this.CmdName = cmdName; this.commandArgs = args; this.Parameters = parameters; }

}
public delegate void SubCmdD(SubCmdArgs args);