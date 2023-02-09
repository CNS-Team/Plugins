using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace NPCEnhancer
{
	public class EnhSetting
	{
		public Func<NPC, bool>? Condition { get; }
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
			TargetIDs = new List<int>();
			Condition = condition;
			DefAdd = defAdd;
			DefMult = defMult;
			LifeAdd = lifeAdd;
			LifeMult = lifeMult;

			TargetIDs.AddRange(ids);
		}
		public void Add(params int[] ids)
		{
			TargetIDs.AddRange(ids);
		}
		public bool IsValid(NPC npc)
		{
			return Condition?.Invoke(npc) ?? true;
		}
		public void Apply(NPC npc)
		{
			npc.lifeMax += LifeAdd;
			npc.life += LifeAdd;
			npc.lifeMax = (int)(npc.lifeMax * (1 + LifeMult));
			npc.life = (int)(npc.life * (1 + LifeMult));

			npc.defDefense += DefAdd;
			npc.defense += LifeAdd;
			npc.defDefense = (int)(npc.defDefense * (1 + DefMult));
			npc.defense = (int)(npc.defense * (1 + DefMult));
		}
		public void Attach()
		{
			foreach(var npcID in TargetIDs)
			{
				MainPlugin.Instance!.Enhancements![npcID] ??= new();
				MainPlugin.Instance!.Enhancements![npcID].Add(this);
			}
		}
	}
}
