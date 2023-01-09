using LazyUtils;
using System;
using System.Linq;
using LazyUtils.Commands;
using TShockAPI;

namespace OnlineInfo
{
    [Command("oi")]
    public static class CmdOnlineInfo
    {
        [Permission("onlineinfo")]
        [Alias("c")]
        public static void Count(CommandArgs args)
        {
            try
            {
                using var query = Utils.GetDBQuery<DBModel.OnlineInfo>();
                args.Player.SendInfoMessage("当前全服在线人数: " + query.Select(x => x.PlayerCount).Sum());
            }
            catch (Exception ex) { Logger.ConsoleError($"Failed to count total online, Ex: {ex}"); }
        }

        [Permission("onlineinfo")]
        [Alias("cn")]
        public static void CountWithName(CommandArgs args)
        {
            try
            {
                using var query = Utils.GetDBQuery<DBModel.OnlineInfo>();
                int count = query.Select(x => x.PlayerCount).Sum();
                string[] players = query.Select(x => x.Players).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                args.Player.SendInfoMessage(
                    $"当前全服在线人数: {count}\n" + 
                    $"当前全服在线玩家: {string.Join(",", players)}");
            }
            catch (Exception ex) { Logger.ConsoleError($"Failed to count total online with player names, Ex: {ex}"); }
        }

        public static void Help(CommandArgs args)
        {
            args.Player.SendInfoMessage(
                "用法:\n" +
                "  /oi c\n" +
                "    获取全服在线人数\n" +
                "  /oi cn\n" +
                "    获取全服在线人数及玩家");
        }


    }

    [Command("oic")]
    public static class CmdOnlineInfoCtrl
    {
        public static class Info
        {
            [Permission("onlineinfo.ctrl")]
            public static void All(CommandArgs args)
            {
                try
                {
                    using var query = Utils.GetDBQuery<DBModel.OnlineInfo>();
                    var selectedInfos = query.ToArray();
                    if (selectedInfos.Length == 0)
                    {
                        args.Player.SendInfoMessage("当前数据库内无在线数据");
                        return;
                    }
                    foreach (var i in selectedInfos)
                    {
                        args.Player.SendInfoMessage(i.ToString());
                    }
                }
                catch (Exception ex) { Logger.ConsoleError($"Failed to get online info of all servers, Ex: {ex}"); }
            }

            [Permission("onlineinfo.ctrl")]
            public static void Id(CommandArgs args, int id)
            {
                try
                {
                    using var query = Utils.GetDBQuery<DBModel.OnlineInfo>();
                    var selectedInfo = query.Where(x => x.ServerId == id).SingleOrDefault();
                    if (selectedInfo == null)
                    {
                        args.Player.SendInfoMessage($"未查询到id为{id}的服务器在线数据");
                        return;
                    }
                    args.Player.SendInfoMessage(selectedInfo.ToString());
                }
                catch (Exception ex) { Logger.ConsoleError($"Failed to get online info of server id: {id}, Ex: {ex}"); }
            }

            [Permission("onlineinfo.ctrl")]
            public static void Server(CommandArgs args, string serverName)
            {
                try
                {
                    using var query = Utils.GetDBQuery<DBModel.OnlineInfo>();
                    var selectedInfos = query.Where(x => x.ServerName == serverName).ToArray();
                    if (selectedInfos.Length == 0)
                    {
                        args.Player.SendInfoMessage($"未查询到服务器名称为{serverName}的服务器在线数据");
                        return;
                    }
                    foreach (var i in selectedInfos)
                    {
                        args.Player.SendInfoMessage(i.ToString());
                    }
                }
                catch (Exception ex) { Logger.ConsoleError($"Failed to get online info of server name: {serverName}, Ex: {ex}"); }
            }

            public static void Help(CommandArgs args)
            {
                args.Player.SendInfoMessage(
                    "用法:\n" +
                    "  /oic info all\n" +
                    "    查询所有服务器在线信息\n" +
                    "  /oic info id <服务器id>\n" +
                    "    根据id查询服务器在线信息\n" +
                    "  /oic info server <服务器名称>\n" +
                    "    根据名称查询服务器在线信息");
            }
        }

        [Permission("onlineinfo.ctrl")]
        public static void Update(CommandArgs args)
        {
            if (OnlineInfoPlugin.UpdateOnlineInfo())
                args.Player.SendInfoMessage("更新成功");
            else
                args.Player.SendInfoMessage("更新失败");
        }

        [Permission("onlineinfo.ctrl")]
        public static void AutoUpdate(CommandArgs args, string isEnabled)
        {
            switch (isEnabled)
            {
                case "t":
                case "true":
                    OnlineInfoPlugin.isAutoUpdateEnabled = true;
                    args.Player.SendInfoMessage("启用自动更新服务器在线数据");
                    break;

                case "f":
                case "false":
                    OnlineInfoPlugin.isAutoUpdateEnabled = false;
                    args.Player.SendInfoMessage("禁用自动更新服务器在线数据");
                    break;
            }
        }

        public static void Help(CommandArgs args)
        {
            args.Player.SendInfoMessage(
                "用法:\n" +
                "  /oic info <...>\n" +
                "    查询服务器在线数据\n" +
                "  /oic update\n" +
                "    更新服务器在线数据\n" +
                "  /oic autoupdate <true/false>\n" +
                "    启用或禁用自动更新服务器在线数据");
        }
    }
}
