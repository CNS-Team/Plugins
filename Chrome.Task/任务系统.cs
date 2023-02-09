using Newtonsoft.Json;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace 任务系统;

[ApiVersion(2, 1)]//api版本
public class 任务系统 : TerrariaPlugin
{
    /// <summary>
    /// 插件作者
    /// </summary>
    public override string Author => "奇威复反";
    /// <summary>
    /// 插件说明
    /// </summary>
    public override string Description => "任务系统";
    /// <summary>
    /// 插件名字
    /// </summary>
    public override string Name => "Chrome.Task";
    /// <summary>
    /// 插件版本
    /// </summary>
    public override Version Version => new(1, 0, 0, 0);
    /// <summary>
    /// 插件处理
    /// </summary>
    public 任务系统(Main game) : base(game)
    {
        this.Order = 2;
    }
    public static string path = "tshock/Chrome.Task.json";
    //插件启动时，用于初始化各种狗子
    public static 配置表 配置 = new();
    public static 配置表.解锁数据库 解锁 = new();
    public static List<int> 上锁NPC = new();
    public static 事件配置 事件设置 = new();
    public class 事件配置
    {
        // [JsonProperty("禁止满月")]
        public bool disableFullMoon = false;

        // [JsonProperty("禁止霜月")]
        public bool disableFrostMoon = false;

        //  [JsonProperty("禁止血月")]
        public bool disableBloodMoon = false;

        // [JsonProperty("禁止南瓜月")]
        public bool disablePumpkinMoon = false;

        //[JsonProperty("禁止日食")]
        public bool disableSolarEclipse = false;

        // [JsonProperty("禁止下雨")]
        public bool disableRain = false;

        //  [JsonProperty("禁止史莱姆雨")]
        public bool disableSlimeRain = false;

        // [JsonProperty("禁止哥布林入侵")]
        public bool disableGoblinInvasion = false;

        // [JsonProperty("禁止海盗入侵")]
        public bool disablePirateInvasion = false;

        // [JsonProperty("禁止雪人军团")]
        public bool disableFrostLegion = false;

        //  [JsonProperty("禁止下落陨铁")]
        public bool disableMeteors = false;

        // [JsonProperty("禁止火星人入侵")]
        public bool disableMartianInvasion = false;

        // [JsonProperty("禁止月球入侵")]
        public bool disableLunarInvasion = false;

        //[JsonProperty("禁止拜月教邪教徒")]
        public bool disableCultists = false;

        // [JsonProperty("禁止撒旦军团入侵")]
        public bool DD2Event = false;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);//钩住游戏初始化时
        ServerApi.Hooks.GamePostInitialize.Register(this, new HookHandler<EventArgs>(this.PostInitialize));
        ServerApi.Hooks.NpcKilled.Register(this, NpcKill);
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnJoin);
        GeneralHooks.ReloadEvent += new GeneralHooks.ReloadEventD(this.Reload);
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnNpcSpawn);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        Reload();
        DB.Connect();
    }
    private ulong TickCount;
    private void OnGameUpdate(EventArgs args)
    {
        try
        {
            var flag = false;
            if (this.TickCount % 15uL == 0)
            {
                for (var i = 0; i < Main.npc.Length; i++)
                {
                    var npc = Main.npc[i];
                    if (npc != null && npc.active && 上锁NPC.Contains(npc.type))
                    {
                        npc.active = false;
                        TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
                    }
                }
            }
            if (WorldGen.spawnMeteor && 事件设置.disableMeteors)
            {
                WorldGen.spawnMeteor = false;
            }
            if (Main.moonPhase == 0 && 事件设置.disableFullMoon)
            {
                Main.moonPhase = 1;
                flag = true;
            }
            if (Main.bloodMoon && 事件设置.disableBloodMoon)
            {
                TSPlayer.Server.SetBloodMoon(bloodMoon: false);
            }
            if (Main.snowMoon && 事件设置.disableFrostMoon)
            {
                TSPlayer.Server.SetFrostMoon(snowMoon: false);
            }
            if (Main.pumpkinMoon && 事件设置.disablePumpkinMoon)
            {
                TSPlayer.Server.SetPumpkinMoon(pumpkinMoon: false);
            }
            if (Main.eclipse && 事件设置.disableSolarEclipse)
            {
                TSPlayer.Server.SetEclipse(eclipse: false);
            }
            if (Main.raining && 事件设置.disableRain)
            {
                Main.StopRain();
            }
            if (Main.slimeRain && 事件设置.disableSlimeRain)
            {
                Main.StopSlimeRain(announce: false);
                flag = true;
            }
            if (事件设置.disableCultists)
            {
                WorldGen.GetRidOfCultists();
            }
            if (DD2Event.Ongoing && 事件设置.DD2Event)
            {
                DD2Event.Ongoing = false;
            }
            if (NPC.MoonLordCountdown > 0 && 事件设置.disableLunarInvasion)
            {
                NPC.MoonLordCountdown = 0;
                NPC.LunarApocalypseIsUp = false;
                NPC.TowerActiveNebula = false;
                NPC.TowerActiveSolar = false;
                NPC.TowerActiveStardust = false;
                NPC.TowerActiveVortex = false;
            }
            if (Main.invasionType > 0)
            {
                switch (Main.invasionType)
                {
                    case 1:
                        if (事件设置.disableGoblinInvasion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 2:
                        if (事件设置.disableFrostLegion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 3:
                        if (事件设置.disablePirateInvasion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 4:
                        if (事件设置.disablePumpkinMoon)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 5:
                        if (事件设置.disableFrostMoon)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 6:
                        if (事件设置.disableSolarEclipse)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                    case 7:
                        if (事件设置.disableMartianInvasion)
                        {
                            Main.invasionType = 0;
                            Main.invasionSize = 0;
                            flag = true;
                        }
                        break;
                }
            }
            if (flag)
            {
                TSPlayer.All.SendData(PacketTypes.WorldInfo);
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("[NPCEventBan] [Error] Exception occur in OnGameUpdate, Ex: " + ex);
        }
        this.TickCount++;
    }

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        try
        {
            if (args.Handled)
            {
                return;
            }

            var nPC = Main.npc[args.NpcId];
            if (nPC != null && nPC.active && 上锁NPC.Contains(nPC.type))
            {
                args.Handled = true;
                nPC.active = false;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("[Error] Exception occur in OnNpcSpawn, Ex: " + ex);
        }
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
            ServerApi.Hooks.GamePostInitialize.Deregister(this, new HookHandler<EventArgs>(this.PostInitialize));
            ServerApi.Hooks.NpcKilled.Deregister(this, NpcKill);
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnJoin);
            GeneralHooks.ReloadEvent -= new GeneralHooks.ReloadEventD(this.Reload);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnNpcSpawn);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);

        }
        base.Dispose(disposing);
    }

    private void PostInitialize(EventArgs args)
    {
        DB.ReadValue();
    }

    private void OnInitialize(EventArgs args)//游戏初始化的狗子
    {
        //第一个是权限，第二个是子程序，第三个是指令
        Commands.ChatCommands.Add(new Command("TaskSystem.user", this._TaskSystem, "任务", "task") //测试完成
        {
            HelpText = "查看任务系统所有指令"
        });
        Commands.ChatCommands.Add(new Command("TaskSystem.user", this._TaskSystemList, "tasklist", "任务列表") //测试完成
        {
            HelpText = "查看您接取的所有任务"
        });
        Commands.ChatCommands.Add(new Command("TaskSystem.user", this._TaskSystemQuery, "taskquery", "查询任务") //测试完成
        {
            HelpText = "通过id或任务名称 查询任务信息"
        });
        Commands.ChatCommands.Add(new Command("TaskSystem.user", this._TaskSystemOver, "taskover", "完成任务")
        {
            HelpText = "完成任务,并获取奖励"
        });
        Commands.ChatCommands.Add(new Command("TaskSystem.user", this._TaskSystemPick, "tasklist", "接受任务", "接取任务") //测试完成
        {
            HelpText = "接受指定ID的任务"
        });
        Commands.ChatCommands.Add(new Command("TaskSystem.user", this._放弃任务, "taskgiveup", "放弃任务") //测试完成
        {
            HelpText = "放弃指定ID的任务"
        });
    }


    private void _TaskSystem(CommandArgs args)
    {
        args.Player.SendInfoMessage("命令如下 /查询任务 /接受任务 /放弃任务 /任务列表 /完成任务");
    }
    /// <summary>
    /// 查看所有任务
    /// </summary>
    /// <param name="args"></param>

    private void _TaskSystemList(CommandArgs args)
    {
        var plr = args.Player;
        int s;
        int y;
        if (args.Parameters.Count == 0)
        {
            plr.SendInfoMessage("/任务列表 <主线/支线> <页码>");
            return;
        }
        if (args.Parameters.Count == 1)
        {
            y = 1;//页码
            s = 0;//索引
        }
        else
        {
            if (int.TryParse(args.Parameters[1], out y) && int.TryParse(args.Parameters[1], out s))
            {
                s--;
            }
            else
            {
                plr.SendErrorMessage("页码错误");
                return;
            }
        }
        if (y <= 0)
        {
            plr.SendErrorMessage("这页没有任务");
            return;
        }
        var i = 0;
        for (s = 10 * s; s < 配置.任务.Count; s++, i++)
        {
            switch (args.Parameters[0])
            {
                case "主线":
                    if (!配置.任务[s].是否主线)
                    {

                        i--;
                        continue;
                    }
                    if (i >= 10)
                    {
                        if (配置.任务.Count >= s + 1)
                        {
                            plr.SendInfoMessage($"输入\"/任务列表 主线 {y + 1}\"查看下一页");
                        }
                        return;
                    }
                    plr.SendMessage($"[c/FF6103:id:{配置.任务[s].任务ID}] {配置.任务[s].任务名称}", 255, 255, 255);
                    break;
                case "支线":
                    if (配置.任务[s].是否主线)
                    {

                        i--;
                        continue;
                    }
                    if (i >= 10)
                    {
                        if (配置.任务.Count >= s + 1)
                        {
                            plr.SendInfoMessage($"输入\"/任务列表 支线 {y + 1}\"查看下一页");
                        }
                        return;
                    }
                    plr.SendMessage($"[c/FF6103:id:{配置.任务[s].任务ID}] {配置.任务[s].任务名称}", 255, 255, 255);
                    break;
                default:
                    plr.SendInfoMessage("/任务列表 <主线/支线> <页码>");
                    return;
            }
        }
        plr.SendInfoMessage("已展示所有任务");
    }
    private void _TaskSystemQuery(CommandArgs args)
    {
        var plr = args.Player;
        if (args.Parameters.Count == 0)
        {
            plr.SendInfoMessage("/查询任务 <任务ID/主线/支线>");
            return;
        }
        if (args.Parameters[0] == "主线")
        {
            if (!plr.IsLoggedIn)
            {
                plr.SendInfoMessage("请先登录");
                return;
            }
            var ID = DB.QueryNowTask(plr.Name);
            if (!配置.任务.Exists(s => s.任务ID == ID))
            {
                plr.SendInfoMessage("没有任务");
                return;
            }
            plr.SendInfoMessage(this._TaskInfo(ID));
            return;
        }
        if (args.Parameters[0] == "支线")
        {
            if (!plr.IsLoggedIn)
            {
                plr.SendInfoMessage("请先登录");
                return;
            }
            var 支线ID = DB.QueryNowSideTask(plr.Name);
            var 是否有任务 = false;
            foreach (var ID in 支线ID)
            {
                if (配置.任务.Exists(s => s.任务ID == ID))
                {
                    plr.SendInfoMessage(this._TaskInfo(ID));
                    是否有任务 = true;
                }
            }
            if (!是否有任务)
            {
                plr.SendInfoMessage("没有任务");
                plr.SendInfoMessage("/接受任务 <任务ID>");
                var 主线任务 = "";
                var 可接受支线任务 = "";
                foreach (var z in DB.QueryGetTask(plr.Name))
                {
                    var r = GetTask(z);
                    可接受支线任务 += $"{r.任务ID}.{r.任务名称}\n";
                }
                if (可接受支线任务 == "")
                {
                    可接受支线任务 = "无";
                }
                foreach (var z in DB.QueryBranchTask(plr.Name))
                {
                    var r = GetTask(z);
                    主线任务 += $"{r.任务ID}.{r.任务名称}\n";
                }
                if (主线任务 != "")
                {
                    plr.SendMessage($"[c/FF6103:请选择下一个主线任务:]\n{主线任务}", 18, 252, 221);
                }
                _TaskInfoList(args);
            }
            return;
        }
        if (!int.TryParse(args.Parameters[0], out var id))
        {
            plr.SendInfoMessage("/查询任务 <任务ID>(数字)");
            return;
        }
        if (!配置.任务.Exists(s => s.任务ID == id))
        {
            plr.SendInfoMessage("没有任务");
            return;
        }
        plr.SendInfoMessage(this._TaskInfo(id));
    }
    private void _TaskSystemOver(CommandArgs args)
    {
        var plr = args.Player;
        if (!plr.IsLoggedIn)
        {
            plr.SendInfoMessage("请先登录");
            return;
        }
        if (args.Parameters.Count == 0)
        {
            var 是否显示失败原因 = true;
            var id = DB.QueryNowTask(plr.Name);
            if (Tasklogic(plr, id, 是否显示失败原因) && id != -1)
            {
                args.Player.SendMessage("[c/40E0D0:您已完成主线任务]" + GetTask(id).任务名称, 249, 136, 37);
                DB.CompleteTask(plr, id);

            }
            else
            {
                是否显示失败原因 = false;
            }

            foreach (var item in DB.QueryNowSideTask(plr.Name))
            {
                if (Tasklogic(plr, item, 是否显示失败原因))
                {
                    var Name = GetTask(item).任务名称;
                    args.Player.SendMessage("[c/F58C50:您已完成支线任务]" + Name, 37, 94, 249);
                    DB.CompleteSideTask(plr.Name, item);
                }
                else
                {
                    是否显示失败原因 = false;
                }
            }
        }
        else
        {
            if (!int.TryParse(args.Parameters[0], out var id))
            {
                args.Player.SendErrorMessage("/完成任务 <任务ID>(数字)");
                return;
            }
            if (Tasklogic(plr, id, true))
            {
                var 任务 = GetTask(id);
                if (任务.是否主线)
                {
                    args.Player.SendMessage("[c/40E0D0:您已完成主线任务]" + this.Name, 249, 136, 37);
                    DB.CompleteTask(plr, id);
                }
                else
                {
                    args.Player.SendMessage("[c/F58C50:您已完成支线任务]" + this.Name, 37, 94, 249);
                    DB.CompleteSideTask(plr.Name, id);
                }
            }
        }
    }
    private void _TaskSystemPick(CommandArgs args)
    {
        var plr = args.Player;
        {
            if (!plr.IsLoggedIn)
            {
                plr.SendInfoMessage("请先登录");
                return;
            }
            if (args.Parameters.Count == 0)
            {
                plr.SendInfoMessage("/接受任务 <任务ID>");
                var 主线任务 = "";
                foreach (var z in DB.QueryBranchTask(plr.Name))
                {
                    var r = GetTask(z);
                    主线任务 += $"{r.任务ID}.{r.任务名称}\n";
                }
                if (主线任务 != "")
                {
                    plr.SendMessage($"[c/FF6103:请选择下一个主线任务: ]\n{主线任务}", 18, 252, 221);
                }
                _TaskInfoList(args);
                return;
            }
        }
        {
            if (args.Parameters[0] == "列表")
            {
                _TaskInfoList(args);
                return;
            }
            if (!int.TryParse(args.Parameters[0], out var id))
            {
                plr.SendInfoMessage("/接受任务 <任务ID>(数字)");
                return;
            }
            if (!DB.QueryGetTask(plr.Name).Contains(id) && !DB.QueryBranchTask(plr.Name).Contains(id))
            {
                plr.SendErrorMessage("您不能接取该任务");
                return;
            }
            var r = GetTask(id);
            if (r.职业名限制.Count > 0)
            {
                if (!r.职业名限制.Contains(DB.QueryRPGGrade(plr.Name)))
                {
                    args.Player.SendInfoMessage($"选择失败。您当前职业不能接取该任务");
                    return;
                }
            }
            if (DB.查等级(plr.Name) < r.等级限制)
            {
                args.Player.SendInfoMessage($"选择失败。等级需要达到[c/FF3333:{r.等级限制}]");
                return;
            }
            var 进度 = GetTask(id).进度限制;
            if (进度.Count > 0)
            {
                var 当前进度 = Progress();
                foreach (var b in 进度)
                {
                    if (!当前进度.Contains(b))
                    {
                        args.Player.SendInfoMessage($"选择失败。需要打败[c/FF3333:{b}]");
                        return;
                    }
                }
            }
            if (DB.QueryGetTask(plr.Name).Contains(id))
            {
                DB.DelCanGetTask(plr.Name, id);
                DB.AddSideTask(plr.Name, id);
                plr.SendInfoMessage("[c/FA8723:接取支线任务成功]");
            }
            if (DB.QueryBranchTask(plr.Name).Contains(id))
            {
                DB.UpadatMainSide(plr.Name, id);
                DB.DelBranchTask(plr.Name);
                plr.SendInfoMessage("[c/7923FA:接取主线任务成功]");
            }

            plr.SendMessage("[c/FF8C00:======================]", 255, 255, 255);
            plr.SendMessage(this._TaskInfo(id), 255, 255, 255);
            plr.SendMessage("[c/FF8C00:======================]", 255, 255, 255);
        }
    }

    private void _放弃任务(CommandArgs args)
    {
        var plr = args.Player;
        if (!plr.IsLoggedIn)
        {
            plr.SendInfoMessage("请先登录");
            return;
        }
        if (args.Parameters.Count == 0)
        {
            plr.SendInfoMessage("/放弃任务 <任务ID>");
            return;
        }
        if (!int.TryParse(args.Parameters[0], out var id))
        {
            plr.SendInfoMessage("/放弃任务 <任务ID>(数字)");
            return;
        }
        if (!DB.QueryNowSideTask(plr.Name).Contains(id))
        {
            plr.SendErrorMessage("您没有接取该任务,无法放弃！");
            return;
        }
        DB.DelSideTask(plr.Name, id);
        plr.SendInfoMessage("[c/FA8723:放弃支线任务成功！]");
    }
    #region 核心逻辑
    //任务判定与任务类型 奖励给予等逻辑核心
    private static bool Tasklogic(TSPlayer plr, int 任务ID, bool 提示任务失败原因 = true)
    {
        if (DB.QueryNowTask(plr.Name) != 任务ID && !DB.QueryNowSideTask(plr.Name).Contains(任务ID))
        {
            if (提示任务失败原因)
            {
                plr.SendErrorMessage("没有接取该任务");
            }

            return false;
        }
        if (!配置.任务.Exists(s => s.任务ID == 任务ID))
        {
            return false;
        }
        var 任务 = GetTask(任务ID);
        foreach (var item in 任务.任务要求)
        {
            switch (item.要求ID)
            {
                //任务类型0 消耗物品完成任务
                case 0:
                {
                    var _item = item.目标参数.Split(',');
                    var item_bool = false;
                    for (var i = 10; i < 49; i++) //背包从第二排开始到最后一个背包位置
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
                                if (提示任务失败原因)
                                {
                                    plr.SendErrorMessage("没有足够的物品");
                                }

                                return false;
                            }
                        }
                    }
                    if (!item_bool)
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您的背包中没有任务物品:][i/s{Convert.ToInt32(_item[1])}:{Convert.ToInt32(_item[0])}]");
                            plr.SendInfoMessage("[c/A066D3:任务物品请不要放在背包最上排中]");
                        }

                        return false;
                    }
                }
                break;
                //任务类型1 背包中是否有指定物品
                case 1:
                {
                    var item_bool = false;
                    for (var i = 0; i < 49; i++) //背包从第二排开始到最后一个背包位置
                    {
                        if (plr.TPlayer.inventory[i].netID.ToString() == item.目标参数)
                        {
                            item_bool = true;
                            break;
                        }
                    }
                    if (!item_bool)
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您的背包中没有任务物品: ] [i:{item.目标参数}]");
                        }

                        return false;

                    }
                }
                break;
                //任务类型2 是否击杀指定NPC
                case 2:
                {
                    var _item = item.目标参数.Split(',');
                    var KNpc = DB.QueryTaskTarget(plr.Name);
                    var npcID = int.Parse(_item[0]);
                    if (!KNpc.ContainsKey(npcID))
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage("您没有击杀指定NPC或击杀数量不足(" + new NPC() { netID = int.Parse(_item[0]) }.FullName + ") ：任务数量" + int.Parse(_item[1]) + " 实际数量" + 0);
                        }

                        return false;
                    }
                    if (KNpc[int.Parse(_item[0])] >= int.Parse(_item[1]))
                    {
                        continue;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage("您没有击杀指定NPC或击杀数量不足(" + new NPC() { netID = int.Parse(_item[0]) }.FullName + ") ：任务数量" + int.Parse(_item[1]) + " 实际数量" + KNpc[int.Parse(_item[0])]);
                        }

                        return false;
                    }
                }
                //任务类型3 到达指定地图区域
                case 3:
                {
                    var Coordinate = item.目标参数.Split(',');
                    var x = int.Parse(Coordinate[0]);
                    var y = int.Parse(Coordinate[1]);
                    var deviation = int.Parse(Coordinate[2]);
                    if (plr.X / 16 >= (x - deviation) && plr.X / 16 <= (x + deviation) && plr.Y / 16 >= (y - deviation) && plr.Y / 16 <= (y + deviation))
                    {
                        continue;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage("[c/FF6103:您没有到达指定地图区域: ]您的坐标" + (plr.X / 16) + "," + (plr.Y / 16) + " 指定区域" + x + "," + y + " 允许偏差" + deviation);
                        }

                        return false;
                    }
                }
                //穿戴或拿起指定装备
                case 4:
                {
                    var item_bool = false;
                    for (var i = 50; i < 99; i++)
                    {
                        if (plr.ItemInHand.netID.ToString() == item.目标参数)
                        {
                            item_bool = true;
                            break;
                        }
                        for (var j = 0; j < NetItem.ArmorSlots; j++)
                        {
                            var tritem2 = plr.TPlayer.armor[j];
                            if (tritem2.netID.ToString() == item.目标参数)
                            {
                                item_bool = true;
                                break;
                            }
                        }
                        for (var k = 0; k < NetItem.DyeSlots; k++)
                        {
                            var tritem3 = plr.TPlayer.dye[k];
                            if (tritem3.netID.ToString() == item.目标参数)
                            {
                                item_bool = true;
                                break;
                            }
                        }
                        for (var l = 0; l < NetItem.MiscEquipSlots; l++)
                        {
                            var tritem4 = plr.TPlayer.miscEquips[l];
                            if (tritem4.netID.ToString() == item.目标参数)
                            {
                                item_bool = true;
                                break;
                            }
                        }
                        for (var m = 0; m < NetItem.MiscDyeSlots; m++)
                        {
                            var tritem5 = plr.TPlayer.miscDyes[m];
                            if (tritem5.netID.ToString() == item.目标参数)
                            {
                                item_bool = true;
                                break;
                            }
                        }
                    }
                    if (!item_bool)
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您未穿戴或拿起指定装备: ][i:{item.目标参数}]");
                        }

                        return false;

                    }
                }
                break;
                //拥有指定buff
                case 5:
                {
                    if (plr.TPlayer.buffType.Contains(int.Parse(item.目标参数)))
                    {
                        continue;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您没有拥有指定buff: ] BUFF ID:" + item.目标参数);
                        }

                        return false;
                    }
                }
                //对话指定NPC
                case 6:
                    if (DB.QueryTaskTarget2(plr.Name) == int.Parse(item.目标参数))
                    {
                        continue;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您没有与指定NPC对话: ] NPC ID:" + item.目标参数);
                        }

                        return false;
                    }
                //击杀指定NPC总数
                case 7:
                {
                    var _item = item.目标参数.Split(',');
                    var KNpc = DB.QueryTaskTarget(plr.Name);
                    var Ks = int.Parse(_item[^1]);
                    var s = 0;
                    for (var i = 0; i < _item.Length - 1; i++)
                    {
                        var npcID = int.Parse(_item[i]);
                        if (KNpc.ContainsKey(npcID))
                        {
                            s += KNpc[npcID];
                        }
                    }
                    if (Ks <= s)
                    {
                        continue;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            var NpcName = "";
                            for (var i = 0; i < _item.Length - 1; i++)
                            {
                                NpcName += new NPC() { netID = int.Parse(_item[i]) }.FullName + ",";
                            }
                            plr.SendErrorMessage("您没有击杀指定NPC或击杀数量不足(" + NpcName + ") ：任务数量" + int.Parse(_item[^1]) + " 实际数量" + s);

                        }
                        return false;
                    }
                }
                //达到指定等级
                case 8:
                {
                    if (DB.查等级(plr.Name) >= int.Parse(item.目标参数))
                    {
                        continue;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您没有达到: ] " + item.目标参数 + "级");
                        }

                        return false;
                    }
                }
                //成为指定职业
                case 9:
                {
                    if (DB.QueryRPGGrade(plr.Name) == item.目标参数)
                    {
                        continue;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage($"[c/FF6103:您的职业不是: ] " + item.目标参数);
                        }

                        return false;
                    }
                }
            }
        }
        //消耗任务物品
        var _itemTask = 任务.任务要求.Where(x => x.要求ID.ToString() == "0");
        foreach (var item in _itemTask)
        {
            var _item = item.目标参数.Split(',');
            for (var i = 10; i < 49; i++) //背包从第二排开始到最后一个背包位置
            {
                if (plr.TPlayer.inventory[i].netID == Convert.ToInt32(_item[0]))
                {
                    if (plr.TPlayer.inventory[i].stack >= Convert.ToInt32(_item[1]))
                    {
                        var stack = plr.TPlayer.inventory[i].stack -= Convert.ToInt32(_item[1]);
                        PlayItemSet(plr.Index, i, Convert.ToInt32(_item[0]), stack);
                        break;
                    }
                    else
                    {
                        if (提示任务失败原因)
                        {
                            plr.SendErrorMessage("没有足够的物品");
                        }

                        return false;
                    }
                }
            }
        }
        //给与奖励

        //给与奖励物品
        foreach (var item in 任务.完成指令)
        {
            Commands.HandleCommand(TSPlayer.Server, string.Format(item, plr.Name));
        }

        return true;

    }
    // 给与玩家物品
    public static void PlayItemSet(int ID, int slot, int Item, int stack)//ID 玩家ID，slot 格子ID，Item 物品ID，stack 物品堆叠
    {
        var player = new TSPlayer(ID);
        int index;
        var item = TShock.Utils.GetItemById(Item);
        item.stack = stack;
        //Inventory slots
        if (slot < NetItem.InventorySlots)
        {
            index = slot;
            player.TPlayer.inventory[slot] = item;

            NetMessage.SendData((int) PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.inventory[index].Name), player.Index, slot, player.TPlayer.inventory[index].prefix);
            NetMessage.SendData((int) PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.inventory[index].Name), player.Index, slot, player.TPlayer.inventory[index].prefix);
        }

        //Armor & Accessory slots
        else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots)
        {
            index = slot - NetItem.InventorySlots;
            player.TPlayer.armor[index] = item;

            NetMessage.SendData((int) PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.armor[index].Name), player.Index, slot, player.TPlayer.armor[index].prefix);
            NetMessage.SendData((int) PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.armor[index].Name), player.Index, slot, player.TPlayer.armor[index].prefix);
        }

        //Dye slots
        else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots)
        {
            index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots);
            player.TPlayer.dye[index] = item;

            NetMessage.SendData((int) PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.dye[index].Name), player.Index, slot, player.TPlayer.dye[index].prefix);
            NetMessage.SendData((int) PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.dye[index].Name), player.Index, slot, player.TPlayer.dye[index].prefix);
        }

        //Misc Equipment slots
        else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots)
        {
            index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots);
            player.TPlayer.miscEquips[index] = item;

            NetMessage.SendData((int) PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.miscEquips[index].Name), player.Index, slot, player.TPlayer.miscEquips[index].prefix);
            NetMessage.SendData((int) PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.miscEquips[index].Name), player.Index, slot, player.TPlayer.miscEquips[index].prefix);
        }

        //Misc Dyes slots
        else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots + NetItem.MiscDyeSlots)
        {
            index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots);
            player.TPlayer.miscDyes[index] = item;

            NetMessage.SendData((int) PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.miscDyes[index].Name), player.Index, slot, player.TPlayer.miscDyes[index].prefix);
            NetMessage.SendData((int) PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.miscDyes[index].Name), player.Index, slot, player.TPlayer.miscDyes[index].prefix);
        }
    }
    #endregion

    private static void _TaskInfoList(CommandArgs args)
    {
        var plr = args.Player;
        int s;
        int y;
        if (args.Parameters.Count <= 1)
        {
            y = 1;//页码
            s = 0;//索引
        }
        else
        {
            if (int.TryParse(args.Parameters[1], out y) && int.TryParse(args.Parameters[1], out s))
            {
                s--;
            }
            else
            {
                plr.SendErrorMessage("页码错误");
                return;
            }
        }
        if (y <= 0)
        {
            plr.SendErrorMessage("这页没有任务");
            return;
        }
        var i = 0;
        var 支线任务 = DB.QueryGetTask(plr.Name);
        for (s = 10 * s; s < 配置.任务.Count; s++, i++)
        {
            if (配置.任务[s].是否主线 || !支线任务.Contains(配置.任务[s].任务ID))
            {
                i--;
                continue;
            }
            if (i >= 10)
            {
                if (配置.任务.Count >= s + 1)
                {
                    plr.SendInfoMessage($"输入\"/接受任务 列表 <页码>\"查看下一页");
                }
                return;
            }
            plr.SendMessage($"[c/FF6103:id:{配置.任务[s].任务ID}] {配置.任务[s].任务名称}", 255, 255, 255);
        }
        plr.SendInfoMessage("已展示所有可接受支线任务");
    }
    #region 重载
    private void Reload(ReloadEventArgs args)
    {
        try
        {
            Reload();
            args.Player.SendSuccessMessage($"[Chrome.Task]重载成功！");
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.Task]配置文件读取错误");
        }
    }
    public static void Reload()
    {
        try
        {
            配置表.GetConfig();
            配置 = JsonConvert.DeserializeObject<配置表>(File.ReadAllText(Path.Combine(path)))!;
            File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
            上锁NPC = new() { };
            foreach (var z in 配置.任务)
            {
                foreach (var n in z.解锁NPC)
                {
                    上锁NPC.Add(n);
                }
                foreach (var n in z.解锁事件)
                {
                    解锁事件(n, true);
                }
            }
            DB.ReadValue();
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[Chrome.Task]配置文件读取错误");
        }
    }
    public static void 解锁事件(string n, bool b)
    {
        if (n == "满月")
        {
            事件设置.disableFullMoon = b;
        }
        if (n == "霜月")
        {
            事件设置.disableFrostMoon = b;
        }
        if (n == "血月")
        {
            事件设置.disableBloodMoon = b;
        }
        if (n == "南瓜月")
        {
            事件设置.disablePumpkinMoon = b;
        }
        if (n == "日食")
        {
            事件设置.disableSolarEclipse = b;
        }
        if (n == "下雨")
        {
            事件设置.disableRain = b;
        }
        if (n == "史莱姆雨")
        {
            事件设置.disableSlimeRain = b;
        }
        if (n == "哥布林入侵")
        {
            事件设置.disableGoblinInvasion = b;
        }
        if (n == "海盗入侵")
        {
            事件设置.disablePirateInvasion = b;
        }
        if (n == "雪人军团")
        {
            事件设置.disableFrostLegion = b;
        }
        if (n == "下落陨铁")
        {
            事件设置.disableMeteors = b;
        }
        if (n == "火星人入侵")
        {
            事件设置.disableMartianInvasion = b;
        }
        if (n == "月球入侵")
        {
            事件设置.disableLunarInvasion = b;
        }
        if (n == "拜月教邪教徒")
        {
            事件设置.disableCultists = b;
        }
        if (n == "撒旦军团入侵")
        {
            事件设置.DD2Event = b;
        }
    }
    public static void Write()
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
    }
    #endregion
    #region 基本不用改
    public static 配置表.任务配置 GetTask(int id)
    {
        return 配置.任务.Find(s => s.任务ID == id);
    }
    private string _TaskInfo(int id)
    {
        var 任务 = GetTask(id);
        var 线 = 任务.是否主线 ? "主线" : "支线";
        return $"任务ID:{id}\n任务名:{任务.任务名称}\n任务类型:{线}\n任务简述:{任务.任务简述}\n任务详细要求:{任务.任务详细描述}";
    }
    private void OnJoin(GreetPlayerEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        if (plr != null)
        {
            if (!DB.IsExist(plr.Name))
            {
                TShock.DB.Query("INSERT INTO " + DB.tableName + " (`玩家名`, `主线任务`,`分支任务`, `支线任务`,`可接取任务`,`已完成任务`,`任务目标`,`任务目标2`) VALUES " + "(@0, @1, @2, @3, @4, @5, @6, @7);", plr.Name, 1, "", "", "", "", "", 0);
            }
        }
    }
    private static void NpcKill(NpcKilledEventArgs args)
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
        DB.AddTaskAim(plr.Name, args.npc.netID);
    }

    private void OnGetData(GetDataEventArgs args)
    {
        if (args.Handled || args.MsgID != PacketTypes.NpcTalk)
        {
            return;
        }

        var index = args.Msg.readBuffer[args.Index];
        var plr = TShock.Players[index];
        if (index != args.Msg.whoAmI || plr.TPlayer.talkNPC == -1)
        {
            return;
        }

        var npcID = Main.npc[plr.TPlayer.talkNPC].type;
        DB.UpdataTaskTarget2(plr.Name, npcID);
    }
    private static List<string> Progress()//获取进度详情
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
    #endregion
}