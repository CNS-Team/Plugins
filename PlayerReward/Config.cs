using LazyUtils;
using Newtonsoft.Json;

namespace PlayerReward;

[Config]
internal class Config : Config<Config>
{
    #region Hints

    [JsonProperty(Order = 1)] public static string Hint1 => "各个礼包的名字需唯一，不可重复";
    [JsonProperty(Order = 1)] public static string Hint2 => "Item格式为 'ID*数量*前缀' ";
    [JsonProperty(Order = 1)] public static string Hint3 => "ExecuteCommand 中可以添加 '{name}' 作为玩家名称的占位符， 如 '/bc player name is {name}' ";
    [JsonProperty(Order = 1)] public static string Hint4 => "而 '{account}' 则是玩家账户名称的占位符，如 '/bc player account name is {account}' ";

    #endregion

    [JsonProperty(Order = 2)] public List<PlayerPack> PlayerPacks = new() { new PlayerPack() };
    [JsonProperty(Order = 2)] public List<PlayerPack> SponsorPacks = new() { new PlayerPack() };
}

internal class PlayerPack
{
    public string Name = "Unique Player Pack Name";
    public List<string> Groups = new();
    public List<string> Items = new();
    public List<string> ExecuteCommands = new();
    public bool IsHardMode = false;
}