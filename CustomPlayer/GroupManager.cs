using System.Collections;
using System.Data;
using System.Diagnostics;

using TShockAPI;
using TShockAPI.DB;

using static CustomPlayer.Utils;

namespace CustomPlayer.ModfiyGroup;

public class GroupManager : IEnumerable<Group>
{
    private IDbConnection database;
    public readonly List<Group> groups = new();
    public GroupManager(IDbConnection db)
    {
        database = db;

        // Load Permissions from the DB
        LoadPermisions();

    }
    public bool GroupExists(string group)
    {
        if (group == "superadmin")
            return true;

        return groups.Any(g => g.Name.Equals(group));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public IEnumerator<Group> GetEnumerator()
    {
        return groups.GetEnumerator();
    }
    public Group GetGroupByName(string name)
    {
        var ret = groups.Where(g => g.Name == name);
        return 1 == ret.Count() ? ret.ElementAt(0) : null!;
    }
    public void AddGroup(string name, string parentname, string permissions, string chatcolor)
    {
        if (GroupExists(name))
        {
            throw new GroupExistsException(name);
        }

        var group = new Group(name, null, chatcolor);
        group.Permissions = permissions;
        if (!string.IsNullOrWhiteSpace(parentname))
        {
            var parent = groups.FirstOrDefault(gp => gp.Name == parentname);
            if (parent == null || name == parentname)
            {
                var error = GetString($"Invalid parent group {parentname} for group {group.Name}");
                TShock.Log.ConsoleError(error);
                throw new GroupManagerException(error);
            }
            group.Parent = parent;
        }

        string query = database.GetSqlType() == SqlType.Sqlite
            ? "INSERT OR IGNORE INTO GroupList (GroupName, Parent, Commands, ChatColor) VALUES (@0, @1, @2, @3);"
            : "INSERT IGNORE INTO GroupList SET GroupName=@0, Parent=@1, Commands=@2, ChatColor=@3";
        if (database.Query(query, name, parentname, permissions, chatcolor) == 1)
        {
            groups.Add(group);
        }
        else
            throw new GroupManagerException(GetString($"Failed to add group {name}."));
    }
    public void UpdateGroup(string name, string parentname, string permissions, string chatcolor, string suffix, string prefix)
    {
        Group? group = GetGroupByName(name);
        if (group == null)
            throw new GroupNotExistException(name);

        Group? parent = null;
        if (!string.IsNullOrWhiteSpace(parentname))
        {
            parent = GetGroupByName(parentname);
            if (parent == null || parent == group)
                throw new GroupManagerException(GetString($"Invalid parent group {parentname} for group {name}."));

            // Check if the new parent would cause loops.
            List<Group> groupChain = new List<Group> { group, parent };
            Group checkingGroup = parent.Parent;
            while (checkingGroup != null)
            {
                if (groupChain.Contains(checkingGroup))
                    throw new GroupManagerException(
                        GetString($"Parenting group {group} to {parentname} would cause loops in the parent chain."));

                groupChain.Add(checkingGroup);
                checkingGroup = checkingGroup.Parent;
            }
        }

        // Ensure any group validation is also persisted to the DB.
        var newGroup = new Group(name, parent, chatcolor, permissions);
        newGroup.Prefix = prefix;
        newGroup.Suffix = suffix;
        string query = "UPDATE GroupList SET Parent=@0, Commands=@1, ChatColor=@2, Suffix=@3, Prefix=@4 WHERE GroupName=@5";
        if (database.Query(query, parentname, newGroup.Permissions, newGroup.ChatColor, suffix, prefix, name) != 1)
            throw new GroupManagerException(GetString($"Failed to update group \"{name}\"."));

        group.ChatColor = chatcolor;
        group.Permissions = permissions;
        group.Parent = parent;
        group.Prefix = prefix;
        group.Suffix = suffix;
    }
    public string RenameGroup(string name, string newName)
    {
        if (!GroupExists(name))
        {
            throw new GroupNotExistException(name);
        }

        if (GroupExists(newName))
        {
            throw new GroupExistsException(newName);
        }

        using (var db = database.CloneEx())
        {
            db.Open();
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE GroupList SET GroupName = @0 WHERE GroupName = @1";
                        command.AddParameter("@0", newName);
                        command.AddParameter("@1", name);
                        command.ExecuteNonQuery();
                    }

                    var oldGroup = GetGroupByName(name);
                    var newGroup = new Group(newName, oldGroup.Parent, oldGroup.ChatColor, oldGroup.Permissions)
                    {
                        Prefix = oldGroup.Prefix,
                        Suffix = oldGroup.Suffix
                    };
                    groups.Remove(oldGroup);
                    groups.Add(newGroup);

                    // We need to check if the old group has been referenced as a parent and update those references accordingly
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE GroupList SET Parent = @0 WHERE Parent = @1";
                        command.AddParameter("@0", newName);
                        command.AddParameter("@1", name);
                        command.ExecuteNonQuery();
                    }
                    foreach (var group in groups.Where(g => g.Parent != null && g.Parent == oldGroup))
                    {
                        group.Parent = newGroup;
                    }

                    // We also need to check if any users belong to the old group and automatically apply changes
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE Users SET Usergroup = @0 WHERE Usergroup = @1";
                        command.AddParameter("@0", newName);
                        command.AddParameter("@1", name);
                        command.ExecuteNonQuery();
                    }
                    foreach (var player in CustomPlayerPluginHelpers.Players.Where(p => p?.Group == oldGroup))
                    {
                        player.Group = newGroup;
                    }

                    transaction.Commit();
                    return GetString($"Group {name} has been renamed to {newName}.");
                }
                catch (Exception ex)
                {
                    TShock.Log.Error(GetString($"An exception has occurred during database transaction: {ex.Message}"));
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        TShock.Log.Error(GetString($"An exception has occurred during database rollback: {rollbackEx.Message}"));
                    }
                }
            }
        }

        throw new GroupManagerException(GetString($"Failed to rename group {name}."));
    }

    /// <summary>
    /// Deletes the specified group.
    /// </summary>
    /// <param name="name">The group's name.</param>
    /// <param name="exceptions">Whether exceptions will be thrown in case something goes wrong.</param>
    /// <returns>The result from the operation to be sent back to the user.</returns>
    public string DeleteGroup(string name, bool exceptions = false)
    {
        if (!GroupExists(name))
        {
            if (exceptions)
                throw new GroupNotExistException(name);
            return GetString($"Group {name} doesn't exist.");
        }

        if (database.Query("DELETE FROM GroupList WHERE GroupName=@0", name) == 1)
        {
            groups.Remove(TShock.Groups.GetGroupByName(name));
            return GetString($"Group {name} has been deleted successfully.");
        }

        if (exceptions)
            throw new GroupManagerException(GetString($"Failed to delete group {name}."));
        return GetString($"Failed to delete group {name}.");
    }
    public string AddPermissions(string name, List<string> permissions)
    {
        if (!GroupExists(name))
            return GetString($"Group {name} doesn't exist.");

        var group = GetGroupByName(name);
        var oldperms = group.Permissions; // Store old permissions in case of error
        permissions.ForEach(p => group.AddPermission(p));

        if (database.Query("UPDATE GroupList SET Commands=@0 WHERE GroupName=@1", group.Permissions, name) == 1)
            return "Group " + name + " has been modified successfully.";

        // Restore old permissions so DB and internal object are in a consistent state
        group.Permissions = oldperms;
        return "";
    }
    public string DeletePermissions(string name, List<string> permissions)
    {
        if (!GroupExists(name))
            return GetString($"Group {name} doesn't exist.");

        var group = GetGroupByName(name);
        var oldperms = group.Permissions; // Store old permissions in case of error
        permissions.ForEach(p => group.RemovePermission(p));

        if (database.Query("UPDATE GroupList SET Commands=@0 WHERE GroupName=@1", group.Permissions, name) == 1)
            return "Group " + name + " has been modified successfully.";

        // Restore old permissions so DB and internal object are in a consistent state
        group.Permissions = oldperms;
        return "";
    }
    public void LoadPermisions()
    {
        try
        {
            List<Group> newGroups = new List<Group>(groups.Count);
            Dictionary<string, string> newGroupParents = new Dictionary<string, string>(groups.Count);
            using (var reader = database.QueryReader("SELECT * FROM GroupList"))
            {
                while (reader.Read())
                {
                    string groupName = reader.Get<string>("GroupName");

                    newGroups.Add(new Group(groupName, null, reader.Get<string>("ChatColor"), reader.Get<string>("Commands"))
                    {
                        Prefix = reader.Get<string>("Prefix"),
                        Suffix = reader.Get<string>("Suffix"),
                    });

                    try
                    {
                        newGroupParents.Add(groupName, reader.Get<string>("Parent"));
                    }
                    catch (ArgumentException)
                    {
                        // Just in case somebody messed with the unique primary key.
                        TShock.Log.ConsoleError(GetString($"The group {groupName} appeared more than once. Keeping current group settings."));
                        return;
                    }
                }
            }

            try
            {
                // Get rid of deleted groups.
                for (int i = 0; i < groups.Count; i++)
                    if (newGroups.All(g => g.Name != groups[i].Name))
                        groups.RemoveAt(i--);

                // Apply changed group settings while keeping the current instances and add new groups.
                foreach (Group newGroup in newGroups)
                {
                    Group? currentGroup = groups.FirstOrDefault(g => g.Name == newGroup.Name);
                    if (currentGroup != null)
                        newGroup.AssignTo(currentGroup);
                    else
                        groups.Add(newGroup);
                }

                // Resolve parent groups.
                Debug.Assert(newGroups.Count == newGroupParents.Count);
                for (int i = 0; i < groups.Count; i++)
                {
                    Group group = groups[i];
                    if (!newGroupParents.TryGetValue(group.Name, out var parentGroupName) || string.IsNullOrEmpty(parentGroupName))
                        continue;

                    group.Parent = groups.FirstOrDefault(g => g.Name == parentGroupName);
                    if (group.Parent == null)
                    {
                        TShock.Log.ConsoleError(
                            GetString($"Group {group.Name} is referencing a non existent parent group {parentGroupName}, parent reference was removed."));
                    }
                    else
                    {
                        if (group.Parent == group)
                            TShock.Log.ConsoleWarn(
                                GetString($"Group {group.Name} is referencing itself as parent group; parent reference was removed."));

                        List<Group> groupChain = new List<Group> { group };
                        Group checkingGroup = group;
                        while (checkingGroup.Parent != null)
                        {
                            if (groupChain.Contains(checkingGroup.Parent))
                            {
                                TShock.Log.ConsoleError(
                                    GetString($"Group \"{checkingGroup.Name}\" is referencing parent group {checkingGroup.Parent.Name} which is already part of the parent chain. Parent reference removed."));

                                checkingGroup.Parent = null;
                                break;
                            }
                            groupChain.Add(checkingGroup);
                            checkingGroup = checkingGroup.Parent;
                        }
                    }
                }
            }
            finally
            {
                
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"Error on reloading groups: {ex}"));
        }
    }
}

/// <summary>
/// Represents the base GroupManager exception.
/// </summary>
[Serializable]
public class GroupManagerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupManagerException"/> with the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public GroupManagerException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupManagerException"/> with the specified message and inner exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner exception.</param>
    public GroupManagerException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

/// <summary>
/// Represents the GroupExists exception.
/// This exception is thrown whenever an attempt to add an existing group into the database is made.
/// </summary>
[Serializable]
public class GroupExistsException : GroupManagerException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupExistsException"/> with the specified group name.
    /// </summary>
    /// <param name="name">The group name.</param>
    public GroupExistsException(string name)
        : base(GetString($"Group {name} already exists"))
    {
    }
}

/// <summary>
/// Represents the GroupNotExist exception.
/// This exception is thrown whenever we try to access a group that does not exist.
/// </summary>
[Serializable]
public class GroupNotExistException : GroupManagerException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupNotExistException"/> with the specified group name.
    /// </summary>
    /// <param name="name">The group name.</param>
    public GroupNotExistException(string name)
        : base(GetString($"Group {name} does not exist"))
    {
    }
}
