using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace QwRPG.shop;

[ApiVersion(2, 1)]//api版本
public class ShopPlugin : TerrariaPlugin
{
    /// <summary>
    /// 插件作者
    /// </summary>
    public override string Author => "奇威复反";
    /// <summary>
    /// 插件说明
    /// </summary>
    public override string Description => "Chrome.RPG配套商店插件";
    /// <summary>
    /// 插件名字
    /// </summary>
    public override string Name => "Chrome.Shop";
    /// <summary>
    /// 插件版本
    /// </summary>
    public override Version Version => new(1, 2, 0, 0);
    /// <summary>
    /// 插件处理
    /// </summary>
    public ShopPlugin(Main game) : base(game)
    {
    }
    //插件启动时，用于初始化各种狗子
    public static Config 配置 = new();
    public static Config.QwRPG配置表 Qw配置 = new();
    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);//钩住游戏初始化时
        Config.GetConfig();
        Reload();
    }
    /// <summary>
    /// 插件关闭时
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Deregister hooks here
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);//销毁游戏初始化狗子

        }
        base.Dispose(disposing);
    }

    private void OnInitialize(EventArgs args)//游戏初始化的狗子
    {
        //第一个是权限，第二个是子程序，第三个是指令
        Commands.ChatCommands.Add(new Command("QwRPG.admin", this.重载, "reload") { });
        Commands.ChatCommands.Add(new Command("QwRPG.shop", this.ShopCommand, "shop") { });
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
                    var i = 0;
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
                            args.Player.SendMessage($"[c/FF0000:{s + 1}.]||{配置.Shops[s].显示名}[i/s{配置.Shops[s].商品[0].数量}:{配置.Shops[s].商品[0].物品}]||[c/FF8000:{配置.Shops[s].价格}{Qw配置.货币名}]", 255, 255, 255);
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
                    var 货币 = DB.QueryCost(args.Player.Name);
                    var 货币名 = Qw配置.货币名;
                    long 价格 = 商品.价格;
                    var 进度 = 商品.进度限制;
                    if (进度.Count > 0)
                    {
                        var 当前进度 = this.Progress(args);
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
                    DB.DelCost(args.Player.Name, 价格);
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
    private void 重载(CommandArgs args)
    {
        try
        {
            Reload();
            //args.Player.SendErrorMessage($"[QwRPG.Shop]重载成功！");
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.Shop]配置文件读取错误");
        }
    }
    public static void Reload()
    {
        try
        {
            配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine("tshock/Chrome.Shop.json")));
            File.WriteAllText("tshock/Chrome.Shop.json", JsonConvert.SerializeObject(配置, Formatting.Indented));
            Qw配置 = JsonConvert.DeserializeObject<Config.QwRPG配置表>(File.ReadAllText(Path.Combine("tshock/Chrome.RPG.json")));
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[QwRPG.Shop]配置文件读取错误");
        }
    }
    private List<string> Progress(CommandArgs args)//获取进度详情
    {
        var list = new List<string>();
        if (NPC.downedSlimeKing)//史莱姆王
        {
            list.Add("史莱姆王");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedBoss1)//克苏鲁之眼
        {
            list.Add("克苏鲁之眼");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedBoss2)//世界吞噬者 或 克苏鲁之脑
        {
            list.Add("世界吞噬者,克苏鲁之脑");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedQueenBee)//蜂后
        {
            list.Add("蜂王");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedBoss3)//骷髅王
        {
            list.Add("骷髅王");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedDeerclops)//独眼巨鹿
        {
            list.Add("独眼巨鹿");
        }
        else
        {
            list.Add("");
        }
        if (Main.hardMode)
        {
            list.Add("血肉墙");
        }
        else
        {
            list.Add("");
        }

        if (NPC.downedQueenSlime)
        {
            list.Add("史莱姆皇后");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedMechBoss2)
        {
            list.Add("双子魔眼");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedMechBoss1)
        {
            list.Add("毁灭者");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedMechBoss3)
        {
            list.Add("机械骷髅王");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedPlantBoss)
        {
            list.Add("世纪之花");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedGolemBoss)
        {
            list.Add("石巨人");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedFishron)
        {
            list.Add("猪龙鱼公爵");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedEmpressOfLight)
        {
            list.Add("光之女皇");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedAncientCultist)
        {
            list.Add("拜月教邪教徒");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedMoonlord)
        {
            list.Add("月亮领主");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedGoblins)
        {
            list.Add("哥布林军队");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedPirates)
        {
            list.Add("海盗入侵");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedChristmasTree && NPC.downedChristmasSantank && NPC.downedChristmasIceQueen)
        {
            list.Add("霜月");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedHalloweenTree && NPC.downedHalloweenKing)
        {
            list.Add("南瓜月");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedMartians)
        {
            list.Add("火星暴乱");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedTowerSolar)
        {
            list.Add("日耀柱");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedTowerVortex)
        {
            list.Add("星旋柱");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedTowerNebula)
        {
            list.Add("星云柱");
        }
        else
        {
            list.Add("");
        }
        if (NPC.downedTowerStardust)
        {
            list.Add("星尘柱");
        }
        else
        {
            list.Add("");
        }
        return list;
    }
}