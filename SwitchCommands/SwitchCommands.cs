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
		ServerApi.Hooks.NetGetData.Register((TerrariaPlugin)(object)this, (HookHandler<GetDataEventArgs>)GetData);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.NetGetData.Deregister((TerrariaPlugin)(object)this, (HookHandler<GetDataEventArgs>)GetData);
			database.Write(Database.databasePath);
		}
		this.Dispose(disposing);
	}

	private void GetData(GetDataEventArgs args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Expected O, but got Unknown
		using MemoryStream memoryStream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
		TSPlayer val = TShock.Players[args.Msg.whoAmI];
		PacketTypes msgID = args.MsgID;
		PacketTypes val2 = msgID;
		if ((int)val2 != 59)
		{
			return;
		}
		SwitchPos switchPos = new SwitchPos(StreamExt.ReadInt16((Stream)memoryStream), StreamExt.ReadInt16((Stream)memoryStream));
		ITile val3 = Main.tile[switchPos.X, switchPos.Y];
		if (val3.type == 132)
		{
			if (val3.frameX % 36 == 0)
			{
				switchPos.X++;
			}
			if (val3.frameY == 0)
			{
				switchPos.Y++;
			}
		}
		switch (val.GetData<PluginCommands.PlayerState>("PlayerState"))
		{
		case PluginCommands.PlayerState.SelectingSwitch:
			val.SetData<SwitchPos>("SwitchPos", switchPos);
			val.SendSuccessMessage(StringExt.SFormat("成功绑定位于X：{0}、Y：{1} 的开关", new object[2] { switchPos.X, switchPos.Y }));
			val.SendSuccessMessage(StringExt.SFormat("输入/开关 ，可查看子命令列表", new object[2] { switchPos.X, switchPos.Y }));
			val.SetData<PluginCommands.PlayerState>("PlayerState", PluginCommands.PlayerState.AddingCommands);
			if (database.switchCommandList.ContainsKey(switchPos.ToString()))
			{
				val.SetData<CommandInfo>("CommandInfo", database.switchCommandList[switchPos.ToString()]);
			}
			break;
		case PluginCommands.PlayerState.None:
		{
			if (!database.switchCommandList.ContainsKey(switchPos.ToString()))
			{
				break;
			}
			double num = 999999.0;
			Dictionary<string, DateTime> dictionary = val.GetData<Dictionary<string, DateTime>>("冷却");
			if (dictionary != null && dictionary.ContainsKey(switchPos.ToString()))
			{
				num = (DateTime.Now - val.GetData<Dictionary<string, DateTime>>("冷却")[switchPos.ToString()]).TotalMilliseconds / 1000.0;
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
					group = val.Group;
					val.Group = (Group)new SuperAdminGroup();
				}
				try
				{
					string text = global::PlaceholderAPI.PlaceholderAPI.Instance.placeholderManager.GetText(command.ReplaceTags(val), val);
					Commands.HandleCommand(val, text);
					if (ignorePerms)
					{
						val.Group = group;
					}
				}
				catch
				{
					Commands.HandleCommand(val, command.ReplaceTags(val));
					if (ignorePerms)
					{
						val.Group = group;
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
			val.SetData<Dictionary<string, DateTime>>("冷却", dictionary);
			break;
		}
		}
	}
}
