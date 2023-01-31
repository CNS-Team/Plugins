using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;

namespace Chrome.RPG;

public static class DB
{
    /// <summary>
    /// 生成数据表
    /// </summary>
    public static void Reload()
    {
        var table = new SqlTable("Chrome_RPG", new SqlColumn[]
        {
            new SqlColumn("玩家名", MySqlDbType.VarChar, new int?(256))
            {
                Primary = true
            },
            new SqlColumn("点券", MySqlDbType.Int64),
            new SqlColumn("职业", MySqlDbType.Text),
            new SqlColumn("是否显示", MySqlDbType.Text),
            new SqlColumn("职业重置次数", MySqlDbType.Int64),
        });
        var db = TShock.DB;
        IQueryBuilder provider;
        if (TShock.DB.GetSqlType() != SqlType.Sqlite)
        {
            IQueryBuilder queryBuilder = new MysqlQueryCreator();
            provider = queryBuilder;
        }
        else
        {
            IQueryBuilder queryBuilder = new SqliteQueryCreator();
            provider = queryBuilder;
        }
        var sqlTableCreator = new SqlTableCreator(db, provider);
        sqlTableCreator.EnsureTableStructure(table);
    }
    /// <summary>
    /// +货币
    /// </summary>
    public static bool AddCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = `点券` + @0 WHERE `玩家名` = @1", 点券, 玩家名);
        var plr = TSPlayer.FindByNameOrID(玩家名);
        if (是否头顶显示 && plr.Count != 0 && ChromePlugin.配置.是否头顶显示货币变化)
        {
            var rd = new Random();
            var color = rd.Next(1, 10);
            switch (color)
            {
                case 1:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.AntiqueWhite, plr[0].TPlayer.position);
                    break;
                case 2:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Black, plr[0].TPlayer.position);
                    break;
                case 3:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Red, plr[0].TPlayer.position);
                    break;
                case 4:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Green, plr[0].TPlayer.position);
                    break;
                case 5:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Orange, plr[0].TPlayer.position);
                    break;
                case 6:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Blue, plr[0].TPlayer.position);
                    break;
                case 7:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Yellow, plr[0].TPlayer.position);
                    break;
                case 8:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Brown, plr[0].TPlayer.position);
                    break;
                case 9:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.Purple, plr[0].TPlayer.position);
                    break;
                default:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币增加时头顶显示, 点券), Color.AliceBlue, plr[0].TPlayer.position);
                    break;
            }
        }
        //Chrome.Status(plr[0]);
        TShock.Log.Info($"{玩家名} 货币 + {点券}");
        return true;
    }
    /// <summary>
    /// -货币
    /// </summary>
    public static bool DelCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = `点券` - @0 WHERE `玩家名` = @1", 点券, 玩家名);
        var plr = TSPlayer.FindByNameOrID(玩家名);
        if (是否头顶显示 && plr.Count != 0 && ChromePlugin.配置.是否头顶显示货币变化)
        {
            var rd = new Random();
            var color = rd.Next(1, 10);
            switch (color)
            {
                case 1:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.AntiqueWhite, plr[0].TPlayer.position);
                    break;
                case 2:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Black, plr[0].TPlayer.position);
                    break;
                case 3:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Red, plr[0].TPlayer.position);
                    break;
                case 4:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Green, plr[0].TPlayer.position);
                    break;
                case 5:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Orange, plr[0].TPlayer.position);
                    break;
                case 6:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Blue, plr[0].TPlayer.position);
                    break;
                case 7:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Yellow, plr[0].TPlayer.position);
                    break;
                case 8:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Brown, plr[0].TPlayer.position);
                    break;
                case 9:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.Purple, plr[0].TPlayer.position);
                    break;
                default:
                    Utils.SendCombatMsg(string.Format(ChromePlugin.配置.货币减少时头顶显示, 点券), Color.AliceBlue, plr[0].TPlayer.position);
                    break;
            }
        }
        //Chrome.Status(plr[0]);
        TShock.Log.Info($"{玩家名} 货币 - {点券}");
        return true;

    }
    /// <summary>
    /// =货币
    /// </summary>
    public static bool AmountCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = @0 WHERE `玩家名` = @1", 点券, 玩家名);
        _ = TSPlayer.FindByNameOrID(玩家名);
        //Chrome.Status(plr[0]);
        TShock.Log.Info($"{玩家名} 货币 = {点券}");
        return true;
    }
    public static class Utils
    {
        // Token: 0x06000048 RID: 72 RVA: 0x00003D2C File Offset: 0x00001F2C
        public static void SendCombatMsg(string msg, Color color, Vector2 position)
        {
            Terraria.NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(msg), (int) color.PackedValue, position.X, position.Y, 0f, 0, 0, 0);
        }
    }
    /// <summary>
    /// 查货币
    /// </summary>
    public static long QueryCost(string 玩家名)
    {
        long 货币 = 0;
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名))
        {
            if (表.Read())
            {
                货币 = 表.Get<long>("点券");
            }
        }
        return 货币;
    }/// <summary>
     /// 查重置次数
     /// </summary>
     /// <param name="玩家名"></param>
     /// <returns></returns>
    public static long QueryReset(string 玩家名)
    {
        var 次数 = 0;
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名))
        {
            if (表.Read())
            {
                次数 = 表.Get<int>("职业重置次数");
            }
        }
        return 次数;
    }
    public static bool AddReset(string 玩家名)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `职业重置次数` = `职业重置次数` + 1 WHERE `玩家名` = @0", 玩家名);
        TShock.Log.Info($"{玩家名} 职业已重置次数 + 1");
        return true;
    }
    /// <summary>
    /// 表中是否存在玩家
    /// </summary>
    public static bool IsExist(string 玩家名)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名);
        return 表.Read();
    }
    /// <summary>
    /// 改变职业
    /// </summary>
    public static bool RankGrade(string 玩家名, string 升级到职业)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `职业` = @0 WHERE `玩家名` = @1", 升级到职业, 玩家名);
        _ = TSPlayer.FindByNameOrID(玩家名);
        TShock.Log.Info($"{玩家名} 职业 改变为 {升级到职业}");
        // if (plr.Count != 0)
        //    Chrome.Status(plr[0]);
        return true;
    }
    public static bool StatusGrade(string 玩家名, string 是否显示)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `是否显示` = @0 WHERE `玩家名` = @1", 是否显示, 玩家名);
        _ = TSPlayer.FindByNameOrID(玩家名);
        //  if (plr.Count != 0)
        //    Chrome.Status(plr[0]);
        return true;
    }

    /// <summary>
    /// 查显示状态
    /// </summary>
    /// <param name="玩家名"></param>
    /// <returns></returns>
    public static string QueryStatus(string 玩家名)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名);
        if (表.Read())
        {
            return 表.Get<string>("是否显示") == "false" ? "false" : "true";
        }
        else
        {
            return "true";
        }
    }
    /// <summary>
    /// 查已完成任务
    /// </summary>
    /// <param name="玩家名"></param>
    /// <returns></returns>
    public static List<int> QueryFinishTask(string 玩家名)
    {
        var 已完成任务 = "";
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + "任务系统" + " WHERE `玩家名` = @0", 玩家名))
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
    /// 查职业
    /// </summary>
    /// <param name="玩家名"></param>
    /// <returns></returns>

    public static string QueryRPGGrade(string 玩家名)
    {
        var 职业 = "";
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名))
        {
            if (表.Read())
            {
                职业 = 表.Get<string>("职业");
            }
        }
        return 职业;
    }
}
