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
		reloading = true;
		try
		{
			client?.Dispose();
		}
		catch
		{
		}
		config = new MainServerConfig();
		try
		{
			config = JsonConvert.DeserializeObject<MainServerConfig>(File.ReadAllText(Path.Combine("tshock", "chatsharing.json")));
		}
		catch
		{
			File.WriteAllText(Path.Combine("tshock", "chatsharing.json"), JsonConvert.SerializeObject((object)config));
		}
        this.client = new GameServer(config.host, config.port);
        this.client.SetName(config.servername);
        this.client.OnMessage += delegate(string msg, uint clr)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Color val = default;
			val.PackedValue = clr;
			ChatHelper.BroadcastChatMessage((NetworkText)(object)new GroupNetworkText(msg), val, -1);
		};
		TShock.Log.ConsoleInfo("reload completed.");
		reloading = false;
	}

	public override void Initialize()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		ServerApi.Hooks.ServerBroadcast.Register((TerrariaPlugin)(object)this, (HookHandler<ServerBroadcastEventArgs>)delegate(ServerBroadcastEventArgs args)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Invalid comparison between Unknown and I4
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (!(args.Message is GroupNetworkText) && (int)args.Message._mode == 0)
			{
				GameServer gameServer = client;
				string message = string.Format(config.format, config.servername, args.Message._text);
				Color color = args.Color;
				gameServer.SendMsg(message, color.PackedValue);
			}
		});
		Reload();
		Commands.ChatCommands.Add(new Command("chatsharing.reload", (CommandDelegate)delegate(CommandArgs args)
		{
			try
			{
				Reload();
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
		client.Dispose();
	}
}
