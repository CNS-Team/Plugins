using System.Runtime.CompilerServices;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace AntiItemCheating
{
    // Token: 0x02000006 RID: 6
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000010 RID: 16 RVA: 0x000057FD File Offset: 0x000039FD
        public override string Author
        {
            get
            {
                return "原作者: TOFOUT, 修改者: Dr.Toxic";
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000011 RID: 17 RVA: 0x00005804 File Offset: 0x00003A04
        public override string Description
        {
            get
            {
                return "禁止物品超出当前进度";
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000012 RID: 18 RVA: 0x0000580B File Offset: 0x00003A0B
        public override string Name
        {
            get
            {
                return "超进度物品检测改";
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000013 RID: 19 RVA: 0x00005812 File Offset: 0x00003A12
        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 8);
            }
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000014 RID: 20 RVA: 0x0000581D File Offset: 0x00003A1D
        // (set) Token: 0x06000015 RID: 21 RVA: 0x00005824 File Offset: 0x00003A24
        public static ConfigFile IOPDConfig { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000016 RID: 22 RVA: 0x0000582C File Offset: 0x00003A2C
        internal static string IOPDConfigPath
        {
            get
            {
                return Path.Combine(TShock.SavePath, "超进度物品限制.json");
            }
        }

        // Token: 0x06000017 RID: 23 RVA: 0x0000583D File Offset: 0x00003A3D
        public MainPlugin(Main game) : base(game)
        {
            MainPlugin.IOPDConfig = new ConfigFile();
        }

        // Token: 0x06000018 RID: 24 RVA: 0x00005860 File Offset: 0x00003A60
        public override void Initialize()
        {
            this.updateHandler = new HookHandler<EventArgs>(this.OnUpdate);
            ServerApi.Hooks.GamePostInitialize.Register(this, new HookHandler<EventArgs>(this.OnPostInitialize));
            ServerApi.Hooks.GameUpdate.Register(this, this.updateHandler);
            ServerApi.Hooks.NpcKilled.Register(this, new HookHandler<NpcKilledEventArgs>(this.NpcKilled));
            GetDataHandlers.PlayerSlot += new EventHandler<GetDataHandlers.PlayerSlotEventArgs>(this.OnItemSlot);
            this.ReadConfig();
            Commands.ChatCommands.Add(new Command("重读超进度物品权限", new CommandDelegate(this.CMD), new string[]
            {
                "重读超进度物品限制"
            })
            {
                HelpText = "输入 /重读超进度物品限制 重读超进度物品限制配置"
            });
        }

        // Token: 0x06000019 RID: 25 RVA: 0x00005930 File Offset: 0x00003B30
        protected override void Dispose(bool disposing)
        {
            ServerApi.Hooks.GamePostInitialize.Deregister(this, new HookHandler<EventArgs>(this.OnPostInitialize));
            ServerApi.Hooks.GameUpdate.Deregister(this, this.updateHandler);
            ServerApi.Hooks.NpcKilled.Deregister(this, new HookHandler<NpcKilledEventArgs>(this.NpcKilled));
            GetDataHandlers.PlayerSlot -= new EventHandler<GetDataHandlers.PlayerSlotEventArgs>(this.OnItemSlot);
            base.Dispose(disposing);
        }

        // Token: 0x0600001A RID: 26 RVA: 0x000059B2 File Offset: 0x00003BB2
        private void CMD(CommandArgs args)
        {
            this.ReadConfig();
            args.Player.SendSuccessMessage("重读超进度物品限制");
        }

        // Token: 0x0600001B RID: 27 RVA: 0x000059D0 File Offset: 0x00003BD0
        private void ReadConfig()
        {
            try
            {
                bool flag = !File.Exists(MainPlugin.IOPDConfigPath);
                if (flag)
                {
                    TShock.Log.ConsoleError("未找到超进度物品限制配置，已为您创建！");
                }
                MainPlugin.IOPDConfig = ConfigFile.Read(MainPlugin.IOPDConfigPath);
                MainPlugin.IOPDConfig.Write(MainPlugin.IOPDConfigPath);
                this.checkers = new List<IItemChecker>
                {
                    MainPlugin.CheckerLoader.LoadBlackList(),
                    MainPlugin.CheckerLoader.LoadBeforeGoblin1(),
                    MainPlugin.CheckerLoader.LoadBeforeKingSlime(),
                    MainPlugin.CheckerLoader.LoadBeforeDeerClops(),
                    MainPlugin.CheckerLoader.LoadBeforeEye(),
                    MainPlugin.CheckerLoader.LoadBeforeEvil(),
                    MainPlugin.CheckerLoader.LoadBeforeOld1(),
                    MainPlugin.CheckerLoader.LoadBeforeQueenBee(),
                    MainPlugin.CheckerLoader.LoadBeforeSkeletron(),
                    MainPlugin.CheckerLoader.LoadBeforeWallOfFlesh(),
                    MainPlugin.CheckerLoader.LoadBeforeGoblin2(),
                    MainPlugin.CheckerLoader.LoadBeforePirates(),
                    MainPlugin.CheckerLoader.LoadBeforeEclipse1(),
                    MainPlugin.CheckerLoader.LoadBeforeSQueenSlime(),
                    MainPlugin.CheckerLoader.LoadBeforeAnyMech(),
                    MainPlugin.CheckerLoader.LoadBeforeOld2(),
                    MainPlugin.CheckerLoader.LoadBeforeTwins(),
                    MainPlugin.CheckerLoader.LoadBeforeDestroyer(),
                    MainPlugin.CheckerLoader.LoadBeforePrime(),
                    MainPlugin.CheckerLoader.LoadBeforeAllMech(),
                    MainPlugin.CheckerLoader.LoadBeforeEclipse2(),
                    MainPlugin.CheckerLoader.LoadBeforeDukeFishron(),
                    MainPlugin.CheckerLoader.LoadBeforePlantera(),
                    MainPlugin.CheckerLoader.LoadBeforeEclipse3(),
                    MainPlugin.CheckerLoader.LoadBeforeEverscream(),
                    MainPlugin.CheckerLoader.LoadBeforeSantaNk1(),
                    MainPlugin.CheckerLoader.LoadBeforeIceQueen(),
                    MainPlugin.CheckerLoader.LoadBeforeMouringWood(),
                    MainPlugin.CheckerLoader.LoadBeforePumpking(),
                    MainPlugin.CheckerLoader.LoadBeforeEmpressOfLight(),
                    MainPlugin.CheckerLoader.LoadBeforeGolem(),
                    MainPlugin.CheckerLoader.LoadBeforeOld3(),
                    MainPlugin.CheckerLoader.LoadBeforeMartian(),
                    MainPlugin.CheckerLoader.LoadBeforeCultist(),
                    MainPlugin.CheckerLoader.LoadBeforeSolar(),
                    MainPlugin.CheckerLoader.LoadBeforeVortex(),
                    MainPlugin.CheckerLoader.LoadBeforeStardust(),
                    MainPlugin.CheckerLoader.LoadBeforeNebula(),
                    MainPlugin.CheckerLoader.LoadBeforeAllPillars(),
                    MainPlugin.CheckerLoader.LoadBeforeMoonLord()
                };
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("超进度物品限制配置读取错误:" + ex.ToString());
            }
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00005C4C File Offset: 0x00003E4C
        private void OnPostInitialize(EventArgs args)
        {
            TShock.Log.ConsoleInfo("test");
            bool flag = !NPC.downedGoblins && !NPC.downedSlimeKing && !NPC.downedDeerclops && !NPC.downedBoss1 && !NPC.downedBoss2 && !NPC.downedQueenBee && !NPC.downedBoss3 && !Main.hardMode;
            if (flag)
            {
                MainPlugin.IOPDConfig.哥布林一进行 = false;
                MainPlugin.IOPDConfig.旧日军团一 = false;
                MainPlugin.IOPDConfig.哥布林二进行 = false;
                MainPlugin.IOPDConfig.海盗进行 = false;
                MainPlugin.IOPDConfig.日蚀一进行 = false;
                MainPlugin.IOPDConfig.旧日军团二 = false;
                MainPlugin.IOPDConfig.日蚀二进行 = false;
                MainPlugin.IOPDConfig.日蚀三进行 = false;
                MainPlugin.IOPDConfig.旧日军团三 = false;
                MainPlugin.IOPDConfig.火星进行 = false;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
                this.ReadConfig();
            }
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00005D30 File Offset: 0x00003F30
        private void OnUpdate(object args)
        {
            this.validCheckers = from checker in this.checkers
                                 where !checker.Obsolete
                                 select checker;
            Item[] item = Main.item;
            Item[] array = item;
            foreach (Item item2 in array)
            {
                bool flag = !item2.active;
                if (!flag)
                {
                    foreach (IItemChecker itemChecker in this.validCheckers)
                    {
                        bool flag2 = itemChecker.Contains(item2.type) && !this.InWhiteList(item2.type);
                        if (flag2)
                        {
                            item2.active = false;
                            NetMessage.SendData(90, -1, -1, null, item2.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            break;
                        }
                    }
                }
            }
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00005E48 File Offset: 0x00004048
        private void NpcKilled(NpcKilledEventArgs args)
        {
            bool flag = !MainPlugin.IOPDConfig.哥布林一进行 && (args.npc.netID == 26 || args.npc.netID == 27 || args.npc.netID == 28 || args.npc.netID == 29 || args.npc.netID == 111);
            if (flag)
            {
                MainPlugin.IOPDConfig.哥布林一进行 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag2 = !MainPlugin.IOPDConfig.旧日军团一 && (args.npc.netID == 564 || args.npc.netID == 565);
            if (flag2)
            {
                MainPlugin.IOPDConfig.旧日军团一 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag3 = !MainPlugin.IOPDConfig.哥布林二进行 && args.npc.netID == 471;
            if (flag3)
            {
                MainPlugin.IOPDConfig.哥布林二进行 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag4 = !MainPlugin.IOPDConfig.海盗进行 && (args.npc.netID == 212 || args.npc.netID == 213 || args.npc.netID == 214 || args.npc.netID == 215 || args.npc.netID == 216 || args.npc.netID == 491);
            if (flag4)
            {
                MainPlugin.IOPDConfig.海盗进行 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag5 = !MainPlugin.IOPDConfig.日蚀一进行 && (args.npc.netID == 158 || args.npc.netID == 159 || args.npc.netID == 461);
            if (flag5)
            {
                MainPlugin.IOPDConfig.日蚀一进行 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag6 = !MainPlugin.IOPDConfig.旧日军团二 && (args.npc.netID == 576 || args.npc.netID == 577);
            if (flag6)
            {
                MainPlugin.IOPDConfig.旧日军团二 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag7 = !MainPlugin.IOPDConfig.日蚀二进行 && (args.npc.netID == 253 || args.npc.netID == 477);
            if (flag7)
            {
                MainPlugin.IOPDConfig.日蚀二进行 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag8 = !MainPlugin.IOPDConfig.日蚀三进行 && (args.npc.netID == 460 || args.npc.netID == 463 || args.npc.netID == 466 || args.npc.netID == 467 || args.npc.netID == 468);
            if (flag8)
            {
                MainPlugin.IOPDConfig.日蚀三进行 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag9 = !MainPlugin.IOPDConfig.旧日军团三 && args.npc.netID == 551;
            if (flag9)
            {
                MainPlugin.IOPDConfig.旧日军团三 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
            bool flag10 = !MainPlugin.IOPDConfig.火星进行 && (args.npc.netID == 381 || args.npc.netID == 382 || args.npc.netID == 383 || args.npc.netID == 384 || args.npc.netID == 385 || args.npc.netID == 386 || args.npc.netID == 389 || args.npc.netID == 390 || args.npc.netID == 392);
            if (flag10)
            {
                MainPlugin.IOPDConfig.火星进行 = true;
                ConfigFile.WriteConfig(MainPlugin.IOPDConfig);
            }
        }

        // Token: 0x0600001F RID: 31 RVA: 0x000062AC File Offset: 0x000044AC
        private bool InWhiteList(int type)
        {
            return MainPlugin.IOPDConfig.物品白名单.Count != 0 && Array.BinarySearch<int>(this.quickSort(MainPlugin.IOPDConfig.物品白名单.ToArray(), 0, MainPlugin.IOPDConfig.物品白名单.Count - 1), type, this.comparer) >= 0;
        }

        // Token: 0x06000020 RID: 32 RVA: 0x0000630C File Offset: 0x0000450C
        private int[] quickSort(int[] arr, int low, int high)
        {
            bool flag = low < high;
            if (flag)
            {
                int num = this.partition(arr, low, high);
                this.quickSort(arr, low, num - 1);
                this.quickSort(arr, num + 1, high);
            }
            return arr;
        }

        // Token: 0x06000021 RID: 33 RVA: 0x0000634C File Offset: 0x0000454C
        private int partition(int[] arr, int low, int high)
        {
            int num = arr[high];
            int num2 = low - 1;
            for (int i = low; i <= high - 1; i++)
            {
                bool flag = arr[i] < num;
                if (flag)
                {
                    num2++;
                    this.swap(arr, num2, i);
                }
            }
            this.swap(arr, num2 + 1, high);
            return num2 + 1;
        }

        // Token: 0x06000022 RID: 34 RVA: 0x000063AC File Offset: 0x000045AC
        private int[] swap(int[] arr, int low, int high)
        {
            int num = arr[low];
            arr[low] = arr[high];
            arr[high] = num;
            return arr;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x000063D0 File Offset: 0x000045D0
        private void OnItemSlot(object sender, GetDataHandlers.PlayerSlotEventArgs args)
        {
            TSPlayer tsplayer = TShock.Players[(int)args.PlayerId];
            bool flag = !tsplayer.IsLoggedIn || tsplayer.HasPermission("tshock.ignore.ssc") || tsplayer.HasPermission("无视超进度物品限制") || !tsplayer.Active;
            if (!flag)
            {
                foreach (IItemChecker itemChecker in this.validCheckers)
                {
                    bool flag2 = itemChecker.Contains((int)args.Type) && !this.InWhiteList((int)args.Type) && MainPlugin.IOPDConfig.封禁玩家;
                    if (flag2)
                    {
                        this.LogDetected(args);
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
                        defaultInterpolatedStringHandler.AppendLiteral("[i:");
                        defaultInterpolatedStringHandler.AppendFormatted<short>(args.Type);
                        defaultInterpolatedStringHandler.AppendLiteral("]如果你是清白的,就请来解释一通");
                        string text = defaultInterpolatedStringHandler.ToStringAndClear();
                        tsplayer.Ban(text, "Server");
                        args.Type = 0;
                        args.Handled = true;
                        tsplayer.Disconnect(text);
                    }
                    else
                    {
                        bool flag3 = !itemChecker.Contains((int)args.Type) || this.InWhiteList((int)args.Type) || MainPlugin.IOPDConfig.封禁玩家;
                        if (!flag3)
                        {
                            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
                            defaultInterpolatedStringHandler.AppendLiteral("检测到玩家背包里持有超进度物品: [i:");
                            defaultInterpolatedStringHandler.AppendFormatted<short>(args.Type);
                            defaultInterpolatedStringHandler.AppendLiteral("] 并进行清理！");
                            string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
                            tsplayer.SendErrorMessage(text2);
                            string text3 = "检测到玩家: " + tsplayer.Name + " 背包里持有超进度物品 " + Lang.GetItemNameValue((int)args.Type);
                            TShock.Log.ConsoleInfo(text3);
                            bool flag4 = MainPlugin.IOPDConfig.广播超进度 && ((this.LastType == args.Type && DateTime.Now > this.LastTime) || this.LastType != args.Type);
                            if (flag4)
                            {
                                TShockAPI.Utils utils = TShock.Utils;
                                defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
                                defaultInterpolatedStringHandler.AppendLiteral("好家伙，[i:");
                                defaultInterpolatedStringHandler.AppendFormatted<short>(args.Type);
                                defaultInterpolatedStringHandler.AppendLiteral("]怎么拿到的？说你呢：[");
                                defaultInterpolatedStringHandler.AppendFormatted(args.Player.Name);
                                defaultInterpolatedStringHandler.AppendLiteral("]");
                                utils.Broadcast(defaultInterpolatedStringHandler.ToStringAndClear(), 0, byte.MaxValue, byte.MaxValue);
                            }
                            this.LastTime = DateTime.Now.AddMinutes(1.0);
                            this.LastType = args.Type;
                            bool 启动惩罚 = MainPlugin.IOPDConfig.启动惩罚;
                            if (启动惩罚)
                            {
                                tsplayer.SetBuff(47, 300, false);
                                tsplayer.SetBuff(149, 300, false);
                                tsplayer.SetBuff(156, 300, false);
                            }
                            bool flag5 = args.Slot <= 58;
                            if (flag5)
                            {
                                tsplayer.TPlayer.inventory[(int)args.Slot] = new Item();
                            }
                            else
                            {
                                bool flag6 = args.Slot != 179;
                                if (flag6)
                                {
                                    int num = 59;
                                    Item[] array = tsplayer.TPlayer.armor;
                                    foreach (Item item in array)
                                    {
                                        bool flag7 = item.netID == 0 || (int)args.Type == item.netID;
                                        if (flag7)
                                        {
                                            tsplayer.TPlayer.armor[num - 59] = new Item();
                                            tsplayer.SendData((PacketTypes)5, null, tsplayer.Index, (float)num, 0f, 0f, 0);
                                        }
                                        num++;
                                    }
                                    array = tsplayer.TPlayer.dye;
                                    foreach (Item item2 in array)
                                    {
                                        bool flag8 = item2.netID == 0 || (int)args.Type == item2.netID;
                                        if (flag8)
                                        {
                                            tsplayer.TPlayer.dye[num - 79] = new Item();
                                            tsplayer.SendData((PacketTypes)5, null, tsplayer.Index, (float)num, 0f, 0f, 0);
                                        }
                                        num++;
                                    }
                                    array = tsplayer.TPlayer.miscEquips;
                                    foreach (Item item3 in array)
                                    {
                                        bool flag9 = item3.netID == 0 || (int)args.Type == item3.netID;
                                        if (flag9)
                                        {
                                            tsplayer.TPlayer.miscEquips[num - 89] = new Item();
                                            tsplayer.SendData((PacketTypes)5, null, tsplayer.Index, (float)num, 0f, 0f, 0);
                                        }
                                        num++;
                                    }
                                    array = tsplayer.TPlayer.miscDyes;
                                    foreach (Item item4 in array)
                                    {
                                        bool flag10 = item4.netID == 0 || (int)args.Type == item4.netID;
                                        if (flag10)
                                        {
                                            tsplayer.TPlayer.miscDyes[num - 94] = new Item();
                                            tsplayer.SendData((PacketTypes)5, null, tsplayer.Index, (float)num, 0f, 0f, 0);
                                        }
                                        num++;
                                    }
                                    args.Handled = true;
                                    break;
                                }
                                tsplayer.TPlayer.trashItem = new Item();
                            }
                            tsplayer.SendData((PacketTypes)5, null, tsplayer.Index, (float)args.Slot, 0f, 0f, 0);
                            args.Handled = true;
                        }
                    }
                }
            }
        }

        // Token: 0x06000024 RID: 36 RVA: 0x000069A0 File Offset: 0x00004BA0
        private void LogDetected(GetDataHandlers.PlayerSlotEventArgs args)
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 3);
            defaultInterpolatedStringHandler.AppendFormatted(TShock.Players[(int)args.PlayerId].Name);
            defaultInterpolatedStringHandler.AppendLiteral(" 有违规物品 ");
            defaultInterpolatedStringHandler.AppendFormatted(Lang.GetItemNameValue((int)args.Type));
            defaultInterpolatedStringHandler.AppendLiteral("(");
            defaultInterpolatedStringHandler.AppendFormatted<short>(args.Type);
            defaultInterpolatedStringHandler.AppendLiteral(")), 已被插件自动封禁");
            string text = defaultInterpolatedStringHandler.ToStringAndClear();
            TShock.Log.ConsoleInfo(text);
        }

        // Token: 0x0400003B RID: 59
        private HookHandler<EventArgs> updateHandler;

        // Token: 0x0400003C RID: 60
        private IList<IItemChecker> checkers;

        // Token: 0x0400003D RID: 61
        private IEnumerable<IItemChecker> validCheckers;

        // Token: 0x0400003E RID: 62
        private Comparer<int> comparer = Comparer<int>.Default;

        // Token: 0x0400003F RID: 63
        public static string iPermission = "无视超进度物品检测";

        // Token: 0x04000041 RID: 65
        private DateTime LastTime;

        // Token: 0x04000042 RID: 66
        private short LastType;

        // Token: 0x02000007 RID: 7
        public static bool GetJb(string key)
        {
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", key))
            {
                if (表.Read())
                {

                    return 表.Get<bool>("value");
                }
                else
                {
                    return false;
                }
            }
        }
        private static class CheckerLoader
        {
            // Token: 0x06000026 RID: 38 RVA: 0x00006A3C File Offset: 0x00004C3C
            internal static IItemChecker LoadBlackList()
            {
                IItemChecker itemChecker = new DefaultItemChecker(() => false);
                foreach (int id in MainPlugin.IOPDConfig.物品黑名单)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000027 RID: 39 RVA: 0x00006AC4 File Offset: 0x00004CC4
            internal static IItemChecker LoadBeforeGoblin1()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断哥布林)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedGoblins") || MainPlugin.IOPDConfig.哥布林一进行);
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedGoblins || MainPlugin.IOPDConfig.哥布林一进行);
                }

                foreach (int id in MainPlugin.IOPDConfig.哥布林一前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000028 RID: 40 RVA: 0x00006B4C File Offset: 0x00004D4C
            internal static IItemChecker LoadBeforeKingSlime()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断萌王)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedSlimeKing"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedSlimeKing);
                }

                foreach (int id in MainPlugin.IOPDConfig.萌王前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000029 RID: 41 RVA: 0x00006BD4 File Offset: 0x00004DD4
            internal static IItemChecker LoadBeforeDeerClops()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断鹿角怪)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedDeerclops"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedDeerclops);
                }
                foreach (int id in MainPlugin.IOPDConfig.鹿角怪前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600002A RID: 42 RVA: 0x00006C5C File Offset: 0x00004E5C
            internal static IItemChecker LoadBeforeEye()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断克眼)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedBoss1"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedBoss1);
                }

                foreach (int id in MainPlugin.IOPDConfig.克眼前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600002B RID: 43 RVA: 0x00006CE4 File Offset: 0x00004EE4
            internal static IItemChecker LoadBeforeEvil()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断虫脑)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedBoss2"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedBoss2 || NPC.npcsFoundForCheckActive[13] || NPC.npcsFoundForCheckActive[266]);
                }
                foreach (int id in MainPlugin.IOPDConfig.虫脑前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600002C RID: 44 RVA: 0x00006D6C File Offset: 0x00004F6C
            internal static IItemChecker LoadBeforeOld1()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断旧日一)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("旧日军团一"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedBoss2 & MainPlugin.IOPDConfig.旧日军团一);
                }
                foreach (int id in MainPlugin.IOPDConfig.旧日一前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600002D RID: 45 RVA: 0x00006DF4 File Offset: 0x00004FF4
            internal static IItemChecker LoadBeforeQueenBee()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断蜂后)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedQueenBee"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedQueenBee);
                }

                foreach (int id in MainPlugin.IOPDConfig.蜂后前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600002E RID: 46 RVA: 0x00006E7C File Offset: 0x0000507C
            internal static IItemChecker LoadBeforeSkeletron()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断骷髅)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedBoss3"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedBoss3);
                }

                foreach (int id in MainPlugin.IOPDConfig.骷髅前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600002F RID: 47 RVA: 0x00006F04 File Offset: 0x00005104
            internal static IItemChecker LoadBeforeWallOfFlesh()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断肉墙)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("hardMode"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => Main.hardMode);
                }
                foreach (int id in MainPlugin.IOPDConfig.肉墙前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000030 RID: 48 RVA: 0x00006F8C File Offset: 0x0000518C
            internal static IItemChecker LoadBeforeGoblin2()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断哥布林二)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("哥布林二进行"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => Main.hardMode & MainPlugin.IOPDConfig.哥布林二进行);
                }
                foreach (int id in MainPlugin.IOPDConfig.哥布林二前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000031 RID: 49 RVA: 0x00007014 File Offset: 0x00005214
            internal static IItemChecker LoadBeforePirates()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断海盗)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("海盗进行"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedPirates || MainPlugin.IOPDConfig.海盗进行);
                }
                foreach (int id in MainPlugin.IOPDConfig.海盗前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000032 RID: 50 RVA: 0x0000709C File Offset: 0x0000529C
            internal static IItemChecker LoadBeforeEclipse1()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断日蚀一)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("日蚀一进行"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => Main.hardMode & MainPlugin.IOPDConfig.日蚀一进行);
                }
                foreach (int id in MainPlugin.IOPDConfig.日蚀一前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000033 RID: 51 RVA: 0x00007124 File Offset: 0x00005324
            internal static IItemChecker LoadBeforeSQueenSlime()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断萌后)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedQueenSlime"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedQueenSlime);
                }
                foreach (int id in MainPlugin.IOPDConfig.萌后前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000034 RID: 52 RVA: 0x000071AC File Offset: 0x000053AC
            internal static IItemChecker LoadBeforeAnyMech()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断任一三王)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedMechBossAny"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMechBossAny);
                }
                foreach (int id in MainPlugin.IOPDConfig.任一三王前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000035 RID: 53 RVA: 0x00007234 File Offset: 0x00005434
            internal static IItemChecker LoadBeforeOld2()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断旧日二)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("旧日军团二"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMechBossAny & MainPlugin.IOPDConfig.旧日军团二);
                }
                foreach (int id in MainPlugin.IOPDConfig.旧日二前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000036 RID: 54 RVA: 0x000072BC File Offset: 0x000054BC
            internal static IItemChecker LoadBeforeTwins()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断机械眼)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedMechBoss2"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMechBoss2);
                }

                foreach (int id in MainPlugin.IOPDConfig.机械眼前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000037 RID: 55 RVA: 0x00007344 File Offset: 0x00005544
            internal static IItemChecker LoadBeforeDestroyer()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断机械虫)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedMechBoss1"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMechBoss1);
                }
                foreach (int id in MainPlugin.IOPDConfig.机械虫前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000038 RID: 56 RVA: 0x000073CC File Offset: 0x000055CC
            internal static IItemChecker LoadBeforePrime()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断机械骷髅)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedMechBoss3"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMechBoss3);
                }
                foreach (int id in MainPlugin.IOPDConfig.机械骷髅前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000039 RID: 57 RVA: 0x00007454 File Offset: 0x00005654
            internal static IItemChecker LoadBeforeAllMech()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断三王前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedMechBoss1") & GetJb("downedMechBoss2") & GetJb("downedMechBoss3"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3);
                }
                foreach (int id in MainPlugin.IOPDConfig.三王前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600003A RID: 58 RVA: 0x000074DC File Offset: 0x000056DC
            internal static IItemChecker LoadBeforeEclipse2()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断日蚀二)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("日蚀二进行"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3 & MainPlugin.IOPDConfig.日蚀二进行);
                }
                foreach (int id in MainPlugin.IOPDConfig.日蚀二前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600003B RID: 59 RVA: 0x00007564 File Offset: 0x00005764
            internal static IItemChecker LoadBeforeDukeFishron()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断猪鲨前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedFishron"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedFishron);
                }
                foreach (int id in MainPlugin.IOPDConfig.猪鲨前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600003C RID: 60 RVA: 0x000075EC File Offset: 0x000057EC
            internal static IItemChecker LoadBeforePlantera()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断妖花前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedPlantBoss"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedPlantBoss);
                }
                foreach (int id in MainPlugin.IOPDConfig.妖花前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600003D RID: 61 RVA: 0x00007674 File Offset: 0x00005874
            internal static IItemChecker LoadBeforeEclipse3()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断日蚀三)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("日蚀三进行"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedPlantBoss & MainPlugin.IOPDConfig.日蚀三进行);
                }
                foreach (int id in MainPlugin.IOPDConfig.日蚀三前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600003E RID: 62 RVA: 0x000076FC File Offset: 0x000058FC
            internal static IItemChecker LoadBeforeEverscream()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断霜月树)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedChristmasTree"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedChristmasTree);
                }
                foreach (int id in MainPlugin.IOPDConfig.霜月树前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600003F RID: 63 RVA: 0x00007784 File Offset: 0x00005984
            internal static IItemChecker LoadBeforeSantaNk1()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断霜月坦)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedChristmasSantank"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedChristmasSantank);
                }
                foreach (int id in MainPlugin.IOPDConfig.霜月坦前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000040 RID: 64 RVA: 0x0000780C File Offset: 0x00005A0C
            internal static IItemChecker LoadBeforeIceQueen()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断霜月后)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedChristmasIceQueen"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedChristmasIceQueen);
                }
                foreach (int id in MainPlugin.IOPDConfig.霜月后前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000041 RID: 65 RVA: 0x00007894 File Offset: 0x00005A94
            internal static IItemChecker LoadBeforeMouringWood()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断南瓜树)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedHalloweenTree"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedHalloweenTree);
                }
                foreach (int id in MainPlugin.IOPDConfig.南瓜树前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000042 RID: 66 RVA: 0x0000791C File Offset: 0x00005B1C
            internal static IItemChecker LoadBeforePumpking()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断南瓜王)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedHalloweenKing"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedHalloweenKing);
                }
                foreach (int id in MainPlugin.IOPDConfig.南瓜王前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000043 RID: 67 RVA: 0x000079A4 File Offset: 0x00005BA4
            internal static IItemChecker LoadBeforeEmpressOfLight()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断光女前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedEmpressOfLight"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedEmpressOfLight);
                }
                foreach (int id in MainPlugin.IOPDConfig.光女前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000044 RID: 68 RVA: 0x00007A2C File Offset: 0x00005C2C
            internal static IItemChecker LoadBeforeGolem()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断石巨人)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedGolemBoss"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedGolemBoss);
                }
                foreach (int id in MainPlugin.IOPDConfig.石巨人前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000045 RID: 69 RVA: 0x00007AB4 File Offset: 0x00005CB4
            internal static IItemChecker LoadBeforeOld3()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断旧日三)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("旧日军团三"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedGolemBoss & MainPlugin.IOPDConfig.旧日军团三);
                }
                foreach (int id in MainPlugin.IOPDConfig.旧日三前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000046 RID: 70 RVA: 0x00007B3C File Offset: 0x00005D3C
            internal static IItemChecker LoadBeforeMartian()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断虫脑)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("火星进行"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMartians || MainPlugin.IOPDConfig.火星进行);
                }
                foreach (int id in MainPlugin.IOPDConfig.外星人前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000047 RID: 71 RVA: 0x00007BC4 File Offset: 0x00005DC4
            internal static IItemChecker LoadBeforeCultist()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断教徒前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedAncientCultist"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedAncientCultist);
                }
                foreach (int id in MainPlugin.IOPDConfig.教徒前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000048 RID: 72 RVA: 0x00007C4C File Offset: 0x00005E4C
            internal static IItemChecker LoadBeforeSolar()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断日耀前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedTowerSolar"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedTowerSolar);
                }
                foreach (int id in MainPlugin.IOPDConfig.日耀前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x06000049 RID: 73 RVA: 0x00007CD4 File Offset: 0x00005ED4
            internal static IItemChecker LoadBeforeVortex()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断星旋前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedTowerVortex"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedTowerVortex);
                }
                foreach (int id in MainPlugin.IOPDConfig.星旋前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600004A RID: 74 RVA: 0x00007D5C File Offset: 0x00005F5C
            internal static IItemChecker LoadBeforeStardust()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断星尘前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedTowerStardust"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedTowerStardust);
                }
                foreach (int id in MainPlugin.IOPDConfig.星尘前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600004B RID: 75 RVA: 0x00007DE4 File Offset: 0x00005FE4
            internal static IItemChecker LoadBeforeNebula()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断星云前)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedTowerNebula"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedTowerNebula);
                }
                foreach (int id in MainPlugin.IOPDConfig.星云前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600004C RID: 76 RVA: 0x00007E6C File Offset: 0x0000606C
            internal static IItemChecker LoadBeforeAllPillars()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断所有柱子)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedTowerSolar") & GetJb("downedTowerVortex") & GetJb("downedTowerStardust") & GetJb("downedTowerNebula"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedTowerSolar & NPC.downedTowerVortex & NPC.downedTowerStardust & NPC.downedTowerNebula);
                }
                foreach (int id in MainPlugin.IOPDConfig.所有柱子前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }

            // Token: 0x0600004D RID: 77 RVA: 0x00007EF4 File Offset: 0x000060F4
            internal static IItemChecker LoadBeforeMoonLord()
            {
                IItemChecker itemChecker;
                if (MainPlugin.IOPDConfig.连接远程服务器判断进度 && MainPlugin.IOPDConfig.是否启用远程进度判断月总)
                {
                    itemChecker = new DefaultItemChecker(() => GetJb("downedMoonlord"));
                }
                else
                {
                    itemChecker = new DefaultItemChecker(() => NPC.downedMoonlord);
                }
                foreach (int id in MainPlugin.IOPDConfig.月总前禁物品)
                {
                    itemChecker.Add(id);
                }
                return itemChecker;
            }
        }
    }
}
