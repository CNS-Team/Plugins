using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.Localization;
using TShockAPI;

namespace HousingPlugin;

public static class GetDataHandlers
{
	private static string EditHouse = "house.edit";

	private static Dictionary<PacketTypes, GetDataHandlerDelegate> GetDataHandlerDelegates;

	public static void InitGetDataHandler()
	{
		Dictionary<PacketTypes, GetDataHandlerDelegate> dictionary = new Dictionary<PacketTypes, GetDataHandlerDelegate>();
		dictionary.Add((PacketTypes)17, HandleTile);
		dictionary.Add((PacketTypes)19, HandleDoorUse);
		dictionary.Add((PacketTypes)20, HandleSendTileSquare);
		dictionary.Add((PacketTypes)31, HandleChestOpen);
		dictionary.Add((PacketTypes)32, HandleChestItem);
		dictionary.Add((PacketTypes)33, HandleChestActive);
		dictionary.Add((PacketTypes)34, HandlePlaceChest);
		dictionary.Add((PacketTypes)47, HandleSign);
		dictionary.Add((PacketTypes)48, HandleLiquidSet);
		dictionary.Add((PacketTypes)63, HandlePaintTile);
		dictionary.Add((PacketTypes)64, HandlePaintWall);
		dictionary.Add((PacketTypes)79, HandlePlaceObject);
		dictionary.Add((PacketTypes)87, HandlePlaceTileEntity);
		dictionary.Add((PacketTypes)89, HandlePlaceItemFrame);
		dictionary.Add((PacketTypes)105, HandleGemLockToggle);
		dictionary.Add((PacketTypes)109, HandleMassWireOperation);
		dictionary.Add((PacketTypes)122, HandleRequestTileEntityInteraction);
		dictionary.Add((PacketTypes)123, HandleWeaponsRackTryPlacing);
		GetDataHandlerDelegates = dictionary;
	}

	public static bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (GetDataHandlerDelegates.TryGetValue(type, out var value))
		{
			try
			{
				return value(new GetDataHandlerArgs(player, data));
			}
			catch (Exception ex)
			{
				TShock.Log.Error("房屋插件错误调用事件时出错:" + ex.ToString());
			}
		}
		return false;
	}

	private static bool HandleTile(GetDataHandlerArgs args)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		int num = StreamExt.ReadInt8((Stream)args.Data);
		int num2 = StreamExt.ReadInt16((Stream)args.Data);
		int num3 = StreamExt.ReadInt16((Stream)args.Data);
		ITile val = Main.tile[num2, num3];
		if (Main.tileCut[val.type])
		{
			return false;
		}
		House house = HTools.InAreaHouse(num2, num3);
		if (HousingPlugin.LPlayers[args.Player.Index].Look)
		{
			if (house == null)
			{
				args.Player.SendMessage("敲击处不属于任何房子。", Color.Yellow);
			}
			else
			{
				string text = "";
				try
				{
					text = TShock.UserAccounts.GetUserAccountByID(Convert.ToInt32(house.Author)).Name;
				}
				catch (Exception ex)
				{
					TShock.Log.Error("房屋插件错误超标错误:" + ex.ToString());
				}
				args.Player.SendMessage("敲击处为 " + text + " 的房子: " + house.Name + " 状态: " + ((!house.Locked || HousingPlugin.LConfig.禁止锁房屋) ? "未上锁" : "已上锁"), Color.Yellow);
			}
			args.Player.SendTileSquare(num2, num3, 10);
			HousingPlugin.LPlayers[args.Player.Index].Look = false;
			return true;
		}
		if (args.Player.AwaitingTempPoint > 0)
		{
			args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].X = num2;
			args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].Y = num3;
			if (args.Player.AwaitingTempPoint == 1)
			{
				args.Player.SendMessage("保护区左上角已设置!", Color.Yellow);
			}
			if (args.Player.AwaitingTempPoint == 2)
			{
				args.Player.SendMessage("保护区右下角已设置!", Color.Yellow);
			}
			args.Player.SendTileSquare(num2, num3, 10);
			args.Player.AwaitingTempPoint = 0;
			return true;
		}
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改房子保护!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力损坏被房子保护的地区。");
		args.Player.SendTileSquare(num2, num3, 10);
		return true;
	}

	private static bool HandleDoorUse(GetDataHandlerArgs args)
	{
		StreamExt.ReadInt8((Stream)args.Data);
		int num = StreamExt.ReadInt16((Stream)args.Data);
		int num2 = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (!house.Locked || HousingPlugin.LConfig.禁止锁房屋 || HousingPlugin.LConfig.停用锁门)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改门!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力修改被房子保护的地区的门。");
		args.Player.SendTileSquare(num, num2, 10);
		return true;
	}

	private static bool HandleSendTileSquare(GetDataHandlerArgs args)
	{
		return false;
	}

	private static bool HandleChestOpen(GetDataHandlerArgs args)
	{
		int x = StreamExt.ReadInt16((Stream)args.Data);
		int y = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(x, y);
		if (house == null)
		{
			return false;
		}
		if ((!house.Locked || HousingPlugin.LConfig.禁止锁房屋) && !HousingPlugin.LConfig.始终保护箱子)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权打开箱子!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力打开被房子保护的地区的箱子。");
		return true;
	}

	private static bool HandleChestItem(GetDataHandlerArgs args)
	{
		short num = StreamExt.ReadInt16((Stream)args.Data);
		int x = Main.chest[num].x;
		int y = Main.chest[num].y;
		House house = HTools.InAreaHouse(x, y);
		if (house == null)
		{
			return false;
		}
		if ((!house.Locked || HousingPlugin.LConfig.禁止锁房屋) && !HousingPlugin.LConfig.始终保护箱子)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权更新箱子!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力更新被房子保护的地区的箱子。");
		return true;
	}

	private static bool HandleChestActive(GetDataHandlerArgs args)
	{
		StreamExt.ReadInt16((Stream)args.Data);
		int x = StreamExt.ReadInt16((Stream)args.Data);
		int y = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(x, y);
		if (house == null)
		{
			return false;
		}
		if ((!house.Locked || HousingPlugin.LConfig.禁止锁房屋) && !HousingPlugin.LConfig.始终保护箱子)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改箱子!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力修改被房子保护的地区的箱子。");
		args.Player.SendData((PacketTypes)33, "", -1, 0f, 0f, 0f, 0);
		return true;
	}

	private static bool HandlePlaceChest(GetDataHandlerArgs args)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		args.Data.ReadByte();
		int num = StreamExt.ReadInt16((Stream)args.Data);
		int num2 = StreamExt.ReadInt16((Stream)args.Data);
		Rectangle val = default(Rectangle);
		val.Modified(num, num2, 3, 3);
		for (int i = 0; i < HousingPlugin.Houses.Count; i++)
		{
			House house = HousingPlugin.Houses[i];
			if (house == null)
			{
				continue;
			}
			Rectangle houseArea = house.HouseArea;
			if (houseArea.Intersects(val) && !args.Player.Group.HasPermission(EditHouse) && !(args.Player.Account.ID.ToString() == house.Author) && !HTools.OwnsHouse(args.Player.Account.ID.ToString(), house))
			{
				if (HousingPlugin.LConfig.冻结警告破坏者)
				{
					args.Player.Disable("无权放置家具!", (DisableFlags)2);
				}
				args.Player.SendErrorMessage("你没有权力放置被房子保护的地区的家具。");
				args.Player.SendTileSquare(num, num2, 3);
				return true;
			}
		}
		return false;
	}

	private static bool HandleSignRead(GetDataHandlerArgs args)
	{
		return false;
	}

	private static bool HandleSign(GetDataHandlerArgs args)
	{
		short num = StreamExt.ReadInt16((Stream)args.Data);
		short x = StreamExt.ReadInt16((Stream)args.Data);
		short y = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(x, y);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改标牌!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力修改被房子保护的地区的标牌。");
		args.Player.SendData((PacketTypes)47, "", (int)num, 0f, 0f, 0f, 0);
		return true;
	}

	private static bool HandleLiquidSet(GetDataHandlerArgs args)
	{
		int num = StreamExt.ReadInt16((Stream)args.Data);
		int num2 = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权放水!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力在被房子保护的地区放水。");
		args.Player.SendTileSquare(num, num2, 10);
		return true;
	}

	private static bool HandleHitSwitch(GetDataHandlerArgs args)
	{
		return false;
	}

	private static bool HandlePaintTile(GetDataHandlerArgs args)
	{
		short num = StreamExt.ReadInt16((Stream)args.Data);
		short num2 = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权油漆砖!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力在被房子保护的地区油漆砖。");
		args.Player.SendData((PacketTypes)63, "", (int)num, (float)num2, (float)(int)Main.tile[(int)num, (int)num2].color(), 0f, 0);
		return true;
	}

	private static bool HandlePaintWall(GetDataHandlerArgs args)
	{
		short num = StreamExt.ReadInt16((Stream)args.Data);
		short num2 = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权油漆墙!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力在被房子保护的地区油漆墙。");
		args.Player.SendData((PacketTypes)64, "", (int)num, (float)num2, (float)(int)Main.tile[(int)num, (int)num2].wallColor(), 0f, 0);
		return true;
	}

	private static bool HandleTeleport(GetDataHandlerArgs args)
	{
		return false;
	}

	private static bool HandlePlaceObject(GetDataHandlerArgs args)
	{
		int num = StreamExt.ReadInt16((Stream)args.Data);
		int num2 = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改房子保护!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力修改被房子保护的地区。");
		args.Player.SendTileSquare(num, num2, 10);
		return true;
	}

	private static bool HandlePlaceTileEntity(GetDataHandlerArgs args)
	{
		short num = StreamExt.ReadInt16((Stream)args.Data);
		short num2 = StreamExt.ReadInt16((Stream)args.Data);
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改房子保护!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力修改被房子保护的地区。");
		args.Player.SendTileSquare((int)num, (int)num2, 10);
		return true;
	}

	private static bool HandlePlaceItemFrame(GetDataHandlerArgs args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		short num = StreamExt.ReadInt16((Stream)args.Data);
		short num2 = StreamExt.ReadInt16((Stream)args.Data);
		TEItemFrame val = (TEItemFrame)TileEntity.ByID[TEItemFrame.Find((int)num, (int)num2)];
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改房子保护的物品框!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力修改被房子保护的物品框。");
		NetMessage.SendData(86, -1, -1, NetworkText.Empty, ((TileEntity)val).ID, 0f, 1f, 0f, 0, 0, 0);
		return true;
	}

	private static bool HandleGemLockToggle(GetDataHandlerArgs args)
	{
		int x = StreamExt.ReadInt16((Stream)args.Data);
		int y = StreamExt.ReadInt16((Stream)args.Data);
		if (!HousingPlugin.LConfig.保护宝石锁)
		{
			return false;
		}
		House house = HTools.InAreaHouse(x, y);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权触发房子保护的宝石锁!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力触发被房子保护的宝石锁。");
		return true;
	}

	private static bool HandleMassWireOperation(GetDataHandlerArgs args)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		int num = StreamExt.ReadInt16((Stream)args.Data);
		int num2 = StreamExt.ReadInt16((Stream)args.Data);
		int num3 = StreamExt.ReadInt16((Stream)args.Data);
		int num4 = StreamExt.ReadInt16((Stream)args.Data);
		Rectangle val = default(Rectangle);
		val.Modified(Math.Min(num, num3), (((Entity)args.TPlayer).direction != 1) ? num2 : num4, Math.Abs(num3 - num) + 1, 1);
		Rectangle val2 = default(Rectangle);
		val.Modified((((Entity)args.TPlayer).direction != 1) ? num3 : num, Math.Min(num2, num4), 1, Math.Abs(num4 - num2) + 1);
		for (int i = 0; i < HousingPlugin.Houses.Count; i++)
		{
			House house = HousingPlugin.Houses[i];
			if (house == null)
			{
				continue;
			}
			Rectangle houseArea = house.HouseArea;
			if (!houseArea.Intersects(val))
			{
				houseArea = house.HouseArea;
				if (!houseArea.Intersects(val2))
				{
					continue;
				}
			}
			if (!args.Player.Group.HasPermission(EditHouse) && !(args.Player.Account.ID.ToString() == house.Author) && !HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) && !HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
			{
				return true;
			}
		}
		return false;
	}

	private static bool HandleRequestTileEntityInteraction(GetDataHandlerArgs args)
	{
		int key = StreamExt.ReadInt32((Stream)args.Data);
		byte b = StreamExt.ReadInt8((Stream)args.Data);
		if (!TileEntity.ByID.TryGetValue(key, out var value))
		{
			return false;
		}
		if (value == null)
		{
			return false;
		}
		House house = HTools.InAreaHouse(value.Position.X, value.Position.Y);
		if (house == null)
		{
			return false;
		}
		if ((!house.Locked || HousingPlugin.LConfig.禁止锁房屋) && !HousingPlugin.LConfig.始终保护箱子)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权打开此模木架!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力打开被房子保护的地区的模木架。");
		return true;
	}

	private static bool HandleWeaponsRackTryPlacing(GetDataHandlerArgs args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		short num = StreamExt.ReadInt16((Stream)args.Data);
		short num2 = StreamExt.ReadInt16((Stream)args.Data);
		TEWeaponsRack val = (TEWeaponsRack)TileEntity.ByID[TEWeaponsRack.Find((int)num, (int)num2)];
		House house = HTools.InAreaHouse(num, num2);
		if (house == null)
		{
			return false;
		}
		if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || HTools.OwnsHouse(args.Player.Account.ID.ToString(), house) || HTools.CanUseHouse(args.Player.Account.ID.ToString(), house))
		{
			return false;
		}
		if (HousingPlugin.LConfig.冻结警告破坏者)
		{
			args.Player.Disable("无权修改房子保护的武器板!", (DisableFlags)2);
		}
		args.Player.SendErrorMessage("你没有权力修改被房子保护的武器板。");
		NetMessage.SendData(86, -1, -1, NetworkText.Empty, ((TileEntity)val).ID, 0f, 1f, 0f, 0, 0, 0);
		return true;
	}
}
