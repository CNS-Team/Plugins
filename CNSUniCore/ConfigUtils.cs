using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using TShockAPI;

namespace CNSUniCore;
public static class ConfigUtils
{
    public static readonly string configDir = Path.Combine(TShock.SavePath, "CNSUniCore");

    public static readonly string configPath = Path.Combine(configDir, "config.json");

    public static readonly string serversPath = Path.Combine(configDir, "servers.json");

    public static UConfig config = new UConfig();

    public static List<ServerInfo> servers = new List<ServerInfo>();

    public static void LoadConfig()
    {
        if (Directory.Exists(configDir))
        {
            if (File.Exists(serversPath))
            {
                servers = JsonConvert.DeserializeObject<List<ServerInfo>>(File.ReadAllText(serversPath))!;
            }
            else
            {
                servers.Add(new ServerInfo());
                File.WriteAllText(serversPath, JsonConvert.SerializeObject(servers, Formatting.Indented));
            }
            if (File.Exists(configPath))
            {
                config = JsonConvert.DeserializeObject<UConfig>(File.ReadAllText(configPath))!;
            }
            else
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }
        else
        {
            Directory.CreateDirectory(configDir);
            servers.Add(new ServerInfo());
            File.WriteAllText(serversPath, JsonConvert.SerializeObject(servers, Formatting.Indented));
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }

    public static IDbConnection LoadDb()
    {
        try
        {
            var array = config.HostPort.Split(':');
            var mySqlConnection = new MySqlConnection
            {
                ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};", array[0], (array.Length > 1) ? array[1] : "3306", config.DataBaseName, config.DBUserName, config.DBPassword)
            };
            return mySqlConnection;
        }
        catch (MySqlException)
        {
            throw new Exception("Mysql未正确配置");
        }
    }

    public static void UpdateConfig()
    {
        File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
    }

    public static void UpdateServer()
    {
        File.WriteAllText(serversPath, JsonConvert.SerializeObject(servers, Formatting.Indented));
    }
}