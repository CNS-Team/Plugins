using System;
using System.ComponentModel;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace SignLock;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
	public override string Name => "Sign Lock";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public override string Author => "棱镜";

	public override string Description => "提供锁告示牌功能";

	public MainPlugin(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		GetDataHandlers.Sign += OnSignEdit;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			GetDataHandlers.Sign -= OnSignEdit;
		}
		this.Dispose(disposing);
	}

	private void OnSignEdit(object _, GetDataHandlers.SignEventArgs e)
	{
		if (!((GetDataHandledEventArgs)e).Player.HasBuildPermission(e.X, e.Y, true))
		{
			((HandledEventArgs)(object)e).Handled = true;
			((GetDataHandledEventArgs)e).Player.SendErrorMessage("你无权修改受保护告示牌的内容");
			TSPlayer.All.SendData((PacketTypes)47, "", (int)e.ID, 0f, 0f, 0f, 0);
		}
	}
}
