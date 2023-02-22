using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SurvivalCrisis.Nets
{
    using Item = Terraria.Item;
    public class NetPiggySlot
    {
        private readonly int? owner;

        public int? Slot { get; }

        public GamePlayer Owner => SurvivalCrisis.Instance.Players[(int) this.owner];
        public Item Item => this.Owner.TPlayer.bank.item[(int) this.Slot];
        public int MaxStack => this.Item.maxStack;

        public int Type
        {
            get => this.Item.type;
            set => this.SetItemType(value);
        }
        public int Stack
        {
            get => this.Item.stack;
            set => this.SetItemStack(value);
        }
        public byte Prefix
        {
            get => this.Item.prefix;
            set => this.SetItemPrefix(value);
        }

        public bool IsAir => this.Item.IsAir;

        public NetPiggySlot(int who, int slot)
        {
            this.owner = who;
            this.Slot = slot;
        }

        public void ToAir()
        {
            this.Type = 0;
        }
        public void SetDefaults(int type)
        {
            this.Item.SetDefaults(type);
            this.SendData();
        }

        #region Privates
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetItemPrefix(byte prefix)
        {
            this.Item.prefix = prefix;
            this.SendData();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetItemStack(int stack)
        {
            this.Item.stack = stack;
            this.SendData();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetItemType(int type)
        {
            this.Item.type = type;
            this.SendData();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendData()
        {
            this.Owner.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Owner.Index, (int) this.Slot + TShockAPI.NetItem.PiggyIndex.Item1);
        }
        #endregion
    }
}