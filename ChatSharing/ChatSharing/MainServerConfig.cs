using Newtonsoft.Json;

namespace ChatSharing;

[JsonObject]
public class MainServerConfig
{
    public string host = "localhost";

    public ushort port = 8080;

    public string servername = "Terraria Server";

    public string format = "[{0}] {1}";
}