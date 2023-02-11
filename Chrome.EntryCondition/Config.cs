using Newtonsoft.Json;
using TShockAPI;

namespace Chrome.EntryCondition;

public class Config
{
    public static void GetConfig()
    {
        try
        {
            var path = "tshock/Chrome.EntryCondition.json";
            if (!File.Exists(path))
            {
                FileTools.CreateIfNot(Path.Combine(path), JsonConvert.SerializeObject(EntryCondition.配置, Formatting.Indented));
                EntryCondition.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(path)));
                File.WriteAllText(path, JsonConvert.SerializeObject(EntryCondition.配置, Formatting.Indented));
            }
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.EntryCondition]配置文件读取错误！！！");
        }
    }
    public bool 启用插件 = false;
    public bool 允许管理员忽略进入判断 = true;
    public string 职业阻止进入提示 = "Chrome.RPG\n您当前职业不够资格进入该服务器";
    public string 等级不够阻止进入提示 = "Chrome.RPG\n您当前等级不够资格进入该服务器,需要达到{0}级";
    public Dictionary<string, int> 允许进入的职业 = new() { };
    public string 任务阻止进入提示 = "Chrome.RPG\n您当前需要完成任务\n{0}\n才能进入该服务器";
    public List<int> 进入需要完成的任务 = new() { };
}