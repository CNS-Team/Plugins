using Microsoft.Xna.Framework;
using System.Text;
using TShockAPI;

namespace CNSUniCore.UniCommand;

public class CommandManager
{
    public List<Command> Commands { get; set; }

    public CommandManager()
    {
        this.Commands = new List<Command>
        {
            new Command("uniban.admin", this.UniBan, "uniban"),
            new Command("univip.admin", this.UniVip, "univip"),
            new Command("unicore.admin", this.UniCore, "unicore")
        };
        TShockAPI.Commands.ChatCommands.AddRange(this.Commands);
    }

    private void UniCore(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendInfoMessage("请输入/unicore help 查看帮助");
            return;
        }
        if (args.Parameters.Count == 0 || args.Parameters[0] == "help")
        {
            var stringBuilder4 = new StringBuilder();
            stringBuilder4.AppendLine("/unicore list 列出所有服务器");
            stringBuilder4.AppendLine("/unicore rawcmd 执行命令");
            stringBuilder4.AppendLine("/unicore ban 开/关封禁功能");
            stringBuilder4.AppendLine("/unicore register 开/关多服注册");
            stringBuilder4.AppendLine("/unicore sponsor 开/关赞助系统");
            stringBuilder4.AppendLine("/unicore reload 重载配置文件");
            args.Player.SendInfoMessage(stringBuilder4.ToString());
            return;
        }
        switch (args.Parameters[0])
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
            {
                var stringBuilder = new StringBuilder();
                foreach (var serverInfo in ConfigUtils.servers)
                {
                    stringBuilder.Append($"[{serverInfo.ID}][{serverInfo.IP}][{serverInfo.Port}]");
                }
                args.Player.SendInfoMessage(stringBuilder.ToString());
                break;
            }
            case "ban":
                ConfigUtils.config.EnableBan = !ConfigUtils.config.EnableBan;
                args.Player.SendInfoMessage("[UniCore] 封禁功能 " + (ConfigUtils.config.EnableBan ? "已开启" : "已关闭"));
                ConfigUtils.UpdateConfig();
                break;
            case "rawcmd":
            {
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendInfoMessage("请输入正确的指令 /unicore rawcmd [服务器ID] [指令]");
                    break;
                }
                var text3 = string.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 1));
                if (!text3.StartsWith("/"))
                {
                    text3 = "/" + text3;
                }
                if (args.Parameters[1].ToLower() == "all")
                {
                    foreach (var serverInfo2 in ConfigUtils.servers)
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
                    break;
                }
                if (int.TryParse(args.Parameters[1], out var id))
                {
                    var serverInfo3 = ConfigUtils.servers.Find((ServerInfo s) => s.ID == id);
                    if (serverInfo3 != null)
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
                break;
            }
            case "reload":
                ConfigUtils.LoadConfig();
                args.Player.SendInfoMessage("[UniCore] 插件重载完成");
                break;
        }
    }

    private void UniVip(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendInfoMessage("输入/univip help 查看赞助系统帮助");
            return;
        }
        TSPlayer tsplayer = null;
        switch (args.Parameters[0])
        {
            case "del":
            {
                if (args.Parameters.Count != 2)
                {
                    args.Player.SendInfoMessage("输入/univip del [玩家名]");
                    break;
                }
                var text2 = args.Parameters[1];
                var sponsor = MainPlugin.Instance.dbManager.GetSponsor(text2);
                if (sponsor == null)
                {
                    args.Player.SendInfoMessage("赞助用户不存在");
                    break;
                }
                sponsor.group = sponsor.originGroup;
                sponsor.endTime = DateTime.UtcNow;
                var list = TSPlayer.FindByNameOrID(text2);
                if (list.Count != 0)
                {
                    tsplayer = list[0];
                }
                if (tsplayer != null)
                {
                    tsplayer.SendInfoMessage("服务器已取消你的赞助权限");
                    TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsor.originGroup);
                }
                MainPlugin.Instance.dbManager.UpdateSponsor(sponsor);
                foreach (var serverInfo in ConfigUtils.servers)
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
                break;
            }
            case "add":
            {
                if (args.Parameters.Count != 4)
                {
                    args.Player.SendInfoMessage("请输入/univip add [玩家名] [目标组别] [天数]");
                    break;
                }
                var text3 = args.Parameters[1];
                var text4 = args.Parameters[2];
                var sponsorInfo = MainPlugin.Instance.dbManager.GetSponsor(text3);
                if (int.TryParse(args.Parameters[3], out var num))
                {
                    if (sponsorInfo != null)
                    {
                        sponsorInfo.group = text4;
                        sponsorInfo.endTime.AddDays(num);
                        MainPlugin.Instance.dbManager.UpdateSponsor(sponsorInfo);
                    }
                    else
                    {
                        var userAccountByName = TShock.UserAccounts.GetUserAccountByName(text3);
                        if (userAccountByName == null)
                        {
                            args.Player.SendInfoMessage("用户不存在");
                            break;
                        }
                        sponsorInfo = new SponsorInfo(text3, userAccountByName.Group, text4, DateTime.UtcNow, DateTime.UtcNow.AddDays(num));
                        MainPlugin.Instance.dbManager.AddSponsor(sponsorInfo);
                    }
                    var list2 = TSPlayer.FindByNameOrID(text3);
                    if (list2.Count != 0)
                    {
                        tsplayer = list2[0];
                    }
                    if (tsplayer != null)
                    {
                        TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsorInfo.group);
                    }
                    foreach (var serverInfo2 in ConfigUtils.servers)
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
                    args.Player.SendInfoMessage($"成功添加赞助用户 [{text3}] [{num}]天 [{text4}]组");
                }
                else
                {
                    args.Player.SendInfoMessage("请输入正确的数字");
                }
                break;
            }
            case "list":
            {
                args.Player.SendInfoMessage(string.Join(" ", MainPlugin.Instance.dbManager.GetSponsors().Select(si => $"[{si.name}] 赞助组别:[{si.group}] 剩余天数:[{(si.endTime - si.startTime).Days}]")));
                break;
            }
            case "help":
            {
                var stringBuilder4 = new StringBuilder();
                stringBuilder4.AppendLine("/univip list 列出所有成员");
                stringBuilder4.AppendLine("/univip add [玩家名] [目标组别] [天数] 添加赞助信息");
                stringBuilder4.AppendLine("/univip del [玩家名] 删除赞助信息");
                args.Player.SendInfoMessage(stringBuilder4.ToString());
                break;
            }
        }
    }

    private async void UniBan(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendInfoMessage("请输入/uniban help 查看帮助");
            return;
        }
        var stringBuilder = new StringBuilder();
        switch (args.Parameters[0])
        {
            default:
                stringBuilder.AppendLine("/uniban help 查看帮助");
                stringBuilder.AppendLine("/uniban add [用户名]  <时间> 封禁玩家");
                stringBuilder.AppendLine("/uniban del [用户名] 移除玩家");
                stringBuilder.AppendLine("/uniban list [页码] 列出指定页封禁玩家");
                stringBuilder.AppendLine("/uniban reload 重载配置文件");
                args.Player.SendInfoMessage(stringBuilder.ToString());
                break;
            case "reload":
                ConfigUtils.LoadConfig();
                args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 插件重载成功");
                break;
            case "list":
            {
                args.Player.SendInfoMessage("——流光封禁列表——");
                var players = MainPlugin.Instance.dbManager.GetPlayers();
                int y;
                int s2;
                if (args.Parameters.Count <= 1)
                {
                    y = 1;
                    s2 = 0;
                }
                else
                {
                    if (!int.TryParse(args.Parameters[1], out y))
                    {
                        args.Player.SendErrorMessage("这页没有封禁的玩家");
                        break;
                    }
                    s2 = y - 1;
                }
                if (y <= 0)
                {
                    args.Player.SendErrorMessage("这页没有封禁的玩家");
                    break;
                }
                var j = 0;
                s2 = 20 * s2;
                while (s2 < players.Count)
                {
                    if (j >= 20)
                    {
                        if (players.Count >= s2 + 1)
                        {
                            args.Player.SendInfoMessage($"输入\"/uniban list {y + 1}\"查看下一页");
                        }
                        return;
                    }
                    j++;
                    var BanTime = players[s2].BanTime;
                    if (BanTime == "-1")
                    {
                        BanTime = "永久";
                    }
                    else
                    {
                        var c = DateTime.Now - DateTime.Parse(players[s2].AddTime);
                        var time = BanTime.Split(":");
                        int.TryParse(time[0], out var ti);
                        int.TryParse(time[1], out var ti3);
                        int.TryParse(time[2], out var ti5);
                        int.TryParse(time[3], out var ti7);
                        var cd = ti - c.Days;
                        var ch = ti3 - c.Hours;
                        var cm = ti5 - c.Minutes;
                        var cs = ti7 - c.Seconds;
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
                            var text2 = players[s2].Name;
                            if (MainPlugin.Instance.dbManager.GetPlayer(text2) != null)
                            {
                                MainPlugin.Instance.dbManager.DeletePlayer(text2);
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
                            }
                        }
                    }
                    args.Player.SendMessage($"{j}.{players[s2].Name} 时长: {BanTime} 原因: {players[s2].Reason}", byte.MaxValue, byte.MaxValue, byte.MaxValue);
                    s2++;
                    j++;
                }
                args.Player.SendInfoMessage("已展示所有封禁玩家");
                break;
            }
            case "del":
            {
                if (args.Parameters.Count != 2)
                {
                    args.Player.SendInfoMessage("正确用法 /uniban del [name]");
                    break;
                }
                var text3 = args.Parameters[1];
                if (MainPlugin.Instance.dbManager.GetPlayer(text3) == null)
                {
                    break;
                }
                MainPlugin.Instance.dbManager.DeletePlayer(text3);
                foreach (var serverInfo2 in ConfigUtils.servers)
                {
                    try
                    {
                        serverInfo2.CreateToken();
                        serverInfo2.DelUser(text3);
                    }
                    catch (Exception)
                    {
                    }
                }
                args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 玩家 " + text3 + " 已全服解ban");
                break;
            }
            case "add":
            {
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendInfoMessage("正确用法 /uniban add [name] <时间>");
                    break;
                }
                if (args.Parameters.Count >= 3)
                {
                    var name = args.Parameters[1];
                    var userAccountByName2 = TShock.UserAccounts.GetUserAccountByName(name);
                    if (userAccountByName2 != null)
                    {
                        var ti4 = 0;
                        var ti6 = 0;
                        var BanTime2 = args.Parameters[2];
                        int ti2;
                        if (BanTime2.Contains(":"))
                        {
                            var Time = BanTime2.Split(":");
                            int.TryParse(Time[0], out ti2);
                            if (Time.Count() >= 2)
                            {
                                if (!int.TryParse(Time[1], out ti4))
                                {
                                    args.Player.SendInfoMessage("时间格式错误！");
                                    break;
                                }
                                if (ti4 < 0 || ti4 > 24)
                                {
                                    args.Player.SendInfoMessage("时间格式错误！");
                                    break;
                                }
                                BanTime2 = $"{ti2}:{ti4}:00:00";
                            }
                            if (Time.Count() >= 3)
                            {
                                if (!int.TryParse(Time[2], out ti6))
                                {
                                    args.Player.SendInfoMessage("时间格式错误！");
                                    break;
                                }
                                if (ti6 < 0 || ti6 > 60)
                                {
                                    args.Player.SendInfoMessage("时间格式错误！");
                                    break;
                                }
                                BanTime2 = $"{ti2}:{ti4}:{ti6}:00";
                            }
                            if (Time.Count() >= 4)
                            {
                                if (!int.TryParse(Time[3], out var ti8))
                                {
                                    args.Player.SendInfoMessage("时间格式错误！");
                                    break;
                                }
                                if (ti8 < 0 || ti8 > 60)
                                {
                                    args.Player.SendInfoMessage("时间格式错误！");
                                    break;
                                }
                                BanTime2 = $"{ti2}:{ti4}:{ti6}:{ti8}";
                            }
                        }
                        else
                        {
                            if (!int.TryParse(args.Parameters[2], out ti2))
                            {
                                args.Player.SendInfoMessage("时间格式错误！");
                                break;
                            }
                            if (ti2 == -1)
                            {
                                BanTime2 = "-1";
                            }
                            else
                            {
                                if (ti2 <= 0)
                                {
                                    args.Player.SendInfoMessage("时间格式错误！");
                                    break;
                                }
                                BanTime2 = $"{ti2}:00:00:00";
                            }
                        }
                        var ts2 = Convert.ToString(DateTime.Now.ToString());
                        var flag6 = MainPlugin.Instance.dbManager.GetPlayer(name) != null;
                        var Reason = "";
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
                                BanTime = BanTime2,
                                AddTime = ts2
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
                                BanTime = BanTime2,
                                AddTime = ts2
                            });
                        }
                        foreach (var serverInfo4 in ConfigUtils.servers)
                        {
                            try
                            {
                                serverInfo4.CreateToken();
                                serverInfo4.BanUser(name, userAccountByName2.ID.ToString(), userAccountByName2.UUID, Reason, BanTime2);
                            }
                            catch (Exception ex)
                            {
                                TShock.Log.ConsoleInfo(ex.ToString());
                            }
                        }
                        TShock.Utils.Broadcast("玩家 [" + userAccountByName2.Name + "] 已被全服封禁", Color.MediumAquamarine);
                        args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 成功封禁 " + name);
                        if (TSPlayer.FindByNameOrID(name).Count != 0)
                        {
                            TSPlayer.FindByNameOrID(name)[0].Kick(Reason);
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("玩家数据不存在");
                    }
                    break;
                }
                var text4 = args.Parameters[1];
                var userAccountByName = TShock.UserAccounts.GetUserAccountByName(text4);
                if (userAccountByName != null)
                {
                    var ts = DateTime.Now.ToString();
                    if (MainPlugin.Instance.dbManager.GetPlayer(text4) != null)
                    {
                        MainPlugin.Instance.dbManager.UpdatePlayer(new PlayerInfo
                        {
                            Name = text4,
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
                            Name = text4,
                            IP = userAccountByName.KnownIps,
                            UUID = userAccountByName.UUID,
                            AddTime = ts,
                            BanTime = "-1"
                        });
                    }
                    foreach (var serverInfo3 in ConfigUtils.servers)
                    {
                        try
                        {
                            serverInfo3.CreateToken();
                            serverInfo3.BanUser(text4, userAccountByName.ID.ToString(), userAccountByName.UUID, "", "-1");
                        }
                        catch (Exception ex2)
                        {
                            TShock.Log.ConsoleInfo(ex2.ToString());
                        }
                    }
                    TShock.Utils.Broadcast("玩家 [" + userAccountByName.Name + "] 已被全服封禁", Color.MediumAquamarine);
                    args.Player.SendInfoMessage("[c/55d284:『][c/7ddff8:流][c/81dbf6:光][c/86d7f4:系][c/8ad3f3:统][c/b1d03b:』] 成功封禁 " + text4);
                    if (TSPlayer.FindByNameOrID(text4).Count != 0)
                    {
                        TSPlayer.FindByNameOrID(text4)[0].Kick("因作弊被封禁");
                    }
                }
                else
                {
                    args.Player.SendInfoMessage("玩家数据不存在");
                }
                break;
            }
        }
    }
}