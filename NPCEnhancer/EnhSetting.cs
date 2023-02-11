using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace NPCEnhancer;

public class EnhSetting
{
    private static List<int>[]? npcIDsofBanner;

    public Func<NPC, bool>? Condition { get; set; }
    public List<int> TargetIDs { get; }
    /// <summary>
    /// 防御增加值
    /// </summary>
    public int DefAdd { get; set; }
    /// <summary>
    /// 防御增加比例
    /// </summary>
    public double DefMult { get; set; }
    /// <summary>
    /// 血量增加值
    /// </summary>
    public int LifeAdd { get; set; }
    /// <summary>
    /// 血量增加比例
    /// </summary>
    public double LifeMult { get; set; }
    public EnhSetting(int defAdd = 0, double defMult = 0, int lifeAdd = 0, double lifeMult = 0, Func<NPC, bool>? condition = null, params int[] ids)
    {
        this.TargetIDs = new List<int>();
        this.Condition = condition;
        this.DefAdd = defAdd;
        this.DefMult = defMult;
        this.LifeAdd = lifeAdd;
        this.LifeMult = lifeMult;

        this.TargetIDs.AddRange(ids);
    }
    public void Add(params int[] ids)
    {
        this.Add((IEnumerable<int>) ids);
    }
    public void Add(IEnumerable<int> ids)
    {
        this.TargetIDs.AddRange(ids);
    }
    public void AddBannerNPC(int npcID)
    {
        LoadNpcIDsOfBanner();
        var bannerID = Item.NPCtoBanner(npcID);
        if (bannerID != 0)
        {
            this.Add(npcIDsofBanner![bannerID]);
        }
        else
        {
            this.Add(new[] { npcID });
        }
    }
    public bool IsValid(NPC npc)
    {
        return this.Condition?.Invoke(npc) ?? true;
    }
    public void Apply(NPC npc)
    {
        npc.lifeMax += this.LifeAdd;
        npc.life += this.LifeAdd;
        npc.lifeMax = (int) (npc.lifeMax * (1 + this.LifeMult));
        npc.life = (int) (npc.life * (1 + this.LifeMult));

        npc.defDefense += this.DefAdd;
        npc.defense += this.LifeAdd;
        npc.defDefense = (int) (npc.defDefense * (1 + this.DefMult));
        npc.defense = (int) (npc.defense * (1 + this.DefMult));
    }
    public void Attach()
    {
        foreach (var npcID in this.TargetIDs)
        {
            MainPlugin.Instance!.Enhancements![npcID] ??= new();
            MainPlugin.Instance!.Enhancements![npcID].Add(this);
        }
    }

    private static void LoadNpcIDsOfBanner()
    {
        if (npcIDsofBanner != null)
        {
            return;
        }
        npcIDsofBanner = new List<int>[Main.MaxBannerTypes];
        for (var i = 1; i < NPCID.Count; i++)
        {
            var bannerID = Item.NPCtoBanner(i);
            if (bannerID == 0)
            {
                continue;
            }
            npcIDsofBanner[bannerID] ??= new List<int>();
            npcIDsofBanner[bannerID].Add(i);
        }
    }
}