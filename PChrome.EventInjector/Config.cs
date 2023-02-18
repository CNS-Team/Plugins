using LazyUtils;

namespace PChrome.EventInjector;

public class Config : Config<Config>
{
    public Dictionary<string, string>? events;
}