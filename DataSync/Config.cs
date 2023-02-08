using Newtonsoft.Json;
using System.Reflection;
using TShockAPI;

namespace DataSync;

[JsonConverter(typeof(Config.ProgressConverter))]
public enum ProgressType
{
    [Match(nameof(Terraria.NPC.downedSlimeKing), true)]
    [Alias("史莱姆王")]
    KingSlime,

    [Match(nameof(Terraria.NPC.downedBoss1), true)]
    [Alias("克苏鲁之眼")]
    EyeOfCthulhu,

    [Match(nameof(Terraria.NPC.downedBoss2), true)]
    [Alias("世界吞噬者", "克苏鲁之脑")]
    EaterOfWorlds,

    [Match(nameof(Terraria.NPC.downedQueenBee), true)]
    [Alias("蜂后")]
    QueenBee,

    [Match(nameof(Terraria.NPC.downedBoss3), true)]
    [Alias("骷髅王")]
    Skeletron,

    [Match(nameof(Terraria.Main.hardMode), true)]
    [Alias("肉山", "血肉之墙")]
    WallOfFlesh,

    [Match(nameof(Terraria.NPC.downedMechBoss1), true)]
    [Alias("机械眼", "双子魔眼")]
    TheTwins,

    [Match(nameof(Terraria.NPC.downedMechBoss2), true)]
    [Alias("机械毁灭者")]
    TheDestroyer,

    [Match(nameof(Terraria.NPC.downedMechBoss3), true)]
    [Alias("机械骷髅", "机械骷髅王")]
    SkeletronPrime,

    [Match(nameof(Terraria.NPC.downedPlantBoss), true)]
    [Alias("世纪之花")]
    Plantera,

    [Match(nameof(Terraria.NPC.downedGolemBoss), true)]
    [Alias("石巨人")]
    Golem,

    [Match(nameof(Terraria.NPC.downedFishron), true)]
    [Alias("猪鲨公爵")]
    DukeFishron,

    [Match(nameof(Terraria.NPC.downedAncientCultist), true)]
    [Alias("拜月教徒")]
    LunaticCultist,

    [Match(nameof(Terraria.NPC.downedMoonlord), true)]
    [Alias("月球领主")]
    MoonLord,

    [Match(nameof(Terraria.NPC.downedTowerSolar), true)]
    [Alias("日耀塔")]
    SolarPillar,

    [Match(nameof(Terraria.NPC.downedTowerVortex), true)]
    [Alias("星旋塔")]
    VortexPillar,

    [Match(nameof(Terraria.NPC.downedTowerNebula), true)]
    [Alias("星云塔")]
    NebulaPillar,

    [Match(nameof(Terraria.NPC.downedTowerStardust), true)]
    [Alias("星尘塔")]
    StardustPillar,

    [Match(nameof(Terraria.NPC.downedChristmasIceQueen), true)]
    [Alias("冰雪女王")]
    ChristmasIceQueen,

    [Match(nameof(Terraria.NPC.downedChristmasSantank), true)]
    [Alias("圣诞坦克")]
    ChristmasSantank,

    [Match(nameof(Terraria.NPC.downedChristmasTree), true)]
    [Alias("圣诞树")]
    ChristmasTree,

    [Match(nameof(Terraria.NPC.downedHalloweenTree), true)]
    [Alias("万圣树")]
    HalloweenTree,

    [Match(nameof(Terraria.NPC.downedQueenSlime), true)]
    [Alias("史莱姆女王")]
    QueenSlime,

    [Match(nameof(Terraria.NPC.downedDeerclops), true)]
    [Alias("鹿角怪")]
    Deerclops,

    [Match(nameof(Terraria.NPC.downedEmpressOfLight), true)]
    [Alias("光之女皇")]
    EmpressOfLight,

    #region Invasion
    [Match(nameof(Terraria.Main.bloodMoon), true)]
    [Alias("血月")]
    BloodMoon,

    [Match(nameof(Terraria.Main.eclipse), true)]
    [Alias("日蚀")]
    SolarEclipse,

    [Match(nameof(Terraria.Main.pumpkinMoon), true)]
    [Alias("南瓜月")]
    PumpkinMoon,

    [Match(nameof(Terraria.Main.snowMoon), true)]
    [Alias("霜月", "雪月")]
    FrostMoon,

    [Match(nameof(Terraria.Main.invasionType), 4)]
    [Alias("火星暴乱")]
    MartianMadness,

    [Match(nameof(Terraria.GameContent.Events.DD2Event.Ongoing), true)]
    [Alias("旧日军团")]
    OldOnesArmy,

    [Match(nameof(Terraria.Main.invasionType), 1)]
    [Alias("哥布林军团")]
    GoblinsArmy,

    [Match(nameof(Terraria.Main.invasionType), 3)]
    [Alias("海盗军团")]
    PiratesArmy,

    [Match(nameof(Terraria.Main.invasionType), 2)]
    [Alias("雪人军团")]
    FrostLegion,

    [Match(nameof(Terraria.GameContent.Events.DD2Event._downedDarkMageT1), true)]
    [Alias("暗黑法师")]
    DD2Mage,

    [Match(nameof(Terraria.GameContent.Events.DD2Event._downedOgreT2), true)]
    [Alias("巨魔")]
    DD2Orge,

    [Match(nameof(Terraria.GameContent.Events.DD2Event._spawnedBetsyT3), true)]
    [Alias("贝蒂斯")]
    DD2Betsy,
    #endregion
}

public static class Config
{
    internal static readonly Dictionary<ProgressType, string> _default = typeof(ProgressType)
        .GetFields()
        .ToDictionary(f => (ProgressType) f.GetValue(null)!, f => f.GetCustomAttribute<AliasAttribute>()!.Alias[0]);
    internal static readonly Dictionary<string, ProgressType> _names = typeof(ProgressType)
        .GetFields()
        .SelectMany(field => field.GetCustomAttribute<AliasAttribute>()!.Alias.Select(a => (field, a)))
        .ToDictionary(t => t.a, t => (ProgressType) t.field.GetValue(null)!);

    public static string GetProgressName(ProgressType type)
    {
        return _default[type];
    }

    public static ProgressType? GetProgressType(string? name)
    {
        if (name is null) return null;
        return _names.TryGetValue(name, out var type) ? type : null;
    }

    public static Dictionary<ProgressType, bool> ShouldSyncProgress { get; set; } = new Dictionary<ProgressType, bool>();

    public static void LoadConfig()
    {
        var PATH = Path.Combine(TShock.SavePath, "DataSync.json");
        try
        {
            if (!File.Exists(PATH))
            {
                FileTools.CreateIfNot(PATH, JsonConvert.SerializeObject(ShouldSyncProgress, Formatting.Indented));
            }
            ShouldSyncProgress = JsonConvert.DeserializeObject<Dictionary<ProgressType, bool>>(File.ReadAllText(PATH))!;
            File.WriteAllText(PATH, JsonConvert.SerializeObject(ShouldSyncProgress, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            TSPlayer.Server.SendErrorMessage("[DataSync]配置文件读取错误！！！");
        }
    }

    public class ProgressConverter : JsonConverter<ProgressType>
    {
        public override ProgressType ReadJson(JsonReader reader, Type objectType, ProgressType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = $"{reader.Value}";
            if (GetProgressType(value) is ProgressType type)
                return type;
            throw new JsonSerializationException($"无法识别的进度类型：{value}");
        }

        public override void WriteJson(JsonWriter writer, ProgressType value, JsonSerializer serializer)
        {
            writer.WriteValue(GetProgressName(value));
        }
    }
}
