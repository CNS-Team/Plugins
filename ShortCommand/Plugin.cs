using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1.Utilities;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ShortCommand
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public class CommandCD
        {
            public string Name { get; set; }

            public string Cmd { get; set; }

            public DateTime LastTime { get; set; }

            public CommandCD(string name, string cmd)
            {
                Name = name;
                Cmd = cmd;
                LastTime = DateTime.UtcNow;
            }

        }
        public static bool jump { get; set; }
        internal string PATH = Path.Combine(TShock.SavePath, "简短指令.json");

        private readonly Dictionary<string, CMD> ShortCmd = new Dictionary<string, CMD>();

        private readonly HashSet<string> NotSourceCmd = new HashSet<string>();

        public override string Name => "简短指令改良版";

        public override string Author => "GK 超级改良 & Update By Cai";

        public override Version Version => new Version(1, 0, 1, 7);

        public override string Description => "由GK改良的简短指令插件！";

        private Config Config { get; set; }

        private List<CommandCD> CmdCD { get; set; }

        public Plugin(Main game)
            : base(game)
        {
            CmdCD = new List<CommandCD>();
            Config = new Config();
            base.Order += 1000000;
        }

        private void RC()
        {
            try
            {
                if (!File.Exists(PATH))
                {
                    Config.Commands.Add(new CMD());
                    TShock.Log.ConsoleError("未找到简短指令配置，已为您创建！");
                }
                else
                {
                    Config = ShortCommand.Config.Read(PATH);
                }
                Config.Write(PATH);
                ShortCmd.Clear();
                NotSourceCmd.Clear();
                Config.Commands.ForEach(delegate (CMD x)
                {
                    if (x != null)
                    {
                        ShortCmd[x.NewCommand] = x;
                        if (x.NotSource && !string.IsNullOrEmpty(x.SourceCommand))
                        {
                            string[] array = x.SourceCommand.Split(' ');
                            if (array.Length != 0)
                            {
                                NotSourceCmd.Add(array[0]);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("简短指令配置读取错误:" + ex.ToString());
            }
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            PlayerHooks.PlayerCommand += OnChat;
            GeneralHooks.ReloadEvent += delegate (ReloadEventArgs e)
            {
                RC();
                e.Player.SendSuccessMessage("简短指令重读成功!");
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                PlayerHooks.PlayerCommand -= OnChat;
            }
            Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            RC();
        }

        private void OnChat(PlayerCommandEventArgs args)
        {
            PlayerCommandEventArgs args2 = args;
            if (args2.Handled)
            {
                return;
            }
            if (jump)
            {
                jump = false;
                return;
            }
            if (NotSourceCmd.Contains(args2.CommandName))
            {
                args2.Player.SendErrorMessage("该指令已被禁止使用!");
                args2.Handled = true;
            }
            else
            {
                if (!ShortCmd.TryGetValue(args2.CommandName, out var cmd) || cmd == null)
                {
                    return;
                }
                string cmd2 = cmd.SourceCommand;
                if (!SR(ref cmd2, args2.Player.Name, args2.Parameters, cmd.Supplement))
                {
                    return;
                }
                if (args2.Player.Index >= 0)
                {
                    if (cmd.Condition == ConditionType.Alive && (args2.Player.Dead || args2.Player.TPlayer.statLife < 1))
                    {
                        args2.Player.SendErrorMessage("此指令要求你必须活着才能使用！");
                        args2.Handled = true;
                        return;
                    }
                    if (cmd.Condition == ConditionType.Death && (!args2.Player.Dead || args2.Player.TPlayer.statLife > 0))
                    {
                        args2.Player.SendErrorMessage("此指令要求你必须死亡才能使用！");
                        args2.Handled = true;
                        return;
                    }
                    int cD = GetCD(args2.Player.Name, cmd.SourceCommand, cmd.CD, cmd.ShareCD);
                    if (cD > 0)
                    {
                        args2.Player.SendErrorMessage("此指令正在冷却，还有{0}秒才能使用！", cD);
                        args2.Handled = true;
                        return;
                    }
                    jump = true;
                    if (Commands.HandleCommand(args2.Player, args2.CommandPrefix + cmd2) && !CmdCD.Exists((CommandCD t) => t.Name == args2.Player.Name && t.Cmd == cmd.NewCommand))
                    {
                        CmdCD.Add(new CommandCD(args2.Player.Name, cmd.NewCommand));
                    }
                }
                else
                {
                    jump = true;
                    Commands.HandleCommand(args2.Player, args2.CommandPrefix + cmd2);
                }
                args2.Handled = true;
            }
        }

        private int GetCD(string plyName, string Cmd, int CD, bool share)
        {
            for (int i = 0; i < CmdCD.Count; i++)
            {
                if (share)
                {
                    if (!(CmdCD[i].Cmd != Cmd))
                    {
                    }
                }
                else if (!(CmdCD[i].Cmd != Cmd) && !(CmdCD[i].Name != plyName))
                {
                    int num = (int) (DateTime.UtcNow - CmdCD[i].LastTime).TotalSeconds;
                    num = CD - num;
                    if (num > 0)
                    {
                        return num;
                    }
                    CmdCD.RemoveAt(i);
                    return 0;
                }
            }
            return 0;
        }

        private bool SR(ref string cmd, string plyName, List<string> cmdArgs, bool Supplement)
        {
            string text = "";
            for (int i = 0; i < cmdArgs.Count; i++)
            {
                string text2 = "{" + i + "}";
                if (cmd.Contains(text2))
                {
                    cmd = cmd.Replace(text2, cmdArgs[i]);
                    continue;
                }
                if (Supplement)
                {
                    text = text + " " + cmdArgs[i];
                    continue;
                }
                return false;
            }
            string text3 = "{player}";
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
}
