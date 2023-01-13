using Newtonsoft.Json;
using TShockAPI;

namespace DataSync;
public class Config
{
    public string 进度 = "史莱姆王,克苏鲁之眼,世界吞噬者,克苏鲁之脑,蜂王,骷髅王,独眼巨鹿,血肉墙,史莱姆皇后,双子魔眼,毁灭者,机械骷髅王,世纪之花,石巨人,猪龙鱼公爵,光之女皇,拜月教邪教徒,月亮领主,哥布林军队,海盗入侵,霜月,南瓜月,火星暴乱,日耀柱,星旋柱,星云柱,星尘柱";

    public bool 是否同步 = true;

    public bool 进度同步哥布林 = true;

    public bool 进度同步萌王 = true;

    public bool 进度同步鹿角怪 = true;

    public bool 进度同步克眼 = true;

    public bool 进度同步虫脑 = true;

    public bool 进度同步蜂后 = true;

    public bool 进度同步骷髅 = true;

    public bool 进度同步肉墙 = true;

    public bool 进度同步海盗 = true;

    public bool 进度同步萌后 = true;

    public bool 进度同步任一三王 = true;

    public bool 进度同步机械眼 = true;

    public bool 进度同步机械虫 = true;

    public bool 进度同步机械骷髅 = true;

    public bool 进度同步猪鲨前 = true;

    public bool 进度同步妖花前 = true;

    public bool 进度同步霜月树 = true;

    public bool 进度同步霜月坦 = true;

    public bool 进度同步霜月后 = true;

    public bool 进度同步南瓜树 = true;

    public bool 进度同步南瓜王 = true;

    public bool 进度同步光女前 = true;

    public bool 进度同步石巨人 = true;

    public bool 进度同步教徒前 = true;

    public bool 进度同步日耀前 = true;

    public bool 进度同步星旋前 = true;

    public bool 进度同步星尘前 = true;

    public bool 进度同步星云前 = true;

    public bool 进度同步月总 = true;

    public static void GetConfig()
    {
        string PATH = Path.Combine(TShock.SavePath, "DataSync.json");
        try
        {
            if (!File.Exists(PATH))
            {
                FileTools.CreateIfNot(PATH, JsonConvert.SerializeObject(Plugin.配置, Formatting.Indented));
                Plugin.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(PATH))!;
                File.WriteAllText(PATH, JsonConvert.SerializeObject(Plugin.配置, Formatting.Indented));
            }
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage("[DataSync]配置文件读取错误！！！");
        }
    }
}
