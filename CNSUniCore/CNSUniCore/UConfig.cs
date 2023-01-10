using System;
using Newtonsoft.Json;
using TShockAPI;

namespace CNSUniCore
{
	// Token: 0x02000008 RID: 8
	public class UConfig
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000036B6 File Offset: 0x000018B6
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000036BE File Offset: 0x000018BE
		[JsonProperty("开启赞助功能")]
		public bool EnableSponsor { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000036C7 File Offset: 0x000018C7
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000036CF File Offset: 0x000018CF
		[JsonProperty("开启封禁功能")]
		public bool EnableBan { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000036D8 File Offset: 0x000018D8
		// (set) Token: 0x06000047 RID: 71 RVA: 0x000036E0 File Offset: 0x000018E0
		[JsonProperty("开启多服注册功能")]
		public bool EnableRegister { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000036E9 File Offset: 0x000018E9
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000036F1 File Offset: 0x000018F1
		[JsonProperty("数据库地址")]
		public string HostPort { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000036FA File Offset: 0x000018FA
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00003702 File Offset: 0x00001902
		[JsonProperty("数据库名称")]
		public string DataBaseName { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004C RID: 76 RVA: 0x0000370B File Offset: 0x0000190B
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00003713 File Offset: 0x00001913
		[JsonProperty("数据库用户名")]
		public string DBUserName { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600004E RID: 78 RVA: 0x0000371C File Offset: 0x0000191C
		// (set) Token: 0x0600004F RID: 79 RVA: 0x00003724 File Offset: 0x00001924
		[JsonProperty("数据库密码")]
		public string DBPassword { get; set; }

		// Token: 0x06000050 RID: 80 RVA: 0x00003730 File Offset: 0x00001930
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
