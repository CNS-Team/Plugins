using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;


namespace SurvivalCrisis.Nets
{
    public struct NetGroundItem
    {
        private bool noAutoUpdate;

        public int Index
        {
            get;
        }
        public bool AutoUpdate
        {
            get => !this.noAutoUpdate;
            set => this.noAutoUpdate = !value;
        }

        public Item Item => Main.item[this.Index];

        public bool Active
        {
            get => this.Item.active && this.Stack > 0 && this.ID > 0;
            set
            {
                value &= this.Stack > 0 && this.ID > 0;
                this.Item.active = value;
                if (this.AutoUpdate)
                {
                    this.UpdateToClient();
                }
            }
        }
        public int ID
        {
            get => this.Item.type;
            set
            {
                this.Item.type = value;
                this.Active = value > 0;
                if (this.AutoUpdate)
                {
                    this.UpdateToClient();
                }
            }
        }
        public int Stack
        {
            get => this.Item.stack;
            set
            {
                this.Item.stack = value;
                this.Item.active = value > 0;
                if (this.AutoUpdate)
                {
                    this.UpdateToClient();
                }
            }
        }
        public byte Prefix
        {
            get => this.Item.prefix;
            set
            {
                this.Item.Prefix(value);
                if (this.AutoUpdate && this.Active)
                {
                    this.UpdateToClient();
                }
            }
        }

        public NetGroundItem(int index)
        {
            this.Index = index;
            this.noAutoUpdate = false;
        }

        public void UpdateToClient(int clientID = -1)
        {
            NetMessage.SendData((int) PacketTypes.UpdateItemDrop, clientID, -1, null, this.Index);
        }
    }
}