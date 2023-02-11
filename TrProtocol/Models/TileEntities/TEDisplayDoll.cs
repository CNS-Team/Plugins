namespace TrProtocol.Models.TileEntities;

public partial class TEDisplayDoll : TileEntity
{
    public override void WriteExtraData(BinaryWriter writer)
    {
        BitsByte bb = 0;
        bb[0] = this.Items[0] != null;
        bb[1] = this.Items[1] != null;
        bb[2] = this.Items[2] != null;
        bb[3] = this.Items[3] != null;
        bb[4] = this.Items[4] != null;
        bb[5] = this.Items[5] != null;
        bb[6] = this.Items[6] != null;
        bb[7] = this.Items[7] != null;
        BitsByte bb2 = 0;
        bb2[0] = this.Dyes[0] != null;
        bb2[1] = this.Dyes[1] != null;
        bb2[2] = this.Dyes[2] != null;
        bb2[3] = this.Dyes[3] != null;
        bb2[4] = this.Dyes[4] != null;
        bb2[5] = this.Dyes[5] != null;
        bb2[6] = this.Dyes[6] != null;
        bb2[7] = this.Dyes[7] != null;
        writer.Write(bb);
        writer.Write(bb2);
        for (var i = 0; i < 8; i++)
        {
            var item = this.Items[i];
            item?.Write(writer);
        }
        for (var j = 0; j < 8; j++)
        {
            var item = this.Dyes[j];
            item?.Write(writer);
        }
    }
    public override TEDisplayDoll ReadExtraData(BinaryReader reader)
    {
        BitsByte bitsByte = reader.ReadByte();
        BitsByte bitsByte2 = reader.ReadByte();
        for (var i = 0; i < 8; i++)
        {
            if (bitsByte[i])
            {
                this.Items[i] = new ItemData(reader);
            }
        }
        for (var j = 0; j < 8; j++)
        {
            if (bitsByte2[j])
            {
                this.Dyes[j] = new ItemData(reader);
            }
        }
        return this;
    }
    public override TileEntityType EntityType => TileEntityType.TEDisplayDoll;
    public ItemData[] Items { get; set; } = new ItemData[8];
    public ItemData[] Dyes { get; set; } = new ItemData[8];
}