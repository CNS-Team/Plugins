using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.Hooks;

namespace deal;

public class Config
{
    public static void GetConfig()
    {
        try
        {
            if (!File.Exists("tshock/Chrome.Deal.json"))
            {
                FileTools.CreateIfNot(Path.Combine("tshock/Chrome.Deal.json"), JsonConvert.SerializeObject(deal.配置, Formatting.Indented));
                deal.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.Deal.json")));
                File.WriteAllText("tshock/Chrome.Deal.json", JsonConvert.SerializeObject(deal.配置, Formatting.Indented));
            }
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.Deal]配置文件读取错误！！！");
        }
    }
    public static void Reload(ReloadEventArgs args)
    {
        try
        {
            Reload();
        }
        catch
        {
            args.Player.SendErrorMessage($"[Chrome.Deal]配置文件读取错误");
        }
    }
    public static void Reload()
    {
        DB.Reload();
        deal.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.Deal.json")));
        File.WriteAllText("tshock/Chrome.Deal.json", JsonConvert.SerializeObject(deal.配置, Formatting.Indented));
    }
    public double 税率 = 0.2;
    public int 最大价格 = 999999999;
    public bool 广播上架物品 = true;
    public bool 广播下架物品 = true;
    public bool 广播购买成功 = true;
}

