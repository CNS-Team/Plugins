using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AbandanTempleEnter
{
    [ApiVersion(2, 1)]
    public class AbandanTempleEnter : TerrariaPlugin
    {

        public override string Author
        {
            get
            {
                return "Cai";
            }
        }


        public override string Description
        {
            get
            {
                return "阻止玩家在解锁进度前进入神庙";
            }
        }

        public override string Name
        {
            get
            {
                return "AbandanTempleEnter";
            }
        }

        public override Version Version
        {
            get
            {
                return new Version(1, 1, 0, 0);
            }
        }

        public AbandanTempleEnter(Main game) : base(game)
        {
            Utils.Config = Config.GetConfig();
        }

        public override void Initialize()
        {
            WorldGen.Hooks.OnWorldLoad += Worldload;
            GeneralHooks.ReloadEvent += OnReload;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WorldGen.Hooks.OnWorldLoad -= Worldload;
                GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
        }
        private void Worldload()
        {
            Thread thread = new Thread(CheckThread);
            thread.IsBackground = true;
            thread.Start();
        }

        private void OnReload(ReloadEventArgs args)
        {
            Utils.Config = Config.GetConfig();
            args.Player.SendWarningMessage("[阻止进入神庙]: 配置文件已重载!");
        }

        private static void CheckThread()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(Utils.Config.checkTime);
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
                catch { }
            }
        }
    }
}
