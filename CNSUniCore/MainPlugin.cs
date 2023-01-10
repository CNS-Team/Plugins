using CNSUniCore.UniCommand;
using CNSUniCore.UniInfos;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CNSUniCore
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {        public static MainPlugin Instance
        {
            get
            {
                return MainPlugin.instace;
            }
        }
        public override string Name
        {
            get
            {
                return "CNSUniCore";
            }
        }
        public override Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        public override string Author
        {
            get
            {
                return "豆沙";
            }
        }
        public override string Description
        {
            get
            {
                return "流光统一REST核心";
            }
        }

        public MainPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ConfigUtils.LoadConfig();
            MainPlugin.instace = this;
            this.dbManager = new DBManager(ConfigUtils.LoadDb());
            this.restManager = new UniRest.RestManager();
            this.commandManager = new CommandManager();
            ServerApi.Hooks.ServerJoin.Register(this, new HookHandler<JoinEventArgs>(this.OnJoin));
            ServerApi.Hooks.GamePostInitialize.Register(this, new HookHandler<EventArgs>(this.OnPostInit));
            ServerApi.Hooks.NetGreetPlayer.Register(this, new HookHandler<GreetPlayerEventArgs>(this.OnNetGreetPlayer));
            AccountHooks.AccountCreate += new AccountHooks.AccountCreateD(this.OnCreateAccount);
            PlayerHooks.PlayerPostLogin += new PlayerHooks.PlayerPostLoginD(this.OnPlayerLogin);
        }

        private void OnPlayerLogin(PlayerPostLoginEventArgs args)
        {
            bool flag = !ConfigUtils.config.EnableRegister;
            if (!flag)
            {
                UserInfo info = new UserInfo();
                info.ID = args.Player.Account.ID;
                info.KnownIPs = args.Player.Account.KnownIps;
                info.LastAccessed = args.Player.Account.LastAccessed;
                info.Name = args.Player.Account.Name;
                info.Password = args.Player.Account.Password;
                info.Registered = args.Player.Account.Registered;
                info.UserGroup = args.Player.Account.Group;
                Task.Run(delegate ()
                {
                    foreach (ServerInfo serverInfo in ConfigUtils.servers)
                    {
                        try
                        {
                            serverInfo.UpdateUser(info);
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
            }
        }

        private void OnCreateAccount(AccountCreateEventArgs args)
        {
            bool flag = !ConfigUtils.config.EnableRegister;
            if (!flag)
            {
                UserInfo info = new UserInfo();
                info.ID = args.Account.ID;
                info.KnownIPs = args.Account.KnownIps;
                info.LastAccessed = args.Account.LastAccessed;
                info.Name = args.Account.Name;
                info.Password = args.Account.Password;
                info.Registered = args.Account.Registered;
                info.UserGroup = args.Account.Group;
                Task.Run(delegate ()
                {
                    foreach (ServerInfo serverInfo in ConfigUtils.servers)
                    {
                        try
                        {
                            serverInfo.AddUser(info);
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
            }
        }

        private void OnNetGreetPlayer(GreetPlayerEventArgs args)
        {
            Task.Run(delegate ()
            {
                bool enableSponsor = ConfigUtils.config.EnableSponsor;
                if (enableSponsor)
                {
                    TSPlayer tsplayer = TShock.Players[args.Who];
                    SponsorInfo sponsor = this.dbManager.GetSponsor(tsplayer.Name);
                    UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(tsplayer.Name);
                    tsplayer.SendSuccessMessage("激活赞助权限中...");
                    Task.Delay(3000);
                    bool flag = sponsor != null && userAccountByName != null && sponsor.endTime - sponsor.startTime > DateTime.UtcNow - sponsor.startTime;
                    if (flag)
                    {
                        tsplayer.Group = TShock.Groups.GetGroupByName(sponsor.group);
                        TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsor.group);
                        tsplayer.SendSuccessMessage("成功激活赞助权限");
                    }
                    else
                    {
                        bool flag2 = sponsor != null && userAccountByName != null && sponsor.endTime - sponsor.startTime <= DateTime.UtcNow - sponsor.startTime;
                        if (flag2)
                        {
                            TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsor.originGroup);
                            sponsor.group = sponsor.originGroup;
                            tsplayer.Group = TShock.Groups.GetGroupByName(sponsor.group);
                            this.dbManager.UpdateSponsor(sponsor);
                            tsplayer.SendSuccessMessage("赞助权限已过期");
                        }
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

            TSPlayer tsplayer = TShock.Players[args.Who];

            if (!this.dbManager.GetPlayers().Exists(s => s.Name == tsplayer.Name))
            {
                return;
            }
            var pl = this.dbManager.GetPlayers().Find(s => s.Name == tsplayer.Name);
            List<PlayerInfo> players = this.dbManager.GetPlayers();
            List<string> list = (from p in players
                                 select p.Name).ToList<string>();
            List<string> list2 = (from p in players
                                  select p.UUID).ToList<string>();
            List<string> list3 = (from p in players
                                  select p.IP).ToList<string>();
            bool flag = false;
            bool flag2 = list.Contains(tsplayer.Name);
            string BanTime = pl.BanTime;
            if (flag2)
            {

                if (pl.BanTime == "-1")
                {

                    BanTime = $"永久";
                }
                else
                {
                    string[] time = pl.BanTime.Split(":");
                    var c = DateTime.Now - DateTime.Parse(pl.AddTime);
                    int ti = 0;
                    int ti2 = 0;
                    int ti3 = 0;
                    int ti4 = 0;
                    int.TryParse(time[0], out ti);
                    int.TryParse(time[1], out ti2);
                    int.TryParse(time[2], out ti3);
                    int.TryParse(time[3], out ti4);
                    int cd = ti - c.Days;
                    int ch = ti2 - c.Hours;
                    int cm = ti3 - c.Minutes;
                    int cs = ti4 - c.Seconds;
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
                        string text2 = tsplayer.Name;
                        bool flag9 = MainPlugin.Instance.dbManager.GetPlayer(text2) == null;
                        if (!flag9)
                        {
                            MainPlugin.Instance.dbManager.DeletePlayer(text2);
                            foreach (ServerInfo serverInfo in ConfigUtils.servers)
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
                            //args.Player.SendInfoMessage("[Uniban] 玩家 " + text2 + " 已全服解ban");
                            TShock.Log.ConsoleInfo("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 玩家 " + text2 + " 已全服解ban");
                            return;
                        }
                    }


                }
                flag = true;
                TShock.Log.ConsoleInfo("玩家 " + tsplayer.Name + " 处于名称封禁列表中");

            }
            bool flag3 = list2.Contains(tsplayer.UUID);
            if (flag3)
            {
                flag = true;
                TShock.Log.ConsoleInfo(string.Concat(new string[]
                {
                    "玩家 ",
                    tsplayer.Name,
                    " (",
                    tsplayer.UUID,
                    ") 处于UUID封禁列表中"
                }));
            }
            bool flag4 = list3.Contains(tsplayer.IP);
            if (flag4)
            {
                flag = true;
                TShock.Log.ConsoleInfo(string.Concat(new string[]
                {
                    "玩家 ",
                    tsplayer.Name,
                    " (",
                    tsplayer.IP,
                    ") 处于IP封禁列表中"
                }));
            }
            bool flag5 = flag;
            if (flag5)
            {
                tsplayer.Disconnect($"————流光系统———\n" +
                                    $"封禁时间：{BanTime}\n" +
                                    $"封禁原因：{pl.Reason}");
            }
        }

        // Token: 0x0600001F RID: 31 RVA: 0x00002EFC File Offset: 0x000010FC
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dbManager = null;
                this.commandManager = null;
                this.restManager = null;
                ServerApi.Hooks.ServerJoin.Deregister(this, new HookHandler<JoinEventArgs>(this.OnJoin));
                ServerApi.Hooks.GamePostInitialize.Deregister(this, new HookHandler<EventArgs>(this.OnPostInit));
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, new HookHandler<GreetPlayerEventArgs>(this.OnNetGreetPlayer));
            }
            base.Dispose(disposing);
        }
        private static MainPlugin instace;
        public DBManager dbManager;
        public UniRest.RestManager restManager;
        public CommandManager commandManager;
    }
}
