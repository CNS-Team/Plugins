using Newtonsoft.Json;
using TerrariaApi.Server;
using TShockAPI;

namespace BetterWhitelist;

[ApiVersion(2, 1)]
public class Main : TerrariaPlugin
{
    public static string config_path;

    public static string translation_path;

    public static BConfig _config;

    public static Translation _translation;

    public static Dictionary<string, TSPlayer> players = new();
    public override string Name => "BetterWhitelist";

    public override Version Version => new Version(2, 1);

    public override string Author => "Bean_Paste";

    public override string Description => "A whitelist of players by checking their names";

    public Main(Terraria.Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        if (Directory.Exists(TShock.SavePath + "/BetterWhitelist"))
        {
            this.Load();
        }
        else
        {
            Directory.CreateDirectory(TShock.SavePath + "/BetterWhitelist");
            this.Load();
        }
        Commands.ChatCommands.Add(new Command("bwl.use", new CommandDelegate(this.bwl), new string[1] { "bwl" }));
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var val = new TSPlayer(args.Who);
        if (val != null)
        {
            players.Remove(val.Name);
        }
    }

    private void bwl(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(_translation.language["HelpText"]);
            return;
        }
        switch (args.Parameters[0])
        {
            case "add":
                if (_config.Disabled)
                {
                    args.Player.SendErrorMessage(_translation.language["NotEnabled"]);
                    break;
                }
                if (_config.WhitePlayers.Contains(args.Parameters[1]))
                {
                    args.Player.SendSuccessMessage(_translation.language["FailedAdd"]);
                    break;
                }
                _config.WhitePlayers.Add(args.Parameters[1]);
                args.Player.SendSuccessMessage(_translation.language["SuccessfullyAdd"]);
                File.WriteAllText(config_path, JsonConvert.SerializeObject(_config, Formatting.Indented));
                break;
            case "del":
                if (_config.Disabled)
                {
                    args.Player.SendErrorMessage(_translation.language["NotEnabled"]);
                }
                else if (_config.WhitePlayers.Contains(args.Parameters[1]))
                {
                    _config.WhitePlayers.Remove(args.Parameters[1]);
                    args.Player.SendSuccessMessage(_translation.language["SuccessfullyDelete"]);
                    File.WriteAllText(config_path, JsonConvert.SerializeObject(_config, Formatting.Indented));
                    if (players.ContainsKey(args.Parameters[1]))
                    {
                        players[args.Parameters[1]].Disconnect(_translation.language["DisconnectReason"]);
                    }
                }
                break;
            case "list":
            {
                foreach (var whitePlayer in _config.WhitePlayers)
                {
                    args.Player.SendInfoMessage(whitePlayer);
                }
                break;
            }
            case "help":
                args.Player.SendInfoMessage("-------[BetterWhitelist]-------");
                args.Player.SendInfoMessage(_translation.language["AllHelpText"]);
                break;
            case "true":
                if (!_config.Disabled)
                {
                    args.Player.SendErrorMessage(_translation.language["FailedEnable"]);
                    break;
                }
                _config.Disabled = false;
                args.Player.SendSuccessMessage(_translation.language["SuccessfullyEnable"]);
                if (players.Count > 0)
                {
                    if (_config.WhitePlayers.Count > 0)
                    {
                        foreach (var ply in players)
                        {
                            if (!_config.WhitePlayers.Contains(ply.Key))
                            {
                                ply.Value.Disconnect(_translation.language["NotOnList"]);
                            }
                        }
                    }
                    else
                    {
                        foreach (var value in players.Values)
                        {
                            value.Disconnect(_translation.language["NotOnList"]);
                        }
                    }
                }
                File.WriteAllText(config_path, JsonConvert.SerializeObject(_config, Formatting.Indented));
                break;
            case "false":
                if (_config.Disabled)
                {
                    args.Player.SendErrorMessage(_translation.language["FailedDisable"]);
                    break;
                }
                _config.Disabled = true;
                args.Player.SendSuccessMessage(_translation.language["SuccessfullyDisable"]);
                File.WriteAllText(config_path, JsonConvert.SerializeObject(_config, Formatting.Indented));
                break;
            case "reload":
                _config = JsonConvert.DeserializeObject<BConfig>(File.ReadAllText(config_path))!;
                _translation = JsonConvert.DeserializeObject<Translation>(File.ReadAllText(translation_path))!;
                args.Player.SendSuccessMessage(_translation.language["SuccessfullyReload"]);
                break;
        }
    }

    private void OnJoin(JoinEventArgs args)
    {
        var val = new TSPlayer(args.Who);
        if (args.Handled || val == null)
        {
            return;
        }

        if (players.ContainsKey(val.Name))
        {
            val.Disconnect("等待角色退出后重试!");
            return;
        }
        if (_config.Disabled)
        {
            TShock.Log.ConsoleInfo(_translation.language["NotEnabled"]);
        }
        else if (!_config.WhitePlayers.Contains(val.Name))
        {
            val.Disconnect(_translation.language["NotOnList"]);
        }
    }

    private void Load()
    {
        _config = BConfig.Load(config_path);
        _translation = Translation.Load(translation_path);
        File.WriteAllText(config_path, JsonConvert.SerializeObject(_config, Formatting.Indented));
        File.WriteAllText(translation_path, JsonConvert.SerializeObject(_translation, Formatting.Indented));
    }



    static Main()
    {
        config_path = TShock.SavePath + "/BetterWhitelist/config.json";
        translation_path = TShock.SavePath + "/BetterWhitelist/language.json";
        players = new Dictionary<string, TSPlayer>();
    }
}
