using StatusTxtMgr.Utils;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace StatusTxtMgr
{
    [ApiVersion(2, 1)]
    public class StatusTxtMgr : TerrariaPlugin
    {
        #region Plugin Infos
        public override string Name => "Status Text Manager";
        public override Version Version => new("1.0.0");
        public override string Author => "LaoSparrow (Team CNS)";
        public override string Description => "Manage status text of different plugins";
        #endregion

        #region Fields
        // Config
        private static string ConfigFilePath = Path.Combine(TShock.SavePath, "StatusTxtMgr.json");
        private static ConfigFile<STMSettings> Config = new();
        internal static STMSettings Settings => Config.Settings;

        // Exported Hooks
        public static ApiShortcut Hooks = new();
        private static readonly StatusTextUpdateHandlerList handlerList = new();

        // States
        private readonly bool[] isPlrNeedInit = new bool[Main.maxPlayers];
        private readonly bool[] isPlrSTVisible = new bool[Main.maxPlayers];
        #endregion

        #region Initialize / Dispose
        public StatusTxtMgr(Main game) : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnGameInitialize);
            ServerApi.Hooks.GamePostUpdate.Register(this, OnGamePostUpdate);
            ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin);
            GeneralHooks.ReloadEvent += OnReload;

            Commands.ChatCommands.Add(new Command(cmdst, "statustext", "st"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnGameInitialize);
                ServerApi.Hooks.GamePostUpdate.Deregister(this, OnGamePostUpdate);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnServerJoin);
                GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Cmds
        private void cmdst(CommandArgs args)
        {
            switch (args.Parameters.Count)
            {
                case 0:
                    isPlrSTVisible[args.Player.Index] = !isPlrSTVisible[args.Player.Index];
                    if (isPlrSTVisible[args.Player.Index])
                    {
                        isPlrNeedInit[args.Player.Index] = true;
                        args.Player.SendSuccessMessage("已开启模板显示");
                    }
                    else
                    {
                        isPlrNeedInit[args.Player.Index] = false;
                        args.Player.SendData(PacketTypes.Status, "", 0, 0x1f);
                        args.Player.SendSuccessMessage("已关闭模板显示");
                    }
                    break;

                case 1:
                    switch (args.Parameters[0])
                    {
                        case "on":
                        case "show":
                            if (!isPlrSTVisible[args.Player.Index])
                            {
                                isPlrSTVisible[args.Player.Index] = true;
                                isPlrNeedInit[args.Player.Index] = true;
                            }
                            args.Player.SendSuccessMessage("已开启模板显示");
                            break;

                        case "off":
                        case "hide":
                            if (isPlrSTVisible[args.Player.Index])
                            {
                                isPlrSTVisible[args.Player.Index] = false;
                                isPlrNeedInit[args.Player.Index] = false;
                                args.Player.SendData(PacketTypes.Status, "", 0, 0x1f);
                            }
                            args.Player.SendSuccessMessage("已关闭模板显示");
                            break;

                        case "help":
                        default:
                            args.Player.SendInfoMessage("用法：/st <on/show/off/hide>");
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region Hooks
        private void OnGameInitialize(EventArgs args)
        {
            // Config Loading
            if (!Directory.Exists(TShock.SavePath))
                Directory.CreateDirectory(TShock.SavePath);
            LoadConfig();
        }

        private void OnReload(ReloadEventArgs args)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                Config.Read(ConfigFilePath, out var incompleteSettings);
                if (incompleteSettings)
                    Config.Write(ConfigFilePath);
                handlerList.LoadSettings();
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to load config, Ex: " + ex);
            }
        }

        private void OnGamePostUpdate(EventArgs args)
        {
            try
            {
                foreach (var tsplr in Utils.Common.PlayersOnline)
                {
                    if (!isPlrSTVisible[tsplr.Index])
                        continue;

                    StringBuilder sb = Utils.StringBuilderCache.Acquire();
                    if (handlerList.Invoke(tsplr, sb, isPlrNeedInit[tsplr.Index]))
                    {
                        tsplr.SendData(PacketTypes.Status, Utils.StringBuilderCache.GetStringAndRelease(sb), 0, 0x1f);
                        // 0x1f -> HideStatusTextPercent
                    }
                    else
                    {
                        Utils.StringBuilderCache.Release(sb);
                    }
                    isPlrNeedInit[tsplr.Index] = false;
                }

                Utils.Common.CountTick();
            }
            catch (Exception ex)
            {
                Logger.Warn("Exception occur in OnGamePostUpdate, Ex: " + ex);
            }
        }

        private void OnServerJoin(JoinEventArgs args)
        {
            isPlrSTVisible[args.Who] = true;
            isPlrNeedInit[args.Who] = true;
        }
        #endregion

        #region Api Shortcut
        public class ApiShortcut
        {
            public StatusTextUpdateHandlerList StatusTextUpdate => handlerList;
        }
        #endregion
    }
}
