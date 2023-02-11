using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AbandanTempleEnter;

[ApiVersion(2, 1)]
public class AbandanTempleEnter : TerrariaPlugin
{

    public override string Author => "Cai";


    public override string Description => "阻止玩家在解锁进度前进入神庙";

    public override string Name => "AbandanTempleEnter";

    public override Version Version => new Version(1, 1, 0, 0);

    public AbandanTempleEnter(Main game) : base(game)
    {
        this.Order = 5;
    }

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += this.OnReload;
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        Utils.Config = Config.GetConfig();
    }

    public static int checkTimes = 0;

    private void OnGameUpdate(EventArgs args)
    {
        //Console.WriteLine(checkTimes);
        if (checkTimes == Utils.Config.checkTime)
        {
            checkTimes = 0;
            CheckPos();
        }
        checkTimes++;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this.OnReload;
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
        }
        base.Dispose(disposing);
    }

    private void OnReload(ReloadEventArgs args)
    {
        Utils.Config = Config.GetConfig();
        args.Player.SendWarningMessage("[阻止进入神庙]配置文件已重载!");
    }

    private static void CheckPos()
    {
        try
        {
            foreach (var plr in TShock.Players)
            {
                if (plr != null && plr.Active && !plr.HasPermission("AbandanTempleEnterCheck.ignore") && !plr.Dead)
                {
                    if (Utils.IsInZoneLihzhardTemple(plr) && !Utils.CheckProgress())
                    {
                        if (Utils.Config.kill)
                        {
                            if (Utils.Config.Broadcast)
                            {
                                TSPlayer.All.SendErrorMessage(Utils.Config.killText, plr.Name);
                            }
                            else
                            {
                                plr.SendErrorMessage(Utils.Config.killText, plr.Name);
                            }
                            plr.KillPlayer();
                        }
                        else
                        {
                            if (Utils.Config.Broadcast)
                            {
                                TSPlayer.All.SendErrorMessage(Utils.Config.spawnText, plr.Name);
                            }
                            else
                            {
                                plr.SendErrorMessage(Utils.Config.spawnText, plr.Name);
                            }
                            plr.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.Message);
        }
    }
}