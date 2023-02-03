using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace ProgressBuff
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        public override string Author => "少司命";
        public override string Description => "进度buff";
        public override string Name => "ProgressBuff";
        public override Version Version => new Version(1, 0, 0, 0);

        public Config config = new Config();

        private int[] playerDet = new int[Main.maxPlayers];

        public string path = Path.Combine(new string[] { TShock.SavePath, "ProgressBuff.json" });
        public MainPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            LoadConfig();
            GetDataHandlers.PlayerUpdate.Register(OnUpdata);
            Commands.ChatCommands.Add(new Command("progress.buff.use", AddBuff, "增益", "zy"));
            GeneralHooks.ReloadEvent += (e) => LoadConfig();
        }

        private List<int> GetNotHaBuffs()
        {
            List<int> buffs = new();
            var scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            var ProgressNames = ProgressQuery.Utils.GetProgressNames();
            if (scheme == null) return buffs;
            var GameProcess = ProgressQuery.Utils.GetGameProgress();
            var going = ProgressQuery.Utils.Ongoing();
            foreach (var f in scheme.ProgressBuff)
            {

                if (!scheme.SkipProgressDetection.Exists(x => x == f.Key))
                { 
                    if (going.ContainsKey(f.Key))
                        if (!going[f.Key])
                            buffs.AddRange(f.Value);

                    if (!GameProcess[f.Key])
                        buffs.AddRange(f.Value);
                }
            }
            return buffs.Distinct().ToList();
        }


        private List<int> GetHaBuffs()
        {
            List<int> buffs = new();
            var scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            var ProgressNames = ProgressQuery.Utils.GetProgressNames();
            if (scheme == null) return buffs;
            var GameProcess = ProgressQuery.Utils.GetGameProgress();
            var going = ProgressQuery.Utils.Ongoing();
            foreach (var f in scheme.ProgressBuff)
            {
                if (!scheme.SkipRemoteDetection.Exists(x => x == f.Key))
                {
                    if (DataSync.Plugin.GetJb(ProgressNames[f.Key]))
                        buffs.AddRange(f.Value);
                }
                if (scheme.SkipProgressDetection.Exists(x => x == f.Key))
                    buffs.AddRange(f.Value);

                if (going.ContainsKey(f.Key))
                    if (going[f.Key])
                        buffs.AddRange(f.Value);

                if (GameProcess[f.Key])
                    buffs.AddRange(f.Value);
            }

            return buffs.Distinct().ToList();
        }

        private void AddBuff(CommandArgs args)
        {
            void ShowList(List<string> line)
            {
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
                    return;

                PaginationTools.SendPage(args.Player, pageNumber, line,
                new PaginationTools.Settings
                {
                    MaxLinesPerPage = 10,
                    NothingToDisplayString = "没有可用的增益",
                    HeaderFormat = "增益列表 ({0}/{1})：",
                    FooterFormat = "输入 {0}增益 list {{0}} 查看更多".SFormat(Commands.Specifier)
                });
            }
            var buffs = GetHaBuffs();
            if (args.Parameters.Count() == 2 && int.TryParse(args.Parameters[1], out int s) && int.TryParse(args.Parameters[0], out int index))
            {

                if (!buffs.IndexInRange(index - 1))
                {
                    args.Player.SendErrorMessage("输入错误的序号!");
                    return;
                }
                if (s > 0 && s < 30001 && Terraria.ID.BuffID.Search.ContainsId(buffs[index - 1]))
                {
                    args.Player.SetBuff(buffs[index - 1], s);
                    args.Player.SendSuccessMessage("增幅成功!");
                }
                else
                {
                    args.Player.SendErrorMessage("你输入的参数有误!");
                }
            }
            else if (args.Parameters.Count() >= 1 && args.Parameters[0].ToLower() == "list")
            {
                List<string> lines = new();
                for (int i = 0; i < buffs.Count; i++)
                {

                    lines.Add($"{i + 1}.{TShock.Utils.GetBuffName(buffs[i])}".Color(TShockAPI.Utils.PinkHighlight));
                }
                ShowList(lines);
            }
            else
            {
                args.Player.SendInfoMessage("/增益 <序号> <秒数>");
                args.Player.SendInfoMessage("/增益 list");
            }
        }


        private void OnUpdata(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
        {
            if (e.Player.HasPermission("progress.buff.white") || e.Handled || !config.Enabled || !e.Player.IsLoggedIn || playerDet[e.Player.Index] == DateTime.Now.Second) return;
            var scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            var ProgressNames = ProgressQuery.Utils.GetProgressNames();
            playerDet[e.Player.Index] = DateTime.Now.Second;
            if (scheme == null) return;
            var bossProcess = ProgressQuery.Utils.GetGameProgress();
            var going = ProgressQuery.Utils.Ongoing();
            var buffs = GetNotHaBuffs();
            for (int i = 0; i < e.Player.TPlayer.buffType.Length; i++)
            {
                if (buffs.Exists(f => f == e.Player.TPlayer.buffType[i]))
                {
                    if (config.Broadcast)
                    {
                        TShock.Utils.Broadcast($"玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Player.TPlayer.buffType[i])} ,已清除!", Microsoft.Xna.Framework.Color.Red);
                    }

                    if (config.WriteLog)
                    {
                        TShock.Log.Write($"[ProgressBuff]: 玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Player.TPlayer.buffType[i])} ,已清除!", System.Diagnostics.TraceLevel.Info);
                    }

                    if (config.ClearBuff)
                    { 
                        e.Player.TPlayer.buffType[i] = 0;
                        e.Player.TPlayer.buffTime[i] = 0;
                        e.Player.SendData(PacketTypes.PlayerBuff, "", e.Player.Index, i, 0, 0);
                    }
                }
            }
        }

        private void LoadConfig()
        {
            if (File.Exists(path))
            {
                try
                {
                    config = Config.Read(path);
                }
                catch (Exception e)
                {
                    TShock.Log.ConsoleError("ProgressBuff.json配置读取错误:{0}", e.ToString());
                }
            }
            else
            {
                Scheme scheme = new Scheme();
                foreach (var pr in ProgressQuery.Utils.GetGameProgress())
                {
                    scheme.ProgressBuff.Add(pr.Key, new List<int>());
                }
                config.Schemes.Add(scheme);
            }
            config.Write(path);
        }
    }
}