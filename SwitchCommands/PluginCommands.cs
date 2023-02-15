using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace SwitchCommands;

public class PluginCommands
{
	public enum PlayerState
	{
		None,
		AddingCommands,
		SelectingSwitch
	}

	public static string switchParameters = "/开关 <添加/列表/删除/冷却/权限忽略/取消/重绑/完成>";

	public static void RegisterCommands()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		Commands.ChatCommands.Add(new Command("开关", new CommandDelegate(SwitchCmd), new string[3] { "开关", "kg", "switch" }));
		Commands.ChatCommands.Add(new Command("开关", new CommandDelegate(SwitchReload), new string[2] { "重载开关", "reload" }));
	}

	private static void SwitchReload(CommandArgs args)
	{
		SwitchCommands.database = Database.Read(Database.databasePath);
		args.Player.SendErrorMessage("开关插件重载成功！！！");
		if (!File.Exists(Database.databasePath))
		{
			SwitchCommands.database.Write(Database.databasePath);
		}
	}

	private static void SwitchCmd(CommandArgs args)
	{
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		TSPlayer player = args.Player;
		switch (player.GetData<PlayerState>("PlayerState"))
		{
		case PlayerState.None:
			player.SendSuccessMessage("激活一个开关以将其绑定,之后可输入/开关 ，查看子命令");
			player.SetData<PlayerState>("PlayerState", PlayerState.SelectingSwitch);
			break;
		case PlayerState.AddingCommands:
		{
			if (args.Parameters.Count == 0)
			{
				player.SendErrorMessage("正确指令：");
				player.SendErrorMessage(switchParameters);
				break;
			}
			if (player.GetData<CommandInfo>("CommandInfo") == null)
			{
				player.SetData<CommandInfo>("CommandInfo", new CommandInfo());
			}
			CommandInfo data = player.GetData<CommandInfo>("CommandInfo");
			switch (args.Parameters[0].ToLower())
			{
			case "add":
			case "添加":
			case "tj":
			{
				string text = "/" + string.Join(" ", args.Parameters.Skip(1));
				data.commandList.Add(text);
				player.SendSuccessMessage(StringExt.SFormat("成功添加: {0}", new object[1] { text }));
				SwitchCommands.database.Write(Database.databasePath);
				break;
			}
			case "list":
			case "列表":
			case "lb":
			{
				player.SendMessage("当前开关绑定的指令:", Color.Green);
				for (int i = 0; i < data.commandList.Count; i++)
				{
					player.SendMessage(StringExt.SFormat("({0}) ", new object[1] { i }) + data.commandList[i], Color.Yellow);
					SwitchCommands.database.Write(Database.databasePath);
				}
				break;
			}
			case "del":
			case "删除":
			case "sc":
			{
				int result3 = 0;
				if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out result3))
				{
					player.SendErrorMessage("语法错误：/开关 del <指令>");
					SwitchCommands.database.Write(Database.databasePath);
					return;
				}
				string text2 = data.commandList[result3];
				data.commandList.RemoveAt(result3);
				player.SendSuccessMessage(StringExt.SFormat("成功删除了第{1}条指令：{0}。", new object[2] { text2, result3 }));
				SwitchCommands.database.Write(Database.databasePath);
				break;
			}
			case "冷却":
			case "cooldown":
			case "lq":
			{
				float result = 0f;
				if (args.Parameters.Count < 2 || !float.TryParse(args.Parameters[1], out result))
				{
					player.SendErrorMessage("语法错误：/开关 冷却 <秒>");
					SwitchCommands.database.Write(Database.databasePath);
					return;
				}
				data.cooldown = result;
				player.SendSuccessMessage(StringExt.SFormat("冷却时间已设置为 {0} 秒", new object[1] { result }));
				SwitchCommands.database.Write(Database.databasePath);
				break;
			}
			case "权限忽略":
			case "ignoreperms":
			case "qxhl":
			{
				bool result2 = false;
				if (args.Parameters.Count < 2 || !bool.TryParse(args.Parameters[1], out result2))
				{
					player.SendErrorMessage("语法错误：/开关 权限忽略 <true/false>");
					SwitchCommands.database.Write(Database.databasePath);
					return;
				}
				data.ignorePerms = result2;
				player.SendSuccessMessage(StringExt.SFormat("是否忽略玩家权限设置为: {0}.", new object[1] { result2 }));
				SwitchCommands.database.Write(Database.databasePath);
				break;
			}
			case "取消":
			case "cancel":
			case "qx":
				player.SetData<PlayerState>("PlayerState", PlayerState.None);
				player.SetData<CommandInfo>("CommandInfo", new CommandInfo());
				player.SendSuccessMessage("已取消添加要添加的命令");
				SwitchCommands.database.Write(Database.databasePath);
				return;
			case "重绑":
			case "rebind":
			case "zb":
				player.SendSuccessMessage("重新激活开关后可以重新绑定");
				player.SetData<PlayerState>("PlayerState", PlayerState.SelectingSwitch);
				SwitchCommands.database.Write(Database.databasePath);
				return;
			case "完成":
			case "done":
			case "wc":
			{
				SwitchPos data2 = player.GetData<SwitchPos>("SwitchPos");
				player.SendSuccessMessage(StringExt.SFormat("设置成功的开关位于 X： {0}， Y： {1}", new object[2] { data2.X, data2.Y }));
				foreach (string command in data.commandList)
				{
					player.SendMessage(command, Color.Yellow);
					SwitchCommands.database.Write(Database.databasePath);
				}
				SwitchCommands.database.switchCommandList[player.GetData<SwitchPos>("SwitchPos").ToString()] = data;
				player.SetData<PlayerState>("PlayerState", PlayerState.None);
				player.SetData<Vector2>("SwitchPos", default(Vector2));
				player.SetData<CommandInfo>("CommandInfo", new CommandInfo());
				SwitchCommands.database.Write(Database.databasePath);
				return;
			}
			default:
				player.SendErrorMessage("语法无效. " + switchParameters);
				return;
			}
			player.SetData<CommandInfo>("CommandInfo", data);
			break;
		}
		}
	}
}
