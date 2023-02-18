using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;
namespace Permabuffs_V2;

public static class DB
{
    private static IDbConnection db => TShock.DB;
    public static Dictionary<int, DBInfo> PlayerBuffs = new Dictionary<int, DBInfo>();

    public static void Connect()
    {
        var sqlcreator = new SqlTableCreator(db, new MysqlQueryCreator());

        sqlcreator.EnsureTableStructure(new SqlTable("Permabuffs",
            new SqlColumn("UserID", MySqlDbType.Int32) { Primary = true, Unique = true, Length = 4 },
            new SqlColumn("ActiveBuffs", MySqlDbType.Text) { Length = 100 }));

    }

    public static bool LoadUserBuffs(int userid)
    {
        using var result = db.QueryReader("SELECT * FROM Permabuffs WHERE UserID=@0;", userid);
        if (result.Read())
        {
            PlayerBuffs.Add(userid, new DBInfo(result.Get<string>("ActiveBuffs")));
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void AddNewUser(int userid)
    {
        db.Query("INSERT INTO Permabuffs (UserId, ActiveBuffs) VALUES (@0, @1);", userid, string.Empty);
        PlayerBuffs.Add(userid, new DBInfo(""));
    }

    public static void UpdatePlayerBuffs(int userid, List<int> bufflist)
    {
        var buffstring = string.Join(",", bufflist.Select(p => p.ToString()));

        db.Query("UPDATE Permabuffs SET ActiveBuffs=@0 WHERE UserID=@1;", buffstring, userid);
    }

    public static void ClearDB()
    {
        db.Query("DELETE FROM Permabuffs;");
        PlayerBuffs = new Dictionary<int, DBInfo>();
    }

    public static void ClearPlayerBuffs(int userid)
    {
        db.Query("DELETE FROM Permabuffs WHERE UserID=@0;", userid);
        PlayerBuffs[userid] = new DBInfo("");
    }
}