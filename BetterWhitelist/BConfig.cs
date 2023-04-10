using Newtonsoft.Json;

namespace BetterWhitelist;

public class BConfig
{
    public List<string> WhitePlayers = new List<string>();

    public bool Disabled { get; set; }

    public static BConfig Load(string path)
    {
        return File.Exists(path)
            ? JsonConvert.DeserializeObject<BConfig>(File.ReadAllText(path))!
            : new BConfig
            {
                Disabled = false
            };
    }
}