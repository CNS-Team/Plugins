using Terraria.Localization;

namespace ChatSharing;

public class GroupNetworkText : NetworkText
{
    public GroupNetworkText(string text)
        : base(text, (Mode) 0)
    {
    }
}