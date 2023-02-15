using System.IO;
using Newtonsoft.Json;

namespace MulitRegister;

public class Config
{
	[JsonProperty("主城MySQLIP地址")]
	public string MysqlIP { get; set; } = "127.0.0.1";


	[JsonProperty("主城MySQL端口")]
	public int MysqlPort { get; set; } = 3306;


	[JsonProperty("主城MySQL用户名")]
	public string MysqlUsername { get; set; } = "";


	[JsonProperty("主城MySQL密码")]
	public string MysqlPassword { get; set; } = "";


	[JsonProperty("主城MySQL数据库名")]
	public string MysqlDb { get; set; } = "";


	public void Save()
	{
		using StreamWriter streamWriter = new StreamWriter("tshock/MulitRegister.json");
		streamWriter.Write(JsonConvert.SerializeObject((object)this, (Formatting)1));
	}

	public static Config GetConfig()
	{
		Config config = new Config();
		if (!File.Exists("tshock/MulitRegister.json"))
		{
			config.Save();
			return config;
		}
		using (StreamReader streamReader = new StreamReader("tshock/MulitRegister.json"))
		{
			config = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
		}
		return config;
	}
}
