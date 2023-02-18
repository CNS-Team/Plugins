using LazyUtils;

namespace PChrome.CustomPlayer;

public class Customized : PlayerConfigBase<Customized>
{
    public string? permission { get; set; }
    public string? prefix { get; set; }
    public string? suffix { get; set; }
    public string? color { get; set; }
}