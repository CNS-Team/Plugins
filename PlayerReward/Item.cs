namespace PlayerReward;

internal class Item
{
    public int ID { get; set; }
    public int Stack { get; set; }
    public int Prefix { get; set; }

    public static Item Parse(string s)
    {
        var splits = s.Split('*');
        if (splits.Length != 3)
            throw new FormatException("wrong item string format");

        return new Item
        {
            ID = int.Parse(splits[0]),
            Stack = int.Parse(splits[1]),
            Prefix = int.Parse(splits[2])
        };
    }

    public override string ToString() => $"{ID}*{Stack}*{Prefix}";
}