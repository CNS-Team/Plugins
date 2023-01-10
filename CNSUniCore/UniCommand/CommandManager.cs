using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Text;
using TShockAPI;
using TShockAPI.DB;

namespace CNSUniCore.UniCommand
{
    // Token: 0x0200000B RID: 11
    public class CommandManager
    {
        // Token: 0x1700001F RID: 31
        // (get) Token: 0x0600006A RID: 106 RVA: 0x00004044 File Offset: 0x00002244
        // (set) Token: 0x0600006B RID: 107 RVA: 0x0000404C File Offset: 0x0000224C
        public List<Command> Commands { get; set; }

        // Token: 0x0600006C RID: 108 RVA: 0x00004058 File Offset: 0x00002258
        public CommandManager()
        {
            this.Commands = new List<Command>();
            this.Commands.Add(new Command("uniban.admin", new CommandDelegate(this.UniBan), new string[]
            {
                "uniban"
            }));
            this.Commands.Add(new Command("univip.admin", new CommandDelegate(this.UniVip), new string[]
            {
                "univip"
            }));
            this.Commands.Add(new Command("unicore.admin", new CommandDelegate(this.UniCore), new string[]
            {
                "unicore"
            }));
            TShockAPI.Commands.ChatCommands.AddRange(this.Commands);
        }

        // Token: 0x0600006D RID: 109 RVA: 0x0000411C File Offset: 0x0000231C
        private void UniCore(CommandArgs args)
        {
            bool flag = args.Parameters.Count < 1;
            if (flag)
            {
                args.Player.SendInfoMessage("请输入/unicore help 查看帮助");
            }
            else
            {
                if (args.Parameters.Count == 0 || args.Parameters[0] == "help")
                {
                    StringBuilder stringBuilder4 = new StringBuilder();
                    stringBuilder4.AppendLine("/unicore list 列出所有服务器");
                    stringBuilder4.AppendLine("/unicore rawcmd 执行命令");
                    stringBuilder4.AppendLine("/unicore ban 开/关封禁功能");
                    stringBuilder4.AppendLine("/unicore register 开/关多服注册");
                    stringBuilder4.AppendLine("/unicore sponsor 开/关赞助系统");
                    stringBuilder4.AppendLine("/unicore reload 重载配置文件");
                    args.Player.SendInfoMessage(stringBuilder4.ToString());
                    return;
                }
                string text = args.Parameters[0];
                string text2 = text;
                switch (text2)
                {
                    case "register":
                        ConfigUtils.config.EnableRegister = !ConfigUtils.config.EnableRegister;
                        args.Player.SendInfoMessage("[UniCore] 多服注册功能 " + (ConfigUtils.config.EnableRegister ? "已开启" : "已关闭"));
                        ConfigUtils.UpdateConfig();
                        break;
                    case "sponsor":
                        ConfigUtils.config.EnableSponsor = !ConfigUtils.config.EnableSponsor;
                        args.Player.SendInfoMessage("[UniCore] 赞助系统功能 " + (ConfigUtils.config.EnableSponsor ? "已开启" : "已关闭"));
                        ConfigUtils.UpdateConfig();
                        break;
                    case "list":
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (ServerInfo serverInfo in ConfigUtils.servers)
                        {
                            StringBuilder stringBuilder2 = stringBuilder;
                            StringBuilder stringBuilder3 = stringBuilder2;
                            StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(6, 3, stringBuilder2);
                            appendInterpolatedStringHandler.AppendLiteral("[");
                            appendInterpolatedStringHandler.AppendFormatted<int>(serverInfo.ID);
                            appendInterpolatedStringHandler.AppendLiteral("][");
                            appendInterpolatedStringHandler.AppendFormatted(serverInfo.IP);
                            appendInterpolatedStringHandler.AppendLiteral("][");
                            appendInterpolatedStringHandler.AppendFormatted<int>(serverInfo.Port);
                            appendInterpolatedStringHandler.AppendLiteral("]");
                            stringBuilder3.AppendLine(ref appendInterpolatedStringHandler);
                        }
                        args.Player.SendInfoMessage(stringBuilder.ToString());
                        break;
                    case "ban":
                        ConfigUtils.config.EnableBan = !ConfigUtils.config.EnableBan;
                        args.Player.SendInfoMessage("[UniCore] 封禁功能 " + (ConfigUtils.config.EnableBan ? "已开启" : "已关闭"));
                        ConfigUtils.UpdateConfig();
                        break;
                    case "rawcmd":
                        bool flag2 = args.Parameters.Count < 2;
                        if (flag2)
                        {
                            args.Player.SendInfoMessage("请输入正确的指令 /unicore rawcmd [服务器ID] [指令]");
                        }
                        else
                        {
                            List<string> range = args.Parameters.GetRange(2, args.Parameters.Count - 1);
                            string text3 = string.Join(" ", range);
                            bool flag3 = !text3.StartsWith("/");
                            if (flag3)
                            {
                                text3 = "/" + text3;
                            }
                            bool flag4 = args.Parameters[1].ToLower() == "all";
                            if (flag4)
                            {
                                foreach (ServerInfo serverInfo2 in ConfigUtils.servers)
                                {
                                    try
                                    {
                                        serverInfo2.CreateToken();
                                        serverInfo2.RawCommand(text3);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            else
                            {
                                int id;
                                bool flag5 = int.TryParse(args.Parameters[1], out id);
                                if (flag5)
                                {
                                    ServerInfo serverInfo3 = ConfigUtils.servers.Find((ServerInfo s) => s.ID == id);
                                    bool flag6 = serverInfo3 != null;
                                    if (flag6)
                                    {
                                        serverInfo3.RawCommand(text3);
                                    }
                                    else
                                    {
                                        args.Player.SendInfoMessage("未找到服务器");
                                    }
                                }
                                else
                                {
                                    args.Player.SendInfoMessage("请输入正确的数字");
                                }
                            }
                        }
                        break;
                    case "reload":
                        ConfigUtils.LoadConfig();
                        args.Player.SendInfoMessage("[UniCore] 插件重载完成");
                        break;
                }
            }
        }
        // Token: 0x0600006E RID: 110 RVA: 0x0000464C File Offset: 0x0000284C
        private void UniVip(CommandArgs args)
        {
            bool flag = args.Parameters.Count < 1;
            if (flag)
            {
                args.Player.SendInfoMessage("输入/univip help 查看赞助系统帮助");
            }
            else
            {
                TSPlayer tsplayer = null;
                string text = args.Parameters[0];
                string a = text;
                if (!(a == "help"))
                {
                    if (!(a == "list"))
                    {
                        if (!(a == "add"))
                        {
                            if (a == "del")
                            {
                                bool flag2 = args.Parameters.Count != 2;
                                if (flag2)
                                {
                                    args.Player.SendInfoMessage("输入/univip del [玩家名]");
                                }
                                else
                                {
                                    string text2 = args.Parameters[1];
                                    SponsorInfo sponsor = MainPlugin.Instance.dbManager.GetSponsor(text2);
                                    bool flag3 = sponsor == null;
                                    if (flag3)
                                    {
                                        args.Player.SendInfoMessage("赞助用户不存在");
                                    }
                                    else
                                    {
                                        sponsor.group = sponsor.originGroup;
                                        sponsor.endTime = DateTime.UtcNow;
                                        List<TSPlayer> list = TSPlayer.FindByNameOrID(text2);
                                        bool flag4 = list.Count != 0;
                                        if (flag4)
                                        {
                                            tsplayer = list[0];
                                        }
                                        bool flag5 = tsplayer != null;
                                        if (flag5)
                                        {
                                            tsplayer.SendInfoMessage("服务器已取消你的赞助权限");
                                            TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsor.originGroup);
                                        }
                                        MainPlugin.Instance.dbManager.UpdateSponsor(sponsor);
                                        foreach (ServerInfo serverInfo in ConfigUtils.servers)
                                        {
                                            try
                                            {
                                                serverInfo.CreateToken();
                                                serverInfo.DelVip(sponsor.name);
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                        args.Player.SendInfoMessage("成功删除赞助玩家");
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool flag6 = args.Parameters.Count != 4;
                            if (flag6)
                            {
                                args.Player.SendInfoMessage("请输入/univip add [玩家名] [目标组别] [天数]");
                            }
                            else
                            {
                                string text3 = args.Parameters[1];
                                string text4 = args.Parameters[2];
                                SponsorInfo sponsorInfo = MainPlugin.Instance.dbManager.GetSponsor(text3);
                                int num;
                                bool flag7 = int.TryParse(args.Parameters[3], out num);
                                if (flag7)
                                {
                                    bool flag8 = sponsorInfo != null;
                                    if (flag8)
                                    {
                                        sponsorInfo.group = text4;
                                        sponsorInfo.endTime.AddDays((double)num);
                                        MainPlugin.Instance.dbManager.UpdateSponsor(sponsorInfo);
                                    }
                                    else
                                    {
                                        UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(text3);
                                        bool flag9 = userAccountByName == null;
                                        if (flag9)
                                        {
                                            args.Player.SendInfoMessage("用户不存在");
                                            return;
                                        }
                                        sponsorInfo = new SponsorInfo(text3, userAccountByName.Group, text4, DateTime.UtcNow, DateTime.UtcNow.AddDays((double)num));
                                        MainPlugin.Instance.dbManager.AddSponsor(sponsorInfo);
                                    }
                                    List<TSPlayer> list2 = TSPlayer.FindByNameOrID(text3);
                                    bool flag10 = list2.Count != 0;
                                    if (flag10)
                                    {
                                        tsplayer = list2[0];
                                    }
                                    bool flag11 = tsplayer != null;
                                    if (flag11)
                                    {
                                        TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsorInfo.group);
                                    }
                                    foreach (ServerInfo serverInfo2 in ConfigUtils.servers)
                                    {
                                        try
                                        {
                                            serverInfo2.CreateToken();
                                            serverInfo2.AddVip(sponsorInfo);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    TSPlayer player = args.Player;
                                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 3);
                                    defaultInterpolatedStringHandler.AppendLiteral("成功添加赞助用户 [");
                                    defaultInterpolatedStringHandler.AppendFormatted(text3);
                                    defaultInterpolatedStringHandler.AppendLiteral("] [");
                                    defaultInterpolatedStringHandler.AppendFormatted<int>(num);
                                    defaultInterpolatedStringHandler.AppendLiteral("]天 [");
                                    defaultInterpolatedStringHandler.AppendFormatted(text4);
                                    defaultInterpolatedStringHandler.AppendLiteral("]组");
                                    player.SendInfoMessage(defaultInterpolatedStringHandler.ToStringAndClear());
                                }
                                else
                                {
                                    args.Player.SendInfoMessage("请输入正确的数字");
                                }
                            }
                        }
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (SponsorInfo sponsorInfo2 in MainPlugin.Instance.dbManager.GetSponsors())
                        {
                            StringBuilder stringBuilder2 = stringBuilder;
                            StringBuilder stringBuilder3 = stringBuilder2;
                            StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(19, 3, stringBuilder2);
                            appendInterpolatedStringHandler.AppendLiteral("[");
                            appendInterpolatedStringHandler.AppendFormatted(sponsorInfo2.name);
                            appendInterpolatedStringHandler.AppendLiteral("] 赞助组别:[");
                            appendInterpolatedStringHandler.AppendFormatted(sponsorInfo2.group);
                            appendInterpolatedStringHandler.AppendLiteral("] 剩余天数:[");
                            appendInterpolatedStringHandler.AppendFormatted<int>((sponsorInfo2.endTime - sponsorInfo2.startTime).Days);
                            appendInterpolatedStringHandler.AppendLiteral("] ");
                            stringBuilder3.AppendLine(ref appendInterpolatedStringHandler);
                        }
                        args.Player.SendInfoMessage(stringBuilder.ToString());
                    }
                }
                else
                {
                    StringBuilder stringBuilder4 = new StringBuilder();
                    stringBuilder4.AppendLine("/univip list 列出所有成员");
                    stringBuilder4.AppendLine("/univip add [玩家名] [目标组别] [天数] 添加赞助信息");
                    stringBuilder4.AppendLine("/univip del [玩家名] 删除赞助信息");
                    args.Player.SendInfoMessage(stringBuilder4.ToString());
                }
            }
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00004BEC File Offset: 0x00002DEC
        private async void UniBan(CommandArgs args)
        {
            bool flag = args.Parameters.Count < 1;
            if (flag)
            {
                args.Player.SendInfoMessage("请输入/uniban help 查看帮助");
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                string text = args.Parameters[0];
                string a = text;
                if (!(a == "add"))
                {
                    if (!(a == "del"))
                    {
                        if (!(a == "list"))
                        {
                            if (!(a == "reload"))
                            {
                                stringBuilder.AppendLine("/uniban help 查看帮助");
                                stringBuilder.AppendLine("/uniban add [用户名]  <时间> 封禁玩家");
                                stringBuilder.AppendLine("/uniban del [用户名] 移除玩家");
                                stringBuilder.AppendLine("/uniban list [页码] 列出指定页封禁玩家");
                                stringBuilder.AppendLine("/uniban reload 重载配置文件");
                                args.Player.SendInfoMessage(stringBuilder.ToString());
                            }
                            else
                            {
                                ConfigUtils.LoadConfig();
                                args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 插件重载成功");
                            }
                        }
                        else
                        {
                            args.Player.SendInfoMessage("——流光封禁列表——");
                            List<PlayerInfo> players = MainPlugin.Instance.dbManager.GetPlayers();
                            //int i = 0;
                            int s;
                            int y;
                            if (args.Parameters.Count <= 1)
                            {
                                y = 1;
                                s = 0;
                            }
                            else
                            {
                                if (int.TryParse(args.Parameters[1], out y))
                                {
                                    s = y - 1;
                                }
                                else
                                {
                                    args.Player.SendErrorMessage("这页没有封禁的玩家");
                                    return;
                                }
                            }
                            if (y <= 0)
                            {
                                args.Player.SendErrorMessage("这页没有封禁的玩家");
                                return;
                            }
                            int i = 0;
                            for (s = 20 * s; s < players.Count; s++, i++)
                            {
                                if (i >= 20)
                                {
                                    if (players.Count >= s + 1)
                                    {
                                        args.Player.SendInfoMessage($"输入\"/uniban list {y + 1}\"查看下一页");
                                        return;
                                    }
                                    return;
                                }
                                else
                                {
                                    i++;

                                    string BanTime = players[s].BanTime;
                                    if (BanTime == "-1") { BanTime = "永久"; }
                                    else
                                    {

                                        var c = DateTime.Now - DateTime.Parse(players[s].AddTime);
                                        int ti = 0;
                                        int ti2 = 0;
                                        int ti3 = 0;
                                        int ti4 = 0;
                                        string[] time = BanTime.Split(":");
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
                                            string text2 = players[s].Name;
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
                                            }
                                        }
                                    }
                                    args.Player.SendMessage($"{i}.{players[s].Name} 时长: {BanTime} 原因: {players[s].Reason}", 255, 255, 255);

                                }
                            }
                            args.Player.SendInfoMessage("已展示所有封禁玩家");
                        }
                    }
                    else
                    {
                        bool flag2 = args.Parameters.Count != 2;
                        if (flag2)
                        {
                            args.Player.SendInfoMessage("正确用法 /uniban del [name]");
                        }
                        else
                        {
                            string text2 = args.Parameters[1];
                            bool flag3 = MainPlugin.Instance.dbManager.GetPlayer(text2) == null;
                            if (!flag3)
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
                                args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 玩家 " + text2 + " 已全服解ban");
                            }
                        }
                    }
                }
                else
                {
                    bool flag4 = args.Parameters.Count < 2;
                    if (flag4)
                    {
                        args.Player.SendInfoMessage("正确用法 /uniban add [name] <时间>");
                    }
                    else
                    {
                        if (args.Parameters.Count >= 3)
                        {
                            string name = args.Parameters[1];
                            UserAccount userAccountByName2 = TShock.UserAccounts.GetUserAccountByName(name);
                            if (userAccountByName2 != null)
                            {
                                int ti;
                                int ti2 = 0;
                                int ti3 = 0;
                                int ti4;
                                string BanTime = args.Parameters[2];
                                if (BanTime.Contains(":"))
                                {

                                    string[] Time = BanTime.Split(":");
                                    int.TryParse(Time[0], out ti);
                                    if (Time.Count() >= 2)
                                    {
                                        if (int.TryParse(Time[1], out ti2))
                                        {
                                            if (ti2 >= 0 && ti2 <= 24)
                                            {
                                                BanTime = $"{ti}:{ti2}:00:00";
                                            }
                                            else
                                            {
                                                args.Player.SendInfoMessage("时间格式错误！");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            args.Player.SendInfoMessage("时间格式错误！");
                                            return;
                                        }

                                    }
                                    if (Time.Count() >= 3)
                                    {
                                        if (int.TryParse(Time[2], out ti3))
                                        {
                                            if (ti3 >= 0 && ti3 <= 60)
                                            {
                                                BanTime = $"{ti}:{ti2}:{ti3}:00";
                                            }
                                            else
                                            {
                                                args.Player.SendInfoMessage("时间格式错误！");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            args.Player.SendInfoMessage("时间格式错误！");
                                            return;
                                        }

                                    }
                                    if (Time.Count() >= 4)
                                    {
                                        if (int.TryParse(Time[3], out ti4))
                                        {
                                            if (ti4 >= 0 && ti4 <= 60)
                                            {
                                                BanTime = $"{ti}:{ti2}:{ti3}:{ti4}";
                                            }
                                            else
                                            {
                                                args.Player.SendInfoMessage("时间格式错误！");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            args.Player.SendInfoMessage("时间格式错误！");
                                            return;
                                        }

                                    }
                                }
                                else
                                {
                                    if (int.TryParse(args.Parameters[2], out ti))
                                    {
                                        if (ti == -1)
                                        {
                                            BanTime = "-1";
                                        }
                                        else if (ti > 0)
                                        {
                                            BanTime = $"{ti}:00:00:00";
                                        }
                                        else
                                        {
                                            args.Player.SendInfoMessage("时间格式错误！");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        args.Player.SendInfoMessage("时间格式错误！");
                                        return;
                                    }
                                }
                                //TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
                                //得到精确到秒的时间戳（长度10位）
                                //long ts = (long)mTimeSpan.TotalSeconds;
                                string ts = Convert.ToString(DateTime.Now.ToString());
                                bool flag6 = MainPlugin.Instance.dbManager.GetPlayer(name) != null;
                                string Reason = "";
                                if (args.Parameters.Count >= 4)
                                {
                                    Reason = args.Parameters[3];
                                }
                                if (flag6)
                                {

                                    MainPlugin.Instance.dbManager.UpdatePlayer(new PlayerInfo
                                    {
                                        Name = name,
                                        IP = userAccountByName2.KnownIps,
                                        UUID = userAccountByName2.UUID,
                                        Reason = Reason,
                                        BanTime = BanTime,
                                        AddTime = ts
                                    });
                                }
                                else
                                {
                                    MainPlugin.Instance.dbManager.AddPlayer(new PlayerInfo
                                    {
                                        Name = name,
                                        IP = userAccountByName2.KnownIps,
                                        UUID = userAccountByName2.UUID,
                                        Reason = Reason,
                                        BanTime = BanTime,
                                        AddTime = ts
                                    });
                                }

                                foreach (ServerInfo serverInfo2 in ConfigUtils.servers)
                                {
                                    try
                                    {
                                        serverInfo2.CreateToken(); 
                                        serverInfo2.BanUser(name, userAccountByName2.ID.ToString(), userAccountByName2.UUID, Reason, BanTime);
                                        //var l = serverInfo2.RawCommand("/kick {name}");
                                        //await Task.Delay(100);
                                       
                                        //string url = $"http://{serverInfo2.IP}:{serverInfo2.Port}/v3/server/rawcmd?token={serverInfo2.Token}&cmd=/kick {name}";
                                        //var l =  serverInfo2.RawCommand("/kick" + name);
          
                                        //args.Player.SendInfoMessage(serverInfo2.Name + l.ToString() + "\n");

                                    }
                                    catch (Exception c)
                                    {
                                        TShock.Log.ConsoleInfo(c.ToString());
                                    }
                                }
                                TShock.Utils.Broadcast("玩家 [" + userAccountByName2.Name + "] 已被全服封禁", Color.MediumAquamarine);
                                args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 成功封禁 " + name);
                                bool flag7 = TSPlayer.FindByNameOrID(name).Count != 0;
                                if (flag7)
                                {
                                    TSPlayer.FindByNameOrID(name)[0].Kick(Reason, false, false, null, false);
                                }
                            }
                            else
                            {
                                args.Player.SendInfoMessage("玩家数据不存在");
                            }




                            return;
                        }
                        string text3 = args.Parameters[1];
                        UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(text3);
                        bool flag5 = userAccountByName != null;
                        if (flag5)
                        {
                            //TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
                            //得到精确到秒的时间戳（长度10位）
                            //long ts = (long)mTimeSpan.TotalSeconds;
                            string ts = DateTime.Now.ToString();
                            bool flag6 = MainPlugin.Instance.dbManager.GetPlayer(text3) != null;
                            if (flag6)
                            {
                                MainPlugin.Instance.dbManager.UpdatePlayer(new PlayerInfo
                                {
                                    Name = text3,
                                    IP = userAccountByName.KnownIps,
                                    UUID = userAccountByName.UUID,
                                    AddTime = ts,
                                    BanTime = "-1"


                                });
                            }
                            else
                            {
                                MainPlugin.Instance.dbManager.AddPlayer(new PlayerInfo
                                {
                                    Name = text3,
                                    IP = userAccountByName.KnownIps,
                                    UUID = userAccountByName.UUID,
                                    AddTime = ts,
                                    BanTime = "-1"
                                });
                            }
                            foreach (ServerInfo serverInfo2 in ConfigUtils.servers)
                            {
                                try
                                {
                                    serverInfo2.CreateToken();
                                    serverInfo2.BanUser(text3, userAccountByName.ID.ToString(), userAccountByName.UUID, "", "-1");

                                    //string url = $"http://{serverInfo2.IP}:{serverInfo2.Port}/v3/server/rawcmd?token={serverInfo2.Token}&cmd=/kick {userAccountByName.Name}";
                                    //JObject.Parse(new HttpClient().GetAsync(url).Result.Content.ReadAsStringAsync().Result);a
                                    //var b = await new HttpClient().GetStringAsync(url);
                                    //string url = $"http://{serverInfo2.IP}:{serverInfo2.Port}/v3/server/rawcmd?token={serverInfo2.Token}&cmd=/kick {userAccountByName.Name}";
                                    //var l =  serverInfo2.RawCommand("/kick" + name);

                                    //var l = serverInfo2.GetHttp(url);
                                    //args.Player.SendInfoMessage(serverInfo2.Name + l.ToString() + "\n");
                                }
                                catch (Exception c)
                                {
                                    TShock.Log.ConsoleInfo(c.ToString());
                                }
                            }
                            TShock.Utils.Broadcast("玩家 [" + userAccountByName.Name + "] 已被全服封禁", Color.MediumAquamarine);
                            args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 成功封禁 " + text3);
                            bool flag7 = TSPlayer.FindByNameOrID(text3).Count != 0;
                            if (flag7)
                            {
                                TSPlayer.FindByNameOrID(text3)[0].Kick("因作弊被封禁", false, false, null, false);
                            }
                        }
                        else
                        {
                            args.Player.SendInfoMessage("玩家数据不存在");
                        }

                    }
                }
            }
        }
    }
}
