using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace NoSummonArea;

[ApiVersion(2, 1)]
public class NoSummonArea : TerrariaPlugin
{
	private List<bool> sent = new List<bool>();

	public Dictionary<int, int> Multiples = new Dictionary<int, int>();

	public override string Author => "Leader";

	public override string Description => "区域禁止召唤";

	public override string Name => "NoSummerArea";

	public override Version Version => new Version(1, 0, 0, 0);

	public NoSummonArea(Main game)
		: base(game)
	{
		Config.GetConfig();
	}

	public override void Initialize()
	{
		ServerApi.Hooks.ServerJoin.Register((TerrariaPlugin)(object)this, (HookHandler<JoinEventArgs>)OnServerJoin);
		ServerApi.Hooks.NetGetData.Register((TerrariaPlugin)(object)this, (HookHandler<GetDataEventArgs>)OnNetGetdata);
		ServerApi.Hooks.NpcStrike.Register((TerrariaPlugin)(object)this, (HookHandler<NpcStrikeEventArgs>)OnNpcStrike);
		ServerApi.Hooks.ProjectileAIUpdate.Register((TerrariaPlugin)(object)this, (HookHandler<ProjectileAiUpdateEventArgs>)OnProj);
		for (int i = 0; i < 256; i++)
		{
			sent.Add(item: false);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.ServerJoin.Deregister((TerrariaPlugin)(object)this, (HookHandler<JoinEventArgs>)OnServerJoin);
			ServerApi.Hooks.NetGetData.Deregister((TerrariaPlugin)(object)this, (HookHandler<GetDataEventArgs>)OnNetGetdata);
			ServerApi.Hooks.NpcStrike.Deregister((TerrariaPlugin)(object)this, (HookHandler<NpcStrikeEventArgs>)OnNpcStrike);
			ServerApi.Hooks.ProjectileAIUpdate.Deregister((TerrariaPlugin)(object)this, (HookHandler<ProjectileAiUpdateEventArgs>)OnProj);
		}
		this.Dispose(disposing);
	}

	private void OnServerJoin(JoinEventArgs args)
	{
		Multiples[args.Who] = 0;
	}

	private void OnProj(ProjectileAiUpdateEventArgs args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Config config = Config.GetConfig();
		if (!config.Projs.Contains(args.Projectile.type))
		{
			return;
		}
		Vector2 position = ((Entity)args.Projectile).position;
		position.X /= 16f;
		position.Y /= 16f;
		if (position.X > (float)config.Xmin && position.X < (float)config.Xmax && position.Y > (float)config.Ymin && position.Y < (float)config.Ymax)
		{
			Projectile projectile = args.Projectile;
			if (projectile.owner != 255)
			{
				TSPlayer val = TShock.Players[projectile.owner];
				TShock.Log.Warn($"{val.Name}疑似在保护区内({(int)((Entity)projectile).position.X / 16},{(int)((Entity)projectile).position.Y / 16})使用炸药");
				TSPlayer.All.SendWarningMessage(string.Format("警告:在({0},{1})检测到爆炸物，由玩家:{2}发射", (int)((Entity)projectile).position.X / 16, (int)((Entity)projectile).position.Y / 16, ((val != null) ? val.Name : null) ?? "未知"));
				AddCount(projectile.owner);
			}
			((Entity)projectile).active = false;
			projectile.type = 0;
			TSPlayer.All.SendData((PacketTypes)27, "", ((Entity)projectile).whoAmI, 0f, 0f, 0f, 0);
		}
	}

	private void OnNpcStrike(NpcStrikeEventArgs args)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (args.Npc.netID == 661)
		{
			TSPlayer val = TShock.Players[((Entity)args.Player).whoAmI];
			Config config = Config.GetConfig();
			if (val.TileX < config.Xmax && val.TileX > config.Xmin && val.TileY > config.Ymin && val.TileY < config.Ymax)
			{
				TShock.Utils.Broadcast(val.Name + "试图在禁用地区召唤", Color.Red);
				args.Damage = 0;
				((HandledEventArgs)(object)args).Handled = true;
				((Entity)args.Npc).active = false;
				AddCount(val.Index);
			}
		}
	}

	private void OnNetGetdata(GetDataEventArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Invalid comparison between Unknown and I4
		TSPlayer val = TShock.Players[args.Msg.whoAmI];
		Config config = Config.GetConfig();
		if ((int)args.MsgID == 61)
		{
			TShock.Utils.Broadcast(val.Name + "试图召唤", Color.Red);
			if (val.TileX < config.Xmax && val.TileX > config.Xmin && val.TileY > config.Ymin && val.TileY < config.Ymax)
			{
				TShock.Utils.Broadcast(val.Name + "试图在禁用地区召唤", Color.Red);
				((HandledEventArgs)(object)args).Handled = true;
				AddCount(val.Index);
			}
		}
		if ((int)args.MsgID != 13)
		{
			return;
		}
		if (val.TileX < config.Xmax && val.TileX > config.Xmin && val.TileY > config.Ymin && val.TileY < config.Ymax)
		{
			if (!sent[args.Msg.whoAmI])
			{
				val.SendInfoMessage("您已进入禁止召唤区域");
				sent[args.Msg.whoAmI] = true;
			}
		}
		else if (sent[args.Msg.whoAmI])
		{
			sent[args.Msg.whoAmI] = false;
			val.SendInfoMessage("您已离开禁止召唤区域");
		}
	}

	private void AddCount(int index)
	{
		if (Config.GetConfig().Multiplekickout)
		{
			Multiples[index]++;
			if (Multiples[index] >= Config.GetConfig().MultipleCount)
			{
				TShock.Players[index].Kick(TShock.Players[index].Name + "多次在禁地召唤或在保护区使用炸药,已踢出", true, false, (string)null, false);
			}
		}
	}
}
