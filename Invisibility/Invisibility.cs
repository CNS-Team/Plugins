
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Creative;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class PluginContainer : TerrariaPlugin
{
    public override string Name => "Invisibility";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public override string Author => "1413";

    public PluginContainer(Main game)
        : base(game)
    {

    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("tofout.invisibility.on", cmd => this.ToGhost(cmd.Player), "invon"));
        Commands.ChatCommands.Add(new Command("tofout.invisibility.off", cmd => this.ToActive(cmd.Player), "invoff"));
    }

    protected override void Dispose(bool disposing)
    {
        Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("invon"));
        Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("invoff"));
    }

    private void ToGhost(TSPlayer tsplayer)
    {
        if (tsplayer == null)
        {
            return;
        }
        var index = tsplayer.Index;
        tsplayer.RespawnTimer = int.MaxValue;
        // IsGhost = true;
        NetMessage.SendData(14, -1, index, null, index, false.GetHashCode());
        tsplayer.GodMode = true;
        var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
        power.SetEnabledState(index, true);

        if (false)
        {
            NetMessage.SendData(4, -1, index, null, index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(13, -1, index, null, index, 0f, 0f, 0f, 0, 0, 0);
        }
    }
    public void ToActive(TSPlayer tsplayer)
    {
        var tplayer = tsplayer.TPlayer;
        var index = tsplayer.Index;
        tplayer.active = true;
        // IsGhost = false;
        var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
        power.SetEnabledState(index, false);
        tsplayer.GodMode = false;
        NetMessage.SendData(14, -1, index, null, index, true.GetHashCode());

        if (true)
        {
            NetMessage.SendData(4, -1, index, null, index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(13, -1, index, null, index, 0f, 0f, 0f, 0, 0, 0);
        }
    }
}