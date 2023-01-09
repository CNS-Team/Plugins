using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace ItemMonitor
{
    internal class ItemHolder : Dictionary<int, int>, IEquatable<ItemHolder>
    {
        public bool Equals(ItemHolder other)
        {
            foreach (var item in this)
            {
                if (other.TryGetValue(item.Key, out var num))
                {
                    if (num != item.Value) return false;
                }
                else if (item.Value != 0) return false;
            }

            foreach (var item in other)
                if (item.Value != 0 && !ContainsKey(item.Key)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = -170788016;
            hashCode = hashCode * -1521134295 + EqualityComparer<KeyCollection>.Default.GetHashCode(Keys);
            hashCode = hashCode * -1521134295 + EqualityComparer<ValueCollection>.Default.GetHashCode(Values);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemHolder holder && Equals(holder);
        }

        public static bool operator ==(ItemHolder a, ItemHolder b) => a.Equals(b);
        public static bool operator !=(ItemHolder a, ItemHolder b) => !(a == b);

        private static string GetItemName(int netId)
        {
            Item item = new Item();
            item.netDefaults(netId);
            return item.Name;
        }

        public override string ToString()
        {
            return $"({string.Join(", ", this.Where(pair => pair.Value != 0).OrderBy(pair => (pair.Key > 70 && pair.Key < 75 ? 10000 : 0) + pair.Key).Select(item => $"{GetItemName(item.Key)}({item.Key})x{item.Value}"))})";
        }

        public void BalanceMoney()
        {
            int money = 0;
            if (TryGetValue(74, out var val)) money += val * 1000000;
            if (TryGetValue(73, out val)) money += val * 10000;
            if (TryGetValue(72, out val)) money += val * 100;
            if (TryGetValue(71, out val)) money += val * 1;

            this[74] = money / 1000000;
            this[73] = (money / 10000) % 100;
            this[72] = (money / 100) % 100;
            this[71] = (money) % 100;
        }

        public static readonly ItemHolder Empty = new ItemHolder();
        public bool IsEmpty() => this == Empty;
    }

    internal sealed class ItemValidator : IDisposable
    {
        private readonly ItemHolder items = new ItemHolder();
        private const int settle_count = 180;
        private int count = 0;
        private readonly TSPlayer player;

        public ItemValidator(TSPlayer player)
        {
            this.player = player;
        }

        private static bool IsIgnore(int type)
        {
            Item itemToPickUp = new Item();
            itemToPickUp.netDefaults(type);
            if (itemToPickUp.IsAir || ItemID.Sets.NebulaPickup[itemToPickUp.type]) return true;
            if (itemToPickUp.type == 58 || itemToPickUp.type == 1734 || itemToPickUp.type == 1867) return true;
            if (itemToPickUp.type == 184 || itemToPickUp.type == 1735 || itemToPickUp.type == 1868) return true;
            if (itemToPickUp.type == 4143) return true;
            return false;
        }

        public void AddItem(int item, int num)
        {
            if (IsIgnore(item)) return;
            if (!items.ContainsKey(item)) items[item] = 0;
            items[item] += num;
            count = 0;
        }

        private void Settle()
        {
            items.BalanceMoney();
            if (!items.IsEmpty())
            {
                TShock.Log.Info($"item imbalance detected for {player.Name}: {items}");
            }
            items.Clear();
        }

        public void Update()
        {
            if (++count == settle_count)
            {
                Settle();
                count = 0;
            }
        }

        public void Dispose()
        {
            Settle();
        }
    }
}
