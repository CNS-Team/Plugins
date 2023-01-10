using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TShockAPI;

namespace CNSUniCore
{
	// Token: 0x02000002 RID: 2
	public static class ConfigUtils
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
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

		// Token: 0x06000002 RID: 2 RVA: 0x00002148 File Offset: 0x00000348
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

		// Token: 0x06000003 RID: 3 RVA: 0x000021EC File Offset: 0x000003EC
		public static void UpdateConfig()
		{
			File.WriteAllText(ConfigUtils.configPath, JsonConvert.SerializeObject(ConfigUtils.config, (Formatting)1));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002205 File Offset: 0x00000405
		public static void UpdateServer()
		{
			File.WriteAllText(ConfigUtils.serversPath, JsonConvert.SerializeObject(ConfigUtils.servers, (Formatting)1));
		}

		// Token: 0x04000001 RID: 1
		public static readonly string configDir = TShock.SavePath + "/CNSUniCore";

		// Token: 0x04000002 RID: 2
		public static readonly string configPath = ConfigUtils.configDir + "/config.json";

		// Token: 0x04000003 RID: 3
		public static readonly string serversPath = ConfigUtils.configDir + "/servers.json";

		// Token: 0x04000004 RID: 4
		public static UConfig config = new UConfig();

		// Token: 0x04000005 RID: 5
		public static List<ServerInfo> servers = new List<ServerInfo>();
	}
}
