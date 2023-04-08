global using TShockAPI;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace ServerTools;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    private long TimerCount = 0;

    private event TimerEvent? OnTimer;

    public static  Config Config = new Config();

    public static  string PATH = Path.Combine(TShock.SavePath, "ServerTool.json");
    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        this.Load();
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, BetterWhitelist.Main.OnJoin);
        ServerApi.Hooks.NetGreetPlayer.Register(this, CGive.Main.OnGreetPlayer);
        ServerApi.Hooks.NetGetData.Register(this, Chameleon.Main.OnGetData, 9999);
        ServerApi.Hooks.GamePostInitialize.Register(this, Chameleon.Main.OnPostInit, 9999);
        GetDataHandlers.Sign += ServerTools.SignLock.Main.OnSignEdit!;
        GetDataHandlers.ItemDrop += ServerTools.AntiItemFlood.Main.OnItemDrop;
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += (_) => this.LoadConfig();
        OnTimer += ServerTools.AdminExtensionX.Main.Update;
        TShock.RestApi.Register("/getWarehouse", CGive.Main.getWarehouse);
        this.RegisterCommands();
    }



    private void Load()
    {
        this.LoadConfig();
        CGive.Data.Init();
    }

    private void LoadConfig()
    {
        if (File.Exists(PATH))
        {
            try
            {
                Config = Config.Read(PATH);
            }
            catch (Exception e)
            {
                TShock.Log.ConsoleError("ServerTools配置读取错误：{0}", e.ToString());
            }
        }
        Config.Write(PATH);
    }

    private void RegisterCommands()
    {
        AdminExtensionX.Main.ACmd();
        BetterWhitelist.Main.Acmd();
        CGive.Main.ACmd();
    }

    private void OnUpdate(EventArgs args)
    {
        this.TimerCount++;
        if (this.TimerCount % 60 == 0)
        {
            this.OnTimer?.Invoke(new EventArgs());
        }
    }
}
