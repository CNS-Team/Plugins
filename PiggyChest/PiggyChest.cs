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


namespace PiggyChest;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override Version Version => new Version(1, 0);
    public override string Author => "1413";
    public override string Name => "PiggyChest";

    public static MainPlugin? Instance { get; private set; }




    public MainPlugin(Main game) : base(game)
    {

    }


    public override void Initialize()
    {
        Instance = this;
        Commands.ChatCommands.Add(new Command("piggychest.hreload", this.HotReloadCmd, "piggychestreload"));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("piggychestreload"));

            Instance = null;
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
        PluginContainer? Container = null;
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
        var path = Path.Combine(ServerApi.PluginsPath, "PiggyChest.dll");
        if (File.Exists("PiggyChest.pdb"))
        {
            pdb = File.ReadAllBytes("PiggyChest.pdb");
        }
        var newPlugin = System.Reflection.Assembly.Load(File.ReadAllBytes(path), pdb);
        var pluginClass = newPlugin.GetType(typeof(MainPlugin).FullName!)!;
        var instance = Activator.CreateInstance(pluginClass, new[] { Main.instance });
        #endregion
        #region Replace
        Container!
            .GetType()
            .GetProperty(nameof(Container.Plugin))
            !.SetValue(Container, instance);
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