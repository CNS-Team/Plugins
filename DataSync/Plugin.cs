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

    private int _frameCount = 0;
    public override void Initialize()
    {
        Config.GetConfig();
        EnsureTable();
        ServerApi.Hooks.NpcKilled.Register(this, new HookHandler<NpcKilledEventArgs>(this.NpcKilled));
        ServerApi.Hooks.GameUpdate.Register(this, new HookHandler<EventArgs>((args) =>
        {
            this._frameCount++;
            if (this._frameCount % 300 == 0)
            {
                LoadProgress();
            }
        }));
        Commands.ChatCommands.Add(new Command("DataSync", this.Re, "reload"));
        Commands.ChatCommands.Add(new Command("DataSync", this.Remove, "重置进度同步"));
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
        TSPlayer.Server.SendInfoMessage($"[DataSync]上传进度 {key} {value}");
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
                if (配置.进度同步哥布林 && NPC.downedGoblins && !QueryProgress(nameof(NPC.downedGoblins)))
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
                if (配置.进度同步海盗 && NPC.downedPirates && !QueryProgress(nameof(NPC.downedPirates)))
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
                if (配置.进度同步萌王 && NPC.downedSlimeKing && !QueryProgress(nameof(NPC.downedSlimeKing)))
                {
                    UploadProgress(nameof(NPC.downedSlimeKing), true);
                }
                break;
            case NPCID.QueenBee:
                if (配置.进度同步蜂后 && NPC.downedQueenBee && !QueryProgress(nameof(NPC.downedQueenBee)))
                {
                    UploadProgress(nameof(NPC.downedQueenBee), true);
                }
                break;
            case NPCID.Pumpking:
                if (配置.进度同步南瓜王 && NPC.downedHalloweenKing && !QueryProgress(nameof(NPC.downedHalloweenKing)))
                {
                    UploadProgress(nameof(NPC.downedHalloweenKing), true);
                }
                break;
            case NPCID.IceQueen:
                if (配置.进度同步霜月后 && NPC.downedChristmasIceQueen && !QueryProgress(nameof(NPC.downedChristmasIceQueen)))
                {
                    UploadProgress(nameof(NPC.downedChristmasIceQueen), true);
                }
                break;
            case NPCID.HallowBoss:
                if (配置.进度同步光女前 && NPC.downedEmpressOfLight && !QueryProgress(nameof(NPC.downedEmpressOfLight)))
                {
                    UploadProgress(nameof(NPC.downedEmpressOfLight), true);
                }
                break;
            case NPCID.TheDestroyer:
            case NPCID.TheDestroyerBody:
            case NPCID.TheDestroyerTail:
                if (配置.进度同步机械虫 && NPC.downedMechBoss1 && !QueryProgress(nameof(NPC.downedMechBoss1)))
                {
                    UploadProgress(nameof(NPC.downedMechBoss1), true);
                }
                if (配置.进度同步任一三王 && NPC.downedMechBossAny && !QueryProgress(nameof(NPC.downedMechBossAny)))
                {
                    UploadProgress(nameof(NPC.downedMechBossAny), true);
                }
                break;
            case NPCID.SkeletronPrime:
                if (配置.进度同步机械骷髅 && NPC.downedMechBoss3 && !QueryProgress(nameof(NPC.downedMechBoss3)))
                {
                    UploadProgress(nameof(NPC.downedMechBoss3), true);
                }
                if (配置.进度同步任一三王 && NPC.downedMechBossAny && !QueryProgress(nameof(NPC.downedMechBossAny)))
                {
                    UploadProgress(nameof(NPC.downedMechBossAny), true);
                }
                break;
            case NPCID.BrainofCthulhu:
            case NPCID.EaterofWorldsBody:
            case NPCID.EaterofWorldsTail:
                if (配置.进度同步虫脑 && NPC.downedBoss2 && !QueryProgress(nameof(NPC.downedBoss2)))
                {
                    UploadProgress(nameof(NPC.downedBoss2), true);
                }
                break;
            case NPCID.EyeofCthulhu:
                if (配置.进度同步克眼 && NPC.downedBoss1 && !QueryProgress(nameof(NPC.downedBoss1)))
                {
                    UploadProgress(nameof(NPC.downedBoss1), true);
                }
                break;
            case NPCID.SkeletronHead:
            case NPCID.SkeletronHand:
                if (配置.进度同步骷髅 && NPC.downedBoss3 && !QueryProgress(nameof(NPC.downedBoss3)))
                {
                    UploadProgress(nameof(NPC.downedBoss3), true);
                }
                break;
            case NPCID.GolemHead:
            case NPCID.Golem:
                if (配置.进度同步石巨人 && NPC.downedGolemBoss && !QueryProgress(nameof(NPC.downedGolemBoss)))
                {
                    UploadProgress(nameof(NPC.downedGolemBoss), true);
                }
                break;
            case NPCID.QueenSlimeBoss:
                if (配置.进度同步萌后 && NPC.downedQueenSlime && !QueryProgress(nameof(NPC.downedQueenSlime)))
                {
                    UploadProgress(nameof(NPC.downedQueenSlime), true);
                }
                break;
            case NPCID.Plantera:
                if (配置.进度同步妖花前 && NPC.downedPlantBoss && !QueryProgress(nameof(NPC.downedPlantBoss)))
                {
                    UploadProgress(nameof(NPC.downedPlantBoss), true);
                }
                break;
            case NPCID.Retinazer:
            case NPCID.Spazmatism:
                if (配置.进度同步机械眼 && NPC.downedMechBoss2 && !QueryProgress(nameof(NPC.downedMechBoss2)))
                {
                    UploadProgress(nameof(NPC.downedMechBoss2), true);
                }
                if (配置.进度同步任一三王 && NPC.downedMechBossAny && !QueryProgress(nameof(NPC.downedMechBossAny)))
                {
                    UploadProgress(nameof(NPC.downedMechBossAny), true);
                }
                break;
            case NPCID.WallofFlesh:
                if (配置.进度同步肉墙 && Main.hardMode && !QueryProgress(nameof(Main.hardMode)))
                {
                    UploadProgress(nameof(Main.hardMode), true);
                }
                break;
            case NPCID.MoonLordHead:
            case NPCID.MoonLordHand:
                if (配置.进度同步月总 && NPC.downedMoonlord && !QueryProgress(nameof(NPC.downedMoonlord)))
                {
                    UploadProgress(nameof(NPC.downedMoonlord), true);
                }
                break;
            case NPCID.DukeFishron:
                if (配置.进度同步猪鲨前 && NPC.downedFishron && !QueryProgress(nameof(NPC.downedFishron)))
                {
                    UploadProgress(nameof(NPC.downedFishron), true);
                }
                break;
            case NPCID.LunarTowerVortex:
                if (配置.进度同步星旋前 && NPC.downedTowerVortex && !QueryProgress(nameof(NPC.downedTowerVortex)))
                {
                    UploadProgress(nameof(NPC.downedTowerVortex), true);
                }
                break;
            case NPCID.LunarTowerStardust:
                if (配置.进度同步星尘前 && NPC.downedTowerStardust && !QueryProgress(nameof(NPC.downedTowerStardust)))
                {
                    UploadProgress(nameof(NPC.downedTowerStardust), true);
                }
                break;
            case NPCID.LunarTowerNebula:
                if (配置.进度同步星云前 && NPC.downedTowerNebula && !QueryProgress(nameof(NPC.downedTowerNebula)))
                {
                    UploadProgress(nameof(NPC.downedTowerNebula), true);
                }
                break;
            case NPCID.LunarTowerSolar:
                if (配置.进度同步日耀前 && NPC.downedTowerSolar && !QueryProgress(nameof(NPC.downedTowerSolar)))
                {
                    UploadProgress(nameof(NPC.downedTowerSolar), true);
                }
                break;
            case NPCID.Deerclops:
                if (配置.进度同步鹿角怪 && NPC.downedDeerclops && !QueryProgress(nameof(NPC.downedDeerclops)))
                {
                    UploadProgress(nameof(NPC.downedDeerclops), true);
                }
                break;
            case NPCID.CultistBoss:
                if (配置.进度同步教徒前 && NPC.downedAncientCultist && !QueryProgress(nameof(NPC.downedAncientCultist)))
                {
                    UploadProgress(nameof(NPC.downedAncientCultist), true);
                }
                break;
            case NPCID.MourningWood:
                if (配置.进度同步南瓜树 && NPC.downedHalloweenTree && !QueryProgress(nameof(NPC.downedHalloweenTree)))
                {
                    UploadProgress(nameof(NPC.downedHalloweenTree), true);
                }
                break;
            case NPCID.SantaNK1:
                if (配置.进度同步霜月坦 && NPC.downedChristmasSantank && !QueryProgress(nameof(NPC.downedChristmasSantank)))
                {
                    UploadProgress(nameof(NPC.downedChristmasSantank), true);
                }
                break;
            case NPCID.Everscream:
                if (配置.进度同步霜月树 && NPC.downedChristmasTree && !QueryProgress(nameof(NPC.downedChristmasTree)))
                {
                    UploadProgress(nameof(NPC.downedChristmasTree), true);
                }
                break;
            default:
                return;
        }
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
        var data = LoadFromDb();
        if (配置.进度同步哥布林)
        {
            if (data.TryGetValue(nameof(NPC.downedGoblins), out var v) && v != NPC.downedGoblins)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]哥布林进度同步为{v}");
                NPC.downedGoblins = v;
            }
        }
        if (配置.进度同步萌王)
        {
            if (data.TryGetValue(nameof(NPC.downedSlimeKing), out var v) && v != NPC.downedSlimeKing)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]萌王进度同步为{v}");
                NPC.downedSlimeKing = v;
            }
        }
        if (配置.进度同步鹿角怪)
        {
            if (data.TryGetValue(nameof(NPC.downedDeerclops), out var v) && v != NPC.downedDeerclops)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]鹿角怪进度同步为{v}");
                NPC.downedDeerclops = v;
            }
        }
        if (配置.进度同步克眼)
        {
            if (data.TryGetValue(nameof(NPC.downedBoss1), out var v) && v != NPC.downedBoss1)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]克眼进度同步为{v}");
                NPC.downedBoss1 = v;
            }
        }
        if (配置.进度同步虫脑)
        {
            if (data.TryGetValue(nameof(NPC.downedBoss2), out var v) && v != NPC.downedBoss2)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]虫脑进度同步为{v}");
                NPC.downedBoss2 = v;
            }
        }
        if (配置.进度同步蜂后)
        {
            if (data.TryGetValue(nameof(NPC.downedQueenBee), out var v) && v != NPC.downedQueenBee)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]蜂后进度同步为{v}");
                NPC.downedQueenBee = v;
            }
        }
        if (配置.进度同步骷髅)
        {
            if (data.TryGetValue(nameof(NPC.downedBoss3), out var v) && v != NPC.downedBoss3)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]骷髅进度同步为{v}");
                NPC.downedBoss3 = v;
            }
        }
        if (配置.进度同步肉墙)
        {
            if (data.TryGetValue(nameof(Main.hardMode), out var v) && v != Main.hardMode)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]肉墙进度同步为{v}");
                Main.hardMode = v;
            }
        }
        if (配置.进度同步海盗)
        {
            if (data.TryGetValue(nameof(NPC.downedPirates), out var v) && v != NPC.downedPirates)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]海盗进度同步为{v}");
                NPC.downedPirates = v;
            }
        }
        if (配置.进度同步萌后)
        {
            if (data.TryGetValue(nameof(NPC.downedQueenSlime), out var v) && v != NPC.downedQueenSlime)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]萌后进度同步为{v}");
                NPC.downedQueenSlime = v;
            }
        }
        if (配置.进度同步任一三王)
        {
            if (data.TryGetValue(nameof(NPC.downedMechBossAny), out var v) && v != NPC.downedMechBossAny)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]任一三王进度同步为{v}");
                NPC.downedMechBossAny = v;
            }
        }
        if (配置.进度同步机械眼)
        {
            if (data.TryGetValue(nameof(NPC.downedMechBoss2), out var v) && v != NPC.downedMechBoss2)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]机械眼进度同步为{v}");
                NPC.downedMechBoss2 = v;
            }
        }
        if (配置.进度同步机械虫)
        {
            if (data.TryGetValue(nameof(NPC.downedMechBoss1), out var v) && v != NPC.downedMechBoss1)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]机械虫进度同步为{v}");
                NPC.downedMechBoss1 = v;
            }
        }
        if (配置.进度同步机械骷髅)
        {
            if (data.TryGetValue(nameof(NPC.downedMechBoss3), out var v) && v != NPC.downedMechBoss3)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]机械骷髅进度同步为{v}");
                NPC.downedMechBoss3 = v;
            }
        }
        if (配置.进度同步猪鲨前)
        {
            if (data.TryGetValue(nameof(NPC.downedFishron), out var v) && v != NPC.downedFishron)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]猪鲨前进度同步为{v}");
                NPC.downedFishron = v;
            }
        }
        if (配置.进度同步妖花前)
        {
            if (data.TryGetValue(nameof(NPC.downedPlantBoss), out var v) && v != NPC.downedPlantBoss)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]妖花前进度同步为{v}");
                NPC.downedPlantBoss = v;
            }
        }
        if (配置.进度同步霜月树)
        {
            if (data.TryGetValue(nameof(NPC.downedChristmasTree), out var v) && v != NPC.downedChristmasTree)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]霜月树进度同步为{v}");
                NPC.downedChristmasTree = v;
            }
        }
        if (配置.进度同步霜月坦)
        {
            if (data.TryGetValue(nameof(NPC.downedChristmasSantank), out var v) && v != NPC.downedChristmasSantank)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]霜月坦进度同步为{v}");
                NPC.downedChristmasSantank = v;
            }
        }
        if (配置.进度同步霜月后)
        {
            if (data.TryGetValue(nameof(NPC.downedChristmasIceQueen), out var v) && v != NPC.downedChristmasIceQueen)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]霜月后进度同步为{v}");
                NPC.downedChristmasIceQueen = v;
            }
        }
        if (配置.进度同步南瓜树)
        {
            if (data.TryGetValue(nameof(NPC.downedHalloweenTree), out var v) && v != NPC.downedHalloweenTree)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]南瓜树进度同步为{v}");
                NPC.downedHalloweenTree = v;
            }
        }
        if (配置.进度同步南瓜王)
        {
            if (data.TryGetValue(nameof(NPC.downedHalloweenKing), out var v) && v != NPC.downedHalloweenKing)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]南瓜王进度同步为{v}");
                NPC.downedHalloweenKing = v;
            }
        }
        if (配置.进度同步光女前)
        {
            if (data.TryGetValue(nameof(NPC.downedEmpressOfLight), out var v) && v != NPC.downedEmpressOfLight)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]光女前进度同步为{v}");
                NPC.downedEmpressOfLight = v;
            }
        }
        if (配置.进度同步石巨人)
        {
            if (data.TryGetValue(nameof(NPC.downedGolemBoss), out var v) && v != NPC.downedGolemBoss)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]石巨人进度同步为{v}");
                NPC.downedGolemBoss = v;
            }
        }
        if (配置.进度同步教徒前)
        {
            if (data.TryGetValue(nameof(NPC.downedAncientCultist), out var v) && v != NPC.downedAncientCultist)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]教徒前进度同步为{v}");
                NPC.downedAncientCultist = v;
            }
        }
        if (配置.进度同步日耀前)
        {
            if (data.TryGetValue(nameof(NPC.downedTowerSolar), out var v) && v != NPC.downedTowerSolar)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]日耀前进度同步为{v}");
                NPC.downedTowerSolar = v;
            }
        }
        if (配置.进度同步星旋前)
        {
            if (data.TryGetValue(nameof(NPC.downedTowerVortex), out var v) && v != NPC.downedTowerVortex)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]星旋前进度同步为{v}");
                NPC.downedTowerVortex = v;
            }
        }
        if (配置.进度同步星尘前)
        {
            if (data.TryGetValue(nameof(NPC.downedTowerStardust), out var v) && v != NPC.downedTowerStardust)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]星尘前进度同步为{v}");
                NPC.downedTowerStardust = v;
            }
        }
        if (配置.进度同步星云前)
        {
            if (data.TryGetValue(nameof(NPC.downedTowerNebula), out var v) && v != NPC.downedTowerNebula)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]星云前进度同步为{v}");
                NPC.downedTowerNebula = v;
            }
        }
        if (配置.进度同步月总)
        {
            if (data.TryGetValue(nameof(NPC.downedMoonlord), out var v) && v != NPC.downedMoonlord)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]月总进度同步为{v}");
                NPC.downedMoonlord = v;
            }
        }

        Monitor.Exit(kg);
    }

    public static object kg = new object();
}