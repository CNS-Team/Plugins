﻿namespace TrProtocol.Models.TileEntities;

public partial class TEFoodPlatter : TileEntity
{
    public override void WriteExtraData(BinaryWriter writer)
    {
        this.Item.Write(writer);
    }

    public override TEFoodPlatter ReadExtraData(BinaryReader reader)
    {
        this.Item = new ItemData(reader);
        return this;
    }
    public override TileEntityType EntityType => TileEntityType.TEFoodPlatter;
    public ItemData Item { get; set; }
}