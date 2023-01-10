using System;
using Newtonsoft.Json;
using TShockAPI;

namespace CNSUniCore
{
	public class UConfig
	{
		[JsonProperty("开启赞助功能")]
		public bool EnableSponsor { get; set; }

		[JsonProperty("开启封禁功能")]
		public bool EnableBan { get; set; }

		[JsonProperty("开启多服注册功能")]
		public bool EnableRegister { get; set; }

		[JsonProperty("数据库地址")]
		public string HostPort { get; set; }
		[JsonProperty("数据库名称")]
		public string DataBaseName { get; set; }
		[JsonProperty("数据库用户名")]
		public string DBUserName { get; set; }
		[JsonProperty("数据库密码")]
		public string DBPassword { get; set; }
		public UConfig()
		{
			this.EnableBan = true;
			this.EnableSponsor = true;
			this.EnableRegister = true;
			this.HostPort = TShock.Config.Settings.MySqlHost;
			this.DataBaseName = TShock.Config.Settings.MySqlDbName;
			this.DBUserName = TShock.Config.Settings.MySqlUsername;
			this.DBPassword = TShock.Config.Settings.MySqlPassword;
		}
	}
}
