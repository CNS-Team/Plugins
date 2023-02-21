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

    public Config Config { get; set; }
    public StorageManager Storage { get; set; }

    public static MainPlugin? Instance { get; private set; }




    public MainPlugin(Main game) : base(game)
    {
        Instance = this;
    }


    public override void Initialize()
    {
        Instance = this;
        Commands.ChatCommands.Add(new Command("piggychest.hreload", this.HotReloadCmd, "piggychestreload"));
        Commands.ChatCommands.Add(new Command("piggychest.admin", this.HandleCommand, "piggychest"));
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        this.Config = Config.LoadConfig();
        this.Storage = new StorageManager(this.Config);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("piggychestreload"));
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("piggychest"));
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            Instance = null;
        }
    }



    // todo: 后续可能需要添加点检查以防客户端乱发包
    private void OnGetData(GetDataEventArgs args)
    {
        switch (args.MsgID)
        {
            case PacketTypes.ChestGetContents:
            {
                var player = TShock.Players[args.Msg.whoAmI];
                using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                var reader = new BinaryReader(stream);

                int x = reader.ReadInt16();
                int y = reader.ReadInt16();
                int chestID = Chest.FindChest(x, y);

                // TSPlayer.All.SendInfoMessage($"request: chest[{chestID}]:{(chestID == -1 ? "-1" : Main.chest[chestID].name)}");

                if (chestID > -1 && Chest.UsingChest(chestID) == -1 && this.Config.ChestNames.Contains(Main.chest[chestID].name))
                {
                    var piggyBank = this.Storage.GetBankItems(player.Account.ID, Main.chest[chestID].name);
                    // TSPlayer.All.SendInfoMessage($"piggy: list[{piggyBank.Count}]");
                    for (int i = 0; i < Math.Min(40, piggyBank.Count); i++)
                    {
                        Main.chest[chestID].item[i] = piggyBank[i].ToItem();
                    }
                    for (int i = Math.Min(40, piggyBank.Count); i < 40; i++)
                    {
                        Main.chest[chestID].item[i].TurnToAir();
                    }
                }
            }
            break;

            case PacketTypes.ChestOpen:
            {
                var player = TShock.Players[args.Msg.whoAmI];
                using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                var reader = new BinaryReader(stream);

                int chestID = reader.ReadInt16();
                // TSPlayer.All.SendInfoMessage($"open: chest[{chestID}]:{(chestID==-1?"-1":Main.chest[chestID].name)}");

                if (chestID != -1)
                {
                    return;
                }

                chestID = player.ActiveChest;

                int num3 = reader.ReadInt16();
                int num4 = reader.ReadInt16();
                int num5 = reader.ReadByte();
                string name = string.Empty;
                if (num5 != 0)
                {
                    if (num5 <= 20)
                    {
                        name = reader.ReadString();
                    }
                    else if (num5 != 255)
                    {
                        num5 = 0;
                    }
                }


                if (!this.Config.ChestNames.Contains(Main.chest[player.ActiveChest].name))
                {
                    if (num5 != 0 && this.Config.ChestNames.Contains(name)) // 改名检查，只允许空箱子改名
                    {
                        if (Main.chest[player.ActiveChest].item.Sum(i => i.stack) > 0)
                        {
                            NetMessage.TrySendData(80, -1, -1, null, player.Index, chestID);
                            NetMessage.TrySendData(69, -1, -1, null, chestID, Main.chest[player.ActiveChest].x, Main.chest[player.ActiveChest].y);
                            player.SendErrorMessage($"只有空箱子可被重命名为{name}");
                            args.Handled = true;
                        }
                    }
                    return;
                }

                var piggyBank = this.Storage.GetBankItems(player.Account.ID, Main.chest[chestID].name);
                if (piggyBank.Count > 40)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        piggyBank[i] = Main.chest[chestID].item[i];
                    }
                    piggyBank.RemoveAll(item => item == default);
                }
                else
                {
                    piggyBank = Main.chest[chestID].item.Where(item => !item.IsAir).Select(i => (ItemInfo) i).ToList();
                }
                if (num5 != 0 && !this.Config.ChestNames.Contains(name))
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Main.chest[chestID].item[i].TurnToAir();
                    }
                }
                else
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Main.chest[chestID].item[i].SetDefaults(ItemID.Coal);
                    }
                }
                Storage.SaveBankItems(player.Account.ID, Main.chest[chestID].name, piggyBank);
            }
            break;
        }
    }

    private void HandleCommand(CommandArgs args)
    {
        switch(args.Parameters[0])
        {
            case "add":
                int userID = int.Parse(args.Parameters[1]);
                string chestName = args.Parameters[2];
                int itemID = int.Parse(args.Parameters[3]);
                int stack = int.Parse(args.Parameters[4]);
                int prefix = int.Parse(args.Parameters[5]);
                this.Storage.AddBankItems(userID, chestName, (itemID, stack, prefix));
                break;
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
            .GetMethod(nameof(PostHotReload))!
            .Invoke(instance, Array.Empty<object>());
        #endregion
    }
    #endregion
}