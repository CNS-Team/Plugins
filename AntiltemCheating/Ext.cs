namespace AntiItemCheating;

internal static class Ext
{
	public static void Add(this IItemChecker checker, params int[] ids)
	{
		foreach (int id in ids)
		{
			checker.Add(id);
		}
	}
}
