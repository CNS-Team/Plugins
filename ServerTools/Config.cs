using Newtonsoft.Json;

namespace ServerTools;

public class Config
{
    [JsonProperty("超过掉落物次数踢出玩家")]
    public bool Multiplekickout = false;

    [JsonProperty("掉落物品次数限制")]
    public int MultipleCount = 3;

    [JsonProperty("玩家白名单列表")]
    public HashSet<string> BwlList = new();

    [JsonProperty("白名单阻止语句")]
    public string BwlPrompt = string.Empty;

    [JsonProperty("等待列表长度")]
    public ushort AwaitBufferSize = 10;

    [JsonProperty("启用强制提示显示")]
    public bool EnableForcedHint = true;

    [JsonProperty("密码长度限制")]
    public int LimitPasswordLength = 10;

    [JsonProperty("强制提示欢迎语")]
    public string Greeting = "   欢迎来到Sin喵窝";

    [JsonProperty("验证失败提示语")]
    public string VerficationFailedMessage = "         账户密码错误.\r\n\r\n         若你第一次进服，请换一个人物名:\r\n         若忘记密码, 请在群内发送\"重置密码\".";

    [JsonProperty("强制提示文本")]
    public string[] Hints = new string[5] { "", " ↓↓ 请看下面的提示以进服 ↓↓", " 1.请确保你已经阅读进服教程,群内发\"IP\"查看", " 2.请再次加服,在\"服务器密码\"中输入你要设置的密码,", "注: 你需要记住你的密码." };
    /// <summary>
    /// 读取配置文件内容
    /// </summary>
    /// <param name="Path">配置文件路径</param>
    /// <returns></returns>
    public static Config Read(string Path)
    {
        if (!File.Exists(Path)) return new Config();
        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Read(fs);
    }

    /// <summary>
    /// 通过文件流读取文件内容
    /// </summary>
    /// <param name="stream">流</param>
    /// <returns></returns>
    public static Config Read(Stream stream)//给定流文件进行读取
    {
        using var sr = new StreamReader(stream);
        var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
        return cf == null ? new Config() : cf;
    }

    /// <summary>
    /// 写入配置
    /// </summary>
    /// <param name="Path">配置文件路径</param>
    public void Write(string Path)//给定路径进行写
    {
        using (var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
        { Write(fs); }
    }

    /// <summary>
    /// 通过文件流写入配置
    /// </summary>
    /// <param name="stream">文件流</param>
    public void Write(Stream stream)//给定流文件写
    {
        var data = JsonConvert.SerializeObject(this, Formatting.Indented);
        var sw = new StreamWriter(stream);
        sw.Write(data);
        sw.Close();
    }
}