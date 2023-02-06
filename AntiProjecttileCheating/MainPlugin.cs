using ProgressQuery;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AntiProjecttileCheating;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override string Author => "少司命";
    public override string Description => "根据进度限制弹幕";
    public override string Name => "超进度弹幕限制";
    public override Version Version => new(1, 0, 0, 0);

    private Scheme? scheme;

    private readonly Dictionary<string, string> ProgressNames = ProgressQuery.Utils.GetProgressNames();

    private readonly Dictionary<string, HashSet<int>> DetectionProgress = new();

    private Dictionary<string, bool> GameProgress = new();

    public Config config = new Config();

    private HashSet<int> DetectionProj = new();

    public string path = Path.Combine(new string[] { TShock.SavePath, "超进度弹幕检测.json" });
    public MainPlugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.LoadConfig();
        GetDataHandlers.NewProjectile.Register(this.OnProj);
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPost);
        GeneralHooks.ReloadEvent += (e) =>
        {
            this.LoadConfig();
            this.UpdateDetProj();
        };
        ProgressQuery.MainPlugin.OnGameProgressEvent += this.OnProgress;
        DataSync.Plugin.OnDataSyncEvent += this.OnPost;
    }

    private void UpdateDetProj()
    {
        var projs = new List<int>();
        foreach (var f in this.DetectionProgress)
        {
            if (!this.GameProgress[f.Key])
            {
                if (!DataSync.Plugin.GetJb(this.ProgressNames[f.Key]))
                    projs.AddRange(f.Value);
            }
        }
        this.DetectionProj = projs.Distinct().ToHashSet();
    }

    private void OnProgress(OnGameProgressEventArgs e)
    {
        this.GameProgress[e.Name] = e.code;
        this.UpdateDetProj();
    }

    private void OnPost(EventArgs args)
    {
        this.GameProgress = ProgressQuery.Utils.GetGameProgress();
        this.UpdateDetProj();
    }

    private void CacheData()
    {
        this.scheme = this.config.Schemes.Find(f => f.SchemeName == this.config.UseScheme);
        if (this.scheme != null)
            this.scheme.AntiProjecttileCheating.ForEach(x =>
            {
                if (!this.scheme.SkipProgressDetection.Contains(x.Key))
                    this.DetectionProgress[x.Key] = x.Value;
            });
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
                TShock.Log.ConsoleError("超进度弹幕检测.json配置读取错误:{0}", e.ToString());
            }
        }
        else
        {
            var scheme = new Scheme();
            foreach (var pr in ProgressQuery.Utils.GetGameProgress())
            {
                scheme.AntiProjecttileCheating.Add(pr.Key, new HashSet<int>());
            }
            this.config.Schemes.Add(scheme);
        }

        this.CacheData();
        this.config.Write(this.path);
    }

    private void OnProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (e.Player.HasPermission("progress.projecttile.white") || e.Handled || !this.config.Enabled || !e.Player.IsLoggedIn) return;
        if (this.DetectionProj.Contains(e.Type))
        {
            if (this.config.punishPlayer)
            {
                e.Player.SetBuff(156, 60 * this.config.punishTime, false);
            }
            e.Player.SendErrorMessage($"检测到超进度弹幕{Lang.GetProjectileName(e.Type).Value}!");
            if (this.config.Broadcast)
            {
                TShock.Utils.Broadcast($"检测到{e.Player.Name}使用超进度弹幕{Lang.GetProjectileName(e.Type).Value}!", Microsoft.Xna.Framework.Color.DarkRed);
            }
            if (this.config.WriteLog)
            {
                TShock.Log.Write($"[超进度弹幕限制] 玩家{e.Player.Name} 使用超进度弹幕 {Lang.GetProjectileName(e.Type).Value} ID =>{e.Type}", System.Diagnostics.TraceLevel.Info);
            }
            if (this.config.ClearItem)
            {
                Main.projectile[e.Index].active = false;
                Main.projectile[e.Index].type = 0;
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", e.Index);
            }

            if (this.config.KickPlayer)
            {
                e.Player.Kick("使用超进度弹幕");
            }
        }
    }
}