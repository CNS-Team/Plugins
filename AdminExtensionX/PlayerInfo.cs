using TShockAPI;

namespace AdminExtension;

public class PlayerInfo
{
    public const string KEY = "AdminExtension_Data";

    public PlayerData Backup { get; set; }

    public string CopyingUserName { get; set; }

    public int UserID { get; set; }

    public PlayerInfo()
    {
        this.Backup = null;
    }

    public bool Restore(TSPlayer player)
    {
        if (this.Backup == null)
        {
            return false;
        }
        this.Backup.RestoreCharacter(player);
        this.Backup = null;
        this.CopyingUserName = "";
        return true;
    }
}