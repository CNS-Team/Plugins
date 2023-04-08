using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace ChromeAutoReset;

[ApiVersion(2, 1)]
public class AutoResetPlugin : TerrariaPlugin
{
    private readonly string ConfigPath = Path.Combine(TShock.SavePath, "reset_config.json");

    private readonly string FilePath = Path.Combine(TShock.SavePath, "backup_files");

    private ResetConfig config;

    private Status status;

    private GenerationProgress generationProgress;

    public override string Name => "ChromeAutoReset";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public override string Author => "Designed by cc004,edit by Leader,Prism and Cai";

    public override string Description => "流光之城服务器 完全自动重置插件";

    public AutoResetPlugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        //IL_0104: Unknown result type (might be due to invalid IL or missing references)
        //IL_011c: Expected O, but got Unknown
        //IL_0117: Unknown result type (might be due to invalid IL or missing references)
        //IL_0121: Expected O, but got Unknown
        //IL_0165: Unknown result type (might be due to invalid IL or missing references)
        //IL_016f: Expected O, but got Unknown
        if (!Directory.Exists(this.FilePath))
        {
            Directory.CreateDirectory(this.FilePath);
        }
        if (!File.Exists(this.ConfigPath))
        {
            this.config = new ResetConfig
            {
                PreResetCommands = new string[0],
                PostResetCommands = new string[3] { "/重读超进度物品限制", "/重读进度补给", "/th loadspawn 1.sec" },
                SQLs = new string[3] { "DELETE FROM tsCharacter", "DELETE FROM ChromeEconomy", "DELETE FROM PlayerMoney" },
                Files = new Dictionary<string, string>
                {
                    ["超进度物品限制.json"] = "超进度物品限制.json",
                    ["进度补给.json"] = "进度补给.json",
                    ["禁止怪物表.json"] = "禁止怪物表.json"
                }
            };
            File.WriteAllText(this.ConfigPath, this.config.ToJson());
        }
        else
        {
            this.config = JsonConvert.DeserializeObject<ResetConfig>(File.ReadAllText(this.ConfigPath));
        }
        Commands.ChatCommands.Add(new Command("reset.admin", new CommandDelegate(this.ResetCmd), new string[1] { "reset" }));
        ServerApi.Hooks.ServerConnect.Register((TerrariaPlugin) (object) this, (HookHandler<ConnectEventArgs>) this.OnServerConnect);
        ServerApi.Hooks.WorldSave.Register((TerrariaPlugin) (object) this, (HookHandler<WorldSaveEventArgs>) this.OnWorldSave, int.MaxValue);
        GeneralHooks.ReloadEvent += (GeneralHooks.ReloadEventD) delegate (ReloadEventArgs e)
        {
            if (File.Exists(this.ConfigPath))
            {
                this.config = JsonConvert.DeserializeObject<ResetConfig>(File.ReadAllText(this.ConfigPath));
            }
            else
            {
                this.config = new ResetConfig
                {
                    PreResetCommands = new string[0],
                    PostResetCommands = new string[3] { "/重读超进度物品限制", "/重读进度补给", "/th loadspawn 1.sec" },
                    SQLs = new string[3] { "DELETE FROM tsCharacter", "DELETE FROM ChromeEconomy", "DELETE FROM PlayerMoney" },
                    Files = new Dictionary<string, string>
                    {
                        ["超进度物品限制.json"] = "超进度物品限制.json",
                        ["进度补给.json"] = "进度补给.json",
                        ["禁止怪物表.json"] = "禁止怪物表.json"
                    }
                };
                File.WriteAllText(this.ConfigPath, this.config.ToJson());
            }
            e.Player.SendSuccessMessage("自动重置插件配置已重载");
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerConnect.Deregister((TerrariaPlugin) (object) this, (HookHandler<ConnectEventArgs>) this.OnServerConnect);
            ServerApi.Hooks.WorldSave.Deregister((TerrariaPlugin) (object) this, (HookHandler<WorldSaveEventArgs>) this.OnWorldSave);
        }
        this.Dispose(disposing);
    }

    private void ResetCmd(CommandArgs args)
    {
        //IL_008a: Unknown result type (might be due to invalid IL or missing references)
        //IL_0094: Expected O, but got Unknown
        //IL_00dd: Unknown result type (might be due to invalid IL or missing references)
        //IL_00e7: Expected O, but got Unknown
        Task.Run(delegate
        {
            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            for (var num = 5; num >= 0; num--)
            {
                TShock.Utils.Broadcast($"重置进程已启动,{num}s后所有玩家将被移出服务器", Color.Yellow);
                Thread.Sleep(1000);
            }
            LinqExt.ForEach<TSPlayer>((IEnumerable<TSPlayer>) TShock.Players, (Action<TSPlayer>) delegate (TSPlayer p)
            {
                if (p != null)
                {
                    p.Kick("服务器已开始重置", true, true, (string) null, false);
                }
            });
        }).Wait();
        this.status = Status.Cleaning;
        LinqExt.ForEach<string>((IEnumerable<string>) this.config.PreResetCommands, (Action<string>) delegate (string c)
        {
            Commands.HandleCommand((TSPlayer) (object) TSPlayer.Server, c);
        });
        Main.WorldFileMetadata = null;
        Main.gameMenu = true;
        Main.maxTilesX = 8400;
        Main.maxTilesY = 2400;
        UIWorldCreation.ProcessSpecialWorldSeeds(WorldGen.currentWorldSeed);
        this.generationProgress = new GenerationProgress();
        var task = WorldGen.CreateNewWorld(this.generationProgress);
        this.status = Status.Generating;
        while (!task.IsCompleted)
        {
            TShock.Log.ConsoleInfo(this.GetProgress());
            Thread.Sleep(100);
        }
        this.status = Status.Cleaning;
        Main.rand = new UnifiedRandom((int) DateTime.Now.Ticks);
        WorldFile.LoadWorld(false);
        Main.dayTime = WorldFile._tempDayTime;
        Main.time = WorldFile._tempTime;
        Main.raining = WorldFile._tempRaining;
        Main.rainTime = WorldFile._tempRainTime;
        Main.maxRaining = WorldFile._tempMaxRain;
        Main.cloudAlpha = WorldFile._tempMaxRain;
        Main.moonPhase = WorldFile._tempMoonPhase;
        Main.bloodMoon = WorldFile._tempBloodMoon;
        Main.eclipse = WorldFile._tempEclipse;
        Main.gameMenu = false;
        try
        {
            this.PostReset();
        }
        finally
        {
            this.generationProgress = null;
            this.status = Status.Available;
        }
    }

    private void PostReset()
    {
        LinqExt.ForEach<string>((IEnumerable<string>) this.config.SQLs, (Action<string>) delegate (string c)
        {
            DbExt.Query(TShock.DB, c, Array.Empty<object>());
        });
        foreach (var file in this.config.Files)
        {
            File.Copy(Path.Combine(this.FilePath, file.Value), Path.Combine(TShock.SavePath, file.Key), overwrite: true);
        }
        LinqExt.ForEach<string>((IEnumerable<string>) this.config.PostResetCommands, (Action<string>) delegate (string c)
        {
            Commands.HandleCommand((TSPlayer) (object) TSPlayer.Server, c);
        });
    }

    private string GetProgress()
    {
        return string.Format("{0:0.0%} - " + this.generationProgress.Message + " - {1:0.0%}", this.generationProgress.TotalProgress, this.generationProgress.Value);
    }

    private void OnServerConnect(ConnectEventArgs args)
    {
        switch (this.status)
        {
            case Status.Generating:
                NetMessage.BootPlayer(args.Who, NetworkText.FromLiteral("生成地图中:" + this.GetProgress()));
                ((HandledEventArgs) (object) args).Handled = true;
                break;
            case Status.Cleaning:
                NetMessage.BootPlayer(args.Who, NetworkText.FromLiteral("重置数据中，请稍后"));
                ((HandledEventArgs) (object) args).Handled = true;
                break;
        }
    }

    private void OnWorldSave(WorldSaveEventArgs args)
    {
        ((HandledEventArgs) (object) args).Handled = this.status != 0 && Main.WorldFileMetadata == null;
    }
}