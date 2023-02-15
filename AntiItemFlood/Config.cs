using System.IO;
using Newtonsoft.Json;

namespace AntiItemFlood;

internal class Config
{
	private const string path = "tshock/AntiItemFlood.json";

	private static Config _config = GetConfigInternal();

	public bool Multiplekickout { get; set; } = true;


	public int MultipleCount { get; set; } = 3;


	public void Save()
	{
		using StreamWriter streamWriter = new StreamWriter("tshock/AntiItemFlood.json");
		streamWriter.Write(JsonConvert.SerializeObject((object)this, (Formatting)1));
	}

	public static Config GetConfig()
	{
		if (_config == null)
		{
			_config = GetConfigInternal();
		}
		return _config;
	}

	private static Config GetConfigInternal()
	{
		Config config = new Config();
		if (File.Exists("tshock/AntiItemFlood.json"))
		{
			using (StreamReader streamReader = new StreamReader("tshock/AntiItemFlood.json"))
			{
				return JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
			}
		}
		config.Save();
		return config;
	}
}
