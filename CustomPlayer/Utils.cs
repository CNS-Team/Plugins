using System.Data;
using System.Reflection;

using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

using MySql.Data.MySqlClient;

using GetText;

using VBY.Basic;
using VBY.Basic.Extension;
using static CustomPlayer.TableInfo;

namespace CustomPlayer;
static class Utils
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public static Func<FormattableStringAdapter, object[], string> GetStringMethod;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public static SqlTable SqlTableCreate(Type tableClassType)
    {
        var fields = tableClassType.GetFields();
        var columns = new List<SqlColumn>(fields.Length);
        foreach (var field in fields.OrderBy(x => x.MetadataToken))
        {
            if (field.FieldType == TypeOf.Int32)
            {
                columns.Add(new SqlColumn(field.Name, MySqlDbType.Int32));
            }
            else if(field.FieldType == TypeOf.DateTime)
            {
                columns.Add(new SqlColumn(field.Name, MySqlDbType.DateTime));
            }
            else if(field.FieldType == TypeOf.String)
            {
                columns.Add(new SqlColumn(field.Name, MySqlDbType.Text, field.GetCustomAttribute<LengthAttribute>()?.Length));
            }
        }
        return new SqlTable(tableClassType.Name, columns.ToArray());
    }
    public static string GetString(FormattableStringAdapter text, params object[] args)
    {
        return GetStringMethod(text, args)!;
    }

    public static CustomPlayer? FindPlayer(string name)
    {
        return CustomPlayerPluginHelpers.Players.Find(x => x?.Name == name);
    }

    public static int Query(string commandText, params object[] args)
    {
        return DbExt.Query(CustomPlayerPluginHelpers.DB, commandText, args);
    }

    public static QueryResult QueryReader(string commandText, params object[] args)
    {
        return DbExt.QueryReader(CustomPlayerPluginHelpers.DB, commandText, args);
    }

    public static QueryResult QueryPlayer(string name)
    {
        return QueryReader("select Commands,`Group`,ChatColor from PlayerList where Name = @0", name);
    }

    public static QueryResult TitleQuery(string type, string name)
    {
        return QueryReader($"select Id,Value,StartTime,EndTime,DurationText from {type} where Name  = @0", name);
    }

    public static QueryResult TitleQuery(string type, string name, int titleId)
    {
        return QueryReader($"select Value,StartTime,EndTime,DurationText from {type} where Name  = @0 AND Id = @1", name, titleId);
    }

    public static QueryResult PrefixQuery(string name)
    {
        return TitleQuery("Prefix", name);
    }

    public static QueryResult SuffixQuery(string name)
    {
        return TitleQuery("Suffix", name);
    }

    public static void I(string msg)
    {
        if (CustomPlayerPlugin.ReadConfig.Root.Debug)
        {
            TSPlayer.Server.SendInfoMessage(msg);
        }
    }
    public static void I(string msg, params object[] args)
    {
        if (CustomPlayerPlugin.ReadConfig.Root.Debug)
        {
            TSPlayer.Server.SendInfoMessage(msg, args);
        }
    }
    public static bool NotifyPlayer(string typeChinese, bool forever, TimeOutObject data, out CustomPlayer cply)
    {
        cply = FindPlayer(data.Name)!;
        if (cply != null)
        {
            if (forever)
            {
                cply.Player.SendInfoMessage($"你已获得永久{typeChinese}:{data.Value}");
            }
            else
            {
                cply.Player.SendInfoMessage($"你获得{typeChinese}:{data.Value}");
            }

            CustomPlayerPluginHelpers.TimeOutList.Add(data);
            return true;
        }
        return false;
    }
    #region Extension
    public static string NullOrEmptyReturn(this string? args, string value)
    {
        return string.IsNullOrEmpty(args) ? value : args;
    }

    public static CustomPlayer GetPlayer(this TSPlayer player)
    {
        return CustomPlayerPluginHelpers.Players[player.Index];
    }

    public static void Reload(this CustomPlayer cply)
    {
        CustomPlayerPlugin.OnPlayerLogout(new PlayerLogoutEventArgs(cply.Player));
        CustomPlayerPlugin.OnPlayerPostLogin(new PlayerPostLoginEventArgs(cply.Player));
    }
    public static TimeOutObject GetTimeOutObject(this IDataReader reader, string name, string type)
    {
        return new TimeOutObject(name, reader.GetString(nameof(TableInfo.ExpirationInfo.Value)), type, reader.GetDateTime(nameof(TableInfo.ExpirationInfo.StartTime)), reader.GetDateTime(nameof(TableInfo.ExpirationInfo.EndTime)), reader.GetString(nameof(TableInfo.ExpirationInfo.DurationText))); ;
    }
    public static void Add(this List<Prefix> list, TimeOutObject data)
    {
        list.Add(new Prefix(data.Name, data.Id, data.Value, data.StartTime, data.EndTime, data.DurationText));
    }
    #endregion
}
