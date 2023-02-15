using System.Collections.Generic;
using ProgressCommonSystem;

namespace AutoProgressControl;

internal class BossOrEvent
{
	public static readonly Dictionary<BossProgress, string> Boss = new Dictionary<BossProgress, string>
	{
		[BossProgress.KingSlime] = "史莱姆王",
		[BossProgress.EyeOfCthulhu] = "克苏鲁之眼",
		[BossProgress.EaterOfWorlds] = "世界吞噬者",
		[BossProgress.BrainOfCthulhu] = "克苏鲁之脑",
		[BossProgress.QueenBee] = "蜂王",
		[BossProgress.Skeletron] = "骷髅王",
		[BossProgress.Deerclops] = "独眼巨鹿",
		[BossProgress.WallOfFlesh] = "肉山",
		[BossProgress.QueenSlime] = "史莱姆皇后",
		[BossProgress.TheTwins] = "双子魔眼",
		[BossProgress.TheDestroyer] = "毁灭者",
		[BossProgress.SkeletronPrime] = "机械骷髅王",
		[BossProgress.Plantera] = "世纪之花",
		[BossProgress.Golem] = "石巨人",
		[BossProgress.DukeFishron] = "猪鲨",
		[BossProgress.EmpressOfLight] = "光之女皇",
		[BossProgress.LunaticCultist] = "拜月邪教徒",
		[BossProgress.SolarPillar] = "日耀柱",
		[BossProgress.NebulaPillar] = "星云柱",
		[BossProgress.VortexPillar] = "星旋柱",
		[BossProgress.StardustPillar] = "星尘柱",
		[BossProgress.MoonLord] = "月球领主"
	};

	public static readonly Dictionary<EventProgress, string> Event = new Dictionary<EventProgress, string>
	{
		[EventProgress.BloodMoon] = "血月",
		[EventProgress.GoblinArmy] = "哥布林军队",
		[EventProgress.TheOldOnesArmy] = "旧日军团",
		[EventProgress.FrostLegion] = "雪人军团",
		[EventProgress.SolarEclipse] = "日食",
		[EventProgress.PirateInvasion] = "海岛入侵",
		[EventProgress.PumpkinMoon] = "南瓜月",
		[EventProgress.FrostMoon] = "霜月",
		[EventProgress.MartianMadness] = "火星暴乱"
	};
}
