using Terraria;
using Terraria.ID;
using TShockAPI;

namespace ItemMonitor;

internal class ItemHolder : Dictionary<int, int>, IEquatable<ItemHolder>
{
    public bool Equals(ItemHolder other)
    {
        foreach (var item in this)
        {
            if (other.TryGetValue(item.Key, out var num))
            {
                if (num != item.Value)
                {
                    return false;
                }
            }
            else if (item.Value != 0)
            {
                return false;
            }
        }

        foreach (var item in other)
        {
            if (item.Value != 0 && !this.ContainsKey(item.Key))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = -170788016;
        hashCode = (hashCode * -1521134295) + EqualityComparer<KeyCollection>.Default.GetHashCode(this.Keys);
        hashCode = (hashCode * -1521134295) + EqualityComparer<ValueCollection>.Default.GetHashCode(this.Values);
        return hashCode;
    }

    public override bool Equals(object? obj)
    {
        return obj is ItemHolder holder && this.Equals(holder);
    }

    public static bool operator ==(ItemHolder a, ItemHolder b) => a.Equals(b);
    public static bool operator !=(ItemHolder a, ItemHolder b) => !(a == b);

    private static string GetItemName(int netId)
    {
        var item = new Item();
        item.netDefaults(netId);
        return item.Name;
    }

    public override string ToString()
    {
        return $"({string.Join(", ", this.Where(pair => pair.Value != 0).OrderBy(pair => (pair.Key > 70 && pair.Key < 75 ? 10000 : 0) + pair.Key).Select(item => $"{GetItemName(item.Key)}({item.Key})x{item.Value}"))})";
    }

    public void BalanceMoney()
    {
        var money = 0;
        if (this.TryGetValue(74, out var val))
        {
            money += val * 1000000;
        }

        if (this.TryGetValue(73, out val))
        {
            money += val * 10000;
        }

        if (this.TryGetValue(72, out val))
        {
            money += val * 100;
        }

        if (this.TryGetValue(71, out val))
        {
            money += val * 1;
        }

        this[74] = money / 1000000;
        this[73] = money / 10000 % 100;
        this[72] = money / 100 % 100;
        this[71] = money % 100;
    }

    public static readonly ItemHolder Empty = new ItemHolder();
    public bool IsEmpty()
    {
        return this == Empty;
    }
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
        var itemToPickUp = new Item();
        itemToPickUp.netDefaults(type);
        return itemToPickUp.IsAir || ItemID.Sets.NebulaPickup[itemToPickUp.type]
            || itemToPickUp.type == ItemID.Heart
            || itemToPickUp.type == ItemID.CandyApple
            || itemToPickUp.type == ItemID.CandyCane
            || itemToPickUp.type == ItemID.Star
            || itemToPickUp.type == ItemID.SoulCake
            || itemToPickUp.type == ItemID.SugarPlum
            || itemToPickUp.type == ItemID.ManaCloakStar;
    }

    public void AddItem(int item, int num)
    {
        if (IsIgnore(item))
        {
            return;
        }

        if (!this.items.ContainsKey(item))
        {
            this.items[item] = 0;
        }

        this.items[item] += num;
        this.count = 0;
    }

    private void Settle()
    {
        this.items.BalanceMoney();
        if (!this.items.IsEmpty())
        {
            TShock.Log.Info($"item imbalance detected for {this.player.Name}: {this.items}");
        }
        this.items.Clear();
    }

    public void Update()
    {
        if (++this.count == settle_count)
        {
            this.Settle();
            this.count = 0;
        }
    }

    public void Dispose()
    {
        this.Settle();
    }
}