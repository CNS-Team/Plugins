using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using TShockAPI.DB;
using System;


namespace CNSUniCore
{
    public class DBManager
    {
        public DBManager(IDbConnection connection)
        {
            this.connection = connection;
        }

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

        public void DeletePlayer(string plr)
        {
            string text = "DELETE FROM unibans WHERE Name='" + plr + "';";
            DbExt.Query(this.connection, text, Array.Empty<object>());
        }

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

        public void DeleteSponsor(string plr)
        {
            string text = "DELETE FROM unisponsors WHERE Name='" + plr + "';";
            DbExt.Query(this.connection, text, Array.Empty<object>());
        }

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

        private IDbConnection connection;
    }
}
