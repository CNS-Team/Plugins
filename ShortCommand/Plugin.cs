using Org.BouncyCastle.Asn1.CryptoPro;
using System.ComponentModel;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ShortCommand;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public class CC
    {
        public string Name { get; set; }

        public string Cmd { get; set; }

        public DateTime LastTime { get; set; }

        public CC(string name, string cmd)
        {
            this.Name = name;
            this.Cmd = cmd;
            this.LastTime = DateTime.UtcNow;
        }
    }

    public override string Name => "简短指令改良版";

    public override string Author => "GK 超级改良";

    public override Version Version => new Version(1, 0, 1, 7);

    public override string Description => "由GK改良的简短指令插件！";

    private Config Config { get; set; }

    internal string PATH = Path.Combine(TShock.SavePath, "简短指令.json");

    private List<CC> LCC { get; set; }

    public static bool jump { get; set; }

    public Plugin(Main game)
        : base(game)
    {
        this.LCC = new List<CC>();
        this.Config = new Config();
        this.Order += 1000000;
    }

    private void RC()
    {
        try
        {
            if (!File.Exists(this.PATH))
            {
                this.Config.Commands.Add(new CMD());
                TShock.Log.ConsoleError("未找到简短指令配置，已为您创建！");
            }
            else
            { 
                this.Config = Config.Read(this.PATH);
            }
            this.Config.Write(this.PATH);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("简短指令配置读取错误:" + ex.ToString());
        }
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        PlayerHooks.PlayerCommand += new PlayerHooks.PlayerCommandD(this.OnChat);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            PlayerHooks.PlayerCommand -= new PlayerHooks.PlayerCommandD(this.OnChat);
        }
        this.Dispose(disposing);
    }

    private void OnInitialize(EventArgs args)
    {
        this.RC();
    }

    private void OnChat(PlayerCommandEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }
        if (jump)
        {
            jump = false;
            return;
        }
        if (Commands.TShockCommands.Any(p => p.Name == args.CommandName) || Commands.ChatCommands.Any(p => p.Name == args.CommandName))
        {
            foreach (var cmd in this.Config.Commands)
            {
                var sourcecmd = cmd.SourceCommand;
                if (this.SR(ref sourcecmd, args.Player.Name, args.Parameters, cmd.Supplement) && sourcecmd == args.CommandText && cmd.NotSource)
                {
                    args.Player.SendErrorMessage("此指令已被禁用，故无法使用！");
                    args.Handled = true;
                    return;
                }

                if (cmd.NewCommand == "" || !(cmd.NewCommand == args.CommandName))
                {
                    continue;
                }
                var sourceCmd = cmd.SourceCommand;
                if (!this.SR(ref sourceCmd, args.Player.Name, args.Parameters, cmd.Supplement))
                {
                    continue;
                }
                if (args.Player.Index >= 0)
                {
                    if (!cmd.Death && (args.Player.Dead || args.Player.TPlayer.statLife < 1))
                    {
                        args.Player.SendErrorMessage("此指令要求你必须活着才能使用！");
                        args.Handled = true;
                        break;
                    }
                    if (cmd.Death && (!args.Player.Dead || args.Player.TPlayer.statLife > 0))
                    {
                        args.Player.SendErrorMessage("此指令要求你必须死亡才能使用！");
                        args.Handled = true;
                        break;
                    }
                    var num = this.GetCD(args.Player.Name, cmd.SourceCommand, cmd.CD, cmd.ShareCD);
                    if (num > 0)
                    {
                        args.Player.SendErrorMessage("此指令正在冷却，还有{0}秒才能使用！", num);
                        args.Handled = true;
                        break;
                    }
                    jump = true;
                    if (Commands.HandleCommand(args.Player, args.CommandPrefix + sourceCmd))
                    {
                        if (!this.LCC.Exists((t) => t.Name == args.Player.Name && t.Cmd == cmd.NewCommand))
                        {
                            this.LCC.Add(new CC(args.Player.Name, cmd.NewCommand));
                        }
                    }
                }
                else
                {
                    jump = true;
                    Commands.HandleCommand(args.Player, args.CommandPrefix + sourceCmd);
                }
                args.Handled = true;
                break;
            }
        }
    }

    private int GetCD(string plyName, string Cmd, int CD, bool share)
    {
        for (var i = 0; i < this.LCC.Count; i++)
        {
            if (share)
            {
                if (this.LCC[i].Cmd != Cmd)
                {
                    continue;
                }
            }
            else if (this.LCC[i].Cmd != Cmd || this.LCC[i].Name != plyName)
            {
                continue;
            }
            else
            { 
                var num = (int) (DateTime.UtcNow - this.LCC[i].LastTime).TotalSeconds;
                num = CD - num;
                if (num > 0)
                {
                    return num;
                }
                this.LCC.RemoveAt(i);
                return 0;
            }
        }
        return 0;
    }

    private bool SR(ref string cmd, string plyName, List<string> cmdArgs, bool Supplement)
    {
        var text = "";
        for (var i = 0; i < cmdArgs.Count; i++)
        {
            var text2 = $"{i}";
            if (cmd.Contains(text2))
            {
                cmd = cmd.Replace(text2, cmdArgs[i]);
                continue;
            }
            if (Supplement)
            {
                text = $"{text} {cmdArgs[i]}";
                continue;
            }
            return false;
        }
        var text3 = "{player}";
        if (cmd.Contains(text3))
        {
            cmd = cmd.Replace(text3, plyName);
        }
        if (cmd.Contains("{") && cmd.Contains("}"))
        {
            return false;
        }
        if (Supplement)
        {
            cmd += text;
        }
        return true;
    }
}