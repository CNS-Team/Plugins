using Newtonsoft.Json;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace 任务系统
{
    [ApiVersion(2, 1)]//api版本
    public class 任务系统 : TerrariaPlugin
    {
        /// 插件作者
        public override string Author => "奇威复反";
        /// 插件说明
        public override string Description => "任务系统";
        /// 插件名字
        public override string Name => "Chrome.Task";
        /// 插件版本
        public override Version Version => new(1, 0, 0, 0);
        /// 插件处理
        public 任务系统(Main game) : base(game)
        {
            Order = 2;
        }
        public static string path = "tshock/Chrome.Task.json";
        //插件启动时，用于初始化各种狗子 
        public static 配置表 配置 = new();
        public static 配置表.解锁数据库 解锁 = new();
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);//钩住游戏初始化时
            ServerApi.Hooks.NpcKilled.Register(this, NpcKill);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
            GeneralHooks.ReloadEvent += new GeneralHooks.ReloadEventD(this.Reload);
            配置表.GetConfig();
            Reload();
            DB.Connect();
        }

        /// 插件关闭时
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Deregister hooks here
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);//销毁游戏初始化狗子
                ServerApi.Hooks.NpcKilled.Deregister(this, NpcKill);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
                GeneralHooks.ReloadEvent -= new GeneralHooks.ReloadEventD(this.Reload);

            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)//游戏初始化的狗子
        {
            //第一个是权限，第二个是子程序，第三个是指令
            Commands.ChatCommands.Add(new Command("TaskSystem.user", _TaskSystem, "任务", "task") //测试完成
            {
                HelpText = "查看任务系统所有指令"
            });
            Commands.ChatCommands.Add(new Command("TaskSystem.user", _TaskSystemList, "tasklist", "任务列表") //测试完成
            {
                HelpText = "查看您接取的所有任务"
            });
            Commands.ChatCommands.Add(new Command("TaskSystem.user", _TaskSystemQuery, "taskquery", "查询任务") //测试完成
            {
                HelpText = "通过id或任务名称 查询任务信息"
            });
            Commands.ChatCommands.Add(new Command("TaskSystem.user", _TaskSystemOver, "taskover", "完成任务")
            {
                HelpText = "完成任务,并获取奖励"
            }); Commands.ChatCommands.Add(new Command("TaskSystem.user", _TaskSystemPick, "tasklist", "接受任务", "接取任务") //测试完成
            {
                HelpText = "接受任务指定ID的任务"
            });
        }



        private void _TaskSystem(CommandArgs args)
        {
            args.Player.SendInfoMessage("命令如下 /查询任务 /接受任务 /任务列表 /完成任务");
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
            int i = 0;
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
                int ID = DB.QueryNowTask(plr.Name);
                if (!配置.任务.Exists(s => s.任务ID == ID))
                {
                    plr.SendInfoMessage("没有任务");
                    return;
                }
                plr.SendInfoMessage(_TaskInfo(ID));
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
                bool 是否有任务 = false;
                foreach (var ID in 支线ID)
                {
                    if (配置.任务.Exists(s => s.任务ID == ID))
                    {
                        plr.SendInfoMessage(_TaskInfo(ID));
                        是否有任务 = true;
                    }
                }
                if (!是否有任务)
                {
                    plr.SendInfoMessage("没有任务");
                    plr.SendInfoMessage("/接受任务 <任务ID>");
                    string 主线任务 = "";
                    string 可接受支线任务 = "";
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
            if (!int.TryParse(args.Parameters[0], out int id))
            {
                plr.SendInfoMessage("/查询任务 <任务ID>(数字)");
                return;
            }
            if (!配置.任务.Exists(s => s.任务ID == id))
            {
                plr.SendInfoMessage("没有任务");
                return;
            }
            plr.SendInfoMessage(_TaskInfo(id));
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
                bool 是否显示失败原因 = true;
                int id = DB.QueryNowTask(plr.Name);
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
                        string Name = GetTask(item).任务名称;
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
                if (!int.TryParse(args.Parameters[0], out int id))
                {
                    args.Player.SendErrorMessage("/完成任务 <任务ID>(数字)");
                    return;
                }
                if (Tasklogic(plr, id, true))
                {
                    var 任务 = GetTask(id);
                    if (任务.是否主线)
                    {
                        args.Player.SendMessage("[c/40E0D0:您已完成主线任务]" + Name, 249, 136, 37);
                        DB.CompleteTask(plr, id);
                    }
                    else
                    {
                        args.Player.SendMessage("[c/F58C50:您已完成支线任务]" + Name, 37, 94, 249);
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
                    string 主线任务 = "";
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
                if (!int.TryParse(args.Parameters[0], out int id))
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
                    List<string> 当前进度 = Progress();
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
                plr.SendMessage(_TaskInfo(id), 255, 255, 255);
                plr.SendMessage("[c/FF8C00:======================]", 255, 255, 255);
            }
        }

        //任务判定与任务类型 奖励给予等逻辑核心
        private static bool Tasklogic(TSPlayer plr, int 任务ID, bool 提示任务失败原因 = true)
        {
            if (DB.QueryNowTask(plr.Name) != 任务ID && !DB.QueryNowSideTask(plr.Name).Contains(任务ID))
            {
                if (提示任务失败原因)
                    plr.SendErrorMessage("没有接取该任务");
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
                                        if (提示任务失败原因)
                                            plr.SendErrorMessage("没有足够的物品");
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
                            bool item_bool = false;
                            for (int i = 0; i < 49; i++) //背包从第二排开始到最后一个背包位置
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
                                    plr.SendErrorMessage($"[c/FF6103:您的背包中没有任务物品: ] [i:{item.目标参数}]");
                                return false;

                            }
                        }
                        break;
                    //任务类型2 是否击杀指定NPC
                    case 2:
                        {
                            var _item = item.目标参数.Split(',');
                            var KNpc = DB.QueryTaskTarget(plr.Name);
                            int npcID = int.Parse(_item[0]);
                            if (!KNpc.ContainsKey(npcID))
                            {
                                if (提示任务失败原因)
                                    plr.SendErrorMessage("您没有击杀指定NPC或击杀数量不足(" + new NPC() { netID = int.Parse(_item[0]) }.FullName + ") ：任务数量" + int.Parse(_item[1]) + " 实际数量" + 0);
                                return false;
                            }
                            if (KNpc[int.Parse(_item[0])] >= int.Parse(_item[1]))
                            {
                                continue;
                            }
                            else
                            {
                                if (提示任务失败原因)
                                    plr.SendErrorMessage("您没有击杀指定NPC或击杀数量不足(" + new NPC() { netID = int.Parse(_item[0]) }.FullName + ") ：任务数量" + int.Parse(_item[1]) + " 实际数量" + KNpc[int.Parse(_item[0])]);
                                return false;
                            }
                        }
                    //任务类型3 到达指定地图区域
                    case 3:
                        {
                            var Coordinate = item.目标参数.Split(',');
                            int x = int.Parse(Coordinate[0]);
                            int y = int.Parse(Coordinate[1]);
                            int deviation = int.Parse(Coordinate[2]);
                            if (plr.X / 16 >= (x - deviation) && plr.X / 16 <= (x + deviation) && plr.Y / 16 >= (y - deviation) && plr.Y / 16 <= (y + deviation))
                            {
                                continue;
                            }
                            else
                            {
                                if (提示任务失败原因)
                                    plr.SendErrorMessage("[c/FF6103:您没有到达指定地图区域: ]您的坐标" + plr.X / 16 + "," + plr.Y / 16 + " 指定区域" + x + "," + y + " 允许偏差" + deviation);
                                return false;
                            }
                        }
                    //穿戴或拿起指定装备
                    case 4:
                        {
                            bool item_bool = false;
                            for (int i = 50; i < 99; i++)
                            {
                                if (plr.ItemInHand.netID.ToString() == item.目标参数)
                                {
                                    item_bool = true;
                                    break;
                                }
                                for (int j = 0; j < NetItem.ArmorSlots; j++)
                                {
                                    Item tritem2 = plr.TPlayer.armor[j];
                                    if (tritem2.netID.ToString() == item.目标参数)
                                    {
                                        item_bool = true;
                                        break;
                                    }

                                }
                                for (int k = 0; k < NetItem.DyeSlots; k++)
                                {
                                    Item tritem3 = plr.TPlayer.dye[k];
                                    if (tritem3.netID.ToString() == item.目标参数)
                                    {
                                        item_bool = true;
                                        break;
                                    }
                                }
                                for (int l = 0; l < NetItem.MiscEquipSlots; l++)
                                {
                                    Item tritem4 = plr.TPlayer.miscEquips[l];
                                    if (tritem4.netID.ToString() == item.目标参数)
                                    {
                                        item_bool = true;
                                        break;
                                    }
                                }
                                for (int m = 0; m < NetItem.MiscDyeSlots; m++)
                                {
                                    Item tritem5 = plr.TPlayer.miscDyes[m];
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
                                    plr.SendErrorMessage($"[c/FF6103:您未穿戴或拿起指定装备: ][i:{item.目标参数}]");
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
                                    plr.SendErrorMessage($"[c/FF6103:您没有拥有指定buff: ] BUFF ID:" + item.目标参数);
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
                                plr.SendErrorMessage($"[c/FF6103:您没有与指定NPC对话: ] NPC ID:" + item.目标参数);

                            return false;
                        }
                    //击杀指定NPC总数
                    case 7:
                        {
                            var _item = item.目标参数.Split(',');
                            var KNpc = DB.QueryTaskTarget(plr.Name);
                            int Ks = int.Parse(_item[_item.Length - 1]);
                            int s = 0;
                            for (int i = 0; i < _item.Length - 1; i++)
                            {
                                int npcID = int.Parse(_item[i]);
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
                                    string NpcName = "";
                                    for (int i = 0; i < _item.Length - 1; i++)
                                    {
                                        NpcName += new NPC() { netID = int.Parse(_item[i]) }.FullName + ",";
                                    }
                                    plr.SendErrorMessage("您没有击杀指定NPC或击杀数量不足(" + NpcName + ") ：任务数量" + int.Parse(_item[_item.Length - 1]) + " 实际数量" + s);

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
                for (int i = 10; i < 49; i++) //背包从第二排开始到最后一个背包位置
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
                                plr.SendErrorMessage("没有足够的物品");
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
            TSPlayer player = new TSPlayer(ID);
            int index;
            Item item = TShock.Utils.GetItemById(Item);
            item.stack = stack;
            //Inventory slots
            if (slot < NetItem.InventorySlots)
            {
                index = slot;
                player.TPlayer.inventory[slot] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.inventory[index].Name), player.Index, slot, player.TPlayer.inventory[index].prefix);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.inventory[index].Name), player.Index, slot, player.TPlayer.inventory[index].prefix);
            }

            //Armor & Accessory slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots)
            {
                index = slot - NetItem.InventorySlots;
                player.TPlayer.armor[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.armor[index].Name), player.Index, slot, player.TPlayer.armor[index].prefix);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.armor[index].Name), player.Index, slot, player.TPlayer.armor[index].prefix);
            }

            //Dye slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots)
            {
                index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots);
                player.TPlayer.dye[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.dye[index].Name), player.Index, slot, player.TPlayer.dye[index].prefix);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.dye[index].Name), player.Index, slot, player.TPlayer.dye[index].prefix);
            }

            //Misc Equipment slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots)
            {
                index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots);
                player.TPlayer.miscEquips[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.miscEquips[index].Name), player.Index, slot, player.TPlayer.miscEquips[index].prefix);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.miscEquips[index].Name), player.Index, slot, player.TPlayer.miscEquips[index].prefix);
            }

            //Misc Dyes slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots + NetItem.MiscDyeSlots)
            {
                index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots);
                player.TPlayer.miscDyes[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.miscDyes[index].Name), player.Index, slot, player.TPlayer.miscDyes[index].prefix);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, NetworkText.FromLiteral(player.TPlayer.miscDyes[index].Name), player.Index, slot, player.TPlayer.miscDyes[index].prefix);
            }
        }
        public static 配置表.任务配置 GetTask(int id)
        {
            return 配置.任务.Find(s => s.任务ID == id);
        }
        private string _TaskInfo(int id)
        {
            var 任务 = GetTask(id);
            string 线 = 任务.是否主线 ? "主线" : "支线";
            return $"任务ID:{id}\n任务名:{任务.任务名称}\n任务类型:{线}\n任务简述:{任务.任务简述}\n任务详细要求:{任务.任务详细描述}";
        }
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
            int i = 0;
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
        private void OnJoin(GreetPlayerEventArgs args)
        {
            var plr = TShock.Players[args.Who];
            if (plr != null)
                if (!DB.IsExist(plr.Name))
                {
                    TShock.DB.Query("INSERT INTO " + DB.tableName + " (`玩家名`, `主线任务`,`分支任务`, `支线任务`,`可接取任务`,`已完成任务`,`任务目标`,`任务目标2`) VALUES " + "(@0, @1, @2, @3, @4, @5, @6, @7);", plr.Name, 1, "", "", "", "", "", 0);
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
                return;
            var index = args.Msg.readBuffer[args.Index];
            var plr = TShock.Players[index];
            if (index != args.Msg.whoAmI || plr.TPlayer.talkNPC == -1)
                return;
            var npcID = Main.npc[plr.TPlayer.talkNPC].type;
            DB.UpdataTaskTarget2(plr.Name, npcID);
        }
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
                配置 = JsonConvert.DeserializeObject<配置表>(File.ReadAllText(Path.Combine(path)));
                File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
             
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[Chrome.Task]配置文件读取错误");
            }
        }
        public static void Write()
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
        }
        private List<string> Progress()//获取进度详情
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
    }
}