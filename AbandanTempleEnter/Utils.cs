using Terraria;
using TShockAPI;


namespace AbandanTempleEnter;

internal static class Utils
{
    public static Config Config { get; set; } = new();

    public static bool IsInZoneLihzhardTemple(TSPlayer plr)
    {
        return Main.player[plr.Index].ZoneLihzhardTemple;
    }

    public static bool CheckProgress()
    {
        if (Config.hardMode)
        {
            if (!Main.hardMode)
            {
                return false;
            }
        }
        if (Config.threeBoss)
        {
            if (!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3)
            {
                return false;
            }
        }
        if (Config.plantBoss)
        {
            if (!NPC.downedPlantBoss)
            {
                return false;
            }
        }
        return true;
    }

}