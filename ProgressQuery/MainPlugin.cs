﻿using Microsoft.Xna.Framework;
using Rests;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.ID;

namespace ProgressQuery
{
    [ApiVersion(2, 1)]//api版本
    public class MainPlugin : TerrariaPlugin
    {
        public override string Author => "少司命";// 插件作者
        public override string Description => "查询游戏进度";// 插件说明
        public override string Name => "进度查询";// 插件名字
        public override Version Version => new Version(1, 0, 0, 5);// 插件版本

        public static event OnGameProgressHandler? OnGameProgressEvent;

        public static HashSet<int> DetectNPCs = Utils.GetDetectNPCs();

        private Dictionary<string, bool> GameProgress;

        private Dictionary<string, HashSet<int>> ProgressNpcIds = Utils.GetProgressNpcIds();

        private Dictionary<string, string> ProgressFields = Utils.GetProgressNames();

        public MainPlugin(Main game) : base(game)// 插件处理
        {
            Order = 5;
        }

        public override void Initialize()
        {
            string[] cmd = { "进度查询", "查询进度", "progress" };
            ServerApi.Hooks.NpcKilled.Register(this, OnkillNpc);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPost);
            TShock.RestApi.Register(new SecureRestCommand("/Progress", ProgressAPI, "ProgressQuery.use"));
            Commands.ChatCommands.Add(new Command("ProgressQuery.use", Query, cmd));
            Commands.ChatCommands.Add(new Command("Progress.admin", Set, "进度设置"));
        }

        private void OnPost(EventArgs args)
        {
            GameProgress = Utils.GetGameProgress();
        }

        private void OnkillNpc(NpcKilledEventArgs args)
        {
            if (DetectNPCs.Contains(args.npc.type))
            {
                var going = Utils.Ongoing();
                ProgressNpcIds.ForEach(x =>
                {
                    if (x.Value.Contains(args.npc.type))
                    {
                        if (GameProgress.TryGetValue(x.Key, out bool cod))
                        {
                            //如果进度false
                            if (!cod)
                            {
                                //更新缓存
                                GameProgress[x.Key] = true;
                                //如果事件
                                if (going.ContainsKey(x.Key))
                                { 
                                    //更新同步进度数据库
                                    DataSync.Plugin.UploadProgress(ProgressFields[x.Key], true);
                                    //设置进度true
                                    Utils.SetGameProgress(x.Key, true);
                                }

                                OnGameProgressEvent?.Invoke(new OnGameProgressEventArgs
                                {
                                    Name = x.Key,
                                    code = true
                                });
                            }
                        }
                    }
                });
            }
        }

        private void Set(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage("输入/进度设置 <名称>");
                return;
            }
            if (Utils.GetProgressFilelds().TryGetValue(args.Parameters[0], out FieldInfo? field) && field != null)
            {
                var code = !Convert.ToBoolean(field.GetValue(null));
                field.SetValue(null, code);
                OnGameProgressEvent?.Invoke(new OnGameProgressEventArgs()
                {
                    Name = args.Parameters[0],
                    code = code
                });
                DataSync.Plugin.UploadProgress(ProgressFields[args.Parameters[0]], code);
                GameProgress[args.Parameters[0]] = code;
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
            StringBuilder Killed = new StringBuilder("目前已击杀:");
            StringBuilder NotKilled = new StringBuilder("目前未击杀:");
            Killed.AppendJoin(",", Progress.Where(x => x.Value).Select(x=>x.Key));
            NotKilled.AppendJoin(",", Progress.Where(x => !x.Value).Select(x => x.Key));
            args.Player.SendInfoMessage(Killed.Color(TShockAPI.Utils.GreenHighlight));
            args.Player.SendInfoMessage(NotKilled.Color(TShockAPI.Utils.PinkHighlight));
        }

        protected override void Dispose(bool disposing)// 插件关闭时
        {
            base.Dispose(disposing);
        }
    }
}