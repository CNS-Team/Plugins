using System.Collections.Generic;
using Newtonsoft.Json;
using TShockAPI;

namespace ProgressKitsV2;

public class PKit
{
	[JsonProperty("礼包序号")]
	public int id;

	[JsonProperty("礼包名称")]
	public string name;

	[JsonProperty("触发怪物")]
	public List<int> mobs;

	[JsonProperty("给予物品")]
	public List<NetItem> items;

	public PKit()
	{
	}

	public PKit(int id, string name)
	{
		this.id = id;
		this.name = name;
		mobs = new List<int>();
		items = new List<NetItem>();
	}

	public PKit GetCopy()
	{
		return new PKit
		{
			id = id,
			name = name,
			mobs = mobs,
			items = items
		};
	}

	public static PKit MergeKits(params int[] kitIDs)
	{
		PKit pKit = new PKit();
		for (int i = 0; i < kitIDs.Length; i++)
		{
			PKit kitByID = ConfigUtils.GetKitByID(kitIDs[i]);
			pKit.items.AddRange(kitByID.items);
			PKit pKit2 = pKit;
			pKit2.name = pKit2.name + "|" + kitByID.name;
			if (i == kitIDs.Length)
			{
				PKit pKit3 = pKit;
				pKit3.name += "的合并礼包";
			}
		}
		return pKit;
	}
}
