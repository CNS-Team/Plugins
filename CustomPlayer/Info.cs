using Microsoft.Xna.Framework;
using System.Text;
using TShockAPI;
using VBY.Basic.Extension;
using CP = CustomPlayer.CustomPlayerPlugin;

namespace CustomPlayer;
public class CustomPlayer
{
    public string Name;
    public TSPlayer Player;
    public Group? Group;
    public List<string> Permissions = new(), NegatedPermissions = new();
    public static bool 对接称号插件 { get; set; } = true;
    public string? Prefix
    {
        get => 对接称号插件 ? 称号插件.称号插件.称号信息[this.Name].前缀 : this.prefix;
        set
        {
            if (对接称号插件)
            {
                称号插件.称号插件.称号信息[this.Name].前缀 = value;
            }
            else
            {
                this.prefix = value;
            }
        }
    }
    private string? prefix;
    public string? Suffix
    {
        get => 对接称号插件 ? 称号插件.称号插件.称号信息[this.Name].后缀 : this.suffix;
        set
        {
            if (对接称号插件)
            {
                称号插件.称号插件.称号信息[this.Name].后缀 = value;
            }
            else
            {
                this.suffix = value;
            }
        }
    }
    private string? suffix;
    public Color? ChatColor
    {
        get => 对接称号插件 ? 称号插件.称号插件.称号信息[this.Name].颜色 : this.chatColor;
        set
        {
            if (对接称号插件)
            {
                称号插件.称号插件.称号信息[this.Name].颜色 = value;
            }
            else
            {
                this.chatColor = value;
            }
        }
    }
    private Color? chatColor;
    public List<TableInfo.Prefix> PrefixList = new(), SuffixList = new();
    public List<string> HaveGroupNames = new();
    public CustomPlayer(string name, TSPlayer player)
    {
        this.Name = name;
        this.Player = player;
    }
    public void AddPermission(string permission)
    {
        if (permission.StartsWith('!'))
        {
            this.NegatedPermissions.Add(permission[1..]);
        }
        else
        {
            this.Permissions.Add(permission);
        }
    }
    public bool DelPermission(string permission)
    {
        return permission.StartsWith('!') ? this.NegatedPermissions.Remove(permission[1..]) : this.Permissions.Remove(permission);
    }
    public static CustomPlayer Read(string name, TSPlayer player)
    {
        CustomPlayer cply = new(name, player);

        //I("select player");
        using (var playerReader = Utils.QueryPlayer(name))
        {
            if (playerReader.Read())
            {
                if (CP.ReadConfig.Root.EnablePermission)
                {
                    foreach (var x in playerReader.Reader.GetString(nameof(TableInfo.PlayerList.Commands)).Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        cply.AddPermission(x);
                    }
                }

                if (!playerReader.Reader.IsDBNull(2))
                {
                    var colorStrs = playerReader.Reader.GetString(nameof(TableInfo.PlayerList.ChatColor)).Split(',');
                    cply.ChatColor = new Color(int.Parse(colorStrs[0]), int.Parse(colorStrs[1]), int.Parse(colorStrs[2]));
                }
            }
        }

        //I("select perm");
        if (CP.ReadConfig.Root.EnablePermission)
        {
            using var timeoutReader = Utils.QueryReader("select Value,StartTime,EndTime,DurationText from ExpirationInfo where Type = 'Permission' AND Name = @0", name);
            timeoutReader.Reader.ForEach(x =>
            {
                var tobj = x.GetTimeOutObject(name, nameof(Permission));
                if (tobj.NoExpired)
                {
                    CustomPlayerPluginHelpers.TimeOutList.Add(tobj);
                    cply.AddPermission(tobj.Value);
                }
                else
                {
                    player.SendInfoMessage($"权限:{tobj.Value} 已过期");
                    tobj.Delete();
                }
            });
        }

        var converGroup = false;
        //I("select group");
        if (CP.ReadConfig.Root.EnableGroup)
        {
            using (var groupReader = Utils.QueryReader("select Value,StartTime,EndTime,DurationText from ExpirationInfo where Type = 'Group' AND Name = @0", name))
            {
                groupReader.Reader.ForEach(x =>
                {
                    var tobj = x.GetTimeOutObject(name, nameof(Group));
                    if (tobj.NoExpired)
                    {
                        cply.HaveGroupNames.Add(tobj.Value);
                        CustomPlayerPluginHelpers.TimeOutList.Add(tobj);
                        //Utils.I(tobj.Value);
                    }
                    else
                    {
                        player.SendInfoMessage($"组:{tobj.Value} 已过期");
                        tobj.Delete();
                    }
                });
            }
            Group? curGroup = null;
            List<string> noExistsName = new();
            foreach (var haveGroupName in cply.HaveGroupNames)
            {
                if (CustomPlayerPluginHelpers.Groups.GroupExists(haveGroupName))
                {
                    if (curGroup is null)
                    {
                        curGroup = CustomPlayerPluginHelpers.Groups.GetGroupByName(haveGroupName);
                    }
                    else
                    {
                        var newGroup = CustomPlayerPluginHelpers.Groups.GetGroupByName(haveGroupName);
                        if (CustomPlayerPluginHelpers.GroupGrade[curGroup.Name] < CustomPlayerPluginHelpers.GroupGrade[newGroup.Name])
                        {
                            curGroup = newGroup;
                        }
                    }

                }
                else
                {
                    player.SendErrorMessage("组:{0} 不存在", haveGroupName);
                    TSPlayer.Server.SendErrorMessage("[CustomPlayerPlugin]组:{0} 不存在", haveGroupName);
                    TShock.Log.Error("[CustomPlayerPlugin]组:{0} 不存在", haveGroupName);
                    noExistsName.Add(haveGroupName);
                }
            }
            cply.HaveGroupNames.RemoveRange(noExistsName);
            cply.Group = curGroup!;
            //TSPlayer.Server.SendInfoMessage(curGroup is null ? "无" : curGroup.Name);
            if (CP.ReadConfig.Root.CoverGroup && curGroup is not null)
            {
                cply.Group = player.Group;
                player.Group = curGroup;
                converGroup = true;
            }
        }

        //I("select prefix");
        if (CP.ReadConfig.Root.EnablePrefix)
        {
            using var prefixReader = Utils.PrefixQuery(name);
            prefixReader.Reader.ForEach(x =>
            {
                var tobj = new TimeOutObject(name, x.GetString(nameof(TableInfo.Prefix.Value)), nameof(Prefix), x.GetDateTime(nameof(TableInfo.Prefix.StartTime)), x.GetDateTime(nameof(TableInfo.Prefix.EndTime)), x.GetString(nameof(TableInfo.ExpirationInfo.DurationText)), x.GetInt32(nameof(TableInfo.Prefix.Id)));
                if (tobj.NoExpired)
                {
                    cply.PrefixList.Add(new TableInfo.Prefix(name, tobj.Id, tobj.Value, tobj.StartTime, tobj.EndTime, tobj.DurationText));
                    CustomPlayerPluginHelpers.TimeOutList.Add(tobj);
                }
                else
                {
                    player.SendInfoMessage($"前缀:{tobj.Value} 已过期");
                    tobj.Delete();
                }
            });
        }

        //I("select suffix");
        if (CP.ReadConfig.Root.EnableSuffix)
        {
            using var suffixReader = Utils.SuffixQuery(name);
            suffixReader.Reader.ForEach(x =>
            {
                var tobj = new TimeOutObject(name, x.GetString(nameof(TableInfo.Suffix.Value)), nameof(Suffix), x.GetDateTime(nameof(TableInfo.Suffix.StartTime)), x.GetDateTime(nameof(TableInfo.Suffix.EndTime)), x.GetString(nameof(TableInfo.Suffix.DurationText)), x.GetInt32(nameof(TableInfo.Suffix.Id)));
                if (tobj.NoExpired)
                {
                    cply.SuffixList.Add(new TableInfo.Prefix(name, tobj.Id, tobj.Value, tobj.StartTime, tobj.EndTime, tobj.DurationText));
                    CustomPlayerPluginHelpers.TimeOutList.Add(tobj);
                }
                else
                {
                    player.SendInfoMessage($"后缀:{tobj.Value} 已过期");
                    tobj.Delete();
                }
            });
        }

        //I("对接:{0}", 对接称号插件);
        if (对接称号插件)
        {
            称号插件.称号插件.称号信息[name] = new 称号插件.Config.聊天信息() { 前前缀 = "", 前缀 = null, 角色名 = null, 后缀 = null, 后后缀 = "" };
        }
        //I("select useing");
        using (var usingReader = Utils.QueryReader("select PrefixId,SuffixId from Useing where Name = @0 and ServerId = @1", name, CP.ReadConfig.Root.ServerId))
        {
            if (usingReader.Read())
            {
                cply.Prefix = cply.PrefixList.Find(x => x.Id == usingReader.Reader.GetInt32(nameof(TableInfo.Useing.PrefixId)))?.Value;
                cply.Suffix = cply.SuffixList.Find(x => x.Id == usingReader.Reader.GetInt32(nameof(TableInfo.Useing.SuffixId)))?.Value;
            }
            else
            {
                Utils.Query("insert into Useing(Name,ServerId,PrefixId,SuffixId) values(@0,@1,-1,-1)", name, CP.ReadConfig.Root.ServerId);
                //TSPlayer.Server.SendInfoMessage("insert data");
            }
        }
        Utils.I("return CustomPlayer:{0} {1}group:{2}", name, converGroup ? "conver" : "", converGroup ? player.Group.Name : cply.Group?.Name ?? "null");
        return cply;
    }
}

public class TimeOutObject
{
    public string Name, Value, Type, DurationText;
    public DateTime StartTime, EndTime;
    public bool TimeOuted = false;
    public int Id;
    public TimeOutObject(string name, string value, string type, DateTime startTime, DateTime endTime, string durationText, int id = -1)
    {
        this.Name = name;
        this.Value = value;
        this.Type = type;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.DurationText = durationText;
        this.Id = id;
    }
    public bool NoExpired => this.DurationText == "-1" || this.EndTime > DateTime.Now;
    public string RemainTime
    {
        get
        {
            if (this.DurationText == "-1")
            {
                return "无限";
            }
            else
            {
                var remainingTime = this.EndTime - DateTime.Now;
                return remainingTime < TimeSpan.Zero ? "已过期" : remainingTime.ToString(@"d\.hh\:mm\:ss");
            }
        }
    }
    public void Delete()
    {
        switch (this.Type)
        {
            case nameof(Permission):
            case nameof(Group):
            {
                Utils.Query("delete from ExpirationInfo where Name = @0 AND Value = @1 AND Type = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", this.Name, this.Value, this.Type, this.StartTime, this.EndTime, this.DurationText);
                //TSPlayer.Server.SendInfoMessage($"delete {Type}:{Value}");
                break;
            }
            case nameof(CustomPlayer.Prefix):
            case nameof(CustomPlayer.Suffix):
            {
                Utils.Query($"delete from {this.Type} where Name = @0 AND Value = @1 AND ID = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", this.Name, this.Value, this.Id, this.StartTime, this.EndTime, this.DurationText);
                //TSPlayer.Server.SendInfoMessage($"delete prefix:{Value}");
                break;
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class LengthAttribute : Attribute
{
    public int Length;
    public LengthAttribute(int length)
    {
        this.Length = length;
    }
}