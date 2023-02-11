using CustomPlayer.ModfiyGroup;
using System.Text;
using TShockAPI;
using TShockAPI.Hooks;
using VBY.Basic.Command;
using VBY.Basic.Extension;
using static CustomPlayer.Utils;

namespace CustomPlayer;
public partial class CustomPlayerPlugin
{
    private void CmdPermissionList(SubCmdArgs args)
    {
        var player = args.commandArgs.Player;
        var permissions = CustomPlayerPluginHelpers.TimeOutList.Where(x => x.Type == nameof(Permission) && x.Name == player.Name);
        if (!permissions.Any())
        {
            player.SendInfoMessage("你当前没有权限");
        }

        foreach (var permisssionInfo in permissions)
        {
            player.SendInfoMessage($"权限:{permisssionInfo.Value} 剩余时间:{permisssionInfo.RemainTime}");
        }
    }
    private void CmdReload(SubCmdArgs args)
    {
        OnPlayerLogout(new PlayerLogoutEventArgs(args.commandArgs.Player));
        OnPlayerPostLogin(new PlayerPostLoginEventArgs(args.commandArgs.Player));
        args.commandArgs.Player.SendSuccessMessage("重载成功");
    }
    private static void Group(CommandArgs args)
    {
        var subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();

        switch (subCmd)
        {
            case "add":
            #region Add group
            {
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group add <group name> [permissions].", Commands.Specifier));
                    return;
                }

                var groupName = args.Parameters[1];
                args.Parameters.RemoveRange(0, 2);
                var permissions = string.Join(",", args.Parameters);

                try
                {
                    CustomPlayerPluginHelpers.Groups.AddGroup(groupName, null!, permissions, TShockAPI.Group.defaultChatColor);
                    args.Player.SendSuccessMessage(GetString($"Group {groupName} was added successfully."));
                }
                catch (GroupExistsException)
                {
                    args.Player.SendErrorMessage(GetString("A group with the same name already exists."));
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.ToString());
                }
            }
            #endregion
            return;
            case "addperm":
            #region Add permissions
            {
                if (args.Parameters.Count < 3)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group addperm <group name> <permissions...>.", Commands.Specifier));
                    return;
                }

                var groupName = args.Parameters[1];
                args.Parameters.RemoveRange(0, 2);
                if (groupName == "*")
                {
                    foreach (var g in CustomPlayerPluginHelpers.Groups)
                    {
                        CustomPlayerPluginHelpers.Groups.AddPermissions(g.Name, args.Parameters);
                    }
                    args.Player.SendSuccessMessage(GetString("The permissions have been added to all of the groups in the system."));
                    return;
                }
                try
                {
                    var response = CustomPlayerPluginHelpers.Groups.AddPermissions(groupName, args.Parameters);
                    if (response.Length > 0)
                    {
                        args.Player.SendSuccessMessage(response);
                    }
                    return;
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.ToString());
                }
            }
            #endregion
            return;
            case "help":
            #region Help
            {
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
                {
                    return;
                }

                var lines = new List<string>
                        {
                            GetString("add <name> <permissions...> - Adds a new group."),
                            GetString("addperm <group> <permissions...> - Adds permissions to a group."),
                            GetString("color <group> <rrr,ggg,bbb> - Changes a group's chat color."),
                            GetString("rename <group> <new name> - Changes a group's name."),
                            GetString("del <group> - Deletes a group."),
                            GetString("delperm <group> <permissions...> - Removes permissions from a group."),
                            GetString("list [page] - Lists groups."),
                            GetString("listperm <group> [page] - Lists a group's permissions."),
                            GetString("parent <group> <parent group> - Changes a group's parent group."),
                            GetString("prefix <group> <prefix> - Changes a group's prefix."),
                            GetString("suffix <group> <suffix> - Changes a group's suffix.")
                        };

                PaginationTools.SendPage(args.Player, pageNumber, lines,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("Group Sub-Commands ({{0}}/{{1}}):"),
                        FooterFormat = GetString("Type {0}group help {{0}} for more sub-commands.", Commands.Specifier)
                    }
                );
            }
            #endregion
            return;
            case "parent":
            #region Parent
            {
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group parent <group name> [new parent group name].", Commands.Specifier));
                    return;
                }

                var groupName = args.Parameters[1];
                var group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName);
                if (group == null)
                {
                    args.Player.SendErrorMessage(GetString("No such group \"{0}\".", groupName));
                    return;
                }

                if (args.Parameters.Count > 2)
                {
                    var newParentGroupName = string.Join(" ", args.Parameters.Skip(2));
                    if (!string.IsNullOrWhiteSpace(newParentGroupName) && !CustomPlayerPluginHelpers.Groups.GroupExists(newParentGroupName))
                    {
                        args.Player.SendErrorMessage(GetString("No such group \"{0}\".", newParentGroupName));
                        return;
                    }

                    try
                    {
                        CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, newParentGroupName, group.Permissions, group.ChatColor, group.Suffix, group.Prefix);

                        if (!string.IsNullOrWhiteSpace(newParentGroupName))
                        {
                            args.Player.SendSuccessMessage(GetString("Parent of group \"{0}\" set to \"{1}\".", groupName, newParentGroupName));
                        }
                        else
                        {
                            args.Player.SendSuccessMessage(GetString("Removed parent of group \"{0}\".", groupName));
                        }
                    }
                    catch (GroupManagerException ex)
                    {
                        args.Player.SendErrorMessage(ex.Message);
                    }
                }
                else
                {
                    if (group.Parent != null)
                    {
                        args.Player.SendSuccessMessage(GetString("Parent of \"{0}\" is \"{1}\".", group.Name, group.Parent.Name));
                    }
                    else
                    {
                        args.Player.SendSuccessMessage(GetString("Group \"{0}\" has no parent.", group.Name));
                    }
                }
            }
            #endregion
            return;
            case "suffix":
            #region Suffix
            {
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group suffix <group name> [new suffix].", Commands.Specifier));
                    return;
                }

                var groupName = args.Parameters[1];
                var group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName);
                if (group == null)
                {
                    args.Player.SendErrorMessage(GetString("No such group \"{0}\".", groupName));
                    return;
                }

                if (args.Parameters.Count > 2)
                {
                    var newSuffix = string.Join(" ", args.Parameters.Skip(2));

                    try
                    {
                        CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, newSuffix, group.Prefix);

                        if (!string.IsNullOrWhiteSpace(newSuffix))
                        {
                            args.Player.SendSuccessMessage(GetString("Suffix of group \"{0}\" set to \"{1}\".", groupName, newSuffix));
                        }
                        else
                        {
                            args.Player.SendSuccessMessage(GetString("Removed suffix of group \"{0}\".", groupName));
                        }
                    }
                    catch (GroupManagerException ex)
                    {
                        args.Player.SendErrorMessage(ex.Message);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(group.Suffix))
                    {
                        args.Player.SendSuccessMessage(GetString("Suffix of \"{0}\" is \"{1}\".", group.Name, group.Suffix));
                    }
                    else
                    {
                        args.Player.SendSuccessMessage(GetString("Group \"{0}\" has no suffix.", group.Name));
                    }
                }
            }
            #endregion
            return;
            case "prefix":
            #region Prefix
            {
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group prefix <group name> [new prefix].", Commands.Specifier));
                    return;
                }

                var groupName = args.Parameters[1];
                var group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName);
                if (group == null)
                {
                    args.Player.SendErrorMessage(GetString("No such group \"{0}\".", groupName));
                    return;
                }

                if (args.Parameters.Count > 2)
                {
                    var newPrefix = string.Join(" ", args.Parameters.Skip(2));

                    try
                    {
                        CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, group.Suffix, newPrefix);

                        if (!string.IsNullOrWhiteSpace(newPrefix))
                        {
                            args.Player.SendSuccessMessage(GetString("Prefix of group \"{0}\" set to \"{1}\".", groupName, newPrefix));
                        }
                        else
                        {
                            args.Player.SendSuccessMessage(GetString("Removed prefix of group \"{0}\".", groupName));
                        }
                    }
                    catch (GroupManagerException ex)
                    {
                        args.Player.SendErrorMessage(ex.Message);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(group.Prefix))
                    {
                        args.Player.SendSuccessMessage(GetString("Prefix of \"{0}\" is \"{1}\".", group.Name, group.Prefix));
                    }
                    else
                    {
                        args.Player.SendSuccessMessage(GetString("Group \"{0}\" has no prefix.", group.Name));
                    }
                }
            }
            #endregion
            return;
            case "color":
            #region Color
            {
                if (args.Parameters.Count < 2 || args.Parameters.Count > 3)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group color <group name> [new color(000,000,000)].", Commands.Specifier));
                    return;
                }

                var groupName = args.Parameters[1];
                var group = CustomPlayerPluginHelpers.Groups.GetGroupByName(groupName);
                if (group == null)
                {
                    args.Player.SendErrorMessage(GetString("No such group \"{0}\".", groupName));
                    return;
                }

                if (args.Parameters.Count == 3)
                {
                    var newColor = args.Parameters[2];

                    var parts = newColor.Split(',');
                    if (parts.Length == 3 && byte.TryParse(parts[0], out var r) && byte.TryParse(parts[1], out var g) && byte.TryParse(parts[2], out var b))
                    {
                        try
                        {
                            CustomPlayerPluginHelpers.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, newColor, group.Suffix, group.Prefix);

                            args.Player.SendSuccessMessage(GetString("Chat color for group \"{0}\" set to \"{1}\".", groupName, newColor));
                        }
                        catch (GroupManagerException ex)
                        {
                            args.Player.SendErrorMessage(ex.Message);
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("Invalid syntax for color, expected \"rrr,ggg,bbb\"."));
                    }
                }
                else
                {
                    args.Player.SendSuccessMessage(GetString("Chat color for \"{0}\" is \"{1}\".", group.Name, group.ChatColor));
                }
            }
            #endregion
            return;
            case "rename":
            #region Rename group
            {
                if (args.Parameters.Count != 3)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group rename <group> <new name>.", Commands.Specifier));
                    return;
                }

                var group = args.Parameters[1];
                var newName = args.Parameters[2];
                try
                {
                    var response = CustomPlayerPluginHelpers.Groups.RenameGroup(group, newName);
                    args.Player.SendSuccessMessage(response);
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.Message);
                }
            }
            #endregion
            return;
            case "del":
            #region Delete group
            {
                if (args.Parameters.Count != 2)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group del <group name>.", Commands.Specifier));
                    return;
                }

                try
                {
                    var response = CustomPlayerPluginHelpers.Groups.DeleteGroup(args.Parameters[1], true);
                    if (response.Length > 0)
                    {
                        args.Player.SendSuccessMessage(response);
                    }
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.Message);
                }
            }
            #endregion
            return;
            case "delperm":
            #region Delete permissions
            {
                if (args.Parameters.Count < 3)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group delperm <group name> <permissions...>.", Commands.Specifier));
                    return;
                }

                var groupName = args.Parameters[1];
                args.Parameters.RemoveRange(0, 2);
                if (groupName == "*")
                {
                    foreach (var g in CustomPlayerPluginHelpers.Groups)
                    {
                        CustomPlayerPluginHelpers.Groups.DeletePermissions(g.Name, args.Parameters);
                    }
                    args.Player.SendSuccessMessage(GetString("The permissions have been removed from all of the groups in the system."));
                    return;
                }
                try
                {
                    var response = CustomPlayerPluginHelpers.Groups.DeletePermissions(groupName, args.Parameters);
                    if (response.Length > 0)
                    {
                        args.Player.SendSuccessMessage(response);
                    }
                    return;
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.Message);
                }
            }
            #endregion
            return;
            case "list":
            #region List groups
            {
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
                {
                    return;
                }

                var groupNames = from grp in CustomPlayerPluginHelpers.Groups.groups
                                 select grp.Name;
                PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(groupNames),
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("Groups ({{0}}/{{1}}):"),
                        FooterFormat = GetString("Type {0}group list {{0}} for more.", Commands.Specifier)
                    });
            }
            #endregion
            return;
            case "listperm":
            #region List permissions
            {
                if (args.Parameters.Count == 1)
                {
                    args.Player.SendErrorMessage(GetString("Invalid syntax. Proper syntax: {0}group listperm <group name> [page].", Commands.Specifier));
                    return;
                }
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out var pageNumber))
                {
                    return;
                }

                if (!CustomPlayerPluginHelpers.Groups.GroupExists(args.Parameters[1]))
                {
                    args.Player.SendErrorMessage(GetString("Invalid group."));
                    return;
                }
                var grp = CustomPlayerPluginHelpers.Groups.GetGroupByName(args.Parameters[1]);
                var permissions = grp.TotalPermissions;

                PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(permissions),
                    new PaginationTools.Settings
                    {
                        HeaderFormat = GetString("Permissions for {0} ({{0}}/{{1}}):", grp.Name),
                        FooterFormat = GetString("Type {0}group listperm {1} {{0}} for more.", Commands.Specifier, grp.Name),
                        NothingToDisplayString = GetString($"There are currently no permissions for {grp.Name}.")
                    });
            }
            #endregion
            return;
            default:
                args.Player.SendErrorMessage(GetString("Invalid subcommand! Type {0}group help for more information on valid commands.", Commands.Specifier));
                return;
        }
    }
    private void CtlGroupCtlAdd(SubCmdArgs args)
    {
        var name = args.Parameters[0];
        var groupName = args.Parameters[1];
        var time = args.Parameters[2];
        var player = args.commandArgs.Player;
        var startTime = DateTime.Now;
        DateTime endTime = new();
        TimeSpan addTime = new();
        var forever = time == "-1";

        if (TimeParse(forever, time, ref startTime, ref endTime, ref addTime, player))
        {
            return;
        }

        var haveGroups = new List<string>();
        using var groupReader = QueryReader("select Value,Type,StartTime,EndTime,DurationText from ExpirationInfo where Type = 'Group' AND Name = @0", name);
        groupReader.Reader.ForEach(x =>
        {
            {
                var withBlock = x;
                var tobj = new TimeOutObject(name, withBlock.GetString(nameof(TableInfo.ExpirationInfo.Value)), withBlock.GetString(nameof(TableInfo.ExpirationInfo.Type)), withBlock.GetDateTime(nameof(TableInfo.ExpirationInfo.StartTime)), withBlock.GetDateTime(nameof(TableInfo.ExpirationInfo.EndTime)), withBlock.GetString(nameof(TableInfo.ExpirationInfo.DurationText)));
                if (tobj.NoExpired)
                {
                    haveGroups.Add(tobj.Value);
                }
                else
                {
                    player.SendInfoMessage($"组:{tobj.Value} 已过期");
                    tobj.Delete();
                }
            }
        });
        if (haveGroups.Contains(groupName))
        {
            player.SendInfoMessage("玩家:{0} 已拥有组:{1}", name, groupName);
            return;
        }
        if (!CustomPlayerPluginHelpers.Groups.GroupExists(groupName))
        {
            player.SendInfoMessage("组:{0} 不存在,如果确定存在,请执行:{1}{2} {3}", groupName, Commands.Specifier, this.AddCommands[1].Name, this.CtlCommand["Reload"]!.Names[0]);
            return;
        }
        var oldGroupGrade = haveGroups.Any() ? haveGroups.Select(x => CustomPlayerPluginHelpers.GroupGrade[x]).Max() : -1;
        var newGroupGrade = CustomPlayerPluginHelpers.GroupGrade[groupName];
        if (oldGroupGrade < newGroupGrade)
        {
            player.SendInfoMessage("玩家:{0} 组升级为 {1}", name, groupName);
        }
        Query("insert into ExpirationInfo(Name,Value,Type,StartTime,EndTime,DurationText) values(@0,@1,@2,@3,@4,@5)", name, groupName, nameof(Group), startTime, endTime, time);
        if (forever)
        {
            player.SendInfoMessage($"添加成功 玩家:{name} 组:{groupName} 持续时间:永久");
        }
        else
        {
            player.SendInfoMessage($"添加成功 玩家:{name} 组:{groupName} 起始时间:{startTime} 结束时间:{endTime} 持续时间:{addTime}");
        }

        FindPlayer(name)?.Reload();
    }
    private void CtlGroupCtlDel(SubCmdArgs args)
    {
        var name = args.Parameters[0];
        var groupName = args.Parameters[1];
        var player = args.commandArgs.Player;

        var haveGroups = new List<string>();
        TimeOutObject? curObj = null;
        using var groupReader = QueryReader("select Value,Type,StartTime,EndTime,DurationText from ExpirationInfo where Type = 'Group' AND Name = @0", name);
        groupReader.Reader.ForEach(x =>
        {
            {
                var withBlock = x;
                var tobj = new TimeOutObject(name, withBlock.GetString(nameof(TableInfo.ExpirationInfo.Value)), withBlock.GetString(nameof(TableInfo.ExpirationInfo.Type)), withBlock.GetDateTime(nameof(TableInfo.ExpirationInfo.StartTime)), withBlock.GetDateTime(nameof(TableInfo.ExpirationInfo.EndTime)), withBlock.GetString(nameof(TableInfo.ExpirationInfo.DurationText)));
                if (tobj.NoExpired)
                {
                    haveGroups.Add(tobj.Value);
                }
                else
                {
                    player.SendInfoMessage($"组:{tobj.Value} 已过期");
                    tobj.Delete();
                }
                if (tobj.Value == groupName)
                {
                    curObj = tobj;
                }
            }
        });
        if (curObj is null)
        {
            player.SendInfoMessage("玩家:{0} 不拥有组:{1}", name, groupName);
            return;
        }
        curObj.Delete();

        var cply = FindPlayer(name);
        if (cply != null)
        {
            cply.Reload();
            cply.Player.SendInfoMessage("你的组:{0} 已被管理员删除", groupName);
        }

        player.SendInfoMessage($"删除成功 玩家:{name} 组:{groupName}");
    }
    private void CtlGroupCtlList(SubCmdArgs args)
    {
        var name = args.Parameters[0];
        var player = args.commandArgs.Player;

        using var groupReader = QueryReader("select Value,Type,StartTime,EndTime,DurationText from ExpirationInfo where Type = 'Group' AND Name = @0", name);
        List<TimeOutObject> objs = new();
        groupReader.Reader.ForEach(x =>
        {
            {
                var withBlock = x;
                var tobj = new TimeOutObject(name, withBlock.GetString(nameof(TableInfo.ExpirationInfo.Value)), withBlock.GetString(nameof(TableInfo.ExpirationInfo.Type)), withBlock.GetDateTime(nameof(TableInfo.ExpirationInfo.StartTime)), withBlock.GetDateTime(nameof(TableInfo.ExpirationInfo.EndTime)), withBlock.GetString(nameof(TableInfo.ExpirationInfo.DurationText)));
                if (tobj.NoExpired)
                {
                    objs.Add(tobj);
                }
                else
                {
                    player.SendInfoMessage($"组:{tobj.Value} 已过期");
                    tobj.Delete();
                }
            }
        });
        if (!objs.Any())
        {
            player.SendInfoMessage("此玩家没有组");
            return;
        }
        var orderObjs = objs.OrderByDescending(x => CustomPlayerPluginHelpers.GroupGrade[x.Value]).ToArray();
        player.SendSuccessMessage("组:{0} 剩余时间:{1}", orderObjs[0].Value, orderObjs[0].RemainTime);
        for (var i = 1; i < orderObjs.Length; i++)
        {
            player.SendInfoMessage("组:{0} 剩余时间:{1}", orderObjs[i].Value, orderObjs[i].RemainTime);
        }
    }
    private void CtlPermissionAdd(SubCmdArgs args)
    {
        var name = args.Parameters[0];
        var addPermission = args.Parameters[1];
        var time = args.Parameters[2];
        var player = args.commandArgs.Player;
        var startTime = DateTime.Now;
        DateTime endTime = new();
        TimeSpan addTime = new();
        var forever = time == "-1";

        if (TimeParse(forever, time, ref startTime, ref endTime, ref addTime, player))
        {
            return;
        }

        if (Utils.NotifyPlayer("权限", forever, new TimeOutObject(name, addPermission, nameof(Permission), startTime, endTime, time), out var cply))
        {
            cply.AddPermission(addPermission);
        }

        Query("insert into ExpirationInfo(Name,Value,Type,StartTime,EndTime,DurationText) values(@0,@1,@2,@3,@4,@5)", name, addPermission, nameof(Permission), startTime, endTime, time);
        if (forever)
        {
            player.SendInfoMessage($"添加成功 玩家:{name} 权限:{addPermission} 持续时间:永久");
        }
        else
        {
            player.SendInfoMessage($"添加成功 玩家:{name} 权限:{addPermission} 起始时间:{startTime} 结束时间:{endTime} 持续时间:{addTime}");
        }
    }
    private void CtlPermissionDel(SubCmdArgs args)
    {
        var player = args.commandArgs.Player;
        var name = args.Parameters[0];
        var delPermission = args.Parameters[1];

        var delCount = Query("delete from ExpirationInfo where Name = @0 AND Value = @1", name, delPermission);
        if (delCount == 0)
        {
            player.SendInfoMessage($"未在数据库找到 玩家:{name} 的权限:{delPermission}");
        }
        else
        {
            player.SendSuccessMessage($"删除成功");
            var cply = FindPlayer(name);
            if (cply != null)
            {
                CustomPlayerPluginHelpers.TimeOutList.RemoveAll(x => x.Name == name && x.Value == delPermission);
                cply.DelPermission(delPermission);
                cply.Player.SendInfoMessage($"你的权限:{delPermission} 已被管理员提前删除");
            }
        }
    }
    private void CtlPermissionList(SubCmdArgs args)
    {
        var player = args.commandArgs.Player;
        var name = args.Parameters[0];
        using (var reader = QueryReader("select Value,EndTime,DurationText from ExpirationInfo where name = @0 AND Type = @1", name, nameof(Permission)))
        {
            if (reader.Read())
            {
                player.SendInfoMessage($"玩家:{name} 的权限");
                reader.Reader.DoForEach(x => player.SendInfoMessage($"权限:{x.GetString("Value")} 剩余时间:{(x.GetString("DurationText") == "-1" ? "永久" : (x.GetDateTime("EndTime") - DateTime.Now).ToString(@"d\.hh\:mm\:ss"))}"));
            }
            else
            {
                player.SendInfoMessage("没有找到此玩家的权限");
            }
        }
        using (var reader = QueryReader("select Commands from PlayerList where name = @0", name))
        {
            if (reader.Read() && !reader.Reader.IsDBNull(0))
            {
                player.SendInfoMessage("永久权限");
                foreach (var perm in reader.Reader.GetString(0).Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    player.SendInfoMessage($"权限:{perm} 剩余时间:永久");
                }
            }
        }
    }
    private void CtlReload(SubCmdArgs args)
    {
        if (ReadConfig.Read(args.commandArgs.Player, false, true, ReadConfig.Root))
        {
            PluginInit();
            CustomPlayerPluginHelpers.Groups = new GroupManager(CustomPlayerPluginHelpers.DB);
            CustomPlayerPluginHelpers.GroupGrade.Clear();
            CustomPlayerPluginHelpers.GroupLevelSet();
            for (var i = 0; i <= CustomPlayerPluginHelpers.Players.Length - 1; i++)
            {
                if (CustomPlayerPluginHelpers.Players[i] != null)
                {
                    OnPlayerLogout(new PlayerLogoutEventArgs(CustomPlayerPluginHelpers.Players[i].Player));
                }
            }
            foreach (var player in TShock.Players.Where(x => x != null && x.IsLoggedIn))
            {
                OnPlayerLogout(new PlayerLogoutEventArgs(player));
            }

            args.commandArgs.Player.SendSuccessMessage("重载成功");
        }
        else
        {
            args.commandArgs.Player.SendSuccessMessage("重载错误");
            args.commandArgs.Player.SendErrorMessage(ReadConfig.ErrorString);
        }
    }
    private void CtlPrefixAdd(SubCmdArgs args)
    {
        CtlTitleAdd(ref args, "Prefix");
    }

    private void CtlPrefixDel(SubCmdArgs args)
    {
        CtlTitleDel(ref args, "Prefix");
    }

    private void CtlPrefixList(SubCmdArgs args)
    {
        CtlTitleList(ref args, "Prefix");
    }

    private void CtlPrefixTest(SubCmdArgs args)
    {
        CtlTitleTest(ref args, "Prefix");
    }

    private void CtlPrefixWear(SubCmdArgs args)
    {
        CtlTitleWear(ref args, "Prefix");
    }

    private void CtlSuffixAdd(SubCmdArgs args)
    {
        CtlTitleAdd(ref args, "Suffix");
    }

    private void CtlSuffixDel(SubCmdArgs args)
    {
        CtlTitleDel(ref args, "Suffix");
    }

    private void CtlSuffixList(SubCmdArgs args)
    {
        CtlTitleList(ref args, "Suffix");
    }

    private void CtlSuffixTest(SubCmdArgs args)
    {
        CtlTitleTest(ref args, "Suffix");
    }

    private void CtlSuffixWear(SubCmdArgs args)
    {
        CtlTitleWear(ref args, "Suffix");
    }
}