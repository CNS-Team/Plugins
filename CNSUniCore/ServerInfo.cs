using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using CNSUniCore.UniInfos;
using Newtonsoft.Json.Linq;

namespace CNSUniCore
{
	// Token: 0x02000006 RID: 6
	public class ServerInfo
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002FC1 File Offset: 0x000011C1
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002FC9 File Offset: 0x000011C9
		public int ID { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002FD2 File Offset: 0x000011D2
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002FDA File Offset: 0x000011DA
		public string Name { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002FE3 File Offset: 0x000011E3
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002FEB File Offset: 0x000011EB
		public string IP { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002FF4 File Offset: 0x000011F4
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002FFC File Offset: 0x000011FC
		public int Port { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00003005 File Offset: 0x00001205
		// (set) Token: 0x06000030 RID: 48 RVA: 0x0000300D File Offset: 0x0000120D
		public string RestUser { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00003016 File Offset: 0x00001216
		// (set) Token: 0x06000032 RID: 50 RVA: 0x0000301E File Offset: 0x0000121E
		public string RestPwd { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003027 File Offset: 0x00001227
		// (set) Token: 0x06000034 RID: 52 RVA: 0x0000302F File Offset: 0x0000122F
		public string Token { get; set; }

		// Token: 0x06000035 RID: 53 RVA: 0x00003038 File Offset: 0x00001238
		public ServerInfo()
		{
			this.ID = 1;
			this.Name = "实例服务器";
			this.IP = "127.0.0.1";
			this.Port = 7878;
			this.RestUser = "豆沙666";
			this.RestPwd = "dousha666";
			this.Token = "";
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000030A0 File Offset: 0x000012A0
		public JObject BanUser(string name, string ip, string uuid , string reason,string time)
		{
			bool flag = this.TestToken();
			JObject result;
			if (flag)
			{
				string url = string.Concat(new string[]
				{
					this.GetUrl(false),
					"/uniban/add?ip=",
					ip,
					"&uuid=",
					uuid,
					"&name=",
					name,
                    "&reason=",
                    reason,
                   "&time=",
                    time,
                    "&token=",
					this.Token
				});
				result = this.GetHttp(url);
			}
			else
			{
				result = null;
			}
			return result;
		}
        // Token: 0x06000037 RID: 55 RVA: 0x00003114 File Offset: 0x00001314
        public JObject AddVip(SponsorInfo sponsorInfo)
		{
			bool flag = this.TestToken();
			JObject result;
			if (flag)
			{
				string url = string.Concat(new string[]
				{
					this.GetUrl(false),
					"/univip/add?name=",
					sponsorInfo.name,
					"&group=",
					sponsorInfo.group,
					"&origin=",
					sponsorInfo.originGroup,
					"&start=",
					sponsorInfo.startTime.ToString(),
					"&end=",
					sponsorInfo.endTime.ToString(),
					"&token=",
					this.Token
				});
				result = this.GetHttp(url);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000031CC File Offset: 0x000013CC
		public JObject DelVip(string name)
		{
			bool flag = this.TestToken();
			JObject result;
			if (flag)
			{
				string url = string.Concat(new string[]
				{
					this.GetUrl(false),
					"/univip/del?name=",
					name,
					"&token=",
					this.Token
				});
				result = this.GetHttp(url);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003228 File Offset: 0x00001428
		public JObject DelUser(string name)
		{
			bool flag = this.TestToken();
			JObject result;
			if (flag)
			{
				string url = string.Concat(new string[]
				{
					this.GetUrl(false),
					"/uniban/del?name=",
					name,
					"&token=",
					this.Token
				});
				result = this.GetHttp(url);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003284 File Offset: 0x00001484
		public string GetUrl(bool https = false)
		{
			bool flag = !https;
			string result;
			if (flag)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
				defaultInterpolatedStringHandler.AppendLiteral("http://");
				defaultInterpolatedStringHandler.AppendFormatted(this.IP);
				defaultInterpolatedStringHandler.AppendLiteral(":");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.Port);
				result = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
				defaultInterpolatedStringHandler.AppendLiteral("https://");
				defaultInterpolatedStringHandler.AppendFormatted(this.IP);
				defaultInterpolatedStringHandler.AppendLiteral(":");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.Port);
				result = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return result;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003330 File Offset: 0x00001530
		public bool TestToken()
		{
			string url = this.GetUrl(false) + "/tokentest?token=" + this.Token;
			return this.GetHttp(url)["status"].ToString() == "200";
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000337C File Offset: 0x0000157C
		public void CreateToken()
		{
			bool flag = string.IsNullOrEmpty(this.Token);
			if (flag)
			{
				string url = string.Concat(new string[]
				{
					this.GetUrl(false),
					"/v2/token/create?username=",
					this.RestUser,
					"&password=",
					this.RestPwd
				});
				JObject http = this.GetHttp(url);
				this.Token = http["token"].ToString();
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000033F4 File Offset: 0x000015F4
		public JObject GetHttp(string url)
		{
			return JObject.Parse(new HttpClient().GetAsync(url).Result.Content.ReadAsStringAsync().Result);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000342C File Offset: 0x0000162C
		public JObject RawCommand(string cmd)
		{
			bool flag = this.TestToken();
			JObject result;
			if (flag)
			{
				string url = string.Concat(new string[]
				{
					this.GetUrl(false),
					"/v3/server/rawcmd?cmd=",
					cmd,
					"&token=",
					this.Token
				});
				result = this.GetHttp(url);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003488 File Offset: 0x00001688
		public JObject AddUser(UserInfo info)
		{
			bool flag = this.TestToken();
			JObject result;
			if (flag)
			{
				string url = this.GetUrl(false);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(81, 9);
				defaultInterpolatedStringHandler.AppendLiteral("/unireg/add?id&=");
				defaultInterpolatedStringHandler.AppendFormatted<int>(info.ID);
				defaultInterpolatedStringHandler.AppendLiteral("name=");
				defaultInterpolatedStringHandler.AppendFormatted(info.Name);
				defaultInterpolatedStringHandler.AppendLiteral("&lastaccessed=");
				defaultInterpolatedStringHandler.AppendFormatted(info.LastAccessed);
				defaultInterpolatedStringHandler.AppendLiteral("&ip=");
				defaultInterpolatedStringHandler.AppendFormatted(info.KnownIPs);
				defaultInterpolatedStringHandler.AppendLiteral("&group=");
				defaultInterpolatedStringHandler.AppendFormatted(info.UserGroup);
				defaultInterpolatedStringHandler.AppendLiteral("&password=");
				defaultInterpolatedStringHandler.AppendFormatted(info.Password);
				defaultInterpolatedStringHandler.AppendLiteral("&uuid=");
				defaultInterpolatedStringHandler.AppendFormatted(info.UUID);
				defaultInterpolatedStringHandler.AppendLiteral("&registered=");
				defaultInterpolatedStringHandler.AppendFormatted(info.Registered);
				defaultInterpolatedStringHandler.AppendLiteral("&token=");
				defaultInterpolatedStringHandler.AppendFormatted(this.Token);
				string url2 = url + defaultInterpolatedStringHandler.ToStringAndClear();
				result = this.GetHttp(url2);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000035C8 File Offset: 0x000017C8
		public JObject UpdateUser(UserInfo info)
		{
			bool flag = this.TestToken();
			JObject result;
			if (flag)
			{
				string url = string.Concat(new string[]
				{
					this.GetUrl(false),
					"/unireg/update?name=",
					info.Name,
					"&uuid=",
					info.UUID,
					"&group=",
					info.UserGroup,
					"&registered=",
					info.Registered,
					"&lastaccessed=",
					info.LastAccessed,
					"&ip=",
					info.KnownIPs,
					"&token=",
					this.Token
				});
				result = this.GetHttp(url);
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
