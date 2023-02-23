using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SurvivalCrisis.Nets
{
    public class NetGroundItemCollection
    {
        private readonly NetGroundItem[] items;

        public int Count => this.items.Length;
        public NetGroundItem this[int index] => this.items[index];

        public NetGroundItemCollection()
        {
            this.items = new NetGroundItem[Main.maxItems];
            for (var i = 0; i < this.items.Length; i++)
            {
                this.items[i] = new NetGroundItem(i);
            }
        }

        public void Clear()
        {
            for (var i = 0; i < this.items.Length; i++)
            {
                this.items[i].Active = false;
            }
        }
        public void Clear(int itemID)
        {
            for (var i = 0; i < this.items.Length; i++)
            {
                if (this.items[i].ID == itemID)
                {
                    this.items[i].Active = false;
                }
            }
        }
    }
}