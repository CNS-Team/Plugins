using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        public DateTime LastTiem { get; set; }

        public CC(string name, string cmd)
        {
            this.Name = name;
            this.Cmd = cmd;
            this.LastTiem = DateTime.UtcNow;
        }
    }

    public override string Name => "简短指令改良版";

    public override string Author => "GK 超级改良";

    public override Version Version => new Version(1, 0, 1, 7);

    public override string Description => "由GK改良的简短指令插件！";

    public static ConfigFile LConfig { get; set; }

    internal static string LConfigPath => Path.Combine(TShock.SavePath, "简短指令.json");

    private List<CC> LCC { get; set; }

    public static bool jump { get; set; }

    public Plugin(Main game)
        : base(game)
    {
        this.LCC = new List<CC>();
        LConfig = new ConfigFile();
        this.Order += 1000000;
    }

    private void RC()
    {
        try
        {
            if (!File.Exists(LConfigPath))
            {
                TShock.Log.ConsoleError("未找到简短指令配置，已为您创建！");
            }
            LConfig = ConfigFile.Read(LConfigPath);
            LConfig.Write(LConfigPath);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("简短指令配置读取错误:" + ex.ToString());
        }
    }

    public override void Initialize()
    {
        //IL_000c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0022: Expected O, but got Unknown
        //IL_002a: Unknown result type (might be due to invalid IL or missing references)
        //IL_0034: Expected O, but got Unknown
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        PlayerHooks.PlayerCommand += new PlayerHooks.PlayerCommandD(this.OnChat);
    }

    protected override void Dispose(bool disposing)
    {
        //IL_0012: Unknown result type (might be due to invalid IL or missing references)
        //IL_0028: Expected O, but got Unknown
        //IL_0030: Unknown result type (might be due to invalid IL or missing references)
        //IL_003a: Expected O, but got Unknown
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
        if (Commands.TShockCommands.Count((p) => p.Name == args.CommandName) > 0 || Commands.ChatCommands.Count((p) => p.Name == args.CommandName) > 0)
        {
            for (var j = 0; j < LConfig.Commands.Count; j++)
            {
                var O = LConfig.Commands[j].SourceCommand;
                if (this.SR(ref O, args.Player.Name, args.Parameters, LConfig.Commands[j].Supplement) && O == args.CommandText && LConfig.Commands[j].NotSource)
                {
                    args.Player.SendErrorMessage("此指令已被禁用，故无法使用！");
                    args.Handled = true;
                    return;
                }
            }
        }
        int i;
        for (i = 0; i < LConfig.Commands.Count; i++)
        {
            if (LConfig.Commands[i].NewCommand == "" || !(LConfig.Commands[i].NewCommand == args.CommandName))
            {
                continue;
            }
            var O2 = LConfig.Commands[i].SourceCommand;
            if (!this.SR(ref O2, args.Player.Name, args.Parameters, LConfig.Commands[i].Supplement))
            {
                continue;
            }
            if (args.Player.Index >= 0)
            {
                if (LConfig.Commands[i].Death == 1 && (args.Player.Dead || args.Player.TPlayer.statLife < 1))
                {
                    args.Player.SendErrorMessage("此指令要求你必须活着才能使用！");
                    ((HandledEventArgs) (object) args).Handled = true;
                    break;
                }
                if (LConfig.Commands[i].Death == -1 && (!args.Player.Dead || args.Player.TPlayer.statLife > 0))
                {
                    args.Player.SendErrorMessage("此指令要求你必须死亡才能使用！");
                    ((HandledEventArgs) (object) args).Handled = true;
                    break;
                }
                var num = this.CJ(args.Player.Name, LConfig.Commands[i].SourceCommand, LConfig.Commands[i].CD, LConfig.Commands[i].ShareCD);
                if (num > 0)
                {
                    args.Player.SendErrorMessage("此指令正在冷却，还有{0}秒才能使用！", new object[1] { num });
                    ((HandledEventArgs) (object) args).Handled = true;
                    break;
                }
                jump = true;
                if (Commands.HandleCommand(args.Player, args.CommandPrefix + O2))
                {
                    lock (this.LCC)
                    {
                        if (!this.LCC.Exists((t) => t.Name == args.Player.Name && t.Cmd == LConfig.Commands[i].NewCommand))
                        {
                            this.LCC.Add(new CC(args.Player.Name, LConfig.Commands[i].NewCommand));
                        }
                    }
                }
            }
            else
            {
                jump = true;
                Commands.HandleCommand(args.Player, args.CommandPrefix + O2);
            }
            ((HandledEventArgs) (object) args).Handled = true;
            break;
        }
    }

    private int CJ(string name, string Cmd, int C, bool share)
    {
        lock (this.LCC)
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
                else if (this.LCC[i].Cmd != Cmd || this.LCC[i].Name != name)
                {
                    continue;
                }
                var num = (int) (DateTime.UtcNow - this.LCC[i].LastTiem).TotalSeconds;
                num = C - num;
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

    private bool SR(ref string O, string N, List<string> P, bool R)
    {
        var text = "";
        for (var i = 0; i < P.Count; i++)
        {
            var text2 = "{" + i + "}";
            if (O.Contains(text2))
            {
                O = O.Replace(text2, P[i]);
                continue;
            }
            if (R)
            {
                text = text + " " + P[i];
                continue;
            }
            return false;
        }
        var text3 = "{player}";
        if (O.Contains(text3))
        {
            O = O.Replace(text3, N);
        }
        if (O.Contains("{") && O.Contains("}"))
        {
            return false;
        }
        if (R)
        {
            O += text;
        }
        return true;
    }
}