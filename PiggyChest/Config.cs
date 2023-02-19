using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiggyChest;

public class Config
{
    public const string ConfigPath = "/tshock/PiggyChest.json";

    [JsonProperty(PropertyName = "存储路径")]public string StoragePath { get; set; }
    [JsonProperty(PropertyName = "箱子名称列表")]public List<string> ChestNames { get; set; }

    public Config()
    {
        this.StoragePath = "/PiggyChest/";
        this.ChestNames = new List<string> { "piggy" };
    }

    public static Config LoadConfig()
    {
        var config = new Config();
        if (File.Exists(ConfigPath))
        {
            var text = File.ReadAllText(ConfigPath);
            config = JsonConvert.DeserializeObject<Config>(text)!;
        }
        else
        {
            config.Write();
        }
        return config;
    }
    public void Write()
    {
        var text = JsonConvert.SerializeObject(this);
        File.WriteAllText(ConfigPath, text);
    }
}
