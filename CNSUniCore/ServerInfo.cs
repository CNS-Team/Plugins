using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using CNSUniCore.UniInfos;
using Newtonsoft.Json.Linq;

namespace CNSUniCore
{
	public class ServerInfo
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public string IP { get; set; }
		public int Port { get; set; }
		public string RestUser { get; set; }

		public string RestPwd { get; set; }

		public string Token { get; set; }

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

		public bool TestToken()
		{
			string url = this.GetUrl(false) + "/tokentest?token=" + this.Token;
			return this.GetHttp(url)["status"].ToString() == "200";
		}

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
		public JObject GetHttp(string url)
		{
			return JObject.Parse(new HttpClient().GetAsync(url).Result.Content.ReadAsStringAsync().Result);
		}
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
