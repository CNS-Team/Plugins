namespace TrProtocol.Models;

public partial class ItemData
{
    public ItemData(BinaryReader br)
    {
        this.Read(br);
    }
    public ItemData() { }
    public override string ToString()
    {
        return $"[{this.ItemID}, {this.Prefix}, {this.Stack}]";
    }
    public void Write(BinaryWriter bw)
    {
        bw.Write(this.ItemID);
        bw.Write(this.Prefix);
        bw.Write(this.Stack);
    }
    public ItemData Read(BinaryReader br)
    {
        this.ItemID = br.ReadInt16();
        this.Prefix = br.ReadByte();
        this.Stack = br.ReadInt16();
        return this;
    }
    public short ItemID { get; set; }
    public byte Prefix { get; set; }
    public short Stack { get; set; }
}