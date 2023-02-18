using System.Runtime.InteropServices;

namespace TrProtocol.Models;

[StructLayout(LayoutKind.Sequential)]
public partial struct ShortPosition
{
    public ShortPosition(short x, short y)
    {
        this.X = x;
        this.Y = y;
    }
    public short X;
    public short Y;
    public override string ToString()
    {
        return $"[{this.X}, {this.Y}]";
    }
}