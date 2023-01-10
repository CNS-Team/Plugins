using Microsoft.Xna.Framework;
using Rests;
using TShockAPI;
using TShockAPI.DB;

namespace CNSUniCore.UniRest;

public class RestManager
{
    public class player
    {
        public string Name = "";

        public string Reason = "";

        public string Time = "";
    }

    public DBManager dbManager;

    public RestManager()
    {
        TShock.RestApi.Register(new SecureRestCommand("/uniban/add", this.AddBan, "uniban.admin.add"));
        TShock.RestApi.Register(new SecureRestCommand("/uniban/del", this.DelBan, "uniban.admin.del"));
        TShock.RestApi.Register(new SecureRestCommand("/uniban/list", this.ListBan, "uniban.admin.list"));
        TShock.RestApi.Register(new SecureRestCommand("/uniban/broadcast", this.Broadcast, "uniban.admin.bc"));
        TShock.RestApi.Register(new SecureRestCommand("/univip/add", this.UniVipAdd, "univip.admin.add"));
        TShock.RestApi.Register(new SecureRestCommand("/univip/del", this.UniVipDel, "univip.admin.del"));
        TShock.RestApi.Register(new SecureRestCommand("/univip/adduser", this.UniUserAdd, "unireg.admin.add"));
        TShock.RestApi.Register(new SecureRestCommand("/univip/updateuser", this.UniUserUpdate, "unireg.admin.update"));
    }

    private object UniUserUpdate(RestRequestArgs args)
    {
        var num = int.Parse(args.Parameters["id"]);
        var text = args.Parameters["name"];
        var text2 = args.Parameters["group"];
        var text3 = args.Parameters["uuid"];
        var text4 = args.Parameters["ip"];
        var text5 = args.Parameters["registered"];
        var text6 = args.Parameters["lastaccessed"];
        var userAccountByName = TShock.UserAccounts.GetUserAccountByName(text);
        if (userAccountByName != null)
        {
            userAccountByName.ID = num;
            userAccountByName.Name = text;
            userAccountByName.Group = text2;
            userAccountByName.UUID = text3;
            userAccountByName.KnownIps = text4;
            userAccountByName.Registered = text5;
            userAccountByName.LastAccessed = text6;
            DbExt.Query(query: $"UPDATE Users SET ID={num},Username='{text}',UUID='{text3}',Usergroup='{text2}',Registered='{text5}',LastAccessed='{text6}',KnownIPs='{text4}' WHERE Username='{text}';", olddb: TShock.DB, args: Array.Empty<object>());
        }
        return null;
    }

    private object UniUserAdd(RestRequestArgs args)
    {
        var id = int.Parse(args.Parameters["id"]);
        var account = new UserAccount(args.Parameters["name"], args.Parameters["password"], group: args.Parameters["group"], uuid: args.Parameters["uuid"], known: args.Parameters["ip"], registered: args.Parameters["registered"], last: args.Parameters["lastaccessed"])
        {
            ID = id
        };
        TShock.UserAccounts.AddUserAccount(account);
        return null;
    }

    private object UniVipDel(RestRequestArgs args)
    {
        if (!ConfigUtils.config.EnableSponsor)
        {
            return null;
        }
        var text = args.Parameters["name"];
        var list = TSPlayer.FindByNameOrID(text);
        TSPlayer tsplayer = null;
        var sponsor = MainPlugin.Instance.dbManager.GetSponsor(text);
        if (sponsor != null)
        {
            sponsor.group = sponsor.originGroup;
            sponsor.endTime = DateTime.UtcNow;
            MainPlugin.Instance.dbManager.UpdateSponsor(sponsor);
            if (list.Count != 0)
            {
                tsplayer = list[0];
            }
            if (tsplayer != null)
            {
                tsplayer.SendInfoMessage("服务器已取消你的赞助权限");
                TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsor.originGroup);
            }
        }
        return null;
    }

    private object UniVipAdd(RestRequestArgs args)
    {
        if (!ConfigUtils.config.EnableSponsor)
        {
            return null;
        }
        var text = args.Parameters["name"];
        var group = args.Parameters["group"];
        var text2 = args.Parameters["origin"];
        var s = args.Parameters["start"];
        var s2 = args.Parameters["end"];
        var list = TSPlayer.FindByNameOrID(text);
        TSPlayer tsplayer = null;
        var sponsorInfo = MainPlugin.Instance.dbManager.GetSponsor(text);
        if (sponsorInfo != null)
        {
            sponsorInfo.group = group;
            sponsorInfo.originGroup = text2;
            sponsorInfo.endTime.Add(DateTime.Parse(s2).Subtract(sponsorInfo.endTime));
            MainPlugin.Instance.dbManager.UpdateSponsor(sponsorInfo);
        }
        else
        {
            sponsorInfo = new SponsorInfo(text, text2, group, DateTime.Parse(s), DateTime.Parse(s2));
            MainPlugin.Instance.dbManager.AddSponsor(sponsorInfo);
        }
        if (list.Count != 0)
        {
            tsplayer = list[0];
        }
        if (tsplayer != null)
        {
            TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsorInfo.group);
        }
        return null;
    }

    private object Broadcast(RestRequestArgs args)
    {
        var text = args.Parameters["text"];
        TShock.Utils.Broadcast(text, Color.DarkTurquoise);
        return null;
    }

    private object ListBan(RestRequestArgs args)
    {
        var players = this.dbManager.GetPlayers();
        var player = new List<player>();
        for (var i = 0; i < players.Count; i++)
        {
            var w = players[i];
            player.Add(new player
            {
                Name = w.Name,
                Reason = w.Reason,
                Time = w.BanTime
            });
        }
        return new RestObject("200") { { "back", player } };
    }

    private object DelBan(RestRequestArgs args)
    {
        if (!ConfigUtils.config.EnableBan)
        {
            return null;
        }
        var text = args.Parameters["name"];
        if (MainPlugin.Instance.dbManager.GetPlayer(text) != null)
        {
            MainPlugin.Instance.dbManager.DeletePlayer(text);
        }
        return null;
    }

    private object AddBan(RestRequestArgs args)
    {
        if (!ConfigUtils.config.EnableBan)
        {
            return new RestObject("402") { { "error", "发生错误" } };
        }
        var text = args.Parameters["name"];
        var ip = args.Parameters["ip"];
        var uuid = args.Parameters["uuid"];
        var reason = args.Parameters["reason"];
        var time = args.Parameters["time"];
        var name = text;
        var userAccountByName2 = TShock.UserAccounts.GetUserAccountByName(name);
        Commands.HandleCommand(TSPlayer.Server, "/kick " + name);
        if (userAccountByName2 != null)
        {
            var ti = 0;
            var ti2 = 0;
            var ti3 = 0;
            var ti4 = 0;
            var BanTime = time;
            if (BanTime.Contains(":"))
            {
                var Time = BanTime.Split(":");
                if (!int.TryParse(Time[0], out ti) || !int.TryParse(Time[1], out ti2) || !int.TryParse(Time[2], out ti3) || !int.TryParse(Time[3], out ti4))
                {
                    return new RestObject("401") { { "error", "时间格式错误" } };
                }
                if (ti2 < 0)
                {
                    return new RestObject("401") { { "error", "时间格式错误" } };
                }
                BanTime = $"{ti}:{ti2}:00:00";
            }
            else
            {
                if (!int.TryParse(time, out ti))
                {
                    return new RestObject("401") { { "error", "时间格式错误" } };
                }
                if (ti == -1)
                {
                    BanTime = "-1";
                }
                else
                {
                    if (ti <= 0)
                    {
                        return new RestObject("401") { { "error", "时间格式错误" } };
                    }
                    BanTime = $"{ti}:00:00:00";
                }
            }
            var ts = DateTime.Now.ToString();
            if (TSPlayer.FindByNameOrID(name).Count != 0)
            {
                TSPlayer.FindByNameOrID(name)[0].Kick("因作弊被封禁");
            }
            if (MainPlugin.Instance.dbManager.GetPlayer(text) != null)
            {
                MainPlugin.Instance.dbManager.UpdatePlayer(new PlayerInfo
                {
                    Name = text,
                    IP = ip,
                    UUID = uuid,
                    AddTime = ts,
                    Reason = reason,
                    BanTime = BanTime
                });
            }
            else
            {
                MainPlugin.Instance.dbManager.AddPlayer(new PlayerInfo
                {
                    Name = text,
                    IP = ip,
                    UUID = uuid,
                    AddTime = ts,
                    Reason = reason,
                    BanTime = BanTime
                });
            }
            if (TSPlayer.FindByNameOrID(name).Count != 0)
            {
                TSPlayer.FindByNameOrID(name)[0].Kick("因作弊被封禁");
            }
            return new RestObject("200") { { "back", "成功" } };
        }
        return new RestObject("402") { { "error", "发生错误" } };
    }
}