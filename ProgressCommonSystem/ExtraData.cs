using System.IO;
using TShockAPI;

namespace ProgressCommonSystem;

internal class ExtraData
{
    public static readonly string SavePath = Path.Combine(TShock.SavePath, "event_data");

    public static ExtraData Instance;

    public bool downedBloodMoon;

    public bool downedPumpkinMoon;

    public bool downedFrostMoon;

    public bool downedSolarEclipse;
}