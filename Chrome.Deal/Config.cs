using Newtonsoft.Json;
using TShockAPI;

namespace deal;

public class Config
{
    public static void GetConfig()
    {
        try
        {
            if (!File.Exists("tshock/Chrome.Deal.json"))
            {
                FileTools.CreateIfNot(Path.Combine("tshock/Chrome.Deal.json"), JsonConvert.SerializeObject(Plugin.配置, Formatting.Indented));
                Plugin.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.Deal.json")));
                File.WriteAllText("tshock/Chrome.Deal.json", JsonConvert.SerializeObject(Plugin.配置, Formatting.Indented));
            }
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.Deal]配置文件读取错误！！！");
        }
    }
    public double 税率 = 0.2;
    public int 最大价格 = 999999999;
    public bool 广播上架物品 = true;
    public bool 广播下架物品 = true;
    public bool 广播购买成功 = true;
    public class QwRPG配置表
    {
        public string 货币名 = "点券";
        public bool 是否头顶显示货币变化 = true;
        public string 货币增加时头顶显示 = "+ {0} EXP";
        public string 货币减少时头顶显示 = "- {0} EXP";
    }

}

