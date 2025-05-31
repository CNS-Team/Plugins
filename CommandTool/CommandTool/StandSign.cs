using Microsoft.Xna.Framework;

namespace CommandTool;

internal class StandSign
{
    public bool Enable { get; set; }

    public Color Color { get; set; }

    public string? OnEnter { get; set; }

    public string? Loop { get; set; }
}