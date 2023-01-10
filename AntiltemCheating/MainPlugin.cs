using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace AntiItemCheating;
[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    private static class CheckerLoader
    {
        internal static IItemChecker LoadBlackList()
        {
            IItemChecker itemChecker = new DefaultItemChecker(() => false);
            foreach (var item in IOPDConfig.物品黑名单)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeGoblin1()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断哥布林) ? new DefaultItemChecker(() => GetJb("downedGoblins") || IOPDConfig.哥布林一进行) : new DefaultItemChecker(() => NPC.downedGoblins || IOPDConfig.哥布林一进行);
            foreach (var item in IOPDConfig.哥布林一前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeKingSlime()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断萌王) ? new DefaultItemChecker(() => GetJb("downedSlimeKing")) : new DefaultItemChecker(() => NPC.downedSlimeKing);
            foreach (var item in IOPDConfig.萌王前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeDeerClops()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断鹿角怪) ? new DefaultItemChecker(() => GetJb("downedDeerclops")) : new DefaultItemChecker(() => NPC.downedDeerclops);
            foreach (var item in IOPDConfig.鹿角怪前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeEye()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断克眼) ? new DefaultItemChecker(() => GetJb("downedBoss1")) : new DefaultItemChecker(() => NPC.downedBoss1);
            foreach (var item in IOPDConfig.克眼前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeEvil()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断虫脑) ? new DefaultItemChecker(() => GetJb("downedBoss2")) : new DefaultItemChecker(() => NPC.downedBoss2 || NPC.npcsFoundForCheckActive[13] || NPC.npcsFoundForCheckActive[266]);
            foreach (var item in IOPDConfig.虫脑前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeOld1()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断旧日一) ? new DefaultItemChecker(() => GetJb("旧日军团一")) : new DefaultItemChecker(() => NPC.downedBoss2 & IOPDConfig.旧日军团一);
            foreach (var item in IOPDConfig.旧日一前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeQueenBee()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断蜂后) ? new DefaultItemChecker(() => GetJb("downedQueenBee")) : new DefaultItemChecker(() => NPC.downedQueenBee);
            foreach (var item in IOPDConfig.蜂后前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeSkeletron()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断骷髅) ? new DefaultItemChecker(() => GetJb("downedBoss3")) : new DefaultItemChecker(() => NPC.downedBoss3);
            foreach (var item in IOPDConfig.骷髅前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeWallOfFlesh()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断肉墙) ? new DefaultItemChecker(() => GetJb("hardMode")) : new DefaultItemChecker(() => Main.hardMode);
            foreach (var item in IOPDConfig.肉墙前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeGoblin2()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断哥布林二) ? new DefaultItemChecker(() => GetJb("哥布林二进行")) : new DefaultItemChecker(() => Main.hardMode & IOPDConfig.哥布林二进行);
            foreach (var item in IOPDConfig.哥布林二前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforePirates()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断海盗) ? new DefaultItemChecker(() => GetJb("海盗进行")) : new DefaultItemChecker(() => NPC.downedPirates || IOPDConfig.海盗进行);
            foreach (var item in IOPDConfig.海盗前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeEclipse1()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断日蚀一) ? new DefaultItemChecker(() => GetJb("日蚀一进行")) : new DefaultItemChecker(() => Main.hardMode & IOPDConfig.日蚀一进行);
            foreach (var item in IOPDConfig.日蚀一前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeSQueenSlime()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断萌后) ? new DefaultItemChecker(() => GetJb("downedQueenSlime")) : new DefaultItemChecker(() => NPC.downedQueenSlime);
            foreach (var item in IOPDConfig.萌后前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeAnyMech()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断任一三王) ? new DefaultItemChecker(() => GetJb("downedMechBossAny")) : new DefaultItemChecker(() => NPC.downedMechBossAny);
            foreach (var item in IOPDConfig.任一三王前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeOld2()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断旧日二) ? new DefaultItemChecker(() => GetJb("旧日军团二")) : new DefaultItemChecker(() => NPC.downedMechBossAny & IOPDConfig.旧日军团二);
            foreach (var item in IOPDConfig.旧日二前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeTwins()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断机械眼) ? new DefaultItemChecker(() => GetJb("downedMechBoss2")) : new DefaultItemChecker(() => NPC.downedMechBoss2);
            foreach (var item in IOPDConfig.机械眼前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeDestroyer()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断机械虫) ? new DefaultItemChecker(() => GetJb("downedMechBoss1")) : new DefaultItemChecker(() => NPC.downedMechBoss1);
            foreach (var item in IOPDConfig.机械虫前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforePrime()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断机械骷髅) ? new DefaultItemChecker(() => GetJb("downedMechBoss3")) : new DefaultItemChecker(() => NPC.downedMechBoss3);
            foreach (var item in IOPDConfig.机械骷髅前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeAllMech()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断三王前) ? new DefaultItemChecker(() => GetJb("downedMechBoss1") & GetJb("downedMechBoss2") & GetJb("downedMechBoss3")) : new DefaultItemChecker(() => NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3);
            foreach (var item in IOPDConfig.三王前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeEclipse2()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断日蚀二) ? new DefaultItemChecker(() => GetJb("日蚀二进行")) : new DefaultItemChecker(() => NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3 & IOPDConfig.日蚀二进行);
            foreach (var item in IOPDConfig.日蚀二前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeDukeFishron()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断猪鲨前) ? new DefaultItemChecker(() => GetJb("downedFishron")) : new DefaultItemChecker(() => NPC.downedFishron);
            foreach (var item in IOPDConfig.猪鲨前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforePlantera()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断妖花前) ? new DefaultItemChecker(() => GetJb("downedPlantBoss")) : new DefaultItemChecker(() => NPC.downedPlantBoss);
            foreach (var item in IOPDConfig.妖花前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeEclipse3()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断日蚀三) ? new DefaultItemChecker(() => GetJb("日蚀三进行")) : new DefaultItemChecker(() => NPC.downedPlantBoss & IOPDConfig.日蚀三进行);
            foreach (var item in IOPDConfig.日蚀三前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeEverscream()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断霜月树) ? new DefaultItemChecker(() => GetJb("downedChristmasTree")) : new DefaultItemChecker(() => NPC.downedChristmasTree);
            foreach (var item in IOPDConfig.霜月树前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeSantaNk1()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断霜月坦) ? new DefaultItemChecker(() => GetJb("downedChristmasSantank")) : new DefaultItemChecker(() => NPC.downedChristmasSantank);
            foreach (var item in IOPDConfig.霜月坦前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeIceQueen()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断霜月后) ? new DefaultItemChecker(() => GetJb("downedChristmasIceQueen")) : new DefaultItemChecker(() => NPC.downedChristmasIceQueen);
            foreach (var item in IOPDConfig.霜月后前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeMouringWood()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断南瓜树) ? new DefaultItemChecker(() => GetJb("downedHalloweenTree")) : new DefaultItemChecker(() => NPC.downedHalloweenTree);
            foreach (var item in IOPDConfig.南瓜树前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforePumpking()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断南瓜王) ? new DefaultItemChecker(() => GetJb("downedHalloweenKing")) : new DefaultItemChecker(() => NPC.downedHalloweenKing);
            foreach (var item in IOPDConfig.南瓜王前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeEmpressOfLight()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断光女前) ? new DefaultItemChecker(() => GetJb("downedEmpressOfLight")) : new DefaultItemChecker(() => NPC.downedEmpressOfLight);
            foreach (var item in IOPDConfig.光女前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeGolem()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断石巨人) ? new DefaultItemChecker(() => GetJb("downedGolemBoss")) : new DefaultItemChecker(() => NPC.downedGolemBoss);
            foreach (var item in IOPDConfig.石巨人前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeOld3()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断旧日三) ? new DefaultItemChecker(() => GetJb("旧日军团三")) : new DefaultItemChecker(() => NPC.downedGolemBoss & IOPDConfig.旧日军团三);
            foreach (var item in IOPDConfig.旧日三前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeMartian()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断虫脑) ? new DefaultItemChecker(() => GetJb("火星进行")) : new DefaultItemChecker(() => NPC.downedMartians || IOPDConfig.火星进行);
            foreach (var item in IOPDConfig.外星人前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeCultist()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断教徒前) ? new DefaultItemChecker(() => GetJb("downedAncientCultist")) : new DefaultItemChecker(() => NPC.downedAncientCultist);
            foreach (var item in IOPDConfig.教徒前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeSolar()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断日耀前) ? new DefaultItemChecker(() => GetJb("downedTowerSolar")) : new DefaultItemChecker(() => NPC.downedTowerSolar);
            foreach (var item in IOPDConfig.日耀前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeVortex()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断星旋前) ? new DefaultItemChecker(() => GetJb("downedTowerVortex")) : new DefaultItemChecker(() => NPC.downedTowerVortex);
            foreach (var item in IOPDConfig.星旋前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeStardust()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断星尘前) ? new DefaultItemChecker(() => GetJb("downedTowerStardust")) : new DefaultItemChecker(() => NPC.downedTowerStardust);
            foreach (var item in IOPDConfig.星尘前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeNebula()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断星云前) ? new DefaultItemChecker(() => GetJb("downedTowerNebula")) : new DefaultItemChecker(() => NPC.downedTowerNebula);
            foreach (var item in IOPDConfig.星云前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeAllPillars()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断所有柱子) ? new DefaultItemChecker(() => GetJb("downedTowerSolar") & GetJb("downedTowerVortex") & GetJb("downedTowerStardust") & GetJb("downedTowerNebula")) : new DefaultItemChecker(() => NPC.downedTowerSolar & NPC.downedTowerVortex & NPC.downedTowerStardust & NPC.downedTowerNebula);
            foreach (var item in IOPDConfig.所有柱子前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }

        internal static IItemChecker LoadBeforeMoonLord()
        {
            IItemChecker itemChecker = (IOPDConfig.连接远程服务器判断进度 && IOPDConfig.是否启用远程进度判断月总) ? new DefaultItemChecker(() => GetJb("downedMoonlord")) : new DefaultItemChecker(() => NPC.downedMoonlord);
            foreach (var item in IOPDConfig.月总前禁物品)
            {
                itemChecker.Add(item);
            }
            return itemChecker;
        }
    }

    private HookHandler<EventArgs> updateHandler;

    private IList<IItemChecker> checkers;

    private IEnumerable<IItemChecker> validCheckers;

    private readonly Comparer<int> comparer = Comparer<int>.Default;

    public static string iPermission = "无视超进度物品检测";

    private DateTime LastTime;

    private short LastType;

    public override string Author => "原作者: TOFOUT, 修改者: Dr.Toxic";

    public override string Description => "禁止物品超出当前进度";

    public override string Name => "超进度物品检测改";

    public override Version Version => new Version(1, 0, 0, 8);

    public static ConfigFile IOPDConfig { get; set; }

    internal static string IOPDConfigPath => Path.Combine(TShock.SavePath, "超进度物品限制.json");

    public MainPlugin(Main game)
        : base(game)
    {
        IOPDConfig = new ConfigFile();
    }

    public override void Initialize()
    {
        this.updateHandler = this.OnUpdate;
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPostInitialize);
        ServerApi.Hooks.GameUpdate.Register(this, this.updateHandler);
        ServerApi.Hooks.NpcKilled.Register(this, this.NpcKilled);
        GetDataHandlers.PlayerSlot += new EventHandler<GetDataHandlers.PlayerSlotEventArgs>(this.OnItemSlot);
        this.ReadConfig();
        Commands.ChatCommands.Add(new Command("重读超进度物品权限", this.CMD, "重读超进度物品限制")
        {
            HelpText = "输入 /重读超进度物品限制 重读超进度物品限制配置"
        });
    }

    protected override void Dispose(bool disposing)
    {
        ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnPostInitialize);
        ServerApi.Hooks.GameUpdate.Deregister(this, this.updateHandler);
        ServerApi.Hooks.NpcKilled.Deregister(this, this.NpcKilled);
        GetDataHandlers.PlayerSlot -= new EventHandler<GetDataHandlers.PlayerSlotEventArgs>(this.OnItemSlot);
        base.Dispose(disposing);
    }

    private void CMD(CommandArgs args)
    {
        this.ReadConfig();
        args.Player.SendSuccessMessage("重读超进度物品限制");
    }

    private void ReadConfig()
    {
        try
        {
            if (!File.Exists(IOPDConfigPath))
            {
                TShock.Log.ConsoleError("未找到超进度物品限制配置，已为您创建！");
            }
            IOPDConfig = ConfigFile.Read(IOPDConfigPath);
            IOPDConfig.Write(IOPDConfigPath);
            this.checkers = new List<IItemChecker>
            {
                CheckerLoader.LoadBlackList(),
                CheckerLoader.LoadBeforeGoblin1(),
                CheckerLoader.LoadBeforeKingSlime(),
                CheckerLoader.LoadBeforeDeerClops(),
                CheckerLoader.LoadBeforeEye(),
                CheckerLoader.LoadBeforeEvil(),
                CheckerLoader.LoadBeforeOld1(),
                CheckerLoader.LoadBeforeQueenBee(),
                CheckerLoader.LoadBeforeSkeletron(),
                CheckerLoader.LoadBeforeWallOfFlesh(),
                CheckerLoader.LoadBeforeGoblin2(),
                CheckerLoader.LoadBeforePirates(),
                CheckerLoader.LoadBeforeEclipse1(),
                CheckerLoader.LoadBeforeSQueenSlime(),
                CheckerLoader.LoadBeforeAnyMech(),
                CheckerLoader.LoadBeforeOld2(),
                CheckerLoader.LoadBeforeTwins(),
                CheckerLoader.LoadBeforeDestroyer(),
                CheckerLoader.LoadBeforePrime(),
                CheckerLoader.LoadBeforeAllMech(),
                CheckerLoader.LoadBeforeEclipse2(),
                CheckerLoader.LoadBeforeDukeFishron(),
                CheckerLoader.LoadBeforePlantera(),
                CheckerLoader.LoadBeforeEclipse3(),
                CheckerLoader.LoadBeforeEverscream(),
                CheckerLoader.LoadBeforeSantaNk1(),
                CheckerLoader.LoadBeforeIceQueen(),
                CheckerLoader.LoadBeforeMouringWood(),
                CheckerLoader.LoadBeforePumpking(),
                CheckerLoader.LoadBeforeEmpressOfLight(),
                CheckerLoader.LoadBeforeGolem(),
                CheckerLoader.LoadBeforeOld3(),
                CheckerLoader.LoadBeforeMartian(),
                CheckerLoader.LoadBeforeCultist(),
                CheckerLoader.LoadBeforeSolar(),
                CheckerLoader.LoadBeforeVortex(),
                CheckerLoader.LoadBeforeStardust(),
                CheckerLoader.LoadBeforeNebula(),
                CheckerLoader.LoadBeforeAllPillars(),
                CheckerLoader.LoadBeforeMoonLord()
            };
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("超进度物品限制配置读取错误:" + ex.ToString());
        }
    }

    private void OnPostInitialize(EventArgs args)
    {
        TShock.Log.ConsoleInfo("test");
        if (!NPC.downedGoblins && !NPC.downedSlimeKing && !NPC.downedDeerclops && !NPC.downedBoss1 && !NPC.downedBoss2 && !NPC.downedQueenBee && !NPC.downedBoss3 && !Main.hardMode)
        {
            IOPDConfig.哥布林一进行 = false;
            IOPDConfig.旧日军团一 = false;
            IOPDConfig.哥布林二进行 = false;
            IOPDConfig.海盗进行 = false;
            IOPDConfig.日蚀一进行 = false;
            IOPDConfig.旧日军团二 = false;
            IOPDConfig.日蚀二进行 = false;
            IOPDConfig.日蚀三进行 = false;
            IOPDConfig.旧日军团三 = false;
            IOPDConfig.火星进行 = false;
            ConfigFile.WriteConfig(IOPDConfig);
            this.ReadConfig();
        }
    }

    private void OnUpdate(object args)
    {
        this.validCheckers = this.checkers.Where((IItemChecker checker) => !checker.Obsolete);
        var item3 = Main.item;
        foreach (var item2 in item3)
        {
            if (!item2.active)
            {
                continue;
            }
            foreach (var validChecker in this.validCheckers)
            {
                if (validChecker.Contains(item2.type) && !this.InWhiteList(item2.type))
                {
                    item2.active = false;
                    NetMessage.SendData(90, -1, -1, null, item2.whoAmI);
                    break;
                }
            }
        }
    }

    private void NpcKilled(NpcKilledEventArgs args)
    {
        if (!IOPDConfig.哥布林一进行 && (args.npc.netID == 26 || args.npc.netID == 27 || args.npc.netID == 28 || args.npc.netID == 29 || args.npc.netID == 111))
        {
            IOPDConfig.哥布林一进行 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.旧日军团一 && (args.npc.netID == 564 || args.npc.netID == 565))
        {
            IOPDConfig.旧日军团一 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.哥布林二进行 && args.npc.netID == 471)
        {
            IOPDConfig.哥布林二进行 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.海盗进行 && (args.npc.netID == 212 || args.npc.netID == 213 || args.npc.netID == 214 || args.npc.netID == 215 || args.npc.netID == 216 || args.npc.netID == 491))
        {
            IOPDConfig.海盗进行 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.日蚀一进行 && (args.npc.netID == 158 || args.npc.netID == 159 || args.npc.netID == 461))
        {
            IOPDConfig.日蚀一进行 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.旧日军团二 && (args.npc.netID == 576 || args.npc.netID == 577))
        {
            IOPDConfig.旧日军团二 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.日蚀二进行 && (args.npc.netID == 253 || args.npc.netID == 477))
        {
            IOPDConfig.日蚀二进行 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.日蚀三进行 && (args.npc.netID == 460 || args.npc.netID == 463 || args.npc.netID == 466 || args.npc.netID == 467 || args.npc.netID == 468))
        {
            IOPDConfig.日蚀三进行 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.旧日军团三 && args.npc.netID == 551)
        {
            IOPDConfig.旧日军团三 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
        if (!IOPDConfig.火星进行 && (args.npc.netID == 381 || args.npc.netID == 382 || args.npc.netID == 383 || args.npc.netID == 384 || args.npc.netID == 385 || args.npc.netID == 386 || args.npc.netID == 389 || args.npc.netID == 390 || args.npc.netID == 392))
        {
            IOPDConfig.火星进行 = true;
            ConfigFile.WriteConfig(IOPDConfig);
        }
    }

    private bool InWhiteList(int type)
    {
        return IOPDConfig.物品白名单.Count != 0 && Array.BinarySearch(this.quickSort(IOPDConfig.物品白名单.ToArray(), 0, IOPDConfig.物品白名单.Count - 1), type, this.comparer) >= 0;
    }

    private int[] quickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            var num = this.partition(arr, low, high);
            this.quickSort(arr, low, num - 1);
            this.quickSort(arr, num + 1, high);
        }
        return arr;
    }

    private int partition(int[] arr, int low, int high)
    {
        var num = arr[high];
        var num2 = low - 1;
        for (var i = low; i <= high - 1; i++)
        {
            if (arr[i] < num)
            {
                num2++;
                this.swap(arr, num2, i);
            }
        }
        this.swap(arr, num2 + 1, high);
        return num2 + 1;
    }

    private int[] swap(int[] arr, int low, int high)
    {
        var num = arr[low];
        arr[low] = arr[high];
        arr[high] = num;
        return arr;
    }

    private void OnItemSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs args)
    {
        var tsplayer = TShock.Players[args.PlayerId];
        if (!tsplayer.IsLoggedIn || tsplayer.HasPermission("tshock.ignore.ssc") || tsplayer.HasPermission("无视超进度物品限制") || !tsplayer.Active)
        {
            return;
        }
        foreach (var itemChecker in this.validCheckers)
        {
            if (itemChecker.Contains(args.Type) && !this.InWhiteList(args.Type) && IOPDConfig.封禁玩家)
            {
                this.LogDetected(args);
                var text = $"[i:{args.Type}]如果你是清白的,就请来解释一通";
                tsplayer.Ban(text, "Server");
                args.Type = 0;
                args.Handled = true;
                tsplayer.Disconnect(text);
            }
            else
            {
                if (!itemChecker.Contains(args.Type) || this.InWhiteList(args.Type) || IOPDConfig.封禁玩家)
                {
                    continue;
                }
                tsplayer.SendErrorMessage($"检测到玩家背包里持有超进度物品: [i:{args.Type}] 并进行清理！");
                var text2 = "检测到玩家: " + tsplayer.Name + " 背包里持有超进度物品 " + Lang.GetItemNameValue(args.Type);
                TShock.Log.ConsoleInfo(text2);
                if (IOPDConfig.广播超进度 && ((this.LastType == args.Type && DateTime.Now > this.LastTime) || this.LastType != args.Type))
                {
                    TShock.Utils.Broadcast($"好家伙，[i:{args.Type}]怎么拿到的？说你呢：[{args.Player.Name}]", 0, byte.MaxValue, byte.MaxValue);
                }
                this.LastTime = DateTime.Now.AddMinutes(1.0);
                this.LastType = args.Type;
                if (IOPDConfig.启动惩罚)
                {
                    tsplayer.SetBuff(47, 300);
                    tsplayer.SetBuff(149, 300);
                    tsplayer.SetBuff(156, 300);
                }
                if (args.Slot <= 58)
                {
                    tsplayer.TPlayer.inventory[args.Slot] = new Item();
                }
                else
                {
                    if (args.Slot != 179)
                    {
                        var num = 59;
                        var armor = tsplayer.TPlayer.armor;
                        foreach (var item in armor)
                        {
                            if (item.netID == 0 || args.Type == item.netID)
                            {
                                tsplayer.TPlayer.armor[num - 59] = new Item();
                                tsplayer.SendData(PacketTypes.PlayerSlot, null, tsplayer.Index, num);
                            }
                            num++;
                        }
                        var dye = tsplayer.TPlayer.dye;
                        foreach (var item2 in dye)
                        {
                            if (item2.netID == 0 || args.Type == item2.netID)
                            {
                                tsplayer.TPlayer.dye[num - 79] = new Item();
                                tsplayer.SendData(PacketTypes.PlayerSlot, null, tsplayer.Index, num);
                            }
                            num++;
                        }
                        var miscEquips = tsplayer.TPlayer.miscEquips;
                        foreach (var item3 in miscEquips)
                        {
                            if (item3.netID == 0 || args.Type == item3.netID)
                            {
                                tsplayer.TPlayer.miscEquips[num - 89] = new Item();
                                tsplayer.SendData(PacketTypes.PlayerSlot, null, tsplayer.Index, num);
                            }
                            num++;
                        }
                        var miscDyes = tsplayer.TPlayer.miscDyes;
                        foreach (var item4 in miscDyes)
                        {
                            if (item4.netID == 0 || args.Type == item4.netID)
                            {
                                tsplayer.TPlayer.miscDyes[num - 94] = new Item();
                                tsplayer.SendData(PacketTypes.PlayerSlot, null, tsplayer.Index, num);
                            }
                            num++;
                        }
                        args.Handled = true;
                        break;
                    }
                    tsplayer.TPlayer.trashItem = new Item();
                }
                tsplayer.SendData(PacketTypes.PlayerSlot, null, tsplayer.Index, args.Slot);
                args.Handled = true;
            }
        }
    }

    private void LogDetected(GetDataHandlers.PlayerSlotEventArgs args)
    {
        var text = $"{TShock.Players[args.PlayerId].Name} 有违规物品 {Lang.GetItemNameValue(args.Type)}({args.Type})), 已被插件自动封禁";
        TShock.Log.ConsoleInfo(text);
    }

    public static bool GetJb(string key)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", key);
        return 表.Read() && 表.Get<bool>("value");
    }
}