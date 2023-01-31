using Microsoft.Xna.Framework;
using Rests;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ProgressQuery;

[ApiVersion(2, 1)]//api版本
public class MainPlugin : TerrariaPlugin
{
    public override string Author => "少司命";// 插件作者
    public override string Description => "查询游戏进度";// 插件说明
    public override string Name => "进度查询";// 插件名字
    public override Version Version => new Version(1, 0, 0, 5);// 插件版本

    public MainPlugin(Main game) : base(game)// 插件处理
    {
        Order = 3;
    }

    public override void Initialize()
    {
        string[] cmd = { "进度查询", "查询进度", "progress" };
        TShock.RestApi.Register(new SecureRestCommand("/Progress", progress, "ProgressQuery.use"));
        Commands.ChatCommands.Add(new Command("ProgressQuery.use", Query, cmd));
        Commands.ChatCommands.Add(new Command("Progress.admin", Set, "进度设置"));
    }

    private void Set(CommandArgs args)
    {
        var exec = true;
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage("输入/进度设置 <名称> <true|false>");
            return;
        }
        bool code;
        if (args.Parameters.Count == 2 && args.Parameters[1].ToLower() == "true")
        {
            code = true;
        }
        else if (args.Parameters.Count == 2 && args.Parameters[1].ToLower() == "false")
        {
            code = false;
        }
        else
        {
            args.Player.SendInfoMessage("输入/进度设置 <名称> <true|false>");
            return;
        }
       
        switch (args.Parameters[0])
        {
            case "史莱姆王":
                NPC.downedSlimeKing = code;
                break;
            case "克苏鲁之眼":
                NPC.downedBoss1 = code;
                break;
            case "世界吞噬怪":
                NPC.downedBoss2 = code;
                break;
            case "克苏鲁之脑":
                goto case "世界吞噬怪";
            case "骷髅王":
                NPC.downedBoss3 = code;
                break;
            case "独眼巨鹿":
                NPC.downedDeerclops = code;
                break;
            case "蜂王":
                NPC.downedQueenBee = code;
                break;
            case "血肉墙":
                Main.hardMode = code;
                break;
            case "史莱姆皇后":
                NPC.downedQueenSlime = code;
                break;
            case "双子魔眼":
                NPC.downedMechBoss2 = code;
                break;
            case "毁灭者":
                NPC.downedMechBoss1 = code;
                break;
            case "机械骷髅王":
                NPC.downedMechBoss3 = code;
                break;
            case "世纪之花":
                NPC.downedPlantBoss = code;
                break;
            case "石巨人":
                NPC.downedGolemBoss = code;
                break;
            case "猪鲨":
                NPC.downedFishron = code;
                break;
            case "光之女皇":
                NPC.downedEmpressOfLight = code;
                break;
            case "拜月邪教徒":
                NPC.downedAncientCultist = code;
                break;
            case "日耀柱":
                NPC.downedTowerSolar = code;
                break;
            case "星云柱":
                NPC.downedTowerNebula = code;
                break;
            case "星旋柱":
                NPC.downedTowerVortex = code;
                break;
            case "星尘柱":
                NPC.downedTowerStardust = code;
                break;
            case "月球领主":
                NPC.downedMoonlord = code;
                break;
            case "雪人军团":
                NPC.downedFrost = code;
                break;
            case "哥布林军队":
                NPC.downedGoblins = code;
                break;
            case "海盗入侵":
                NPC.downedPirates = code;
                break;
            case "南瓜月":
                NPC.downedHalloweenKing = code;
                break;
            case "火星暴乱":
                NPC.downedMartians = code;
                break;
            case "霜月":
                NPC.downedChristmasIceQueen = code;
                break;
            default:
                exec = true;
                args.Player.SendErrorMessage("不包含此进度，无法设置!");
                break;
        }
        if (!exec)
        {
            args.Player.SendSuccessMessage("进度设置成功!");
        }
    }

    private object progress(RestRequestArgs args)
    {
        Dictionary<string, bool> Bosst = BossData();
        RestObject obj = new RestObject
        {
            Status = "200",
            Response = "查询成功"
        };
        obj["data"] = Bosst;
        return obj;
    }

    private Dictionary<string, Boolean> BossData()
    {
        Dictionary<string, bool> Progress = new Dictionary<string, bool>();
        Progress.Add("史莱姆王", NPC.downedSlimeKing);
        Progress.Add("克苏鲁之眼", NPC.downedBoss1);
        Progress.Add("世界吞噬怪", NPC.downedBoss2);
        Progress.Add("克苏鲁之脑", NPC.downedBoss2);
        Progress.Add("骷髅王", NPC.downedBoss3);
        Progress.Add("独眼巨鹿", NPC.downedDeerclops);
        Progress.Add("蜂王", NPC.downedQueenBee);
        Progress.Add("血肉墙", Main.hardMode);
        Progress.Add("史莱姆皇后", NPC.downedQueenSlime);
        Progress.Add("双子魔眼", NPC.downedMechBoss2);
        Progress.Add("毁灭者", NPC.downedMechBoss1);
        Progress.Add("机械骷髅王", NPC.downedMechBoss3);
        Progress.Add("世纪之花", NPC.downedPlantBoss);
        Progress.Add("石巨人", NPC.downedGolemBoss);
        Progress.Add("猪鲨", NPC.downedFishron);
        Progress.Add("光之女皇", NPC.downedEmpressOfLight);
        Progress.Add("拜月教邪教徒", NPC.downedAncientCultist);
        Progress.Add("日耀柱", NPC.downedTowerSolar);
        Progress.Add("星云柱", NPC.downedTowerNebula);
        Progress.Add("星旋柱", NPC.downedTowerVortex);
        Progress.Add("星尘柱", NPC.downedTowerStardust);
        Progress.Add("月球领主", NPC.downedMoonlord);
        Progress.Add("雪人军团", NPC.downedFrost);
        Progress.Add("哥布林军队", NPC.downedGoblins);
        Progress.Add("海盗入侵", NPC.downedPirates);
        Progress.Add("南瓜月", NPC.downedHalloweenKing);
        Progress.Add("火星暴乱", NPC.downedMartians);
        Progress.Add("霜月", NPC.downedChristmasIceQueen);
        return Progress;

    }
    private void Query(CommandArgs args)
    {
        Dictionary<string, bool> Progress = BossData();
        string Killed = "目前已击杀:";
        string NotKilled = "目前未击杀:";
        foreach (var Boss in Progress)
        {
            if (Boss.Value)
            {
                Killed += Boss.Key + ", ";
            }
            else
            {
                NotKilled += Boss.Key + ", ";
            }
        }
        args.Player.SendInfoMessage(Killed.TrimEnd(',', ' ').Color(TShockAPI.Utils.GreenHighlight));
        args.Player.SendInfoMessage(NotKilled.TrimEnd(',', ' ').Color(TShockAPI.Utils.PinkHighlight));
    }

    protected override void Dispose(bool disposing)// 插件关闭时
    {
        base.Dispose(disposing);
    }
}