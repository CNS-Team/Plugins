using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using TShockAPI;

namespace ProgressKitsV2;

internal class KitPlayer
{
	[JsonProperty("玩家序号")]
	public int id;

	[JsonProperty("玩家名称")]
	public string name;

	[JsonProperty("可领取礼包")]
	public List<int> availableKits;

	[JsonProperty("已领取礼包")]
	public List<int> aquiredKits;

	[JsonProperty("云仓")]
	public List<NetItem> cloudItems;

	[JsonIgnore]
	public TSPlayer player;

	public KitPlayer(int id, string name)
	{
		this.id = id;
		this.name = name;
		availableKits = new List<int>();
		aquiredKits = new List<int>();
		cloudItems = new List<NetItem>();
		player = null;
	}

	public bool CanAquire(int kitID)
	{
		return availableKits.Contains(kitID) && !aquiredKits.Contains(kitID);
	}

	public bool CanAquire(string name)
	{
		PKit kitByName = ConfigUtils.GetKitByName(name);
		return kitByName != null && CanAquire(kitByName.id);
	}

	public int GetEmptySlotCount()
	{
		if (player == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 49; i++)
		{
			if (player.TPlayer.inventory[i].netID == 0)
			{
				num++;
			}
		}
		return num;
	}

	public void GetKit(int id)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (!CanAquire(id))
		{
			player.SendInfoMessage("你未拥有该礼包");
			return;
		}
		PKit kitByID = ConfigUtils.GetKitByID(id);
		if (kitByID == null)
		{
			player.SendInfoMessage("礼包不存在");
			return;
		}
		for (int i = 0; i < kitByID.items.Count; i++)
		{
			NetItem val = kitByID.items[i];
			NetItem val2;
			if (GetEmptySlotCount() > 0)
			{
				int emptySlotIndex = GetEmptySlotIndex();
				var utils = TShock.Utils;
				val2 = kitByID.items[i];
				Item itemById = utils.GetItemById(val2.NetId);
				val2 = kitByID.items[i];
				itemById.stack = val2.Stack;
				val2 = kitByID.items[i];
				itemById.prefix = val2.PrefixId;
				player.TPlayer.inventory[emptySlotIndex] = itemById;
				player.SendData((PacketTypes)5, itemById.Name, player.Index, (float)emptySlotIndex, (float)(int)itemById.prefix, 0f, 0);
			}
			else
			{
				PutItemInCloud(kitByID.items[i]);
				TSPlayer obj = player;
				val2 = kitByID.items[i];
				obj.SendMessage($"因背包空间不足,已将[i:{val2.NetId}]放入云仓(输入/pkit store 查看云仓)", Color.MediumAquamarine);
			}
		}
		availableKits.Remove(id);
		aquiredKits.Add(id);
		player.SendSuccessMessage("已成功领取 " + kitByID.name + " ");
		ConfigUtils.UpdatePlayer();
	}

	public void GetAll()
	{
		for (int i = 0; i < availableKits.Count; i++)
		{
			GetKit(availableKits[i]);
		}
	}

	public bool GetItemFromCloud(int index, int count)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (index >= cloudItems.Count)
		{
			player.SendMessage("你输入的标号超出范围", Color.MediumAquamarine);
			return result;
		}
		if (GetEmptySlotCount() == 0)
		{
			player.SendInfoMessage("你的空间不足,无法从云仓取出物品");
			return result;
		}
		NetItem val = cloudItems[index];
		if (val.Stack < count || count > 999)
		{
			player.SendInfoMessage("你输入的数量超出实际物品数量(范围0~999)");
			return result;
		}
		int emptySlotIndex = GetEmptySlotIndex();
		var utils = TShock.Utils;
		val = cloudItems[index];
		Item itemById = utils.GetItemById(val.NetId);
		itemById.stack = count;
		val = cloudItems[index];
		itemById.prefix = val.PrefixId;
		player.TPlayer.inventory[emptySlotIndex] = itemById;
		player.SendData((PacketTypes)5, itemById.Name, player.Index, (float)emptySlotIndex, (float)(int)itemById.prefix, 0f, 0);
		result = true;
		if (result)
		{
			List<NetItem> list = cloudItems;
			val = cloudItems[index];
			int netId = val.NetId;
			val = cloudItems[index];
			int num = val.Stack - count;
			val = cloudItems[index];
			list[index] = new NetItem(netId, num, val.PrefixId);
		}
		return result;
	}

	public bool PutItemInCloud(NetItem item)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (cloudItems.Exists((NetItem i) => i.NetId == item.NetId) && cloudItems.Exists((NetItem i) => i.PrefixId == item.PrefixId))
		{
			int num = cloudItems.FindIndex((NetItem i) => i.NetId == item.NetId && i.PrefixId == item.PrefixId);
			return true;
		}
		cloudItems.Add(item);
		return true;
	}

	public void ShowCloudInfo()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("云仓物品如下-->");
		for (int i = 0; i < cloudItems.Count; i++)
		{
			NetItem val = cloudItems[i];
			if (val.Stack != 0)
			{
				stringBuilder.Append($"{i}.[i/s{val.Stack}:{val.NetId}](实际数量:{val.Stack})");
			}
			else if (val.PrefixId != 0)
			{
				stringBuilder.Append($"{i}.[i/p{val.PrefixId}:{val.NetId}](实际数量:{val.Stack})");
			}
			else
			{
				stringBuilder.Append($"{i}.[i:{val.NetId}](实际数量:{val.Stack})");
			}
			if (i % 10 == 0)
			{
				stringBuilder.Append("\n");
			}
		}
		stringBuilder.AppendLine("输入/pkit getitem [物品序号] [数量] 取出物品");
		player.SendMessage(stringBuilder.ToString(), Color.MediumAquamarine);
	}

	public int GetEmptySlotIndex()
	{
		if (player == null)
		{
			return 0;
		}
		int result = 0;
		for (int i = 0; i < 49; i++)
		{
			if (player.TPlayer.inventory[i].netID == 0)
			{
				result = i;
				break;
			}
		}
		return result;
	}
}
