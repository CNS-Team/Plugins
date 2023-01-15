using LazyUtils;
using Terraria;
using TerrariaApi.Server;

namespace PlayerReward;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Name => "Player Reward";
    public override string Author => "LaoSparrow";

    public Plugin(Main game) : base(game)
    {
    }
}