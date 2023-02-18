using MySql.Data.MySqlClient;
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
            new SqlColumn("ID", MySqlDbType.Int64){ Primary = true,Unique=true},
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
}