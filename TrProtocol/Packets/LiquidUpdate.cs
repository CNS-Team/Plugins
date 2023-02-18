namespace TrProtocol.Packets;

public class LiquidUpdate : Packet
{
    [Obsolete]
    public override MessageID Type => MessageID.LiquidUpdate;
    public short TileX { get; set; }
    public short TileY { get; set; }
    public byte Liquid { get; set; }
    public byte LiquidType { get; set; }
}