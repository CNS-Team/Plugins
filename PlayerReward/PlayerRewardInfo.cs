using LazyUtils;
using LinqToDB;
using LinqToDB.Mapping;

namespace PlayerReward;

[Table(Name = "PlayerReward")]
internal class PlayerRewardInfo : PlayerConfigBase<PlayerRewardInfo>
{
    [Column(Name = "obtained_player_packs", DataType = DataType.Text)]
    public string ObtainedPlayerPacks { get; set; } = "";

    [Column(Name = "obtained_sponsor_packs", DataType = DataType.Text)]
    public string ObtainedSponsorPacks { get; set; } = "";

    #region ObtainedPlayerPacks Helper

    public static void AddObtainedPlayerPacks(string playerName, string packName)
    {
        DbStringArrayEmulator.AddElement<PlayerRewardInfo>(playerName, x => x.ObtainedPlayerPacks, packName);
    }

    public static void RemoveObtainedPlayerPacks(string playerName, string packName)
    {
        DbStringArrayEmulator.RemoveElement<PlayerRewardInfo>(playerName, x => x.ObtainedPlayerPacks, packName);
    }

    public static void ClearObtainedPlayerPacks(string playerName)
    {
        DbStringArrayEmulator.ClearElements<PlayerRewardInfo>(playerName, x => x.ObtainedPlayerPacks);
    }

    #endregion

    #region ObtainedSponsorPacks Helper

    public static void AddObtainedSponsorPacks(string playerName, string packName)
    {
        DbStringArrayEmulator.AddElement<PlayerRewardInfo>(playerName, x => x.ObtainedSponsorPacks, packName);
    }

    public static void RemoveObtainedSponsorPacks(string playerName, string packName)
    {
        DbStringArrayEmulator.RemoveElement<PlayerRewardInfo>(playerName, x => x.ObtainedSponsorPacks, packName);
    }

    public static void ClearObtainedSponsorPacks(string playerName)
    {
        DbStringArrayEmulator.ClearElements<PlayerRewardInfo>(playerName, x => x.ObtainedSponsorPacks);
    }

    #endregion

}