using System;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PlaceholderAPI;

[ApiVersion(2, 1)]
public class PlaceholderAPI : TerrariaPlugin
{
	private static PlaceholderAPI instance;

	public PlaceholderManager placeholderManager = new PlaceholderManager();

	public override string Name => "PlaceholderAPI";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public override string Author => "豆沙";

	public override string Description => "一款通用占位符插件";

	public static PlaceholderAPI Instance => instance;

	public PlaceholderAPI(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		instance = this;
		Register();
		Hooks.PreGetText += OnGetText;
	}

	private void OnGetText(Hooks.GetTextArgs args)
	{
		TSPlayer player = args.Player;
		args.List["{player}"] = player.Name;
		args.List["{group}"] = player.Group.Name;
		args.List["{helditem}"] = player.TPlayer.HeldItem.netID.ToString();
		args.List["{playerDead}"] = (player.Dead ? "已死亡" : "存活");
		args.List["{playerMaxHP}"] = player.TPlayer.statLifeMax.ToString();
		args.List["{playerMaxMana}"] = player.TPlayer.statManaMax.ToString();
		args.List["{playerHP}"] = player.TPlayer.statLife.ToString();
		args.List["{playerMana}"] = player.TPlayer.statMana.ToString();
		args.List["{region}"] = ((player.CurrentRegion == null) ? "无" : player.CurrentRegion.Name);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			Hooks.PreGetText -= OnGetText;
		}
		this.Dispose(disposing);
	}

	private void Register()
	{
		placeholderManager.Register("{player}");
		placeholderManager.Register("{group}");
		placeholderManager.Register("{helditem}");
		placeholderManager.Register("{playerDead}");
		placeholderManager.Register("{playerMaxHP}");
		placeholderManager.Register("{playerMaxMana}");
		placeholderManager.Register("{playerHP}");
		placeholderManager.Register("{playerMana}");
		placeholderManager.Register("{region}");
	}
}
