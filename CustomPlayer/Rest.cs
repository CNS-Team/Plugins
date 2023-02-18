using Rests;
using TShockAPI;

using VBY.Basic.Extension;

namespace CustomPlayer;

public partial class CustomPlayerPlugin
{
    private void RestInit()
    {
        TShock.RestApi.Register(new SecureRestCommand("/custom/add/perm", args => this.RestAdd(args, "Perm"), "custom.admin"));
        TShock.RestApi.Register(new SecureRestCommand("/custom/add/prefix", args => this.RestAdd(args, "Prefix"), "custom.admin"));
        TShock.RestApi.Register(new SecureRestCommand("/custom/add/suffix", args => this.RestAdd(args, "Suffix"), "custom.admin"));

        TShock.RestApi.Register(new SecureRestCommand("/custom/list/perm", args => this.RestList(args, "Perm"), "custom.admin"));
        TShock.RestApi.Register(new SecureRestCommand("/custom/list/prefix", args => this.RestList(args, "Prefix"), "custom.admin"));
        TShock.RestApi.Register(new SecureRestCommand("/custom/list/suffix", args => this.RestList(args, "Suffix"), "custom.admin"));

        TShock.RestApi.Register(new SecureRestCommand("custom/wear/prefix", args => this.RestWear(args, "Prefix"), "custom.admin"));
        TShock.RestApi.Register(new SecureRestCommand("custom/wear/suffix", args => this.RestWear(args, "Suffix"), "custom.admin"));
    }
    private object RestAdd(RestRequestArgs args, string type)
    {
        var result = new RestObject("200");
        if (args.Parameters.Count() < 3)
        {
            return this.SetError(result, "参数不足3个");
        }

        var name = args.Parameters["name"];
        var value = args.Parameters["value"];
        var time = args.Parameters["time"];
        if (string.IsNullOrEmpty(name))
        {
            return this.SetError(result, "name is empty");
        }

        if (string.IsNullOrEmpty(value))
        {
            return this.SetError(result, "value is empty");
        }

        if (string.IsNullOrEmpty(time))
        {
            return this.SetError(result, "time is empty");
        }

        var startTime = DateTime.Now;
        DateTime endTime = new();
        TimeSpan addTime = new();
        var forever = time == "-1";

        if (TimeParse(forever, time, ref startTime, ref endTime, ref addTime, null))
        {
            return this.SetError(result, "时间转换失败");
        }

        if (type == "Perm")
        {
            if (Utils.NotifyPlayer("权限", forever, new TimeOutObject(name, value, nameof(Permission), startTime, endTime, time), out var cply))
            {
                cply.AddPermission(value);
            }

            Utils.Query("insert into ExpirationInfo(Name,Value,Type,StartTime,EndTime,DurationText) values(@0,@1,@2,@3,@4,@5)", name, value, nameof(Permission), startTime, endTime, time);
            result.Response = forever ? $"添加成功 玩家:{name} 权限:{value} 持续时间:永久" : $"添加成功 玩家:{name} 权限:{value} 起始时间:{startTime} 结束时间:{endTime} 持续时间:{addTime}";
        }
        else
        {
            var addId = 0;
            var isPrefix = type == nameof(CustomPlayer.Prefix);
            var typeChinese = isPrefix ? "前缀" : "后缀";
            if (TitleParse(type, ref value, null))
            {
                return this.SetError(result, "get未找到值");
            }

            var data = new TimeOutObject(name, value, type, startTime, endTime, time, addId);
            if (Utils.NotifyPlayer(typeChinese, forever, data, out var cply))
            {
                (isPrefix ? cply.PrefixList : cply.SuffixList).Add(data);
            }

            using (var maxReader = Utils.QueryReader($"select max(Id) from {type} where Name = @0", name))
            {
                if (maxReader.Read() && !maxReader.Reader.IsDBNull(0))
                {
                    addId = maxReader.Reader.GetInt32(0) + 1;
                }
            }
            Utils.Query($"insert into {type}(Name,Id,Value,StartTime,EndTime,DurationText) values(@0,@1,@2,@3,@4,@5)", name, addId, value, startTime, endTime, time);
            result.Response = forever
                ? $"添加成功 玩家:{name} {typeChinese}:{value} Id:{addId} 持续时间:永久"
                : $"添加成功 玩家:{name} {typeChinese}:{value} Id:{addId} 起始时间:{startTime} 结束时间:{endTime} 持续时间:{addTime}";
        }
        return result;
    }
    private object RestList(RestRequestArgs args, string type)
    {
        var result = new RestObject("200");
        var name = args.Parameters["name"];
        if (string.IsNullOrEmpty(name))
        {
            return this.SetError(result, "name is empty");
        }

        if (type == "Perm")
        {
            var list = new List<PermInfo>() { };
            using (var reader = Utils.QueryReader("select Value,EndTime,DurationText from ExpirationInfo where name = @0 AND Type = @1", name, nameof(Permission)))
            {
                if (reader.Read())
                {
                    reader.Reader.DoForEach(x => list.Add(new PermInfo(x.GetString("Value"), x.GetString("DurationText") == "-1" ? "永久" : (x.GetDateTime("EndTime") - DateTime.Now).ToString(@"d\.hh\:mm\:ss"))));
                }
            }
            using (var reader = Utils.QueryReader("select Commands from PlayerList where name = @0", name))
            {
                if (reader.Read() && !reader.Reader.IsDBNull(0))
                {
                    foreach (var perm in reader.Reader.GetString(0).Split(",", StringSplitOptions.RemoveEmptyEntries))
                    {
                        list.Add(new PermInfo(perm, "永久"));
                    }
                }
            }
            result.Add("Permissions", list);
        }
        else
        {
            var list = new List<TitleInfo>() { };
            var isPrefix = type == nameof(CustomPlayer.Prefix);
            var typeChinese = isPrefix ? "前缀" : "后缀";
            using var reader = isPrefix ? Utils.PrefixQuery(name) : Utils.SuffixQuery(name);
            if (reader.Read())
            {
                reader.Reader.DoForEach(x => list.Add(new TitleInfo(x.GetString("Value"), x.GetInt32("Id"), x.GetString("DurationText") == "-1" ? "永久" : (x.GetDateTime("EndTime") - DateTime.Now).ToString(@"d\.hh\:mm\:ss"))));
            }
            result.Add(type + "s", list);
        }
        return result;
    }
    private object RestWear(RestRequestArgs args, string type)
    {
        var result = new RestObject("200");
        var name = args.Parameters["name"];
        var isPrefix = type == nameof(CustomPlayer.Prefix);
        var typeChinese = isPrefix ? "前缀" : "后缀";
        if (!int.TryParse(args.Parameters["id"], out var titleId))
        {
            return this.SetError(result, $"转换{typeChinese}Id失败");
        }

        if (!int.TryParse(args.Parameters["serverid"], out var serverId))
        {
            return this.SetError(result, "转换ServerId失败");
        }

        using (var reader = Utils.TitleQuery(type, name, titleId))
        {
            if (reader.Read())
            {
                using (var usingReader = Utils.QueryReader("select PrefixId,SuffixId from Useing where Name = @0 and ServerId = @1", name, serverId))
                {
                    if (usingReader.Read())
                    {
                        Utils.Query($"update Useing set {type}Id = @0 where ServerId = @1", titleId, serverId);
                    }
                    else
                    {
                        Utils.Query($"insert into Useing(Name,ServerId,{type}Id) values(@0,@1,@2)", name, serverId, titleId);
                    }
                }
                result.Response = $"已为玩家:{name} 佩戴{typeChinese}Id:{titleId} 到服务器:{serverId}";
            }
            else
            {
                this.SetError(result, $"玩家:{name} {typeChinese}Id:{titleId} 未找到");
            }
        }
        return result;
    }
    private RestObject SetError(RestObject obj, string error)
    {
        obj.Status = "400";
        obj.Error = error;
        return obj;
    }
    public class PermInfo
    {
        public string Perm, Time;
        public PermInfo(string perm, string time)
        {
            this.Perm = perm;
            this.Time = time;
        }
    }
    public class TitleInfo
    {
        public string Title, Time;
        public int TitleId;
        public TitleInfo(string title, int titleId, string time)
        {
            this.Title = title;
            this.TitleId = titleId;
            this.Time = time;
        }
    }
}