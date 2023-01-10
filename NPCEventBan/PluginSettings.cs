using Newtonsoft.Json;
using System.Collections.Generic;

namespace NPCEventBan;

public class PluginSettings
{
    [JsonProperty("启用NPC限制")]
    public bool NPCBanEnabled = true;

    [JsonProperty("NPC限制白名单")]
    public List<int> NPCWhiteList = new List<int>();

    [JsonProperty("NPC限制黑名单")]
    public List<int> NPCBlackList = new List<int>();

    [JsonProperty("禁止满月")]
    public bool disableFullMoon = true;

    [JsonProperty("禁止霜月")]
    public bool disableFrostMoon = true;

    [JsonProperty("禁止血月")]
    public bool disableBloodMoon = true;

    [JsonProperty("禁止南瓜月")]
    public bool disablePumpkinMoon = true;

    [JsonProperty("禁止日食")]
    public bool disableSolarEclipse = true;

    [JsonProperty("禁止下雨")]
    public bool disableRain = true;

    [JsonProperty("禁止史莱姆雨")]
    public bool disableSlimeRain = true;

    [JsonProperty("禁止哥布林入侵")]
    public bool disableGoblinInvasion = true;

    [JsonProperty("禁止海盗入侵")]
    public bool disablePirateInvasion = true;

    [JsonProperty("禁止雪人军团")]
    public bool disableFrostLegion = true;

    [JsonProperty("禁止下落陨铁")]
    public bool disableMeteors = true;

    [JsonProperty("禁止火星人入侵")]
    public bool disableMartianInvasion = true;

    [JsonProperty("禁止月球入侵")]
    public bool disableLunarInvasion = true;

    [JsonProperty("禁止拜月教邪教徒")]
    public bool disableCultists = true;
}