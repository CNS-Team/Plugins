using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using PlaceholderAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace SwitchCommands;

[ApiVersion(2, 1)]
public class SwitchCommands : TerrariaPlugin
{
	public static Database database;

	public override string Name => "SwitchCommands 开关指令";

	public override string Author => "Johuan，奇威复反汉化";

	public override string Description => "触发开关可以执行指令";

	public override Version Version => new Version(1, 3, 0, 0);

	public SwitchCommands(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		database = Database.Read(Database.databasePath);
		if (!File.Exists(Database.databasePath))
		{
			database.Write(Database.databasePath);
		}
		PluginCommands.RegisterCommands();
		ServerApi.Hooks.NetGetData.Register(this, GetData);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.NetGetData.Deregister(this, GetData);
			database.Write(Database.databasePath);
		}
		base.Dispose(disposing);
	}

	private void GetData(GetDataEventArgs args)
	{
		using MemoryStream s = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
		TSPlayer tSPlayer = TShock.Players[args.Msg.whoAmI];
		PacketTypes msgID = args.MsgID;
		PacketTypes packetTypes = msgID;
		if (packetTypes != PacketTypes.HitSwitch)
		{
			return;
		}
		SwitchPos switchPos = new SwitchPos(s.ReadInt16(), s.ReadInt16());
		ITile tile = Main.tile[switchPos.X, switchPos.Y];
		if (tile.type == 132)
		{
			if (tile.frameX % 36 == 0)
			{
				switchPos.X++;
			}
			if (tile.frameY == 0)
			{
				switchPos.Y++;
			}
		}
		switch (tSPlayer.GetData<PluginCommands.PlayerState>("PlayerState"))
		{
		case PluginCommands.PlayerState.SelectingSwitch:
			tSPlayer.SetData("SwitchPos", switchPos);
			tSPlayer.SendSuccessMessage("成功绑定位于X：{0}、Y：{1} 的开关".SFormat(switchPos.X, switchPos.Y));
			tSPlayer.SendSuccessMessage("输入/开关 ，可查看子命令列表".SFormat(switchPos.X, switchPos.Y));
			tSPlayer.SetData("PlayerState", PluginCommands.PlayerState.AddingCommands);
			if (database.switchCommandList.ContainsKey(switchPos.ToString()))
			{
				tSPlayer.SetData("CommandInfo", database.switchCommandList[switchPos.ToString()]);
			}
			break;
		case PluginCommands.PlayerState.None:
		{
			if (!database.switchCommandList.ContainsKey(switchPos.ToString()))
			{
				break;
			}
			double num = 999999.0;
			Dictionary<string, DateTime> dictionary = tSPlayer.GetData<Dictionary<string, DateTime>>("冷却");
			if (dictionary != null && dictionary.ContainsKey(switchPos.ToString()))
			{
				num = (DateTime.Now - tSPlayer.GetData<Dictionary<string, DateTime>>("冷却")[switchPos.ToString()]).TotalMilliseconds / 1000.0;
			}
			if (num < (double)database.switchCommandList[switchPos.ToString()].cooldown)
			{
				break;
			}
			Group group = null;
			bool ignorePerms = database.switchCommandList[switchPos.ToString()].ignorePerms;
			foreach (string command in database.switchCommandList[switchPos.ToString()].commandList)
			{
				if (ignorePerms)
				{
					group = tSPlayer.Group;
					tSPlayer.Group = new SuperAdminGroup();
				}
				try
				{
					string text = global::PlaceholderAPI.PlaceholderAPI.Instance.placeholderManager.GetText(command.ReplaceTags(tSPlayer), tSPlayer);
					Commands.HandleCommand(tSPlayer, text);
					if (ignorePerms)
					{
						tSPlayer.Group = group;
					}
				}
				catch
				{
					Commands.HandleCommand(tSPlayer, command.ReplaceTags(tSPlayer));
					if (ignorePerms)
					{
						tSPlayer.Group = group;
					}
				}
			}
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, DateTime> { 
				{
					switchPos.ToString(),
					DateTime.Now
				} };
			}
			else
			{
				dictionary[switchPos.ToString()] = DateTime.Now;
			}
			tSPlayer.SetData("冷却", dictionary);
			break;
		}
		}
	}
}
