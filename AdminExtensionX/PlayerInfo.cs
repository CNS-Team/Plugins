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
		Backup = null;
	}

	public bool Restore(TSPlayer player)
	{
		if (Backup == null)
		{
			return false;
		}
		Backup.RestoreCharacter(player);
		Backup = null;
		CopyingUserName = "";
		return true;
	}
}
