using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AntiProjecttileCheating;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override string Author => "少司命";
    public override string Description => "根据进度限制弹幕";
    public override string Name => "超进度弹幕限制";
    public override Version Version => new(1, 0, 0, 0);

    public Config config;

    private readonly bool[] Restricted = new bool[ProjectileID.Count];

    public string path = Path.Combine(TShock.SavePath, "超进度弹幕检测.json");

    public MainPlugin(Main game) : base(game)
    {
        this.config = Config.LoadConfig(this.path);
    }

    public override void Initialize()
    {
        this.config = Config.LoadConfig(this.path);
        GetDataHandlers.NewProjectile.Register(this.OnProj);
        ServerApi.Hooks.GamePostInitialize.Register(this, _ => this.UpdateRestricted());
        GeneralHooks.ReloadEvent += _ =>
        {
            this.config = Config.LoadConfig(this.path);
            this.UpdateRestricted();
        };
        DataSync.Plugin.OnProgressChanged += this.UpdateRestricted;
    }

    private void UpdateRestricted()
    {
        foreach (var f in this.config.Schemes)
        {
            if (f.AllowRemoteUnlocked && DataSync.Plugin.SyncedProgress.TryGetValue(f.Progress, out var value) && value)
            {
                continue;
            }

            foreach (var proj in f.Restricted)
            {
                this.Restricted[proj] = true;
            }
        }
    }

    private void OnProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (e.Player.HasPermission("progress.projecttile.white") || e.Handled || !e.Player.IsLoggedIn)
        {
            return;
        }

        if (this.Restricted[e.Type])
        {
            if (this.config.PunishPlayer)
            {
                e.Player.SetBuff(156, 60 * this.config.PunishTime, false);
            }
            e.Player.SendErrorMessage($"检测到超进度弹幕{Lang.GetProjectileName(e.Type).Value}!");
            if (this.config.Broadcast)
            {
                TShock.Utils.Broadcast($"检测到{e.Player.Name}使用超进度弹幕{Lang.GetProjectileName(e.Type).Value}!", Microsoft.Xna.Framework.Color.DarkRed);
            }
            if (this.config.WriteLog)
            {
                TShock.Log.Write($"[超进度弹幕限制] 玩家{e.Player.Name} 使用超进度弹幕 {Lang.GetProjectileName(e.Type).Value} ID =>{e.Type}", System.Diagnostics.TraceLevel.Info);
            }
            if (this.config.ClearItem)
            {
                Main.projectile[e.Index].active = false;
                Main.projectile[e.Index].type = 0;
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", e.Index);
            }

            if (this.config.KickPlayer)
            {
                e.Player.Kick("使用超进度弹幕");
            }
        }
    }
}