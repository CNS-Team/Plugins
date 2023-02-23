using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Effects
{
    public class PlayerWarp : Effect
    {
        public Vector2 Dest
        {
            get;
        }
        public int CountDown
        {
            get => this.Target.WarpingCountDown;
            set => this.Target.WarpingCountDown = value;
        }
        public PlayerWarp(GamePlayer target, int countDown, Vector2 dest) : base(Type.Player, target)
        {
            this.CountDown = countDown;
            this.Dest = dest;
        }

        public override void Apply()
        {

        }

        public override void Update()
        {
            if (SurvivalCrisis.Instance.TraitorEMPTime > 0)
            {
                this.Target.SendText("跃迁系统遭到干扰，本次跃迁中止", Color.Red);
                this.CountDown = 0;
                this.Abort();
                return;
            }
            if (this.CountDown == 0)
            {
                this.End();
                return;
            }
            if (this.CountDown % 60 == 0)
            {
                this.Target.AddStatusMessage($"{this.CountDown / 60}s后开始跃迁");
            }
            this.CountDown--;
        }

        public override void End()
        {
            this.Target.WarpingCount++;
            this.Target.TeleportTo(this.Dest);
            base.End();
        }
    }
}