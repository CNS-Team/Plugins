using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI.DB;

namespace CNSUniCore;

// Token: 0x02000003 RID: 3
public class DBManager
{
    // Token: 0x06000006 RID: 6 RVA: 0x0000227D File Offset: 0x0000047D
    public DBManager(IDbConnection connection)
    {
        this.connection = connection;
    }

    // Token: 0x06000007 RID: 7 RVA: 0x00002290 File Offset: 0x00000490
    public void CreateBansTable()
    {
        var sqlTable = new SqlTable("unibans", new SqlColumn[]
        {
            new SqlColumn("Name", MySqlDbType.VarChar, new int?(255))
            {
                Unique = true
            },
            new SqlColumn("IP", MySqlDbType.Text, new int?(255)),
            new SqlColumn("UUID", MySqlDbType.VarChar, new int?(255)),
            new SqlColumn("Reason", MySqlDbType.VarChar, new int?(255)),
            new SqlColumn("BanTime", MySqlDbType.VarChar, new int?(255)),
            new SqlColumn("AddTime", MySqlDbType.VarChar, new int?(255))
        });
        var dbConnection = this.connection;
        var flag = (int) DbExt.GetSqlType(this.connection) != 1;
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
        new SqlTableCreator(dbConnection, queryBuilder2).EnsureTableStructure(sqlTable);
    }

    // Token: 0x06000008 RID: 8 RVA: 0x00002350 File Offset: 0x00000550
    public void CreateSponsorTable()
    {
        var sqlTable = new SqlTable("unisponsors", new SqlColumn[]
        {
            new SqlColumn("Name", MySqlDbType.VarChar, new int?(255))
            {
                Unique = true
            },
            new SqlColumn("OriginGroup", MySqlDbType.VarChar, new int?(255)),
            new SqlColumn("NowGroup", MySqlDbType.VarChar, new int?(255)),
            new SqlColumn("StartTime", MySqlDbType.VarChar, new int?(255)),
            new SqlColumn("EndTime", MySqlDbType.VarChar, new int?(255))
        });
        var dbConnection = this.connection;
        var flag = (int) DbExt.GetSqlType(this.connection) != 1;
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
        new SqlTableCreator(dbConnection, queryBuilder2).EnsureTableStructure(sqlTable);
    }

    // Token: 0x06000009 RID: 9 RVA: 0x00002448 File Offset: 0x00000648
    public void AddPlayer(PlayerInfo plr)
    {
        var text = string.Concat(new string[]
        {
            "INSERT INTO unibans (Name,IP,UUID,AddTime, BanTime,Reason) VALUES ('",
            plr.Name,
            "','",
            plr.IP,
            "','",
            plr.UUID,
            "','",
            plr.AddTime,
            "','",
            plr.BanTime,
            "','",
            plr.Reason,
            "');"
        });
        DbExt.Query(this.connection, text, Array.Empty<object>());
    }

    // Token: 0x0600000A RID: 10 RVA: 0x000024B0 File Offset: 0x000006B0
    public void DeletePlayer(string plr)
    {
        var text = "DELETE FROM unibans WHERE Name='" + plr + "';";
        DbExt.Query(this.connection, text, Array.Empty<object>());
    }

    // Token: 0x0600000B RID: 11 RVA: 0x000024E4 File Offset: 0x000006E4
    public void UpdatePlayer(PlayerInfo plr)
    {
        var text = string.Concat(new string[]
        {
            "UPDATE unibans SET Name='",
            plr.Name,
            "',IP='",
            plr.IP,
            "',UUID='",
            plr.UUID,
            "',AddTime='",
            plr.AddTime,
            "',BanTime='",
            plr.BanTime,
           "',Reason='",
            plr.Reason,
            "' WHERE Name='",
            plr.Name,
            "';"
        });
        DbExt.Query(this.connection, text, Array.Empty<object>());
    }

    // Token: 0x0600000C RID: 12 RVA: 0x00002560 File Offset: 0x00000760
    public PlayerInfo? GetPlayer(string plrname)
    {
        var text = "SELECT * FROM unibans WHERE Name='" + plrname + "';";
        PlayerInfo? result;
        using (var queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
        {
            var flag = queryResult.Read();
            if (flag)
            {
                var name = queryResult.Get<string>("Name");
                var ip = queryResult.Get<string>("IP");
                var uuid = queryResult.Get<string>("UUID");
                var Addtime = queryResult.Get<string>("AddTime");
                var BanTime = queryResult.Get<string>("BanTime");
                var reason = queryResult.Get<string>("Reason");
                result = new PlayerInfo
                {
                    Name = name,
                    IP = ip,
                    UUID = uuid,
                    AddTime = Addtime,
                    BanTime = BanTime,
                    Reason = reason
                };
            }
            else
            {
                result = null;
            }
        }
        return result;
    }

    // Token: 0x0600000D RID: 13 RVA: 0x00002608 File Offset: 0x00000808
    public List<PlayerInfo> GetPlayers()
    {
        var text = "SELECT * FROM unibans;";
        var list = new List<PlayerInfo>();
        List<PlayerInfo> result;
        using (var queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
        {
            while (queryResult.Read())
            {
                var name = queryResult.Get<string>("Name");
                var ip = queryResult.Get<string>("IP");
                var uuid = queryResult.Get<string>("UUID");
                var Addtime = queryResult.Get<string>("AddTime");
                var BanTime = queryResult.Get<string>("BanTime");
                var reason = queryResult.Get<string>("Reason");
                var item = new PlayerInfo
                {
                    Name = name,
                    IP = ip,
                    UUID = uuid,
                    AddTime = Addtime,
                    BanTime = BanTime,
                    Reason = reason
                };
                list.Add(item);
            }
            result = list;
        }
        return result;
    }

    // Token: 0x0600000E RID: 14 RVA: 0x000026B8 File Offset: 0x000008B8
    public void AddSponsor(SponsorInfo plr)
    {
        DbExt.Query(query: $"INSERT INTO unisponsors (Name,OriginGroup,NowGroup,StartTime,EndTime) VALUES ('{plr.name}','{plr.originGroup}','{plr.group}','{plr.startTime}','{plr.endTime}');", olddb: this.connection, args: Array.Empty<object>());
    }

    // Token: 0x0600000F RID: 15 RVA: 0x00002780 File Offset: 0x00000980
    public void DeleteSponsor(string plr)
    {
        var text = "DELETE FROM unisponsors WHERE Name='" + plr + "';";
        DbExt.Query(this.connection, text, Array.Empty<object>());
    }

    // Token: 0x06000010 RID: 16 RVA: 0x000027B4 File Offset: 0x000009B4
    public void UpdateSponsor(SponsorInfo plr)
    {
        DbExt.Query(query: $"UPDATE unisponsors SET Name='{plr.name}',OriginGroup='{plr.originGroup}',NowGroup='{plr.group}',StartTime='{plr.startTime}',EndTime='{plr.endTime}' WHERE Name='{plr.name}';", olddb: this.connection, args: Array.Empty<object>());
    }

    // Token: 0x06000011 RID: 17 RVA: 0x00002898 File Offset: 0x00000A98
    public SponsorInfo? GetSponsor(string plrname)
    {
        var text = "SELECT * FROM unisponsors WHERE Name='" + plrname + "';";
        SponsorInfo? result;
        using (var queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
        {
            var flag = queryResult.Read();
            if (flag)
            {
                var name = queryResult.Get<string>("Name");
                var origin = queryResult.Get<string>("OriginGroup");
                var group = queryResult.Get<string>("NowGroup");
                var s = queryResult.Get<string>("StartTime");
                var end = DateTime.Parse(queryResult.Get<string>("EndTime"));
                result = new SponsorInfo(name, origin, group, DateTime.Parse(s), end);
            }
            else
            {
                result = null;
            }
        }
        return result;
    }

    // Token: 0x06000012 RID: 18 RVA: 0x00002954 File Offset: 0x00000B54
    public List<SponsorInfo> GetSponsors()
    {
        var text = "SELECT * FROM unisponsors;";
        var list = new List<SponsorInfo>();
        List<SponsorInfo> result;
        using (var queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
        {
            while (queryResult.Read())
            {
                var name = queryResult.Get<string>("Name");
                var origin = queryResult.Get<string>("OriginGroup");
                var group = queryResult.Get<string>("NowGroup");
                var s = queryResult.Get<string>("StartTime");
                var end = DateTime.Parse(queryResult.Get<string>("EndTime"));
                var item = new SponsorInfo(name, origin, group, DateTime.Parse(s), end);
                list.Add(item);
            }
            result = list;
        }
        return result;
    }

    // Token: 0x04000006 RID: 6
    private readonly IDbConnection connection;
}