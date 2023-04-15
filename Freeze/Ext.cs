using Microsoft.Xna.Framework;
using TShockAPI;

namespace ServerTool;
public static class Ext
{
    public static void PluginMessage(this TSPlayer player, string message, Color color)
    {
        player.SendMessage(Plugin.Tag + message, color);
    }

    public static void PluginErrorMessage(this TSPlayer player, string message)
    {
        player.PluginMessage(message, Color.Red);
    }

    public static void PluginInfoMessage(this TSPlayer player, string message)
    {
        player.PluginMessage(message, Color.Yellow);
    }

    public static void PluginSuccessMessage(this TSPlayer player, string message)
    {
        player.PluginMessage(message, Color.Green);
    }

    public static void PluginWarningMessage(this TSPlayer player, string message)
    {
        player.PluginMessage(message, Color.OrangeRed);
    }

}
