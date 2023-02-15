using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Freeze;

[ApiVersion(2, 1)]
public class FreezePlugin : TerrariaPlugin
{
	private readonly HashSet<string> hash = new HashSet<string>();

	private bool freezeAll = false;

	public override string Name => "Freeze";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public override string Author => "棱镜";

	public override string Description => "临时冻结玩家";

	public FreezePlugin(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		ServerApi.Hooks.GamePostUpdate.Register((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)PostGameUpdate);
		Commands.ChatCommands.Add(new Command("tshock.admin.kick", new CommandDelegate(FreezeCmd), new string[1] { "freeze" })
		{
			HelpText = "用法:/freeze <玩家名>"
		});
		Commands.ChatCommands.Add(new Command("tshock.admin.kick", new CommandDelegate(FreezeAllCmd), new string[1] { "freezeall" }));
	}

	private void FreezeAllCmd(CommandArgs args)
	{
		freezeAll = !freezeAll;
		args.Player.SendSuccessMessage("全服冻结已" + (freezeAll ? "开启" : "关闭"));
		TSPlayer.All.SendInfoMessage(args.Player.Name + (freezeAll ? "开启" : "关闭") + "了全服冻结");
		if (args.Parameters.Count > 0)
		{
			TSPlayer.All.SendInfoMessage(args.Parameters[0]);
		}
	}

	private void PostGameUpdate(EventArgs args)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		TSPlayer[] players = TShock.Players;
		TSPlayer[] array = players;
		foreach (TSPlayer val in array)
		{
			if (val != null && (hash.Contains(val.Name) || freezeAll) && !val.HasPermission("tshock.admin.kick"))
			{
				val.Disable("", (DisableFlags)0);
				((Entity)val.TPlayer).Bottom = new Vector2((float)(Main.spawnTileX * 16), (float)(Main.spawnTileY * 16));
				val.SendData((PacketTypes)13, "", val.Index, 0f, 0f, 0f, 0);
			}
		}
	}

	private void FreezeCmd(CommandArgs args)
	{
		if (args.Parameters.Count == 0)
		{
			args.Player.SendInfoMessage("用法:/freeze <玩家名>");
			return;
		}
		List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
		if (list.Count > 1)
		{
			args.Player.SendMultipleMatchError((IEnumerable<object>)list);
			return;
		}
		if (list.Count == 0)
		{
			args.Player.SendErrorMessage("找不到玩家");
			return;
		}
		TSPlayer val = list[0];
		if (hash.Add(val.Name))
		{
			val.Disable("被管理员冻结", (DisableFlags)0);
			args.Player.SendSuccessMessage("已冻结" + val.Name);
		}
		else
		{
			args.Player.SendSuccessMessage("已解除冻结" + val.Name);
			hash.Remove(val.Name);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
		}
		this.Dispose(disposing);
	}
}
