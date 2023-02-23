using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SurvivalCrisis
{
    public abstract class SpecialEvent
    {
        public bool Active { get; set; }
        public bool Triggered { get; set; }
        public int StartDelay { get; set; }
        public int TimeLeft { get; set; }

        protected SpecialEvent()
        {

        }

        public abstract void Update();

        public virtual void Start()
        {
            this.Active = true;
            this.Triggered = true;
        }
        public virtual void End(in bool gameEnd = false)
        {
            this.Active = false;
        }
        public virtual void Reset()
        {
            this.Triggered = false;
        }
    }
}