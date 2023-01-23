using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Chrome.EntryCondition
{
    [ApiVersion(2, 1)]//api版本
    public class 快速配置rest : TerrariaPlugin
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
        public 快速配置rest(Main game) : base(game)
        {
        }
        public static string path = "tshock/Chrome.EntryCondition.json";
        //插件启动时，用于初始化各种狗子
        public static Config 配置 = new();
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);//钩住游戏初始化时
            ServerApi.Hooks.NetGreetPlayer.Register(this, new HookHandler<GreetPlayerEventArgs>(this.GreetPlayer));
            Config.GetConfig();
            Reload();
        }
        /// 插件关闭时
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Deregister hooks here
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);//销毁游戏初始化狗子
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, new HookHandler<GreetPlayerEventArgs>(this.GreetPlayer));

            }
            base.Dispose(disposing);
        }
        public void GreetPlayer(GreetPlayerEventArgs args)
        {
            var plr = TShock.Players[args.Who];
            if (plr.HasPermission("Chrome.EntryCondition"))
            {
                return;
            }
            if(配置.启用插件)
            {
                string 职业 = DB.QueryGrade(plr.Name);
                if (配置.允许进入的职业.Contains(职业))
                {
                    return;
                }
                else
                {
                    plr.Disconnect(配置.阻止进入提示);
                }
            }
        }
        private void OnInitialize(EventArgs args)//游戏初始化的狗子
        {
            //第一个是权限，第二个是子程序，第三个是指令
            Commands.ChatCommands.Add(new Command("Chrome.EntryCondition", 重载, "reload") { });
        }

        private void 重载(CommandArgs args)
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