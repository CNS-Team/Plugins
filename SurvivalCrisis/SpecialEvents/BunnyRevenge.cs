using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
    public class BunnyRevenge : BunnyTime
    {
        public BunnyRevenge()
        {
            this.bunnyType = NPCID.ExplosiveBunny;
            this.text = Texts.SpecialEvents.BunnyRevenge;
            this.color = Texts.SpecialEvents.BunnyRevengeColor;
        }
    }
}