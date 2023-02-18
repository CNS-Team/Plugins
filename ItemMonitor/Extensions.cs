using TShockAPI;

namespace ItemMonitor;

internal static class Extensions
{
    public static ItemValidator GetValidator(this TSPlayer player)
    {
        return player.GetData<ItemValidator>("ItemValidator");
    }
}