using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BetterWhitelist;

public class BConfig
{
	public List<string> WhitePlayers = new List<string>();

	public bool Disabled { get; set; }

	public static BConfig Load(string path)
	{
		if (File.Exists(path))
		{
			return JsonConvert.DeserializeObject<BConfig>(File.ReadAllText(path));
		}
		return new BConfig
		{
			Disabled = false
		};
	}
}
