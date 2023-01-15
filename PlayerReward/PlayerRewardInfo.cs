using System.Linq.Expressions;
using LazyUtils;
using LinqToDB;
using LinqToDB.Mapping;

namespace PlayerReward;

[Table(Name = "PlayerReward")]
internal class PlayerRewardInfo : PlayerConfigBase<PlayerRewardInfo>
{
    [Column(Name = "obtained_group_packs", DataType = DataType.Text)]
    public string ObtainedGroupPacks { get; set; } = "";

    [Column(Name = "available_sponsor_packs", DataType = DataType.Text)]
    public string AvailableSponsorPacks { get; set; } = "";

    public static void AddObtainedPack(string playerName, string packName) =>
        EmulateStringArrayAdd(playerName, x => x.ObtainedGroupPacks, packName);

    public static void RemoveObtainedPack(string playerName, string packName) =>
        EmulateStringArrayRemove(playerName, x => x.ObtainedGroupPacks, packName);

    public static void ClearObtainedPack(string playerName) =>
        EmulateStringArrayClear(playerName, x => x.ObtainedGroupPacks);

    public static void AddSponsorPack(string playerName, string packName) =>
        EmulateStringArrayAdd(playerName, x => x.AvailableSponsorPacks, packName);

    public static void RemoveSponsorPack(string playerName, string packName) =>
        EmulateStringArrayRemove(playerName, x => x.AvailableSponsorPacks, packName);

    public static void ClearSponsorPack(string playerName) =>
        EmulateStringArrayClear(playerName, x => x.AvailableSponsorPacks);

    #region String Array Emulator

    private static void EmulateStringArrayAdd(string playerName, Expression<Func<PlayerRewardInfo, string>> selector, string element)
    {
        IEnumerable<string> stringArray;
        using (var query = Db.Get<PlayerRewardInfo>(playerName))
        {
            stringArray = query.Select(selector).Single().Split(',');
        }

        stringArray = stringArray.Where(x => x != "").Append(element);

        using (var query = Db.Get<PlayerRewardInfo>(playerName))
        {
            query.Set(selector, string.Join(',', stringArray)).Update();
        }
    }

    private static void EmulateStringArrayRemove(string playerName, Expression<Func<PlayerRewardInfo, string>> selector, string element)
    {
        List<string> stringArray;
        using (var query = Db.Get<PlayerRewardInfo>(playerName))
        {
            stringArray = new List<string>(query.Select(selector).Single().Split(','));
        }

        stringArray.Remove(element);

        using (var query = Db.Get<PlayerRewardInfo>(playerName))
        {
            query.Set(selector, string.Join(',', stringArray)).Update();
        }
    }

    private static void EmulateStringArrayClear(string playerName, Expression<Func<PlayerRewardInfo, string>> selector)
    {
        using var query = Db.Get<PlayerRewardInfo>(playerName);
        query.Set(selector, "").Update();
    }

    #endregion

}