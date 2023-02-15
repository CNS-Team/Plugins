using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MulitRegister;

public static class Utils
{
	public static JObject GetHttp(string uri)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		HttpClient val = new HttpClient();
		try
		{
			return JObject.Parse(val.GetAsync(uri).Result.Content.ReadAsStringAsync().Result);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static string RawCmd(Server server, string cmd)
	{
		try
		{
			JObject http = GetHttp("http://" + server.Host + ":" + server.RestPort + "/v3/server/rawcmd?token=" + server.Token + "&cmd=" + cmd);
			return ((object)http["response"]).ToString();
		}
		catch
		{
			return "";
		}
	}

	public static void Register(Server server, string name, string pass, string group)
	{
		JObject http = GetHttp("http://" + server.Host + ":" + server.RestPort + "/v2/users/create?user=" + name + "&password=" + pass + "&group=" + group + "&token=" + server.Token);
	}
}
