using Newtonsoft.Json;

namespace BetterWhitelist;

public class Translation
{
    public Dictionary<string, string> language = new Dictionary<string, string>();

    public static Translation Load(string path)
    {
        if (File.Exists(path))
        {
            return JsonConvert.DeserializeObject<Translation>(File.ReadAllText(path))!;
        }
        var translation = new Translation();
        translation.language.Add("SuccessfullyDelete", "Delete successfully!");
        translation.language.Add("SuccessfullyAdd", "Add successfully!");
        translation.language.Add("SuccessfullyEnable", "Enable successfully!");
        translation.language.Add("SuccessfullyDisable", "Disable successfully!");
        translation.language.Add("SuccessfullyReload", "Reload successfully!");
        translation.language.Add("FailedAdd", "Add Failed ! The player already exists in the whitelist");
        translation.language.Add("FailedDelete", "Delete failed ! The player does not exist in the whitelist");
        translation.language.Add("FailedEnable", "Enable failed ! The plugin is already open");
        translation.language.Add("FailedDisable", "Disable failed ! The plugin is already closed");
        translation.language.Add("DisconnectReason", "You are removed from the whitelist!");
        translation.language.Add("HelpText", "Usage: type /bwl help to show the help info.");
        translation.language.Add("NotEnabled", "The switch of plugin is disabled,please check the config!");
        translation.language.Add("AllHelpText", "/bwl help to show help info\n/bwl add {name} to add a player name to the whitelist\n/bwl del {name} to remove a player from the whitelist\n/bwl list to show all players on the whitelist\n/bwl true to enable the plugin\n/bwl false to disable the plugin\n/bwl reload to reload the plugin");
        translation.language.Add("NotOnList", "You are not in the server whitelist");
        return translation;
    }
}