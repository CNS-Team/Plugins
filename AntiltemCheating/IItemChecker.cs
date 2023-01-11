namespace AntiItemCheating;

internal interface IItemChecker
{
	bool Obsolete { get; }

	void Add(int id);

	bool Contains(int id);
}
