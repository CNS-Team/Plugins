using StatusTxtMgr;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Chrome.RPG
{
    [ApiVersion(2, 1)]//api版本
    public class Chrome : TerrariaPlugin
    {
        #region 插件信息
        /// 插件作者
        public override string Author => "奇威复反";
        /// 插件说明
        public override string Description => "Chrome.RPG，一个RPG系统";
        /// 插件名字
        public override string Name => "Chrome.RPG";
        /// 插件版本
        public override Version Version => new(1, 5, 0, 0);
        /// 插件处理
        public Chrome(Main game) : base(game)
        {
        }
        #endregion
        public static Config 配置 = new();
        #region 钩子
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);//钩住游戏初始化时
            ServerApi.Hooks.ServerJoin.Register(this, new HookHandler<JoinEventArgs>(this.OnJoin));
            ServerApi.Hooks.NpcKilled.Register(this, new HookHandler<NpcKilledEventArgs>(this.OnNpcKill));//动物死亡钩子
            GetDataHandlers.KillMe += Kill;
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            global::StatusTxtMgr.StatusTxtMgr.Hooks.StatusTextUpdate.Register(StatusText, 180uL);
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
                ServerApi.Hooks.ServerJoin.Deregister(this, new HookHandler<JoinEventArgs>(this.OnJoin));
                ServerApi.Hooks.NpcKilled.Deregister(this, new HookHandler<NpcKilledEventArgs>(this.OnNpcKill));//动物死亡钩子
                GetDataHandlers.KillMe -= Kill;
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
                global::StatusTxtMgr.StatusTxtMgr.Hooks.StatusTextUpdate.Deregister(StatusText);
                GeneralHooks.ReloadEvent -= new GeneralHooks.ReloadEventD(Config.Reload);
            }
            base.Dispose(disposing);
        }
        private void OnJoin(JoinEventArgs args)
        {
            var plr = TShock.Players[args.Who];
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE 玩家名=@0", plr.Name))
            {
                if (!表.Read())
                    TShock.DB.Query("INSERT INTO Chrome_RPG (`玩家名`, `点券`, `职业`,`等级`,`是否显示`,`职业重置次数`) VALUES " + "(@0, @1, @2, @3, @4, @5);", plr.Name, 0, 配置.玩家初始职业, 1, "true", 0);
            }
        }
        private void OnNpcKill(NpcKilledEventArgs args)
        {
            if (args.npc.lastInteraction == 255 || args.npc.lastInteraction < 0)
            {
                return;
            }
            var plr = TShock.Players[args.npc.lastInteraction];
            if (!plr.IsLoggedIn)
            {
                return;
            }
            if (args.npc.SpawnedFromStatue && 配置.杀死雕像生成NPC不给货币)
            {
                return;
            }
            string 当前职业 = "无";
            //long 货币 = 0;
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM Chrome_RPG WHERE `玩家名`=@0", plr.Name))
            {
                if (表.Read())
                {
                    当前职业 = 表.Get<string>("职业");
                    //货币 = 表.Get<long>("点券");
                }
            }
            if (配置.击败肉山前不继续加货币职业.Exists(s => s == 当前职业))
            {
                return;
            }
            //if()
            if (配置.杀死不给货币NPC.Exists(s => s == args.npc.type)) { return; }


            var 怪物血量 = args.npc.lifeMax;
            DB.AddCost(plr.Name, (long)(怪物血量 * 配置.获取货币血量比例), true);
        }
        private void Kill(object? o, GetDataHandlers.KillMeEventArgs args)
        {
            if (!args.Player.IsLoggedIn)
            {
                return;
            }
            if (配置.死亡掉落货币百分率 < 1)
            {
                long 货币 = DB.QueryCost(args.Player.Name);
                if (货币 > 0)
                {
                    货币 = (long)(货币 * 配置.死亡掉落货币百分率);
                    DB.DelCost(args.Player.Name, 货币, true);
                }
            }
        }
        public static void OnChat(ServerChatEventArgs args)
        {
            var plr = TShock.Players[args.Who];
            string 职业名 = Config.查职业(plr.Name);
            var 职业 = Config.获取职业(职业名);
            if (职业 == null)
            {
                //plr.SendErrorMessage("未知职业!");
                return;
            }
            var 等级 = Config.获取等级(职业, Config.查等级(plr.Name));
            string 等级名;
            string 颜色;
            int 级;
            if (等级 == null)
            {
                等级名 = "";
                颜色 = "";
                级 = 1;
            }
            else
            {
                等级名 = 等级.等级名;
                级 = 等级.等级;
                颜色 = 等级.颜色;
            }
            string 前缀 = 职业.前缀.Replace("{等级}", 级.ToString()).Replace("{职业}", 职业名).Replace("{等级名}", 等级名).Replace("{颜色}", 颜色);
            string 后缀 = 职业.后缀.Replace("{等级}", 级.ToString()).Replace("{职业}", 职业名).Replace("{等级名}", 等级名).Replace("{颜色}", 颜色);
            称号插件.称号插件.称号信息[plr.Name].前前缀 = 前缀;
            称号插件.称号插件.称号信息[plr.Name].后后缀 = 后缀;
        }
        private void StatusText(StatusTextUpdateEventArgs args)
        {
            try
            {
                var plr = args.tsplayer;
                var sb = args.statusTextBuilder;
                long 货币 = DB.QueryCost(plr.Name);
                long 货币上限;
                if (Main.hardMode)
                {
                    货币上限 = 配置.肉山前最大货币数量;
                }
                else
                {
                    货币上限 = 配置.肉山后最大货币数量;
                }
                if (货币 > 货币上限)
                {
                    DB.AmountCost(plr.Name, 货币上限);
                    货币 = 货币上限;
                }
                if (配置.启用侧边栏信息 && DB.QueryStatus(plr.Name) == "true")
                {
                    string 职业名 = Config.查职业(plr.Name);
                    var 职业 = Config.获取职业(职业名);
                    if (职业 == null) return;
                    int 等级 = Config.查等级(plr.Name);
                    string percentText;
                    string 等级名;
                    long 升级货币;
                    var 级 = Config.获取等级(职业, 等级);
                    //string 颜色 = "";
                    if (级 == null)
                    {
                        等级名 = "";
                        //颜色 = "FFFFFF";
                    }
                    else
                    {
                        等级名 = 级.等级名;
                        //颜色 = 级.颜色;
                    }
                    if (!Config.是否有下级(职业, 等级))
                    {
                        percentText = "100%";
                        升级货币 = 0;
                    }
                    else
                    {
                        升级货币 = Config.获取升级所需货币(职业, 等级);
                        double percent = (double)(货币 / 升级货币);
                        percentText = percent.ToString("0.0%");
                        if (percent > 100)
                        {
                            percentText = "100%";
                        }
                    }

                    sb.AppendLine($"[c/F5AA19:—][c/F23812:流][c/F36C15:光][c/F48817:之][c/F5AA19:城]·[c/F5AA19:苍][c/F48817:穹][c/F36C15:传][c/F23812:说][c/F5AA19:—]\n[c/D2B369:职业]: [c/D166FF:{职业名}]\n[c/CB7942:等级]:[c/FFC466:{等级}] [c/D166FF:{等级名}]\n[c/53D9E2:{配置.货币名}]: [c/5CFD17:{货币}]/[c/FD4C17:{升级货币}] [c/FF6866:（{percentText}）]");
                }
            }
            catch (Exception e)
            {
                if (配置.显示报错)
                    TShock.Log.ConsoleInfo(e.ToString());
            }
        }
        #endregion
        #region 指令
        private void OnInitialize(EventArgs args)//游戏初始化的狗子
        {
            //第一个是权限，第二个是子程序，第三个是指令
            LanguageManager.Instance.SetLanguage(GameCulture.FromCultureName(GameCulture.CultureName.Chinese));
            Commands.ChatCommands.Add(new Command("QwRPG.use", Rank, "升级", "rank", "sj") { HelpText = "升级职业" });
            Commands.ChatCommands.Add(new Command("QwRPG.use", LookRank, "下一级") { HelpText = "查询下一级" });
            Commands.ChatCommands.Add(new Command("QwRPG.use", Query, "查询") { HelpText = "查询货币" });
            Commands.ChatCommands.Add(new Command("QwRPG.use", Bank, "bank") { HelpText = "操作经济系统." });
            Commands.ChatCommands.Add(new Command("QwRPG.use", Reset, "重置职业", "重选职业") { HelpText = "重置职业" });
            Commands.ChatCommands.Add(new Command("QwRPG.use", CmdStatus, "status", "切换显示") { HelpText = "切换显示" });
        }
        private static void CmdStatus(CommandArgs args)
        {
            if (!args.Player.IsLoggedIn)
            {
                args.Player.SendErrorMessage("你必须登录后执行此命令!");
                return;
            }
            string 当前状态 = DB.QueryStatus(args.Player.Name);
            if (当前状态 == "false")
            {
                DB.StatusGrade(args.Player.Name, "true");
                args.Player.SendInfoMessage("状态栏已切换为可见");
            }
            else
            {
                DB.StatusGrade(args.Player.Name, "false");
                args.Player.SendInfoMessage("状态栏已切换为不可见");
            }
        }
        private void Reset(CommandArgs args)
        {
            if (args.Parameters.Count == 0 || args.Parameters[0] != "yes")
            {
                args.Player.SendErrorMessage("请确定要重置职业：/重置职业 yes");
                return;
            }
            if (DB.QueryReset(args.Player.Name) >= 配置.玩家重置职业次数上限)
            {
                args.Player.SendErrorMessage("您重置职业的次数已达上限！");
                return;
            }

            DB.RankGrade(args.Player.Name, 配置.玩家初始职业);
            args.Player.tempGroup = TShock.Groups.GetGroupByName("superadmin");
            foreach (var z in 配置.重置职业指令)
            {
                if (z == null || z == "") { continue; }
                string text = string.Format(z, args.Player.Name);
                Commands.HandleCommand(args.Player, text);
            }
            args.Player.tempGroup = null;
            DB.AddReset(args.Player.Name);
            args.Player.SendInfoMessage("职业重置成功！");
        }
        private void Query(CommandArgs args)
        {
            string 货币名 = 配置.货币名;
            long 货币 = DB.QueryCost(args.Player.Name);
            args.Player.SendInfoMessage($"我拥有{货币}{货币名}");
        }
        private void Bank(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0 || args.Parameters[0] == "help")
                {
                    args.Player.SendInfoMessage("/bank query --查询货币");
                    args.Player.SendInfoMessage("/bank pay <用户名> <货币数量> --转账");
                    if (args.Player.HasPermission("QwRPG.admin"))
                    {
                        args.Player.SendInfoMessage("/bank clear <用户名> --清空货币");
                        args.Player.SendInfoMessage("/bank add <用户名> <货币数量> --给予货币");
                        args.Player.SendInfoMessage("/bank del <用户名> <货币数量> --收走货币");
                    }
                    return;
                }
                string 货币名 = 配置.货币名;
                switch (args.Parameters[0])
                {
                    case "query":
                    case "查询":
                        long 货币 = DB.QueryCost(args.Player.Name);
                        args.Player.SendInfoMessage($"我拥有{货币}{货币名}");
                        break;
                    case "pay":
                        if (args.Parameters.Count != 3)
                        {
                            args.Player.SendInfoMessage("/bank pay <用户名> <货币数量> --转账");
                            return;
                        }
                        货币 = DB.QueryCost(args.Player.Name);
                        long 给货币;
                        if (!long.TryParse(args.Parameters[2], out 给货币) || 给货币 <= 0)
                        {
                            args.Player.SendErrorMessage($"请输入正确的货币数量");
                            return;
                        }
                        if (货币 < 给货币)
                        {
                            args.Player.SendErrorMessage($"您没有这么多{货币名}，您只有{货币}{货币名}");
                            return;
                        }
                        if (!DB.IsExist(args.Parameters[1]))
                        {
                            args.Player.SendErrorMessage("没有找到该玩家");
                            return;
                        }
                        DB.AddCost(args.Parameters[1], 货币, true);
                        DB.DelCost(args.Player.Name, 给货币, true);
                        args.Player.SendInfoMessage($"转账成功！您还剩{货币名}{货币 - 给货币}");
                        break;
                    case "clear":
                        if (!args.Player.HasPermission("QwRPG.admin")) { args.Player.SendErrorMessage("你没有权限使用此命令!"); return; }
                        if (args.Parameters.Count != 2)
                        {
                            args.Player.SendInfoMessage("/bank clear <用户名> --清空货币");
                            return;
                        }
                        货币 = DB.QueryCost(args.Player.Name);
                        if (!DB.IsExist(args.Parameters[1]))
                        {
                            args.Player.SendErrorMessage("没有找到该玩家");
                            return;
                        }
                        DB.DelCost(args.Parameters[1], 货币, true);
                        args.Player.SendInfoMessage($"清空成功！");
                        break;
                    case "add":
                        给货币 = 0;

                        if (!args.Player.HasPermission("QwRPG.admin")) { args.Player.SendErrorMessage("你没有权限使用此命令!"); return; }
                        if (args.Parameters.Count != 3)
                        {
                            args.Player.SendInfoMessage("/bank add <用户名> <货币数量> --给予货币");
                            return;
                        }
                        if (!long.TryParse(args.Parameters[2], out 给货币))
                        {
                            args.Player.SendErrorMessage($"请输入正确的货币数量");
                            return;
                        }
                        if (!DB.IsExist(args.Parameters[1]))
                        {
                            args.Player.SendErrorMessage("没有找到该玩家");
                            return;
                        }
                        DB.AddCost(args.Parameters[1], 给货币, true);
                        args.Player.SendInfoMessage($"给予成功！");
                        break;
                    case "del":
                        if (!args.Player.HasPermission("QwRPG.admin")) { args.Player.SendErrorMessage("你没有权限使用此命令!"); return; }

                        if (args.Parameters.Count != 3)
                        {
                            args.Player.SendInfoMessage("/bank del <用户名> <货币数量> --收走货币");
                            return;
                        }
                        给货币 = 0;
                        if (!long.TryParse(args.Parameters[2], out 给货币))
                        {
                            args.Player.SendErrorMessage($"请输入正确的货币数量");
                            return;
                        }

                        if (!DB.IsExist(args.Parameters[1]))
                        {
                            args.Player.SendErrorMessage("没有找到该玩家");
                            return;
                        }
                        DB.DelCost(args.Parameters[1], 给货币, true);
                        args.Player.SendInfoMessage($"收走成功！");
                        break;
                    default:
                        args.Player.SendInfoMessage("/bank query --查询货币");
                        args.Player.SendInfoMessage("/bank pay <用户名> <货币数量> --转账");
                        if (args.Player.HasPermission("QwRPG.admin"))
                        {
                            args.Player.SendInfoMessage("/bank clear <用户名> --清空货币");
                            args.Player.SendInfoMessage("/bank add <用户名> <货币数量> --给予货币");
                            args.Player.SendInfoMessage("/bank del <用户名> <货币数量> --收走货币");
                        }
                        return;
                }
            }
            catch (Exception e)
            {
                args.Player.SendErrorMessage("发生错误！");
                TShock.Log.ConsoleInfo(e.ToString());
            }
        }
        private void LookRank(CommandArgs args)
        {
            var plr = args.Player;
            if (!plr.IsLoggedIn)
            {
                plr.SendErrorMessage("你必须登录后执行此命令!");
                return;
            }
            string 玩家名 = plr.Name;
            string 职业名 = Config.查职业(玩家名);
            var 职业 = Config.获取职业(职业名);
            if (职业 == null)
            {
                plr.SendErrorMessage("未知职业!");
                return;
            }
            int 等级 = Config.查等级(玩家名);
            if (Config.是否有下级(职业, 等级))
            {
                升级预览(plr, 职业, 等级);
            }
            else
            {
                List<Config.转职格式> 可转职职业 = 职业.转职职业;
                if (可转职职业.Count == 0)
                {
                    plr.SendInfoMessage("已升至最高等级!");
                    return;
                }
                转职预览(plr, 职业, 等级);
            }
        }

        private void Rank(CommandArgs args)
        {
            var plr = args.Player;
            if (!plr.IsLoggedIn)
            {
                plr.SendErrorMessage("你必须登录后执行此命令!");
                return;
            }
            string 玩家名 = plr.Name;
            string 职业名 = Config.查职业(玩家名);
            var 职业 = Config.获取职业(职业名);
            if (职业 == null)
            {
                plr.SendErrorMessage("未知职业!");
                return;
            }
            int 等级 = Config.查等级(玩家名);
            if (Config.是否有下级(职业, 等级))
            {
                升级操作(plr, Config.获取等级(职业, 等级));
            }
            else
            {
                var 可转职职业 = 职业.转职职业;
                if (可转职职业.Count == 0)
                {
                    args.Player.SendInfoMessage("已升至最高等级!");
                    return;
                }
                if (可转职职业.Count == 1)
                {

                    转职操作(args.Player, 职业, 可转职职业[0], Config.获取等级(职业, 等级));
                    return;
                }
                if (可转职职业.Count > 1 && args.Parameters.Count == 0)
                {
                    转职预览(plr, 职业, 等级);
                    return;
                }
                string 升级到职业 = args.Parameters[0];
                if (!可转职职业.Exists(f => f.职业 == 升级到职业))
                {
                    args.Player.SendErrorMessage($"无法升级到{升级到职业}，没有该选项!");
                    return;
                }
                转职操作(args.Player, 职业, 可转职职业.Find(s => s.职业 == 升级到职业), Config.获取等级(职业, 等级));
                return;
            }
        }
        #endregion
        #region 操作
        public static void 升级预览(TSPlayer plr, Config.职业格式 职业, int 等级)
        {
            plr.SendInfoMessage($"当前职业下一级预览：");
            var 级 = Config.获取等级(职业, 等级 + 1);
            long 升级货币 = 级.升级货币;
            plr.SendInfoMessage($"/升级({配置.货币名}{升级货币})");
            if (配置.转职时显示职业预览)
            {
                string 升级物品 = "";
                string 升级BUFF = "";
                foreach (var z in 级.升级指令)
                {
                    if (z.Contains("/g ") || z.Contains(".g "))//g 1 我 数量 前缀
                    {
                        var w = z.Split(" ");
                        if (w.Length == 2)
                        {
                            升级物品 = 升级物品 + $"[i:{w[1]}] ";
                        }
                        if (w.Length >= 3)
                        {
                            升级物品 = 升级物品 + $"[i/s{w[3]}:{w[1]}] ";
                        }
                    }
                    if (z.Contains(".gpermabuff ") || z.Contains("/gpermabuff"))//gpermabuff id 我
                    {
                        var b = z.Split(" ");
                        if (b.Length >= 2)
                        {
                            升级BUFF = 升级BUFF + "[" + TShock.Utils.GetBuffName(Convert.ToInt32(b[1])) + "]";
                        }
                    }
                }
                if (升级物品 == "")
                {
                    升级物品 = "无";
                }
                if (升级BUFF == "")
                {
                    升级BUFF = "无";
                }
                plr.SendInfoMessage($" >升级物品:{升级物品}");
                plr.SendInfoMessage($" >升级BUFF:{升级BUFF}");
            }
        }
        public static void 转职预览(TSPlayer plr, Config.职业格式 当前职业, int 等级)
        {
            plr.SendInfoMessage($"当前职业转职预览：");
            long 升级货币 = Config.获取升级所需货币(当前职业, 等级);
            foreach (var text in 当前职业.转职职业)
            {
                long 转职货币 = 升级货币 + text.转职货币;
                plr.SendInfoMessage($"/升级 {text.职业}({配置.货币名}{转职货币})");
                if (配置.转职时显示职业预览)
                {
                    string 升级物品 = "";
                    string 升级BUFF = "";
                    foreach (var z in text.转职指令)
                    {
                        if (z.Contains("/g ") || z.Contains(".g "))//g 1 我 数量 前缀
                        {
                            var w = z.Split(" ");
                            if (w.Length == 2)
                            {
                                升级物品 += $"[i:{w[1]}] ";
                            }
                            if (w.Length >= 3)
                            {
                                升级物品 += $"[i/s{w[3]}:{w[1]}] ";
                            }
                        }
                        if (z.Contains(".gpermabuff ") || z.Contains("/gpermabuff"))//gpermabuff id 我
                        {
                            var b = z.Split(" ");
                            if (b.Length >= 2)
                            {
                                升级BUFF = 升级BUFF + "[" + TShock.Utils.GetBuffName(Convert.ToInt32(b[1])) + "]";
                            }
                        }
                    }
                    if (升级物品 == "")
                    {
                        升级物品 = "无";
                    }
                    if (升级BUFF == "")
                    {
                        升级BUFF = "无";
                    }
                    plr.SendInfoMessage($" >升级物品:{升级物品}");
                    plr.SendInfoMessage($" >升级BUFF:{升级BUFF}");
                }
            }
        }
        public void 升级操作(TSPlayer plr, Config.等级格式 等级)//当前等级,当前职业
        {
            string 玩家名 = plr.Name;
            long 货币 = DB.QueryCost(玩家名);
            long 升级货币 = 等级.升级货币;
            string 货币名 = 配置.货币名;
            List<string> 进度 = 等级.进度限制;
            List<int> 任务 = 等级.任务限制;
            if (进度.Count > 0)
            {
                List<string> 当前进度 = 进度详情();
                foreach (var b in 进度)
                {
                    if (!当前进度.Contains(b))
                    {
                        plr.SendInfoMessage($"升级失败。需要打败[c/FF3333:{b}]");
                        return;
                    }
                }
            }
            if (任务.Count > 0)
            {
                List<int> 完成任务 = 任务系统.DB.QueryFinishTask(玩家名);
                string text = "";
                foreach (var b in 任务)
                {
                    if (!完成任务.Contains(b))
                    {
                        var r = 任务系统.任务系统.GetTask(b);
                        text += $"{r.任务ID}.{r.任务名称}\n";
                        plr.SendMessage($"[c/DCE923:升级失败。需要完成任务]\n{text}", 18, 252, 221);
                        return;
                    }
                }
            }
            if (等级.物品升级.Count != 0)
            {
                {
                    foreach (var r in 等级.物品升级)
                    {
                        var _item = r.Split(',');
                        bool item_bool = false;
                        for (int i = 10; i < 49; i++) //背包从第二排开始到最后一个背包位置
                        {
                            if (plr.TPlayer.inventory[i].netID == Convert.ToInt32(_item[0]))
                            {
                                if (plr.TPlayer.inventory[i].stack >= Convert.ToInt32(_item[1]))
                                {
                                    item_bool = true;
                                    break;
                                }
                                else
                                {

                                    plr.SendErrorMessage("没有足够的物品");
                                    return;
                                }
                            }
                        }
                        if (!item_bool)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您的背包中没有任务物品:][i/s{Convert.ToInt32(_item[1])}:{Convert.ToInt32(_item[0])}]");
                            plr.SendInfoMessage("[c/A066D3:任务物品请不要放在背包最上排中]");
                            return;
                        }
                    }
                }
                //消耗任务物品
                {
                    foreach (var r in 等级.物品升级)
                    {
                        var _item = r.Split(',');
                        for (int i = 10; i < 49; i++) //背包从第二排开始到最后一个背包位置
                        {
                            if (plr.TPlayer.inventory[i].netID == Convert.ToInt32(_item[0]))
                            {
                                if (plr.TPlayer.inventory[i].stack >= Convert.ToInt32(_item[1]))
                                {
                                    var stack = plr.TPlayer.inventory[i].stack -= Convert.ToInt32(_item[1]);
                                    任务系统.任务系统.PlayItemSet(plr.Index, i, Convert.ToInt32(_item[0]), stack);
                                    break;
                                }
                                else
                                {

                                    plr.SendErrorMessage("没有足够的物品");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (货币 < 升级货币)
            {
                plr.SendInfoMessage($"升级失败,你当前{货币名}{货币},升级所需{货币名}{升级货币}");
                return;
            }
            DB.DelCost(plr.Name, 升级货币, true);
            DB.RankGrade(玩家名);
            //args.Player.tempGroup = TShock.Groups.GetGroupByName("superadmin");
            foreach (var z in 等级.升级指令)
            {
                string text = string.Format(z, 玩家名);
                Commands.HandleCommand(TSPlayer.Server, text);
            }
            //args.Player.tempGroup = null;
            if (配置.升级广播) TShock.Utils.Broadcast($"恭喜[c/FFFF66:{玩家名}]升到[c/FFFF33:{等级.等级 + 1}]级!", 55, 169, 226);
        }
        public void 转职操作(TSPlayer plr, Config.职业格式 当前职业, Config.转职格式 转职职业, Config.等级格式 等级)
        {
            string 玩家名 = plr.Name;
            string 升级到职业 = 转职职业.职业;
            long 升级货币 = 等级.升级货币;
            long 转职货币 = 升级货币 + 转职职业.转职货币;
            List<string> 进度 = 等级.进度限制;
            List<int> 任务 = 等级.任务限制;
            long 当前货币 = DB.QueryCost(plr.Name);
            string 货币名 = 配置.货币名;
            if (进度.Count > 0)
            {
                List<string> 当前进度 = 进度详情();
                foreach (var b in 进度)
                {
                    if (!当前进度.Contains(b))
                    {
                        plr.SendInfoMessage($"转职失败。需要打败[c/FF3333:{b}]");
                        return;
                    }
                }
            }
            if (任务.Count > 0)
            {
                List<int> 完成任务 = 任务系统.DB.QueryFinishTask(玩家名);
                string text = "";
                foreach (var b in 任务)
                {
                    if (!完成任务.Contains(b))
                    {
                        var r = 任务系统.任务系统.GetTask(b);
                        text += $"{r.任务ID}.{r.任务名称}\n";
                        plr.SendMessage($"[c/DCE923:转职失败。需要完成任务]\n{text}", 18, 252, 221);
                        return;
                    }
                }
            }
            if (等级.物品升级.Count != 0)
            {
                {
                    foreach (var r in 等级.物品升级)
                    {
                        var _item = r.Split(',');
                        bool item_bool = false;
                        for (int i = 10; i < 49; i++) //背包从第二排开始到最后一个背包位置
                        {
                            if (plr.TPlayer.inventory[i].netID == Convert.ToInt32(_item[0]))
                            {
                                if (plr.TPlayer.inventory[i].stack >= Convert.ToInt32(_item[1]))
                                {
                                    item_bool = true;
                                    break;
                                }
                                else
                                {

                                    plr.SendErrorMessage("没有足够的物品");
                                    return;
                                }
                            }
                        }
                        if (!item_bool)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您的背包中没有任务物品:][i/s{Convert.ToInt32(_item[1])}:{Convert.ToInt32(_item[0])}]");
                            plr.SendInfoMessage("[c/A066D3:任务物品请不要放在背包最上排中]");
                            return;
                        }
                    }
                }
                //消耗任务物品
                {
                    foreach (var r in 等级.物品升级)
                    {
                        var _item = r.Split(',');
                        for (int i = 10; i < 49; i++) //背包从第二排开始到最后一个背包位置
                        {
                            if (plr.TPlayer.inventory[i].netID == Convert.ToInt32(_item[0]))
                            {
                                if (plr.TPlayer.inventory[i].stack >= Convert.ToInt32(_item[1]))
                                {
                                    var stack = plr.TPlayer.inventory[i].stack -= Convert.ToInt32(_item[1]);
                                    任务系统.任务系统.PlayItemSet(plr.Index, i, Convert.ToInt32(_item[0]), stack);
                                    break;
                                }
                                else
                                {

                                    plr.SendErrorMessage("没有足够的物品");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (当前货币 < 转职货币)
            {
                plr.SendInfoMessage($"转职失败,你当前{货币名}{当前货币},升级所需{货币名}{转职货币}");
                return;
            }
            DB.DelCost(plr.Name, 转职货币, true);
            DB.RankGrade(plr.Name, 升级到职业);
            //args.Player.tempGroup = TShock.Groups.GetGroupByName("superadmin");
            foreach (var z in 等级.升级指令)
            {
                string text = string.Format(z, 玩家名);
                Commands.HandleCommand(TSPlayer.Server, text);
            }
            foreach (var z in 转职职业.转职指令)
            {
                string text = string.Format(z, 玩家名);
                Commands.HandleCommand(TSPlayer.Server, text);
            }
            //args.Player.tempGroup = null;
            if (配置.升级广播) TShock.Utils.Broadcast($"恭喜[c/FFFF66:{玩家名}]从[c/FFFF33:{当前职业.职业}]转职为[c/FF8000:{转职职业.职业}]", 55, 169, 226);

        }



        public static List<string> 进度详情()//获取进度详情
        {
            List<string> list = new List<string>();
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
        #endregion
    }
}