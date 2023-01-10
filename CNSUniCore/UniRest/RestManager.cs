using Microsoft.Xna.Framework;
using Rests;
using System.Runtime.CompilerServices;
using TShockAPI;
using TShockAPI.DB;

namespace CNSUniCore.UniRest
{
    // Token: 0x02000009 RID: 9
    public class RestManager
    {
        public DBManager dbManager;
        // Token: 0x06000051 RID: 81 RVA: 0x000037B8 File Offset: 0x000019B8
        public RestManager()
        {
            TShock.RestApi.Register(new SecureRestCommand("/uniban/add", new RestCommandD(this.AddBan), new string[]
            {
                "uniban.admin.add"
            }));
            TShock.RestApi.Register(new SecureRestCommand("/uniban/del", new RestCommandD(this.DelBan), new string[]
            {
                "uniban.admin.del"
            }));
            TShock.RestApi.Register(new SecureRestCommand("/uniban/list", new RestCommandD(this.ListBan), new string[]
            {
                "uniban.admin.list"
            }));
            TShock.RestApi.Register(new SecureRestCommand("/uniban/broadcast", new RestCommandD(this.Broadcast), new string[]
            {
                "uniban.admin.bc"
            }));
            TShock.RestApi.Register(new SecureRestCommand("/univip/add", new RestCommandD(this.UniVipAdd), new string[]
            {
                "univip.admin.add"
            }));
            TShock.RestApi.Register(new SecureRestCommand("/univip/del", new RestCommandD(this.UniVipDel), new string[]
            {
                "univip.admin.del"
            }));
            TShock.RestApi.Register(new SecureRestCommand("/univip/adduser", new RestCommandD(this.UniUserAdd), new string[]
            {
                "unireg.admin.add"
            }));
            TShock.RestApi.Register(new SecureRestCommand("/univip/updateuser", new RestCommandD(this.UniUserUpdate), new string[]
            {
                "unireg.admin.update"
            }));
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00003918 File Offset: 0x00001B18
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
            var flag = userAccountByName != null;
            if (flag)
            {
                userAccountByName.ID = num;
                userAccountByName.Name = text;
                userAccountByName.Group = text2;
                userAccountByName.UUID = text3;
                userAccountByName.KnownIps = text4;
                userAccountByName.Registered = text5;
                userAccountByName.LastAccessed = text6;
                var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(114, 8);
                defaultInterpolatedStringHandler.AppendLiteral("UPDATE Users SET ID=");
                defaultInterpolatedStringHandler.AppendFormatted<int>(num);
                defaultInterpolatedStringHandler.AppendLiteral(",Username='");
                defaultInterpolatedStringHandler.AppendFormatted(text);
                defaultInterpolatedStringHandler.AppendLiteral("',UUID='");
                defaultInterpolatedStringHandler.AppendFormatted(text3);
                defaultInterpolatedStringHandler.AppendLiteral("',Usergroup='");
                defaultInterpolatedStringHandler.AppendFormatted(text2);
                defaultInterpolatedStringHandler.AppendLiteral("',Registered='");
                defaultInterpolatedStringHandler.AppendFormatted(text5);
                defaultInterpolatedStringHandler.AppendLiteral("',LastAccessed='");
                defaultInterpolatedStringHandler.AppendFormatted(text6);
                defaultInterpolatedStringHandler.AppendLiteral("',KnownIPs='");
                defaultInterpolatedStringHandler.AppendFormatted(text4);
                defaultInterpolatedStringHandler.AppendLiteral("' WHERE Username='");
                defaultInterpolatedStringHandler.AppendFormatted(text);
                defaultInterpolatedStringHandler.AppendLiteral("';");
                var text7 = defaultInterpolatedStringHandler.ToStringAndClear();
                DbExt.Query(TShock.DB, text7, Array.Empty<object>());
            }
            return null;
        }

        // Token: 0x06000053 RID: 83 RVA: 0x00003AF4 File Offset: 0x00001CF4
        private object UniUserAdd(RestRequestArgs args)
        {
            var id = int.Parse(args.Parameters["id"]);
            var text = args.Parameters["name"];
            var text2 = args.Parameters["password"];
            var text3 = args.Parameters["group"];
            var text4 = args.Parameters["uuid"];
            var text5 = args.Parameters["ip"];
            var text6 = args.Parameters["registered"];
            var text7 = args.Parameters["lastaccessed"];
            var userAccount = new UserAccount(text, text2, text4, text3, text6, text7, text5)
            {
                ID = id
            };
            TShock.UserAccounts.AddUserAccount(userAccount);
            return null;
        }

        // Token: 0x06000054 RID: 84 RVA: 0x00003BC4 File Offset: 0x00001DC4
        private object UniVipDel(RestRequestArgs args)
        {
            var flag = !ConfigUtils.config.EnableSponsor;
            object result;
            if (flag)
            {
                result = null;
            }
            else
            {
                var text = args.Parameters["name"];
                var list = TSPlayer.FindByNameOrID(text);
                TSPlayer tsplayer = null;
                var sponsor = MainPlugin.Instance.dbManager.GetSponsor(text);
                var flag2 = sponsor != null;
                if (flag2)
                {
                    sponsor.group = sponsor.originGroup;
                    sponsor.endTime = DateTime.UtcNow;
                    MainPlugin.Instance.dbManager.UpdateSponsor(sponsor);
                    var flag3 = list.Count != 0;
                    if (flag3)
                    {
                        tsplayer = list[0];
                    }
                    var flag4 = tsplayer != null;
                    if (flag4)
                    {
                        tsplayer.SendInfoMessage("服务器已取消你的赞助权限");
                        TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsor.originGroup);
                    }
                }
                result = null;
            }
            return result;
        }

        // Token: 0x06000055 RID: 85 RVA: 0x00003C9C File Offset: 0x00001E9C
        private object UniVipAdd(RestRequestArgs args)
        {
            var flag = !ConfigUtils.config.EnableSponsor;
            object result;
            if (flag)
            {
                result = null;
            }
            else
            {
                var text = args.Parameters["name"];
                var group = args.Parameters["group"];
                var text2 = args.Parameters["origin"];
                var s = args.Parameters["start"];
                var s2 = args.Parameters["end"];
                var list = TSPlayer.FindByNameOrID(text);
                TSPlayer tsplayer = null;
                var sponsorInfo = MainPlugin.Instance.dbManager.GetSponsor(text);
                var flag2 = sponsorInfo != null;
                if (flag2)
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
                var flag3 = list.Count != 0;
                if (flag3)
                {
                    tsplayer = list[0];
                }
                var flag4 = tsplayer != null;
                if (flag4)
                {
                    TShock.UserAccounts.SetUserGroup(tsplayer.Account, sponsorInfo.group);
                }
                result = null;
            }
            return result;
        }

        // Token: 0x06000056 RID: 86 RVA: 0x00003E04 File Offset: 0x00002004
        private object Broadcast(RestRequestArgs args)
        {
            var text = args.Parameters["text"];
            TShock.Utils.Broadcast(text, Color.DarkTurquoise);
            return null;
        }

        // Token: 0x06000057 RID: 87 RVA: 0x00003E3C File Offset: 0x0000203C
        private object ListBan(RestRequestArgs args)
        {
            var players = this.dbManager.GetPlayers();
            List<player> player = new() { };
            object result;
            for (var i = 0; i < players.Count; i++)
            {
                var w = players[i];
                player.Add(new() { Name = w.Name, Reason = w.Reason, Time = w.BanTime });
            }
            var restObject = new RestObject("200")
            {
                { "back", player }
            };
            result = restObject;
            return result;
        }
        public class player
        {
            public string Name = "";
            public string Reason = "";
            public string Time = "";
        }
        private object DelBan(RestRequestArgs args)
        {
            var flag = !ConfigUtils.config.EnableBan;
            object result;
            if (flag)
            {
                result = null;
            }
            else
            {
                var text = args.Parameters["name"];
                var flag2 = MainPlugin.Instance.dbManager.GetPlayer(text) != null;
                if (flag2)
                {
                    MainPlugin.Instance.dbManager.DeletePlayer(text);
                }
                result = null;
            }
            return result;
        }

        // Token: 0x06000058 RID: 88 RVA: 0x00003EA0 File Offset: 0x000020A0
        private object AddBan(RestRequestArgs args)
        {
            var flag = !ConfigUtils.config.EnableBan;
            object result;
            RestObject restObject;
            if (flag)
            {
                restObject = new RestObject("402")
                {
                    { "error", "发生错误" }
                };
                result = restObject;
                return result;
            }
            else
            {
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
                        if (int.TryParse(Time[0], out ti) && int.TryParse(Time[1], out ti2) && int.TryParse(Time[2], out ti3) && int.TryParse(Time[3], out ti4))
                        {
                            if (ti2 >= 0)
                            {
                                BanTime = $"{ti}:{ti2}:00:00";
                            }
                            else
                            {
                                var restObject2 = new RestObject("401")
                                {
                                    { "error", "时间格式错误" }
                                };
                                result = restObject2;
                                return result;
                            }
                        }
                        else
                        {
                            RestObject restObject3;
                            restObject3 = new RestObject("401")
                            {
                                { "error", "时间格式错误" }
                            };
                            result = restObject3;
                            return result;
                        }

                    }
                    else
                    {
                        if (int.TryParse(time, out ti))
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
                                var restObject8 = new RestObject("401")
                                {
                                    { "error", "时间格式错误" }
                                };
                                result = restObject8;
                                return result;
                            }
                        }
                        else
                        {
                            var restObject9 = new RestObject("401")
                            {
                                { "error", "时间格式错误" }
                            };
                            result = restObject9;
                            return result;
                        }
                    }
                    //TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
                    //得到精确到秒的时间戳（长度10位）
                    //long ts = (long)mTimeSpan.TotalSeconds;
                    var ts = DateTime.Now.ToString();
                    var flag7 = TSPlayer.FindByNameOrID(name).Count != 0;
                    if (flag7)
                    {
                        TSPlayer.FindByNameOrID(name)[0].Kick("因作弊被封禁", false, false, null, false);
                    }
                    //bool flag6 = MainPlugin.Instance.dbManager.GetPlayer(name) != null;

                    /*TSPlayer tsplayer = null;
                    bool flag2 = TSPlayer.FindByNameOrID(text).Count != 0;
                    if (flag2)
                    {
                        tsplayer = TSPlayer.FindByNameOrID(text)[0];
                    }
                    if (tsplayer != null)
                    {
                        tsplayer.Disconnect("你已被服务器封禁");
                    }*/

                    var flag3 = MainPlugin.Instance.dbManager.GetPlayer(text) != null;
                    if (flag3)
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
                    flag7 = TSPlayer.FindByNameOrID(name).Count != 0;
                    if (flag7)
                    {
                        TSPlayer.FindByNameOrID(name)[0].Kick("因作弊被封禁", false, false, null, false);
                    }
                    var restObject10 = new RestObject("200")
                    {
                        { "back", "成功" }
                    };
                    result = restObject10;
                    return result;
                }
            }
            var restObject11 = new RestObject("402")
            {
                { "error", "发生错误" }
            };
            result = restObject11;
            return result;
        }
    }
}