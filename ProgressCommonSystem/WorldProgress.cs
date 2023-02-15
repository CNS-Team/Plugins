using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TShockAPI;

namespace ProgressCommonSystem;

public class WorldProgress
{
	internal readonly HashSet<BossProgress> downedBoss;

	internal readonly HashSet<EventProgress> downedEvent;

	public static WorldProgress CurrentWorldProgress { get; internal set; }

	private WorldProgress()
	{
		downedBoss = new HashSet<BossProgress>();
		downedEvent = new HashSet<EventProgress>();
	}

	public bool DownedBoss(BossProgress boss)
	{
		return downedBoss.Contains(boss);
	}

	public bool DownedEvent(EventProgress @event)
	{
		return downedEvent.Contains(@event);
	}

	internal static WorldProgress GetWorldProgress()
	{
		WorldProgress ret = new WorldProgress();
		LinqExt.ForEach<BossProgress>(ConditionChecker.BossFields.Where(delegate(KeyValuePair<string, BossProgress> k)
		{
			string[] array2 = k.Key.Split('.');
			if (array2[0] == "Main")
			{
				return (bool)typeof(Main).GetField(array2[1], BindingFlags.Static | BindingFlags.Public).GetValue(null);
			}
			if (!(array2[0] == "NPC"))
			{
				throw new NotImplementedException();
			}
			return (bool)typeof(NPC).GetField(array2[1], BindingFlags.Static | BindingFlags.Public).GetValue(null);
		}).Select(delegate(KeyValuePair<string, BossProgress> k)
		{
			KeyValuePair<string, BossProgress> keyValuePair2 = k;
			return keyValuePair2.Value;
		}), (Action<BossProgress>)delegate(BossProgress k)
		{
			ret.downedBoss.Add(k);
		});
		LinqExt.ForEach<EventProgress>(ConditionChecker.EventFields.Where(delegate(KeyValuePair<string, EventProgress> k)
		{
			string[] array = k.Key.Split('.');
			if (array[0] == "Main")
			{
				return (bool)typeof(Main).GetField(array[1], BindingFlags.Static | BindingFlags.Public).GetValue(null);
			}
			if (array[0] == "NPC")
			{
				return (bool)typeof(NPC).GetField(array[1], BindingFlags.Static | BindingFlags.Public).GetValue(null);
			}
			if (!(array[0] == "ExtraData"))
			{
				throw new NotImplementedException();
			}
			return (bool)typeof(ExtraData).GetField(array[1], BindingFlags.Instance | BindingFlags.Public).GetValue(ExtraData.Instance);
		}).Select(delegate(KeyValuePair<string, EventProgress> k)
		{
			KeyValuePair<string, EventProgress> keyValuePair = k;
			return keyValuePair.Value;
		}), (Action<EventProgress>)delegate(EventProgress k)
		{
			ret.downedEvent.Add(k);
		});
		return ret;
	}
}
