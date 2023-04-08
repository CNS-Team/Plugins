using Newtonsoft.Json;

namespace MulitRegister;

public class Server
{
    [JsonProperty("服务器IP")]
    public string Host { get; set; } = "127.0.0.1";


    [JsonProperty("服务器Rest端口")]
    public int RestPort { get; set; } = 7878;


    [JsonProperty("Rest密钥")]
    public string Token { get; set; } = "";

}