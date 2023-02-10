using LazyUtils;
using OTAPI;
using Terraria;
using TerrariaApi.Server;

namespace AsyncSocket;

[ApiVersion(2, 1)]
public class PluginContainer : LazyPlugin
{
    public PluginContainer(Main game) : base(game)
    {
        this.Order = 10;
    }

    public override void Initialize()
    {
        base.Initialize();
        Hooks.Netplay.CreateTcpListener += this.Netplay_CreateTcpListener;
    }

    private void Netplay_CreateTcpListener(object sender, Hooks.Netplay.CreateTcpListenerEventArgs e)
    {
        e.Result = new AsyncSocket();
    }
}