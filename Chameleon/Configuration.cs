using Newtonsoft.Json;
using TShockAPI;

namespace Chameleon;

internal class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "Chameleon.json");

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

    public void Write(string path)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using var streamWriter = new StreamWriter(stream);
        streamWriter.Write(value);
    }

    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
        {
            return new Configuration();
        }
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var streamReader = new StreamReader(stream);
        return JsonConvert.DeserializeObject<Configuration>(streamReader.ReadToEnd())!;
    }
}