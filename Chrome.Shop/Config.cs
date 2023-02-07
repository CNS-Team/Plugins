using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.Hooks;

namespace QwRPG.shop;

public class Config
{
    public static void GetConfig()
    {
        try
        {
            if (!File.Exists("tshock/Chrome.Shop.json"))
            {
                FileTools.CreateIfNot(Path.Combine("tshock/Chrome.Shop.json"), JsonConvert.SerializeObject(shop.Shop.配置, Formatting.Indented));
                shop.Shop.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.Shop.json")))!;
                shop.Shop.配置.Shops.Add(new() { 价格 = 1000, 显示名 = "木材", 进度限制 = new() { "史莱姆王" }, 商品 = new() { new() { 数量 = 100, 前缀 = 0, 物品 = 9 } } });
                File.WriteAllText("tshock/Chrome.Shop.json", JsonConvert.SerializeObject(shop.Shop.配置, Formatting.Indented));
            }
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.shop]配置文件读取错误！！！");
        }
    }
    public static void Reload(ReloadEventArgs args)
    {
        try
        {
            Reload();
            //args.Player.SendErrorMessage($"[QwRPG.Shop]重载成功！");
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.Shop]配置文件读取错误");
        }
    }
    public static void Reload()
    {
        try
        {
            shop.Shop.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.Shop.json")));
            File.WriteAllText("tshock/Chrome.Shop.json", JsonConvert.SerializeObject(shop.Shop.配置, Formatting.Indented));
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[QwRPG.Shop]配置文件读取错误");
        }
    }
    public string 可选进度 = "史莱姆王,克苏鲁之眼,世界吞噬者,克苏鲁之脑,蜂王,骷髅王,独眼巨鹿,血肉墙,史莱姆皇后,双子魔眼,毁灭者,机械骷髅王,世纪之花,石巨人,猪龙鱼公爵,光之女皇,拜月教邪教徒,月亮领主,哥布林军队,海盗入侵,霜月,南瓜月,火星暴乱,日耀柱,星旋柱,星云柱,星尘柱";
    public List<Shop> Shops = new() { };
    public class Shop
    {
        public string 显示名 = "";
        public int 价格 = 0;
        public List<string> 进度限制 = new() { };
        public List<Item> 商品 = new() { };
    }
    public class Item
    {
        public int 物品 = 0;
        public int 数量 = 0;
        public int 前缀 = 0;
    }
}

