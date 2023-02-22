using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis
{
    public abstract class Effect
    {
        public enum Type
        {
            Global,
            Traitor,
            Survivor,
            Player
        }
        public Type EffectType { get; }
        public GamePlayer Target { get; }
        /// <summary>
        /// 当调用Effect::End()时将被设置为true
        /// </summary>
        public bool IsEnd
        {
            get;
            private set;
        }
        protected Effect(Type effectType, GamePlayer target = null)
        {
            this.EffectType = effectType;
            this.Target = target;
        }

        public abstract void Apply();
        public abstract void Update();
        public virtual void Abort()
        {
            this.IsEnd = true;
        }
        public virtual void End()
        {
            this.IsEnd = true;
        }
    }
}