using LazyUtils;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using TShockAPI;
using TShockAPI.DB;

namespace OnlineInfo;

internal static class Utils
{
    public static IDataProvider DBProvider => OIConfig.Instance.DBType switch
    {
        SqlType.Mysql => MySqlTools.GetDataProvider(),
        SqlType.Sqlite => SQLiteTools.GetDataProvider("SQLite.Classic"),
        _ => null,
    };

    public static string DBConnectionString => OIConfig.Instance.DBType switch
    {
        SqlType.Mysql => new MySqlConnectionStringBuilder()
        {
            Server = OIConfig.Instance.MySqlHost,
            Port = OIConfig.Instance.MySqlPort,
            Database = OIConfig.Instance.MySqlName,
            UserID = OIConfig.Instance.MySqlUser,
            Password = OIConfig.Instance.MySqlPassword
        }.ConnectionString,

        SqlType.Sqlite => new SqliteConnectionStringBuilder()
        {
            DataSource = Path.IsPathRooted(OIConfig.Instance.SqlitePath) ? OIConfig.Instance.SqlitePath : Path.Combine(TShock.SavePath, OIConfig.Instance.SqlitePath),
            Pooling = true
        }.ConnectionString,

        _ => string.Empty
    };

    public static DataConnection GetDBConnection()
    {
        var dc = new DataConnection(DBProvider, DBConnectionString);
        dc.MappingSchema.AddScalarType(typeof(string), new LinqToDB.SqlQuery.SqlDataType(DataType.NVarChar, 255));
        return dc;
    }

    public static DisposableQuery<T> GetDBQuery<T>() where T : class
    {
        var dbconn = GetDBConnection();
        return new DisposableQuery<T>(dbconn.GetTable<T>(), dbconn);
    }
}

internal static class Logger
{
    private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name!;

    public static void ConsoleInfo(string message)
    {
        TShock.Log.ConsoleInfo($"[{AssemblyName}] [INFO] {message}");
    }

    public static void ConsoleError(string message, [CallerMemberName] string callerName = "")
    {
        TShock.Log.ConsoleError($"[{AssemblyName}] [ERROR] [{callerName}] {message}");
    }

    public static void ConsoleDebug(string message, [CallerMemberName] string callerName = "")
    {
        TShock.Log.ConsoleDebug($"[{AssemblyName}] [DEBUG] [{callerName}] {message}");
    }
}