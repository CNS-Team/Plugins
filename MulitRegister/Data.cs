using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using TShockAPI;
using TShockAPI.DB;

namespace MulitRegister;

public class Data
{
    public static IDbConnection db;

    public static void Init()
    {
        //IL_0007: Unknown result type (might be due to invalid IL or missing references)
        //IL_000d: Expected O, but got Unknown
        var config = Config.GetConfig();
        var val = new MySqlConnection();
        ((DbConnection) (object) val).ConnectionString = $"Server={config.MysqlIP}; Port={config.MysqlPort.ToString()}; Database={config.MysqlDb}; Uid={config.MysqlUsername}; Pwd={config.MysqlPassword};";
        db = (IDbConnection) val;
    }

    public static bool AddAccountFromHost(string name)
    {
        var result = false;
        var val = DbExt.QueryReader(db, "select Username,Password,UUID,Usergroup,Registered,LastAccessed,KnownIPs from Users where Username='" + name + "'", Array.Empty<object>());
        try
        {
            if (val.Read())
            {
                result = true;
                DbExt.Query(TShock.DB, "insert into Users(Username,Password,UUID,Usergroup,Registered,LastAccessed,KnownIPs)values('" + val.Reader.GetString(0) + "','" + val.Reader.GetString(1) + "','" + val.Reader.GetString(2) + "','" + val.Reader.GetString(3) + "','" + val.Reader.GetString(4) + "','" + val.Reader.GetString(5) + "','" + val.Reader.GetString(6) + "')", Array.Empty<object>());
            }
        }
        finally
        {
            ((IDisposable) val)?.Dispose();
        }
        return result;
    }

    public static bool UpdateAccount(string name)
    {
        var result = false;
        var val = DbExt.QueryReader(db, "select Username,Password,UUID,Usergroup,Registered,LastAccessed,KnownIPs from Users where Username='" + name + "'", Array.Empty<object>());
        try
        {
            if (val.Read())
            {
                result = true;
                DbExt.Query(TShock.DB, "update Users set Password='" + val.Reader.GetString(1) + "',UUID='" + val.Reader.GetString(2) + "',Registered='" + val.Reader.GetString(4) + "',LastAccessed='" + val.Reader.GetString(5) + "',KnownIPs='" + val.Reader.GetString(6) + "' where Username='" + name + "'", Array.Empty<object>());
            }
        }
        finally
        {
            ((IDisposable) val)?.Dispose();
        }
        return result;
    }
}