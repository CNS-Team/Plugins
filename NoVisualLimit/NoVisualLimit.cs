using System;
using System.ComponentModel;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;

namespace NoVisualLimit;

[ApiVersion(2, 1)]
public class NoVisualLimit : TerrariaPlugin
{
    public override string Author => "Leader";

    public override string Description => "取消服务器的版本验证";

    public override string Name => "NoVisualLimit";

    public override Version Version => new Version(1, 0, 0, 0);

    public NoVisualLimit(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register((TerrariaPlugin) (object) this, (HookHandler<GetDataEventArgs>) this.OnNetGetData);
    }

    private void OnNetGetData(GetDataEventArgs args)
    {
        //IL_0002: Unknown result type (might be due to invalid IL or missing references)
        //IL_0008: Invalid comparison between Unknown and I4
        if ((int) args.MsgID != 1)
        {
            return;
        }
        ((HandledEventArgs) (object) args).Handled = true;
        if (Main.netMode != 2)
        {
            return;
        }
        if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[args.Msg.whoAmI].Socket.GetRemoteAddress()))
        {
            NetMessage.TrySendData(2, args.Msg.whoAmI, -1, Lang.mp[3].ToNetworkText(), 0, 0f, 0f, 0f, 0, 0, 0);
        }
        else if (Netplay.Clients[args.Msg.whoAmI].State == 0)
        {
            if (string.IsNullOrEmpty(Netplay.ServerPassword))
            {
                Netplay.Clients[args.Msg.whoAmI].State = 1;
                NetMessage.TrySendData(3, args.Msg.whoAmI, -1, (NetworkText) null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
            else
            {
                Netplay.Clients[args.Msg.whoAmI].State = -1;
                NetMessage.TrySendData(37, args.Msg.whoAmI, -1, (NetworkText) null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister((TerrariaPlugin) (object) this, (HookHandler<GetDataEventArgs>) this.OnNetGetData);
        }
        this.Dispose(disposing);
    }
}