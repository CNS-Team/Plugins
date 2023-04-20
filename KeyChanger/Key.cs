using Terraria;
using TShockAPI.DB;

namespace KeyChanger;

public class Key
{
    public string Name { get; set; }

    public KeyTypes Type { get; set; }

    public Region Region { get; set; }

    public bool Enabled { get; set; }

    public List<Item> Items { get; set; }

    public Key()
    {
        this.Items = new List<Item>();
    }

    public Key(string name, KeyTypes type, bool enabled)
        : this()
    {
        this.Name = name;
        this.Type = type;
        this.Enabled = enabled;
    }
}