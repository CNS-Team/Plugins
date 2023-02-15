using System;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace KeyChanger;

public class Config
{
	public bool UseSSC = true;

	public int NumberOfKeys = 1;

	public bool EnableRegionExchanges = false;

	public bool MarketMode = false;

	public bool EnableTempleKey = true;

	public bool EnableJungleKey = true;

	public bool EnableCorruptionKey = true;

	public bool EnableCrimsonKey = true;

	public bool EnableHallowedKey = true;

	public bool EnableFrozenKey = true;

	public bool EnableDesertKey = true;

	public int[] TempleKeyItem = new int[1] { 1293 };

	public int[] JungleKeyItem = new int[1] { 1156 };

	public int[] CorruptionKeyItem = new int[1] { 1571 };

	public int[] CrimsonKeyItem = new int[1] { 1569 };

	public int[] HallowedKeyItem = new int[1] { 1260 };

	public int[] FrozenKeyItem = new int[1] { 1572 };

	public int[] DesertKeyItem = new int[1] { 4607 };

	public string MarketRegion = null;

	public string TempleRegion = null;

	public string JungleRegion = null;

	public string CorruptionRegion = null;

	public string CrimsonRegion = null;

	public string HallowedRegion = null;

	public string FrozenRegion = null;

	public string DesertRegion = null;

	public static Config Read(string savepath = "")
	{
		if (string.IsNullOrWhiteSpace(savepath))
		{
			savepath = TShock.SavePath;
		}
		savepath = Path.Combine(savepath, "KeyChangerConfig.json");
		Directory.CreateDirectory(Path.GetDirectoryName(savepath));
		try
		{
			Config config = new Config();
			if (File.Exists(savepath))
			{
				config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(savepath));
			}
			else
			{
				TShock.Log.ConsoleInfo("Creating config file for KeyChangerSSC...");
			}
			File.WriteAllText(savepath, JsonConvert.SerializeObject((object)config, (Formatting)1));
			return config;
		}
		catch (Exception ex)
		{
			TShock.Log.ConsoleError(ex.ToString());
			return new Config();
		}
	}

	public bool Write(string savepath = "")
	{
		if (string.IsNullOrWhiteSpace(savepath))
		{
			savepath = TShock.SavePath;
		}
		savepath = Path.Combine(savepath, "KeyChangerConfig.json");
		Directory.CreateDirectory(Path.GetDirectoryName(savepath));
		try
		{
			File.WriteAllText(savepath, JsonConvert.SerializeObject((object)this, (Formatting)1));
			return true;
		}
		catch (Exception ex)
		{
			TShock.Log.ConsoleError(ex.ToString());
			return false;
		}
	}
}
