namespace TrProtocol.Models.TileEntities;

public partial class TEHatRack : TileEntity
{
    public override TileEntityType EntityType => TileEntityType.TEHatRack;
    public override void WriteExtraData(BinaryWriter writer)
    {
        BitsByte bb = 0;
        bb[0] = this.Items[0] != null;
        bb[1] = this.Items[1] != null;
        bb[2] = this.Dyes[0] != null;
        bb[3] = this.Dyes[1] != null;
        writer.Write(bb);
        for (var i = 0; i < 2; i++)
        {
            this.Items[i]?.Write(writer);
        }
        for (var j = 0; j < 2; j++)
        {
            this.Dyes[j]?.Write(writer);
        }
    }

    public override TEHatRack ReadExtraData(BinaryReader reader)
    {
        BitsByte bitsByte = reader.ReadByte();
        for (var i = 0; i < 2; i++)
        {
            if (bitsByte[i])
            {
                this.Items[i] = new ItemData(reader);
            }
        }
        for (var j = 0; j < 2; j++)
        {
            if (bitsByte[j + 2])
            {
                this.Dyes[j] = new ItemData(reader);
            }
        }
        return this;
    }
    public ItemData[] Items { get; set; } = new ItemData[2];

    public ItemData[] Dyes { get; set; } = new ItemData[2];
}