using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TShockAPI;

namespace CNSUniCore
{
	public static class ConfigUtils
	{
		public static void LoadConfig()
		{
			bool flag = Directory.Exists(ConfigUtils.configDir);
			if (flag)
			{
				bool flag2 = File.Exists(ConfigUtils.serversPath);
				if (flag2)
				{
					ConfigUtils.servers = JsonConvert.DeserializeObject<List<ServerInfo>>(File.ReadAllText(ConfigUtils.serversPath));
				}
				else
				{
					ConfigUtils.servers.Add(new ServerInfo());
					File.WriteAllText(ConfigUtils.serversPath, JsonConvert.SerializeObject(ConfigUtils.servers, (Formatting)1));
				}
				bool flag3 = File.Exists(ConfigUtils.configPath);
				if (flag3)
				{
					ConfigUtils.config = JsonConvert.DeserializeObject<UConfig>(File.ReadAllText(ConfigUtils.configPath));
				}
				else
				{
					File.WriteAllText(ConfigUtils.configPath, JsonConvert.SerializeObject(ConfigUtils.config, (Formatting)1));
				}
			}
			else
			{
				Directory.CreateDirectory(ConfigUtils.configDir);
				ConfigUtils.servers.Add(new ServerInfo());
				File.WriteAllText(ConfigUtils.serversPath, JsonConvert.SerializeObject(ConfigUtils.servers, (Formatting)1));
				File.WriteAllText(ConfigUtils.configPath, JsonConvert.SerializeObject(ConfigUtils.config, (Formatting)1));
			}
		}

		public static IDbConnection LoadDb()
		{
			IDbConnection result;
			try
			{
				string[] array = ConfigUtils.config.HostPort.Split(':', StringSplitOptions.None);
				result = new MySqlConnection
				{
					ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};", new object[]
					{
						array[0],
						(array.Length > 1) ? array[1] : "3306",
						ConfigUtils.config.DataBaseName,
						ConfigUtils.config.DBUserName,
						ConfigUtils.config.DBPassword
					})
				};
			}
			catch (MySqlException)
			{
				throw new Exception("Mysql未正确配置");
			}
			return result;
		}

		public static void UpdateConfig()
		{
			File.WriteAllText(ConfigUtils.configPath, JsonConvert.SerializeObject(ConfigUtils.config, (Formatting)1));
		}

		public static void UpdateServer()
		{
			File.WriteAllText(ConfigUtils.serversPath, JsonConvert.SerializeObject(ConfigUtils.servers, (Formatting)1));
		}

		public static readonly string configDir = TShock.SavePath + "/CNSUniCore";

		public static readonly string configPath = ConfigUtils.configDir + "/config.json";

		public static readonly string serversPath = ConfigUtils.configDir + "/servers.json";

		public static UConfig config = new UConfig();

		public static List<ServerInfo> servers = new List<ServerInfo>();
	}
}
