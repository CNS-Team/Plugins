using LazyUtils;
using Newtonsoft.Json;

namespace PlayerReward;

[Config]
internal class Config : Config<Config>
{
    [JsonProperty] public static string Hint1 => "各个礼包的名字需唯一，不可重复";

    [JsonProperty] public static string Hint2 => "Item格式为 'ID*数量*前缀' ";

    [JsonProperty] public static string Hint3 => "ExecuteCommand 中可以添加 '{name}' 作为玩家名称的占位符， 如 '/bc player name is {name}' ";
    [JsonProperty] public static string Hint4 => "而 '{account}' 则是玩家账户名称的占位符，如 '/bc player account name is {account}' ";

    public List<GroupPack> GroupPacks = new() { new GroupPack() };
    public List<SponsorPack> SponsorPacks = new() { new SponsorPack() };
}

internal interface IPack
{
    public string Name { get; }
    public List<string> Items { get; }
    public List<string> ExecuteCommands { get; }
}

internal class GroupPack : IPack
{
    public string Name { get; set; } = "Group Pack Name";
    public List<string> Groups { get; set; } = new();
    public List<string> Items { get; set; } = new();
    public List<string> ExecuteCommands { get; set; } = new();
}

internal class SponsorPack : IPack
{
    public string Name { get; set; } = "Sponsor Pack Name";
    public List<string> Items { get; set; } = new();
    public List<string> ExecuteCommands { get; set; } = new();
}