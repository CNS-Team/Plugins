using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ShortCommand;

public class ConfigFile
{
    public class CMD
    {
        [JsonProperty("原始命令")]
        public string SourceCommand = "warp {0}";

        [JsonProperty("新的命令")]
        public string NewCommand = "传送";

        [JsonProperty("余段补充")]
        public bool Supplement = false;

        [JsonProperty("阻止原始")]
        public bool NotSource = false;

        [JsonProperty("死亡条件")]
        public int Death = 0;

        [JsonProperty("冷却秒数")]
        public int CD = 0;

        [JsonProperty("冷却共享")]
        public bool ShareCD = false;
    }

    [JsonProperty("命令表")]
    [Description("命令表")]
    public List<CMD> Commands = new List<CMD>();

    public static Action<ConfigFile> ConfigR;

    public static ConfigFile Read(string Path)
    {
        if (!File.Exists(Path))
        {
            var configFile = new ConfigFile();
            configFile.Commands.Add(new CMD());
            configFile.Commands.Add(new CMD
            {
                SourceCommand = "spawn",
                NewCommand = "回城"
            });
            return configFile;
        }
        using var stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Read(stream);
    }

    public static ConfigFile Read(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        var configFile = JsonConvert.DeserializeObject<ConfigFile>(streamReader.ReadToEnd());
        ConfigR?.Invoke(configFile);
        return configFile;
    }

    public void Write(string Path)
    {
        using var stream = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(stream);
    }

    public void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, (Formatting) 1);
        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write(value);
    }
}