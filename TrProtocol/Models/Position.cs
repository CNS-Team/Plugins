using System.Runtime.InteropServices;

namespace TrProtocol.Models;

[StructLayout(LayoutKind.Sequential)]
public partial struct Position
{
    public Position(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    public int X, Y;
    public override string ToString()
    {
        return $"[{this.X}, {this.Y}]";
    }
}