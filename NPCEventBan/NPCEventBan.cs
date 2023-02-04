using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Events;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace NPCEventBan;

[ApiVersion(2, 1)]
public class NPCEventBan : TerrariaPlugin
{
    public static string ConfigFilePath = Path.Combine(TShock.SavePath, "NPCEventBan.json");

    public static ConfigFile<PluginSettings> Config = new ConfigFile<PluginSettings>();

    private ulong TickCount;

    public override string Name => "NPCEventBan NPC生成与事件限制";

    public override string Author => "LaoSparrow";

    public override Version Version => new Version("1.0.0");

    public override string Description => "限制NPC生成与事件发生";

    internal static PluginSettings Settings => Config.Settings;

    public NPCEventBan(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnGameInitialize);
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnNpcSpawn);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        GeneralHooks.ReloadEvent += this.OnReload;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnGameInitialize);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnNpcSpawn);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
            GeneralHooks.ReloadEvent -= this.OnReload;
        }
        base.Dispose(disposing);
    }

    private void OnGameInitialize(EventArgs args)
    {
        if (!Directory.Exists(TShock.SavePath))
        {
            Directory.CreateDirectory(TShock.SavePath);
        }
        this.LoadConfig();
    }

    private void OnReload(ReloadEventArgs args)
    {
        this.LoadConfig();
    }

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        try
        {
            if (Settings.NPCBanEnabled && !args.Handled)
            {
                var nPC = Main.npc[args.NpcId];
                if (nPC != null && nPC.active && ((Settings.NPCWhiteList.Count == 0 && Settings.NPCBlackList.Count == 0) || (Settings.NPCWhiteList.Count > 0 && !Settings.NPCWhiteList.Contains(nPC.netID)) || (Settings.NPCBlackList.Count > 0 && Settings.NPCBlackList.Contains(nPC.netID))))
                {
                    args.Handled = true;
                    nPC.active = false;
                }
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("[NPCEventBan] [Error] Exception occur in OnNpcSpawn, Ex: " + ex);
        }
    }

    private void OnGameUpdate(EventArgs args)
    {
        try
        {
            var flag = false;
            if (Settings.NPCBanEnabled && this.TickCount % 15uL == 0)
            {
                for (var i = 0; i < Main.npc.Length; i++)
                {
                    var npc = Main.npc[i];
                    if (npc != null && npc.active && ((Settings.NPCWhiteList.Count == 0 && Settings.NPCBlackList.Count == 0) || (Settings.NPCWhiteList.Count > 0 && !Settings.NPCWhiteList.Contains(npc.netID)) || (Settings.NPCBlackList.Count > 0 && Settings.NPCBlackList.Contains(npc.netID))))
                    {
                        npc.active = false;
                        TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
                    }
                }
            }
            if (WorldGen.spawnMeteor && Settings.disableMeteors)
            {
                WorldGen.spawnMeteor = false;
            }
            if (Main.moonPhase == 0 && Settings.disableFullMoon)
            {
                Main.moonPhase = 1;
                flag = true;
            }
            if (Main.bloodMoon && Settings.disableBloodMoon)
            {
                TSPlayer.Server.SetBloodMoon(bloodMoon: false);
            }
            if (Main.snowMoon && Settings.disableFrostMoon)
            {
                TSPlayer.Server.SetFrostMoon(snowMoon: false);
            }
            if (Main.pumpkinMoon && Settings.disablePumpkinMoon)
            {
                TSPlayer.Server.SetPumpkinMoon(pumpkinMoon: false);
            }
            if (Main.eclipse && Settings.disableSolarEclipse)
            {
                TSPlayer.Server.SetEclipse(eclipse: false);
            }
            if (Main.raining && Settings.disableRain)
            {
                Main.StopRain();
            }
            if (Main.slimeRain && Settings.disableSlimeRain)
            {
                Main.StopSlimeRain(announce: false);
                flag = true;
            }
            if (Settings.disableCultists)
            {
                WorldGen.GetRidOfCultists();
            }
            if (DD2Event.Ongoing && Settings.DD2Event)
            {
                DD2Event.Ongoing = false;
            }
            if (NPC.MoonLordCountdown > 0 && Settings.disableLunarInvasion)
            {
                NPC.MoonLordCountdown = 0;
                NPC.LunarApocalypseIsUp = false;
                NPC.TowerActiveNebula = false;
                NPC.TowerActiveSolar = false;
                NPC.TowerActiveStardust = false;
                NPC.TowerActiveVortex = false;
            }
            if (Main.invasionType > 0)
            {
                switch (Main.invasionType)
                {
                    case 1:
                        if (Settings.disableGoblinInvasion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 2:
                        if (Settings.disableFrostLegion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 3:
                        if (Settings.disablePirateInvasion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 4:
                        if (Settings.disablePumpkinMoon)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 5:
                        if (Settings.disableFrostMoon)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 6:
                        if (Settings.disableSolarEclipse)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 7:
                        if (Settings.disableMartianInvasion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                }
            }
            if (flag)
            {
                TSPlayer.All.SendData(PacketTypes.WorldInfo);
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("[NPCEventBan] [Error] Exception occur in OnGameUpdate, Ex: " + ex);
        }
        this.TickCount++;
    }

    private void LoadConfig()
    {
        try
        {
            Config.Read(ConfigFilePath, out var incompleteSettings);
            if (incompleteSettings)
            {
                Config.Write(ConfigFilePath);
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("[NPCEventBan] [Error] Failed to load config, Ex: " + ex);
        }
    }
}