using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChromeAutoReset;

internal class ResetConfig
{
    [JsonProperty("重置前指令")]
    public string[] PreResetCommands;

    [JsonProperty("重置后指令")]
    public string[] PostResetCommands;

    [JsonProperty("替换文件")]
    public Dictionary<string, string> Files;

    [JsonProperty("重置后SQL命令")]
    public string[] SQLs;

    public string ToJson()
    {
        return JsonConvert.SerializeObject((object) this, (Formatting) 1);
    }
}