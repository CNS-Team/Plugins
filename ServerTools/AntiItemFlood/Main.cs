using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.AntiItemFlood;
public class Main
{
    private static readonly Dictionary<int, int> Multiples = new Dictionary<int, int>();

    public static void OnItemDrop(object? sender, GetDataHandlers.ItemDropEventArgs e)
    {
        var position = e.Position;
        if (e.ID == 400 && TShockAPI.Utils.Distance(position, e.Player.TPlayer.Center + e.Player.TPlayer.velocity) >= 48f)
        {
            var val = (from p in TShock.Players
                       where p != null
                       orderby p.TPlayer.Distance(e.Position)
                       select p).ElementAt(0);
            var value = TShockAPI.Utils.Distance(position, val.TPlayer.Center) / 8f;
            if (val != e.Player)
            {
                TSPlayer.All.SendMessage($"{e.Player.Name}在{val.Name}附近({value}格)生成了物品:[i:{e.Type}]，两人实际距离为{e.Player.TPlayer.Distance(val.TPlayer.Center) / 8f}格", Microsoft.Xna.Framework.Color.LightGoldenrodYellow);
            }
            AddCount(e.Player.Index);
            TShock.Log.Info($"警告:玩家{e.Player.Name}疑似使用物品洪水攻击,物品为{e.Type},坐标为{e.Position},受害者可能是{val.Name}(生成点在距离{value}格处)");
            e.Handled = true;
        }
    }

    private static void AddCount(int index)
    {
        if (Plugin.Config.Multiplekickout)
        {
            Multiples[index]++;
            if (Multiples[index] >= Plugin.Config.MultipleCount)
            {
                TShock.Players[index].Kick(TShock.Players[index].Name + "疑似使用物品洪水攻击,已踢出", true, false, string.Empty, false);
            }
        }
    }
}
