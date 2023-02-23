using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Nets
{
    public class NetPiggybank
    {
        private readonly NetPiggySlot[] slots;
        public GamePlayer Player { get; }
        public int Count => this.slots.Length;
        public NetPiggySlot this[int slot] => this.slots[slot];
        public NetPiggybank(GamePlayer player)
        {
            this.Player = player;
            this.slots = new NetPiggySlot[player.TPlayer.bank.item.Length];
            for (var i = 0; i < this.slots.Length; i++)
            {
                this.slots[i] = new NetPiggySlot(player.Index, i);
            }
        }
        public void Clear()
        {
            for (var i = 0; i < this.Count; i++)
            {
                this[i].ToAir();
            }
        }

    }
}