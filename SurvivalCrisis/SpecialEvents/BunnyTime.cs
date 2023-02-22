using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
    public class BunnyTime : SpecialEvent
    {
        private SurvivalCrisis Game;
        protected int bunnyType;
        protected string text;
        protected Color color;
        public BunnyTime()
        {
            this.bunnyType = NPCID.Bunny;
            this.text = Texts.SpecialEvents.BunnyTime;
            this.color = Texts.SpecialEvents.BunnyTimeColor;
        }

        public override void Reset()
        {
            base.Reset();
            this.Game = SurvivalCrisis.Instance;
            this.StartDelay = 0;
            this.TimeLeft = 60 * 60;
        }

        public override void Start()
        {
            base.Start();
            SurvivalCrisis.Instance.BCToAll(this.text, this.color);
            foreach (var player in this.Game.Participants)
            {
                if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
                {
                    player.TSPlayer.SetBuff(BuffID.Confused, 10 * 60);
                    player.TSPlayer.SetBuff(BuffID.Featherfall, 60 * 60);
                }
            }
        }

        public override void Update()
        {
            foreach (var player in this.Game.Participants)
            {
                if (player?.TSPlayer != null && player.IsValid() && player.Identity != PlayerIdentity.Watcher && this.TimeLeft % 60 == player.Index)
                {
                    if (Main.npc.Count(npc => npc.active && npc.type == this.bunnyType && npc.Distance(player.TPlayer.Center) < 16 * 100) < 20)
                    {
                        SurvivalCrisis.GameOperations.TrySpawnEnemies(player, 2, new[] { (this.bunnyType, 0.005) });
                    }
                }
            }
        }
    }
}