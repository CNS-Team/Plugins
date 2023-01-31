using VBY.Basic.Config;

namespace CustomPlayer;

public class Config : MainConfig<Root>
{
    public Config(string configDirectory, string fileName = "CustomPlayer.json") : base(configDirectory, fileName)
    {
    }
}

public class Root : MainRoot
{
    public int ServerId;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public string MysqlHost, MysqlUser, MysqlPass, MysqlDatabase;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public uint MysqlPort;
    public bool Debug = true;
    public bool EnableGroup = true;
    public bool EnablePrefix = true;
    public bool EnableSuffix = true;
    public bool EnablePermission = true;
    public bool CoverGroup = true;
    public bool 对接称号插件 = false;
}
