using System.Runtime.InteropServices;

namespace TrProtocol.Models;

[StructLayout(LayoutKind.Sequential)]
public partial struct Vector2
{
    public Vector2(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    public float X;
    public float Y;

    public override string ToString()
    {
        return $"[{this.X}, {this.Y}]";
    }
}