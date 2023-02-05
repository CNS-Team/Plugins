using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using ProgressQuery;

namespace AntiProjecttileCheating
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        public override string Author => "少司命";
        public override string Description => "根据进度限制弹幕";
        public override string Name => "超进度弹幕限制";
        public override Version Version => new(1, 0, 0, 0);

        private Scheme? scheme;

        private Dictionary<string, string> ProgressNames = ProgressQuery.Utils.GetProgressNames();

        private Dictionary<string,HashSet<int>> DetectionProgress = new();

        private Dictionary<string, bool> GameProgress = new();

        public Config config = new Config();

        private HashSet<int> DetectionProj = new();

        public string path = Path.Combine(new string[] { TShock.SavePath, "超进度弹幕检测.json" });
        public MainPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            LoadConfig();
            GetDataHandlers.NewProjectile.Register(OnProj);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPost);
            GeneralHooks.ReloadEvent += (e) =>
            {
                LoadConfig();
                UpdateDetProj();
            };
            ProgressQuery.MainPlugin.OnGameProgressEvent += OnProgress;
            DataSync.Plugin.OnDataSyncEvent += OnPost;
        }

        private void UpdateDetProj()
        {
            var projs = new List<int>();
            foreach (var f in DetectionProgress)
            {
                if (!GameProgress[f.Key])
                {
                    if (!DataSync.Plugin.GetJb(ProgressNames[f.Key]))
                        projs.AddRange(f.Value);
                }
            }
            DetectionProj = projs.Distinct().ToHashSet();
        }

        private void OnProgress(OnGameProgressEventArgs e)
        {
            GameProgress[e.Name] = e.code;
            UpdateDetProj();
        }

        private void OnPost(EventArgs args)
        {
            GameProgress = ProgressQuery.Utils.GetGameProgress();
            UpdateDetProj();
        }

        private void CacheData()
        {
            scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            if (scheme != null)
                scheme.AntiProjecttileCheating.ForEach(x =>
                {
                    if (!scheme.SkipProgressDetection.Contains(x.Key))
                        DetectionProgress[x.Key] =  x.Value;
                });
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
                    TShock.Log.ConsoleError("超进度弹幕检测.json配置读取错误:{0}", e.ToString());
                }
            }
            else
            {
                Scheme scheme = new Scheme();
                foreach (var pr in ProgressQuery.Utils.GetGameProgress())
                {
                    scheme.AntiProjecttileCheating.Add(pr.Key, new HashSet<int>());
                }
                config.Schemes.Add(scheme);
            }

            CacheData();
            config.Write(path);
        }

        private void OnProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
        {
            if (e.Player.HasPermission("progress.projecttile.white") || e.Handled || !config.Enabled || !e.Player.IsLoggedIn) return;
            if (DetectionProj.Contains(e.Type))
            {
                if (config.punishPlayer)
                {
                    e.Player.SetBuff(156, 60 * config.punishTime, false);
                }
                e.Player.SendErrorMessage($"检测到超进度弹幕{Lang.GetProjectileName(e.Type).Value}!");
                if (config.Broadcast)
                {
                    TShock.Utils.Broadcast($"检测到{e.Player.Name}使用超进度弹幕{Lang.GetProjectileName(e.Type).Value}!", Microsoft.Xna.Framework.Color.DarkRed);
                }
                if (config.WriteLog)
                {
                    TShock.Log.Write($"[超进度弹幕限制] 玩家{e.Player.Name} 使用超进度弹幕 {Lang.GetProjectileName(e.Type).Value} ID =>{e.Type}", System.Diagnostics.TraceLevel.Info);
                }
                if (config.ClearItem)
                {
                    Main.projectile[e.Index].active = false;
                    Main.projectile[e.Index].type = 0;
                    TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", e.Index);
                }
                        
                if (config.KickPlayer)
                {
                    e.Player.Kick("使用超进度弹幕");
                }
            }
        }
    }
}