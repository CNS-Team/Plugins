using Newtonsoft.Json;

namespace Replenisher;

public class Config
{
    public bool GenerateInProtectedAreas;

    public bool AutomaticallRefill;

    public int AutoRefillTimerInMinutes = 30;

    public bool ReplenOres;

    public List<string> OreToReplen = new List<string> { "Copper", "Iron" };

    public int OreAmount;

    public bool ReplenChests;

    public int ChestAmount;

    public bool ReplenPots;

    public int PotsAmount;

    public bool ReplenLifeCrystals;

    public int LifeCrystalAmount;

    public bool ReplenTrees;

    public int TreesAmount;

    [JsonConstructor]
    public Config()
    {
        this.OreToReplen = new List<string>();
    }
}