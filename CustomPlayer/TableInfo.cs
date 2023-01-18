namespace CustomPlayer;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
public class TableInfo
{
    public class Prefix
    {
        public string Name;
        public int Id;
        public string Value;
        public DateTime StartTime, EndTime;
        public string DurationText;
        public Prefix(string name, int id, string value, DateTime startTime, DateTime endTime, string durationText)
        {
            Name = name;
            Id = id;
            Value = value;
            StartTime = startTime;
            EndTime = endTime;
            DurationText = durationText;
        }
    }
    public class Suffix : Prefix
    {
        public Suffix(string name, int id, string value, DateTime startTime, DateTime endTime, string durationText) : base(name, id, value, startTime, endTime, durationText)
        {
        }
    }
    public class Useing
    {
        public string Name;
        public int ServerId, PrefixId, SuffixId;
    }
    public class PlayerList
    {
        public string Name, Commands, Group, ChatColor;
    }
    public class GroupList
    {
        public string GroupName, Parent, Commands, ChatColor, Prefix, Suffix;
    }
    public class ExpirationInfo
    {
        public string Name, Value, Type;
        public DateTime StartTime, EndTime;
        public string DurationText;
    }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。