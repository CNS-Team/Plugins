using Rests;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CGive;

[ApiVersion(2, 1)]
public class Main : TerrariaPlugin
{
    public override string Author => "Leader";

    public override string Description => "离线give";

    public override string Name => "CGive";

    public override Version Version => new Version(1, 0, 0, 0);

    public Main(Terraria.Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("cgive.admin", cgive, "cgive"));
        ServerApi.Hooks.GameInitialize.Register(this, OnGameInit);
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
        TShock.RestApi.Register("/getWarehouse", getWarehouse);
    }

    private object getWarehouse(RestRequestArgs args)
    {
        string text = args.Parameters["name"];
        string specifier = Commands.Specifier;
        if (TShock.UserAccounts.GetUserAccountsByName(text).Count == 0)
        {
            return new RestObject("201")
            {
                Response = "没有找到注册名为" + text + "的账户"
            };
        }
        List<Warehouse> list = new List<Warehouse>();
        foreach (CGive item2 in CGive.GetCGive())
        {
            if (!(item2.who == text))
            {
                continue;
            }
            string[] array = item2.cmd.Split(' ');
            if (array.Length < 3 || (!(array[0].ToLower() == specifier + "give") && !(array[0].ToLower() == specifier + "g")))
            {
                continue;
            }
            int result = 0;
            if (array.Length >= 4 && !int.TryParse(array[3], out result))
            {
                continue;
            }
            List<Item> itemByIdOrName = TShock.Utils.GetItemByIdOrName(array[1]);
            if (itemByIdOrName.Count == 0)
            {
                continue;
            }
            Item item = itemByIdOrName[0];
            if (array.Length == 3)
            {
                list.Add(new Warehouse(item.maxStack, item.netID));
                continue;
            }
            if (array.Length == 4)
            {
                list.Add(new Warehouse(result, item.netID));
                continue;
            }
            List<int> prefixByIdOrName = TShock.Utils.GetPrefixByIdOrName(array[4]);
            if (prefixByIdOrName.Count != 0)
            {
                list.Add(new Warehouse(result, item.netID, prefixByIdOrName[0]));
            }
        }
        if (list.Count == 0)
        {
            return new RestObject("202")
            {
                Response = "该玩家没有仓库"
            };
        }
        return new RestObject("200")
        {
            { "response", "获取成功" },
            { "data", list }
        };
    }

    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        foreach (CGive item in CGive.GetCGive())
        {
            if (item.who == "-1")
            {
                Given given = new Given
                {
                    Name = TShock.Players[args.Who].Name,
                    id = item.id
                };
                if (!given.IsGiven())
                {
                    item.who = given.Name;
                    if (item.Execute())
                    {
                        given.Save();
                    }
                }
            }
            else if (item.Execute())
            {
                item.Del();
            }
        }
    }

    private void OnGameInit(EventArgs args)
    {
        Data.Init();
    }

    private void cgive(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage("/cgive personal 命令 被执行者");
            args.Player.SendInfoMessage("/cgive all 执行者 命令");
            args.Player.SendInfoMessage("/cgive list,列出所有离线命令");
            args.Player.SendInfoMessage("/cgive del id,删除指定id的离线命令");
            args.Player.SendInfoMessage("/cgive init,重置所有数据");
            return;
        }
        switch (args.Parameters[0])
        {
            case "init":
                Data.Command("delete from CGive,Given");
                args.Player.SendSuccessMessage("成功删除所有数据");
                break;
            case "del":
            {
                CGive cGive2 = new CGive();
                cGive2.id = int.Parse(args.Parameters[1]);
                cGive2.Del();
                args.Player.SendSuccessMessage("已执行删除");
                break;
            }
            case "list":
            {
                foreach (CGive item in CGive.GetCGive())
                {
                    TSPlayer player = args.Player;
                    player.SendInfoMessage($"执行者:{item.Executer} 被执行者:{item.who} 命令:{item.cmd} id:{item.id}");
                }
                break;
            }
            case "all":
            {
                string executer2 = args.Parameters[1];
                string cmd2 = args.Parameters[2];
                string who2 = "-1";
                CGive cGive3 = new CGive
                {
                    who = who2,
                    Executer = executer2,
                    cmd = cmd2
                };
                cGive3.Execute();
                break;
            }
            case "personal":
            {
                string executer = "Server";
                string who = args.Parameters[2];
                string cmd = args.Parameters[1];
                CGive cGive = new CGive
                {
                    Executer = executer,
                    who = who,
                    cmd = cmd
                };
                if (!cGive.Execute())
                {
                    args.Player.SendInfoMessage("命令已保存");
                    cGive.Save();
                }
                else
                {
                    args.Player.SendInfoMessage("命令执行成功！");
                }
                break;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll((Command CommandDelegate) => CommandDelegate.HasAlias("cgive"));
            ServerApi.Hooks.GameInitialize.Deregister(this, OnGameInit);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
        }
        base.Dispose(disposing);
    }
}
