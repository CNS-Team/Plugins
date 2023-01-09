using LazyUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TShockAPI.DB;

namespace OnlineInfo
{
    [Config]
    internal class OIConfig : Config<OIConfig>
    {
        public bool AutoUpdate = false;

        public int ServerID = 0;

        public string ServerName = "ServerName";

        [JsonConverter(typeof(StringEnumConverter))]
        public SqlType DBType = SqlType.Sqlite;

        public string SqlitePath = "onlineinfo.sqlite";

        public string MySqlHost = "localhost";

        public uint MySqlPort = 3306;

        public string MySqlName = "Database Name";

        public string MySqlUser = "root";

        public string MySqlPassword = "password";
    }
}
