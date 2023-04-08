using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace NoSummonArea;

[ApiVersion(2, 1)]
public class NoSummonArea : TerrariaPlugin
{
    private readonly List<bool> sent = new List<bool>();

    public Dictionary<int, int> Multiples = new Dictionary<int, int>();

    public override string Author => "Leader";

    public override string Description => "区域禁止召唤";

    public override string Name => "NoSummerArea";

    public override Version Version => new Version(1, 0, 0, 0);

    public NoSummonArea(Main game)
        : base(game)
    {
        Config.GetConfig();
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
        ServerApi.Hooks.NetGetData.Register(this, this.OnNetGetdata);
        ServerApi.Hooks.NpcStrike.Register(this, this.OnNpcStrike);
        ServerApi.Hooks.ProjectileAIUpdate.Register(this, this.OnProj);
        for (int i = 0; i < 256; i++)
        {
            this.sent.Add(item: false);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnNetGetdata);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnNpcStrike);
            ServerApi.Hooks.ProjectileAIUpdate.Deregister(this, this.OnProj);
        }
        base.Dispose(disposing);
    }

    private void OnServerJoin(JoinEventArgs args)
    {
        this.Multiples[args.Who] = 0;
    }

    private void OnProj(ProjectileAiUpdateEventArgs args)
    {
        Config config = Config.GetConfig();
        if (!config.Projs.Contains(args.Projectile.type))
        {
            return;
        }
        Vector2 position = args.Projectile.position;
        position.X /= 16f;
        position.Y /= 16f;
        if (position.X > config.Xmin && position.X < config.Xmax && position.Y > config.Ymin && position.Y < config.Ymax)
        {
            Projectile projectile = args.Projectile;
            if (projectile.owner != 255)
            {
                TSPlayer tSPlayer = TShock.Players[projectile.owner];
                TShock.Log.Warn($"{tSPlayer.Name}疑似在保护区内({(int) projectile.position.X / 16},{(int) projectile.position.Y / 16})使用炸药");
                TSPlayer.All.SendWarningMessage(string.Format("警告:在({0},{1})检测到爆炸物，由玩家:{2}发射", (int) projectile.position.X / 16, (int) projectile.position.Y / 16, tSPlayer?.Name ?? "未知"));
                this.AddCount(projectile.owner);
            }
            projectile.active = false;
            projectile.type = 0;
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", projectile.whoAmI);
        }
    }

    private void OnNpcStrike(NpcStrikeEventArgs args)
    {
        if (args.Npc.netID == 661)
        {
            TSPlayer tSPlayer = TShock.Players[args.Player.whoAmI];
            Config config = Config.GetConfig();
            if (tSPlayer.TileX < config.Xmax && tSPlayer.TileX > config.Xmin && tSPlayer.TileY > config.Ymin && tSPlayer.TileY < config.Ymax)
            {
                TShock.Utils.Broadcast(tSPlayer.Name + "试图在禁用地区召唤", Color.Red);
                args.Damage = 0;
                args.Handled = true;
                args.Npc.active = false;
                this.AddCount(tSPlayer.Index);
            }
        }
    }

    private void OnNetGetdata(GetDataEventArgs args)
    {
        TSPlayer tSPlayer = TShock.Players[args.Msg.whoAmI];
        Config config = Config.GetConfig();
        if (args.MsgID == PacketTypes.SpawnBossorInvasion)
        {
            TShock.Utils.Broadcast(tSPlayer.Name + "试图召唤", Color.Red);
            if (tSPlayer.TileX < config.Xmax && tSPlayer.TileX > config.Xmin && tSPlayer.TileY > config.Ymin && tSPlayer.TileY < config.Ymax)
            {
                TShock.Utils.Broadcast(tSPlayer.Name + "试图在禁用地区召唤", Color.Red);
                args.Handled = true;
                this.AddCount(tSPlayer.Index);
            }
        }
        if (args.MsgID != PacketTypes.PlayerUpdate)
        {
            return;
        }
        if (tSPlayer.TileX < config.Xmax && tSPlayer.TileX > config.Xmin && tSPlayer.TileY > config.Ymin && tSPlayer.TileY < config.Ymax)
        {
            if (!this.sent[args.Msg.whoAmI])
            {
                tSPlayer.SendInfoMessage("您已进入禁止召唤区域");
                this.sent[args.Msg.whoAmI] = true;
            }
        }
        else if (this.sent[args.Msg.whoAmI])
        {
            this.sent[args.Msg.whoAmI] = false;
            tSPlayer.SendInfoMessage("您已离开禁止召唤区域");
        }
    }

    private void AddCount(int index)
    {
        if (Config.GetConfig().Multiplekickout)
        {
            this.Multiples[index]++;
            if (this.Multiples[index] >= Config.GetConfig().MultipleCount)
            {
                TShock.Players[index].Kick(TShock.Players[index].Name + "多次在禁地召唤或在保护区使用炸药,已踢出", force: true);
            }
        }
    }
}
