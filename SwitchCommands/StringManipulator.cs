using System.Collections.Generic;
using System.Linq;
using TShockAPI;

namespace SwitchCommands;

public static class StringManipulator
{
	public static string ReplaceTags(this string s, TSPlayer player)
	{
		List<string> list = s.Split(' ').ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] == "$name")
			{
				list[num] = "\"" + player.Name + "\"";
			}
		}
		return string.Join(" ", list);
	}
}
