using TShockAPI;

namespace SwitchCommands;

public class SwitchPos
{
	public int X = 0;

	public int Y = 0;

	public SwitchPos()
	{
		X = 0;
		Y = 0;
	}

	public SwitchPos(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return StringExt.SFormat("X: {0}, Y: {1}", new object[2] { X, Y });
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SwitchPos switchPos))
		{
			return false;
		}
		return switchPos.X == X && switchPos.Y == Y;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
