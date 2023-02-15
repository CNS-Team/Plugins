using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace ProgressKitsV2;

internal static class ConfigUtils
{
	public static string configDir = TShock.SavePath + "/ProgressKitsV2";

	public static string kitPath = configDir + "/kits.json";

	public static string playerPath = configDir + "/players.json";

	public static string firstKitsPath = configDir + "/firstkits.json";

	public static List<PKit> loadedKits = new List<PKit>();

	public static List<KitPlayer> players = new List<KitPlayer>();

	[JsonProperty("新人给予礼包")]
	public static List<int> firstKits = new List<int>();

	public static void LoadConfig()
	{
		if (!Directory.Exists(configDir))
		{
			Directory.CreateDirectory(configDir);
			File.WriteAllText(kitPath, JsonConvert.SerializeObject((object)loadedKits, (Formatting)1));
			File.WriteAllText(playerPath, JsonConvert.SerializeObject((object)players, (Formatting)1));
			File.WriteAllText(firstKitsPath, JsonConvert.SerializeObject((object)firstKits, (Formatting)1));
			return;
		}
		if (File.Exists(kitPath))
		{
			loadedKits = JsonConvert.DeserializeObject<List<PKit>>(File.ReadAllText(kitPath));
		}
		else
		{
			File.WriteAllText(kitPath, JsonConvert.SerializeObject((object)loadedKits, (Formatting)1));
		}
		if (File.Exists(playerPath))
		{
			players = JsonConvert.DeserializeObject<List<KitPlayer>>(File.ReadAllText(playerPath));
		}
		else
		{
			File.WriteAllText(playerPath, JsonConvert.SerializeObject((object)players, (Formatting)1));
		}
		if (File.Exists(firstKitsPath))
		{
			firstKits = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(firstKitsPath));
		}
		else
		{
			File.WriteAllText(firstKitsPath, JsonConvert.SerializeObject((object)firstKits, (Formatting)1));
		}
	}

	public static KitPlayer GetPlayerByID(int id)
	{
		return players.Find((KitPlayer p) => p.id == id);
	}

	public static KitPlayer GetPlayerByName(string name)
	{
		string name2 = name;
		return players.Find((KitPlayer p) => p.name == name2);
	}

	public static PKit GetKitByID(int id)
	{
		return loadedKits.Find((PKit k) => k.id == id);
	}

	public static PKit GetKitByName(string name)
	{
		string name2 = name;
		return loadedKits.Find((PKit k) => k.name == name2);
	}

	public static void UpdatePlayer()
	{
		File.WriteAllText(playerPath, JsonConvert.SerializeObject((object)players, (Formatting)1));
	}

	public static void UpdateKits()
	{
		File.WriteAllText(kitPath, JsonConvert.SerializeObject((object)loadedKits, (Formatting)1));
	}

	public static void UpdateFirst()
	{
		File.WriteAllText(firstKitsPath, JsonConvert.SerializeObject((object)firstKits, (Formatting)1));
	}
}
