using Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using System.IO.Streams;
using TShockAPI.Hooks;
using Terraria.Localization;
using TShockAPI.DB;

namespace ServerTools.Chameleon;
internal class Main
{
    public const string WaitPwd4Reg = "reg-pwd";

    public const ushort Size = 10;

    public static string[] PrepareList = new string[10];
    internal static void OnGetData(GetDataEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }
        var msgID = args.MsgID;
        var val = TShock.Players[args.Msg.whoAmI];
        if (val == null || !val.ConnectionAlive)
        {
            args.Handled = true;
            return;
        }
        if (val.RequiresPassword && (int) msgID != 38)
        {
            args.Handled = true;
            return;
        }
        if ((val.State < 10 || val.Dead) && (int) msgID > 12 && (int) msgID != 16 && (int) msgID != 42 && (int) msgID != 50 && (int) msgID != 38 && (int) msgID != 21 && (int) msgID != 22)
        {
            args.Handled = true;
            return;
        }
        using var memoryStream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1);
        if ((int) msgID == 6)
        {
            args.Handled = HandleConnecting(val);
        }
        else if ((int) msgID == 38)
        {
            args.Handled = HandlePassword(val, StreamExt.ReadString(memoryStream));
        }
    }

    internal static void OnPostInit(EventArgs args)
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

    private static bool HandleConnecting(TSPlayer player)
    {
        //IL_0030: Unknown result type (might be due to invalid IL or missing references)
        //IL_003a: Expected O, but got Unknown
        var player2 = player;
        var userAccountByName = TShock.UserAccounts.GetUserAccountByName(player2.Name);
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
                NetMessage.SendData(7, player2.Index, -1, (NetworkText) null, 0, 0f, 0f, 0f, 0, 0, 0);
                player2.PlayerData = TShock.CharacterDB.GetPlayerData(player2, userAccountByName.ID);
                var groupByName = TShock.Groups.GetGroupByName(userAccountByName.Group);
                player2.Group = groupByName;
                player2.tempGroup = null;
                player2.Account = userAccountByName;
                player2.IsLoggedIn = true;
                player2.IsDisabledForSSC = false;
                if (Terraria.Main.ServerSideCharacter)
                {
                    if (player2.HasPermission(Permissions.bypassssc))
                    {
                        player2.PlayerData.CopyCharacter(player2);
                        TShock.CharacterDB.InsertPlayerData(player2, false);
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
                NetMessage.SendData(9, player2.Index, -1, NetworkText.FromLiteral("[流光系统]输入你的账号密码登录...\n在\"服务器密码\"中\n"), 1, 0f, 0f, 0f, 0, 0, 0);
                Thread.Sleep(3000);
                player2.RequiresPassword = true;
                NetMessage.SendData(37, player2.Index, -1, NetworkText.Empty, 0, 0f, 0f, 0f, 0, 0, 0);
            });
            return true;
        }
        if (Plugin.Config.EnableForcedHint && !PrepareList.Contains(player2.Name))
        {
            AddToList(player2.Name);
            Kick(player2, string.Join("\n", Plugin.Config.Hints), Plugin.Config.Greeting);
            return true;
        }
        Task.Run(delegate
        {
            Thread.Sleep(1000);
            NetMessage.SendData(9, player2.Index, -1, NetworkText.FromLiteral("[流光系统]请设置你的账号密码...\n在\"服务器密码\"中\n"), 1, 0f, 0f, 0f, 0, 0, 0);
            Thread.Sleep(3000);
            player2.SetData<bool>("reg-pwd", true);
            NetMessage.SendData(37, player2.Index, -1, NetworkText.Empty, 0, 0f, 0f, 0f, 0, 0, 0);
        });
        return true;
    }

    private static bool HandlePassword(TSPlayer player, string password)
    {
        //IL_0278: Unknown result type (might be due to invalid IL or missing references)
        //IL_027d: Unknown result type (might be due to invalid IL or missing references)
        //IL_028a: Unknown result type (might be due to invalid IL or missing references)
        //IL_02a0: Unknown result type (might be due to invalid IL or missing references)
        //IL_02af: Expected O, but got Unknown
        var data = player.GetData<bool>("reg-pwd");
        if (string.IsNullOrEmpty(password))
        {
            player.Disconnect("[流光系统]你输入了一个空密码!");
            return true;
        }
        if (password.Length > Plugin.Config.LimitPasswordLength)
        {
            player.Disconnect($"[流光系统]密码长度不得超过{Plugin.Config.LimitPasswordLength}");
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
                NetMessage.SendData(7, player.Index, -1, NetworkText.Empty, 0, 0f, 0f, 0f, 0, 0, 0);
                var groupByName = TShock.Groups.GetGroupByName(userAccountByName.Group);
                player.Group = groupByName;
                player.tempGroup = null;
                player.Account = userAccountByName;
                player.IsLoggedIn = true;
                player.IsDisabledForSSC = false;
                if (Terraria.Main.ServerSideCharacter)
                {
                    if (player.HasPermission(Permissions.bypassssc))
                    {
                        player.PlayerData.CopyCharacter(player);
                        TShock.CharacterDB.InsertPlayerData(player, false);
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
            Kick(player, Plugin.Config.VerficationFailedMessage, "验证失败");
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
            player.SendSuccessMessage("账户{0}注册成功。", new object[1] { userAccountByName.Name });
            player.SendSuccessMessage("你的密码是{0}", new object[1] { password });
            TShock.UserAccounts.AddUserAccount(userAccountByName);
            TShock.Log.ConsoleInfo("玩家{0}注册了新账户：{1}", new object[2] { player.Name, userAccountByName.Name });
            player.RequiresPassword = false;
            player.SetData("reg-pwd", false);
            player.PlayerData = TShock.CharacterDB.GetPlayerData(player, userAccountByName.ID);
            if (player.State == 1)
            {
                player.State = 2;
            }
            NetMessage.SendData(7, player.Index, -1, NetworkText.Empty, 0, 0f, 0f, 0f, 0, 0, 0);
            var groupByName2 = TShock.Groups.GetGroupByName(userAccountByName.Group);
            player.Group = groupByName2;
            player.tempGroup = null;
            player.Account = userAccountByName;
            player.IsLoggedIn = true;
            player.IsDisabledForSSC = false;
            if (Terraria.Main.ServerSideCharacter)
            {
                if (player.HasPermission(Permissions.bypassssc))
                {
                    player.PlayerData.CopyCharacter(player);
                    TShock.CharacterDB.InsertPlayerData(player, false);
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
}
