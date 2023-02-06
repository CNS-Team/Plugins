using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Chrome;

namespace Chrome.EntryCondition
{
    [ApiVersion(2, 1)]//api版本
    public class EntryCondition : TerrariaPlugin
    {
        /// 插件作者
        public override string Author => "奇威复反";
        /// 插件说明
        public override string Description => "Chrome.EntryCondition";
        /// 插件名字
        public override string Name => "Chrome.EntryCondition";
        /// 插件版本
        public override Version Version => new(1, 0, 0, 0);
        /// 插件处理
        public EntryCondition(Main game) : base(game)
        {
        }
        public static string path = "tshock/Chrome.EntryCondition.json";
        //插件启动时，用于初始化各种狗子
        public static Config 配置 = new();
        public override void Initialize()
        {
            ServerApi.Hooks.ServerJoin.Register(this, new HookHandler<JoinEventArgs>(this.OnJoin));
            //ServerApi.Hooks.NetGreetPlayer.Register(this, new HookHandler<GreetPlayerEventArgs>(this.GreetPlayer));
            //ServerApi.Hooks.NetGetData.Register(this, new HookHandler<GetDataEventArgs>(this.OnNetGetdata));
            //ServerApi.Hooks.NpcStrike.Register(this, new HookHandler<NpcStrikeEventArgs>(this.OnNpcStrike));
            GeneralHooks.ReloadEvent += new GeneralHooks.ReloadEventD(this.Reload);
            Config.GetConfig();
            Reload();
        }
        /// 插件关闭时
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerJoin.Deregister(this, new HookHandler<JoinEventArgs>(this.OnJoin));
                //ServerApi.Hooks.NetGreetPlayer.Deregister(this, new HookHandler<GreetPlayerEventArgs>(this.GreetPlayer));
                //ServerApi.Hooks.NetGetData.Deregister(this, new HookHandler<GetDataEventArgs>(this.OnNetGetdata));
                //ServerApi.Hooks.NpcStrike.Deregister(this, new HookHandler<NpcStrikeEventArgs>(this.OnNpcStrike));
                GeneralHooks.ReloadEvent -= new GeneralHooks.ReloadEventD(this.Reload);

            }
            base.Dispose(disposing);
        }
        /*
        private void OnNpcStrike(NpcStrikeEventArgs args)
        {
            bool flag = args.Npc.netID == 661;
            if (flag)
            {
                TSPlayer tsplayer = TShock.Players[args.Player.whoAmI];
                Config config = Config.GetConfig();
                bool flag2 = tsplayer.TileX < config.Xmax && tsplayer.TileX > config.Xmin && tsplayer.TileY > config.Ymin && tsplayer.TileY < config.Ymax;
                if (flag2)
                {
                    TShock.Utils.Broadcast(tsplayer.Name + "试图在禁用地区召唤", Color.Red);
                    args.Damage = 0;
                    args.Handled = true;
                    args.Npc.active = false;
                    this.AddCount(tsplayer.Index);
                }
            }
        }
        
        // Token: 0x06000021 RID: 33 RVA: 0x00002660 File Offset: 0x00000860
        private void OnNetGetdata(GetDataEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.Msg.whoAmI];
            if (plr == null) return;
            if ((int)args.MsgID == 61)
            {
                
            }
        }
        */
        private void OnJoin(JoinEventArgs args)
        {

            var plr = TShock.Players[args.Who];
            if (plr == null) return;
            if (!配置.启用插件) return;
            if (plr.HasPermission("Chrome.EntryCondition") && 配置.允许管理员忽略进入判断) return;
            string 阻止提示 = "";
            if (配置.允许进入的职业.Count > 0)
            {
                string 职业 = Chrome.RPG.Config.查职业(plr.Name);
                if (!配置.允许进入的职业.ContainsKey(职业))
                {
                    阻止提示 = 配置.职业阻止进入提示 + "\n";
                }
                else if (RPG.Config.查等级(plr.Name) < 配置.允许进入的职业[职业])
                {
                    阻止提示 = 配置.等级不够阻止进入提示.Replace("{0}", 配置.允许进入的职业[职业].ToString()) + "\n";
                }
            }
            List<int> 任务 = 任务系统.DB.QueryFinishTask(plr.Name);
            foreach (var z in 配置.进入需要完成的任务)
            {
                if (!任务.Contains(z))
                {
                    var r = 任务系统.任务系统.GetTask(z);
                    if (r.是否主线)
                    {
                        阻止提示 += 配置.任务阻止进入提示.Replace("{0}", "主线").Replace("{1}", r.任务ID.ToString()).Replace("{2}", r.任务名称) + "\n";
                    }
                    else
                    {
                        阻止提示 += 配置.任务阻止进入提示.Replace("{0}", "支线").Replace("{1}", r.任务ID.ToString()).Replace("{2}", r.任务名称) + "\n";
                    }
                }
            }
            if (阻止提示 != "")
            {
                plr.Disconnect(阻止提示);
            }
        }

        private void Reload(ReloadEventArgs args)
        {
            try
            {
                Reload();
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[Chrome.EntryCondition]配置文件读取错误");
            }
        }
        public static void Reload()
        {
            try
            {
                配置 = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(path)));
                File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[Chrome.EntryCondition]配置文件读取错误");
            }
        }
    }
}