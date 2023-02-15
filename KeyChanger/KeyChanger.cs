using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace KeyChanger;

[ApiVersion(2, 1)]
public class KeyChanger : TerrariaPlugin
{
	public override string Author => "Dr&Cai&CJX改适配";

	public static Config Config { get; set; }

	public override string Description => "使用特殊宝箱钥匙兑换其宝箱特别物品.";

	public KeyTypes?[] Exchanging { get; private set; }

	public override string Name => "钥匙兑换（汉化版）";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public KeyChanger(Main game)
		: base(game)
	{
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.NetGetData.Deregister((TerrariaPlugin)(object)this, (HookHandler<GetDataEventArgs>)onGetData);
			ServerApi.Hooks.ServerLeave.Deregister((TerrariaPlugin)(object)this, (HookHandler<LeaveEventArgs>)onLeave);
		}
	}

	public override void Initialize()
	{
		onInitialize();
		ServerApi.Hooks.NetGetData.Register((TerrariaPlugin)(object)this, (HookHandler<GetDataEventArgs>)onGetData, -1);
		ServerApi.Hooks.ServerLeave.Register((TerrariaPlugin)(object)this, (HookHandler<LeaveEventArgs>)onLeave);
	}

	private void onGetData(GetDataEventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		if (Config.UseSSC || ((HandledEventArgs)(object)e).Handled || (int)e.MsgID != 21 || TShock.Players[e.Msg.whoAmI] == null || !TShock.Players[e.Msg.whoAmI].Active || !Exchanging[e.Msg.whoAmI].HasValue)
		{
			return;
		}
		using BinaryReader binaryReader = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length));
		int num = binaryReader.ReadInt16();
		if (num != 400)
		{
			return;
		}
		binaryReader.ReadSingle();
		binaryReader.ReadSingle();
		binaryReader.ReadSingle();
		binaryReader.ReadSingle();
		short num2 = binaryReader.ReadInt16();
		binaryReader.ReadByte();
		binaryReader.ReadByte();
		int num3 = binaryReader.ReadInt16();
		if (num3 != (int)Exchanging[e.Msg.whoAmI].Value)
		{
			return;
		}
		if (!TShock.Players[e.Msg.whoAmI].InventorySlotAvailable)
		{
			TShock.Players[e.Msg.whoAmI].SendWarningMessage("兑换时请确保背包里最少有一个空置位.");
			return;
		}
		Key key = Utils.LoadKey(Exchanging[e.Msg.whoAmI].Value);
		if (Config.EnableRegionExchanges)
		{
			Region val = ((!Config.MarketMode) ? key.Region : TShock.Regions.GetRegionByName(Config.MarketRegion));
			if (!val.InArea((int)TShock.Players[e.Msg.whoAmI].X, (int)TShock.Players[e.Msg.whoAmI].Y))
			{
				return;
			}
		}
		TShock.Players[e.Msg.whoAmI].SendData((PacketTypes)21, "", num, 0f, 0f, 0f, 0);
		Random random = new Random();
		Item val2 = key.Items[random.Next(0, key.Items.Count)];
		if (val2.maxStack >= num2)
		{
			TShock.Players[e.Msg.whoAmI].GiveItem(val2.netID, (int)num2, 0);
			Item itemById = TShock.Utils.GetItemById((int)key.Type);
			TSPlayer val3 = TShock.Players[e.Msg.whoAmI];
			val3.SendSuccessMessage($"使用 {num2}个 {itemById.Name} 兑换 {num2}个 {val2.Name}!");
		}
		else
		{
			TShock.Players[e.Msg.whoAmI].GiveItem(val2.netID, 1, 0);
			Item itemById2 = TShock.Utils.GetItemById((int)key.Type);
			TShock.Players[e.Msg.whoAmI].SendSuccessMessage("使用 1个 " + itemById2.Name + " 兑换 1个 " + val2.Name + " !");
			TShock.Players[e.Msg.whoAmI].GiveItem(val2.netID, num2 - 1, 0);
			TShock.Players[e.Msg.whoAmI].SendSuccessMessage("返还剩余的钥匙.");
		}
		Exchanging[e.Msg.whoAmI] = null;
		((HandledEventArgs)(object)e).Handled = true;
	}

	private void onInitialize()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		Config = Config.Read();
		List<Command> chatCommands = Commands.ChatCommands;
		Command val = new Command(new List<string> { "key.change", "key.reload", "key.mode" }, new CommandDelegate(KeyChange), new string[1] { "key" });
		val.HelpDesc = new string[6]
		{
			Commands.Specifier + "key - 显示插件内容.",
			Commands.Specifier + "key change <type> - 使用特定钥匙兑换特定物品.",
			Commands.Specifier + "key list - 显示所有能兑换的钥匙.",
			Commands.Specifier + "key mode <mode> - 更换兑换模式.",
			Commands.Specifier + "key reload - 重读配置文件.",
			"如兑换失败, 请确保背包里有足够位置."
		};
		chatCommands.Add(val);
		Exchanging = new KeyTypes?[Main.maxNetPlayers];
	}

	private void onLeave(LeaveEventArgs e)
	{
		if (e.Who >= 0 || e.Who < Main.maxNetPlayers)
		{
			Exchanging[e.Who] = null;
		}
	}

	private void KeyChange(CommandArgs args)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		TSPlayer player = args.Player;
		if (!Main.ServerSideCharacter)
		{
			player.SendWarningMessage("[警告] 本插件在非SSC环境下无法运行.");
		}
		if (args.Parameters.Count < 1)
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			player.SendMessage("插件内容: 使用特殊宝箱钥匙兑换特定物品", Color.SkyBlue);
			player.SendMessage("格式: " + Commands.Specifier + "key <list/mode/change/reload> [type]", Color.SkyBlue);
			player.SendMessage("输入指令 " + Commands.Specifier + "help key 以换取更多资讯", Color.SkyBlue);
		}
		else if (args.Parameters[0].ToLower() == "change" && args.Parameters.Count == 1)
		{
			player.SendErrorMessage("格式错误! 正确格式为: {0}key change <type>", new object[1] { Commands.Specifier });
		}
		else if (args.Parameters.Count > 0)
		{
			switch (args.Parameters[0].ToLower())
			{
			default:
				player.SendErrorMessage(Utils.ErrorMessage(player));
				break;
			case "mode":
				if (!player.HasPermission("key.mode"))
				{
					player.SendErrorMessage("你没有使用此指令的权限.");
					break;
				}
				if (args.Parameters.Count < 2)
				{
					player.SendErrorMessage("格式错误! 正确格式为: {0}key mode <normal/region/market>", new object[1] { Commands.Specifier });
					break;
				}
				switch (args.Parameters[1].ToLower())
				{
				case "normal":
					Config.EnableRegionExchanges = false;
					player.SendSuccessMessage("兑换模式设置为普通 (随地兑换).");
					break;
				case "region":
					Config.EnableRegionExchanges = true;
					Config.MarketMode = false;
					player.SendSuccessMessage("兑换模式设置为领地 (一个钥匙对应一个领地).");
					break;
				default:
					player.SendErrorMessage("格式错误! 正确格式为: {0}key mode <normal/region/market>", new object[1] { Commands.Specifier });
					return;
				case "market":
					Config.EnableRegionExchanges = true;
					Config.MarketMode = true;
					player.SendSuccessMessage("兑换模式设置为市场 (一个领地可兑换所有钥匙).");
					break;
				}
				Config.Write();
				break;
			case "list":
				player.SendMessage("Temple Key - " + string.Join(", ", Utils.LoadKey(KeyTypes.Temple).Items.Select((Item i) => i.Name)), Color.Goldenrod);
				player.SendMessage("Jungle Key - " + string.Join(", ", Utils.LoadKey(KeyTypes.Jungle).Items.Select((Item i) => i.Name)), Color.Goldenrod);
				player.SendMessage("Corruption Key - " + string.Join(", ", Utils.LoadKey(KeyTypes.Corruption).Items.Select((Item i) => i.Name)), Color.Goldenrod);
				player.SendMessage("Crimson Key - " + string.Join(", ", Utils.LoadKey(KeyTypes.Crimson).Items.Select((Item i) => i.Name)), Color.Goldenrod);
				player.SendMessage("Hallowed Key - " + string.Join(", ", Utils.LoadKey(KeyTypes.Hallowed).Items.Select((Item i) => i.Name)), Color.Goldenrod);
				player.SendMessage("Frozen Key - " + string.Join(", ", Utils.LoadKey(KeyTypes.Frozen).Items.Select((Item i) => i.Name)), Color.Goldenrod);
				player.SendMessage("Desert Key - " + string.Join(", ", Utils.LoadKey(KeyTypes.Desert).Items.Select((Item i) => i.Name)), Color.Goldenrod);
				break;
			case "reload":
				if (!player.HasPermission("key.reload"))
				{
					player.SendErrorMessage("你没有使用此指令的权限");
					break;
				}
				Config = Config.Read();
				player.SendSuccessMessage("KeyChangerConfig.json 重载成功.");
				break;
			case "change":
			{
				if (!NPC.downedPlantBoss)
				{
					player.SendErrorMessage("只有击败世纪之花后才可以使用KeyChange.");
					break;
				}
				if (!player.RealPlayer)
				{
					player.SendErrorMessage("你必须在有戏里使用本指令.");
					break;
				}
				if (!player.HasPermission("key.change"))
				{
					player.SendErrorMessage("你没有使用此指令的权限.");
					break;
				}
				if (!Enum.TryParse<KeyTypes>(args.Parameters[1].ToLowerInvariant(), ignoreCase: true, out var result))
				{
					player.SendErrorMessage("钥匙种类错误! 可兑换的种类: " + string.Join(", ", Config.EnableTempleKey ? "temple" : null, Config.EnableJungleKey ? "jungle" : null, Config.EnableCorruptionKey ? "corruption" : null, Config.EnableCrimsonKey ? "crimson" : null, Config.EnableHallowedKey ? "hallowed" : null, Config.EnableFrozenKey ? "frozen" : null, Config.EnableDesertKey ? "desert" : null));
					break;
				}
				Key key = Utils.LoadKey(result);
				if (!key.Enabled)
				{
					player.SendInfoMessage("你所选择的钥匙当前不能兑换.");
					break;
				}
				if (!Config.UseSSC)
				{
					Exchanging[args.Player.Index] = result;
					player.SendInfoMessage("掉落 (右键拿着) 任意数量的 " + key.Name + " 钥匙以继续.");
					break;
				}
				Item val = ((IEnumerable<Item>)player.TPlayer.inventory).FirstOrDefault((Func<Item, bool>)((Item i) => i.netID == (int)key.Type));
				if (val == null)
				{
					player.SendErrorMessage("请确保背包里有所选择的钥匙.");
					break;
				}
				if (Config.EnableRegionExchanges)
				{
					Region val2 = ((!Config.MarketMode) ? key.Region : TShock.Regions.GetRegionByName(Config.MarketRegion));
					if (val2 == null)
					{
						player.SendInfoMessage("没有设置兑换此钥匙的领地.");
						break;
					}
					if (!val2.InArea(args.Player.TileX, args.Player.TileY))
					{
						player.SendErrorMessage("你当前位置非指定兑换领地.");
						break;
					}
				}
				for (int j = 0; j < 50; j++)
				{
					Item val3 = player.TPlayer.inventory[j];
					if (val3.netID == (int)key.Type)
					{
						if (val3.stack < Config.NumberOfKeys)
						{
							player.SendErrorMessage("请确保你最少有 {0} 个钥匙", new object[1] { Config.NumberOfKeys });
						}
						else if (player.InventorySlotAvailable)
						{
							Item obj = player.TPlayer.inventory[j];
							obj.stack -= Config.NumberOfKeys;
							NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, (float)j, 0f, 0f, 0, 0, 0);
							Random random = new Random();
							Item val4 = key.Items[random.Next(0, key.Items.Count)];
							player.GiveItem(val4.netID, 1, 0);
							Item itemById = TShock.Utils.GetItemById((int)key.Type);
							player.SendSuccessMessage("使用 {2}个 {0} 兑换 1个 {1}!", new object[3] { itemById.Name, val4.Name, Config.NumberOfKeys });
						}
						else
						{
							player.SendErrorMessage("请确保你背包里有最少一个空置位置.");
						}
						break;
					}
				}
				break;
			}
			}
		}
		else
		{
			player.SendErrorMessage(Utils.ErrorMessage(player));
		}
	}
}
