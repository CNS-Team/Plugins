﻿namespace TrProtocol.Models.TileEntities;

public partial class TETrainingDummy : TileEntity
{
    public override TileEntityType EntityType => TileEntityType.TETrainingDummy;
    public override void WriteExtraData(BinaryWriter writer)
    {
        writer.Write((short) this.NPC);
    }

    public override TETrainingDummy ReadExtraData(BinaryReader reader)
    {
        this.NPC = reader.ReadInt16();
        return this;
    }
    public int NPC { get; set; }
}