using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Nets
{
    public class NetInventory
    {
        private readonly NetInventorySlot[] slots;
        public GamePlayer Player { get; }
        public int Count => this.slots.Length;
        public NetInventory(GamePlayer player)
        {
            this.Player = player;
            this.slots = new NetInventorySlot[player.TPlayer.inventory.Length];
            for (var i = 0; i < this.slots.Length; i++)
            {
                this.slots[i] = new NetInventorySlot(player.Index, i);
            }
        }
        public NetInventorySlot this[int slot] => this.slots[slot];
    }
}