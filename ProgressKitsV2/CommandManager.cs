using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace ProgressKitsV2;

public class CommandManager
{
	public CommandManager()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		Commands.ChatCommands.Add(new Command("pkit.user", new CommandDelegate(PUser), new string[1] { "pkit" }));
		Commands.ChatCommands.Add(new Command("pkit.admin", new CommandDelegate(PAdmin), new string[1] { "pkadmin" }));
	}

	private void PAdmin(CommandArgs args)
	{
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		if (args.Parameters.Count < 1)
		{
			args.Player.SendInfoMessage("输入/pkadmin help 查看帮助");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		PKit pKit = null;
		switch (args.Parameters[0])
		{
		case "create":
			if (args.Parameters.Count != 2)
			{
				args.Player.SendInfoMessage("请输入正确的指令");
				break;
			}
			pKit = new PKit(ConfigUtils.loadedKits.Count + 1, args.Parameters[1]);
			args.Player.SendInfoMessage("已创建空白礼包模板,请去配置文件修改");
			ConfigUtils.loadedKits.Add(pKit);
			ConfigUtils.UpdateKits();
			break;
		case "list":
		{
			stringBuilder.AppendLine("当前礼包列表-->");
			for (int i = 0; i < ConfigUtils.loadedKits.Count; i++)
			{
				pKit = ConfigUtils.GetKitByID(ConfigUtils.loadedKits[i].id);
				if (pKit != null)
				{
					stringBuilder.AppendLine($"[{pKit.id}][{pKit.name}]");
				}
			}
			args.Player.SendMessage(stringBuilder.ToString(), Color.MediumAquamarine);
			break;
		}
		case "additem":
		{
			if (args.Parameters.Count != 2)
			{
				args.Player.SendInfoMessage("请输入正确的指令");
				break;
			}
			pKit = ((!int.TryParse(args.Parameters[1], out var result3)) ? ConfigUtils.GetKitByName(args.Parameters[1]) : ConfigUtils.GetKitByID(result3));
			if (pKit == null)
			{
				args.Player.SendInfoMessage("礼包不存在");
				break;
			}
			Item selectedItem = args.Player.SelectedItem;
			pKit.items.Add(new NetItem(selectedItem.netID, selectedItem.stack, selectedItem.prefix));
			ConfigUtils.UpdateKits();
			args.Player.SendInfoMessage("成功添加手上的物品");
			break;
		}
		case "help":
			stringBuilder.AppendLine("/pkadmin create [名字] 创建礼包空白模板");
			stringBuilder.AppendLine("/pkadmin remove [礼包序号/名字] 删除此礼包");
			stringBuilder.AppendLine("/pkadmin give [礼包序号/名字] [玩家名] 将指定礼包给予玩家");
			stringBuilder.AppendLine("/pkadmin additem [礼包序号/名字] 将手上的物品添加进背包");
			stringBuilder.AppendLine("/pkadmin restore [玩家名字] 重置某玩家进度礼包");
			stringBuilder.AppendLine("/pkadmin list 列出所有进度礼包");
			stringBuilder.AppendLine("/pkadmin reload 重载配置文件");
			args.Player.SendMessage(stringBuilder.ToString(), Color.MediumAquamarine);
			break;
		case "remove":
		{
			if (args.Parameters.Count != 2)
			{
				break;
			}
			pKit = ((!int.TryParse(args.Parameters[1], out var result2)) ? ConfigUtils.GetKitByName(args.Parameters[1]) : ConfigUtils.GetKitByID(result2));
			if (pKit == null)
			{
				args.Player.SendInfoMessage("礼包不存在");
				break;
			}
			ConfigUtils.loadedKits.Remove(pKit);
			ConfigUtils.firstKits.Remove(pKit.id);
			foreach (KitPlayer player in ConfigUtils.players)
			{
				player.aquiredKits.Remove(pKit.id);
				player.availableKits.Remove(pKit.id);
			}
			ConfigUtils.UpdateKits();
			ConfigUtils.UpdateFirst();
			ConfigUtils.UpdatePlayer();
			args.Player.SendInfoMessage($"成功删除礼包 [{pKit.id}][{pKit.name}]");
			break;
		}
		case "give":
		{
			if (args.Parameters.Count != 3)
			{
				break;
			}
			pKit = ((!int.TryParse(args.Parameters[1], out var result)) ? ConfigUtils.GetKitByName(args.Parameters[1]) : ConfigUtils.GetKitByID(result));
			if (pKit == null)
			{
				args.Player.SendInfoMessage("礼包不存在");
				break;
			}
			KitPlayer playerByName2 = ConfigUtils.GetPlayerByName(args.Parameters[2]);
			if (playerByName2 == null)
			{
				args.Player.SendInfoMessage("玩家不存在");
			}
			else if (!playerByName2.availableKits.Contains(pKit.id))
			{
				playerByName2.availableKits.Add(pKit.id);
				if (playerByName2.player != null)
				{
					playerByName2.player.SendInfoMessage($"管理员给予你一个礼包 [{pKit.id}][{pKit.name}]");
				}
				ConfigUtils.UpdatePlayer();
			}
			else
			{
				args.Player.SendInfoMessage("玩家已拥有该礼包");
			}
			break;
		}
		case "reload":
			ConfigUtils.LoadConfig();
			foreach (KitPlayer player2 in ConfigUtils.players)
			{
				if (TSPlayer.FindByNameOrID(player2.name).Count != 0)
				{
					player2.player = TSPlayer.FindByNameOrID(player2.name)[0];
				}
			}
			args.Player.SendInfoMessage("[ProgressKit] 配置文件重载完成");
			break;
		case "restore":
		{
			if (args.Parameters.Count != 2)
			{
				ConfigUtils.firstKits.Clear();
				ConfigUtils.players.Clear();
				ConfigUtils.UpdateFirst();
				ConfigUtils.UpdatePlayer();
				args.Player.SendInfoMessage("除了礼包数据外的其他数据都已被重置");
				break;
			}
			if (args.Parameters[1] == "all")
			{
				foreach (KitPlayer player3 in ConfigUtils.players)
				{
					player3.availableKits.Clear();
					player3.aquiredKits.Clear();
					player3.availableKits.AddRange(ConfigUtils.firstKits);
					if (player3.player != null)
					{
						player3.player.SendInfoMessage("你的礼包数据已被重置");
					}
				}
				break;
			}
			KitPlayer playerByName = ConfigUtils.GetPlayerByName(args.Parameters[1]);
			if (playerByName == null)
			{
				args.Player.SendInfoMessage("玩家不存在");
				break;
			}
			playerByName.availableKits.Clear();
			playerByName.aquiredKits.Clear();
			playerByName.availableKits.AddRange(ConfigUtils.firstKits);
			if (playerByName.player != null)
			{
				playerByName.player.SendInfoMessage("你的礼包数据已被重置");
			}
			break;
		}
		default:
			args.Player.SendInfoMessage("输入/pkadmin help 查看帮助");
			break;
		}
	}

	private void PUser(CommandArgs args)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Parameters.Count < 1)
		{
			args.Player.SendInfoMessage("输入/pkit help 查看帮助");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int result = 0;
		int result2 = 0;
		KitPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
		if (playerByName == null)
		{
			args.Player.SendErrorMessage("玩家数据出错请联系管理员");
			return;
		}
		switch (args.Parameters[0])
		{
		case "help":
			stringBuilder.AppendLine("/pkit get [礼包ID/名字] 领取礼包");
			stringBuilder.AppendLine("/pkit getitem [物品序号] [数量] 从云仓拿物品");
			stringBuilder.AppendLine("/pkit store 查看云仓");
			stringBuilder.AppendLine("/pkit list 列出可以领取的礼包");
			args.Player.SendMessage(stringBuilder.ToString(), Color.MediumAquamarine);
			break;
		case "list":
		{
			stringBuilder.AppendLine("可领取礼包-->");
			for (int i = 0; i < playerByName.availableKits.Count; i++)
			{
				PKit kitByID = ConfigUtils.GetKitByID(playerByName.availableKits[i]);
				if (kitByID != null)
				{
					stringBuilder.AppendLine($"[{kitByID.id}][{kitByID.name}]");
				}
			}
			args.Player.SendMessage(stringBuilder.ToString(), Color.MediumAquamarine);
			break;
		}
		default:
			args.Player.SendInfoMessage("输入/pkit help 查看更多指令");
			break;
		case "store":
			playerByName.ShowCloudInfo();
			break;
		case "getitem":
			if (args.Parameters.Count != 3)
			{
				args.Player.SendInfoMessage("请输入正确的指令");
			}
			else if (int.TryParse(args.Parameters[1], out result) && int.TryParse(args.Parameters[2], out result2))
			{
				if (playerByName.GetItemFromCloud(result, result2))
				{
					args.Player.SendInfoMessage("成功取出物品");
				}
				ConfigUtils.UpdatePlayer();
			}
			else
			{
				args.Player.SendInfoMessage("请输入正确的数字");
			}
			break;
		case "get":
		{
			if (args.Parameters.Count != 2)
			{
				playerByName.GetAll();
				args.Player.SendInfoMessage("已领取全部礼包，部分物品可能放入云仓");
				break;
			}
			PKit pKit = ((!int.TryParse(args.Parameters[1], out result)) ? ConfigUtils.GetKitByName(args.Parameters[1]) : ConfigUtils.GetKitByID(result));
			if (pKit != null && playerByName.CanAquire(pKit.id))
			{
				playerByName.GetKit(pKit.id);
			}
			else
			{
				args.Player.SendInfoMessage("礼包不存在");
			}
			break;
		}
		}
	}
}
