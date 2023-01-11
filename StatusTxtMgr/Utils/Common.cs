using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace StatusTxtMgr.Utils
{
    internal static class Common
    {
        public static IEnumerable<TSPlayer> PlayersOnline => from p in TShock.Players where p is { Active: true } select p;

        public static StringBuilder AcquirePlrSB(this StringBuilder?[] sbs, TSPlayer tsplr)
        {
            var plrId = tsplr.Index;
            var sb = sbs[plrId];
            if (sb == null)
            {
                sb = new StringBuilder();
                sbs[plrId] = sb;
                return sb;
            }
            return sb.Clear();
        }

        public static ulong TickCount = 0;
        public static void CountTick() => TickCount++;
        public static void ClearTickCount() => TickCount = 0;
    }
}
