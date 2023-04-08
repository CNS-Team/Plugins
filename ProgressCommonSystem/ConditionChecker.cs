using System.Collections.Generic;

namespace ProgressCommonSystem;

internal static class ConditionChecker
{
    public static Dictionary<string, BossProgress> BossFields;

    public static Dictionary<string, EventProgress> EventFields;

    static ConditionChecker()
    {
        BossFields = new Dictionary<string, BossProgress>();
        EventFields = new Dictionary<string, EventProgress>();
        BossFields["NPC.downedSlimeKing"] = BossProgress.KingSlime;
        BossFields["NPC.downedBoss1"] = BossProgress.EyeOfCthulhu;
        BossFields["NPC.downedBoss2"] = BossProgress.EaterOfWorlds;
        BossFields["NPC.downedBoss2"] = BossProgress.BrainOfCthulhu;
        BossFields["NPC.downedBoss3"] = BossProgress.Skeletron;
        BossFields["NPC.downedQueenBee"] = BossProgress.QueenBee;
        BossFields["NPC.downedDeerclops"] = BossProgress.Deerclops;
        BossFields["Main.hardMode"] = BossProgress.WallOfFlesh;
        BossFields["NPC.downedQueenSlime"] = BossProgress.QueenSlime;
        BossFields["NPC.downedMechBoss1"] = BossProgress.TheDestroyer;
        BossFields["NPC.downedMechBoss2"] = BossProgress.TheTwins;
        BossFields["NPC.downedMechBoss3"] = BossProgress.SkeletronPrime;
        BossFields["NPC.downedPlantBoss"] = BossProgress.Plantera;
        BossFields["NPC.downedGolemBoss"] = BossProgress.Golem;
        BossFields["NPC.downedAncientCultist"] = BossProgress.LunaticCultist;
        BossFields["NPC.downedTowerSolar"] = BossProgress.SolarPillar;
        BossFields["NPC.downedTowerVortex"] = BossProgress.VortexPillar;
        BossFields["NPC.downedTowerNebula"] = BossProgress.NebulaPillar;
        BossFields["NPC.downedTowerStardust"] = BossProgress.StardustPillar;
        BossFields["NPC.downedMoonlord"] = BossProgress.MoonLord;
        EventFields["NPC.downedGoblins"] = EventProgress.GoblinArmy;
        EventFields["NPC.downedFrost"] = EventProgress.FrostLegion;
        EventFields["NPC.downedPirates"] = EventProgress.PirateInvasion;
        EventFields["NPC.downedMartians"] = EventProgress.MartianMadness;
        EventFields["ExtraData.downedBloodMoon"] = EventProgress.BloodMoon;
        EventFields["ExtraData.downedPumpkinMoon"] = EventProgress.PumpkinMoon;
        EventFields["ExtraData.downedFrostMoon"] = EventProgress.FrostMoon;
        EventFields["ExtraData.downedSolarEclipse"] = EventProgress.SolarEclipse;
    }
}