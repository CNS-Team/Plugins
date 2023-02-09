﻿using Microsoft.Xna.Framework;
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


namespace NPCEnhancer
{
    [ApiVersion(2, 1)]
    public class MainPlugin : TerrariaPlugin
    {
        public override Version Version => new Version(1, 0);
        public override string Author => "1413";
        public override string Name => "NPCEnhancer";

        public static MainPlugin? Instance { get; private set; }
        public List<EnhSetting>[]? Enhancements { get; private set; }
        public List<EnhSettingByBanner>[]? EnhancementsByBanner { get; private set; }




		public MainPlugin(Main game) : base(game)
        {
            ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn);
		}


		public override void Initialize()
		{
			Instance = this;
            Enhancements = new List<EnhSetting>[NPCID.Count];
            EnhancementsByBanner = new List<EnhSettingByBanner>[Main.MaxBannerTypes];

            Commands.ChatCommands.Add(new Command("npcenhance.hreload", HotReloadCmd, "nenhreload"));

			LoadEnhancements();
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
			{
                Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("nenhreload"));
				ServerApi.Hooks.NpcSpawn.Deregister(this, OnNpcSpawn);
				Instance = null;
			}
        }

        private void LoadEnhancements()
        {
            // var zombies = new int[]{ NPCID.Zombie, NPCID.ZombieDoctor, NPCID.ZombieElf, NPCID.ZombieElfBeard, NPCID.ZombieMushroom };

            var zombies = new EnhSettingByBanner
            {
                LifeAdd = 100,
                DefAdd = 20
            };
            zombies.AddByNPCID(NPCID.Zombie);


            var zombiesHard = new EnhSettingByBanner
            {
                LifeMult = 1,
                Condition = _ => Main.hardMode
            };
			zombiesHard.AddByNPCID(NPCID.Zombie);


			var demonEyes = new EnhSetting(lifeAdd: 100);
			demonEyes.Add(NPCID.DemonEye);
			demonEyes.Add(NPCID.DemonEyeOwl);
			demonEyes.Add(NPCID.DemonEyeSpaceship);

			var enhancements = new List<EnhSetting>()
            {
                demonEyes
            };
			var enhancementsByBanner = new List<EnhSettingByBanner>()
			{
				zombies,
				zombiesHard,
			};

            foreach(var enhancement in enhancements)
            {
                enhancement.Attach();
			}
			foreach (var enhancement in enhancementsByBanner)
			{
				enhancement.Attach();
			}
		}

		private void OnNpcSpawn(NpcSpawnEventArgs args)
		{
            var npc = Main.npc[args.NpcId];
            if (Enhancements![npc.type] != null)
            {
                foreach (var enhancement in Enhancements![npc.type])
                {
                    if (enhancement.IsValid(npc))
                    {
						enhancement.Apply(npc);
                    }
                }
            }
            var bannerID = Item.NPCtoBanner(npc.type);

			if (EnhancementsByBanner![bannerID] != null)
			{
				foreach (var enhancement in EnhancementsByBanner![bannerID])
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
            Dispose();
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
}
