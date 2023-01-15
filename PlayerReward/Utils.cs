using LazyUtils;
using TShockAPI;

namespace PlayerReward;

internal static class Utils
{
    public static bool IsInGroup(this TSPlayer player, string groupName)
    {
        var g = player.Group;
        while (g != null)
        {
            if (g.Name == groupName)
                return true;
            g = g.Parent;
        }
        return false;
    }

    public static IEnumerable<string> GetAllInheritedGroupNames(this Group group)
    {
        while (group != null)
        {
            yield return group.Name;
            group = group.Parent;
        }
    }

    public static (GroupPack[], SponsorPack[]) GetAvailablePacks(this TSPlayer player)
    {
        string[] obtainedPackNames;
        string[] availableSponsorPackNames;
        using (var record = player.Get<PlayerRewardInfo>())
        {
            var info = record.Single();
            obtainedPackNames = info.ObtainedGroupPacks.Split(',');
            availableSponsorPackNames = info.AvailableSponsorPacks.Split(',');
        }

        var playerGroupNames = player.Group.GetAllInheritedGroupNames().ToArray();
        var availableGroupPacks = Config.Instance.GroupPacks
            .Where(pack => !obtainedPackNames.Contains(pack.Name) &&
                           playerGroupNames.Any(name => pack.Groups.Contains(name)))
            .ToArray();

        var availableSponsorPacks = availableSponsorPackNames
            .Select(name => Config.Instance.SponsorPacks.Find(pack => pack.Name == name))
            .Where(x => x != null)
            .ToArray();

        return (availableGroupPacks, availableSponsorPacks)!;
    }

    public static (GroupPack[], SponsorPack[])? GetAvailablePacksByPlayerName(string playerName)
    {
        var account = TShock.UserAccounts.GetUserAccountByName(playerName);
        if (account == null)
            return null;

        var group = TShock.Groups.GetGroupByName(account.Group);
        if (group == null)
            return null;

        string[] obtainedPackNames;
        string[] availableSponsorPackNames;
        using (var record = Db.Get<PlayerRewardInfo>(account.Name))
        {
            var info = record.Single();
            obtainedPackNames = info.ObtainedGroupPacks.Split(',');
            availableSponsorPackNames = info.AvailableSponsorPacks.Split(',');
        }

        var playerGroupNames = group.GetAllInheritedGroupNames().ToArray();
        var availableGroupPacks = Config.Instance.GroupPacks
            .Where(pack => !obtainedPackNames.Contains(pack.Name) &&
                           playerGroupNames.Any(name => pack.Groups.Contains(name)))
            .ToArray();

        var availableSponsorPacks = availableSponsorPackNames
            .Select(name => Config.Instance.SponsorPacks.Find(pack => pack.Name == name))
            .Where(x => x != null)
            .ToArray();

        return (availableGroupPacks, availableSponsorPacks)!;
    }

    public static void GivePack(this TSPlayer player, IPack pack)
    {
        switch (pack)
        {
            case GroupPack:
                PlayerRewardInfo.AddObtainedPack(player.Account.Name, pack.Name);
                break;

            case SponsorPack:
                PlayerRewardInfo.RemoveSponsorPack(player.Account.Name, pack.Name);
                break;
        }

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