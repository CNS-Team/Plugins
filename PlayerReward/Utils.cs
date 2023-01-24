using LazyUtils;
using System.Linq.Expressions;
using CustomPlayer;
using LinqToDB;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace PlayerReward;

internal static class Utils
{
    // 因应需求取消继承（懒了）
    public static IEnumerable<string> GetAllInheritedGroupNames(this Group group)
    {
        yield return group.Name;
        //while (group != null)
        //{
        //    yield return group.Name;
        //    group = group.Parent;
        //}
    }

    public static (UserAccount?, Group?) GetAccountAndGroupByPlayerName(string playerName)
    {
        // 本来想着从当前玩家列表取的，谁知道有插件会改 player group 啊 (恼
        //var player = TShock.Players.FirstOrDefault(x => x?.Account?.Name == playerName);
        //if (player != null)
        //    return (player.Account, player.Group);

        var account = TShock.UserAccounts.GetUserAccountByName(playerName);
        if (account == null)
            return (null, null);

        var group = TShock.Groups.GetGroupByName(account.Group);
        return (account, group);
    }

    public static PlayerPack[]? GetAvailablePlayerPacks(this TSPlayer player) =>
        GetAvailablePlayerPacks(player.Account.Name);

    public static PlayerPack[]? GetAvailableSponsorPacks(this TSPlayer player) =>
        GetAvailableSponsorPacks(player.Account.Name);

    public static PlayerPack[]? GetAvailablePlayerPacks(string playerName)
    {
        var (account, group) = GetAccountAndGroupByPlayerName(playerName);
        if (account == null || group == null)
            return null;

        using var query = Db.Get<PlayerRewardInfo>(account.Name);
        var obtainedPlayerPackNames = query.Select(x => x.ObtainedPlayerPacks).Single().Split(',');
        var playerGroupNames = group.GetAllInheritedGroupNames().ToArray();
        return Config.Instance.PlayerPacks
            .Where(pack => (!pack.IsHardMode || Main.hardMode) && // pack.IsHardMode ? Main.hardMode : true
                           !obtainedPlayerPackNames.Contains(pack.Name) &&
                           playerGroupNames.Any(name => pack.Groups.Contains(name)))
            .ToArray();
    }

    public static PlayerPack[]? GetAvailableSponsorPacks(string playerName)
    {
        var (account, group) = GetAccountAndGroupByPlayerName(playerName);
        if (account == null || group == null)
            return null;

        using var query = Db.Get<PlayerRewardInfo>(account.Name);
        var obtainedSponsorPackNames = query.Select(x => x.ObtainedSponsorPacks).Single().Split(',');
        var customPlayerGroupNames = CustomPlayerAdapter.GetCustomPlayer(account.Name)
            .GetCustomGroupNames()
            .ToArray();

        return Config.Instance.SponsorPacks
            .Where(pack => (!pack.IsHardMode || Main.hardMode) && // pack.IsHardMode ? Main.hardMode : true
                            !obtainedSponsorPackNames.Contains(pack.Name) &&
                           customPlayerGroupNames.Any(name => pack.Groups.Contains(name)))
            .ToArray();
    }

    public static void Give(this TSPlayer player, PlayerPack pack)
    {
        var items = pack.Items.Select(Item.Parse);
        items.ForEach(x => player.GiveItem(x.ID, x.Stack, x.Prefix));
        pack.ExecuteCommands.ForEach(x =>
        {
            Commands.HandleCommand(
                TSPlayer.Server,
                x.Replace("{name}", player.Name).Replace("{account}", player.Account?.Name));
        });
    }
}

internal static class CustomPlayerAdapter
{
    public static CustomPlayer.CustomPlayer GetCustomPlayer(string playerName) =>
        CustomPlayer.Utils.FindPlayer(playerName) ??
        CustomPlayer.CustomPlayer.Read(playerName, new FakeTSPlayer(playerName));

    // 因应需求取消继承（懒了X2）
    public static IEnumerable<string> GetCustomGroupNames(this CustomPlayer.CustomPlayer cp) =>
        cp.HaveGroupNames;
        //cp.HaveGroupNames
        //    .SelectMany(x => CustomPlayerPluginHelpers.Groups.GetGroupByName(x).GetAllInheritedGroupNames())
        //    .Distinct();

    private class FakeTSPlayer : TSPlayer
    {
        public FakeTSPlayer(string playerName) : base(playerName) { }

        public override void SendMessage(string msg, Color color) { }
        public override void SendMessage(string msg, byte red, byte green, byte blue) { }
        public override void SendInfoMessage(string msg) { }
        public override void SendSuccessMessage(string msg) { }
        public override void SendWarningMessage(string msg) { }
        public override void SendErrorMessage(string msg) { }

    }
}

internal static class DbStringArrayEmulator
{
    public static void AddElement<T>(string playerName, Expression<Func<T, string>> selector, string element) where T : PlayerConfigBase<T>
    {
        using var query = Db.Get<T>(playerName);
        var elements = query.Select(selector)
            .Single()
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Append(element);
        query.Set(selector, string.Join(',', elements))
            .Update();
    }
    public static void RemoveElement<T>(string playerName, Expression<Func<T, string>> selector, string element) where T : PlayerConfigBase<T>
    {
        using var query = Db.Get<T>(playerName);
        var elements = query.Select(selector)
            .Single()
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToList()
            .Remove(element);
        query.Set(selector, string.Join(',', elements))
            .Update();
    }

    public static void ClearElements<T>(string playerName, Expression<Func<T, string>> selector) where T : PlayerConfigBase<T>
    {
        using var query = Db.Get<T>(playerName);
        query.Set(selector, "").Update();
    }
}