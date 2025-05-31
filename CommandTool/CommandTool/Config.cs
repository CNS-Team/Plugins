using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;

namespace CommandTool;

internal class Config
{
    public bool Enable;

    public RectangleInfo[] Rectangles = Array.Empty<RectangleInfo>();

    public Dictionary<string, SwitchCommandInfo> SwitchCommandList = new ();

    public static readonly string ConfigPath = Path.Combine(TShock.SavePath, "CommandTool.json");

    public static Config GetConfig()
    {
        if (File.Exists(ConfigPath))
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
        }

        var config = new Config();
        config.Enable = false;
        config.Rectangles = new RectangleInfo[1] { new() { Commands = new StandCommand[1] { new() { Text = "", Permission = "" } }, Sign = new StandSign { Enable = false, Color = Color.Gold, OnEnter = "此处输入进入该区域的提示", Loop = "此处输入每秒提示(效果：据Text还有x秒)" } } };
        var config2 = config;
        config2.Write();
        return config2;
    }

    public void Write()
    {
        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject((object) this, (Formatting) 1));
    }
}