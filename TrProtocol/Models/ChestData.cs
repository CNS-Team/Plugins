namespace TrProtocol.Models;

public struct ChestData
{
    public override string ToString()
    {
        return $"[{this.TileX}, {this.TileY}] {this.Name}";
    }
    public short ID { get; set; }
    public short TileX { get; set; }
    public short TileY { get; set; }
    public string Name { get; set; }
}