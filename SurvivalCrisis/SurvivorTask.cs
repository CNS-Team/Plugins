using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis
{
    using Effects;
    public class SurvivorTask
    {
        public class ItemInfo
        {
            public int ID { get; }
            public double Score { get; }
            public int RequiredCount
            {
                get;
                private set;
            }
            public int CurrentCount
            {
                get;
                private set;
            }

            public int CountStillNeed => this.RequiredCount - this.CurrentCount;
            public int[] Contributions { get; }

            public ItemInfo(int id, int count, double score)
            {
                this.ID = id;
                this.Score = score;
                this.RequiredCount = count;
                this.CurrentCount = 0;
                this.Contributions = new int[40];
            }


            public void Contribute(GamePlayer player, int count)
            {
                this.CurrentCount += count;
                this.Contributions[player.Index] += count;
            }
            public string GetDetailContributions()
            {
                var sb = new StringBuilder(20);
                sb.Append(this.ToItemIcon());
                sb.Append(": ");
                foreach (var player in SurvivalCrisis.Instance.Participants)
                {
                    sb.Append($"{player.Index}号({this.Contributions[player.Index]})   ");
                }
                return sb.ToString();
            }
            public string ToItemIcon()
            {
                return $"[i/s{this.RequiredCount}:{this.ID}]";
            }
            public override string ToString()
            {
                return this.CountStillNeed == 0 ? string.Empty : $"[i/s{this.CountStillNeed}:{this.ID}]";
            }
            public string ToString2()
            {
                return $"[i:{this.ID}]({this.CurrentCount}/{this.RequiredCount})";
            }
        }
        /// <summary>
        /// 中二就完事了
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 完整描述
        /// </summary>
        public string Text { get; }
        /// <summary>
        /// 中二就完事了
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// 描述奖励
        /// </summary>
        public string BonusDescription { get; }
        /// <summary>
        /// 任务所需物品
        /// </summary>
        public ItemInfo[] Items { get; }
        public int ID { get; }

        public SurvivorTask(int id)
        {
            this.ID = id;
            switch (id)
            {
                case 1:
                {
                    this.Name = "[c/4444df:通讯系统维修]";
                    this.Description = @"你和你的同伴们发现先前用于联络救援的设备受到了不明损害.
好在, 修理起来并不困难, 只需要稍微费点功夫......";
                    this.BonusDescription = "[c/fefe11:开启全服通讯，手持天气收音机时发言将全服可见]";
                    this.Items = new[]
                    {
                            new ItemInfo(ItemID.FallenStar, 8, 2),
                            new ItemInfo(ItemID.LifeCrystal, 8, 4),
                            new ItemInfo(ItemID.IronOre, 60, 0.35),
                            new ItemInfo(ItemID.GoldOre, 60, 0.4)
                        };
                    this.Text = this.BuildText();
                }
                break;
            }
        }

        private string BuildText()
        {
            var sb = new StringBuilder(30);
            sb.Append("生存者任务 -- ").AppendLine(this.Name);
            sb.AppendLine(this.Description);
            sb.Append("  所需物品: ");
            foreach (var item in this.Items)
            {
                sb.Append(item.ToItemIcon());
            }
            sb.AppendLine();
            sb.AppendLine("奖励: ");
            sb.Append("     ").Append(this.BonusDescription);
            return sb.ToString();
        }

        public string CurrentProcess()
        {
            var str = string.Join("", (object[]) this.Items);
            return "还需:" + str;
        }

        public void CheckPiggy(GamePlayer player)
        {
            var anyContribution = false;
            var piggy = player.Piggybank;
            //var sb = new StringBuilder(50 * 6);
            //for (int i = 0; i < piggy.Count; i++)
            //{
            //	if (i % 10 == 0)
            //	{
            //		sb.AppendLine();
            //	}
            //	sb.Append(piggy[i].IsAir ? "□" : $"[i/s{piggy[i].Stack}:{piggy[i].Type}]");
            //}
            //SurvivalCrisis.DebugMessage(sb.ToString());
            for (var i = 0; i < piggy.Count; i++)
            {
                for (var j = 0; j < this.Items.Length; j++)
                {
                    if (this.Items[j].CountStillNeed == 0)
                    {
                        continue;
                    }
                    if (piggy[i].Type == this.Items[j].ID)
                    {
                        var c = piggy[i].Stack > this.Items[j].CountStillNeed ? this.Items[j].CountStillNeed : piggy[i].Stack;
                        this.Items[j].Contribute(player, c);
                        piggy[i].Stack -= c;
                        SurvivalCrisis.Instance.BCToAll($"{player.Index}号为任务贡献了[i/s{c}:{this.Items[j].ID}]", Color.Yellow);
                        player.AddPerformanceScore(c * this.Items[j].Score, "提交任务物品");
                        anyContribution = true;
                    }
                }
            }
            if (anyContribution)
            {
                if (this.Items.Sum(item => item.CountStillNeed) > 0)
                {
                    // SurvivalCrisis.Instance.BCToAll(CurrentProcess(), Color.LawnGreen);
                }
                else
                {
                    this.CompleteTask();
                }
            }
        }

        public void CompleteTask()
        {
            SurvivalCrisis.Instance.BCToAll(Texts.TaskCompleted, Color.LightGoldenrodYellow);
            switch (this.ID)
            {
                case 1:
                {
                    foreach (var player in SurvivalCrisis.Instance.Participants)
                    {
                        if (player.TSPlayer != null)
                        {
                            player.AddPerformanceScore(20, "任务完成");
                        }
                    }
                    SurvivalCrisis.GameOperations.AddEffect(new GlobalChat());
                    SurvivalCrisis.Instance.BCToAll(Texts.GlobalChatIsAvaiable, Color.ForestGreen);
                }
                break;
            }
            SurvivalCrisis.GameOperations.ToNextTask();
        }
    }
}