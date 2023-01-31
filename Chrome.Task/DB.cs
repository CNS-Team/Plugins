using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.DB;

namespace 任务系统
{
    public static class DB
    {
        public static string tableName = "任务系统";
        public static string RPGtableName = "Chrome_RPG";
        /// <summary>
        /// 创建表
        /// </summary>
        public static void Connect()
        {
            var sqlcreator = new SqlTableCreator(TShock.DB, TShock.DB.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());

            sqlcreator.EnsureTableStructure(new SqlTable(tableName,
                new SqlColumn("玩家名", MySqlDbType.VarChar, new int?(256)) { Primary = true },
                new SqlColumn("主线任务", MySqlDbType.Int32) { Length = 255 },
                new SqlColumn("分支任务", MySqlDbType.Text) { Length = 500 },
                new SqlColumn("支线任务", MySqlDbType.Text) { Length = 500 },
                new SqlColumn("可接取任务", MySqlDbType.Text) { Length = 500 },
                new SqlColumn("已完成任务", MySqlDbType.Text) { Length = 500 },
                new SqlColumn("任务目标", MySqlDbType.Text) { Length = 500 },
                new SqlColumn("任务目标2", MySqlDbType.Int32) { Length = 255 }));
        }
        /// <summary>
        /// 查当前主线任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>
        public static int QueryNowTask(string 玩家名)
        {
            var 任务ID = 0;
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    任务ID = 表.Get<int>("主线任务");
                }
            }
            return 任务ID;
        }
        /// <summary>
        /// 查当前分支任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>
        public static List<int> QueryBranchTask(string 玩家名)
        {
            var 分支任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    分支任务 = 表.Get<string>("分支任务");
                }
            }
            var 任务 = 分支任务.Split(",");
            List<int> list = new();
            foreach (var z in 任务)
            {
                if (int.TryParse(z, out var id))
                {
                    list.Add(id);
                }
            }
            return list;
        }
        /// <summary>
        /// 查当前支线任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>
        public static List<int> QueryNowSideTask(string 玩家名)
        {
            var 支线任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    支线任务 = 表.Get<string>("支线任务");
                }
            }
            var 任务 = 支线任务.Split(",");
            List<int> list = new();
            foreach (var z in 任务)
            {
                if (int.TryParse(z, out var id))
                {
                    list.Add(id);
                }
            }
            return list;
        }
        /// <summary>
        /// 查可接取任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>
        public static List<int> QueryGetTask(string 玩家名)
        {
            var 可接取任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    可接取任务 = 表.Get<string>("可接取任务");
                }
            }
            var 任务 = 可接取任务.Split(",");
            List<int> list = new();
            foreach (var z in 任务)
            {
                if (int.TryParse(z, out var id))
                {
                    list.Add(id);
                }
            }
            return list;
        }
        /// <summary>
        /// 查已完成任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>
        public static List<int> QueryFinishTask(string 玩家名)
        {
            var 已完成任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    已完成任务 = 表.Get<string>("已完成任务");
                }
            }
            var 任务 = 已完成任务.Split(",");
            List<int> list = new();
            foreach (var z in 任务)
            {
                if (int.TryParse(z, out var id))
                {
                    list.Add(id);
                }
            }
            return list;
        }
        /// <summary>
        /// 查任务目标
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>
        public static Dictionary<int, int> QueryTaskTarget(string 玩家名)
        {
            var 任务目标 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    任务目标 = 表.Get<string>("任务目标");
                }
            }
            var j = JsonConvert.DeserializeObject<Dictionary<int, int>>(任务目标);
            return j ?? new() { };
        }
        /// <summary>
        /// 查任务目标2
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>
        public static int QueryTaskTarget2(string 玩家名)
        {
            var 任务目标 = 0;
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    任务目标 = 表.Get<int>("任务目标2");
                }
            }
            return 任务目标;
        }
        /// <summary>
        /// 表中是否存在玩家
        /// </summary>
        public static bool IsExist(string 玩家名)
        {
            using var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名);
            return 表.Read();
        }
        /// <summary>
        /// 完成主线任务
        /// </summary>
        public static void CompleteTask(TSPlayer plr, int 任务ID)
        {
            var 玩家名 = plr.Name;
            List<int> 主线 = new() { };
            var 支线 = false;
            var 任务 = 任务系统.配置.任务.Find(s => s.任务ID == 任务ID);
            AddCompleteTask(玩家名, 任务ID);
            foreach (var z in 任务.解锁任务)
            {
                var 下一任务 = 任务系统.配置.任务.Find(s => s.任务ID == z);
                if (下一任务.是否主线)
                {
                    AddBranchTask(玩家名, z);
                    主线.Add(z);
                }
                else
                {
                    var r = 任务系统.GetTask(任务ID);
                    if (r.职业名限制.Count != 0 && !r.职业名限制.Contains(QueryRPGGrade(玩家名)))
                    {
                        return;
                    }
                    if (!支线)
                    {
                        plr.SendSuccessMessage("[c/12FC41:您有新的支线任务，输入\"/查询任务 支线 \"查看]");
                    }

                    支线 = true;
                    AddCanGetTask(玩家名, z);
                }
            }
            if (主线.Count == 0)
            {
                DelBranchTask(玩家名);
                UpadatMainSide(玩家名, 0);
                plr.SendSuccessMessage("您已完成所有主线任务");
            }
            if (主线.Count == 1)
            {
                DelBranchTask(玩家名);
                UpadatMainSide(玩家名, 主线[0]);
                plr.SendSuccessMessage("[c/12FC41:你有新的主线任务，输入\"/查询任务 主线 \"查看]");
            }
            if (主线.Count > 1)
            {
                UpadatMainSide(玩家名, 0);
                plr.SendSuccessMessage("[c/9712FC:你需要选择自己的下个主线任务了，输入\"/接受任务\"查看]");
            }
            TShock.Log.Info($"{玩家名} 完成主线任务 {任务ID}");
        }

        /// <summary>
        /// 完成支线任务
        /// </summary>
        public static void CompleteSideTask(string 玩家名, int 任务ID)
        {
            var plr = TSPlayer.FindByNameOrID(玩家名)[0];
            var 任务 = 任务系统.配置.任务.Find(s => s.任务ID == 任务ID);
            var 支线 = false;
            AddCompleteTask(玩家名, 任务ID);
            DelSideTask(玩家名, 任务ID);
            foreach (var z in 任务.解锁任务)
            {
                var 下一任务 = 任务系统.配置.任务.Find(s => s.任务ID == z);
                if (下一任务.是否主线)
                {
                    // AddBranchTask(玩家名, z);
                    // plr.SendSuccessMessage("您有新的主线任务，输入\"/查询任务 主线 \"查看");
                }
                else
                {
                    var r = 任务系统.GetTask(任务ID);
                    if (r.职业名限制.Count != 0 && !r.职业名限制.Contains(QueryRPGGrade(玩家名)))
                    {
                        return;
                    }
                    if (!支线)
                    {
                        plr.SendSuccessMessage("[c/12FC41:您有新的支线任务，输入\"/查询任务 支线 \"查看]");
                    }

                    支线 = true;
                    AddCanGetTask(玩家名, z);
                }
            }
            TShock.Log.Info($"{玩家名} 完成支线任务 {任务ID}");
        }
        /// <summary>
        /// 查职业
        /// </summary>
        /// <param name="玩家名"></param>
        /// <returns></returns>

        public static string QueryRPGGrade(string 玩家名)
        {
            var 职业 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + RPGtableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    职业 = 表.Get<string>("职业");
                }
            }
            return 职业;
        }
        /// <summary>
        /// 修改主线任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务ID"></param>
        public static void UpadatMainSide(string 玩家名, int 任务ID)
        {
            TShock.DB.Query("UPDATE " + tableName + " SET `主线任务` = @0 WHERE `玩家名` = @1", 任务ID, 玩家名);
        }
        /// <summary>
        /// 添加已完成任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务ID"></param>
        public static void AddCompleteTask(string 玩家名, int 任务ID)
        {
            var 已完成任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    已完成任务 = 表.Get<string>("已完成任务");
                }
            }
            TShock.DB.Query("UPDATE " + tableName + " SET `已完成任务` = @0 WHERE `玩家名` = @1", 已完成任务 + "," + 任务ID.ToString(), 玩家名);
        }
        /// <summary>
        /// 添加当前分支任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务ID"></param>
        public static void AddBranchTask(string 玩家名, int 任务ID)
        {
            var 分支任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    分支任务 = 表.Get<string>("分支任务");
                }
            }
            TShock.DB.Query("UPDATE " + tableName + " SET `分支任务` = @0 WHERE `玩家名` = @1", 分支任务 + "," + 任务ID.ToString(), 玩家名);
        }
        /// <summary>
        /// 添加当前支线任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务ID"></param>
        public static void AddSideTask(string 玩家名, int 任务ID)
        {
            var 支线任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    支线任务 = 表.Get<string>("支线任务");
                }
            }
            TShock.DB.Query("UPDATE " + tableName + " SET `支线任务` = @0 WHERE `玩家名` = @1", 支线任务 + "," + 任务ID.ToString(), 玩家名);
        }
        /// <summary>
        /// 添加可接取任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务ID"></param>
        public static void AddCanGetTask(string 玩家名, int 任务ID)
        {
            var 可接取任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    可接取任务 = 表.Get<string>("可接取任务");
                }
            }
            TShock.DB.Query("UPDATE " + tableName + " SET `可接取任务` = @0 WHERE `玩家名` = @1", 可接取任务 + "," + 任务ID.ToString(), 玩家名);
        }
        /// <summary>
        /// 任务目标+1
        /// </summary>
        public static void AddTaskAim(string 玩家名, int npcID)
        {
            var t = QueryTaskTarget(玩家名);
            if (!t.ContainsKey(npcID))
            {
                t.Add(npcID, 0);
            }
            t[npcID]++;
            var 任务目标 = JsonConvert.SerializeObject(t, Formatting.Indented);
            TShock.DB.Query("UPDATE " + tableName + " SET `任务目标` = @0 WHERE `玩家名` = @1", 任务目标, 玩家名);
        }
        /// <summary>
        /// 删除已完成支线任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务ID"></param>
        public static void DelSideTask(string 玩家名, int 任务ID)
        {
            var 支线任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    支线任务 = 表.Get<string>("支线任务");
                }
            }
            var z = 支线任务.Replace("," + 任务ID, "");
            TShock.DB.Query("UPDATE " + tableName + " SET `支线任务` = @0 WHERE `玩家名` = @1", z, 玩家名);
        }
        /// <summary>
        /// 删除可接取任务
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务ID"></param>
        public static void DelCanGetTask(string 玩家名, int 任务ID)
        {
            var 可接取任务 = "";
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + tableName + " WHERE `玩家名` = @0", 玩家名))
            {
                if (表.Read())
                {
                    可接取任务 = 表.Get<string>("可接取任务");
                }
            }
            var z = 可接取任务.Replace("," + 任务ID, "");
            TShock.DB.Query("UPDATE " + tableName + " SET `可接取任务` = @0 WHERE `玩家名` = @1", z, 玩家名);
        }
        /// <summary>
        /// 清除分支任务
        /// </summary>
        /// <param name="玩家名"></param>
        public static void DelBranchTask(string 玩家名)
        {
            TShock.DB.Query("UPDATE " + tableName + " SET `分支任务` = @0 WHERE `玩家名` = @1", "", 玩家名);
        }
        /// <summary>
        /// 修改任务目标2
        /// </summary>
        /// <param name="玩家名"></param>
        /// <param name="任务目标"></param>
        public static void UpdataTaskTarget2(string 玩家名, int 任务目标)
        {
            TShock.DB.Query("UPDATE " + tableName + " SET `任务目标2` = @0 WHERE `玩家名` = @1", 任务目标, 玩家名);
        }
    }
}