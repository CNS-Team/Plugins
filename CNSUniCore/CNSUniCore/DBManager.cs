using MySql.Data.MySqlClient;
using System.Data;
using System.Runtime.CompilerServices;
using TShockAPI.DB;

namespace CNSUniCore
{
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
            SqlTable sqlTable = new SqlTable("unibans", new SqlColumn[]
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
            IDbConnection dbConnection = this.connection;
            bool flag = (int)DbExt.GetSqlType(this.connection) != 1;
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
            SqlTable sqlTable = new SqlTable("unisponsors", new SqlColumn[]
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
            IDbConnection dbConnection = this.connection;
            bool flag = (int)DbExt.GetSqlType(this.connection) != 1;
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
            string text = string.Concat(new string[]
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
            string text = "DELETE FROM unibans WHERE Name='" + plr + "';";
            DbExt.Query(this.connection, text, Array.Empty<object>());
        }

        // Token: 0x0600000B RID: 11 RVA: 0x000024E4 File Offset: 0x000006E4
        public void UpdatePlayer(PlayerInfo plr)
        {
            string text = string.Concat(new string[]
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
        public PlayerInfo GetPlayer(string plrname)
        {
            string text = "SELECT * FROM unibans WHERE Name='" + plrname + "';";
            PlayerInfo result;
            using (QueryResult queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
            {
                bool flag = queryResult.Read();
                if (flag)
                {
                    string name = queryResult.Get<string>("Name");
                    string ip = queryResult.Get<string>("IP");
                    string uuid = queryResult.Get<string>("UUID");
                    string Addtime = queryResult.Get<string>("AddTime");
                    string BanTime = queryResult.Get<string>("BanTime");
                    string reason = queryResult.Get<string>("Reason");
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
            string text = "SELECT * FROM unibans;";
            List<PlayerInfo> list = new List<PlayerInfo>();
            List<PlayerInfo> result;
            using (QueryResult queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
            {
                while (queryResult.Read())
                {
                    string name = queryResult.Get<string>("Name");
                    string ip = queryResult.Get<string>("IP");
                    string uuid = queryResult.Get<string>("UUID");
                    string Addtime = queryResult.Get<string>("AddTime");
                    string BanTime = queryResult.Get<string>("BanTime");
                    string reason = queryResult.Get<string>("Reason");
                    PlayerInfo item = new PlayerInfo
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
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(94, 5);
            defaultInterpolatedStringHandler.AppendLiteral("INSERT INTO unisponsors (Name,OriginGroup,NowGroup,StartTime,EndTime) VALUES ('");
            defaultInterpolatedStringHandler.AppendFormatted(plr.name);
            defaultInterpolatedStringHandler.AppendLiteral("','");
            defaultInterpolatedStringHandler.AppendFormatted(plr.originGroup);
            defaultInterpolatedStringHandler.AppendLiteral("','");
            defaultInterpolatedStringHandler.AppendFormatted(plr.group);
            defaultInterpolatedStringHandler.AppendLiteral("','");
            defaultInterpolatedStringHandler.AppendFormatted<DateTime>(plr.startTime);
            defaultInterpolatedStringHandler.AppendLiteral("','");
            defaultInterpolatedStringHandler.AppendFormatted<DateTime>(plr.endTime);
            defaultInterpolatedStringHandler.AppendLiteral("');");
            string text = defaultInterpolatedStringHandler.ToStringAndClear();
            DbExt.Query(this.connection, text, Array.Empty<object>());
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00002780 File Offset: 0x00000980
        public void DeleteSponsor(string plr)
        {
            string text = "DELETE FROM unisponsors WHERE Name='" + plr + "';";
            DbExt.Query(this.connection, text, Array.Empty<object>());
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000027B4 File Offset: 0x000009B4
        public void UpdateSponsor(SponsorInfo plr)
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(96, 6);
            defaultInterpolatedStringHandler.AppendLiteral("UPDATE unisponsors SET Name='");
            defaultInterpolatedStringHandler.AppendFormatted(plr.name);
            defaultInterpolatedStringHandler.AppendLiteral("',OriginGroup='");
            defaultInterpolatedStringHandler.AppendFormatted(plr.originGroup);
            defaultInterpolatedStringHandler.AppendLiteral("',NowGroup='");
            defaultInterpolatedStringHandler.AppendFormatted(plr.group);
            defaultInterpolatedStringHandler.AppendLiteral("',StartTime='");
            defaultInterpolatedStringHandler.AppendFormatted<DateTime>(plr.startTime);
            defaultInterpolatedStringHandler.AppendLiteral("',EndTime='");
            defaultInterpolatedStringHandler.AppendFormatted<DateTime>(plr.endTime);
            defaultInterpolatedStringHandler.AppendLiteral("' WHERE Name='");
            defaultInterpolatedStringHandler.AppendFormatted(plr.name);
            defaultInterpolatedStringHandler.AppendLiteral("';");
            string text = defaultInterpolatedStringHandler.ToStringAndClear();
            DbExt.Query(this.connection, text, Array.Empty<object>());
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002898 File Offset: 0x00000A98
        public SponsorInfo GetSponsor(string plrname)
        {
            string text = "SELECT * FROM unisponsors WHERE Name='" + plrname + "';";
            SponsorInfo result;
            using (QueryResult queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
            {
                bool flag = queryResult.Read();
                if (flag)
                {
                    string name = queryResult.Get<string>("Name");
                    string origin = queryResult.Get<string>("OriginGroup");
                    string group = queryResult.Get<string>("NowGroup");
                    string s = queryResult.Get<string>("StartTime");
                    DateTime end = DateTime.Parse(queryResult.Get<string>("EndTime"));
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
            string text = "SELECT * FROM unisponsors;";
            List<SponsorInfo> list = new List<SponsorInfo>();
            List<SponsorInfo> result;
            using (QueryResult queryResult = DbExt.QueryReader(this.connection, text, Array.Empty<object>()))
            {
                while (queryResult.Read())
                {
                    string name = queryResult.Get<string>("Name");
                    string origin = queryResult.Get<string>("OriginGroup");
                    string group = queryResult.Get<string>("NowGroup");
                    string s = queryResult.Get<string>("StartTime");
                    DateTime end = DateTime.Parse(queryResult.Get<string>("EndTime"));
                    SponsorInfo item = new SponsorInfo(name, origin, group, DateTime.Parse(s), end);
                    list.Add(item);
                }
                result = list;
            }
            return result;
        }

        // Token: 0x04000006 RID: 6
        private IDbConnection connection;
    }
}
