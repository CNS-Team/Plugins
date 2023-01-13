using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace DataSync;

// Token: 0x02000004 RID: 4
[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    // Token: 0x17000003 RID: 3
    // (get) Token: 0x06000006 RID: 6 RVA: 0x0000208E File Offset: 0x0000028E
    public override string Name => "DataSync";

    // Token: 0x06000007 RID: 7 RVA: 0x00002095 File Offset: 0x00000295
    public Plugin(Main game) : base(game)
    {
    }
    public static Config 配置 = new();
    private void Remove(CommandArgs args)
    {
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", nameof(Main.hardMode)))
        {
            if (表.Read())
            {
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(Main.hardMode), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedSlimeKing), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedDeerclops), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedClown), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedPlantBoss), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedGolemBoss), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMartians), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedFishron), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedHalloweenTree), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedHalloweenKing), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedChristmasIceQueen), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedChristmasTree), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedChristmasSantank), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedAncientCultist), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMoonlord), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedPirates), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedFrost), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedQueenBee), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedBoss1), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedBoss2), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedBoss3), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerSolar), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerVortex), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerStardust), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerNebula), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBoss1), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBoss2), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBoss3), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedEmpressOfLight), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedQueenSlime), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBossAny), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedGoblins), false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedStylist", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedBartender", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedMech", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedGolfer", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedAngler", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedWizard", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedTaxCollector", false);
            }
        }
        using (var 表2 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", "哥布林一进行"))
        {
            if (表2.Read())
            {
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "哥布林一进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "旧日军团一", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "哥布林二进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "海盗进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "日蚀一进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "旧日军团二", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "日蚀二进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "日蚀三进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "旧日军团三", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "火星进行", false);
            }
        }
    }

    private static void EnsureTable()
    {
        var db = TShock.DB;
        var sqlTable = new SqlTable("synctable", new SqlColumn[]
        {
            new SqlColumn("key", MySqlDbType.VarChar)
            {
                Primary = true,
                Length = 256
            },
            new SqlColumn("value", MySqlDbType.VarChar)
            {
                Length = 256
            }
        });
        var sqlTableCreator = new SqlTableCreator(db, (db.GetSqlType() == SqlType.Sqlite)
            ? new SqliteQueryCreator()
            : new MysqlQueryCreator());
        sqlTableCreator.EnsureTableStructure(sqlTable);

        using (var result = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", nameof(Main.hardMode)))
        {
            if (!result.Read())
            {
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(Main.hardMode), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedSlimeKing), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedDeerclops), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedClown), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedPlantBoss), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedGolemBoss), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMartians), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedFishron), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedHalloweenTree), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedHalloweenKing), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedChristmasIceQueen), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedChristmasTree), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedChristmasSantank), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedAncientCultist), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMoonlord), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedPirates), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedFrost), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedQueenBee), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedBoss1), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedBoss2), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedBoss3), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerSolar), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerVortex), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerStardust), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedTowerNebula), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBoss1), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBoss2), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBoss3), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedEmpressOfLight), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedQueenSlime), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedMechBossAny), false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", nameof(NPC.downedGoblins), false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedStylist", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedBartender", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedMech", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedGolfer", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedAngler", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedWizard", false);
                //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedTaxCollector", false);
            }
        }
        using (var result = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", "哥布林一进行"))
        {
            if (!result.Read())
            {
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "哥布林一进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "旧日军团一", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "哥布林二进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "海盗进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "日蚀一进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "旧日军团二", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "日蚀二进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "日蚀三进行", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "旧日军团三", false);
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1);", "火星进行", false);
            }
        }
    }

    public override void Initialize()
    {
        EnsureTable();
        ServerApi.Hooks.NpcKilled.Register(this, new HookHandler<NpcKilledEventArgs>(this.NpcKilled));
        ServerApi.Hooks.ServerJoin.Register(this, new HookHandler<JoinEventArgs>((args) => LoadProgress()));
        Commands.ChatCommands.Add(new Command("DataSync", this.Re, "reload"));
        Commands.ChatCommands.Add(new Command("DataSync", this.Remove, "重置进度同步"));
        Config.GetConfig();
    }

    public static bool QueryProgress(string key)
    {
        using (var result = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", key))
        {
            return result.Read() ? result.Get<bool>("value") : false;
        }
    }

    public static void UploadProgress(string key, object value)
    {
        TShock.DB.Query("UPDATE synctable SET value = @1 WHERE `key` = @0", key, value);
    }

    private void NpcKilled(NpcKilledEventArgs args)
    {
        switch (args.npc.netID)
        {
            case NPCID.GoblinPeon:
            case NPCID.GoblinThief:
            case NPCID.GoblinWarrior:
            case NPCID.GoblinSorcerer:
            case NPCID.GoblinArcher:
                if (!QueryProgress("哥布林一进行"))
                {
                    UploadProgress("哥布林一进行", true);
                }
                if (配置.进度同步哥布林 && NPC.downedGoblins)
                {
                    UploadProgress(nameof(NPC.downedGoblins), true);
                }
                break;
            case NPCID.DD2DarkMageT1:
            case NPCID.DD2DarkMageT3:
                if (!QueryProgress("旧日军团一"))
                {
                    UploadProgress("旧日军团一", true);
                }
                break;
            case NPCID.ShadowFlameApparition:
                if (!QueryProgress("哥布林二进行"))
                {
                    UploadProgress("哥布林二进行", true);
                }
                break;
            case NPCID.PirateDeckhand:
            case NPCID.PirateCorsair:
            case NPCID.PirateDeadeye:
            case NPCID.PirateCrossbower:
            case NPCID.PirateCaptain:
            case NPCID.PirateShip:
                if (!QueryProgress("海盗进行"))
                {
                    UploadProgress("海盗进行", true);
                }
                if (配置.进度同步海盗 || NPC.downedPirates)
                {
                    UploadProgress(nameof(NPC.downedPirates), true);
                }
                break;
            case NPCID.VampireBat:
            case NPCID.Vampire:
            case NPCID.CreatureFromTheDeep:
                if (!QueryProgress("日蚀一进行"))
                {
                    UploadProgress("日蚀一进行", true);
                }
                break;
            case NPCID.DD2OgreT2:
            case NPCID.DD2OgreT3:
                if (!QueryProgress("旧日军团二"))
                {
                    UploadProgress("旧日军团二", true);
                }
                break;
            case NPCID.Reaper:
            case NPCID.Mothron:
                if (!QueryProgress("日蚀二进行"))
                {
                    UploadProgress("日蚀二进行", true);
                }
                break;
            case NPCID.Butcher:
            case NPCID.Nailhead:
            case NPCID.Psycho:
            case NPCID.DeadlySphere:
            case NPCID.DrManFly:
                if (!QueryProgress("日蚀三进行"))
                {
                    UploadProgress("日蚀三进行", true);
                }
                break;
            case NPCID.DD2Betsy:
                if (!QueryProgress("旧日军团三"))
                {
                    UploadProgress("旧日军团三", true);
                }
                break;
            case NPCID.BrainScrambler:
            case NPCID.RayGunner:
            case NPCID.MartianOfficer:
            case NPCID.ForceBubble:
            case NPCID.GrayGrunt:
            case NPCID.MartianEngineer:
            case NPCID.GigaZapper:
            case NPCID.ScutlixRider:
            case NPCID.MartianSaucer:
                if (!QueryProgress("火星进行"))
                {
                    UploadProgress("火星进行", true);
                }
                break;
            case NPCID.KingSlime:
                if (配置.进度同步萌王 && NPC.downedSlimeKing)
                {
                    UploadProgress(nameof(NPC.downedSlimeKing), true);
                }
                break;
            case NPCID.QueenBee:
                if (配置.进度同步蜂后 || NPC.downedQueenBee)
                {
                    UploadProgress(nameof(NPC.downedQueenBee), true);
                }
                break;
            case NPCID.Pumpking:
                if (配置.进度同步南瓜王 || NPC.downedHalloweenKing)
                {
                    UploadProgress(nameof(NPC.downedHalloweenKing), true);
                }
                break;
            case NPCID.IceQueen:
                if (配置.进度同步霜月后 || NPC.downedChristmasIceQueen)
                {
                    UploadProgress(nameof(NPC.downedChristmasIceQueen), true);
                }
                break;
            case NPCID.HallowBoss:
                if (配置.进度同步光女前 || NPC.downedEmpressOfLight)
                {
                    UploadProgress(nameof(NPC.downedEmpressOfLight), true);
                }
                break;
            case NPCID.TheDestroyer:
            case NPCID.TheDestroyerBody:
            case NPCID.TheDestroyerTail:
                if (配置.进度同步机械虫 || NPC.downedMechBoss1)
                {
                    UploadProgress(nameof(NPC.downedMechBoss1), true);
                }
                if (配置.进度同步任一三王 || NPC.downedMechBossAny)
                {
                    UploadProgress(nameof(NPC.downedMechBossAny), true);
                }
                break;
            case NPCID.SkeletronPrime:
                if (配置.进度同步机械骷髅 || NPC.downedMechBoss3)
                {
                    UploadProgress(nameof(NPC.downedMechBoss3), true);
                }
                if (配置.进度同步任一三王 || NPC.downedMechBossAny)
                {
                    UploadProgress(nameof(NPC.downedMechBossAny), true);
                }
                break;
            case NPCID.BrainofCthulhu:
                if (配置.进度同步虫脑 || NPC.downedBoss2)
                {
                    UploadProgress(nameof(NPC.downedBoss2), true);
                }
                break;
            case NPCID.EyeofCthulhu:
                if (配置.进度同步克眼 && NPC.downedBoss1)
                {
                    UploadProgress(nameof(NPC.downedBoss1), true);
                }
                break;
            case NPCID.SkeletronHead:
            case NPCID.SkeletronHand:
                if (配置.进度同步骷髅 || NPC.downedBoss3)
                {
                    UploadProgress(nameof(NPC.downedBoss3), true);
                }
                break;
            case NPCID.GolemHead:
            case NPCID.Golem:
                if (配置.进度同步石巨人 || NPC.downedGolemBoss)
                {
                    UploadProgress(nameof(NPC.downedGolemBoss), true);
                }
                break;
            case NPCID.QueenSlimeBoss:
                if (配置.进度同步萌后 || NPC.downedQueenSlime)
                {
                    UploadProgress(nameof(NPC.downedQueenSlime), true);
                }
                break;
            case NPCID.Plantera:
                if (配置.进度同步妖花前 || NPC.downedPlantBoss)
                {
                    UploadProgress(nameof(NPC.downedPlantBoss), true);
                }
                break;
            case NPCID.EaterofWorldsBody:
            case NPCID.EaterofWorldsTail:

                if (配置.进度同步虫脑 || NPC.downedBoss2)
                {
                    UploadProgress(nameof(NPC.downedBoss2), true);
                }
                break;
            case NPCID.Retinazer:
            case NPCID.Spazmatism:
                if (配置.进度同步机械眼 || NPC.downedMechBoss2)
                {
                    UploadProgress(nameof(NPC.downedMechBoss2), true);
                }
                if (配置.进度同步任一三王 || NPC.downedMechBossAny)
                {
                    UploadProgress(nameof(NPC.downedMechBossAny), true);
                }
                break;
            case NPCID.WallofFlesh:
                if (配置.进度同步肉墙 || Main.hardMode)
                {
                    UploadProgress(nameof(Main.hardMode), true);
                }
                break;
            case NPCID.MoonLordHead:
            case NPCID.MoonLordHand:
                if (配置.进度同步月总 || NPC.downedMoonlord)
                {
                    UploadProgress(nameof(NPC.downedMoonlord), true);
                }
                break;
            case NPCID.DukeFishron:
                if (配置.进度同步猪鲨前 || NPC.downedFishron)
                {
                    UploadProgress(nameof(NPC.downedFishron), true);
                }
                break;
            case NPCID.LunarTowerVortex:
                if (配置.进度同步星旋前 || NPC.downedTowerVortex)
                {
                    UploadProgress(nameof(NPC.downedTowerVortex), true);
                }
                break;
            case NPCID.LunarTowerStardust:
                if (配置.进度同步星尘前 || NPC.downedTowerStardust)
                {
                    UploadProgress(nameof(NPC.downedTowerStardust), true);
                }
                break;
            case NPCID.LunarTowerNebula:
                if (配置.进度同步星云前 || NPC.downedTowerNebula)
                {
                    UploadProgress(nameof(NPC.downedTowerNebula), true);
                }
                break;
            case NPCID.LunarTowerSolar:
                if (配置.进度同步日耀前 || NPC.downedTowerSolar)
                {
                    UploadProgress(nameof(NPC.downedTowerSolar), true);
                }
                break;
            case NPCID.Deerclops:
                if (配置.进度同步鹿角怪 && NPC.downedDeerclops)
                {
                    UploadProgress(nameof(NPC.downedDeerclops), true);
                }
                break;
            case NPCID.CultistBoss:
                if (配置.进度同步教徒前 || NPC.downedAncientCultist)
                {
                    UploadProgress(nameof(NPC.downedAncientCultist), true);
                }
                break;
            case NPCID.MourningWood:
                if (配置.进度同步南瓜树 || NPC.downedHalloweenTree)
                {
                    UploadProgress(nameof(NPC.downedHalloweenTree), true);
                }
                break;
            case NPCID.SantaNK1:
                if (配置.进度同步霜月坦 || NPC.downedChristmasSantank)
                {
                    UploadProgress(nameof(NPC.downedChristmasSantank), true);
                }
                break;
            case NPCID.Everscream:
                if (配置.进度同步霜月树 || NPC.downedChristmasTree)
                {
                    UploadProgress(nameof(NPC.downedChristmasTree), true);
                }
                break;
            default:
                return;
        }
        LoadProgress();
    }
    private void Re(CommandArgs args)
    {
        try
        {
            Config.GetConfig();
            EnsureTable();
            LoadProgress();
            //args.Player.SendErrorMessage($"[QwRPG.Shop]重载成功！");
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[DataSync]配置文件读取错误");
        }
    }
    public static Dictionary<string, bool> LoadFromDb()
    {
        Dictionary<string, bool> list = new() { };
        using (var result = TShock.DB.QueryReader("SELECT * FROM synctable;"))
        {
            while (result.Read())
            {
                var key = result.Get<string>("key");
                var value = result.Get<bool>("value");
                list.Add(key, value);
            }
            return list;
        }
    }
    public static void LoadProgress()
    {
        if (!配置.是否同步)
        {
            return;
        }
        if (!Monitor.TryEnter(kg))
        {
            return;
        }
        foreach (var kvp in LoadFromDb())
        {
            if (kvp.Key.Contains("进行"))
            {
                continue;
            }
            if (配置.进度同步哥布林 && kvp.Key == nameof(NPC.downedGoblins))
            {
                NPC.downedGoblins = kvp.Value;
            }
            else if (配置.进度同步萌王 && kvp.Key == nameof(NPC.downedSlimeKing))
            {
                NPC.downedSlimeKing = kvp.Value;
            }
            else if (配置.进度同步鹿角怪 && kvp.Key == nameof(NPC.downedDeerclops))
            {
                NPC.downedDeerclops = kvp.Value;
            }
            else if (配置.进度同步克眼 && kvp.Key == nameof(NPC.downedBoss1))
            {
                NPC.downedBoss1 = kvp.Value;
            }
            else if (配置.进度同步虫脑 && kvp.Key == nameof(NPC.downedBoss2))
            {
                NPC.downedBoss2 = kvp.Value;
            }
            else if (配置.进度同步蜂后 && kvp.Key == nameof(NPC.downedQueenBee))
            {
                NPC.downedQueenBee = kvp.Value;
            }
            else if (配置.进度同步骷髅 && kvp.Key == nameof(NPC.downedBoss3))
            {
                NPC.downedBoss3 = kvp.Value;
            }
            else if (配置.进度同步肉墙 && kvp.Key == nameof(Main.hardMode))
            {
                Main.hardMode = kvp.Value;
            }
            else if (配置.进度同步海盗 && kvp.Key == nameof(NPC.downedPirates))
            {
                NPC.downedPirates = kvp.Value;
            }
            else if (配置.进度同步萌后 && kvp.Key == nameof(NPC.downedQueenSlime))
            {
                NPC.downedQueenSlime = kvp.Value;
            }
            else if (配置.进度同步任一三王 && kvp.Key == nameof(NPC.downedMechBossAny))
            {
                NPC.downedMechBossAny = kvp.Value;
            }
            else if (配置.进度同步机械眼 && kvp.Key == nameof(NPC.downedMechBoss2))
            {
                NPC.downedMechBoss2 = kvp.Value;
            }
            else if (配置.进度同步机械虫 && kvp.Key == nameof(NPC.downedMechBoss1))
            {
                NPC.downedMechBoss1 = kvp.Value;
            }
            else if (配置.进度同步机械骷髅 && kvp.Key == nameof(NPC.downedMechBoss3))
            {
                NPC.downedMechBoss3 = kvp.Value;
            }
            else if (配置.进度同步猪鲨前 && kvp.Key == nameof(NPC.downedFishron))
            {
                NPC.downedFishron = kvp.Value;
            }
            else if (配置.进度同步妖花前 && kvp.Key == nameof(NPC.downedPlantBoss))
            {
                NPC.downedPlantBoss = kvp.Value;
            }
            else if (配置.进度同步霜月树 && kvp.Key == nameof(NPC.downedChristmasTree))
            {
                NPC.downedChristmasTree = kvp.Value;
            }
            else if (配置.进度同步霜月坦 && kvp.Key == nameof(NPC.downedChristmasSantank))
            {
                NPC.downedChristmasSantank = kvp.Value;
            }
            else if (配置.进度同步霜月后 && kvp.Key == nameof(NPC.downedChristmasIceQueen))
            {
                NPC.downedChristmasIceQueen = kvp.Value;
            }
            else if (配置.进度同步南瓜树 && kvp.Key == nameof(NPC.downedHalloweenTree))
            {
                NPC.downedHalloweenTree = kvp.Value;
            }
            else if (配置.进度同步南瓜王 && kvp.Key == nameof(NPC.downedHalloweenKing))
            {
                NPC.downedHalloweenKing = kvp.Value;
            }
            else if (配置.进度同步光女前 && kvp.Key == nameof(NPC.downedEmpressOfLight))
            {
                NPC.downedEmpressOfLight = kvp.Value;
            }
            else if (配置.进度同步石巨人 && kvp.Key == nameof(NPC.downedGolemBoss))
            {
                NPC.downedGolemBoss = kvp.Value;
            }
            else if (配置.进度同步教徒前 && kvp.Key == nameof(NPC.downedAncientCultist))
            {
                NPC.downedAncientCultist = kvp.Value;
            }
            else if (配置.进度同步日耀前 && kvp.Key == nameof(NPC.downedTowerSolar))
            {
                NPC.downedTowerSolar = kvp.Value;
            }
            else if (配置.进度同步星旋前 && kvp.Key == nameof(NPC.downedTowerVortex))
            {
                NPC.downedTowerVortex = kvp.Value;
            }
            else if (配置.进度同步星尘前 && kvp.Key == nameof(NPC.downedTowerStardust))
            {
                NPC.downedTowerStardust = kvp.Value;
            }
            else if (配置.进度同步星云前 && kvp.Key == nameof(NPC.downedTowerNebula))
            {
                NPC.downedTowerNebula = kvp.Value;
            }
            else if (配置.进度同步月总 && kvp.Key == nameof(NPC.downedMoonlord))
            {
                NPC.downedMoonlord = kvp.Value;
            }
        }
        Monitor.Exit(kg);
    }

    public static object kg = new object();
}