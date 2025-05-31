using System.Runtime.CompilerServices;

namespace CommandTool;

public class SwitchPos
{
    public int X;

    public int Y;

    public SwitchPos()
        : this(0, 0)
    {
    }

    public SwitchPos(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override string ToString()
    {
        var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
        defaultInterpolatedStringHandler.AppendLiteral("X: ");
        defaultInterpolatedStringHandler.AppendFormatted(this.X);
        defaultInterpolatedStringHandler.AppendLiteral(", Y: ");
        defaultInterpolatedStringHandler.AppendFormatted(this.Y);
        return defaultInterpolatedStringHandler.ToStringAndClear();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (!(obj is SwitchPos switchPos))
        {
            return false;
        }

        return switchPos.X == this.X && switchPos.Y == this.Y;
    }

    public override int GetHashCode()
    {
        return this.X.GetHashCode() ^ this.Y.GetHashCode();
    }
}