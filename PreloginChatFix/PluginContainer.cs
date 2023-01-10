using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Terraria;
using TerrariaApi.Server;

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
            TShockAPI.Hooks.PlayerHooks.PlayerChat += PlayerHooks_PlayerChat;
            base.Initialize();
        }

        private static void PlayerHooks_PlayerChat(TShockAPI.Hooks.PlayerChatEventArgs e)
        {
            if (!e.Player.Active && !e.Player.IsLoggedIn)
                e.Player.Ban("trying to chat before spawn");
        }
    }
}
