using System.IO.Streams;
using System.Reflection;
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

    public static List<string> PrepareList = new();


    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Author => "MistZZT & Cai修复";

    public override string Description => "账户系统交互替换方案";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public Chameleon(Main game)
        : base(game)
    {
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
        if (!TShock.Config.Settings.DisableLoginBeforeJoin)
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
        var msgID = args.MsgID;
        var tSPlayer = TShock.Players[args.Msg.whoAmI];
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
        using var s = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1);
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
        var userAccountByName = TShock.UserAccounts.GetUserAccountByName(player.Name);
        player.DataWhenJoined = new PlayerData(player);
        player.DataWhenJoined.CopyCharacter(player);
        if (userAccountByName != null)
        {
            if (!TShock.Config.Settings.DisableUUIDLogin && userAccountByName.UUID == player.UUID)
            {
                if (player.State == 1)
                {
                    player.State = 2;
                }
                NetMessage.SendData(7, player.Index);
                player.PlayerData = TShock.CharacterDB.GetPlayerData(player, userAccountByName.ID);
                var groupByName = TShock.Groups.GetGroupByName(userAccountByName.Group);
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
                player.SendSuccessMessage("[账号管理]已经验证" + userAccountByName.Name + "登录完毕.");
                TShock.Log.ConsoleInfo(player.Name + "成功验证登录.");
                PlayerHooks.OnPlayerPostLogin(player);
                return true;
            }
            NetMessage.SendData(9, player.Index, -1, NetworkText.FromLiteral("[流光系统]输入你的账号密码登录...\n在\"服务器密码\"中\n"), 1);
            player.RequiresPassword = true;
            NetMessage.SendData(37, player.Index);
            return true;
        }
        if (Config.EnableForcedHint && !PrepareList.Contains(player.Name))
        {
            PrepareList.Add(player.Name);
            Kick(player, string.Join("\n", Config.Hints), Config.Greeting);
            return true;
        }

        NetMessage.SendData(9, player.Index, -1, NetworkText.FromLiteral("[流光系统]请设置你的账号密码...\n在\"服务器密码\"中\n"), 1);
        player.SetData("reg-pwd", value: true);
        NetMessage.SendData(37, player.Index);
        return true;
    }

    private static bool HandlePassword(TSPlayer player, string password)
    {
        var data = player.GetData<bool>("reg-pwd");
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
        var userAccountByName = TShock.UserAccounts.GetUserAccountByName(player.Name);
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
            var groupByName2 = TShock.Groups.GetGroupByName(userAccountByName.Group);
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
            PrepareList.Remove(player.Name);
            return true;
        }
        Kick(player, "该用户名已被占用.", "请更换人物名");
        return true;
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
    }

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player?.SendSuccessMessage("重新加载{0}配置完毕。", typeof(Chameleon).Name);
    }
}
