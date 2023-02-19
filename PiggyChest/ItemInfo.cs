using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;

namespace PiggyChest;

public struct ItemInfo
{
    public int ID { get; set; }
    public short Stack { get; set; }
    public byte Prefix { get; set; }
    public ItemInfo(int id, int stack = 0, int prefix = 0)
    {
        this.ID = id;
        checked
        {
            this.Stack = (short) stack;
            this.Prefix = (byte) prefix;
        }
    }

    public Item ToItem()
    {
        var item = new Item();
        item.SetDefaults(this.ID);
        item.stack = this.Stack;
        item.prefix = this.Prefix;
        return item;
    }
    public override string ToString()
    {
        return $"{this.ID}:{this.Stack}:{this.Prefix}";
    }

    public static implicit operator ItemInfo(Item item) => item.IsAir ? default : new ItemInfo(item.type, item.stack, item.prefix);

    public static implicit operator ItemInfo((int id, int stack, int prefix) tuple) => new ItemInfo(tuple.id, tuple.stack, tuple.prefix);
    public static implicit operator ItemInfo((int id, int stack) tuple) => new ItemInfo(tuple.id, tuple.stack);

    public static bool operator==(in ItemInfo l,in ItemInfo r)
    {
        return
            l.ID == r.ID &&
            l.Stack == r.Stack &&
            l.Prefix == r.Prefix;
    }
    public static bool operator !=(in ItemInfo l, in ItemInfo r)
    {
        return
            l.ID != r.ID ||
            l.Stack != r.Stack ||
            l.Prefix != r.Prefix;
    }


    public static ItemInfo Parse(string text)
    {
        var values = text.Split(':').Select(s => int.Parse(s)).ToArray();
        return (values[0], values[1], values[2]);
    }
    public static List<ItemInfo> ParseList(string text)
    {
        if(text.Length == 0)
        {
            return new List<ItemInfo>();
        }
        return text
            .Split('\n')
            .Select(s => Parse(s))
            .ToList();
    }
}
