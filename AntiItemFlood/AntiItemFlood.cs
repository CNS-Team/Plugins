using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AntiItemFlood;

[ApiVersion(2, 1)]
public class AntiItemFlood : TerrariaPlugin
{
	public Dictionary<int, int> Multiples = new Dictionary<int, int>();

	public override string Name => "Anti-ItemFlood";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public override string Author => "棱镜";

	public override string Description => "阻止玩家使用物品洪水攻击";

	public AntiItemFlood(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		ServerApi.Hooks.ServerJoin.Register((TerrariaPlugin)(object)this, (HookHandler<JoinEventArgs>)OnServerJoin);
		GetDataHandlers.ItemDrop += OnItemDrop;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.ServerJoin.Deregister((TerrariaPlugin)(object)this, (HookHandler<JoinEventArgs>)OnServerJoin);
			GetDataHandlers.ItemDrop -= OnItemDrop;
		}
		this.Dispose(disposing);
	}

	private void OnServerJoin(JoinEventArgs args)
	{
		Multiples[args.Who] = 0;
	}

	private void OnItemDrop(object? sender, GetDataHandlers.ItemDropEventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		var e2 = e;
		var position = e2.Position;
		if (e2.ID == 400 && TShockAPI.Utils.Distance(position, e2.Player.TPlayer.Center + e2.Player.TPlayer.velocity) >= 48f)
		{
			var val = (from p in TShock.Players
				where p != null
				orderby ((Entity)p.TPlayer).Distance(e2.Position)
				select p).ElementAt(0);
			float value = TShockAPI.Utils.Distance(position, ((Entity)val.TPlayer).Center) / 8f;
			if (val != ((GetDataHandledEventArgs)e2).Player)
			{
				TSPlayer.All.SendMessage($"{((GetDataHandledEventArgs)e2).Player.Name}在{val.Name}附近({value}格)生成了物品:[i:{e2.Type}]，两人实际距离为{((Entity)((GetDataHandledEventArgs)e2).Player.TPlayer).Distance(((Entity)val.TPlayer).Center) / 8f}格", Color.LightGoldenrodYellow);
			}
            this.AddCount(e2.Player.Index);
			TShock.Log.Info($"警告:玩家{((GetDataHandledEventArgs)e2).Player.Name}疑似使用物品洪水攻击,物品为{e2.Type},坐标为{e2.Position},受害者可能是{val.Name}(生成点在距离{value}格处)");
			((HandledEventArgs)(object)e2).Handled = true;
		}
	}

	private void AddCount(int index)
	{
		if (Config.GetConfig().Multiplekickout)
		{
			Multiples[index]++;
			if (Multiples[index] >= Config.GetConfig().MultipleCount)
			{
				TShock.Players[index].Kick(TShock.Players[index].Name + "疑似使用物品洪水攻击,已踢出", true, false, (string)null, false);
			}
		}
	}
}
