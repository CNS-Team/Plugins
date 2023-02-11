using CustomPlayer.ModfiyGroup;
using System.Data;
using TShockAPI;

namespace CustomPlayer;
public static class CustomPlayerPluginHelpers
{
    public static GroupManager? Groups;
    public static CustomPlayer[] Players = new CustomPlayer[255];
    public static IDbConnection? DB;
    public static List<TimeOutObject> TimeOutList = new List<TimeOutObject>();
    public static Dictionary<string, int> GroupGrade = new Dictionary<string, int>();
    public static void GroupLevelSet()
    {
        _ = new Dictionary<string, string>();
        var tempGroups = new List<Group>(Groups.groups);
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
        //foreach(var keyValue in GroupGrade)
        //{
        //    Console.WriteLine("{0} {1}", keyValue.Key, keyValue.Value);
        //}
        //Console.ReadKey();
    }
}