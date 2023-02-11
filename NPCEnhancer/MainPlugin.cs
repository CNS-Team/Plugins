using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;


namespace NPCEnhancer;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override Version Version => new Version(1, 0);
    public override string Author => "1413";
    public override string Name => "NPCEnhancer";

    public static MainPlugin? Instance { get; private set; }
    public List<EnhSetting>[]? Enhancements { get; private set; }




    public MainPlugin(Main game) : base(game)
    {
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnNpcSpawn);
    }


    public override void Initialize()
    {
        Instance = this;
        this.Enhancements = new List<EnhSetting>[NPCID.Count];

        Commands.ChatCommands.Add(new Command("npcenhance.hreload", this.HotReloadCmd, "nenhreload"));

        this.LoadEnhancements();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("nenhreload"));
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnNpcSpawn);
            Instance = null;
        }
    }

    private void LoadEnhancements()
    {

        var zombies = new EnhSetting
        {
            LifeAdd = 100,
            DefAdd = 20
        };
        zombies.AddBannerNPC(NPCID.Zombie);


        var zombiesHard = new EnhSetting
        {
            LifeMult = 1,
            Condition = _ => Main.hardMode
        };
        zombiesHard.AddBannerNPC(NPCID.Zombie);


        var demonEyes = new EnhSetting(lifeAdd: 100);
        demonEyes.Add(NPCID.DemonEye);
        demonEyes.Add(NPCID.DemonEyeOwl);
        demonEyes.Add(NPCID.DemonEyeSpaceship);

        var enhancements = new List<EnhSetting>()
        {
            demonEyes,
            zombies,
            zombiesHard,
        };


        foreach (var enhancement in enhancements)
        {
            enhancement.Attach();
        }
    }

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        var npc = Main.npc[args.NpcId];
        if (this.Enhancements![npc.type] != null)
        {
            foreach (var enhancement in this.Enhancements![npc.type])
            {
                if (enhancement.IsValid(npc))
                {
                    enhancement.Apply(npc);
                }
            }
        }

    }


    #region HotReload
    public void PostHotReload()
    {

    }
    private void HotReloadCmd(CommandArgs args)
    {
        this.Dispose();
        #region FindContainer
        PluginContainer Container = null;
        foreach (var container in ServerApi.Plugins)
        {
            if (container.Plugin == this)
            {
                Container = container;
                break;
            }
        }
        #endregion
        #region Load New
        byte[]? pdb = null;
        var path = Path.Combine(ServerApi.PluginsPath, "NPCEnhancer.dll");
        if (File.Exists("NPCEnhancer.pdb"))
        {
            pdb = File.ReadAllBytes("NPCEnhancer.pdb");
        }
        var newPlugin = System.Reflection.Assembly.Load(File.ReadAllBytes(path), pdb);
        var pluginClass = newPlugin.GetType(typeof(MainPlugin).FullName!)!;
        var instance = Activator.CreateInstance(pluginClass, new[] { Main.instance });
        #endregion
        #region Replace
        Container
            .GetType()
            .GetProperty(nameof(Container.Plugin))
            .SetValue(Container, instance);
        #endregion
        #region Initialize
        Container.Initialize();
        pluginClass
            .GetMethod(nameof(PostHotReload))
            .Invoke(instance, Array.Empty<object>());
        #endregion
    }
    #endregion
}