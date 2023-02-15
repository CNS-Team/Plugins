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
            Name = name;
            Cmd = cmd;
            LastTiem = DateTime.UtcNow;
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
        LCC = new List<CC>();
        LConfig = new ConfigFile();
        this.Order = this.Order + 1000000;
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
        ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        PlayerHooks.PlayerCommand += new PlayerHooks.PlayerCommandD(OnChat);
    }

    protected override void Dispose(bool disposing)
    {
        //IL_0012: Unknown result type (might be due to invalid IL or missing references)
        //IL_0028: Expected O, but got Unknown
        //IL_0030: Unknown result type (might be due to invalid IL or missing references)
        //IL_003a: Expected O, but got Unknown
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            PlayerHooks.PlayerCommand -= new PlayerHooks.PlayerCommandD(OnChat);
        }
        this.Dispose(disposing);
    }

    private void OnInitialize(EventArgs args)
    {
        RC();
    }

    private void OnChat(PlayerCommandEventArgs args)
    {
        if (((HandledEventArgs) (object) args).Handled)
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
            for (var j = 0; j < LConfig.命令表.Count; j++)
            {
                var O = LConfig.命令表[j].原始命令;
                if (SR(ref O, args.Player.Name, args.Parameters, LConfig.命令表[j].余段补充) && O == args.CommandText && LConfig.命令表[j].阻止原始)
                {
                    args.Player.SendErrorMessage("此指令已被禁用，故无法使用！");
                    ((HandledEventArgs) (object) args).Handled = true;
                    return;
                }
            }
        }
        int i;
        for (i = 0; i < LConfig.命令表.Count; i++)
        {
            if (LConfig.命令表[i].新的命令 == "" || !(LConfig.命令表[i].新的命令 == args.CommandName))
            {
                continue;
            }
            var O2 = LConfig.命令表[i].原始命令;
            if (!SR(ref O2, args.Player.Name, args.Parameters, LConfig.命令表[i].余段补充))
            {
                continue;
            }
            if (args.Player.Index >= 0)
            {
                if (LConfig.命令表[i].死亡条件 == 1 && (args.Player.Dead || args.Player.TPlayer.statLife < 1))
                {
                    args.Player.SendErrorMessage("此指令要求你必须活着才能使用！");
                    ((HandledEventArgs) (object) args).Handled = true;
                    break;
                }
                if (LConfig.命令表[i].死亡条件 == -1 && (!args.Player.Dead || args.Player.TPlayer.statLife > 0))
                {
                    args.Player.SendErrorMessage("此指令要求你必须死亡才能使用！");
                    ((HandledEventArgs) (object) args).Handled = true;
                    break;
                }
                var num = CJ(args.Player.Name, LConfig.命令表[i].新的命令, LConfig.命令表[i].冷却秒数, LConfig.命令表[i].冷却共用);
                if (num > 0)
                {
                    args.Player.SendErrorMessage("此指令正在冷却，还有{0}秒才能使用！", new object[1] { num });
                    ((HandledEventArgs) (object) args).Handled = true;
                    break;
                }
                jump = true;
                if (Commands.HandleCommand(args.Player, args.CommandPrefix + O2))
                {
                    lock (LCC)
                    {
                        if (!LCC.Exists((t) => t.Name == args.Player.Name && t.Cmd == LConfig.命令表[i].新的命令))
                        {
                            LCC.Add(new CC(args.Player.Name, LConfig.命令表[i].新的命令));
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
        lock (LCC)
        {
            for (var i = 0; i < LCC.Count; i++)
            {
                if (share)
                {
                    if (LCC[i].Cmd != Cmd)
                    {
                        continue;
                    }
                }
                else if (LCC[i].Cmd != Cmd || LCC[i].Name != name)
                {
                    continue;
                }
                var num = (int) (DateTime.UtcNow - LCC[i].LastTiem).TotalSeconds;
                num = C - num;
                if (num > 0)
                {
                    return num;
                }
                LCC.RemoveAt(i);
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
