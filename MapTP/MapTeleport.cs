using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace MapTeleport;

[ApiVersion(2, 1)]
public class MapTeleport : TerrariaPlugin
{
    public const string ALLOWED = "maptp";

    public override Version Version => new Version(2, 1);

    public override string Author => "cjx";

    public override string Name => "MapTeleport";

    public override string Description => "Allows players to teleport to a selected location on the map.";

    public override void Initialize()
    {
        GetDataHandlers.ReadNetModule.Register((EventHandler<GetDataHandlers.ReadNetModuleEventArgs>) this.teleport, (HandlerPriority) 3, false);
    }

    public MapTeleport(Main game)
        : base(game)
    {
        ((TerrariaPlugin) this).Order = 1;
    }

    private void teleport(object unused, GetDataHandlers.ReadNetModuleEventArgs args)
    {
        //IL_0019: Unknown result type (might be due to invalid IL or missing references)
        //IL_001f: Invalid comparison between Unknown and I4
        //IL_0038: Unknown result type (might be due to invalid IL or missing references)
        //IL_003d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0045: Unknown result type (might be due to invalid IL or missing references)
        //IL_0052: Unknown result type (might be due to invalid IL or missing references)
        if (((GetDataHandledEventArgs) args).Player.HasPermission("maptp") && (int) args.ModuleType == 2)
        {
            using (var binaryReader = new BinaryReader(((GetDataHandledEventArgs) args).Data))
            {
                var val = Terraria.Utils.ReadVector2(binaryReader);
                ((GetDataHandledEventArgs) args).Player.Teleport(val.X * 16f, val.Y * 16f, (byte) 1);
            }
        }
    }
}