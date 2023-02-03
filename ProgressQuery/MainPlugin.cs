using Microsoft.Xna.Framework;
using Rests;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ProgressQuery
{
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
            TShock.RestApi.Register(new SecureRestCommand("/Progress", ProgressAPI, "ProgressQuery.use"));
            Commands.ChatCommands.Add(new Command("ProgressQuery.use", Query, cmd));
            Commands.ChatCommands.Add(new Command("Progress.admin", Set, "进度设置"));
        }

        private void Set(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage("输入/进度设置 <名称>");
                return;
            }
            if (Utils.GetNPCFilelds().TryGetValue(args.Parameters[0], out FieldInfo? field) && field != null)
            {
                bool code;
                object type;
                if (args.Parameters[0] == "血肉墙")
                {
                    type = new Main();
                    code = !Convert.ToBoolean(field.GetValue(type));
                }
                else
                {
                    type = new NPC();
                    code = !Convert.ToBoolean(field.GetValue(new NPC()));
                }

                field.SetValue(type, code);
                args.Player.SendSuccessMessage("设置进度{0}为{1}", args.Parameters[0], code);
            }
            else
            {
                args.Player.SendErrorMessage("不包含此进度!");
            }
            
        }

        private object ProgressAPI(RestRequestArgs args)
        {
            RestObject obj = new()
            {
                Status = "200",
                Response = "查询成功"
            };
            obj["data"] = Utils.GetGameProgress();
            return obj;
        }

        private void Query(CommandArgs args)
        {
            Dictionary<string, bool> Progress = Utils.GetGameProgress();
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
}