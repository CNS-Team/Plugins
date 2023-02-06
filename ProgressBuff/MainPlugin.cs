using ProgressQuery;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ProgressBuff;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override string Author => "少司命";
    public override string Description => "进度buff";
    public override string Name => "ProgressBuff";
    public override Version Version => new Version(1, 0, 0, 0);

    public Config config = new Config();

    private readonly int[] playerDet = new int[Main.maxPlayers];

    private Scheme? scheme;

    private Dictionary<string, bool> GameProgress;

    private readonly Dictionary<string, HashSet<int>> DetectionProgress = new Dictionary<string, HashSet<int>>();

    private readonly Dictionary<string, string> ProgressNames = ProgressQuery.Utils.GetProgressNames();

    private HashSet<int> NotHaBuffs;

    private HashSet<int> HaBuffs;

    public string path = Path.Combine(new string[] { TShock.SavePath, "ProgressBuff.json" });
    public MainPlugin(Main game) : base(game)
    {
        this.Order = 6;
    }

    public override void Initialize()
    {
        this.LoadConfig();
        GetDataHandlers.PlayerUpdate.Register(this.OnUpdata);
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPost);
        Commands.ChatCommands.Add(new Command("progress.buff.use", this.AddBuff, "增益", "zy"));
        GeneralHooks.ReloadEvent += (e) =>
        {
            this.LoadConfig();
            this.NotHaBuffs = this.GetNotHaBuffs();
            this.HaBuffs = this.GetHaBuffs();
        };
        ProgressQuery.MainPlugin.OnGameProgressEvent += this.OnProgress;
        DataSync.Plugin.OnDataSyncEvent += this.OnPost;
    }

    private void OnPost(EventArgs args)
    {
        this.GameProgress = ProgressQuery.Utils.GetGameProgress();
        this.NotHaBuffs = this.GetNotHaBuffs();
        this.HaBuffs = this.GetHaBuffs();
    }

    private void OnProgress(OnGameProgressEventArgs e)
    {
        this.GameProgress[e.Name] = e.code;
        this.NotHaBuffs = this.GetNotHaBuffs();
        this.HaBuffs = this.GetHaBuffs();
    }

    private void CacheData()
    {
        this.scheme = this.config.Schemes.Find(f => f.SchemeName == this.config.UseScheme);
        if (this.scheme != null)
        {
            this.scheme.ProgressBuff.ForEach(x =>
            {
                if (!this.scheme.SkipProgressDetection.Contains(x.Key))
                    this.DetectionProgress[x.Key] = x.Value;
            });


        }

    }

    private HashSet<int> GetNotHaBuffs()
    {
        List<int> buffs = new();
        foreach (var f in this.DetectionProgress)
        {
            if (!this.GameProgress[f.Key])
                buffs.AddRange(f.Value);
        }
        return buffs.Distinct().ToHashSet<int>();
    }


    private HashSet<int> GetHaBuffs()
    {
        List<int> buffs = new();
        foreach (var f in this.DetectionProgress)
        {
            if (DataSync.Plugin.GetJb(this.ProgressNames[f.Key]))
                buffs.AddRange(f.Value);

            if (this.GameProgress[f.Key])
                buffs.AddRange(f.Value);
        }
        return buffs.Distinct().ToHashSet<int>();
    }

    private void AddBuff(CommandArgs args)
    {
        void ShowList(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
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
        var buffs = this.HaBuffs.ToList();
        if (args.Parameters.Count() == 2 && int.TryParse(args.Parameters[1], out var s) && int.TryParse(args.Parameters[0], out var index))
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
            for (var i = 0; i < buffs.Count; i++)
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
        if (e.Player.HasPermission("progress.buff.white") || e.Handled || !this.config.Enabled || !e.Player.IsLoggedIn || this.playerDet[e.Player.Index] == DateTime.Now.Second) return;
        this.playerDet[e.Player.Index] = DateTime.Now.Second;
        for (var i = 0; i < e.Player.TPlayer.buffType.Length; i++)
        {
            if (this.NotHaBuffs.Contains(e.Player.TPlayer.buffType[i]))
            {
                if (this.config.Broadcast)
                {
                    TShock.Utils.Broadcast($"玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Player.TPlayer.buffType[i])} ,已清除!", Microsoft.Xna.Framework.Color.Red);
                }

                if (this.config.WriteLog)
                {
                    TShock.Log.Write($"[ProgressBuff]: 玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Player.TPlayer.buffType[i])} ,已清除!", System.Diagnostics.TraceLevel.Info);
                }

                if (this.config.ClearBuff)
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
        if (File.Exists(this.path))
        {
            try
            {
                this.config = Config.Read(this.path);
            }
            catch (Exception e)
            {
                TShock.Log.ConsoleError("ProgressBuff.json配置读取错误:{0}", e.ToString());
            }
        }
        else
        {
            var scheme = new Scheme();
            foreach (var pr in ProgressQuery.Utils.GetGameProgress())
            {
                scheme.ProgressBuff.Add(pr.Key, new HashSet<int>());
            }
            this.config.Schemes.Add(scheme);
        }
        this.CacheData();
        this.config.Write(this.path);
    }
}