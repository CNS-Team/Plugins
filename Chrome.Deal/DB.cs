using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;

namespace deal;

public static class DB
{
    /// <summary>
    /// 生成数据表
    /// </summary>
    public static void Reload()
    {
        var table = new SqlTable("Chrome_Deal", new SqlColumn[]
        {
            new SqlColumn("ID", MySqlDbType.Int64){ Primary = true,Unique=true,AutoIncrement = true},
            new SqlColumn("玩家名", MySqlDbType.VarChar, new int?(256)),
            new SqlColumn("价格", MySqlDbType.Int64){ DefaultValue = "0" },
            new SqlColumn("物品", MySqlDbType.Int64){ DefaultValue = "0" },
            new SqlColumn("前缀", MySqlDbType.Int64){ DefaultValue = "0" },
            new SqlColumn("数量", MySqlDbType.Int64){ DefaultValue = "0" } }); ;
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
        /*
        SqlTable sqlTable = new("Chrome_Deal", new SqlColumn[]
        {
            new SqlColumn("ID", MySqlDbType.Int64){ DefaultValue = "1" ,Primary = true,Unique=true},
            new SqlColumn("玩家名", MySqlDbType.Text){ DefaultValue = null},
            new SqlColumn("价格", MySqlDbType.Int64){ DefaultValue = "0" },
            new SqlColumn("物品", MySqlDbType.Int64){ DefaultValue = "0" },
            new SqlColumn("前缀", MySqlDbType.Int64){ DefaultValue = "0" },
            new SqlColumn("数量", MySqlDbType.Int64){ DefaultValue = "0" },
        });
        var List = new SqlTableCreator(TShock.DB, TShock.DB.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(sqlTable);
        */
    }
    public static int GetMaxId()
    {
        var i = 1;
        while (true)
        {
            using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE `ID`=@0", i);
            if (表.Read())
            {
                i++;
                continue;
            }
            else
            {
                return i - 1;
            }
        }
    }
    public static bool AddItem(string 玩家名, long 价格, int 物品, int 前缀, int 数量)
    {
        TShock.DB.Query("INSERT INTO Chrome_Deal (`ID`, `玩家名`, `价格` , `物品`, `前缀`, `数量`) VALUES " + "(@0, @1, @2, @3, @4, @5);", GetMaxId() + 1, 玩家名, 价格, 物品, 前缀, 数量);
        return true;
    }
    public static bool QueryShop(int ID)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE `ID`=@0", ID);
        return 表.Read();
    }
    public static long QueryPrice(int ID)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE `ID`=@0", ID);
        return 表.Read() ? 表.Get<long>("价格") : -1;
    }
    public static string QueryOwner(int ID)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE `ID`=@0", ID);
        return 表.Read() ? 表.Get<string>("玩家名") : "";
    }

    public static int[] GetShop(int ID)
    {
        using var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE `ID`=@0", ID);
        if (表.Read())
        {
            int[] shop = { 表.Get<int>("物品"), 表.Get<int>("数量"), 表.Get<int>("前缀") };
            return shop;
        }
        else
        {
            int[] shop = { 0, 0, 0 };
            return shop;
        }
    }
    /*
             public static int QueryNetID(int ID)
    {
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE ID=@0", ID))
        {
            if (表.Read())
            {
                return 表.Get<int>("物品");
            }
            else
            {
                return 0;
            }
        }
    }
    public static int QueryPrefix(int ID)
    {
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE ID=@0", ID))
        {
            if (表.Read())
            {
                return 表.Get<int>("前缀");
            }
            else
            {
                return 0;
            }
        }
    }
    public static int QueryStack(int ID)
    {
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE ID=@0", ID))
        {
            if (表.Read())
            {
                return 表.Get<int>("数量");
            }
            else
            {
                return 0;
            }
        }
    }
    */
    public static bool DelItem(int ID, bool 重新排序ID = true)
    {
        TShock.DB.Query("DELETE FROM Chrome_Deal WHERE `ID` = @0", ID);
        while (QueryShop(ID + 1) && 重新排序ID)
        {
            TShock.DB.Query("UPDATE Chrome_Deal SET `ID` = @0 WHERE `ID` = @1", ID, ID + 1);
            ID++;
        }
        return true;
    }
    public static bool AddCost(string 玩家名, long 点券, bool 是否头顶显示 = false)
    {
        TShock.DB.Query("UPDATE Chrome_RPG SET `点券` = `点券` + @0 WHERE `玩家名` = @1", 点券, 玩家名);
        var plr = TSPlayer.FindByNameOrID(玩家名);
        if (是否头顶显示 && plr.Count != 0 && Plugin.Qw配置.是否头顶显示货币变化)
        {
            Utils.SendCombatMsg(string.Format(Plugin.Qw配置.货币增加时头顶显示, 点券), Color.AliceBlue, plr[0].TPlayer.position);
        }
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
        if (是否头顶显示 && plr.Count != 0 && Plugin.Qw配置.是否头顶显示货币变化)
        {
            Utils.SendCombatMsg(string.Format(Plugin.Qw配置.货币减少时头顶显示, 点券), Color.AliceBlue, plr[0].TPlayer.position);
        }
        TShock.Log.Info($"{玩家名} 货币 - {点券}");
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
    }
    /// <summary>
    /// 表中是否存在玩家
    /// </summary>

}
