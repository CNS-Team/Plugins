using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace AntiltemCheating
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        public override string Author => "少司命";
        public override string Description => "根据进度限制物品";
        public override string Name => "超进度物品限制";
        public override Version Version => new Version(1,0,0,0);
        public Config config = new Config();
        public string path = Path.Combine(new string[] { TShock.SavePath, "超进度物品检测.json" });
        public MainPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            LoadConfig();
            GetDataHandlers.PlayerSlot.Register(OnSlot);
            GetDataHandlers.PlayerUpdate.Register(OnUpdata);
            GeneralHooks.ReloadEvent += (e) => LoadConfig();
        }

        private void OnUpdata(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
        {
            if (e.Player.HasPermission("progress.item.white") || e.Handled || !config.Enabled || !config.AccurateDetection || !e.Player.IsLoggedIn) return;
            var scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            var ProgressNames = ProgressQuery.Utils.GetProgressNames();
            if (scheme == null) return;
            var bossProcess = ProgressQuery.Utils.GetGameProgress();
            var going = ProgressQuery.Utils.Ongoing();
            var item = new Item();
            for (int i = 0; i < NetItem.MaxInventory; i++)
            {
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
                foreach (var f in scheme.AntiltemCheating)
                {
                    if (!bossProcess[f.Key])
                    {
                        if (!scheme.SkipRemoteDetection.Exists(x => x == f.Key))
                        {
                            if (DataSync.Plugin.GetJb(ProgressNames[f.Key]))
                                return;
                        }
                        if (scheme.SkipProgressDetection.Exists(x => x == f.Key))
                            return;
                        
                        if (going.ContainsKey(f.Key))
                        {
                            if (going[f.Key]) return;
                        }
                        if (f.Value.FindAll(s => s == item.netID).Count > 0)
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
            config.Write(path);
        }

        private void OnSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
        {
            if (e.Player.HasPermission("progress.item.white") || e.Handled || !config.Enabled || !e.Player.IsLoggedIn) return;
            var scheme = config.Schemes.Find(f => f.SchemeName == config.UseScheme);
            var ProgressNames = ProgressQuery.Utils.GetProgressNames();
            if (scheme == null) return;
            foreach (var f in scheme.AntiltemCheating)
            {
                if (!ProgressQuery.Utils.GetGameProgress()[f.Key])
                {
                    if (!scheme.SkipRemoteDetection.Exists(x => x == f.Key))
                    {
                        if (DataSync.Plugin.GetJb(ProgressNames[f.Key]))
                            return;
                    }
                    if (scheme.SkipProgressDetection.Exists(x => x == f.Key))
                        return;
                    var going = ProgressQuery.Utils.Ongoing();
                    if (going.ContainsKey(f.Key))
                    {
                        if (going[f.Key]) return;
                    }
                    if (f.Value.FindAll(s => s == e.Slot).Count > 0)
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
    }
}