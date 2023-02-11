using System.Text;

using TShockAPI;

using VBY.Basic.Command;
using VBY.Basic.Extension;

using static CustomPlayer.TableInfo;

namespace CustomPlayer;
public partial class CustomPlayerPlugin
{
    private static bool TimeParse(bool forever, string time, ref DateTime startTime, ref DateTime endTime, ref TimeSpan addTime, TSPlayer player)
    {
        if (forever)
        {
            endTime = startTime;
        }
        else if (time == "get")
        {
            if (TestObject.Time.HasValue)
            {
                addTime = TestObject.Time.Value;
                endTime = startTime.Add(TestObject.Time.Value);
            }
            else
            {
                player.SendErrorMessage($"Time.Test 未设置过值");
                return true;
            }
        }
        else if (TimeSpan.TryParse(time, out addTime))
        {
            endTime = startTime.Add(addTime);
        }
        else
        {
            player.SendErrorMessage($"转换 {time} 为 TimeSpan 失败");
            return true;
        }
        return false;
    }
    private static bool TitleParse(string type, ref string title, TSPlayer player)
    {
        if (title == "get")
        {
            switch (type)
            {
                case nameof(CustomPlayer.Prefix):
                {
                    if (string.IsNullOrEmpty(TestObject.Prefix))
                    {
                        player.SendErrorMessage("Prefix.Test 未设置有效值");
                        return true;
                    }
                    else
                    {
                        title = TestObject.Prefix;
                    }

                    break;
                }

                case nameof(CustomPlayer.Suffix):
                {
                    if (string.IsNullOrEmpty(TestObject.Suffix))
                    {
                        player.SendErrorMessage("Suffix.Test 未设置有效值");
                        return true;
                    }
                    else
                    {
                        title = TestObject.Suffix;
                    }

                    break;
                }
            }
        }
        return false;
    }
    private static void TitleList(ref SubCmdArgs args, string type)
    {
        var player = args.commandArgs.Player;
        var cply = player.GetPlayer();
        var list = type == "前缀" ? cply.PrefixList : cply.SuffixList;
        player.SendInfoMessage($"你的所有{type}({list.Count})");
        foreach (var title in list)
        {
            player.SendInfoMessage($"ID:{title.Id} 内容:{title.Value} 剩余时间:{(title.DurationText == "-1" ? "永久" : (DateTime.Now - title.EndTime).ToString())}");
        }
    }
    private static void TitleWear(ref SubCmdArgs args, string type)
    {
        var player = args.commandArgs.Player;
        if (!int.TryParse(args.Parameters[0], out var id))
        {
            player.SendErrorMessage("转换Id失败");
            return;
        }

        var isPrefix = type == nameof(CustomPlayer.Prefix);
        var typeChinese = isPrefix ? "前缀" : "后缀";
        var clear = id == -1;
        var cply = player.GetPlayer();
        var list = isPrefix ? cply.PrefixList : cply.SuffixList;
        if (clear)
        {
            if (isPrefix)
            {
                cply.Prefix = null;
            }
            else
            {
                cply.Suffix = null;
            }

            player.SendSuccessMessage($"佩戴{typeChinese}已清除");
        }
        else
        {
            var fTitle = list.Find(x => x.Id == id);
            if (fTitle == null)
            {
                player.SendInfoMessage($"{typeChinese}ID:{id} 未找到");
                return;
            }
            else
            {
                if (isPrefix)
                {
                    cply.Prefix = fTitle.Value;
                }
                else
                {
                    cply.Suffix = fTitle.Value;
                }

                player.SendSuccessMessage($"{typeChinese}ID:{id} 已佩戴");
            }
        }
        Utils.Query($"update Useing set {type}Id = @0 where ServerId = @1", id, ReadConfig.Root.ServerId);
    }
    private static void CtlTitleList(ref SubCmdArgs args, string type)
    {
        var player = args.commandArgs.Player;
        var name = args.Parameters[0];
        var isPrefix = type == nameof(CustomPlayer.Prefix);
        var typeChinese = isPrefix ? "前缀" : "后缀";
        using var reader = isPrefix ? Utils.PrefixQuery(name) : Utils.SuffixQuery(name);
        if (reader.Read())
        {
            player.SendInfoMessage($"玩家:{name} 的{typeChinese}");
            reader.Reader.DoForEach(x => player.SendInfoMessage($"{typeChinese}:{x.GetString("Value")} Id:{x.GetInt32("Id")} 剩余时间:{(x.GetString("DurationText") == "-1" ? "永久" : (x.GetDateTime("EndTime") - DateTime.Now).ToString(@"d\.hh\:mm\:ss"))}"));
        }
        else
        {
            player.SendInfoMessage($"没有找到此玩家的{typeChinese}");
        }
    }
    private static void CtlTitleAdd(ref SubCmdArgs args, string type)
    {
        var name = args.Parameters[0];
        var title = args.Parameters[1].Replace(";", "");
        var time = args.Parameters[2];
        var player = args.commandArgs.Player;
        var startTime = DateTime.Now;
        DateTime endTime = new();
        TimeSpan addTime = new();
        var forever = time == "-1";
        var addId = 0;
        var isPrefix = type == nameof(CustomPlayer.Prefix);
        var typeChinese = isPrefix ? "前缀" : "后缀";

        if (TimeParse(forever, time, ref startTime, ref endTime, ref addTime, player))
        {
            return;
        }

        if (TitleParse(type, ref title, player))
        {
            return;
        }

        var cply = Utils.FindPlayer(name);
        if (cply != null)
        {
            if (forever)
            {
                cply.Player.SendInfoMessage($"你已获得永久{typeChinese}:{title}");
            }
            else
            {
                cply.Player.SendInfoMessage($"你获得{typeChinese}:{title}");
            }

            CustomPlayerPluginHelpers.TimeOutList.Add(new TimeOutObject(name, title, type, startTime, endTime, time));
            var list = isPrefix ? cply.PrefixList : cply.SuffixList;
            list.Add(new Prefix(name, addId, title, startTime, startTime, time));
        }

        using (var maxReader = Utils.QueryReader($"select max(Id) from {type} where Name = @0", name))
        {
            if (maxReader.Read() && !maxReader.Reader.IsDBNull(0))
            {
                addId = maxReader.Reader.GetInt32(0) + 1;
            }
        }
        Utils.Query($"insert into {type}(Name,Id,Value,StartTime,EndTime,DurationText) values(@0,@1,@2,@3,@4,@5)", name, addId, title, startTime, endTime, time);
        if (forever)
        {
            player.SendInfoMessage($"添加成功 玩家:{name} {typeChinese}:{title} Id:{addId} 持续时间:永久");
        }
        else
        {
            player.SendInfoMessage($"添加成功 玩家:{name} {typeChinese}:{title} Id:{addId} 起始时间:{startTime} 结束时间:{endTime} 持续时间:{addTime}");
        }
    }
    private static void CtlTitleDel(ref SubCmdArgs args, string type)
    {
        var name = args.Parameters[0];
        var delValue = args.Parameters[1];
        var player = args.commandArgs.Player;
        var isPrefix = type == nameof(CustomPlayer.Prefix);
        var typeChinese = isPrefix ? "前缀" : "后缀";
        if (int.TryParse(delValue, out var delId) && Utils.Query($"delete from {type} where Name = @0 AND Id = @1", name, delId) > 0)
        {
            player.SendSuccessMessage($"删除玩家[{name}] {typeChinese}Id:{delId} 成功");
        }
        else if (Utils.Query($"delete from {type} where Name = @0 AND Value = @1", name, delValue) > 0)
        {
            player.SendSuccessMessage($"删除玩家:[{name}] {typeChinese}Value:{delValue} 成功");
        }
        else
        {
            player.SendErrorMessage("删除失败,没有找到");
        }
    }
    private static void CtlTitleTest(ref SubCmdArgs args, string type)
    {
        var cply = args.commandArgs.Player.GetPlayer();
        if (type == "Prefix")
        {
            cply.Prefix = args.Parameters[0];
            TestObject.Prefix = args.Parameters[0];
        }
        else
        {
            cply.Suffix = args.Parameters[0];
            TestObject.Suffix = args.Parameters[0];
        }
    }
    private static void CtlTitleWear(ref SubCmdArgs args, string type)
    {
        var player = args.commandArgs.Player;
        var name = args.Parameters[0];
        var isPrefix = type == nameof(CustomPlayer.Prefix);
        var typeChinese = isPrefix ? "前缀" : "后缀";
        if (!int.TryParse(args.Parameters[1], out var titleId))
        {
            player.SendErrorMessage($"转换{typeChinese}Id失败");
            return;
        }
        if (!int.TryParse(args.Parameters[2], out var serverId))
        {
            player.SendErrorMessage("转换ServerId失败");
            return;
        }

        using var reader = Utils.TitleQuery(type, name, titleId);
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
            player.SendSuccessMessage($"已为玩家:{name} 佩戴{typeChinese}Id:{titleId} 到服务器:{serverId}");
        }
        else
        {
            player.SendErrorMessage($"玩家:{name} {typeChinese}Id:{titleId} 未找到");
        }
    }
}