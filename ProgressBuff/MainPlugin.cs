using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using ProgressQuery;

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

        private Scheme? scheme;

        private Dictionary<string, bool> GameProgress;

        private Dictionary<string, HashSet<int>> DetectionProgress = new Dictionary<string, HashSet<int>>();

        private Dictionary<string,string> ProgressNames = ProgressQuery.Utils.GetProgressNames();

        private HashSet<int> NotHaBuffs;

        private HashSet<int> HaBuffs;

        public string path = Path.Combine(new string[] { TShock.SavePath, "ProgressBuff.json" });
        public MainPlugin(Main game) : base(game)
        {
            Order = 6;
        }

        public override void Initialize()
        {
            LoadConfig();
            GetDataHandlers.PlayerUpdate.Register(OnUpdata);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPost);
            Commands.ChatCommands.Add(new Command("progress.buff.use", AddBuff, "增益", "zy"));
            GeneralHooks.ReloadEvent += (e) =>
            {
                LoadConfig();
                NotHaBuffs = GetNotHaBuffs();
                HaBuffs = GetHaBuffs();
            };
            ProgressQuery.MainPlugin.OnGameProgressEvent += OnProgress;
            DataSync.Plugin.OnDataSyncEvent += OnPost;
        }

        private void OnPost(EventArgs args)
        {
            GameProgress = ProgressQuery.Utils.GetGameProgress();
            NotHaBuffs = GetNotHaBuffs();
            HaBuffs = GetHaBuffs();
        }

        private void OnProgress(OnGameProgressEventArgs e)
        {
            GameProgress[e.Name] = e.code;
            NotHaBuffs = GetNotHaBuffs();
            HaBuffs = GetHaBuffs();
        }

        private void CacheData()
        {
            scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            if (scheme != null)
            { 
                 scheme.ProgressBuff.ForEach(x =>
                {
                    if (!scheme.SkipProgressDetection.Contains(x.Key))
                        DetectionProgress[x.Key] = x.Value;
                });

               
            }
               
        }

        private HashSet<int> GetNotHaBuffs()
        {
            List<int> buffs = new();
            foreach (var f in DetectionProgress)
            {
                if (!GameProgress[f.Key])
                    buffs.AddRange(f.Value);
            }
            return buffs.Distinct().ToHashSet<int>();
        }


        private HashSet<int> GetHaBuffs()
        {
            List<int> buffs = new();
            foreach (var f in DetectionProgress)
            {
                if (DataSync.Plugin.GetJb(ProgressNames[f.Key]))
                    buffs.AddRange(f.Value);

                if (GameProgress[f.Key])
                    buffs.AddRange(f.Value);
            }
            return buffs.Distinct().ToHashSet<int>();
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
            var buffs = HaBuffs.ToList() ;
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
            playerDet[e.Player.Index] = DateTime.Now.Second;
            for (int i = 0; i < e.Player.TPlayer.buffType.Length; i++)
            {
                if (NotHaBuffs.Contains(e.Player.TPlayer.buffType[i]))
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
                        e.Player.SendData(PacketTypes.PlayerBuff, "", i);
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
                    scheme.ProgressBuff.Add(pr.Key, new HashSet<int>());
                }
                config.Schemes.Add(scheme);
            }
            CacheData();
            config.Write(path);
        }
    }
}