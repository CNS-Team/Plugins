using Newtonsoft.Json;
using TShockAPI;

namespace DataSync;

[AttributeUsage(AttributeTargets.Field)]
public class AliasAttribute : Attribute
{
    public string[] Alias { get; }

    public AliasAttribute(params string[] alias)
    {
        Alias = alias;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class MatchAttribute : Attribute
{
    public int[] NPCID { get; }

    public string? Key { get; }
    public object? Value { get; }

    public MatchAttribute(params int[] match)
    {
        NPCID = match;
    }

    public MatchAttribute(string key, object value)
    {
        NPCID = Array.Empty<int>();
        Key = key;
        Value = value;
    }
}