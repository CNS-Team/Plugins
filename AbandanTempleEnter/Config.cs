using Newtonsoft.Json;

namespace AbandanTempleEnter;

internal class Config
{
    [JsonProperty("检测间隔(tick)")]
    public int checkTime = 60;

    [JsonProperty("世纪之花前禁止进入")]
    public bool plantBoss = true;

    [JsonProperty("三王前禁止进入")]
    public bool threeBoss = true;

    [JsonProperty("肉前禁止进入")]
    public bool hardMode = true;

    [JsonProperty("杀死试图进入的玩家")]
    public bool kill = true;

    [JsonProperty("杀死玩家提示")]
    public string killText = "{0}尝试进入丛林神庙!";

    [JsonProperty("将玩家传送回出生点提示")]
    public string spawnText = "{0}尝试进入丛林神庙!";

    [JsonProperty("全服广播")]
    public bool Broadcast = true;

    public const string DefaultPath = "tshock/AbandanTempleEnterConfig.json";

    public void Save()
    {
        using StreamWriter streamWriter = new(DefaultPath);
        streamWriter.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static Config GetConfig()
    {
        Config config = new();
        if (!File.Exists(DefaultPath))
        {
            config.Save();
        }
        else
        {
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(DefaultPath))!;
        }
        return config;
    }

}
