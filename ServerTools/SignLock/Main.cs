using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.SignLock;
internal class Main
{
    public static void OnSignEdit(object _, GetDataHandlers.SignEventArgs e)
    {
        if (!e.Player.HasBuildPermission(e.X, e.Y, true))
        {
            e.Handled = true;
            e.Player.SendErrorMessage("你无权修改受保护告示牌的内容");
            TSPlayer.All.SendData(PacketTypes.SignNew, "", e.ID, 0f, 0f, 0f, 0);
        }
    }
}
