using Newtonsoft.Json;

namespace AntiltemCheating;

public class Scheme
{
    public string SchemeName = "默认方案";
    public List<string> SkipProgressDetection = new List<string>();
    public List<string> SkipRemoteDetection = new List<string>();
    public Dictionary<string, List<int>> AntiltemCheating = new Dictionary<string, List<int>>();
}
public class Config
{
    public string UseScheme = "默认方案";
    public bool Enabled = true;
    public bool punishPlayer = true;
    public int punishTime = 5;
    public bool Broadcast = true;
    public bool WriteLog = true;
    public bool ClearItem = true;
    public bool KickPlayer = false;
    public bool AccurateDetection = true;
    public List<Scheme> Schemes = new List<Scheme>();



    public static Config Read(string Path)//给定文件进行读
    {
        if (!File.Exists(Path)) return new Config();
        using (var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
        { return Read(fs); }
    }
    public static Config Read(Stream stream)//给定流文件进行读取
    {
        using (var sr = new StreamReader(stream))
        {
            var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            if (ConfigR != null)
                ConfigR(cf);
            return cf;
        }
    }
    public void Write(string Path)//给定路径进行写
    {
        using (var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
        { this.Write(fs); }
    }
    public void Write(Stream stream)//给定流文件写
    {
        var str = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (var sw = new StreamWriter(stream))
        { sw.Write(str); }
    }
    public static Action<Config> ConfigR;//定义为常量
}