using CustomPlayer.ModfiyGroup;
using System.Data;
using TShockAPI;

namespace CustomPlayer;
public static class CustomPlayerPluginHelpers
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public static GroupManager Groups;
    public static CustomPlayer[] Players = new CustomPlayer[255];
    public static IDbConnection DB;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public static List<TimeOutObject> TimeOutList = new List<TimeOutObject>();
    public static Dictionary<string, int> GroupGrade = new Dictionary<string, int>();
    public static void GroupLevelSet()
    {
        List<Group> tempGroups = new(Groups.groups);
        while (tempGroups.Count > 0)
        {
            var s = 0;
            var count = tempGroups.Count;
            for (var i = 0; i < count; i++)
            {
                var group = tempGroups[i - s];
                if (group.Parent is null)
                {
                    GroupGrade.Add(group.Name, 0);
                    tempGroups.RemoveAt(i - s);
                    s++;
                }
                else
                {
                    if (GroupGrade.TryGetValue(group.ParentName, out var value))
                    {
                        GroupGrade.TryAdd(group.Name, value + 1);
                        tempGroups.RemoveAt(i - s);
                        s++;
                    }
                }
            }
        }
    }
}