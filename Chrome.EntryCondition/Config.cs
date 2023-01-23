using Newtonsoft.Json;
using TShockAPI;

namespace Chrome.EntryCondition
{
    public class Config
    {
        public static void GetConfig()
        {
            try
            {
                string path = "tshock/Chrome.EntryCondition.json";
                if (!File.Exists(path))
                {
                    FileTools.CreateIfNot(Path.Combine(path), JsonConvert.SerializeObject(快速配置rest.配置, Formatting.Indented));
                    快速配置rest.配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(path)));
                    File.WriteAllText(path, JsonConvert.SerializeObject(快速配置rest.配置, Formatting.Indented));
                }
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[Chrome.EntryCondition]配置文件读取错误！！！");
            }
        }
        public bool 启用插件 = false;
        public bool 允许管理员忽略职业判断 = true;
        public string 阻止进入提示 = "Chrome.RPG\n您当前职业不够资格进入该服务器";
        public List<string> 允许进入的职业 = new() { };
    }
}

