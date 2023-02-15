using System.Collections.Generic;
using System.IO;
using ProgressCommonSystem;
using TShockAPI;

namespace AutoProgressControl;

internal class Config
{
	public static readonly string SavePath = Path.Combine(TShock.SavePath, "AutoProgressControl", "auto_progress.json");

	public Dictionary<BossProgress, int> BossProgressTime = new Dictionary<BossProgress, int>
	{
		[BossProgress.KingSlime] = 120,
		[BossProgress.EyeOfCthulhu] = 240,
		[BossProgress.BrainOfCthulhu] = 1440,
		[BossProgress.EaterOfWorlds] = 1440,
		[BossProgress.QueenBee] = 1440,
		[BossProgress.Skeletron] = 2880,
		[BossProgress.WallOfFlesh] = 4320,
		[BossProgress.QueenSlime] = 4320,
		[BossProgress.SkeletronPrime] = 5760,
		[BossProgress.TheDestroyer] = 5760,
		[BossProgress.TheTwins] = 5760,
		[BossProgress.Plantera] = 7200,
		[BossProgress.DukeFishron] = 8640,
		[BossProgress.Golem] = 8640,
		[BossProgress.EmpressOfLight] = 10080,
		[BossProgress.LunaticCultist] = 10080,
		[BossProgress.SolarPillar] = 10080,
		[BossProgress.NebulaPillar] = 10080,
		[BossProgress.VortexPillar] = 10080,
		[BossProgress.StardustPillar] = 10080,
		[BossProgress.MoonLord] = 11520
	};

	public Dictionary<EventProgress, int> EventProgressTime = new Dictionary<EventProgress, int>
	{
		[EventProgress.BloodMoon] = 240,
		[EventProgress.GoblinArmy] = 240,
		[EventProgress.TheOldOnesArmy] = 2880,
		[EventProgress.FrostLegion] = 240,
		[EventProgress.PirateInvasion] = 4320,
		[EventProgress.SolarEclipse] = 5760,
		[EventProgress.FrostMoon] = 7200,
		[EventProgress.PumpkinMoon] = 7200,
		[EventProgress.MartianMadness] = 10080
	};
}
