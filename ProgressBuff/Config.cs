using Newtonsoft.Json;

namespace ProgressBuff;

public class Scheme
{
    public string SchemeName = "默认方案";
    public HashSet<string> SkipProgressDetection = new HashSet<string>();
    public HashSet<string> SkipRemoteDetection = new HashSet<string>();
    public Dictionary<string, HashSet<int>> ProgressBuff = new Dictionary<string, HashSet<int>>();
}
public class Config
{
    public string UseScheme = "默认方案";
    public bool Enabled = true;
    public bool Broadcast = true;
    public bool WriteLog = true;
    public bool ClearBuff = true;
    public List<Scheme> Schemes = new List<Scheme>();



    public static Config Read(string Path)
    {
        if (!File.Exists(Path))
        {
            return new Config();
        }

        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Read(fs);
    }

    public static Config Read(Stream stream)
    {
        using var sr = new StreamReader(stream);
        var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
        return cf ?? new Config();
    }

    public void Write(string Path)
    {
        using var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fs);
    }

    public void Write(Stream stream)
    {
        var str = JsonConvert.SerializeObject(this, Formatting.Indented);
        using var sw = new StreamWriter(stream);
        sw.Write(str);
    }
}