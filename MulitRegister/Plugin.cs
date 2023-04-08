using System;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace MulitRegister;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "MulitRegister";

    public override string Author => "Leader";

    public override Version Version => new Version(1, 0, 0, 0);

    public Plugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Config.GetConfig();
        Data.Init();
        ServerApi.Hooks.ServerJoin.Register((TerrariaPlugin) (object) this, (HookHandler<JoinEventArgs>) this.OnServerJoin);
    }

    private void OnServerJoin(JoinEventArgs args)
    {
        var val = TShock.Players[args.Who];
        if (val.IsLoggedIn)
        {
            return;
        }
        var userAccountByName = TShock.UserAccounts.GetUserAccountByName(val.Name);
        if (userAccountByName == (UserAccount) null)
        {
            if (Data.AddAccountFromHost(val.Name))
            {
                Commands.HandleCommand(val, "/login");
            }
        }
        else if (Data.UpdateAccount(val.Name))
        {
            Commands.HandleCommand(val, "/login");
        }
        else
        {
            Commands.HandleCommand((TSPlayer) (object) TSPlayer.Server, "/user del " + val.Name);
        }
    }
}