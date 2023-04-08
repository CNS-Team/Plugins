using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Chrome.RPG;

public class Config
{
    #region json配置表读写
    public static void GetConfig()
    {
        try
        {
            if (!File.Exists("tshock/Chrome.RPG.json"))
            {
                FileTools.CreateIfNot(Path.Combine("tshock/Chrome.RPG.json"), JsonConvert.SerializeObject(Chrome.配置, Formatting.Indented));
                Chrome.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.RPG.json")))!;
                Chrome.配置.杀死不给货币NPC.Add(488);
                Chrome.配置.重置职业指令.Add("/clearbuffs");
                Chrome.配置.职业配置表.Add(new() { 职业 = "战士", 等级 = new() { new() { 升级货币 = 1000 }, new() { 升级货币 = 2000, 等级 = 2 } }, 转职职业 = new() { new() { 职业 = "狂战士", 转职货币 = 1000 } } });
                Chrome.配置.职业配置表.Add(new() { 职业 = "狂战士", 等级 = new() { new() { 升级货币 = 1000 }, new() { 升级货币 = 2000, 等级 = 2 } } });
                File.WriteAllText("tshock/Chrome.RPG.json", JsonConvert.SerializeObject(Chrome.配置, Formatting.Indented));
            }
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome_RPG]配置文件读取错误！！！");
        }
    }
    public static void Reload(ReloadEventArgs args)
    {
        try
        {
            Reload();
            args.Player.SendSuccessMessage($"[Chrome.RPG]重载成功！");
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.RPG]配置文件读取错误");
        }
    }
    public static void Reload()
    {
        DB.Reload();
        Chrome.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.RPG.json")))!;
        File.WriteAllText("tshock/Chrome.RPG.json", JsonConvert.SerializeObject(Chrome.配置, Formatting.Indented));
    }
    #endregion
    #region 配置表格式
    public string 说明_可选进度 = "史莱姆王,克苏鲁之眼,世界吞噬者,克苏鲁之脑,蜂王,骷髅王,独眼巨鹿,血肉墙,史莱姆皇后,双子魔眼,毁灭者,机械骷髅王,世纪之花,石巨人,猪龙鱼公爵,光之女皇,拜月教邪教徒,月亮领主,哥布林军队,海盗入侵,霜月,南瓜月,火星暴乱,日耀柱,星旋柱,星云柱,星尘柱";
    public bool 显示报错 = false;
    public string 货币名 = "点券";
    public bool 是否启用前后缀 = true;
    public bool 是否头顶显示货币变化 = true;
    public string 货币增加时头顶显示 = "+ {0} EXP";
    public string 货币减少时头顶显示 = "- {0} EXP";
    public bool 启用侧边栏信息 = true;
    public double 死亡掉落货币百分率 = 0.20;
    public List<int> 杀死不给货币NPC = new();
    public double 获取货币血量比例 = 1.0;
    public bool 杀死雕像生成NPC不给货币 = true;
    public bool 升级广播 = true;
    public bool 转职时显示职业预览 = true;
    public int 玩家重置职业次数上限 = 2;
    public List<string> 重置职业指令 = new() { };
    public string 玩家初始职业 = "无职业";
    public List<string> 击败肉山前不继续加货币职业 = new() { };
    public long 肉山前最大货币数量 = 100000000;
    public long 肉山后最大货币数量 = 1000000000;
    public List<职业格式> 职业配置表 = new() { };
    public class 职业格式
    {
        public string 职业 = "";
        public List<等级格式> 等级 = new() { };
        public string 前缀 = "[c/{颜色}:LV{等级}][c/8651F2:{职业}{等级名}]";
        public string 后缀 = "";
        public List<转职格式> 转职职业 = new() { };
    }
    public class 等级格式
    {
        public int 等级 = 1;
        public string 等级名 = "";
        public string 颜色 = "";
        public List<string> 升级指令 = new() { };
        public List<string> 进度限制 = new() { };
        public List<int> 任务限制 = new() { };
        public List<string> 物品升级 = new() { };
        public long 升级货币 = 0;
    }
    public class 转职格式
    {
        public string 职业 = "";
        public List<string> 转职指令 = new() { };
        public int 转职货币 = 0;
    }
    #endregion
    #region 职业相关的操作
    public static bool 是否有下级(职业格式 z, int 等级)
    {
        return z.等级.Exists(s => s.等级 == 等级 + 1);
    }
    public static bool 是否有下级(string 职业名, int 等级)
    {
        var z = Chrome.配置.职业配置表.Find(s => s.职业 == 职业名);
        return z.等级.Exists(s => s.等级 == 等级 + 1);
    }
    public static 职业格式? 获取职业(string 职业名)
    {
        return Chrome.配置.职业配置表.Find(s => s.职业 == 职业名);
    }
    public static List<职业格式> 获取下一职业(职业格式 当前职业)
    {
        List<职业格式> list = new() { };
        foreach (var z in 当前职业.转职职业)
        {
            var 职业 = 获取职业(z.职业);
            if (职业 != null)
            {
                list.Add(职业);
            }
        }
        return list;
    }
    public static List<职业格式> 获取下一职业(string 当前职业名)
    {
        var 当前职业 = Chrome.配置.职业配置表.Find(s => s.职业 == 当前职业名);
        List<职业格式> list = new() { };
        foreach (var z in 当前职业.转职职业)
        {
            var 职业 = 获取职业(z.职业);
            if (职业 != null)
            {
                list.Add(职业);
            }
        }
        return list;
    }
    public static 等级格式 获取等级(职业格式 当前职业, int 等级)
    {
        var z = 当前职业.等级.Find(s => s.等级 == 等级)!;
        return z;
    }
    public static 等级格式 获取等级(string 职业名, int 等级)
    {
        var 当前职业 = 获取职业(职业名);
        var z = 当前职业.等级.Find(s => s.等级 == 等级);
        return z;
    }
    public static long 获取升级所需货币(职业格式 当前职业, int 等级)
    {
        var z = 当前职业.等级.Find(s => s.等级 == 等级).升级货币;
        return z;
    }
    public static long 获取升级所需货币(string 职业名, int 等级)
    {
        var 当前职业 = 获取职业(职业名);
        var z = 当前职业.等级.Find(s => s.等级 == 等级).升级货币;
        return z;
    }
    public static int 查等级(string 玩家名)
    {
        var 等级 = 1;
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名))
        {
            if (表.Read())
            {
                等级 = 表.Get<int>("等级");
            }
        }
        return 等级;
    }
    public static string 查职业(string 玩家名)
    {
        var 职业 = "";
        using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名` = @0", 玩家名))
        {
            if (表.Read())
            {
                职业 = 表.Get<string>("职业");
            }
        }
        return 职业;
    }
    #endregion
}