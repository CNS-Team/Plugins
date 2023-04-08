using Microsoft.Xna.Framework;
using TShockAPI;

namespace AdminExtension;

public static class TSPlayerExtensions
{
    public static PlayerInfo GetPlayerInfo(this TSPlayer player)
    {
        if (!player.ContainsData("AdminExtension_Data"))
        {
            player.SetData<PlayerInfo>("AdminExtension_Data", new PlayerInfo());
        }
        return player.GetData<PlayerInfo>("AdminExtension_Data");
    }

    public static void PluginMessage(this TSPlayer player, string message, Color color)
    {
        //IL_000d: Unknown result type (might be due to invalid IL or missing references)
        player.SendMessage(AdminExtension.Tag + message, color);
    }

    public static void PluginErrorMessage(this TSPlayer player, string message)
    {
        //IL_0003: Unknown result type (might be due to invalid IL or missing references)
        player.PluginMessage(message, Color.Red);
    }

    public static void PluginInfoMessage(this TSPlayer player, string message)
    {
        //IL_0003: Unknown result type (might be due to invalid IL or missing references)
        player.PluginMessage(message, Color.Yellow);
    }

    public static void PluginSuccessMessage(this TSPlayer player, string message)
    {
        //IL_0003: Unknown result type (might be due to invalid IL or missing references)
        player.PluginMessage(message, Color.Green);
    }

    public static void PluginWarningMessage(this TSPlayer player, string message)
    {
        //IL_0003: Unknown result type (might be due to invalid IL or missing references)
        player.PluginMessage(message, Color.OrangeRed);
    }
}