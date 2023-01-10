using System;
using System.Collections.Generic;

namespace AntiItemCheating
{
	// Token: 0x02000003 RID: 3
	internal class DefaultItemChecker : IItemChecker
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00005774 File Offset: 0x00003974
		public bool Obsolete
		{
			get
			{
				return this.obsoleteNow();
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00005781 File Offset: 0x00003981
		public DefaultItemChecker(Func<bool> obsolete)
		{
			this.obsoleteNow = obsolete;
			this.ids = new SortedSet<int>();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000579D File Offset: 0x0000399D
		public void Add(int id)
		{
			this.ids.Add(id);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000057B0 File Offset: 0x000039B0
		public bool Contains(int id)
		{
			return this.ids.Contains(id);
		}

		// Token: 0x04000039 RID: 57
		private Func<bool> obsoleteNow;

		// Token: 0x0400003A RID: 58
		private ISet<int> ids;
	}
}
