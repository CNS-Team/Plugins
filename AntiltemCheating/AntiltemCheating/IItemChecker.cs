using System;

namespace AntiItemCheating
{
	// Token: 0x02000005 RID: 5
	internal interface IItemChecker
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000D RID: 13
		bool Obsolete { get; }

		// Token: 0x0600000E RID: 14
		void Add(int id);

		// Token: 0x0600000F RID: 15
		bool Contains(int id);
	}
}
