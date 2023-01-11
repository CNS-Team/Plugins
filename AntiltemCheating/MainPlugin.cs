using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using static TShockAPI.GetDataHandlers;

namespace AntiItemCheating;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
	private static class CheckerLoader
	{
		internal static IItemChecker LoadBlackList()
		{
			IItemChecker itemChecker = new DefaultItemChecker(() => false);
			foreach (int item in IOPDConfig.物品黑名单)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeGoblin1()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断哥布林) ? new DefaultItemChecker(() => NPC.downedGoblins || IOPDConfig.哥布林一进行) : new DefaultItemChecker(() => GetJb("downedGoblins") || IOPDConfig.哥布林一进行));
			foreach (int item in IOPDConfig.哥布林一前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeKingSlime()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断萌王) ? new DefaultItemChecker(() => NPC.downedSlimeKing) : new DefaultItemChecker(() => GetJb("downedSlimeKing")));
			foreach (int item in IOPDConfig.萌王前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeDeerClops()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断鹿角怪) ? new DefaultItemChecker(() => NPC.downedDeerclops) : new DefaultItemChecker(() => GetJb("downedDeerclops")));
			foreach (int item in IOPDConfig.鹿角怪前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeEye()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断克眼) ? new DefaultItemChecker(() => NPC.downedBoss1) : new DefaultItemChecker(() => GetJb("downedBoss1")));
			foreach (int item in IOPDConfig.克眼前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeEvil()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断虫脑) ? new DefaultItemChecker(() => NPC.downedBoss2 || NPC.npcsFoundForCheckActive[13] || NPC.npcsFoundForCheckActive[266]) : new DefaultItemChecker(() => GetJb("downedBoss2")));
			foreach (int item in IOPDConfig.虫脑前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeOld1()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断旧日一) ? new DefaultItemChecker(() => NPC.downedBoss2 & IOPDConfig.旧日军团一) : new DefaultItemChecker(() => GetJb("旧日军团一")));
			foreach (int item in IOPDConfig.旧日一前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeQueenBee()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断蜂后) ? new DefaultItemChecker(() => NPC.downedQueenBee) : new DefaultItemChecker(() => GetJb("downedQueenBee")));
			foreach (int item in IOPDConfig.蜂后前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeSkeletron()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断骷髅) ? new DefaultItemChecker(() => NPC.downedBoss3) : new DefaultItemChecker(() => GetJb("downedBoss3")));
			foreach (int item in IOPDConfig.骷髅前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeWallOfFlesh()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断肉墙) ? new DefaultItemChecker(() => Main.hardMode) : new DefaultItemChecker(() => GetJb("hardMode")));
			foreach (int item in IOPDConfig.肉墙前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeGoblin2()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断哥布林二) ? new DefaultItemChecker(() => Main.hardMode & IOPDConfig.哥布林二进行) : new DefaultItemChecker(() => GetJb("哥布林二进行")));
			foreach (int item in IOPDConfig.哥布林二前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforePirates()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断海盗) ? new DefaultItemChecker(() => NPC.downedPirates || IOPDConfig.海盗进行) : new DefaultItemChecker(() => GetJb("海盗进行")));
			foreach (int item in IOPDConfig.海盗前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeEclipse1()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断日蚀一) ? new DefaultItemChecker(() => Main.hardMode & IOPDConfig.日蚀一进行) : new DefaultItemChecker(() => GetJb("日蚀一进行")));
			foreach (int item in IOPDConfig.日蚀一前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeSQueenSlime()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断萌后) ? new DefaultItemChecker(() => NPC.downedQueenSlime) : new DefaultItemChecker(() => GetJb("downedQueenSlime")));
			foreach (int item in IOPDConfig.萌后前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeAnyMech()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断任一三王) ? new DefaultItemChecker(() => NPC.downedMechBossAny) : new DefaultItemChecker(() => GetJb("downedMechBossAny")));
			foreach (int item in IOPDConfig.任一三王前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeOld2()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断旧日二) ? new DefaultItemChecker(() => NPC.downedMechBossAny & IOPDConfig.旧日军团二) : new DefaultItemChecker(() => GetJb("旧日军团二")));
			foreach (int item in IOPDConfig.旧日二前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeTwins()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断机械眼) ? new DefaultItemChecker(() => NPC.downedMechBoss2) : new DefaultItemChecker(() => GetJb("downedMechBoss2")));
			foreach (int item in IOPDConfig.机械眼前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeDestroyer()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断机械虫) ? new DefaultItemChecker(() => NPC.downedMechBoss1) : new DefaultItemChecker(() => GetJb("downedMechBoss1")));
			foreach (int item in IOPDConfig.机械虫前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforePrime()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断机械骷髅) ? new DefaultItemChecker(() => NPC.downedMechBoss3) : new DefaultItemChecker(() => GetJb("downedMechBoss3")));
			foreach (int item in IOPDConfig.机械骷髅前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeAllMech()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断三王前) ? new DefaultItemChecker(() => NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3) : new DefaultItemChecker(() => GetJb("downedMechBoss1") & GetJb("downedMechBoss2") & GetJb("downedMechBoss3")));
			foreach (int item in IOPDConfig.三王前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeEclipse2()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断日蚀二) ? new DefaultItemChecker(() => NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3 & IOPDConfig.日蚀二进行) : new DefaultItemChecker(() => GetJb("日蚀二进行")));
			foreach (int item in IOPDConfig.日蚀二前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeDukeFishron()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断猪鲨前) ? new DefaultItemChecker(() => NPC.downedFishron) : new DefaultItemChecker(() => GetJb("downedFishron")));
			foreach (int item in IOPDConfig.猪鲨前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforePlantera()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断妖花前) ? new DefaultItemChecker(() => NPC.downedPlantBoss) : new DefaultItemChecker(() => GetJb("downedPlantBoss")));
			foreach (int item in IOPDConfig.妖花前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeEclipse3()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断日蚀三) ? new DefaultItemChecker(() => NPC.downedPlantBoss & IOPDConfig.日蚀三进行) : new DefaultItemChecker(() => GetJb("日蚀三进行")));
			foreach (int item in IOPDConfig.日蚀三前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeEverscream()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断霜月树) ? new DefaultItemChecker(() => NPC.downedChristmasTree) : new DefaultItemChecker(() => GetJb("downedChristmasTree")));
			foreach (int item in IOPDConfig.霜月树前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeSantaNk1()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断霜月坦) ? new DefaultItemChecker(() => NPC.downedChristmasSantank) : new DefaultItemChecker(() => GetJb("downedChristmasSantank")));
			foreach (int item in IOPDConfig.霜月坦前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeIceQueen()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断霜月后) ? new DefaultItemChecker(() => NPC.downedChristmasIceQueen) : new DefaultItemChecker(() => GetJb("downedChristmasIceQueen")));
			foreach (int item in IOPDConfig.霜月后前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeMouringWood()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断南瓜树) ? new DefaultItemChecker(() => NPC.downedHalloweenTree) : new DefaultItemChecker(() => GetJb("downedHalloweenTree")));
			foreach (int item in IOPDConfig.南瓜树前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforePumpking()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断南瓜王) ? new DefaultItemChecker(() => NPC.downedHalloweenKing) : new DefaultItemChecker(() => GetJb("downedHalloweenKing")));
			foreach (int item in IOPDConfig.南瓜王前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeEmpressOfLight()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断光女前) ? new DefaultItemChecker(() => NPC.downedEmpressOfLight) : new DefaultItemChecker(() => GetJb("downedEmpressOfLight")));
			foreach (int item in IOPDConfig.光女前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeGolem()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断石巨人) ? new DefaultItemChecker(() => NPC.downedGolemBoss) : new DefaultItemChecker(() => GetJb("downedGolemBoss")));
			foreach (int item in IOPDConfig.石巨人前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeOld3()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断旧日三) ? new DefaultItemChecker(() => NPC.downedGolemBoss & IOPDConfig.旧日军团三) : new DefaultItemChecker(() => GetJb("旧日军团三")));
			foreach (int item in IOPDConfig.旧日三前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeMartian()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断虫脑) ? new DefaultItemChecker(() => NPC.downedMartians || IOPDConfig.火星进行) : new DefaultItemChecker(() => GetJb("火星进行")));
			foreach (int item in IOPDConfig.外星人前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeCultist()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断教徒前) ? new DefaultItemChecker(() => NPC.downedAncientCultist) : new DefaultItemChecker(() => GetJb("downedAncientCultist")));
			foreach (int item in IOPDConfig.教徒前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeSolar()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断日耀前) ? new DefaultItemChecker(() => NPC.downedTowerSolar) : new DefaultItemChecker(() => GetJb("downedTowerSolar")));
			foreach (int item in IOPDConfig.日耀前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeVortex()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断星旋前) ? new DefaultItemChecker(() => NPC.downedTowerVortex) : new DefaultItemChecker(() => GetJb("downedTowerVortex")));
			foreach (int item in IOPDConfig.星旋前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeStardust()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断星尘前) ? new DefaultItemChecker(() => NPC.downedTowerStardust) : new DefaultItemChecker(() => GetJb("downedTowerStardust")));
			foreach (int item in IOPDConfig.星尘前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeNebula()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断星云前) ? new DefaultItemChecker(() => NPC.downedTowerNebula) : new DefaultItemChecker(() => GetJb("downedTowerNebula")));
			foreach (int item in IOPDConfig.星云前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeAllPillars()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断所有柱子) ? new DefaultItemChecker(() => NPC.downedTowerSolar & NPC.downedTowerVortex & NPC.downedTowerStardust & NPC.downedTowerNebula) : new DefaultItemChecker(() => GetJb("downedTowerSolar") & GetJb("downedTowerVortex") & GetJb("downedTowerStardust") & GetJb("downedTowerNebula")));
			foreach (int item in IOPDConfig.所有柱子前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}

		internal static IItemChecker LoadBeforeMoonLord()
		{
			IItemChecker itemChecker = ((!IOPDConfig.连接远程服务器判断进度 || !IOPDConfig.是否启用远程进度判断月总) ? new DefaultItemChecker(() => NPC.downedMoonlord) : new DefaultItemChecker(() => GetJb("downedMoonlord")));
			foreach (int item in IOPDConfig.月总前禁物品)
			{
				itemChecker.Add(item);
			}
			return itemChecker;
		}
	}

	//private HookHandler<EventArgs> updateHandler;

	private IList<IItemChecker> checkers;

	private IEnumerable<IItemChecker> validCheckers;

	//private Comparer<int> comparer = Comparer<int>.Default;

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

		ServerApi.Hooks.GamePostInitialize.Register(this,OnPostInitialize);
		//ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
		ServerApi.Hooks.NpcKilled.Register(this, NpcKilled);
        GetDataHandlers.PlayerSlot += OnItemSlot;
		ReadConfig();
		List<Command> chatCommands = Commands.ChatCommands;
		Command val = new Command("重读超进度物品权限", new CommandDelegate(CMD), new string[1] { "重读超进度物品限制" });
		val.HelpText = "输入 /重读超进度物品限制 重读超进度物品限制配置";
		chatCommands.Add(val);
	}

	protected override void Dispose(bool disposing)
	{
		ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
		//ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
		ServerApi.Hooks.NpcKilled.Deregister(this,NpcKilled);
		GetDataHandlers.PlayerSlot -= OnItemSlot;
		base.Dispose(disposing);
	}

	private void CMD(CommandArgs args)
	{
		ReadConfig();
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
			checkers = new List<IItemChecker>
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
			ReadConfig();
		}
	}

    //private void OnUpdate(object args)
    //{
    //    validCheckers = checkers.Where((IItemChecker checker) => !checker.Obsolete);
    //    Item[] item = Main.item;
    //    Item[] array = item;
    //    Item[] array2 = array;
    //    foreach (Item val in array2)
    //    {
    //        if (!val.active)
    //        {
    //            continue;
    //        }
    //        foreach (IItemChecker validChecker in validCheckers)
    //        {
    //            if (validChecker.Contains(val.type) && !InWhiteList(val.type))
    //            {
    //                val.active = false;
    //                NetMessage.SendData(90, -1, -1, null, val.whoAmI, 0f, 0f, 0f, 0, 0, 0);
    //                break;
    //            }
    //        }
    //    }
    //}

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
		return type == 0 ? true : IOPDConfig.物品白名单.Exists(f => f == type);
		//return IOPDConfig.物品白名单.Count != 0 && Array.BinarySearch(quickSort(IOPDConfig.物品白名单.ToArray(), 0, IOPDConfig.物品白名单.Count - 1), type, comparer) >= 0;
	}

	//private int[] quickSort(int[] arr, int low, int high)
	//{
	//	if (low < high)
	//	{
	//		int num = partition(arr, low, high);
	//		quickSort(arr, low, num - 1);
	//		quickSort(arr, num + 1, high);
	//	}
	//	return arr;
	//}

	//private int partition(int[] arr, int low, int high)
	//{
	//	int num = arr[high];
	//	int num2 = low - 1;
	//	for (int i = low; i <= high - 1; i++)
	//	{
	//		if (arr[i] < num)
	//		{
	//			num2++;
	//			swap(arr, num2, i);
	//		}
	//	}
	//	swap(arr, num2 + 1, high);
	//	return num2 + 1;
	//}

	//private int[] swap(int[] arr, int low, int high)
	//{
	//	int num = arr[low];
	//	arr[low] = arr[high];
	//	arr[high] = num;
	//	return arr;
	//}

	private void OnItemSlot(object sender, PlayerSlotEventArgs args)
	{
		TSPlayer val = args.Player;
		if (!val.IsLoggedIn || val.HasPermission("tshock.ignore.ssc") || val.HasPermission("无视超进度物品限制") || !val.Active)
		{
			return;
		}
		validCheckers = checkers.Where((IItemChecker checker) => !checker.Obsolete);
		if (validCheckers.Any(f=>f.Contains(args.Type)))
		{
			if (!InWhiteList(args.Type))
			{
				if (IOPDConfig.封禁玩家)
				{
					LogDetected(args);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[i:");
					defaultInterpolatedStringHandler.AppendFormatted(args.Type);
					defaultInterpolatedStringHandler.AppendLiteral("]如果你是清白的,就请来解释一通");
					string text = defaultInterpolatedStringHandler.ToStringAndClear();
					val.Ban(text, "Server");
					args.Type = 0;
					args.Handled = true;
					val.Disconnect(text);
					return;
				}

				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 1);
                defaultInterpolatedStringHandler2.AppendLiteral("检测到玩家背包里持有超进度物品: [i:");
                defaultInterpolatedStringHandler2.AppendFormatted(args.Type);
                defaultInterpolatedStringHandler2.AppendLiteral("] 并进行清理！");
                string text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
                val.SendErrorMessage(text2);
                string text3 = "检测到玩家: " + val.Name + " 背包里持有超进度物品 " + Lang.GetItemNameValue(args.Type);
                TShock.Log.ConsoleInfo(text3);

                if (IOPDConfig.启动惩罚)
				{
					val.SetBuff(47, 300, false);
					val.SetBuff(149, 300, false);
					val.SetBuff(156, 300, false);
				}
				args.Stack = 0;
                val.SendData(PacketTypes.PlayerSlot, null, val.Index, args.Slot, 0f, 0f, 0);
            }
        }
        
        //foreach (IItemChecker validChecker in validCheckers)
        //{
        //    if (validChecker.Contains(args.Type) && !InWhiteList(args.Type) && IOPDConfig.封禁玩家)
        //    {
        //        LogDetected(args);
        //        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
        //        defaultInterpolatedStringHandler.AppendLiteral("[i:");
        //        defaultInterpolatedStringHandler.AppendFormatted(args.Type);
        //        defaultInterpolatedStringHandler.AppendLiteral("]如果你是清白的,就请来解释一通");
        //        string text = defaultInterpolatedStringHandler.ToStringAndClear();
        //        val.Ban(text, "Server");
        //        args.Type = 0;
        //        args.Handled = true;
        //        val.Disconnect(text);
        //    }
        //    else
        //    {
        //        if (!validChecker.Contains(args.Type) || InWhiteList(args.Type) || IOPDConfig.封禁玩家)
        //        {
        //            continue;
        //        }
        //        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 1);
        //        defaultInterpolatedStringHandler2.AppendLiteral("检测到玩家背包里持有超进度物品: [i:");
        //        defaultInterpolatedStringHandler2.AppendFormatted(args.Type);
        //        defaultInterpolatedStringHandler2.AppendLiteral("] 并进行清理！");
        //        string text2 = defaultInterpolatedStringHandler2.ToStringAndClear();
        //        val.SendErrorMessage(text2);
        //        string text3 = "检测到玩家: " + val.Name + " 背包里持有超进度物品 " + Lang.GetItemNameValue((int)args.Type);
        //        TShock.Log.ConsoleInfo(text3);
        //        //args.Stack = 0;
        //        //args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Slot);
        //        //if (IOPDConfig.广播超进度 && ((LastType == args.Type && DateTime.Now > LastTime) || LastType != args.Type))
        //        //{
        //        //	defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(20, 2);
        //        //	defaultInterpolatedStringHandler2.AppendLiteral("好家伙，[i:");
        //        //	defaultInterpolatedStringHandler2.AppendFormatted(args.Type);
        //        //	defaultInterpolatedStringHandler2.AppendLiteral("]怎么拿到的？说你呢：[");
        //        //	defaultInterpolatedStringHandler2.AppendFormatted(((GetDataHandledEventArgs)args).Player.Name);
        //        //	defaultInterpolatedStringHandler2.AppendLiteral("]");
        //        //	TShock.Utils.Broadcast(defaultInterpolatedStringHandler2.ToStringAndClear(), (byte)0, byte.MaxValue, byte.MaxValue);
        //        //}
        //        //LastTime = DateTime.Now.AddMinutes(1.0);
        //        //LastType = args.Type;
        //        if (IOPDConfig.启动惩罚)
        //        {
        //            val.SetBuff(47, 300, false);
        //            val.SetBuff(149, 300, false);
        //            val.SetBuff(156, 300, false);
        //        }


        //        if (args.Slot <= 58)
        //        {
        //            val.TPlayer.inventory[args.Slot] = new Item();
        //        }
        //        else
        //        {
        //            if (args.Slot != 179)
        //            {
        //                int num = 59;
        //                Item[] armor = val.TPlayer.armor;
        //                Item[] array = armor;
        //                foreach (Item val2 in array)
        //                {
        //                    if (val2.netID == 0 || args.Type == val2.netID)
        //                    {
        //                        val.TPlayer.armor[num - 59] = new Item();
        //                        val.SendData((PacketTypes)5, (string)null, val.Index, (float)num, 0f, 0f, 0);
        //                    }
        //                    num++;
        //                }
        //                armor = val.TPlayer.dye;
        //                Item[] array2 = armor;
        //                foreach (Item val3 in array2)
        //                {
        //                    if (val3.netID == 0 || args.Type == val3.netID)
        //                    {
        //                        val.TPlayer.dye[num - 79] = new Item();
        //                        val.SendData((PacketTypes)5, (string)null, val.Index, (float)num, 0f, 0f, 0);
        //                    }
        //                    num++;
        //                }
        //                armor = val.TPlayer.miscEquips;
        //                Item[] array3 = armor;
        //                foreach (Item val4 in array3)
        //                {
        //                    if (val4.netID == 0 || args.Type == val4.netID)
        //                    {
        //                        val.TPlayer.miscEquips[num - 89] = new Item();
        //                        val.SendData((PacketTypes)5, (string)null, val.Index, (float)num, 0f, 0f, 0);
        //                    }
        //                    num++;
        //                }
        //                armor = val.TPlayer.miscDyes;
        //                Item[] array4 = armor;
        //                foreach (Item val5 in array4)
        //                {
        //                    if (val5.netID == 0 || args.Type == val5.netID)
        //                    {
        //                        val.TPlayer.miscDyes[num - 94] = new Item();
        //                        val.SendData((PacketTypes)5, (string)null, val.Index, (float)num, 0f, 0f, 0);
        //                    }
        //                    num++;
        //                }
        //                args.Handled = true;
        //                break;
        //            }
        //            val.TPlayer.trashItem = new Item();
        //        }
        //        val.SendData(PacketTypes.PlayerSlot, null, val.Index, args.Slot, 0f, 0f, 0);
        //        args.Handled = true;
        //    }

        //}
    }

	private void LogDetected(PlayerSlotEventArgs args)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 3);
		defaultInterpolatedStringHandler.AppendFormatted(TShock.Players[args.PlayerId].Name);
		defaultInterpolatedStringHandler.AppendLiteral(" 有违规物品 ");
		defaultInterpolatedStringHandler.AppendFormatted(Lang.GetItemNameValue((int)args.Type));
		defaultInterpolatedStringHandler.AppendLiteral("(");
		defaultInterpolatedStringHandler.AppendFormatted(args.Type);
		defaultInterpolatedStringHandler.AppendLiteral(")), 已被插件自动封禁");
		string text = defaultInterpolatedStringHandler.ToStringAndClear();
		TShock.Log.ConsoleInfo(text);
	}

	public static bool GetJb(string key)
	{
		QueryResult val = DbExt.QueryReader(TShock.DB, "SELECT * FROM synctable WHERE `key`=@0", new object[1] { key });
		try
		{
			if (val.Read())
			{
				return val.Get<bool>("value");
			}
			return false;
		}
		finally
		{
			val.Dispose();
		}
	}
}
