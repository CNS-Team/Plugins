// Chameleon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Chameleon.Chameleon
using System;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Chameleon;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Chameleon;

[ApiVersion(2, 1)]
public class Chameleon : TerrariaPlugin
{
	public const string WaitPwd4Reg = "reg-pwd";

	public const ushort Size = 10;

	internal static Configuration Config;

	public static string[] PrepareList = new string[10];

	private readonly string _clientWasBooted;

	public override string Name => Assembly.GetExecutingAssembly().GetName().Name;

	public override string Author => "MistZZT & Cai修复";

	public override string Description => "账户系统交互替换方案";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public Chameleon(Main game)
		: base(game)
	{
		_clientWasBooted = Language.GetTextValue("CLI.ClientWasBooted", "", "").Trim();
	}

	public override void Initialize()
	{
		ServerApi.Hooks.NetGetData.Register(this, OnGetData, 9999);
		ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInit, 9999);
		ServerApi.Hooks.GameInitialize.Register(this, OnInit);
		GeneralHooks.ReloadEvent += ReloadConfig;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
			ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInit);
			ServerApi.Hooks.GameInitialize.Deregister(this, OnInit);
			GeneralHooks.ReloadEvent -= ReloadConfig;
		}
		base.Dispose(disposing);
	}

	private static void OnInit(EventArgs args)
	{
		LoadConfig();
	}

	private static void OnPostInit(EventArgs args)
	{
		if (!string.IsNullOrEmpty(TShock.Config.Settings.ServerPassword) || !string.IsNullOrEmpty(Netplay.ServerPassword))
		{
			TShock.Log.ConsoleError("[流光系统] 在启用本插件的情况下, 服务器密码功能将失效.");
		}
		if (TShock.Config.Settings.DisableLoginBeforeJoin)
		{
			TShock.Log.ConsoleError("[流光系统] 在启用本插件的情况下, 入服前登录将被强制开启.");
			TShock.Config.Settings.DisableLoginBeforeJoin = true;
		}
		if (!TShock.Config.Settings.RequireLogin && !TShock.ServerSideCharacterConfig.Settings.Enabled)
		{
			TShock.Log.ConsoleError("[流光系统] 在启用本插件的情况下, 注册登录将被强制开启.");
			TShock.Config.Settings.RequireLogin = true;
		}
	}

	private static void OnGetData(GetDataEventArgs args)
	{
		if (args.Handled)
		{
			return;
		}
		PacketTypes msgID = args.MsgID;
		TSPlayer tSPlayer = TShock.Players[args.Msg.whoAmI];
		if (tSPlayer == null || !tSPlayer.ConnectionAlive)
		{
			args.Handled = true;
			return;
		}
		if (tSPlayer.RequiresPassword && msgID != PacketTypes.PasswordSend)
		{
			args.Handled = true;
			return;
		}
		if ((tSPlayer.State < 10 || tSPlayer.Dead) && msgID > PacketTypes.PlayerSpawn && msgID != PacketTypes.PlayerHp && msgID != PacketTypes.PlayerMana && msgID != PacketTypes.PlayerBuff && msgID != PacketTypes.PasswordSend && msgID != PacketTypes.ItemDrop && msgID != PacketTypes.ItemOwner)
		{
			args.Handled = true;
			return;
		}
		using MemoryStream s = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1);
		switch (msgID)
		{
		case PacketTypes.ContinueConnecting2:
			args.Handled = HandleConnecting(tSPlayer);
			break;
		case PacketTypes.PasswordSend:
			args.Handled = HandlePassword(tSPlayer, s.ReadString());
			break;
		}
	}

	private static bool HandleConnecting(TSPlayer player)
	{
		TSPlayer player2 = player;
		UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(player2.Name);
		player2.DataWhenJoined = new PlayerData(player2);
		player2.DataWhenJoined.CopyCharacter(player2);
		if (userAccountByName != null)
		{
			if (!TShock.Config.Settings.DisableUUIDLogin && userAccountByName.UUID == player2.UUID)
			{
				if (player2.State == 1)
				{
					player2.State = 2;
				}
				NetMessage.SendData(7, player2.Index);
				player2.PlayerData = TShock.CharacterDB.GetPlayerData(player2, userAccountByName.ID);
				Group groupByName = TShock.Groups.GetGroupByName(userAccountByName.Group);
				player2.Group = groupByName;
				player2.tempGroup = null;
				player2.Account = userAccountByName;
				player2.IsLoggedIn = true;
				player2.IsDisabledForSSC = false;
				if (Main.ServerSideCharacter)
				{
					if (player2.HasPermission(Permissions.bypassssc))
					{
						player2.PlayerData.CopyCharacter(player2);
						TShock.CharacterDB.InsertPlayerData(player2);
					}
					player2.PlayerData.RestoreCharacter(player2);
				}
				player2.LoginFailsBySsi = false;
				if (player2.HasPermission(Permissions.ignorestackhackdetection))
				{
					player2.IsDisabledForStackDetection = false;
				}
				if (player2.HasPermission(Permissions.usebanneditem))
				{
					player2.IsDisabledForBannedWearable = false;
				}
				player2.SendSuccessMessage("[账号管理]已经验证" + userAccountByName.Name + "登录完毕.");
				TShock.Log.ConsoleInfo(player2.Name + "成功验证登录.");
				PlayerHooks.OnPlayerPostLogin(player2);
				return true;
			}
			Task.Run(delegate
			{
				Thread.Sleep(1000);
				NetMessage.SendData(9, player2.Index, -1, NetworkText.FromLiteral("[流光系统]输入你的账号密码登录...\n在\"服务器密码\"中\n"), 1);
				Thread.Sleep(3000);
				player2.RequiresPassword = true;
				NetMessage.SendData(37, player2.Index);
			});
			return true;
		}
		if (Config.EnableForcedHint && !PrepareList.Contains(player2.Name))
		{
			AddToList(player2.Name);
			Kick(player2, string.Join("\n", Config.Hints), Config.Greeting);
			return true;
		}
		Task.Run(delegate
		{
			Thread.Sleep(1000);
			NetMessage.SendData(9, player2.Index, -1, NetworkText.FromLiteral("[流光系统]请设置你的账号密码...\n在\"服务器密码\"中\n"), 1);
			Thread.Sleep(3000);
			player2.SetData("reg-pwd", value: true);
			NetMessage.SendData(37, player2.Index);
		});
		return true;
	}

	private static bool HandlePassword(TSPlayer player, string password)
	{
		bool data = player.GetData<bool>("reg-pwd");
		if (string.IsNullOrEmpty(password))
		{
			player.Disconnect("[流光系统]你输入了一个空密码!");
			return true;
		}
        if (password.Length > Config.LimitPasswordLength)
        {
            player.Disconnect($"[流光系统]密码长度不能超过{Config.LimitPasswordLength}!");
            return true;
        }
		if (!player.RequiresPassword && !data)
		{
			return true;
		}
		if (!data && PlayerHooks.OnPlayerPreLogin(player, player.Name, password))
		{
			return true;
		}
		UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(player.Name);
		if (userAccountByName != null)
		{
			if (userAccountByName.VerifyPassword(password))
			{
				player.RequiresPassword = false;
				player.PlayerData = TShock.CharacterDB.GetPlayerData(player, userAccountByName.ID);
				if (player.State == 1)
				{
					player.State = 2;
				}
				NetMessage.SendData(7, player.Index);
				Group groupByName = TShock.Groups.GetGroupByName(userAccountByName.Group);
				player.Group = groupByName;
				player.tempGroup = null;
				player.Account = userAccountByName;
				player.IsLoggedIn = true;
				player.IsDisabledForSSC = false;
				if (Main.ServerSideCharacter)
				{
					if (player.HasPermission(Permissions.bypassssc))
					{
						player.PlayerData.CopyCharacter(player);
						TShock.CharacterDB.InsertPlayerData(player);
					}
					player.PlayerData.RestoreCharacter(player);
				}
				player.LoginFailsBySsi = false;
				if (player.HasPermission(Permissions.ignorestackhackdetection))
				{
					player.IsDisabledForStackDetection = false;
				}
				if (player.HasPermission(Permissions.usebanneditem))
				{
					player.IsDisabledForBannedWearable = false;
				}
				player.SendSuccessMessage("[账号管理]已经验证" + userAccountByName.Name + "登录完毕。");
				TShock.Log.ConsoleInfo(player.Name + "成功验证登录。");
				TShock.UserAccounts.SetUserAccountUUID(userAccountByName, player.UUID);
				PlayerHooks.OnPlayerPostLogin(player);
				return true;
			}
			Kick(player, Config.VerficationFailedMessage, "验证失败");
			return true;
		}
		if (player.Name != TSServerPlayer.AccountName)
		{
			userAccountByName = new UserAccount
			{
				Name = player.Name,
				Group = TShock.Config.Settings.DefaultRegistrationGroupName,
				UUID = player.UUID
			};
			try
			{
				userAccountByName.CreateBCryptHash(password);
			}
			catch (ArgumentOutOfRangeException)
			{
				Kick(player, "密码位数不能少于" + TShock.Config.Settings.MinimumPasswordLength + "个字符.", "注册失败");
				return true;
			}
			player.SendSuccessMessage("账户{0}注册成功。", userAccountByName.Name);
			player.SendSuccessMessage("你的密码是{0}", password);
			TShock.UserAccounts.AddUserAccount(userAccountByName);
			TShock.Log.ConsoleInfo("玩家{0}注册了新账户：{1}", player.Name, userAccountByName.Name);
			player.RequiresPassword = false;
			player.SetData("reg-pwd", value: false);
			player.PlayerData = TShock.CharacterDB.GetPlayerData(player, userAccountByName.ID);
			if (player.State == 1)
			{
				player.State = 2;
			}
			NetMessage.SendData(7, player.Index);
			Group groupByName2 = TShock.Groups.GetGroupByName(userAccountByName.Group);
			player.Group = groupByName2;
			player.tempGroup = null;
			player.Account = userAccountByName;
			player.IsLoggedIn = true;
			player.IsDisabledForSSC = false;
			if (Main.ServerSideCharacter)
			{
				if (player.HasPermission(Permissions.bypassssc))
				{
					player.PlayerData.CopyCharacter(player);
					TShock.CharacterDB.InsertPlayerData(player);
				}
				player.PlayerData.RestoreCharacter(player);
			}
			player.LoginFailsBySsi = false;
			if (player.HasPermission(Permissions.ignorestackhackdetection))
			{
				player.IsDisabledForStackDetection = false;
			}
			if (player.HasPermission(Permissions.usebanneditem))
			{
				player.IsDisabledForBannedWearable = false;
			}
			player.SendSuccessMessage("[账号管理]已经验证" + userAccountByName.Name + "登录完毕.");
			TShock.Log.ConsoleInfo(player.Name + "成功验证登录.");
			TShock.UserAccounts.SetUserAccountUUID(userAccountByName, player.UUID);
			PlayerHooks.OnPlayerPostLogin(player);
			return true;
		}
		Kick(player, "该用户名已被占用.", "请更换人物名");
		return true;
	}

	private static void AddToList(string playerName)
	{
		int i;
		for (i = 0; i < PrepareList.Length && !string.IsNullOrEmpty(PrepareList[i]); i++)
		{
		}
		PrepareList[i % PrepareList.Length] = playerName;
	}

	public static void Kick(TSPlayer player, string msg, string custom)
	{
		if (player.ConnectionAlive)
		{
			player.SilentKickInProgress = true;
			player.Disconnect("    " + custom + "：" + msg);
			TShock.Log.ConsoleInfo("向" + player.Name + "发送通知完毕.");
		}
	}

	private static void LoadConfig()
	{
		Config = Configuration.Read(Configuration.FilePath);
		Config.Write(Configuration.FilePath);
		if (Config.AwaitBufferSize != 10)
		{
			Array.Resize(ref PrepareList, Config.AwaitBufferSize);
			Array.Clear(PrepareList, 0, Config.AwaitBufferSize);
		}
	}

	private static void ReloadConfig(ReloadEventArgs args)
	{
		LoadConfig();
		args.Player?.SendSuccessMessage("重新加载{0}配置完毕。", typeof(global::Chameleon.Chameleon).Name);
	}
}
