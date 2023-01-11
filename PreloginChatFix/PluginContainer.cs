using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PreloginChatFix
{
    [ApiVersion(2, 1)]
    public class PluginContainer : LazyPlugin
    {
        public PluginContainer(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ServerApi.Hooks.ServerChat.Register(this, ServerChat, 9999);
            base.Initialize();
        }

        private static void ServerChat(ServerChatEventArgs args)
        {
            var plr = TShock.Players[args.Who];
            if (plr.Account == null)
                plr.Disconnect("trying to chat before logged in.");
        }
    }
}
