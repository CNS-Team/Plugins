using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using ProgressQuery;

namespace AntiltemCheating
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        public override string Author => "少司命";
        public override string Description => "根据进度限制物品";
        public override string Name => "超进度物品限制";
        public override Version Version => new Version(1, 0, 0, 0);

        private Scheme? scheme;

        private Dictionary<string, string> ProgressNames = ProgressQuery.Utils.GetProgressNames();

        private Dictionary<string, HashSet<int>> DetectionProgress = new();

        private Dictionary<string, bool> GameProgress = new();

        public Config config = new Config();

        private HashSet<int> DetectionItem = new();

        private int[] playerDet = new int[Main.maxPlayers];

        public string path = Path.Combine(new string[] { TShock.SavePath, "超进度物品检测.json" });
        public MainPlugin(Main game) : base(game)
        {
        }

        public void UpdateDetItems()
        {
            var Items = new List<int>();
            foreach (var f in DetectionProgress)
            {
                if (!GameProgress[f.Key])
                {
                    if (!DataSync.Plugin.GetJb(ProgressNames[f.Key]))
                        Items.AddRange(f.Value);
                }
            }
            DetectionItem = Items.Distinct().ToHashSet();
        }

        public override void Initialize()
        {
            LoadConfig();
            GetDataHandlers.PlayerSlot.Register(OnSlot);
            GetDataHandlers.PlayerUpdate.Register(OnUpdata);
            GeneralHooks.ReloadEvent += (e) =>
            {
                LoadConfig();
                UpdateDetItems();
            };
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPost);
            ProgressQuery.MainPlugin.OnGameProgressEvent += OnGameProgress;
            DataSync.Plugin.OnDataSyncEvent += OnPost;
        }

        private void OnPost(EventArgs args)
        {
            GameProgress = ProgressQuery.Utils.GetGameProgress();
            UpdateDetItems();
        }

        private void OnGameProgress(OnGameProgressEventArgs e)
        {
            GameProgress[e.Name] = e.code;
            UpdateDetItems();
        }

        private void CacheData()
        {
            scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            if (scheme != null)
                scheme.AntiltemCheating.ForEach(x =>
                {
                    if (!scheme.SkipProgressDetection.Exists(f => f == x.Key))
                        DetectionProgress[x.Key] =  x.Value.ToHashSet();
                });
        }

        private void OnUpdata(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
        {
            if (e.Player.HasPermission("progress.item.white") || e.Handled || !config.Enabled || !config.AccurateDetection || !e.Player.IsLoggedIn || playerDet[e.Player.Index] == DateTime.Now.Second) return;
            playerDet[e.Player.Index] = DateTime.Now.Second;
            for (int i = 0; i < NetItem.MaxInventory; i++)
            {
                Item? item;
                if (i < NetItem.InventoryIndex.Item2)
                {
                    item = e.Player.TPlayer.inventory[i];
                }
                else if (i < NetItem.ArmorIndex.Item2)
                {
                    int index = i - NetItem.ArmorIndex.Item1;
                    item = e.Player.TPlayer.armor[index];
                }
                else if (i < NetItem.DyeIndex.Item2)
                {
                    int index = i - NetItem.DyeIndex.Item1;
                    item = e.Player.TPlayer.dye[index];
                }
                else if (i < NetItem.MiscEquipIndex.Item2)
                {
                    int index = i - NetItem.MiscEquipIndex.Item1;
                    item = e.Player.TPlayer.miscEquips[index];
                }
                else if (i < NetItem.MiscDyeIndex.Item2)
                {
                    int index = i - NetItem.MiscDyeIndex.Item1;
                    item = e.Player.TPlayer.miscDyes[index];
                }
                else
                {
                    break;
                }

                if (DetectionItem.Contains(item.netID))
                {
                    if (config.punishPlayer)
                    {
                        e.Player.SetBuff(156, 60 * config.punishTime, false);
                    }
                    e.Player.SendErrorMessage($"检测到超进度物品{TShock.Utils.GetItemById(item.netID).Name}!");
                    if (config.Broadcast)
                    {
                        TShock.Utils.Broadcast($"检测到{e.Player.Name}拥有超进度物品{TShock.Utils.GetItemById(item.netID).Name}!", Microsoft.Xna.Framework.Color.DarkRed);
                    }
                    if (config.WriteLog)
                    {
                        TShock.Log.Write($"[超进度物品限制] 玩家{e.Player.Name} 在背包第{i}格检测到超进度物品 {TShock.Utils.GetItemById(item.netID).Name} x{item.stack}", System.Diagnostics.TraceLevel.Info);
                    }
                    if (config.ClearItem)
                    {
                        item.stack = 0;
                        TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", e.Player.Index, i);
                    }
                            
                    if (config.KickPlayer)
                    {
                        e.Player.Kick("拥有超进度物品");
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
                    TShock.Log.ConsoleError("超进度物品检测.json配置读取错误:{0}", e.ToString());
                }
            }
            else
            {
                Scheme scheme = new Scheme();
                foreach (var pr in ProgressQuery.Utils.GetGameProgress())
                {
                    scheme.AntiltemCheating.Add(pr.Key, new List<int>());
                }
                config.Schemes.Add(scheme);
            }
            CacheData();
            config.Write(path);
        }

        private void OnSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
        {
            if (e.Player.HasPermission("progress.item.white") || e.Handled || !config.Enabled || !e.Player.IsLoggedIn) return;

            if (DetectionItem.Contains(e.Type))
            {
                if (config.punishPlayer)
                {
                    e.Player.SetBuff(156, 60 * config.punishTime, false);
                }
                e.Player.SendErrorMessage($"检测到超进度物品{TShock.Utils.GetItemById(e.Type).Name}!");
                if (config.Broadcast)
                {
                    TShock.Utils.Broadcast($"检测到{e.Player.Name}拥有超进度物品{TShock.Utils.GetItemById(e.Type).Name}!", Microsoft.Xna.Framework.Color.DarkRed);
                }
                if (config.WriteLog)
                {
                    TShock.Log.Write($"[超进度物品限制] 玩家{e.Player.Name} 在背包第{e.Slot}格检测到超进度物品 {TShock.Utils.GetItemById(e.Type).Name} x{e.Stack}", System.Diagnostics.TraceLevel.Info);
                }
                if (config.ClearItem)
                {
                    e.Stack = 0;
                    TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", e.Player.Index, e.Slot);
                }
                if (config.KickPlayer)
                {
                    e.Player.Kick("拥有超进度物品");
                }
            }
        }
    }
}