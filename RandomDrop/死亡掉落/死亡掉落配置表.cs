using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace 死亡掉落;

internal class 死亡掉落配置表
{

    public bool 是否开启进度掉落 = true;

    public bool 是否开启销毁提示 = true;

    public int 进度掉落概率 = 1000;

    public int 进度掉落数量 = 1;

    public int 总掉落百分比 = 1000;

    public List<int> 哥布林一前禁物品 = new ();

    public List<int> 萌王前禁物品 = new ();

    public List<int> 鹿角怪前禁物品 = new ();

    public List<int> 克眼前禁物品 = new ();

    public List<int> 虫脑前禁物品 = new ();

    public List<int> 旧日一前禁物品 = new ();

    public List<int> 蜂后前禁物品 = new ();

    public List<int> 骷髅前禁物品 = new ();

    public List<int> 肉墙前禁物品 = new ();

    public List<int> 哥布林二前禁物品 = new ();

    public List<int> 海盗前禁物品 = new ();

    public List<int> 日蚀一前禁物品 = new ();

    public List<int> 萌后前禁物品 = new ();

    public List<int> 任一三王前禁物品 = new ();

    public List<int> 旧日二前禁物品 = new ();

    public List<int> 机械眼前禁物品 = new ();

    public List<int> 机械虫前禁物品 = new ();

    public List<int> 机械骷髅前禁物品 = new ();

    public List<int> 三王前禁物品 = new ();

    public List<int> 日蚀二前禁物品 = new ();

    public List<int> 猪鲨前禁物品 = new ();

    public List<int> 妖花前禁物品 = new ();

    public List<int> 日蚀三前禁物品 = new ();

    public List<int> 霜月树前禁物品 = new ();

    public List<int> 霜月坦前禁物品 = new ();

    public List<int> 霜月后前禁物品 = new ();

    public List<int> 南瓜树前禁物品 = new ();

    public List<int> 南瓜王前禁物品 = new ();

    public List<int> 光女前禁物品 = new ();

    public List<int> 石巨人前禁物品 = new ();

    public List<int> 旧日三前禁物品 = new ();

    public List<int> 外星人前禁物品 = new ();

    public List<int> 教徒前禁物品 = new ();

    [JsonProperty] public List<int> 日耀前禁物品 = new ();

    public List<int> 星旋前禁物品 = new ();

    public List<int> 星尘前禁物品 = new ();

    public List<int> 星云前禁物品 = new ();

    public List<int> 所有柱子前禁物品 = new ();

    public List<int> 月总前禁物品 = new ();

    public bool 哥布林一进行 = false;

    public bool 旧日军团一 = false;

    public bool 哥布林二进行 = false;

    public bool 海盗进行 = false;

    public bool 日蚀一进行 = false;

    public bool 旧日军团二 = false;

    public bool 日蚀二进行 = false;

    public bool 日蚀三进行 = false;

    public bool 旧日军团三 = false;

    public bool 火星进行 = false;

    private const string Path = "tshock/随机掉落配置表.json";

    private static 死亡掉落配置表? _config = null; 

    public static 死亡掉落配置表? GetConfig()
    {
        if (_config is null) 
        {
            if (!File.Exists(Path))
            {
                _config = new 死亡掉落配置表();
                using var streamWriter = new StreamWriter(Path);
                streamWriter.WriteLine(JsonConvert.SerializeObject(_config, Formatting.Indented));
            }
            else
            {
                using var streamReader = new StreamReader(Path);
                _config = JsonConvert.DeserializeObject<死亡掉落配置表>(streamReader.ReadToEnd());
            }
        }
        

        return _config;
    }

    public static void ReloadConfig()
    {
        _config = null;
        GetConfig();
    }
}