using LazyUtils;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace OnlineInfo;

[ApiVersion(2, 1)]
public class OnlineInfoPlugin : LazyPlugin
{
    #region Info
    public override string Name => "OnlineInfo";
    public override string Author => "LaoSparrow";
    public override string Description => "保存及获取多服务器在线玩家信息";
    #endregion

    #region Init / Dispose
    public OnlineInfoPlugin(Main game) : base(game)
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnGameInitialize);
        PlayerHooks.PlayerPostLogin += this.OnPlayerPostLogin;
        PlayerHooks.PlayerLogout += this.OnPlayerLogout;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnGameInitialize);
            PlayerHooks.PlayerPostLogin -= this.OnPlayerPostLogin;
            PlayerHooks.PlayerLogout -= this.OnPlayerLogout;
        }
        base.Dispose(disposing);
    }
    #endregion

    #region Fields
    internal static bool isAutoUpdateEnabled = false;
    #endregion

    #region Hooks
    private void OnGameInitialize(EventArgs args)
    {
        isAutoUpdateEnabled = OIConfig.Instance.AutoUpdate;
        try
        {
            using var dbconn = Utils.GetDBConnection();
            dbconn.CreateTable<DBModel.OnlineInfo>(tableOptions: TableOptions.CreateIfNotExists);
        }
        catch (Exception ex) { Logger.ConsoleError($"Failed to create table, Ex: {ex}"); }
    }

    private void OnPlayerPostLogin(PlayerPostLoginEventArgs args)
    {
        if (isAutoUpdateEnabled)
        {
            UpdateOnlineInfo(args.Player, true);
        }
    }

    private void OnPlayerLogout(PlayerLogoutEventArgs args)
    {
        if (isAutoUpdateEnabled)
        {
            UpdateOnlineInfo(args.Player, false);
        }
    }
    #endregion

    #region Methods
    internal static bool UpdateOnlineInfo(TSPlayer player = null, bool isJoining = true)
    {
        try
        {
            var playerNames = TShock.Players
                .Where(x => x != null && x.Active && !string.IsNullOrEmpty(x.Account?.Name))
                .Select(x => x.Account.Name)
                .ToList();
            if (!string.IsNullOrEmpty(player?.Account?.Name))
            {
                if (isJoining)
                {
                    playerNames.Add(player.Account.Name);
                }
                else
                {
                    playerNames.RemoveAll(x => x == player.Account.Name);
                }
            }
            playerNames = playerNames.Distinct().ToList();

            using var db = Utils.GetDBConnection();
            var table = db.GetTable<DBModel.OnlineInfo>();
            table.InsertOrUpdate(() => new DBModel.OnlineInfo
            {
                ServerId = OIConfig.Instance.ServerID,
                ServerName = OIConfig.Instance.ServerName,
                Players = string.Join(",", playerNames),
                PlayerCount = playerNames.Count
            }, x => new DBModel.OnlineInfo
            {
                ServerName = OIConfig.Instance.ServerName,
                Players = string.Join(",", playerNames),
                PlayerCount = playerNames.Count
            });
            return true;
        }
        catch (Exception ex) { Logger.ConsoleError($"Failed to update online info, Ex: {ex}"); }
        return false;
    }
    #endregion
}