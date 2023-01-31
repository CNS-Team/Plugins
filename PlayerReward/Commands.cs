using LazyUtils;
using LazyUtils.Commands;
using System.Text;
using TShockAPI;
using LinqToDB;

namespace PlayerReward;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

[Command("pr")]
public static class PlayerRewardCommand
{
    #region User Commands

    [RealPlayer]
    [Permission("playerreward.user")]
    public static void Get(CommandArgs args)
    {
        var availablePacks = args.Player.GetAvailablePlayerPacks();
        if (availablePacks == null || availablePacks.Length == 0)
        {
            args.Player.SendWarningMessage("当前无可获取的玩家礼包");
            return;
        }
        var selectedPack = availablePacks[0];
        if (CommandHelper.ValidateInventorySlotsAvailableFailed(args, selectedPack.Items.Count))
            return;
        PlayerRewardInfo.AddObtainedPlayerPacks(args.Player.Account.Name, selectedPack.Name);
        args.Player.Give(selectedPack);
        args.Player.SendSuccessMessage($"成功获取 {selectedPack.Name} 礼包");
    }

    [RealPlayer]
    [Permission("playerreward.user")]
    public static void Get(CommandArgs args, int id)
    {
        if (id < 0)
        {
            args.Player.SendWarningMessage("ID不得小于0");
            return;
        }

        var availablePacks = args.Player.GetAvailablePlayerPacks();
        if (availablePacks == null || availablePacks.Length == 0)
        {
            args.Player.SendWarningMessage("当前无可获取的玩家礼包");
            return;
        }
        if (id >= availablePacks.Length)
        {
            args.Player.SendWarningMessage($"ID超出范围, 最大ID值为 {availablePacks.Length - 1}");
            return;
        }

        var selectedPack = availablePacks[id];
        if (CommandHelper.ValidateInventorySlotsAvailableFailed(args, selectedPack.Items.Count))
            return;
        PlayerRewardInfo.AddObtainedPlayerPacks(args.Player.Account.Name, selectedPack.Name);
        args.Player.Give(selectedPack);
        args.Player.SendSuccessMessage($"成功获取 {selectedPack.Name} 礼包");
    }

    [RealPlayer]
    [Permissions("playerreward.user")]
    public static void List(CommandArgs args)
    {
        var availablePacks = args.Player.GetAvailablePlayerPacks();
        if (availablePacks == null || availablePacks.Length == 0)
        {
            args.Player.SendWarningMessage("当前无可获取的玩家礼包");
            return;
        }

        var sb = new StringBuilder();
        var i = 0;
        sb.Append("当前可获得的玩家礼包为：\n");
        availablePacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
        args.Player.SendInfoMessage(sb.ToString());
    }

    #endregion

    #region Admin Commands

    public static class Admin
    {
        [Permission("playerreward.admin")]
        public static void List(CommandArgs args, string playerName)
        {
            var availablePacks = Utils.GetAvailablePlayerPacks(playerName);
            if (availablePacks == null)
            {
                args.Player.SendWarningMessage($"未找到名为 {playerName} 的玩家");
                return;
            }

            var sb = new StringBuilder();
            var i = 0;
            sb.Append($"玩家 {playerName} 当前可获得的玩家礼包为：\n");
            availablePacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
            args.Player.SendInfoMessage(sb.ToString());
        }

        public static class Reset
        {
            [Permission("playerreward.admin")]
            public static void Player(CommandArgs args, string playerName)
            {
                if (CommandHelper.ValidatePlayerAccountFailed(args, playerName))
                    return;

                PlayerRewardInfo.ClearObtainedPlayerPacks(playerName);
                args.Player.SendSuccessMessage($"成功重置玩家 {playerName} 的玩家礼包");
            }

            [Permission("playerreward.admin")]
            public static void All(CommandArgs args)
            {
                using var context = new PlayerRewardInfo.Context(null!);
                context.Config
                    .Set(x => x.ObtainedPlayerPacks, "")
                    .Update();
                args.Player.SendSuccessMessage("成功重置所有玩家的玩家礼包");
            }
        }
    }

    #endregion
}

[Command("sr")]
public static class SponsorRewardCommand
{
    #region User Commands

    [RealPlayer]
    [Permission("playerreward.user")]
    public static void Get(CommandArgs args)
    {
        var availablePacks = args.Player.GetAvailableSponsorPacks();
        if (availablePacks == null || availablePacks.Length == 0)
        {
            args.Player.SendWarningMessage("当前无可获取的赞助礼包");
            return;
        }
        var selectedPack = availablePacks[0];
        if (CommandHelper.ValidateInventorySlotsAvailableFailed(args, selectedPack.Items.Count))
            return;
        PlayerRewardInfo.AddObtainedSponsorPacks(args.Player.Account.Name, selectedPack.Name);
        args.Player.Give(selectedPack);
        args.Player.SendSuccessMessage($"成功获取 {selectedPack.Name} 赞助礼包");
    }

    [RealPlayer]
    [Permission("playerreward.user")]
    public static void Get(CommandArgs args, int id)
    {
        if (id < 0)
        {
            args.Player.SendWarningMessage("ID不得小于0");
            return;
        }

        var availablePacks = args.Player.GetAvailableSponsorPacks();
        if (availablePacks == null || availablePacks.Length == 0)
        {
            args.Player.SendWarningMessage("当前无可获取的赞助礼包");
            return;
        }
        if (id >= availablePacks.Length)
        {
            args.Player.SendWarningMessage($"ID超出范围, 最大ID值为 {availablePacks.Length - 1}");
            return;
        }

        var selectedPack = availablePacks[id];
        if (CommandHelper.ValidateInventorySlotsAvailableFailed(args, selectedPack.Items.Count))
            return;
        PlayerRewardInfo.AddObtainedSponsorPacks(args.Player.Account.Name, selectedPack.Name);
        args.Player.Give(selectedPack);
        args.Player.SendSuccessMessage($"成功获取 {selectedPack.Name} 赞助礼包");
    }

    [RealPlayer]
    [Permissions("playerreward.user")]
    public static void List(CommandArgs args)
    {
        var availablePacks = args.Player.GetAvailableSponsorPacks();
        var sb = new StringBuilder();
        if (availablePacks == null || availablePacks.Length == 0)
        {
            args.Player.SendWarningMessage("当前无可获取的赞助礼包");
            return;
        }

        var i = 0;
        sb.Append("当前可获得的赞助礼包为：\n");
        availablePacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
        args.Player.SendInfoMessage(sb.ToString());
    }

    #endregion

    #region Admin Commands

    public static class Admin
    {
        [Permission("playerreward.admin")]
        public static void List(CommandArgs args, string playerName)
        {
            var availablePacks = Utils.GetAvailableSponsorPacks(playerName);
            if (availablePacks == null)
            {
                args.Player.SendWarningMessage($"未找到名为 {playerName} 的玩家");
                return;
            }

            var sb = new StringBuilder();
            var i = 0;
            sb.Append($"玩家 {playerName} 当前可获得的赞助礼包为：\n");
            availablePacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
            args.Player.SendInfoMessage(sb.ToString());
        }

        public static class Reset
        {
            [Permission("playerreward.admin")]
            public static void Player(CommandArgs args, string playerName)
            {
                if (CommandHelper.ValidatePlayerAccountFailed(args, playerName))
                    return;

                PlayerRewardInfo.ClearObtainedSponsorPacks(playerName);
                args.Player.SendSuccessMessage($"成功重置玩家 {playerName} 的赞助礼包");
            }

            [Permission("playerreward.admin")]
            public static void All(CommandArgs args)
            {
                using var context = new PlayerRewardInfo.Context(null!);
                context.Config
                    .Set(x => x.ObtainedSponsorPacks, "")
                    .Update();
                args.Player.SendSuccessMessage("成功重置所有玩家的赞助礼包");
            }
        }
    }

    #endregion
}

internal static class CommandHelper
{
    public static bool ValidateInventorySlotsAvailableFailed(CommandArgs args, int slotsRequired)
    {
        var slotsLeft = args.TPlayer.inventory.Take(50).Count(x => x.type == 0);
        if (slotsRequired <= slotsLeft)
            return false;
        args.Player.SendWarningMessage($"背包空间不足，还需要 {slotsRequired - slotsLeft} 个空位，请清理后再试");
        return true;
    }

    public static bool ValidatePlayerAccountFailed(CommandArgs args, string playerName)
    {
        if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
            return false;
        args.Player.SendInfoMessage($"未找到名为 {playerName} 的玩家");
        return true;
    }
}