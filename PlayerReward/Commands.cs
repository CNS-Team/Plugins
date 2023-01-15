using System.Text;
using LazyUtils;
using LazyUtils.Commands;
using LinqToDB;
using TShockAPI;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace PlayerReward;

[Command("pr")]
public static class PlayerRewardCommand
{
    [RealPlayer]
    [Permission("playerreward.user")]
    public static void Get(CommandArgs args)
    {
        var (availableGroupPacks, availableSponsorPacks) = args.Player.GetAvailablePacks();

        switch (availableGroupPacks.Length + availableSponsorPacks.Length)
        {
            case 0:
                args.Player.SendWarningMessage("当前无礼包可获取");
                return;

            case > 1:
                args.Player.SendWarningMessage("你有多于一个礼包等待领取，请指定礼包ID");
                return;

            default:
                IPack selectedPack =
                    availableGroupPacks.Length == 1 ? availableGroupPacks[0] : availableSponsorPacks[0];
                if (ValidateInventorySlotsAvailableFailed(args, selectedPack.Items.Count))
                    return;
                args.Player.GivePack(selectedPack);
                args.Player.SendSuccessMessage($"成功获取 {selectedPack.Name} 礼包");
                break;
        }
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

        var (groupPacks, sponsorPacks) = args.Player.GetAvailablePacks();
        if (groupPacks.Length + sponsorPacks.Length == 0)
        {
            args.Player.SendWarningMessage("当前没有可获取的礼包");
            return;
        }
        if (id >= groupPacks.Length + sponsorPacks.Length)
        {
            args.Player.SendWarningMessage("ID超出范围");
            return;
        }

        IPack selectedPack = id < groupPacks.Length ? groupPacks[id] : sponsorPacks[id - groupPacks.Length];
        if (ValidateInventorySlotsAvailableFailed(args, selectedPack.Items.Count))
            return;
        args.Player.GivePack(selectedPack);
        args.Player.SendSuccessMessage($"成功获取 {selectedPack.Name} 礼包");
    }

    [RealPlayer]
    [Permissions("playerreward.user")]
    public static void List(CommandArgs args)
    {
        var (availableGroupPacks, availableSponsorPacks) = args.Player.GetAvailablePacks();

        var i = 0;
        var sb = new StringBuilder();
        sb.Append("当前可获得的组别奖励为：\n");
        availableGroupPacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
        sb.Append("当前可获得的赞助奖励为：\n");
        availableSponsorPacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
        args.Player.SendInfoMessage(sb.ToString());
    }

    private static bool ValidateInventorySlotsAvailableFailed(CommandArgs args, int slotsRequired)
    {
        var slotsLeft = args.TPlayer.inventory.Take(50).Count(x => x.type == 0);
        if (slotsRequired <= slotsLeft)
            return false;
        args.Player.SendWarningMessage($"背包空间不足，还需要 {slotsRequired - slotsLeft} 个空位，请清理后再试");
        return true;
    }
}

[Command("pra")]
public static class PlayerRewardAdminCommand
{
    [Permission("playerreward.admin")]
    public static void List(CommandArgs args, string playerName)
    {
        var tuple = Utils.GetAvailablePacksByPlayerName(playerName);
        if (tuple == null)
        {
            args.Player.SendWarningMessage($"未找到名为 {playerName} 的玩家");
            return;
        }

        var (availableGroupPacks, availableSponsorPacks) = tuple.Value;

        var i = 0;
        var sb = new StringBuilder();
        sb.Append($"玩家{playerName}当前可获得的组别奖励为：\n");
        availableGroupPacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
        sb.Append($"玩家{playerName}当前可获得的赞助奖励为：\n");
        availableSponsorPacks.ForEach(x => sb.Append($"  {i++}: {x.Name}\n"));
        args.Player.SendInfoMessage(sb.ToString());
    }

    [Permission("playerreward.admin")]
    public static void Give(CommandArgs args, string playerName, string packName)
    {
        if (ValidatePlayerAccountFailed(args, playerName)) return;
        if (ValidateSponsorPackNameFailed(args, packName)) return;

        PlayerRewardInfo.AddSponsorPack(playerName, packName);
        args.Player.SendSuccessMessage($"成功为 {playerName} 玩家添加 {packName} 赞助礼包");
    }

    [Permission("playerreward.admin")]
    public static void Remove(CommandArgs args, string playerName, string packName)
    {
        if (ValidatePlayerAccountFailed(args, playerName)) return;
        if (ValidateSponsorPackNameFailed(args, packName)) return;

        PlayerRewardInfo.RemoveSponsorPack(playerName, packName);
        args.Player.SendSuccessMessage($"成功为 {playerName} 玩家移除 {packName} 赞助礼包");
    }

    public static class Reset
    {
        public static class Player
        {
            [Permission("playerreward.admin")]
            public static void Group(CommandArgs args, string playerName)
            {
                if (ValidatePlayerAccountFailed(args, playerName)) return;

                PlayerRewardInfo.ClearObtainedPack(playerName);
            }

            [Permission("playerreward.admin")]
            public static void Sponsor(CommandArgs args, string playerName)
            {
                if (ValidatePlayerAccountFailed(args, playerName)) return;

                PlayerRewardInfo.ClearSponsorPack(playerName);
            }

            [Permission("playerreward.admin")]
            public static void All(CommandArgs args, string playerName)
            {
                if (ValidatePlayerAccountFailed(args, playerName)) return;

                PlayerRewardInfo.ClearSponsorPack(playerName);
                PlayerRewardInfo.ClearSponsorPack(playerName);
            }
        }

        public static class AllPlayer
        {
            [Permission("playerreward.admin")]
            public static void Group(CommandArgs args)
            {
                using var context = new PlayerRewardInfo.Context(null);
                context.Config
                    .Set(x => x.ObtainedGroupPacks, "")
                    .Update();
            }

            [Permission("playerreward.admin")]
            public static void Sponsor(CommandArgs args)
            {
                using var context = new PlayerRewardInfo.Context(null);
                context.Config
                    .Set(x => x.AvailableSponsorPacks, "")
                    .Update();
            }

            [Permission("playerreward.admin")]
            public static void All(CommandArgs args)
            {
                using var context = new PlayerRewardInfo.Context(null);
                context.Config
                    .Set(x => x.ObtainedGroupPacks, "")
                    .Set(x => x.AvailableSponsorPacks, "")
                    .Update();
            }
        }
    }

    private static bool ValidatePlayerAccountFailed(CommandArgs args, string playerName)
    {
        if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
            return false;
        args.Player.SendInfoMessage($"未找到名为 {playerName} 的玩家");
        return true;
    }

    private static bool ValidateSponsorPackNameFailed(CommandArgs args, string packName)
    {
        if (Config.Instance.SponsorPacks.Any(x => x.Name == packName))
            return false;
        args.Player.SendInfoMessage($"未找到名为 {packName} 的赞助礼包");
        return true;

    }
}