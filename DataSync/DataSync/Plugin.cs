using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace DataSync
{
    // Token: 0x02000004 RID: 4
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000006 RID: 6 RVA: 0x0000208E File Offset: 0x0000028E
        public override string Name
        {
            get
            {
                return "DataSync";
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002095 File Offset: 0x00000295
        public Plugin(Main game) : base(game)
        {
        }
        public static Config 配置 = new();
        // Token: 0x06000008 RID: 8 RVA: 0x000020A0 File Offset: 0x000002A0
        /*
        public static void AddStaticField<T>(Type type, string field, string key = null, int interval = 600) where T : IEquatable<T>
        {

            FieldInfo fieldinfo = type.GetField(field, (BindingFlags)56);
            Plugin.syncables.Add(key ?? field, new LegacySyncable<T>
            {
                SetValue = delegate (T val)
                {
                    fieldinfo.SetValue(null, val);
                },
                GetValue = (() => (T)((object)fieldinfo.GetValue(null))),
                Interval = interval
            });
        }
        */
        /*
        public static void AddNewStaticField()
        {
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", "哥布林一进行"))
            {
                if (!表.Read())
                {
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "哥布林一进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "旧日军团一", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "哥布林二进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "海盗进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "日蚀一进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "旧日军团二", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "日蚀二进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "日蚀三进行", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "旧日军团三", "false");
                    TShock.DB.Query("INSERT INTO synctable (key, value) VALUES " + "(@0, @1);", "火星进行", "false");

                }
            }

            
            Plugin.AddStaticField<bool>(typeof(Main), "哥布林一进行", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "旧日军团一", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "哥布林二进行", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "海盗进行", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "日蚀一进行", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "旧日军团二", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "日蚀二进行", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "日蚀三进行", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "旧日军团三", null, 600);
            Plugin.AddStaticField<bool>(typeof(Main), "火星进行", null, 600);
            

        }
    */
        // Token: 0x06000009 RID: 9 RVA: 0x00002104 File Offset: 0x00000304
        private static void EnsureTable()
        {
            IDbConnection db = TShock.DB;
            SqlTable sqlTable = new SqlTable("synctable", new SqlColumn[]
            {
                new SqlColumn("key", MySqlDbType.VarChar)
                {
                    Primary = true,
                    Length = new int?(256)
                },
                new SqlColumn("value", MySqlDbType.VarChar)
                {
                    Length = new int?(256)
                }
            });
            bool flag = (int)DbExt.GetSqlType(db) != 1;
            IQueryBuilder queryBuilder2;
            if (flag)
            {
                IQueryBuilder queryBuilder = new MysqlQueryCreator();
                queryBuilder2 = queryBuilder;
            }
            else
            {
                IQueryBuilder queryBuilder3 = new SqliteQueryCreator();
                queryBuilder2 = queryBuilder3;
            }
            SqlTableCreator sqlTableCreator = new SqlTableCreator(db, queryBuilder2);
            sqlTableCreator.EnsureTableStructure(sqlTable);
        }
        // Token: 0x0600000A RID: 10 RVA: 0x000021B3 File Offset: 0x000003B3
        public override void Initialize()
        {
            Plugin.EnsureTable();
            //AddNewStaticField();
            //ServerApi.Hooks.GamePostUpdate.Register(this, p0);
            ServerApi.Hooks.NpcKilled.Register(this, new HookHandler<NpcKilledEventArgs>(this.NpcKilled));
            Commands.ChatCommands.Add(new Command("DataSync", Re, "reload") { });
            Commands.ChatCommands.Add(new Command("DataSync", Remove, "重置进度同步") { });
            Config.GetConfig();
            Reload();
        }

        public static bool GetJb(string key)
        {
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", key))
            {
                if (表.Read())
                {

                    return 表.Get<bool>("value");
                }
                else
                {
                    return false;
                }
            }
        }
        public static void RJb(string key, string value)
        {
            TShock.DB.Query("UPDATE synctable SET value = @1 WHERE `key` = @0", key, value);
        }
        private void NpcKilled(NpcKilledEventArgs args)
        {
            bool flag = !GetJb("哥布林一进行") && (args.npc.netID == 26 || args.npc.netID == 27 || args.npc.netID == 28 || args.npc.netID == 29 || args.npc.netID == 111);
            if (flag)
            {
                RJb("哥布林一进行", "true");
                p1();
                return;
            }
            bool flag2 = !GetJb("旧日军团一") && (args.npc.netID == 564 || args.npc.netID == 565);
            if (flag2)
            {
                RJb("旧日军团一", "true");
                p1();
                return;
            }
            bool flag3 = !GetJb("哥布林二进行") && args.npc.netID == 471;
            if (flag3)
            {
                RJb("哥布林二进行", "true");
                p1(); return;
            }
            bool flag4 = !GetJb("海盗进行") && (args.npc.netID == 212 || args.npc.netID == 213 || args.npc.netID == 214 || args.npc.netID == 215 || args.npc.netID == 216 || args.npc.netID == 491);
            if (flag4)
            {
                RJb("海盗进行", "true");
                p1(); return;
            }
            bool flag5 = !GetJb("日蚀一进行") && (args.npc.netID == 158 || args.npc.netID == 159 || args.npc.netID == 461);
            if (flag5)
            {
                RJb("日蚀一进行", "true");
                p1(); return;
            }
            bool flag6 = !GetJb("旧日军团二") && (args.npc.netID == 576 || args.npc.netID == 577);
            if (flag6)
            {
                RJb("旧日军团二", "true");
                p1();
                return;
            }
            bool flag7 = !GetJb("日蚀二进行") && (args.npc.netID == 253 || args.npc.netID == 477);
            if (flag7)
            {
                RJb("日蚀二进行", "true");
                p1();
                return;
            }
            bool flag8 = !GetJb("日蚀三进行") && (args.npc.netID == 460 || args.npc.netID == 463 || args.npc.netID == 466 || args.npc.netID == 467 || args.npc.netID == 468);
            if (flag8)
            {
                RJb("日蚀三进行", "true");
                p1();
                return;
            }
            bool flag9 = !GetJb("旧日军团三") && args.npc.netID == 551;
            if (flag9)
            {
                RJb("旧日军团三", "true");
                p1();
                return;
            }
            bool flag10 = !GetJb("火星进行") && (args.npc.netID == 381 || args.npc.netID == 382 || args.npc.netID == 383 || args.npc.netID == 384 || args.npc.netID == 385 || args.npc.netID == 386 || args.npc.netID == 389 || args.npc.netID == 390 || args.npc.netID == 392);
            if (flag10)
            {
                RJb("火星进行", "true");
                p1();
                return;
            }
            if (args.npc.netID == 50 || args.npc.netID == 3331 || args.npc.netID == 222 || args.npc.netID == 327 || args.npc.netID == 345 || args.npc.netID == 636 || args.npc.netID == 134 || args.npc.netID == 135 || args.npc.netID == 136 || args.npc.netID == 127 || args.npc.netID == 266 || args.npc.netID == 4 || args.npc.netID == 35 || args.npc.netID == 36 || args.npc.netID == 246 || args.npc.netID == 245 || args.npc.netID == 657 || args.npc.netID == 262 || args.npc.netID == 14 || args.npc.netID == 15 || args.npc.netID == 125 || args.npc.netID == 126 || args.npc.netID == 113 || args.npc.netID == 396 || args.npc.netID == 397 || args.npc.netID == 370 || args.npc.netID == 422 || args.npc.netID == 493 || args.npc.netID == 507 || args.npc.netID == 517)
            {
                if (配置.进度同步哥布林 && NPC.downedGoblins == true)
                    RJb("downedGoblins", "true");

                if (配置.进度同步萌王 || NPC.downedSlimeKing == true)
                    RJb("downedSlimeKing", "true");

                if (配置.进度同步鹿角怪 || NPC.downedDeerclops == true)
                    RJb("downedDeerclops", "true");

                if (配置.进度同步克眼 || NPC.downedBoss1 == true)
                    RJb("downedBoss1", "true");

                if (配置.进度同步虫脑 || NPC.downedBoss2 == true)
                    RJb("downedBoss2", "true");

                if (配置.进度同步蜂后 || NPC.downedQueenBee == true)
                    RJb("downedQueenBee", "true");

                if (配置.进度同步骷髅 || NPC.downedBoss3 == true)
                    RJb("downedBoss3", "true");

                if (配置.进度同步肉墙 || Main.hardMode == true)
                    RJb("hardMode", "true");

                if (配置.进度同步海盗 || NPC.downedPirates == true)
                    RJb("downedPirates", "true");

                if (配置.进度同步萌后 || NPC.downedQueenSlime == true)
                    RJb("downedQueenSlime", "true");

                if (配置.进度同步任一三王 || NPC.downedMechBossAny == true)
                    RJb("downedMechBossAny", "true");

                if (配置.进度同步机械眼 || NPC.downedMechBoss2 == true)
                    RJb("downedMechBoss2", "true");

                if (配置.进度同步机械虫 || NPC.downedMechBoss1 == true)
                    RJb("downedMechBoss1", "true");

                if (配置.进度同步机械骷髅 || NPC.downedMechBoss3 == true)
                    RJb("downedMechBoss3", "true");

                if (配置.进度同步猪鲨前 || NPC.downedFishron == true)
                    RJb("downedFishron", "true");

                if (配置.进度同步妖花前 || NPC.downedPlantBoss == true)
                    RJb("downedPlantBoss", "true");

                if (配置.进度同步霜月树  || NPC.downedChristmasTree == true)
                    RJb("downedChristmasTree", "true");

                if (配置.进度同步霜月坦 || NPC.downedChristmasSantank == true)
                    RJb("downedChristmasSantank", "true");

                if (配置.进度同步霜月后 || NPC.downedChristmasIceQueen == true)
                    RJb("downedChristmasIceQueen", "true");

                if (配置.进度同步南瓜树 || NPC.downedHalloweenTree == true)
                    RJb("downedHalloweenTree", "true");

                if (配置.进度同步南瓜王 || NPC.downedHalloweenKing == true)
                    RJb("downedHalloweenKing", "true");

                if (配置.进度同步光女前 || NPC.downedEmpressOfLight == true)
                    RJb("downedEmpressOfLight", "true");

                if (配置.进度同步石巨人 || NPC.downedGolemBoss == true)
                    RJb("downedGolemBoss", "true");

                if (配置.进度同步教徒前 || NPC.downedAncientCultist == true)
                    RJb("downedAncientCultist", "true");

                if (配置.进度同步日耀前 || NPC.downedTowerSolar == true)
                    RJb("downedTowerSolar", "true");

                if (配置.进度同步星旋前 || NPC.downedTowerVortex == true)
                    RJb("downedTowerVortex", "true");

                if (配置.进度同步星尘前 || NPC.downedTowerStardust == true)
                    RJb("downedTowerStardust", "true");

                if (配置.进度同步星云前 || NPC.downedTowerNebula == true)
                    RJb("downedTowerNebula", "true");

                if (配置.进度同步月总 || NPC.downedMoonlord == true)
                    RJb("downedMoonlord", "true");
                p1();
            }
        }
        private void Remove(CommandArgs args)
        {
            WorldSync.WorldSync.Remove();
        }
        private void Re(CommandArgs args)
        {
            try
            {
                Reload();
                EnsureTable();
                kg = true;
                p1();
                //args.Player.SendErrorMessage($"[QwRPG.Shop]重载成功！");
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[DataSync]配置文件读取错误");
            }
        }
        public static void Reload()
        {
            try
            {
                配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/DataSync.json")));
                File.WriteAllText("tshock/DataSync.json", JsonConvert.SerializeObject(配置, Formatting.Indented));
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[DataSync]配置文件读取错误");
            }
        }
        public static Dictionary<string, bool> GetJbs()
        {
            Dictionary<string, bool> list = new() { };
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM synctable;"))
            {
                while (表.Read())
                {
                    string key = 表.Get<string>("key");
                    bool value = 表.Get<bool>("value");
                    list.Add(key, value);
                }
                return list;
            }
        }
        public static void p1()
        {
            if (!配置.是否同步) { return; }
            if (!kg) { return; }
            kg = false;
            foreach (KeyValuePair<string, bool> keyValuePair in GetJbs())
            {

                if (keyValuePair.Key == "哥布林一进行" || keyValuePair.Key == "旧日军团一" || keyValuePair.Key == "哥布林二进行" || keyValuePair.Key == "海盗进行" || keyValuePair.Key == "日蚀一进行"
                    || keyValuePair.Key == "旧日军团二" || keyValuePair.Key == "日蚀二进行" || keyValuePair.Key == "日蚀三进行" || keyValuePair.Key == "旧日军团三" || keyValuePair.Key == "火星进行")
                {
                    continue;
                }
                if (配置.进度同步哥布林 || keyValuePair.Key == "downedGoblins")
                {
                    if (NPC.downedGoblins != keyValuePair.Value)
                        NPC.downedGoblins = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步萌王 || keyValuePair.Key == "downedSlimeKing")
                {
                    if (NPC.downedSlimeKing != keyValuePair.Value)
                        NPC.downedSlimeKing = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步鹿角怪 || keyValuePair.Key == "downedDeerclops")
                {
                    if (NPC.downedDeerclops != keyValuePair.Value)
                        NPC.downedDeerclops = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步克眼 || keyValuePair.Key == "downedBoss1")
                {
                    if (NPC.downedBoss1 != keyValuePair.Value)
                        NPC.downedBoss1 = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步虫脑 || keyValuePair.Key == "downedBoss2")
                {
                    if (NPC.downedBoss2 != keyValuePair.Value)
                        NPC.downedBoss2 = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步蜂后 || keyValuePair.Key == "downedQueenBee")
                {
                    if (NPC.downedQueenBee != keyValuePair.Value)
                        NPC.downedQueenBee = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步骷髅 || keyValuePair.Key == "downedBoss3")
                {
                    if (NPC.downedBoss3 != keyValuePair.Value)
                        NPC.downedBoss3 = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步肉墙 || keyValuePair.Key == "hardMode")
                {
                    if (Main.hardMode != keyValuePair.Value)
                        Main.hardMode = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步海盗 || keyValuePair.Key == "downedPirates")
                {
                    if (NPC.downedPirates != keyValuePair.Value)
                        NPC.downedPirates = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步萌后 || keyValuePair.Key == "downedQueenSlime")
                {
                    if (NPC.downedQueenSlime != keyValuePair.Value)
                        NPC.downedQueenSlime = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步任一三王 || keyValuePair.Key == "downedMechBossAny")
                {
                    if (NPC.downedMechBossAny != keyValuePair.Value)
                        NPC.downedMechBossAny = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步机械眼 || keyValuePair.Key == "downedMechBoss2")
                {
                    if (NPC.downedMechBoss2 != keyValuePair.Value)
                        NPC.downedMechBoss2 = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步机械虫 || keyValuePair.Key == "downedMechBoss1")
                {
                    if (NPC.downedMechBoss1 != keyValuePair.Value)
                        NPC.downedMechBoss1 = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步机械骷髅 || keyValuePair.Key == "downedMechBoss3")
                {
                    if (NPC.downedMechBoss3 != keyValuePair.Value)
                        NPC.downedMechBoss3 = keyValuePair.Value;
                    continue;
                }

                if (配置.进度同步猪鲨前 || keyValuePair.Key == "downedFishron")
                {
                    if (NPC.downedFishron != keyValuePair.Value)
                        NPC.downedFishron = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步妖花前 || keyValuePair.Key == "downedPlantBoss")
                {
                    if (NPC.downedPlantBoss != keyValuePair.Value)
                        NPC.downedPlantBoss = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步霜月树 || keyValuePair.Key == "downedChristmasTree")
                {
                    if (NPC.downedChristmasTree != keyValuePair.Value)
                        NPC.downedChristmasTree = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步霜月坦 || keyValuePair.Key == "downedChristmasSantank")
                {
                    if (NPC.downedChristmasSantank != keyValuePair.Value)
                        NPC.downedChristmasSantank = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步霜月后 || keyValuePair.Key == "downedChristmasIceQueen")
                {
                    if (NPC.downedChristmasIceQueen != keyValuePair.Value)
                        NPC.downedChristmasIceQueen = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步南瓜树 || keyValuePair.Key == "downedHalloweenTree")
                {
                    if (NPC.downedHalloweenTree != keyValuePair.Value)
                        NPC.downedHalloweenTree = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步南瓜王 || keyValuePair.Key == "downedHalloweenKing")
                {
                    if (NPC.downedHalloweenKing != keyValuePair.Value)
                        NPC.downedHalloweenKing = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步光女前 || keyValuePair.Key == "downedEmpressOfLight")
                {
                    if (NPC.downedEmpressOfLight != keyValuePair.Value)
                        NPC.downedEmpressOfLight = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步石巨人 || keyValuePair.Key == "downedGolemBoss")
                {
                    if (NPC.downedGolemBoss != keyValuePair.Value)
                        NPC.downedGolemBoss = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步教徒前 || keyValuePair.Key == "downedAncientCultist")
                {
                    if (NPC.downedAncientCultist != keyValuePair.Value)
                        NPC.downedAncientCultist = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步日耀前 || keyValuePair.Key == "downedTowerSolar")
                {
                    if (NPC.downedTowerSolar != keyValuePair.Value)
                        NPC.downedTowerSolar = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步星旋前 || keyValuePair.Key == "downedTowerVortex")
                {
                    if (NPC.downedTowerVortex != keyValuePair.Value)
                        NPC.downedTowerVortex = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步星尘前 || keyValuePair.Key == "downedTowerStardust")
                {
                    if (NPC.downedTowerStardust != keyValuePair.Value)
                        NPC.downedTowerStardust = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步星云前 || keyValuePair.Key == "downedTowerNebula")
                {
                    if (NPC.downedTowerNebula != keyValuePair.Value)
                        NPC.downedTowerNebula = keyValuePair.Value;
                    continue;
                }
                if (配置.进度同步月总 || keyValuePair.Key == "downedMoonlord")
                {
                    if (NPC.downedMoonlord != keyValuePair.Value)
                        NPC.downedMoonlord = keyValuePair.Value;
                    continue;
                }
                /*
                if (!(配置.进度同步哥布林 || keyValuePair.Key != "downedGoblins"))
                {
                    continue;
                }
                if (!(配置.进度同步萌王 || keyValuePair.Key != "downedSlimeKing"))
                {
                    continue;
                }
                if (!(配置.进度同步鹿角怪 || keyValuePair.Key != "downedDeerclops"))
                {
                    continue;
                }
                if (!(配置.进度同步克眼 || keyValuePair.Key != "downedBoss1"))
                {
                    continue;
                }
                if (!(配置.进度同步虫脑 || keyValuePair.Key != "downedBoss2"))
                {
                    continue;
                }
                if (!(配置.进度同步蜂后 || keyValuePair.Key != "downedQueenBee"))
                {
                    continue;
                }
                if (!(配置.进度同步骷髅 || keyValuePair.Key != "downedBoss3"))
                {
                    continue;
                }
                if (!(配置.进度同步肉墙 || keyValuePair.Key != "hardMode"))
                {
                    continue;
                }
                if (!(配置.进度同步海盗 || keyValuePair.Key != "downedPirates"))
                {
                    continue;
                }
                if (!(配置.进度同步萌后 || keyValuePair.Key != "downedQueenSlime"))
                {
                    continue;
                }
                if (!(配置.进度同步任一三王 || keyValuePair.Key != "downedMechBossAny"))
                {
                    continue;
                }
                if (!(配置.进度同步机械眼 || keyValuePair.Key != "downedMechBoss2"))
                {
                    continue;
                }
                if (!(配置.进度同步机械虫 || keyValuePair.Key != "downedMechBoss1"))
                {
                    continue;
                }
                if (!(配置.进度同步机械骷髅 || keyValuePair.Key != "downedMechBoss3"))
                {
                    continue;
                }

                if (!(配置.进度同步猪鲨前 || keyValuePair.Key != "downedFishron"))
                {
                    continue;
                }
                if (!(配置.进度同步妖花前 || keyValuePair.Key != "downedPlantBoss"))
                {
                    continue;
                }
                if (!(配置.进度同步霜月树 || keyValuePair.Key != "downedChristmasTree"))
                {
                    continue;
                }
                if (!(配置.进度同步霜月坦 || keyValuePair.Key != "downedChristmasSantank"))
                {
                    continue;
                }
                if (!(配置.进度同步霜月后 || keyValuePair.Key != "downedChristmasIceQueen"))
                {
                    continue;
                }
                if (!(配置.进度同步南瓜树 || keyValuePair.Key != "downedHalloweenTree"))
                {
                    continue;
                }
                if (!(配置.进度同步南瓜王 || keyValuePair.Key != "downedHalloweenKing"))
                {
                    continue;
                }
                if (!(配置.进度同步光女前 || keyValuePair.Key != "downedEmpressOfLight"))
                {
                    continue;
                }
                if (!(配置.进度同步石巨人 || keyValuePair.Key != "downedGolemBoss"))
                {
                    continue;
                }
                if (!(配置.进度同步教徒前 || keyValuePair.Key != "downedAncientCultist"))
                {
                    continue;
                }
                if (!(配置.进度同步日耀前 || keyValuePair.Key != "downedTowerSolar"))
                {
                    continue;
                }
                if (!(配置.进度同步星旋前 || keyValuePair.Key != "downedTowerVortex"))
                {
                    continue;
                }
                if (!(配置.进度同步星尘前 || keyValuePair.Key != "downedTowerStardust"))
                {
                    continue;
                }
                if (!(配置.进度同步星云前 || keyValuePair.Key != "downedTowerNebula"))
                {
                    continue;
                }
                if (!(配置.进度同步月总 || keyValuePair.Key != "downedMoonlord"))
                {
                    continue;
                }
                keyValuePair.Value.CheckSync(keyValuePair.Key);
            */
            }
            kg = true;
        }
        // Token: 0x04000004 RID: 4
        public static bool kg = true;
    }
}
