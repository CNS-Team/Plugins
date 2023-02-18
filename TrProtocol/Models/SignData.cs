namespace TrProtocol.Models;

public partial struct SignData
{
    public override string ToString()
    {
        return $"[{this.TileX}, {this.TileY}] {this.Text}";
    }
    public short ID { get; set; }
    public short TileX { get; set; }
    public short TileY { get; set; }
    public string Text { get; set; }
}