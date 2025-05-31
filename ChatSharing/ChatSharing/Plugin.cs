using System;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace ChatSharing;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    private GameServer client;

    private MainServerConfig config;

    private bool reloading;

    public override string Name => "ChatSharing";

    public Plugin(Main game)
        : base(game)
    {
    }

    private void Reload()
    {
        this.reloading = true;
        try
        {
            this.client?.Dispose();
        }
        catch
        {
        }

        this.config = new MainServerConfig();
        try
        {
            this.config = JsonConvert.DeserializeObject<MainServerConfig>(File.ReadAllText(Path.Combine("tshock", "chatsharing.json")));
        }
        catch
        {
            File.WriteAllText(Path.Combine("tshock", "chatsharing.json"), JsonConvert.SerializeObject((object) this.config));
        }

        this.client = new GameServer(this.config.host, this.config.port);
        this.client.SetName(this.config.servername);
        this.client.OnMessage += delegate(string msg, uint clr)
        {
            ChatHelper.BroadcastChatMessage(color: new Color(clr), text: new GroupNetworkText(msg));
        };
        TShock.Log.ConsoleInfo("reload completed.");
        this.reloading = false;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerBroadcast.Register(this, delegate(ServerBroadcastEventArgs args)
        {
            if (!(args.Message is GroupNetworkText) && args.Message._mode == NetworkText.Mode.Literal)
            {
                this.client.SendMsg(string.Format(this.config.format, this.config.servername, args.Message._text), args.Color.PackedValue);
            }
        });
        this.Reload();
        Commands.ChatCommands.Add(new Command("chatsharing.reload", delegate(CommandArgs args)
        {
            try
            {
                this.Reload();
                args.Player.SendInfoMessage("reload completed.");
            }
            catch (Exception ex)
            {
                args.Player.SendErrorMessage(ex.ToString());
            }
        }, "csreload"));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this.client.Dispose();
    }
}