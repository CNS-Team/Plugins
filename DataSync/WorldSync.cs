using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace WorldSync
{
    // Token: 0x02000002 RID: 2
    [ApiVersion(2, 1)]
    public class WorldSync : TerrariaPlugin
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override string Name => "WorldSync";

        // Token: 0x06000002 RID: 2 RVA: 0x00002067 File Offset: 0x00000267
        public WorldSync(Main game) : base(game)
        {
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002074 File Offset: 0x00000274
        public override void Initialize()
        {
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", "hardMode"))
            {
                if (!表.Read())
                {
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "hardMode", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedSlimeKing", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedDeerclops", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedClown", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedPlantBoss", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedGolemBoss", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMartians", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedFishron", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedHalloweenTree", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedHalloweenKing", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedChristmasIceQueen", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedChristmasTree", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedChristmasSantank", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedAncientCultist", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMoonlord", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedPirates", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedFrost", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedQueenBee", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedBoss1", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedBoss2", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedBoss3", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerSolar", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerVortex", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerStardust", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerNebula", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBoss1", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBoss2", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBoss3", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedEmpressOfLight", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedQueenSlime", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBossAny", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedGoblins", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedStylist", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedBartender", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedMech", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedGolfer", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedAngler", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedWizard", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedTaxCollector", "false");
                }
            }
            using (var 表2 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", "哥布林一进行"))
            {
                if (!表2.Read())
                {
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "哥布林一进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "旧日军团一", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "哥布林二进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "海盗进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "日蚀一进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "旧日军团二", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "日蚀二进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "日蚀三进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "旧日军团三", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "火星进行", "false");
                }
            }
        }
        public static void Remove()
        {
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", "hardMode"))
            {
                if (表.Read())
                {
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "hardMode", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedSlimeKing", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedDeerclops", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedClown", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedPlantBoss", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedGolemBoss", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMartians", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedFishron", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedHalloweenTree", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedHalloweenKing", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedChristmasIceQueen", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedChristmasTree", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedChristmasSantank", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedAncientCultist", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMoonlord", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedPirates", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedFrost", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedQueenBee", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedBoss1", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedBoss2", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedBoss3", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerSolar", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerVortex", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerStardust", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedTowerNebula", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBoss1", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBoss2", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBoss3", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedEmpressOfLight", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedQueenSlime", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedMechBossAny", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "downedGoblins", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedStylist", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedBartender", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedMech", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedGolfer", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedAngler", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedWizard", "false");
                    //TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "savedTaxCollector", "false");
                }
            }
            using (var 表2 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", "哥布林一进行"))
            {
                if (表2.Read())
                {
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "哥布林一进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "旧日军团一", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "哥布林二进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "海盗进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "日蚀一进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "旧日军团二", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "日蚀二进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "日蚀三进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "旧日军团三", "false");
                    TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES " + "(@0, @1);", "火星进行", "false");
                }
            }
        }
    }
}