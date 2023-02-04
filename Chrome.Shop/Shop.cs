using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace QwRPG.shop
{
    [ApiVersion(2, 1)]//api版本
    public class Shop : TerrariaPlugin
    {
        /// 插件作者
        public override string Author => "奇威复反";
        /// 插件说明
        public override string Description => "Chrome.RPG配套商店插件";
        /// 插件名字
        public override string Name => "Chrome.Shop";
        /// 插件版本
        public override Version Version => new(1, 2, 0, 0);
        /// 插件处理
        public Shop(Main game) : base(game)
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
            Commands.ChatCommands.Add(new Command("QwRPG.shop", ShopCommand, "shop") { });
        }
        private void ShopCommand(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0 || args.Parameters[0] == "help")
                {
                    args.Player.SendInfoMessage("/shop list <页码> --查看商品");
                    args.Player.SendInfoMessage("/shop buy <商品ID> --购买");
                    return;
                }
                switch (args.Parameters[0])
                {
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
                            if (int.TryParse(args.Parameters[1], out y) && int.TryParse(args.Parameters[1], out s))
                            {
                                s--;
                            }
                            else
                            {
                                args.Player.SendErrorMessage("这页没有商品!!");
                                return;
                            }
                        }
                        if (y <= 0)
                        {
                            args.Player.SendErrorMessage("这页没有商品");
                            return;
                        }
                        int i = 0;
                        for (s = 10 * s; s < 配置.Shops.Count; s++, i++)
                        {
                            if (i >= 10)
                            {
                                if (配置.Shops.Count >= s + 1)
                                {
                                    args.Player.SendInfoMessage($"输入\"/shop list {y + 1}\"查看下一页");
                                    return;
                                }
                                return;
                            }
                            else
                            {
                                args.Player.SendMessage($"[c/FF0000:{s + 1}.]||{配置.Shops[s].显示名}[i/s{配置.Shops[s].商品[0].数量}:{配置.Shops[s].商品[0].物品}]||[c/FF8000:{配置.Shops[s].价格}{Chrome.RPG.Chrome.配置.货币名}]", 255, 255, 255);
                            }
                        }
                        args.Player.SendInfoMessage("已展示所有商品");
                        break;
                    case "buy":
                    case "购买":
                        if (!args.Player.IsLoggedIn)
                        {
                            args.Player.SendErrorMessage("你必须登录后执行此命令!");
                            return;
                        }
                        if (args.Parameters.Count == 1)
                        {
                            args.Player.SendErrorMessage("用法错误：/shop buy <商品ID>");
                            return;
                        }
                        s = Convert.ToInt32(args.Parameters[1]) - 1;

                        if (配置.Shops.Count < s)
                        {
                            args.Player.SendErrorMessage("没有该商品");
                            return;
                        }
                        var 商品 = 配置.Shops[s];
                        long 货币 = Chrome.RPG.DB.QueryCost(args.Player.Name);
                        string 货币名 = Chrome.RPG.Chrome.配置.货币名;
                        long 价格 = 商品.价格;
                        List<string> 进度 = 商品.进度限制;
                        if (进度.Count > 0)
                        {
                            List<string> 当前进度 = Chrome.RPG.Chrome.进度详情();
                            foreach (var b in 进度)
                            {
                                if (当前进度.Exists(a => a == b))
                                {

                                }
                                else
                                {
                                    args.Player.SendInfoMessage($"购买失败。需要打败[c/FF3333:{b}]");
                                    return;
                                }
                            }
                        }
                        if (货币 < 价格)
                        {
                            args.Player.SendInfoMessage($"您的{货币名}不足，该商品需要{价格}{货币名}");
                            return;
                        }
                       Chrome.RPG.DB.DelCost(args.Player.Name, 价格);
                        foreach (var a in 商品.商品)
                        {
                            args.Player.GiveItem(a.物品, a.数量, a.前缀);
                        }
                        args.Player.SendInfoMessage($"购买成功");
                        break;
                    default:
                        args.Player.SendInfoMessage("/shop list <页码> --查看商品");
                        args.Player.SendInfoMessage("/shop buy <商品ID> --购买");
                        break;
                }
            }
            catch
            {
                args.Player.SendErrorMessage($"[Chrome.Shop]发生错误！");
            }
        }

    }
}