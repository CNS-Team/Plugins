using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace ItemMonitor
{
    internal static class Extensions
    {
        public static ItemValidator GetValidator(this TSPlayer player)
        {
            return player.GetData<ItemValidator>("ItemValidator");
        }
    }
}