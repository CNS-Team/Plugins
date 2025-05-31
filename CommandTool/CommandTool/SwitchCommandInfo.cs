using System.Collections.Generic;

namespace CommandTool;

internal class SwitchCommandInfo
{
    public List<string> commandList = new ();

    public float cooldown = 0f;

    public bool ignorePerms = false;
}