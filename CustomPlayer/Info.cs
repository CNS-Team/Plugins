﻿global using static CustomPlayer.Utils;
using System.Text;
using Microsoft.Xna.Framework;

using TShockAPI;

using VBY.Basic.Extension;
using CP = CustomPlayer.CustomPlayerPlugin;

namespace CustomPlayer;
public class CustomPlayer
{
    public string Name;
    public TSPlayer Player;
    public Group Group;
    public List<string> Permissions = new(), NegatedPermissions = new();
    public static bool 对接称号插件 { get; set; } = true;
    public string? Prefix
    {
        get 
        { 
            if (对接称号插件)
                return 称号插件.称号插件.称号信息[Name].前缀;
            else
                return prefix;
        }
        set
        {
            if (对接称号插件)
                称号插件.称号插件.称号信息[Name].前缀 = value;
            else
                prefix = value;
        }
    }
    private string? prefix;
    public string? Suffix
    {
        get
        {
            if (对接称号插件)
                return 称号插件.称号插件.称号信息[Name].后缀;
            else
                return suffix;
        }
        set
        {
            if (对接称号插件)
                称号插件.称号插件.称号信息[Name].后缀 = value;
            else
                suffix = value;
        }
    }
    private string? suffix;
    public Color? ChatColor
    {
        get
        {
            if (对接称号插件)
                return 称号插件.称号插件.称号信息[Name].颜色;
            else
                return chatColor;
        }
        set
        {
            if (对接称号插件)
                称号插件.称号插件.称号信息[Name].颜色 = value;
            else
                chatColor = value;
        }
    }
    private Color? chatColor;
    public List<TableInfo.Prefix> PrefixList = new(), SuffixList = new();
    public List<string> HaveGroupNames = new();
    public CustomPlayer(string name, TSPlayer player)
    {
        Name = name;
        Player = player;
    }
    public void AddPermission(string permission)
    {
        if (permission.StartsWith('!'))
            NegatedPermissions.Add(permission[1..]);
        else
            Permissions.Add(permission);
    }
    public bool DelPermission(string permission)
    {
        if (permission.StartsWith('!'))
            return NegatedPermissions.Remove(permission[1..]);
        else
            return Permissions.Remove(permission);
    }
    public static CustomPlayer Read(string name, TSPlayer player)
    {
        CustomPlayer cply = new(name, player);

        //I("select player");
        using (var playerReader = QueryPlayer(name))
        {
            if (playerReader.Read())
            {
                if (CP.ReadConfig.Root.EnablePermission)
                {
                    foreach (var x in playerReader.Reader.GetString(nameof(TableInfo.PlayerList.Commands)).Split(',', StringSplitOptions.RemoveEmptyEntries))
                        cply.AddPermission(x);
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
            using var timeoutReader = QueryReader("select Value,StartTime,EndTime,DurationText from ExpirationInfo where Type = 'Permission' AND Name = @0", name);
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

        //I("select group");
        if (CP.ReadConfig.Root.EnableGroup)
        {
            using (var groupReader = QueryReader("select Value,StartTime,EndTime,DurationText from ExpirationInfo where Type = 'Group' AND Name = @0", name))
            {
                groupReader.Reader.ForEach(x =>
                {
                    var tobj = x.GetTimeOutObject(name, nameof(Group));
                    if (tobj.NoExpired)
                    {
                        cply.HaveGroupNames.Add(tobj.Value);
                        CustomPlayerPluginHelpers.TimeOutList.Add(tobj);
                        //TSPlayer.Server.SendInfoMessage(tobj.Value);
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
                        curGroup = CustomPlayerPluginHelpers.Groups.GetGroupByName(haveGroupName);
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
            }
        }

        //I("select prefix");
        if (CP.ReadConfig.Root.EnablePrefix)
        {
            using var prefixReader = PrefixQuery(name);
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
            using var suffixReader = SuffixQuery(name);
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
        using (var usingReader = QueryReader("select PrefixId,SuffixId from Useing where Name = @0 and ServerId = @1", name, CP.ReadConfig.Root.ServerId))
        {
            if (usingReader.Read())
            {
                cply.Prefix = cply.PrefixList.Find(x => x.Id == usingReader.Reader.GetInt32(nameof(TableInfo.Useing.PrefixId)))?.Value;
                cply.Suffix = cply.SuffixList.Find(x => x.Id == usingReader.Reader.GetInt32(nameof(TableInfo.Useing.SuffixId)))?.Value;
            }
            else
            {
                Query("insert into Useing(Name,ServerId,PrefixId,SuffixId) values(@0,@1,-1,-1)", name, CP.ReadConfig.Root.ServerId);
                //TSPlayer.Server.SendInfoMessage("insert data");
            }
        }
        I("return CustomPlayer:{0} group:{1}", name, cply.Group?.Name ?? "null");
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
        Name = name;
        Value = value;
        Type = type;
        StartTime = startTime;
        EndTime = endTime;
        DurationText = durationText;
        Id = id;
    }
    public bool NoExpired { get { return DurationText == "-1" || EndTime > DateTime.Now; } }
    public string RemainTime
    {
        get
        {
            if (DurationText == "-1")
                return "无限";
            else
            {
                var remainingTime = EndTime - DateTime.Now;
                if (remainingTime < TimeSpan.Zero)
                    return "已过期";
                else
                    return remainingTime.ToString(@"d\.hh\:mm\:ss");
            }
        }
    }
    public void Delete()
    {
        switch (Type)
        {
            case nameof(Permission):
            case nameof(Group):
                {
                    Query("delete from ExpirationInfo where Name = @0 AND Value = @1 AND Type = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", Name, Value, Type, StartTime, EndTime, DurationText);
                    //TSPlayer.Server.SendInfoMessage($"delete {Type}:{Value}");
                    break;
                }

            case nameof(CustomPlayer.Prefix):
                {
                    Query("delete from Prefix where Name = @0 AND Value = @1 AND ID = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", Name, Value, Id, StartTime, EndTime, DurationText);
                    //TSPlayer.Server.SendInfoMessage($"delete prefix:{Value}");
                    break;
                }

            case nameof(CustomPlayer.Suffix):
                {
                    Query("delete from Suffix where Name = @0 AND Value = @1 AND ID = @2 AND StartTime = @3 AND EndTime = @4 AND DurationText = @5", Name, Value, Id, StartTime, EndTime, DurationText);
                    //TSPlayer.Server.SendInfoMessage($"delete suffix:{Value}");
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
