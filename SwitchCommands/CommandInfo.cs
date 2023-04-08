using System.Collections.Generic;

namespace SwitchCommands;

public class CommandInfo
{
	public List<string> commandList = new List<string>();

	public float cooldown = 0f;

	public bool ignorePerms = false;
}
