namespace TrProtocol.Models.TileEntities;

public partial class TELogicSensor : TileEntity
{
    public override TileEntityType EntityType => TileEntityType.TELogicSensor;
    public override void WriteExtraData(BinaryWriter writer)
    {
        writer.Write((byte) this.LogicCheck);
        writer.Write(this.On);
    }
    public override TELogicSensor ReadExtraData(BinaryReader reader)
    {
        this.LogicCheck = (LogicCheckType) reader.ReadByte();
        this.On = reader.ReadBoolean();
        return this;
    }
    public LogicCheckType LogicCheck { get; set; }
    public bool On { get; set; }
}