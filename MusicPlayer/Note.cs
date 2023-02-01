namespace MusicPlayer;

public struct Note
{
    public uint time;
    public ushort lasting;
    public byte instrument, pitch, velocity;
}