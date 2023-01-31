using Newtonsoft.Json;
using TShockAPI;

namespace Chrome.RPG
{
    public class Config
    {
        public static void GetConfig()
        {
            try
            {
                if (!File.Exists("tshock/Chrome.RPG.json"))
                {
                    FileTools.CreateIfNot(Path.Combine("tshock/Chrome.RPG.json"), JsonConvert.SerializeObject(Chrome.配置, Formatting.Indented));
                    Chrome.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.RPG.json")));
                    Chrome.配置.杀死不给货币NPC.Add(488);
                    Chrome.配置.重置职业指令.Add("/clearbuffs");
                    Chrome.配置.职业配置表.Add(new() { 职业 = "战士", 前缀 = "【战士】", 升级货币 = 1000, 上一职业 = "无职业" });
                    File.WriteAllText("tshock/Chrome.RPG.json", JsonConvert.SerializeObject(Chrome.配置, Formatting.Indented));
                }
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[Chrome_RPG]配置文件读取错误！！！");
            }
        }
        //public Dictionary<string, string> RPG = new() { { "", "" } };
        public string 说明_可选进度 = "史莱姆王,克苏鲁之眼,世界吞噬者,克苏鲁之脑,蜂王,骷髅王,独眼巨鹿,血肉墙,史莱姆皇后,双子魔眼,毁灭者,机械骷髅王,世纪之花,石巨人,猪龙鱼公爵,光之女皇,拜月教邪教徒,月亮领主,哥布林军队,海盗入侵,霜月,南瓜月,火星暴乱,日耀柱,星旋柱,星云柱,星尘柱";
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
        public bool 是否允许重置职业 = true;
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
            public string 上一职业 = "";
            public string 前缀 = "";
            public string 后缀 = "";
            public List<int> 任务限制 = new() { };
            public List<string> 进度限制 = new() { };
            public List<string> 升级指令 = new() { };
            public List<string> 使用物品升级 = new() { }; 
            public long 升级货币 = 0;
        }
    }
}

