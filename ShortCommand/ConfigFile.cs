using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace ShortCommand;

public class ConfigFile
{
    public class CMD
    {
        public string 原始命令 = "warp {0}";

        public string 新的命令 = "传送";

        public bool 余段补充 = false;

        public bool 阻止原始 = false;

        public int 死亡条件 = 0;

        public int 冷却秒数 = 0;

        public bool 冷却共用 = false;
    }

    [Description("命令表")]
    public List<CMD> 命令表 = new List<CMD>();

    public static Action<ConfigFile> ConfigR;

    public static ConfigFile Read(string Path)
    {
        if (!File.Exists(Path))
        {
            var configFile = new ConfigFile();
            configFile.命令表.Add(new CMD());
            configFile.命令表.Add(new CMD
            {
                原始命令 = "spawn",
                新的命令 = "回城"
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
        if (ConfigR != null)
        {
            ConfigR(configFile);
        }
        return configFile;
    }

    public void Write(string Path)
    {
        using var stream = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        Write(stream);
    }

    public void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, (Formatting) 1);
        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write(value);
    }
}
