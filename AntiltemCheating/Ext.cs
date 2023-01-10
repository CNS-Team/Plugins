using System;

namespace AntiItemCheating
{
	// Token: 0x02000004 RID: 4
	internal static class Ext
	{
		// Token: 0x0600000C RID: 12 RVA: 0x000057D0 File Offset: 0x000039D0
		public static void Add(this IItemChecker checker, params int[] ids)
		{
			foreach (int id in ids)
			{
				checker.Add(id);
			}
		}
	}
}
