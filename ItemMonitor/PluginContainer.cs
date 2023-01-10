using LazyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ItemMonitor
{
    [ApiVersion(2, 1)]
    public class PluginContainer : LazyPlugin
    {
        public PluginContainer(Main game) : base(game)
        {
        }

        private static bool CheckValid(GetDataHandledEventArgs args)
        {
            return args.Player != null && args.Player.IsLoggedIn && !args.Handled;
        }

        private static Item GetCurrentItem(Player player, int slot)
        {
            return slot switch
            {
                >= 240 => new Item(),
                >= 220 => player.bank4.item[slot - 220],
                >= 180 => player.bank3.item[slot - 180],
                >= 179 => player.trashItem,
                >= 139 => player.bank2.item[slot - 139],
                >= 99 => player.bank.item[slot - 99],
                >= 94 => player.miscDyes[slot - 94],
                >= 89 => player.miscEquips[slot - 89],
                >= 79 => player.dye[slot - 79],
                >= 59 => player.armor[slot - 59],
                _ => player.inventory[slot]
            };
        }

        public override void Initialize()
        {
            base.Initialize();
            PlayerHooks.PlayerPostLogin += args => args.Player.SetData("ItemValidator", new ItemValidator(args.Player));
            PlayerHooks.PlayerLogout += args => (args.Player.RemoveData("ItemValidator") as ItemValidator).Dispose();
            GetDataHandlers.PlayerSlot.Register((_, args) =>
            {
                if (!CheckValid(args))
                {
                    return;
                }

                var last = GetCurrentItem(args.Player.TPlayer, args.Slot);
                if (!(args.Stack > 0 && args.Type != 0 && args.Slot == 179)) //trashbin
                {
                    args.Player.GetValidator().AddItem(last.netID, -last.stack);
                }

                args.Player.GetValidator().AddItem(args.Type, args.Stack);
            });
            GetDataHandlers.ChestItemChange.Register((_, args) =>
            {
                if (!CheckValid(args))
                {
                    return;
                }

                var last = Main.chest[args.ID].item[args.Slot];
                args.Player.GetValidator().AddItem(last.netID, -last.stack);
                args.Player.GetValidator().AddItem(args.Type, args.Stacks);
            });
            GetDataHandlers.PlaceItemFrame.Register((_, args) =>
            {
                if (!CheckValid(args))
                {
                    return;
                }

                args.Player.GetValidator().AddItem(args.ItemID, -args.Stack);
            });
            GetDataHandlers.DisplayDollItemSync.Register((_, args) =>
            {
                if (!CheckValid(args))
                {
                    return;
                }

                args.Player.GetValidator().AddItem(args.OldItem.netID, -args.OldItem.stack);
                args.Player.GetValidator().AddItem(args.NewItem.netID, args.NewItem.stack);
            });
            GetDataHandlers.ItemDrop.Register((_, args) =>
            {
                if (!CheckValid(args))
                {
                    return;
                }

                var last = Main.item[args.ID];
                if (args.Stacks == 0 || args.Type == 0 || // pickup
                    args.Type == last.netID && args.Position == last.position && args.Stacks <= last.stack) // pickup return
                {
                    args.Player.GetValidator().AddItem(last.netID, -last.stack);
                }

                args.Player.GetValidator().AddItem(args.Type, args.Stacks);
            });
            GetDataHandlers.TileEdit.Register((_, args) =>
            {
                if (!CheckValid(args))
                {
                    return;
                }

                if (args.Action == GetDataHandlers.EditAction.ReplaceTile || args.Action == GetDataHandlers.EditAction.PlaceTile)
                {
                    if (args.EditData == args.Player.SelectedItem.createTile)
                    {
                        args.Player.GetValidator().AddItem(args.Player.SelectedItem.netID, 1);
                    }
                }
                else if (args.Action == GetDataHandlers.EditAction.ReplaceWall || args.Action == GetDataHandlers.EditAction.PlaceWall)
                {
                    if (args.EditData == args.Player.SelectedItem.createWall)
                    {
                        args.Player.GetValidator().AddItem(args.Player.SelectedItem.netID, 1);
                    }
                }
            });
            //TODO: Add TileEntityHatRackItemSync
            ServerApi.Hooks.GamePostUpdate.Register(this, _ =>
            {
                foreach (var player in TShock.Players)
                {
                    player?.GetValidator()?.Update();
                }
            });
        }
    }
}