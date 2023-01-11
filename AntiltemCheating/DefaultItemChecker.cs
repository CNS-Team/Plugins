using System;
using System.Collections.Generic;

namespace AntiItemCheating;

internal class DefaultItemChecker : IItemChecker
{
	private Func<bool> obsoleteNow;

	private ISet<int> ids;

	public bool Obsolete => obsoleteNow();

	public DefaultItemChecker(Func<bool> obsolete)
	{
		obsoleteNow = obsolete;
		ids = new SortedSet<int>();
	}

	public void Add(int id)
	{
		ids.Add(id);
	}

	public bool Contains(int id)
	{
		return ids.Contains(id);
	}
}
