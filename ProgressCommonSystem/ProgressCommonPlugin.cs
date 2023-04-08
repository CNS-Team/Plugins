using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ProgressCommonSystem;

[ApiVersion(2, 1)]
public class ProgressCommonPlugin : TerrariaPlugin
{
    public override string Name => "Progress Common System";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public override string Author => "棱镜";

    public override string Description => "关于进度操作的前置插件";

    public static event BossProgressAdvancedHandler OnBossProgressAdvanced;

    public static event EventProgressAdvancedHandler OnEventProgressAdvanced;

    public ProgressCommonPlugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.NpcSpawn.Register((TerrariaPlugin) (object) this, (HookHandler<NpcSpawnEventArgs>) this.OnNpcSpawn);
        ServerApi.Hooks.GameUpdate.Register((TerrariaPlugin) (object) this, (HookHandler<EventArgs>) this.OnUpdate);
        ServerApi.Hooks.GamePostInitialize.Register((TerrariaPlugin) (object) this, (HookHandler<EventArgs>) this.PostInitialize);
    }

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        var val = Main.npc[args.NpcId];
        if ((!((Entity) val).active || !val.boss) && val.type != 548)
        {
            return;
        }
        if (val.type == 548 && !WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.TheOldOnesArmy))
        {
            var eventProgressAdvancedEventArgs = new EventProgressAdvancedEventArgs
            {
                EventProgress = EventProgress.TheOldOnesArmy,
                Handled = false
            };
            ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs);
            if (eventProgressAdvancedEventArgs.Handled)
            {
                ((Entity) val).active = false;
                val.type = 0;
                TSPlayer.All.SendData((PacketTypes) 23, "", ((Entity) val).whoAmI, 0f, 0f, 0f, 0);
            }
            else
            {
                WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.TheOldOnesArmy);
            }
            return;
        }
        var type = val.type;
        if (1 == 0)
        {
        }
        var bossProgress = type switch
        {
            50 => BossProgress.KingSlime,
            4 => BossProgress.EyeOfCthulhu,
            13 => BossProgress.EaterOfWorlds,
            266 => BossProgress.BrainOfCthulhu,
            222 => BossProgress.QueenBee,
            35 => BossProgress.Skeletron,
            668 => BossProgress.Deerclops,
            113 => BossProgress.WallOfFlesh,
            657 => BossProgress.QueenSlime,
            131 => BossProgress.TheTwins,
            130 => BossProgress.TheTwins,
            134 => BossProgress.TheDestroyer,
            127 => BossProgress.SkeletronPrime,
            262 => BossProgress.Plantera,
            245 => BossProgress.Golem,
            370 => BossProgress.DukeFishron,
            636 => BossProgress.EmpressOfLight,
            439 => BossProgress.LunaticCultist,
            517 => BossProgress.SolarPillar,
            422 => BossProgress.VortexPillar,
            507 => BossProgress.NebulaPillar,
            493 => BossProgress.StardustPillar,
            398 => BossProgress.MoonLord,
            _ => throw new Exception("Unknown boss type " + type),
        };
        if (1 == 0)
        {
        }
        var bossProgress2 = bossProgress;
        var bossProgress3 = bossProgress2;
        var bossProgress4 = bossProgress3;
        if (!WorldProgress.CurrentWorldProgress.DownedBoss(bossProgress4))
        {
            var bossProgressAdvancedEventArgs = new BossProgressAdvancedEventArgs
            {
                BossProgress = bossProgress4,
                Handled = false
            };
            ProgressCommonPlugin.OnBossProgressAdvanced?.Invoke(bossProgressAdvancedEventArgs);
            if (bossProgressAdvancedEventArgs.Handled)
            {
                ((Entity) val).active = false;
                val.type = 0;
                TSPlayer.All.SendData((PacketTypes) 23, "", ((Entity) val).whoAmI, 0f, 0f, 0f, 0);
            }
            else
            {
                WorldProgress.CurrentWorldProgress.downedBoss.Add(bossProgress4);
            }
        }
    }

    private void OnUpdate(EventArgs args)
    {
        if (Main.bloodMoon && !WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.BloodMoon))
        {
            var eventProgressAdvancedEventArgs = new EventProgressAdvancedEventArgs
            {
                EventProgress = EventProgress.BloodMoon,
                Handled = false
            };
            ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs);
            if (eventProgressAdvancedEventArgs.Handled)
            {
                TSPlayer.Server.SetBloodMoon(false);
            }
            else
            {
                WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.BloodMoon);
                ExtraData.Instance.downedBloodMoon = true;
                File.WriteAllText(ExtraData.SavePath, JsonConvert.SerializeObject((object) ExtraData.Instance, (Formatting) 1));
            }
        }
        if (Main.eclipse && !WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.SolarEclipse))
        {
            var eventProgressAdvancedEventArgs2 = new EventProgressAdvancedEventArgs
            {
                EventProgress = EventProgress.SolarEclipse,
                Handled = false
            };
            ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs2);
            if (eventProgressAdvancedEventArgs2.Handled)
            {
                TSPlayer.Server.SetEclipse(false);
            }
            else
            {
                WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.SolarEclipse);
                ExtraData.Instance.downedSolarEclipse = true;
                File.WriteAllText(ExtraData.SavePath, JsonConvert.SerializeObject((object) ExtraData.Instance, (Formatting) 1));
            }
        }
        if (Main.pumpkinMoon && !WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.PumpkinMoon))
        {
            var eventProgressAdvancedEventArgs3 = new EventProgressAdvancedEventArgs
            {
                EventProgress = EventProgress.PumpkinMoon,
                Handled = false
            };
            ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs3);
            if (eventProgressAdvancedEventArgs3.Handled)
            {
                TSPlayer.Server.SetPumpkinMoon(false);
            }
            else
            {
                WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.PumpkinMoon);
                ExtraData.Instance.downedPumpkinMoon = true;
                File.WriteAllText(ExtraData.SavePath, JsonConvert.SerializeObject((object) ExtraData.Instance, (Formatting) 1));
            }
        }
        if (Main.snowMoon && !WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.FrostMoon))
        {
            var eventProgressAdvancedEventArgs4 = new EventProgressAdvancedEventArgs
            {
                EventProgress = EventProgress.FrostMoon,
                Handled = false
            };
            ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs4);
            if (eventProgressAdvancedEventArgs4.Handled)
            {
                TSPlayer.Server.SetFrostMoon(false);
            }
            else
            {
                WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.FrostMoon);
                ExtraData.Instance.downedFrostMoon = true;
                File.WriteAllText(ExtraData.SavePath, JsonConvert.SerializeObject((object) ExtraData.Instance, (Formatting) 1));
            }
        }
        var flag = false;
        switch (Main.invasionType)
        {
            case 1:
                if (!WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.GoblinArmy))
                {
                    var eventProgressAdvancedEventArgs7 = new EventProgressAdvancedEventArgs
                    {
                        EventProgress = EventProgress.GoblinArmy,
                        Handled = false
                    };
                    ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs7);
                    if (eventProgressAdvancedEventArgs7.Handled)
                    {
                        Main.invasionType = 0;
                        Main.invasionSize = 0;
                    }
                    else
                    {
                        WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.GoblinArmy);
                    }
                }
                break;
            case 2:
                if (!WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.FrostLegion))
                {
                    var eventProgressAdvancedEventArgs8 = new EventProgressAdvancedEventArgs
                    {
                        EventProgress = EventProgress.FrostLegion,
                        Handled = false
                    };
                    ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs8);
                    if (eventProgressAdvancedEventArgs8.Handled)
                    {
                        Main.invasionType = 0;
                        Main.invasionSize = 0;
                    }
                    else
                    {
                        WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.FrostLegion);
                    }
                }
                break;
            case 3:
                if (!WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.PirateInvasion))
                {
                    var eventProgressAdvancedEventArgs10 = new EventProgressAdvancedEventArgs
                    {
                        EventProgress = EventProgress.PirateInvasion,
                        Handled = false
                    };
                    ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs10);
                    if (eventProgressAdvancedEventArgs10.Handled)
                    {
                        Main.invasionType = 0;
                        Main.invasionSize = 0;
                    }
                    else
                    {
                        WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.PirateInvasion);
                    }
                }
                break;
            case 4:
                if (!WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.PumpkinMoon))
                {
                    var eventProgressAdvancedEventArgs9 = new EventProgressAdvancedEventArgs
                    {
                        EventProgress = EventProgress.PumpkinMoon,
                        Handled = false
                    };
                    ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs9);
                    if (eventProgressAdvancedEventArgs9.Handled)
                    {
                        Main.invasionType = 0;
                        Main.invasionSize = 0;
                    }
                    else
                    {
                        WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.PumpkinMoon);
                    }
                }
                break;
            case 5:
                if (!WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.FrostMoon))
                {
                    var eventProgressAdvancedEventArgs6 = new EventProgressAdvancedEventArgs
                    {
                        EventProgress = EventProgress.FrostMoon,
                        Handled = false
                    };
                    ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs6);
                    if (eventProgressAdvancedEventArgs6.Handled)
                    {
                        Main.invasionType = 0;
                        Main.invasionSize = 0;
                    }
                    else
                    {
                        WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.FrostMoon);
                    }
                }
                break;
            case 6:
                if (!WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.SolarEclipse))
                {
                    var eventProgressAdvancedEventArgs11 = new EventProgressAdvancedEventArgs
                    {
                        EventProgress = EventProgress.SolarEclipse,
                        Handled = false
                    };
                    ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs11);
                    if (eventProgressAdvancedEventArgs11.Handled)
                    {
                        Main.invasionType = 0;
                        Main.invasionSize = 0;
                    }
                    else
                    {
                        WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.SolarEclipse);
                    }
                }
                break;
            case 7:
                if (!WorldProgress.CurrentWorldProgress.DownedEvent(EventProgress.MartianMadness))
                {
                    var eventProgressAdvancedEventArgs5 = new EventProgressAdvancedEventArgs
                    {
                        EventProgress = EventProgress.MartianMadness,
                        Handled = false
                    };
                    ProgressCommonPlugin.OnEventProgressAdvanced?.Invoke(eventProgressAdvancedEventArgs5);
                    if (eventProgressAdvancedEventArgs5.Handled)
                    {
                        Main.invasionType = 0;
                        Main.invasionSize = 0;
                    }
                    else
                    {
                        WorldProgress.CurrentWorldProgress.downedEvent.Add(EventProgress.MartianMadness);
                    }
                }
                break;
            default:
                flag = false;
                break;
        }
        if (flag)
        {
            TSPlayer.All.SendData((PacketTypes) 7, "", 0, 0f, 0f, 0f, 0);
        }
    }

    private void PostInitialize(EventArgs args)
    {
        if (!File.Exists(ExtraData.SavePath) || string.IsNullOrEmpty(File.ReadAllText(ExtraData.SavePath)))
        {
            ExtraData.Instance = new ExtraData
            {
                downedPumpkinMoon = NPC.downedHalloweenKing,
                downedBloodMoon = NPC.killCount[489] > 0,
                downedFrostMoon = NPC.downedChristmasIceQueen || NPC.downedChristmasSantank || NPC.downedChristmasTree,
                downedSolarEclipse = NPC.killCount[477] > 0
            };
            File.WriteAllText(ExtraData.SavePath, JsonConvert.SerializeObject((object) ExtraData.Instance, (Formatting) 1));
        }
        else
        {
            ExtraData.Instance = JsonConvert.DeserializeObject<ExtraData>(File.ReadAllText(ExtraData.SavePath));
        }
        WorldProgress.CurrentWorldProgress = WorldProgress.GetWorldProgress();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcSpawn.Deregister((TerrariaPlugin) (object) this, (HookHandler<NpcSpawnEventArgs>) this.OnNpcSpawn);
            ServerApi.Hooks.GameUpdate.Deregister((TerrariaPlugin) (object) this, (HookHandler<EventArgs>) this.OnUpdate);
            ServerApi.Hooks.GamePostInitialize.Deregister((TerrariaPlugin) (object) this, (HookHandler<EventArgs>) this.PostInitialize);
        }
        this.Dispose(disposing);
    }
}