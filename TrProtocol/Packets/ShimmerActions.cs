namespace TrProtocol.Packets;

public class ShimmerActions : Packet, INPCSlot
{
    public override MessageID Type => MessageID.ShimmerActions;
    public byte ShimmerType { get; set; }
    private bool IsItemShimmer => this.ShimmerType == 0;
    private bool IsPlayerShimmer => this.ShimmerType == 1;
    private bool IsNpcShimmer => this.ShimmerType == 2;
    [Condition(nameof(IsItemShimmer))]
    public Vector2 ShimmerPosition { get; set; }
    [Condition(nameof(IsPlayerShimmer))]
    public Vector2 CoinPosition { get; set; }
    [Condition(nameof(IsPlayerShimmer))]
    public int CoinAmount { get; set; }
    [Condition(nameof(IsNpcShimmer))]
    public short NPCSlot { get; set; }
    [Condition(nameof(IsNpcShimmer))]
    public short NPCSlotHighBits { get; set; }
}