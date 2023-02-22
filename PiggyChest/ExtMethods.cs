using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiggyChest;

public static class ExtMethods
{
    public static string ToText(this List<ItemInfo> items)
    {
        return string.Join('\n', items);
    }
    public static void EnsureDirectoryExists(this string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
