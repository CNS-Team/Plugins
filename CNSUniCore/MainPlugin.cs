using CNSUniCore;
using CNSUniCore.UniCommand;
using CNSUniCore.UniInfos;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace CNSUniCore;
[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public DBManager dbManager;

    public CNSUniCore.UniRest.RestManager restManager;

    public CommandManager commandManager;

    public static MainPlugin Instance { get; private set; }

    public override string Name => "CNSUniCore";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public override string Author => "豆沙";

    public override string Description => "流光统一REST核心";

    public MainPlugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        ConfigUtils.LoadConfig();
        Instance = this;
        this.dbManager = new DBManager(ConfigUtils.LoadDb());
        this.restManager = new CNSUniCore.UniRest.RestManager();
        this.commandManager = new CommandManager();
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPostInit);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnNetGreetPlayer);
        AccountHooks.AccountCreate += this.OnCreateAccount;
        PlayerHooks.PlayerPostLogin += this.OnPlayerLogin;
    }

    private void OnPlayerLogin(PlayerPostLoginEventArgs args)
    {
        if (!ConfigUtils.config.EnableRegister)
        {
            return;
        }
        var info = new UserInfo
        {
            ID = args.Player.Account.ID,
            KnownIPs = args.Player.Account.KnownIps,
            LastAccessed = args.Player.Account.LastAccessed,
            Name = args.Player.Account.Name,
            Password = args.Player.Account.Password,
            Registered = args.Player.Account.Registered,
            UserGroup = args.Player.Account.Group
        };
        Task.Run(delegate
        {
            foreach (var current in ConfigUtils.servers)
            {
                try
                {
                    current.UpdateUser(info);
                }
                catch (Exception)
                {
                }
            }
        });
    }

    private void OnCreateAccount(AccountCreateEventArgs args)
    {
        if (!ConfigUtils.config.EnableRegister)
        {
            return;
        }
        var info = new UserInfo
        {
            ID = args.Account.ID,
            KnownIPs = args.Account.KnownIps,
            LastAccessed = args.Account.LastAccessed,
            Name = args.Account.Name,
            Password = args.Account.Password,
            Registered = args.Account.Registered,
            UserGroup = args.Account.Group
        };
        Task.Run(delegate
        {
            foreach (var current in ConfigUtils.servers)
            {
                try
                {
                    current.AddUser(info);
                }
                catch (Exception)
                {
                }
            }
        });
    }

    private void OnNetGreetPlayer(GreetPlayerEventArgs args)
    {
        var args2 = args;
        Task.Run(delegate
        {
            if (ConfigUtils.config.EnableSponsor)
            {
                var tSPlayer = TShock.Players[args2.Who];
                var sponsor = this.dbManager.GetSponsor(tSPlayer.Name);
                var userAccountByName = TShock.UserAccounts.GetUserAccountByName(tSPlayer.Name);
                tSPlayer.SendSuccessMessage("激活赞助权限中...");
                Task.Delay(3000);
                if (sponsor != null && userAccountByName != null && sponsor.endTime - sponsor.startTime > DateTime.UtcNow - sponsor.startTime)
                {
                    tSPlayer.Group = TShock.Groups.GetGroupByName(sponsor.group);
                    TShock.UserAccounts.SetUserGroup(tSPlayer.Account, sponsor.group);
                    tSPlayer.SendSuccessMessage("成功激活赞助权限");
                }
                else if (sponsor != null && userAccountByName != null && sponsor.endTime - sponsor.startTime <= DateTime.UtcNow - sponsor.startTime)
                {
                    TShock.UserAccounts.SetUserGroup(tSPlayer.Account, sponsor.originGroup);
                    sponsor.group = sponsor.originGroup;
                    tSPlayer.Group = TShock.Groups.GetGroupByName(sponsor.group);
                    this.dbManager.UpdateSponsor(sponsor);
                    tSPlayer.SendSuccessMessage("赞助权限已过期");
                }
            }
        });
    }

    private void OnPostInit(EventArgs args)
    {
        this.dbManager.CreateBansTable();
        this.dbManager.CreateSponsorTable();
    }

    private void OnJoin(JoinEventArgs args)
    {
        var tsplayer = TShock.Players[args.Who];
        if (!this.dbManager.GetPlayers().Exists((PlayerInfo s) => s.Name == tsplayer.Name))
        {
            return;
        }
        var pl = this.dbManager.GetPlayers().Find((PlayerInfo s) => s.Name == tsplayer.Name)!;
        var players = this.dbManager.GetPlayers();
        var list = players.Select((PlayerInfo p) => p.Name).ToList();
        var list2 = players.Select((PlayerInfo p) => p.UUID).ToList();
        var list3 = players.Select((PlayerInfo p) => p.IP).ToList();
        var flag = false;
        var flag2 = list.Contains(tsplayer.Name);
        var BanTime = pl.BanTime;
        if (flag2)
        {
            if (pl.BanTime == "-1")
            {
                BanTime = "永久";
            }
            else
            {
                var time = pl.BanTime.Split(":");
                var c = DateTime.Now - DateTime.Parse(pl.AddTime);
                int.TryParse(time[0], out var ti);
                int.TryParse(time[1], out var ti2);
                int.TryParse(time[2], out var ti3);
                int.TryParse(time[3], out var ti4);
                var cd = ti - c.Days;
                var ch = ti2 - c.Hours;
                var cm = ti3 - c.Minutes;
                var cs = ti4 - c.Seconds;
                if (cd > 1)
                {
                    BanTime = $"{cd - 1}天";
                }
                else if (ch > 1)
                {
                    BanTime = $"{ch - 1}h";
                }
                else if (cm > 1)
                {
                    BanTime = $"{cm - 1}min";
                }
                else if (cs > 1)
                {
                    BanTime = $"{cs}s";
                }
                else
                {
                    var text2 = tsplayer.Name;
                    if (Instance.dbManager.GetPlayer(text2) != null)
                    {
                        Instance.dbManager.DeletePlayer(text2);
                        foreach (var serverInfo in ConfigUtils.servers)
                        {
                            try
                            {
                                serverInfo.CreateToken();
                                serverInfo.DelUser(text2);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        TShock.Log.ConsoleInfo("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 玩家 " + text2 + " 已全服解ban");
                        return;
                    }
                }
            }
            flag = true;
            TShock.Log.ConsoleInfo($"玩家 {tsplayer.Name} 处于名称封禁列表中");
        }
        if (list2.Contains(tsplayer.UUID))
        {
            flag = true;
            TShock.Log.ConsoleInfo($"玩家 {tsplayer.Name} ({tsplayer.UUID}) 处于UUID封禁列表中");
        }
        if (list3.Contains(tsplayer.IP))
        {
            flag = true;
            TShock.Log.ConsoleInfo($"玩家 {tsplayer.Name} ({tsplayer.IP}) 处于IP封禁列表中");
        }
        if (flag)
        {
            tsplayer.Disconnect("————流光系统———\n封禁时间：" + BanTime + "\n封禁原因：" + pl.Reason);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.dbManager = null;
            this.commandManager = null;
            this.restManager = null;
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnPostInit);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnNetGreetPlayer);
        }
        base.Dispose(disposing);
    }
}