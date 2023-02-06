using Chrome.RPG;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;


namespace deal
{
    [ApiVersion(2, 1)]//api版本
    public class deal : TerrariaPlugin
    {
        /// 插件作者
        public override string Author => "奇威复反";
        /// 插件说明
        public override string Description => "Chrome.RPG 玩家交易插件";
        /// 插件名字
        public override string Name => "Chrome.Deal";
        /// 插件版本
        public override Version Version => new(1, 5, 0, 0);
        /// 插件处理
        public deal(Main game) : base(game)
        {
        }
        //插件启动时，用于初始化各种狗子
        public static Config 配置 = new();
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);//钩住游戏初始化时
            GeneralHooks.ReloadEvent += new GeneralHooks.ReloadEventD(Config.Reload);
            Config.GetConfig();
            Config.Reload();
        }
        /// 插件关闭时
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Deregister hooks here
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);//销毁游戏初始化狗子
                GeneralHooks.ReloadEvent -= new GeneralHooks.ReloadEventD(Config.Reload);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)//游戏初始化的狗子
        {
            //第一个是权限，第二个是子程序，第三个是指令
            Commands.ChatCommands.Add(new Command("QwRPG.use", Deal, "deal") { });
        }
        private void Deal(CommandArgs args)
        {
            var plr = args.Player;
            var 货币名 = Chrome.RPG.Chrome.配置.货币名;
            if (args.Parameters.Count == 0 || args.Parameters[0] == "help")
            {
                plr.SendInfoMessage("/deal sell <价格> --上架手持物品");
                plr.SendInfoMessage("/deal buy <ID> --购买物品/收回自己上架的物品");
                plr.SendInfoMessage("/deal list <页数> --查看所有上架的物品");
                //plr.SendInfoMessage("/deal me <页数> --查看自己上架的物品");
                if (args.Player.HasPermission("QwRPG.admin"))
                {
                    plr.SendInfoMessage("/deal remove <ID> --销毁指定ID的物品");
                }
                return;
            }
            switch (args.Parameters[0])
            {
                case "sell":
                case "出售":
                case "上架":
                    if (!plr.IsLoggedIn)
                    {
                        plr.SendErrorMessage($"请先登录！");
                        return;
                    }
                    if (plr.SelectedItem.stack == 0 || plr.SelectedItem.netID == 0)
                    {
                        plr.SendErrorMessage($"请手持要上架的物品！");
                        return;
                    }
                    long 价格;
                    if (args.Parameters.Count != 2)
                    {
                        plr.SendInfoMessage("/deal sell <价格> --上架手持物品");
                        return;
                    }
                    if (!long.TryParse(args.Parameters[1], out 价格))
                    {
                        plr.SendErrorMessage($"价格错误！！");
                        return;
                    }
                    if (价格 < 0 || 价格 > 配置.最大价格)
                    {
                        plr.SendErrorMessage($"价格错误！");
                        return;
                    }
                    var item = plr.SelectedItem;
                    DB.AddItem(plr.Name, 价格, item.netID, item.prefix, item.stack);
                    string t = GetItemDesc(item.netID, item.stack, item.prefix);
                    item.netID = 0; item.stack = 0;
                    args.Player.SendData(PacketTypes.PlayerSlot, null, args.Player.Index, plr.TPlayer.selectedItem); //移除玩家背包内的物品
                    plr.SendInfoMessage($"上架{t}成功");
                    if (配置.广播上架物品) TShock.Utils.Broadcast($"{plr.Name}上架了{t},售价{价格}{Chrome.RPG.Chrome.配置.货币名}", 255, 178, 102);
                    break;
                case "buy":
                case "购买":
                    if (!plr.IsLoggedIn)
                    {
                        plr.SendErrorMessage($"请先登录！");
                        return;
                    }
                    int ID;
                    if (!int.TryParse(args.Parameters[1], out ID))
                    {
                        plr.SendErrorMessage($"不存在该商品！");
                        return;
                    }
                    if (!DB.QueryShop(ID))
                    {
                        plr.SendErrorMessage($"不存在该商品");
                        return;
                    }
                    价格 = DB.QueryPrice(ID);
                    if (价格 < 0)
                    {
                        plr.SendErrorMessage($"购买失败！该商品价格异常");
                        return;
                    }
                    string 卖家 = DB.QueryOwner(ID);
                    int NetId;
                    int stack;
                    int prefix;
                    int[] ShopItem;
                    if (卖家 == plr.Name)
                    {
                        ShopItem = DB.GetShop(ID);
                        NetId = ShopItem[0]; stack = ShopItem[1]; prefix = ShopItem[2];
                        if (NetId == 0 || stack == 0)
                        {
                            plr.SendErrorMessage($"收回失败！获取商品ID失败，请尝试重新收回");
                            return;
                        }
                        DB.DelItem(ID);
                        Chrome.RPG.DB.AddCost(卖家, 价格);
                        plr.GiveItem(NetId, stack, prefix);
                        plr.SendInfoMessage($"收回成功");
                        if (配置.广播下架物品) TShock.Utils.Broadcast($"{plr.Name}下架了ID为：{ID}的商品", 255, 178, 102);
                        return;
                    }
                    long 货币 = Chrome.RPG.DB.QueryCost(args.Player.Name);
                    if (货币 < 价格)
                    {
                        args.Player.SendInfoMessage($"您的{货币名}不足，该商品需要{价格}{货币名}");
                        return;
                    }
                    ShopItem = DB.GetShop(ID);
                    NetId = ShopItem[0]; stack = ShopItem[1]; prefix = ShopItem[2];
                    if (NetId == 0 || stack == 0)
                    {
                        plr.SendErrorMessage($"购买失败！获取商品ID失败，请尝试重新购买");
                        return;
                    }
                    DB.DelItem(ID);
                    if (卖家 != "") Chrome.RPG.DB.AddCost(卖家, (long)(价格 * (1 - 配置.税率)));
                    Chrome.RPG.DB.DelCost(plr.Name, 价格);
                    plr.GiveItem(NetId, stack, prefix);
                    plr.SendInfoMessage($"购买成功");
                    if (配置.广播购买成功) TShock.Utils.Broadcast($"{plr.Name}购买了{卖家}的ID为：{ID}的商品,成交价{价格}{货币名},税后{价格 * (1 - 配置.税率)}{货币名}", 255, 178, 102);
                    break;
                case "list":
                case "列表":
                    int s;
                    int y;
                    if (args.Parameters.Count <= 1)
                    {
                        y = 1;
                        s = 0;
                    }
                    else
                    {
                        y = Convert.ToInt32(args.Parameters[1]);
                        s = Convert.ToInt32(args.Parameters[1]) - 1;
                    }
                    if (y <= 0)
                    {
                        args.Player.SendErrorMessage("这页没有商品");
                        return;
                    }
                    int i = 0;
                    for (s = 10 * s; s < DB.GetMaxId(); s++, i++)
                    {
                        if (i >= 10)
                        {
                            if (DB.GetMaxId() >= s + 1)
                            {
                                args.Player.SendInfoMessage($"输入\"/deal list {y + 1}\"查看下一页");
                                return;
                            }
                            return;
                        }
                        else
                        {
                            using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_Deal WHERE `ID` = @0", s + 1))
                            {
                                if (表.Read())
                                {
                                    ID = 表.Get<int>("ID");
                                    string 玩家名 = 表.Get<string>("玩家名");
                                    价格 = 表.Get<long>("价格");
                                    int 物品 = 表.Get<int>("物品");
                                    int 前缀 = 表.Get<int>("前缀");
                                    int 数量 = 表.Get<int>("数量");
                                    args.Player.SendMessage($"[c/FF0000:{ID}.]||{玩家名}[i/s{数量}:{物品}],前缀:{前缀}||[c/FF8000:{价格}{货币名}]", 255, 255, 255);

                                }
                                continue;
                            }
                        }
                    }
                    args.Player.SendInfoMessage("已展示所有商品");
                    break;
                case "remove":
                case "销毁":
                    if (!args.Player.HasPermission("QwRPG.admin")) { args.Player.SendErrorMessage("你没有权限使用此命令!"); return; }
                    if (!int.TryParse(args.Parameters[1], out ID))
                    {
                        plr.SendErrorMessage($"不存在该商品！");
                        return;
                    }
                    if (!DB.QueryShop(ID))
                    {
                        plr.SendErrorMessage($"不存在该商品");
                        return;
                    }
                    DB.DelItem(ID);
                    plr.SendInfoMessage($"销毁成功");
                    if (配置.广播下架物品) TShock.Utils.Broadcast($"[管理]{plr.Name}销毁了ID为：{ID}的商品", 255, 178, 102);
                    break;
                default:
                    plr.SendInfoMessage("/deal sell <价格> --上架手持物品");
                    plr.SendInfoMessage("/deal buy <ID> --购买物品/收回自己上架的物品");
                    plr.SendInfoMessage("/deal list <页数> --查看所有上架的物品");
                    if (args.Player.HasPermission("QwRPG.admin"))
                    {
                        plr.SendInfoMessage("/deal remove <ID> --销毁指定ID的物品");
                    }
                    break;
            }
        }
        public static string GetItemDesc(int NetId, int Stack = 1, int PrefixId = 0)
        {
            if (Stack > 1)
            {
                return $"[i/s{Stack}:{NetId}]";
            }
            else
            {
                if (PrefixId == 0)
                    return $"[i:{NetId}]";
                else
                    return $"[i/p{PrefixId}:{NetId}]";
            }
        }

    }
}