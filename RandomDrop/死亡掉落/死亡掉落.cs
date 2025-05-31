using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.DataStructures;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace 死亡掉落;

[ApiVersion(2, 1)]
public class 死亡掉落 : TerrariaPlugin
{
    private static List<int> Ls0 = new (); 

    private static List<int> Ls1 = 死亡掉落配置表.GetConfig().哥布林一前禁物品;

    private static List<int> Ls2 = 死亡掉落配置表.GetConfig().萌王前禁物品;

    private static List<int> Ls3 = 死亡掉落配置表.GetConfig().鹿角怪前禁物品;

    private static List<int> Ls4 = 死亡掉落配置表.GetConfig().克眼前禁物品;

    private static List<int> Ls5 = 死亡掉落配置表.GetConfig().虫脑前禁物品;

    private static List<int> Ls6 = 死亡掉落配置表.GetConfig().旧日一前禁物品;

    private static List<int> Ls7 = 死亡掉落配置表.GetConfig().蜂后前禁物品;

    private static List<int> Ls8 = 死亡掉落配置表.GetConfig().骷髅前禁物品;

    private static List<int> Ls9 = 死亡掉落配置表.GetConfig().肉墙前禁物品;

    private static List<int> Ls10 = 死亡掉落配置表.GetConfig().哥布林二前禁物品;

    private static List<int> Ls11 = 死亡掉落配置表.GetConfig().海盗前禁物品;

    private static List<int> Ls12 = 死亡掉落配置表.GetConfig().日蚀一前禁物品;

    private static List<int> Ls13 = 死亡掉落配置表.GetConfig().萌后前禁物品;

    private static List<int> Ls14 = 死亡掉落配置表.GetConfig().任一三王前禁物品;

    private static List<int> Ls15 = 死亡掉落配置表.GetConfig().旧日二前禁物品;

    private static List<int> Ls16 = 死亡掉落配置表.GetConfig().机械眼前禁物品;

    private static List<int> Ls17 = 死亡掉落配置表.GetConfig().机械虫前禁物品;

    private static List<int> Ls18 = 死亡掉落配置表.GetConfig().机械骷髅前禁物品;

    private static List<int> Ls19 = 死亡掉落配置表.GetConfig().三王前禁物品;

    private static List<int> Ls20 = 死亡掉落配置表.GetConfig().日蚀二前禁物品;

    private static List<int> Ls21 = 死亡掉落配置表.GetConfig().猪鲨前禁物品;

    private static List<int> Ls22 = 死亡掉落配置表.GetConfig().妖花前禁物品;

    private static List<int> Ls23 = 死亡掉落配置表.GetConfig().日蚀三前禁物品;

    private static List<int> Ls24 = 死亡掉落配置表.GetConfig().霜月树前禁物品;

    private static List<int> Ls25 = 死亡掉落配置表.GetConfig().霜月坦前禁物品;

    private static List<int> Ls26 = 死亡掉落配置表.GetConfig().霜月后前禁物品;

    private static List<int> Ls27 = 死亡掉落配置表.GetConfig().南瓜树前禁物品;

    private static List<int> Ls28 = 死亡掉落配置表.GetConfig().南瓜王前禁物品;

    private static List<int> Ls29 = 死亡掉落配置表.GetConfig().光女前禁物品;

    private static List<int> Ls30 = 死亡掉落配置表.GetConfig().石巨人前禁物品;

    private static List<int> Ls31 = 死亡掉落配置表.GetConfig().旧日三前禁物品;

    private static List<int> Ls32 = 死亡掉落配置表.GetConfig().外星人前禁物品;

    private static List<int> Ls33 = 死亡掉落配置表.GetConfig().教徒前禁物品;

    private static List<int> Ls34 = 死亡掉落配置表.GetConfig().日耀前禁物品;

    private static List<int> Ls35 = 死亡掉落配置表.GetConfig().星旋前禁物品;

    private static List<int> Ls36 = 死亡掉落配置表.GetConfig().星尘前禁物品;

    private static List<int> Ls37 = 死亡掉落配置表.GetConfig().星云前禁物品;

    private static List<int> Ls38 = 死亡掉落配置表.GetConfig().所有柱子前禁物品;

    private static List<int> Ls39 = 死亡掉落配置表.GetConfig().月总前禁物品;

    private static List<int> 所有进度禁止爆出物品ID列表 = Ls1.Union(Ls2).Union(Ls3).Union(Ls4)
        .Union(Ls5)
        .Union(Ls6)
        .Union(Ls7)
        .Union(Ls8)
        .Union(Ls9)
        .Union(Ls10)
        .Union(Ls11)
        .Union(Ls12)
        .Union(Ls13)
        .Union(Ls14)
        .Union(Ls15)
        .Union(Ls16)
        .Union(Ls17)
        .Union(Ls18)
        .Union(Ls19)
        .Union(Ls20)
        .Union(Ls21)
        .Union(Ls22)
        .Union(Ls23)
        .Union(Ls24)
        .Union(Ls25)
        .Union(Ls26)
        .Union(Ls27)
        .Union(Ls28)
        .Union(Ls29)
        .Union(Ls30)
        .Union(Ls31)
        .Union(Ls32)
        .Union(Ls33)
        .Union(Ls34)
        .Union(Ls35)
        .Union(Ls36)
        .Union(Ls37)
        .Union(Ls38)
        .Union(Ls39)
        .ToList();

    public static Random random = new ();

    public override string Author => "大豆子-参考Dr.的进度检测插件";

    public override string Description => "怪物死亡后随机掉落物品";

    public override string Name => "Pigeon定制版-随机掉落1436";

    public override Version Version => new (1, 0, 0, 0);

    public 死亡掉落(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPostInitialize);
        ServerApi.Hooks.NpcKilled.Register(this, this.NPCDead);
        ServerApi.Hooks.NpcKilled.Register(this, this.NpcKilled);
        GeneralHooks.ReloadEvent += this.GeneralHooksOnReloadEvent;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnPostInitialize);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.NPCDead);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.NpcKilled);
            死亡掉落配置表.GetConfig();
        }

        base.Dispose(disposing);
    }
    
    private void GeneralHooksOnReloadEvent(ReloadEventArgs e)
    {
        死亡掉落配置表.ReloadConfig();
        e.Player.SendSuccessMessage("[死亡掉落]配置文件已重载!");
    }

    private void OnPostInitialize(EventArgs args)
    {
        try
        {
            if (!NPC.downedGoblins && !NPC.downedSlimeKing && !NPC.downedDeerclops && !NPC.downedBoss1 && !NPC.downedBoss2 && !NPC.downedQueenBee && !NPC.downedBoss3 && !Main.hardMode)
            {
                this.更改配置表判断符("哥布林一进行", false);
                this.更改配置表判断符("旧日军团一", false);
                this.更改配置表判断符("哥布林二进行", false);
                this.更改配置表判断符("海盗进行", false);
                this.更改配置表判断符("日蚀一进行", false);
                this.更改配置表判断符("旧日军团二", false);
                this.更改配置表判断符("日蚀二进行", false);
                this.更改配置表判断符("日蚀三进行", false);
                this.更改配置表判断符("旧日军团三", false);
                this.更改配置表判断符("火星进行", false);
            }
        }
        catch
        {
        }
    }

    private async void NPCDead(NpcKilledEventArgs args)
    {
        try
        {
            var tsplayer = args.npc.lastInteraction != 255 && args.npc.lastInteraction >= 0 ? TShock.Players[args.npc.lastInteraction] : null;
            if (!死亡掉落配置表.GetConfig().是否开启进度掉落)
            {
                return;
            }

            new List<int>();
            var 禁止爆物表 = 所有进度禁止爆出物品ID列表;
            if (NPC.downedGoblins || 死亡掉落配置表.GetConfig().哥布林一进行)
            {
                var 取表71 = Ls1;
                禁止爆物表 = 移除列表<int>(取表71, 禁止爆物表);
            }

            if (NPC.downedSlimeKing)
            {
                var 取表74 = Ls2;
                禁止爆物表 = 移除列表<int>(取表74, 禁止爆物表);
            }

            if (NPC.downedDeerclops)
            {
                var 取表73 = Ls3;
                禁止爆物表 = 移除列表<int>(取表73, 禁止爆物表);
            }

            if (NPC.downedBoss1)
            {
                var 取表72 = Ls4;
                禁止爆物表 = 移除列表<int>(取表72, 禁止爆物表);
            }

            if (NPC.downedBoss2 || NPC.npcsFoundForCheckActive[13] || NPC.npcsFoundForCheckActive[266])
            {
                var 取表70 = Ls5;
                禁止爆物表 = 移除列表<int>(取表70, 禁止爆物表);
            }

            if (NPC.downedBoss2 & 死亡掉落配置表.GetConfig().旧日军团一)
            {
                var 取表69 = Ls6;
                禁止爆物表 = 移除列表<int>(取表69, 禁止爆物表);
            }

            if (NPC.downedQueenBee)
            {
                var 取表68 = Ls7;
                禁止爆物表 = 移除列表<int>(取表68, 禁止爆物表);
            }

            if (NPC.downedBoss3)
            {
                var 取表67 = Ls8;
                禁止爆物表 = 移除列表<int>(取表67, 禁止爆物表);
            }

            if (Main.hardMode)
            {
                var 取表66 = Ls9;
                禁止爆物表 = 移除列表<int>(取表66, 禁止爆物表);
            }

            if (Main.hardMode & 死亡掉落配置表.GetConfig().哥布林二进行)
            {
                var 取表65 = Ls10;
                禁止爆物表 = 移除列表<int>(取表65, 禁止爆物表);
            }

            if (NPC.downedPirates || 死亡掉落配置表.GetConfig().海盗进行)
            {
                var 取表64 = Ls11;
                禁止爆物表 = 移除列表<int>(取表64, 禁止爆物表);
            }

            if (Main.hardMode & 死亡掉落配置表.GetConfig().日蚀一进行)
            {
                var 取表63 = Ls12;
                禁止爆物表 = 移除列表<int>(取表63, 禁止爆物表);
            }

            if (NPC.downedQueenSlime)
            {
                var 取表62 = Ls13;
                禁止爆物表 = 移除列表<int>(取表62, 禁止爆物表);
            }

            if (NPC.downedMechBossAny)
            {
                var 取表61 = Ls14;
                禁止爆物表 = 移除列表<int>(取表61, 禁止爆物表);
            }

            if (NPC.downedMechBossAny & 死亡掉落配置表.GetConfig().旧日军团二)
            {
                var 取表60 = Ls15;
                禁止爆物表 = 移除列表<int>(取表60, 禁止爆物表);
            }

            if (NPC.downedMechBoss2)
            {
                var 取表59 = Ls16;
                禁止爆物表 = 移除列表<int>(取表59, 禁止爆物表);
            }

            if (NPC.downedMechBoss1)
            {
                var 取表58 = Ls17;
                禁止爆物表 = 移除列表<int>(取表58, 禁止爆物表);
            }

            if (NPC.downedMechBoss3)
            {
                var 取表57 = Ls18;
                禁止爆物表 = 移除列表<int>(取表57, 禁止爆物表);
            }

            if (NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3)
            {
                var 取表56 = Ls19;
                禁止爆物表 = 移除列表<int>(取表56, 禁止爆物表);
            }

            if (NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3 & 死亡掉落配置表.GetConfig().日蚀二进行)
            {
                var 取表55 = Ls20;
                禁止爆物表 = 移除列表<int>(取表55, 禁止爆物表);
            }

            if (NPC.downedFishron)
            {
                var 取表54 = Ls21;
                禁止爆物表 = 移除列表<int>(取表54, 禁止爆物表);
            }

            if (NPC.downedPlantBoss)
            {
                var 取表53 = Ls22;
                禁止爆物表 = 移除列表<int>(取表53, 禁止爆物表);
            }

            if (NPC.downedPlantBoss & 死亡掉落配置表.GetConfig().日蚀三进行)
            {
                var 取表52 = Ls23;
                禁止爆物表 = 移除列表<int>(取表52, 禁止爆物表);
            }

            if (NPC.downedChristmasTree)
            {
                var 取表51 = Ls24;
                禁止爆物表 = 移除列表<int>(取表51, 禁止爆物表);
            }

            if (NPC.downedChristmasSantank)
            {
                var 取表50 = Ls25;
                禁止爆物表 = 移除列表<int>(取表50, 禁止爆物表);
            }

            if (NPC.downedChristmasIceQueen)
            {
                var 取表49 = Ls26;
                禁止爆物表 = 移除列表<int>(取表49, 禁止爆物表);
            }

            if (NPC.downedHalloweenTree)
            {
                var 取表48 = Ls27;
                禁止爆物表 = 移除列表<int>(取表48, 禁止爆物表);
            }

            if (NPC.downedHalloweenKing)
            {
                var 取表47 = Ls28;
                禁止爆物表 = 移除列表<int>(取表47, 禁止爆物表);
            }

            if (NPC.downedEmpressOfLight)
            {
                var 取表46 = Ls29;
                禁止爆物表 = 移除列表<int>(取表46, 禁止爆物表);
            }

            if (NPC.downedGolemBoss)
            {
                var 取表45 = Ls30;
                禁止爆物表 = 移除列表<int>(取表45, 禁止爆物表);
            }

            if (NPC.downedGolemBoss & 死亡掉落配置表.GetConfig().旧日军团三)
            {
                var 取表44 = Ls31;
                禁止爆物表 = 移除列表<int>(取表44, 禁止爆物表);
            }

            if (NPC.downedMartians || 死亡掉落配置表.GetConfig().火星进行)
            {
                var 取表43 = Ls32;
                禁止爆物表 = 移除列表<int>(取表43, 禁止爆物表);
            }

            if (NPC.downedAncientCultist)
            {
                var 取表42 = Ls33;
                禁止爆物表 = 移除列表<int>(取表42, 禁止爆物表);
            }

            if (NPC.downedTowerSolar)
            {
                var 取表41 = Ls34;
                禁止爆物表 = 移除列表<int>(取表41, 禁止爆物表);
            }

            if (NPC.downedTowerVortex)
            {
                var 取表40 = Ls35;
                禁止爆物表 = 移除列表<int>(取表40, 禁止爆物表);
            }

            if (NPC.downedTowerStardust)
            {
                var 取表39 = Ls36;
                禁止爆物表 = 移除列表<int>(取表39, 禁止爆物表);
            }

            if (NPC.downedTowerNebula)
            {
                var 取表38 = Ls37;
                禁止爆物表 = 移除列表<int>(取表38, 禁止爆物表);
            }

            if (NPC.downedTowerSolar & NPC.downedTowerVortex & NPC.downedTowerStardust & NPC.downedTowerNebula)
            {
                var 取表37 = Ls38;
                禁止爆物表 = 移除列表<int>(取表37, 禁止爆物表);
            }

            if (NPC.downedMoonlord)
            {
                var 取表36 = Ls39;
                禁止爆物表 = 移除列表<int>(取表36, 禁止爆物表);
            }

            if (Candorp(死亡掉落配置表.GetConfig().进度掉落概率))
            {
                var r1 = new Random().Next(1, 5124);
                if (禁止爆物表.Contains(r1))
                {
                    this.销毁提示(tsplayer, r1);
                }
                else
                {
                    this.物品掉落(args, tsplayer, r1);
                }
            }
        }
        catch
        {
        }
    }

    private async void NpcKilled(NpcKilledEventArgs args)
    {
        try
        {
            if (!死亡掉落配置表.GetConfig().哥布林一进行 && (args.npc.netID == 26 || args.npc.netID == 27 || args.npc.netID == 28 || args.npc.netID == 29 || args.npc.netID == 111))
            {
                this.更改配置表判断符("哥布林一进行", true);
            }

            if (!死亡掉落配置表.GetConfig().旧日军团一 && (args.npc.netID == 564 || args.npc.netID == 565))
            {
                this.更改配置表判断符("旧日军团一", true);
            }

            if (!死亡掉落配置表.GetConfig().哥布林二进行 && args.npc.netID == 471)
            {
                this.更改配置表判断符("哥布林二进行", true);
            }

            if (!死亡掉落配置表.GetConfig().海盗进行 && (args.npc.netID == 212 || args.npc.netID == 213 || args.npc.netID == 214 || args.npc.netID == 215 || args.npc.netID == 216 || args.npc.netID == 491))
            {
                this.更改配置表判断符("海盗进行", true);
            }

            if (!死亡掉落配置表.GetConfig().日蚀一进行 && (args.npc.netID == 158 || args.npc.netID == 159 || args.npc.netID == 461))
            {
                this.更改配置表判断符("日蚀一进行", true);
            }

            if (!死亡掉落配置表.GetConfig().旧日军团二 && (args.npc.netID == 576 || args.npc.netID == 577))
            {
                this.更改配置表判断符("旧日军团二", true);
            }

            if (!死亡掉落配置表.GetConfig().日蚀二进行 && (args.npc.netID == 253 || args.npc.netID == 477))
            {
                this.更改配置表判断符("日蚀二进行", true);
            }

            if (!死亡掉落配置表.GetConfig().日蚀三进行 && (args.npc.netID == 460 || args.npc.netID == 463 || args.npc.netID == 466 || args.npc.netID == 467 || args.npc.netID == 468))
            {
                this.更改配置表判断符("日蚀三进行", true);
            }

            if (!死亡掉落配置表.GetConfig().旧日军团三 && args.npc.netID == 551)
            {
                this.更改配置表判断符("旧日军团三", true);
            }

            if (!死亡掉落配置表.GetConfig().火星进行 && (args.npc.netID == 381 || args.npc.netID == 382 || args.npc.netID == 383 || args.npc.netID == 384 || args.npc.netID == 385 || args.npc.netID == 386 || args.npc.netID == 389 || args.npc.netID == 390 || args.npc.netID == 392))
            {
                this.更改配置表判断符("火星进行", true);
            }
        }
        catch
        {
        }
    }

    public static bool Candorp(int probability)
    {
        return random.Next(1, 死亡掉落配置表.GetConfig().总掉落百分比) <= probability ? true : false;
    }

    public static List<int> 移除列表<T>(List<int> 要移除表数据, List<int> 被移除表)
    {
        try
        {
            for (var i = 0; i < 要移除表数据.Count; i++)
            {
                if (被移除表.Contains(要移除表数据[i]))
                {
                    被移除表.Remove(要移除表数据[i]);
                }
            }
        }
        catch
        {
        }

        return 被移除表;
    }

    private async void 物品掉落(NpcKilledEventArgs args, TSPlayer tsplayer, int 掉落物ID)
    {
        try
        {
            var number = Item.NewItem(Type: TShock.Utils.GetItemById(掉落物ID).type, source: new EntitySource_DebugCommand(), X: (int) args.npc.position.X, Y: (int) args.npc.position.Y, Width: tsplayer.TPlayer.width, Height: tsplayer.TPlayer.height, Stack: 死亡掉落配置表.GetConfig().进度掉落数量, noBroadcast: true, pfix: 0, noGrabDelay: true);
            tsplayer.SendData(PacketTypes.ItemDrop, "", number);
        }
        catch
        {
        }
    }

    private async void 销毁提示(TSPlayer tsplayer, int 掉落物ID)
    {
        try
        {
            if (死亡掉落配置表.GetConfig().是否开启销毁提示)
            {
                tsplayer.SendErrorMessage($"[i:{掉落物ID}]由于禁爆被销毁");
            }
        }
        catch
        {
        }
    }

    private async void 更改配置表判断符(string 引索文本, bool 更换判断)
    {
        try
        {
            var JsonPath = Path.Combine(TShock.SavePath, "随机掉落配置表.json");
            var JsonString = File.ReadAllText(JsonPath, Encoding.UTF8);
            var jo = JObject.Parse(JsonString);
            jo[引索文本] = 更换判断;
            var convertString = Convert.ToString(jo);
            File.WriteAllText(JsonPath, convertString);
        }
        catch
        {
        }
    }
}