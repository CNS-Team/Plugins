using System.Reflection;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace ProgressQuery;

public static class Utils
{
    public static Dictionary<string, FieldInfo> ProgressFields = GetProgressFilelds();



    public static HashSet<int> GetDetectNPCs()
    {
        var result = new HashSet<int>();
        GetProgressNpcIds().ForEach(x => x.Value.ForEach(f => result.Add(f)));
        return result;
    }

    public static Dictionary<string, HashSet<int>> GetProgressNpcIds()
    {
        return new Dictionary<string, HashSet<int>>
        {
            { "史莱姆王",new HashSet<int>(){ NPCID.KingSlime} },
            { "克苏鲁之眼", new HashSet<int>(){ NPCID.EyeofCthulhu } },
            { "世界吞噬怪", new HashSet<int>(){ NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail} },
            { "克苏鲁之脑", new HashSet<int>(){  NPCID.BrainofCthulhu} },
            { "骷髅王",  new HashSet<int>(){NPCID.SkeletronHead ,NPCID.SkeletronHand }},
            { "鹿角怪", new HashSet<int>(){ NPCID.Deerclops } },
            { "蜂王", new HashSet<int>(){ NPCID.QueenBee } },
            { "血肉墙",new HashSet<int>(){ NPCID.WallofFlesh} },
            { "史莱姆皇后", new HashSet<int>(){ NPCID.QueenSlimeBoss } },
            { "双子魔眼", new HashSet<int>(){ NPCID.Retinazer, NPCID.Spazmatism} },
            { "毁灭者", new HashSet<int>(){ NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail }},
            { "机械骷髅王",new HashSet<int>(){ NPCID.SkeletronPrime } },
            { "世纪之花", new HashSet<int>(){ NPCID.Plantera }},
            { "石巨人",new HashSet<int>(){ NPCID.GolemHead, NPCID.Golem} },
            { "猪龙鱼公爵", new HashSet<int>(){ NPCID.DukeFishron} },
            { "光之女皇", new HashSet<int>(){ NPCID.HallowBoss } },
            { "拜月教邪教徒", new HashSet<int>(){ NPCID.CultistBoss } },
            { "日耀柱", new HashSet<int>(){ NPCID.LunarTowerSolar } },
            { "星云柱", new HashSet<int>(){ NPCID.LunarTowerNebula } },
            { "星旋柱", new HashSet<int>(){ NPCID.LunarTowerVortex } },
            { "星尘柱", new HashSet<int>(){ NPCID.LunarTowerStardust } },
            { "月亮领主", new HashSet<int>(){ NPCID.MoonLordHead, NPCID.MoonLordHand} },
            { "雪人军团", new HashSet<int>(){ NPCID.SnowmanGangsta,NPCID.MisterStabby,NPCID.SnowBalla } },
            { "哥布林军队", new HashSet<int>(){ NPCID.GoblinPeon, NPCID.GoblinThief, NPCID.GoblinWarrior, NPCID.GoblinSorcerer, NPCID.GoblinArcher, NPCID.ShadowFlameApparition } },
            { "海盗入侵", new HashSet<int>(){NPCID.PirateDeckhand, NPCID.PirateCorsair, NPCID.PirateDeadeye, NPCID.PirateCrossbower, NPCID.PirateCaptain, NPCID.PirateShip } },
            { "南瓜月", new HashSet<int>(){ NPCID.Pumpking } },
            { "火星暴乱", new HashSet<int>(){ NPCID.BrainScrambler, NPCID.RayGunner, NPCID.MartianOfficer, NPCID.ForceBubble, NPCID.GrayGrunt, NPCID.MartianEngineer, NPCID.GigaZapper, NPCID.ScutlixRider, NPCID.MartianSaucer } },
            { "霜月", new HashSet<int>(){ NPCID.IceQueen } }
        };
    }

    public static Dictionary<string, string> GetProgressNames()
    {
        return new Dictionary<string, string>
        {
            { "史莱姆王","downedSlimeKing"},
            { "克苏鲁之眼", "downedBoss1" },
            { "世界吞噬怪", "downedBoss2" },
            { "克苏鲁之脑", "downedBoss2" },
            { "骷髅王", "downedBoss3" },
            { "鹿角怪", "downedDeerclops" },
            { "蜂王", "downedQueenBee" },
            { "血肉墙", "hardMode" },
            { "史莱姆皇后", "downedQueenSlime" },
            { "双子魔眼", "downedMechBoss2" },
            { "毁灭者", "downedMechBoss1"},
            { "机械骷髅王","downedMechBoss3" },
            { "世纪之花", "downedPlantBoss"},
            { "石巨人","downedGolemBoss" },
            { "猪龙鱼公爵", "downedFishron"},
            { "光之女皇", "downedEmpressOfLight" },
            { "拜月教邪教徒", "downedAncientCultist" },
            { "日耀柱", "downedTowerSolar" },
            { "星云柱", "downedTowerNebula" },
            { "星旋柱", "downedTowerVortex" },
            { "星尘柱", "downedTowerStardust" },
            { "月亮领主", "downedMoonlord" },
            { "雪人军团", "downedFrost" },
            { "哥布林军队", "downedGoblins" },
            { "海盗入侵", "downedPirates" },
            { "南瓜月", "downedHalloweenKing" },
            { "火星暴乱", "downedMartians" },
            { "霜月", "downedChristmasIceQueen" }
        };
    }

    public static void SetGameProgress(string name, bool code)
    {
        if (ProgressFields.TryGetValue(name, out var field) && field != null)
        {
            field.SetValue(null, code);
        }
    }

    public static Dictionary<string, FieldInfo> GetProgressFilelds()
    {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        var Progress = new Dictionary<string, FieldInfo>();
        GetProgressNames().ForEach(x =>
        {
            if (x.Key == "血肉墙")
            {
                Progress.Add(x.Key, typeof(Main).GetField(x.Value));
            }
            else
            {
                Progress.Add(x.Key, typeof(NPC).GetField(x.Value));
            }
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        });
        return Progress;
    }

    public static Dictionary<string, bool> GetGameProgress()
    {
        Dictionary<string, bool> Progress = new();
        ProgressFields.ForEach(f => Progress.Add(f.Key, Convert.ToBoolean(f.Value.GetValue(null))));
        return Progress;
    }


    public static bool GetGameProgress(string name)
    {
        return ProgressFields.TryGetValue(name, out var field) && field != null && Convert.ToBoolean(field.GetValue(null));
    }

    public static Dictionary<string, bool> Ongoing()
    {
        var going = new Dictionary<string, bool>
        {
            { "霜月", Main.snowMoon },
            { "南瓜月", Main.pumpkinMoon },
            { "哥布林军队", Main.invasionType == 1 },
            { "雪人军团", Main.invasionType == 2 },
            { "海盗入侵", Main.invasionType == 3 },
            { "火星暴乱", Main.invasionType == 4 }
        };
        return going;
    }
}
