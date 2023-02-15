using System.IO;
using Newtonsoft.Json;

namespace NoSummonArea;

internal class Config
{
	private const string path = "tshock/NoSummonArea.json";

	private static Config _config;

	public int Xmin { get; set; }

	public int Ymin { get; set; }

	public int Xmax { get; set; }

	public int Ymax { get; set; }

	public int[] Projs { get; set; } = new int[1];


	public bool Multiplekickout { get; set; } = true;


	public int MultipleCount { get; set; } = 3;


	public void Save()
	{
		using StreamWriter streamWriter = new StreamWriter("tshock/NoSummonArea.json");
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
		if (File.Exists("tshock/NoSummonArea.json"))
		{
			using (StreamReader streamReader = new StreamReader("tshock/NoSummonArea.json"))
			{
				return JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
			}
		}
		config.Save();
		return config;
	}
}
