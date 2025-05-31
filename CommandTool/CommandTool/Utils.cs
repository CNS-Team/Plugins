using System;
using System.Collections.Generic;
using TShockAPI;

namespace CommandTool;

internal static class Utils
{
    public static string ReplaceTags(this string s, TSPlayer player)
    {
        string[] array = s.Split(' ');
        for (var num = array.Length - 1; num >= 0; num--)
        {
            if (array[num] == "$name")
            {
                array[num] = "\"" + player.Name + "\"";
            }
        }

        return string.Join(' ', array);
    }

    public static SwitchState GetSiwtchState(this TSPlayer player)
    {
        return player.GetData<SwitchState>("SwitchState");
    }

    public static void SetSwitchState(this TSPlayer player, SwitchState state)
    {
        player.SetData("SwitchState", state);
    }

    public static SwitchCommandInfo? GetSwitchCommandInfo(this TSPlayer player)
    {
        return player.GetData<SwitchCommandInfo>("SwitchCommandInfo");
    }

    public static void SetSwitchCommandInfo(this TSPlayer player, SwitchCommandInfo commandInfo)
    {
        player.SetData("SwitchCommandInfo", commandInfo);
    }

    public static SwitchPos? GetSwitchPos(this TSPlayer player)
    {
        return player.GetData<SwitchPos>("SwitchPos");
    }

    public static void SetSwitchPos(this TSPlayer player, SwitchPos switchPos)
    {
        player.SetData("SwitchPos", switchPos);
    }

    public static Dictionary<string, DateTime>? GetSwitchCooding(this TSPlayer player)
    {
        return player.GetData<Dictionary<string, DateTime>>("SwitchCooding");
    }

    public static void SetSwitchCooding(this TSPlayer player, Dictionary<string, DateTime> coodingInfo)
    {
        player.SetData("SwitchCooding", coodingInfo);
    }
}