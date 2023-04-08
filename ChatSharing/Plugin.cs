using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.IO;
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
            this.config = JsonConvert.DeserializeObject<MainServerConfig>(File.ReadAllText(Path.Combine("tshock", "chatsharing.json")))!;
        }
        catch
        {
            File.WriteAllText(Path.Combine("tshock", "chatsharing.json"), JsonConvert.SerializeObject(this.config));
        }
        this.client = new GameServer(this.config.host, this.config.port);
        this.client.SetName(this.config.servername);
        this.client.OnMessage += (string msg, uint clr) =>
        {
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            Color val = default;
            val.PackedValue = clr;
            ChatHelper.BroadcastChatMessage(new GroupNetworkText(msg), val, -1);
        };
        TShock.Log.ConsoleInfo("reload completed.");
        this.reloading = false;
    }

    public override void Initialize()
    {
        //IL_0036: Unknown result type (might be due to invalid IL or missing references)
        //IL_004e: Expected O, but got Unknown
        //IL_0049: Unknown result type (might be due to invalid IL or missing references)
        //IL_0053: Expected O, but got Unknown
        ServerApi.Hooks.ServerBroadcast.Register(this, (ServerBroadcastEventArgs args) =>
        {
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_001a: Invalid comparison between Unknown and I4
            //IL_0051: Unknown result type (might be due to invalid IL or missing references)
            //IL_0056: Unknown result type (might be due to invalid IL or missing references)
            if (!(args.Message is GroupNetworkText) && (int) args.Message._mode == 0)
            {
                var gameServer = this.client;
                var message = string.Format(this.config.format, this.config.servername, args.Message._text);
                var color = args.Color;
                gameServer.SendMsg(message, color.PackedValue);
            }
        });
        this.Reload();
        Commands.ChatCommands.Add(new Command("chatsharing.reload", (CommandArgs args) =>
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
        }, new string[1] { "csreload" }));
    }

    protected override void Dispose(bool disposing)
    {
        this.Dispose(disposing);
        this.client.Dispose();
    }
}