using Terraria;
using TShockAPI;

namespace KeyChanger;

internal class Utils
{
    public static Key LoadKey(KeyTypes type)
    {
        Key key;
        switch (type)
        {
            case KeyTypes.Jungle:
                key = new Key("jungle", KeyTypes.Jungle, KeyChanger.Config.EnableJungleKey)
                {
                    Items = GetItems(KeyChanger.Config.JungleKeyItem),
                    Region = TShock.Regions.GetRegionByName(KeyChanger.Config.JungleRegion)
                };
                break;
            case KeyTypes.Corruption:
                key = new Key("corruption", KeyTypes.Corruption, KeyChanger.Config.EnableCorruptionKey)
                {
                    Items = GetItems(KeyChanger.Config.CorruptionKeyItem),
                    Region = TShock.Regions.GetRegionByName(KeyChanger.Config.CorruptionRegion)
                };
                break;
            case KeyTypes.Crimson:
                key = new Key("crimson", KeyTypes.Crimson, KeyChanger.Config.EnableCrimsonKey)
                {
                    Items = GetItems(KeyChanger.Config.CrimsonKeyItem),
                    Region = TShock.Regions.GetRegionByName(KeyChanger.Config.CrimsonRegion)
                };
                break;
            case KeyTypes.Hallowed:
                key = new Key("hallowed", KeyTypes.Hallowed, KeyChanger.Config.EnableHallowedKey)
                {
                    Items = GetItems(KeyChanger.Config.HallowedKeyItem),
                    Region = TShock.Regions.GetRegionByName(KeyChanger.Config.HallowedRegion)
                };
                break;
            case KeyTypes.Frozen:
                key = new Key("frozen", KeyTypes.Frozen, KeyChanger.Config.EnableFrozenKey)
                {
                    Items = GetItems(KeyChanger.Config.FrozenKeyItem),
                    Region = TShock.Regions.GetRegionByName(KeyChanger.Config.FrozenRegion)
                };
                break;
            default:
                return null;
            case KeyTypes.Desert:
                key = new Key("desert", KeyTypes.Desert, KeyChanger.Config.EnableDesertKey)
                {
                    Items = GetItems(KeyChanger.Config.DesertKeyItem),
                    Region = TShock.Regions.GetRegionByName(KeyChanger.Config.DesertRegion)
                };
                break;
            case KeyTypes.Temple:
                key = new Key("temple", KeyTypes.Temple, KeyChanger.Config.EnableTempleKey)
                {
                    Items = GetItems(KeyChanger.Config.TempleKeyItem),
                    Region = TShock.Regions.GetRegionByName(KeyChanger.Config.TempleRegion)
                };
                break;
        }
        return key;
    }

    public static List<Item> GetItems(int[] id)
    {
        var list = new List<Item>();
        foreach (var num in id)
        {
            list.Add(TShock.Utils.GetItemById(num));
        }
        return list;
    }

    public static string ErrorMessage(TSPlayer ply)
    {
        var list = new List<string>
        {
            ply.HasPermission("key.change") ? "change" : null,
            ply.HasPermission("key.reload") ? "reload" : null,
            ply.HasPermission("key.mode") ? "mode" : null,
            "list"
        };
        var value = string.Join("/", list.FindAll((string i) => i != null));
        return $"格式错误! 正确格式为: {Commands.Specifier}key <{value}> [type]";
    }
}