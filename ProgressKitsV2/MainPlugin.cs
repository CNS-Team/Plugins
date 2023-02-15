using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ProgressKitsV2;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
	public static CommandManager commandManager;

	public override string Name => "ProgressKitsV2";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public override string Author => "豆沙";

	public override string Description => "进度礼包第二版";

	public MainPlugin(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		ConfigUtils.LoadConfig();
		commandManager = new CommandManager();
		ServerApi.Hooks.NetGreetPlayer.Register((TerrariaPlugin)(object)this, (HookHandler<GreetPlayerEventArgs>)OnGreetPlayer);
		ServerApi.Hooks.NpcKilled.Register((TerrariaPlugin)(object)this, (HookHandler<NpcKilledEventArgs>)OnKilled);
		ServerApi.Hooks.ServerLeave.Register((TerrariaPlugin)(object)this, (HookHandler<LeaveEventArgs>)OnLeave);
	}

	private void OnLeave(LeaveEventArgs args)
	{
		KitPlayer playerByName = ConfigUtils.GetPlayerByName(TShock.Players[args.Who].Name);
		if (playerByName != null && playerByName.player != null)
		{
			playerByName.player = null;
		}
	}

	private void OnGreetPlayer(GreetPlayerEventArgs args)
	{
		TSPlayer val = TShock.Players[args.Who];
		KitPlayer playerByName = ConfigUtils.GetPlayerByName(val.Name);
		if (playerByName == null)
		{
			playerByName = new KitPlayer(ConfigUtils.players.Count + 1, val.Name);
			playerByName.availableKits.AddRange(ConfigUtils.firstKits);
			playerByName.player = val;
			ConfigUtils.players.Add(playerByName);
			TShock.Log.ConsoleInfo("[ProgressKit] 玩家 " + val.Name + " 数据已添加");
			ConfigUtils.UpdatePlayer();
		}
		else
		{
			playerByName.player = val;
			TShock.Log.ConsoleInfo("[ProgressKit] 玩家 " + val.Name + " 数据已加载");
		}
	}

	private void OnKilled(NpcKilledEventArgs args)
	{
		NpcKilledEventArgs args2 = args;
		Task.Run(delegate
		{
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			foreach (PKit loadedKit in ConfigUtils.loadedKits)
			{
				if (loadedKit.mobs.Contains(args2.npc.netID))
				{
					if ((args2.npc.netID == 13 || args2.npc.netID == 14 || args2.npc.netID == 15) && !NPC.downedBoss2)
					{
						break;
					}
					foreach (KitPlayer player in ConfigUtils.players)
					{
						if (!player.availableKits.Contains(loadedKit.id) && !player.aquiredKits.Contains(loadedKit.id))
						{
							player.availableKits.Add(loadedKit.id);
							if (player.player != null)
							{
								player.player.SendMessage("您有新的进度礼包可以领取了", Color.MediumAquamarine);
							}
							ConfigUtils.UpdatePlayer();
						}
					}
					if (!ConfigUtils.firstKits.Contains(loadedKit.id))
					{
						ConfigUtils.firstKits.Add(loadedKit.id);
						ConfigUtils.UpdateFirst();
					}
				}
			}
		});
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			commandManager = null;
			ServerApi.Hooks.NetGreetPlayer.Deregister((TerrariaPlugin)(object)this, (HookHandler<GreetPlayerEventArgs>)OnGreetPlayer);
			ServerApi.Hooks.NpcKilled.Deregister((TerrariaPlugin)(object)this, (HookHandler<NpcKilledEventArgs>)OnKilled);
			ServerApi.Hooks.ServerLeave.Deregister((TerrariaPlugin)(object)this, (HookHandler<LeaveEventArgs>)OnLeave);
		}
		this.Dispose(disposing);
	}
}
