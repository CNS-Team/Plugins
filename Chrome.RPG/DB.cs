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
            new SqlColumn("等级", MySqlDbType.Int64),
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
    public static void AddCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = `点券` + @0 WHERE `玩家名` = @1", 点券, 玩家名);
        var plr = TSPlayer.FindByNameOrID(玩家名);
        if (是否头顶显示 && plr.Count != 0 && Chrome.配置.是否头顶显示货币变化)
        {
            var rd = new Random();
            var r = rd.Next(0, 255);
            var g = rd.Next(0, 255);
            var b = rd.Next(0, 255);
            var c = new Microsoft.Xna.Framework.Color(r, g, b);
            SendCombatMsg(string.Format(Chrome.配置.货币增加时头顶显示, 点券), c, plr[0].TPlayer.position);
        }
        //Chrome.Status(plr[0]);
        TShock.Log.Info($"{玩家名} 货币 + {点券}");
    }
    /// <summary>
    /// -货币
    /// </summary>
    public static void DelCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = `点券` - @0 WHERE `玩家名` = @1", 点券, 玩家名);
        var plr = TSPlayer.FindByNameOrID(玩家名);
        if (是否头顶显示 && plr.Count != 0 && Chrome.配置.是否头顶显示货币变化)
        {
            var rd = new Random();
            var r = rd.Next(0, 255);
            var g = rd.Next(0, 255);
            var b = rd.Next(0, 255);
            var c = new Microsoft.Xna.Framework.Color(r, g, b);
            SendCombatMsg(string.Format(Chrome.配置.货币减少时头顶显示, 点券), c, plr[0].TPlayer.position);
        }
        //Chrome.Status(plr[0]);
        TShock.Log.Info($"{玩家名} 货币 - {点券}");
    }
    /// <summary>
    /// =货币
    /// </summary>
    public static void AmountCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = @0 WHERE `玩家名` = @1", 点券, 玩家名);
        _ = TSPlayer.FindByNameOrID(玩家名);
        //Chrome.Status(plr[0]);
        TShock.Log.Info($"{玩家名} 货币 = {点券}");
    }
    public static void SendCombatMsg(string msg, Color color, Vector2 position)
    {
        Terraria.NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(msg), (int) color.PackedValue, position.X, position.Y, 0f, 0, 0, 0);
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
    }
    /// <summary>
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
    public static void AddReset(string 玩家名)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `职业重置次数` = `职业重置次数` + 1 WHERE `玩家名` = @0", 玩家名);
        TShock.Log.Info($"{玩家名} 职业已重置次数 + 1");
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
    public static void RankGrade(string 玩家名, string 升级到职业)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `职业` = @0 WHERE `玩家名` = @1", 升级到职业, 玩家名);
        TShock.DB.Query("UPDATE Chrome_RPG SET `等级` = 1 WHERE `玩家名` = @0", 玩家名);
        TShock.Log.Info($"{玩家名} 职业 转为 {升级到职业}");
    }

    /// <summary>
    /// 升级
    /// </summary>
    /// <param name="玩家名"></param>
    public static void RankGrade(string 玩家名)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `等级` =  `等级` + 1 WHERE `玩家名` = @0", 玩家名);
        TShock.Log.Info($"{玩家名} 升级");
    }
    public static void StatusGrade(string 玩家名, string 是否显示)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `是否显示` = @0 WHERE `玩家名` = @1", 是否显示, 玩家名);
    }

    /// <summary>
    /// 查显示状态
    /// </summary>
    /// <param name="玩家名"></param>
    /// <returns></returns>
    public static string QueryStatus(string 玩家名)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名);
        return 表.Read() ? 表.Get<string>("是否显示") == "false" ? "false" : "true" : "true";
    }
}