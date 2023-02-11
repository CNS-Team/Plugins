using LazyUtils;

namespace PChrome.OnlineTime;

[Config]
public class Config : Config<Config>
{
    public List<string>? groups;
    public string? admingroup;
    public int timetoadmin = 5000;
    public int timetodowgrade = 600;
    public int timestodowgrade = 2;
}