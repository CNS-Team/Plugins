namespace DataSync;

[AttributeUsage(AttributeTargets.Field)]
public class AliasAttribute : Attribute
{
    public string[] Alias { get; }

    public AliasAttribute(params string[] alias)
    {
        this.Alias = alias;
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
        this.NPCID = match;
    }

    public MatchAttribute(string key, object value)
    {
        this.NPCID = Array.Empty<int>();
        this.Key = key;
        this.Value = value;
    }
}