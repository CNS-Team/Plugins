using ProgressQuery;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AntiltemCheating;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override string Author => "少司命";
    public override string Description => "根据进度限制物品";
    public override string Name => "超进度物品限制";
    public override Version Version => new Version(1, 0, 0, 0);

    private Scheme? scheme;

    private readonly Dictionary<string, string> ProgressNames = ProgressQuery.Utils.GetProgressNames();

    private readonly Dictionary<string, HashSet<int>> DetectionProgress = new();

    private Dictionary<string, bool> GameProgress = new();

    public Config config = new Config();

    private HashSet<int> DetectionItem = new();

    private readonly int[] playerDet = new int[Main.maxPlayers];

    public string path = Path.Combine(new string[] { TShock.SavePath, "超进度物品检测.json" });
    public MainPlugin(Main game) : base(game)
    {
    }

    public void UpdateDetItems()
    {
        var Items = new List<int>();
        foreach (var f in this.DetectionProgress)
        {
            if (!this.GameProgress[f.Key])
            {
                if (!DataSync.Plugin.GetJb(this.ProgressNames[f.Key]))
                {
                    Items.AddRange(f.Value);
                }
            }
        }
        this.DetectionItem = Items.Distinct().ToHashSet();
    }

    public override void Initialize()
    {
        this.LoadConfig();
        GetDataHandlers.PlayerSlot.Register(this.OnSlot);
        GetDataHandlers.PlayerUpdate.Register(this.OnUpdata);
        GeneralHooks.ReloadEvent += (e) =>
        {
            this.LoadConfig();
            this.UpdateDetItems();
        };
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPost);
        ProgressQuery.MainPlugin.OnGameProgressEvent += this.OnGameProgress;
        DataSync.Plugin.OnDataSyncEvent += this.OnPost;
    }

    private void OnPost(EventArgs args)
    {
        this.GameProgress = ProgressQuery.Utils.GetGameProgress();
        this.UpdateDetItems();
    }

    private void OnGameProgress(OnGameProgressEventArgs e)
    {
        this.GameProgress[e.Name] = e.code;
        this.UpdateDetItems();
    }

    private void CacheData()
    {
        this.scheme = this.config.Schemes.Find(f => f.SchemeName == this.config.UseScheme);
        this.scheme?.AntiltemCheating.ForEach(x =>
            {
                if (!this.scheme.SkipProgressDetection.Exists(f => f == x.Key))
                {
                    this.DetectionProgress[x.Key] = x.Value.ToHashSet();
                }
            });
    }

    private void OnUpdata(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e.Player.HasPermission("progress.item.white") || e.Handled || !this.config.Enabled || !this.config.AccurateDetection || !e.Player.IsLoggedIn || this.playerDet[e.Player.Index] == DateTime.Now.Second)
        {
            return;
        }

        this.playerDet[e.Player.Index] = DateTime.Now.Second;
        for (var i = 0; i < NetItem.MaxInventory; i++)
        {
            Item? item;
            if (i < NetItem.InventoryIndex.Item2)
            {
                item = e.Player.TPlayer.inventory[i];
            }
            else if (i < NetItem.ArmorIndex.Item2)
            {
                var index = i - NetItem.ArmorIndex.Item1;
                item = e.Player.TPlayer.armor[index];
            }
            else if (i < NetItem.DyeIndex.Item2)
            {
                var index = i - NetItem.DyeIndex.Item1;
                item = e.Player.TPlayer.dye[index];
            }
            else if (i < NetItem.MiscEquipIndex.Item2)
            {
                var index = i - NetItem.MiscEquipIndex.Item1;
                item = e.Player.TPlayer.miscEquips[index];
            }
            else if (i < NetItem.MiscDyeIndex.Item2)
            {
                var index = i - NetItem.MiscDyeIndex.Item1;
                item = e.Player.TPlayer.miscDyes[index];
            }
            else
            {
                break;
            }

            if (this.DetectionItem.Contains(item.netID))
            {
                if (this.config.punishPlayer)
                {
                    e.Player.SetBuff(156, 60 * this.config.punishTime, false);
                }
                e.Player.SendErrorMessage($"检测到超进度物品{TShock.Utils.GetItemById(item.netID).Name}!");
                if (this.config.Broadcast)
                {
                    TShock.Utils.Broadcast($"检测到{e.Player.Name}拥有超进度物品{TShock.Utils.GetItemById(item.netID).Name}!", Microsoft.Xna.Framework.Color.DarkRed);
                }
                if (this.config.WriteLog)
                {
                    TShock.Log.Write($"[超进度物品限制] 玩家{e.Player.Name} 在背包第{i}格检测到超进度物品 {TShock.Utils.GetItemById(item.netID).Name} x{item.stack}", System.Diagnostics.TraceLevel.Info);
                }
                if (this.config.ClearItem)
                {
                    item.stack = 0;
                    TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", e.Player.Index, i);
                }

                if (this.config.KickPlayer)
                {
                    e.Player.Kick("拥有超进度物品");
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
                TShock.Log.ConsoleError("超进度物品检测.json配置读取错误:{0}", e.ToString());
            }
        }
        else
        {
            var scheme = new Scheme();
            foreach (var pr in ProgressQuery.Utils.GetGameProgress())
            {
                scheme.AntiltemCheating.Add(pr.Key, new List<int>());
            }
            this.config.Schemes.Add(scheme);
        }
        this.CacheData();
        this.config.Write(this.path);
    }

    private void OnSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
    {
        if (e.Player.HasPermission("progress.item.white") || e.Handled || !this.config.Enabled || !e.Player.IsLoggedIn)
        {
            return;
        }

        if (this.DetectionItem.Contains(e.Type))
        {
            if (this.config.punishPlayer)
            {
                e.Player.SetBuff(156, 60 * this.config.punishTime, false);
            }
            e.Player.SendErrorMessage($"检测到超进度物品{TShock.Utils.GetItemById(e.Type).Name}!");
            if (this.config.Broadcast)
            {
                TShock.Utils.Broadcast($"检测到{e.Player.Name}拥有超进度物品{TShock.Utils.GetItemById(e.Type).Name}!", Microsoft.Xna.Framework.Color.DarkRed);
            }
            if (this.config.WriteLog)
            {
                TShock.Log.Write($"[超进度物品限制] 玩家{e.Player.Name} 在背包第{e.Slot}格检测到超进度物品 {TShock.Utils.GetItemById(e.Type).Name} x{e.Stack}", System.Diagnostics.TraceLevel.Info);
            }
            if (this.config.ClearItem)
            {
                e.Stack = 0;
                TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", e.Player.Index, e.Slot);
            }
            if (this.config.KickPlayer)
            {
                e.Player.Kick("拥有超进度物品");
            }
        }
    }
}