using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace ProgressQuery
{
    public class Utils
    {
        public static Dictionary<string, string> GetProgressNames()
        {
            return new Dictionary<string, string>
            {
                { "史莱姆王","downedSlimeKing"},
                { "克苏鲁之眼", "downedBoss1" },
                { "世界吞噬怪", "downedBoss2" },
                { "克苏鲁之脑", "downedBoss2" },
                { "骷髅王", "downedBoss3" },
                { "独眼巨鹿", "downedDeerclops" },
                { "蜂王", "downedQueenBee" },
                { "血肉墙", "hardMode" },
                { "史莱姆皇后", "downedQueenSlime" },
                { "双子魔眼", "downedMechBoss2" },
                { "毁灭者", "downedMechBoss1"},
                { "机械骷髅王","downedMechBoss3" },
                { "世纪之花", "downedPlantBoss"},
                { "石巨人","downedGolemBoss" },
                { "猪鲨", "downedFishron"},
                { "光之女皇", "downedEmpressOfLight" },
                { "拜月教邪教徒", "downedAncientCultist" },
                { "日耀柱", "downedTowerSolar" },
                { "星云柱", "downedTowerNebula" },
                { "星旋柱", "downedTowerVortex" },
                { "星尘柱", "downedTowerStardust" },
                { "月球领主", "downedMoonlord" },
                { "雪人军团", "downedFrost" },
                { "哥布林军队", "downedGoblins" },
                { "海盗入侵", "downedPirates" },
                { "南瓜月", "downedHalloweenKing" },
                { "火星暴乱", "downedMartians" },
                { "霜月", "downedChristmasIceQueen" }
            };
        }
            
        public static Dictionary<string, FieldInfo> GetNPCFilelds()
        {
            #pragma warning disable CS8604 // 引用类型参数可能为 null。
            var Progress = new Dictionary<string, FieldInfo>();
            GetProgressNames().ForEach(x =>
            {
                if (x.Key == "血肉墙")
                {
                    Progress.Add(x.Key, typeof(Main).GetField(x.Value));
                }
                else
                {
                    Progress.Add(x.Key, typeof(NPC).GetField(x.Value));
                }
                #pragma warning restore CS8604 // 引用类型参数可能为 null。
            });
            return Progress;
        }

        public static Dictionary<string, bool> GetGameProgress()
        {
            Dictionary<string, bool> Progress = new();
            GetNPCFilelds().ForEach(f =>
            {
                Progress.Add(f.Key, Convert.ToBoolean(f.Value.GetValue(null)));
            });
            return Progress;
        }


        public static Dictionary<string, bool> Ongoing()
        {
            Dictionary<string, bool> going = new Dictionary<string, bool>();
            going.Add("霜月", Main.snowMoon);
            going.Add("南瓜月", Main.pumpkinMoon);
            going.Add("哥布林军队", Main.invasionType == 1);
            going.Add("雪人军团", Main.invasionType == 2);
            going.Add("海盗入侵", Main.invasionType == 3);
            going.Add("火星暴乱", Main.invasionType == 4);
            return going;
        }
    }
}
