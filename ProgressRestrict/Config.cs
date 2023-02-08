using DataSync;
using Newtonsoft.Json;
using TShockAPI;

namespace AntiProjecttileCheating;

public class Scheme
{
    public List<int> Restricted = new();
    public ProgressType Progress;
    public bool AllowRemoteUnlocked;
}

public class Config
{
    public bool PunishPlayer = true;
    public int PunishTime = 5;
    public bool Broadcast = true;
    public bool WriteLog = true;
    public bool ClearItem = true;
    public bool ClearBuff = true;
    public bool KickPlayer = false;
    public List<Scheme> Projectiles = new List<Scheme>();
    public List<Scheme> Items = new List<Scheme>();
    public List<Scheme> Buffs = new List<Scheme>();

    public static Config LoadConfig(string path)
    {
        var result = new Config();
        try
        {
            if (!File.Exists(path))
            {
                FileTools.CreateIfNot(path, JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            result = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path))!;
            File.WriteAllText(path, JsonConvert.SerializeObject(result, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
        }
        return result;
    }
}