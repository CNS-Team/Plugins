using System;

namespace CommandTool;

internal class RectangleInfo
{
    public int XMin = 0;

    public int XMax = 0;

    public int YMin = 0;

    public int YMax = 0;

    public int Time = 0;

    public StandCommand[] Commands = Array.Empty<StandCommand>();

    public StandSign Sign = new ();

    public bool Contains(int x, int y)
    {
        return x <= this.XMax && x >= this.XMin && y <= this.YMax && y >= this.YMin;
    }
}