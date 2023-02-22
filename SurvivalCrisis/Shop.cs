using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis
{
    public class Shop
    {
        public class ItemInfo
        {
            public int ID { get; }
            public int Stack { get; }
            public int Prefix { get; }
            public ItemInfo(int id, int stack = 1, int prefix = 0)
            {
                this.ID = id;
                this.Stack = stack;
                this.Prefix = prefix;
            }
            public override string ToString()
            {
                return this.Prefix == 0 ? $"[i/s{this.Stack}:{this.ID}]" : $"[i/p{this.Prefix}:{this.ID}]";
            }
        }
        public class Commodity
        {
            public int ID { get; }
            public ItemInfo[] Items { get; }
            public ItemInfo[] Prices { get; }
            public string Text { get; }
            public Commodity(int id)
            {
                this.ID = id;
                switch (id)
                {
                    case 1:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.PsychoKnife, prefix: PrefixID.Broken)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.LunarOre, stack: 600),
                                new ItemInfo(ItemID.GolemPetItem)
                        };
                    }
                    break;
                    case 2:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.DaoofPow)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.LifeFruit, stack: 4),
                                new ItemInfo(ItemID.CrimsonHeart)
                        };
                    }
                    break;
                    case 3:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.Uzi)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.CactusChest, stack: 10),
                                new ItemInfo(ItemID.MagicLantern)
                        };
                    }
                    break;
                    case 4:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.SniperRifle, prefix: PrefixID.Broken)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.Penguin, stack: 8),
                                new ItemInfo(ItemID.PumpkingPetItem)
                        };
                    }
                    break;
                    case 5:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.ChlorophyteShotbow)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.SlimeChest, stack: 10),
                                new ItemInfo(ItemID.WispinaBottle)
                        };
                    }
                    break;
                    case 6:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.ToxicFlask)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.Hellstone, stack: 150),
                                new ItemInfo(ItemID.ShadowOrb)
                        };
                    }
                    break;
                    case 7:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.CursedFlames)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.Bone, stack: 100),
                                new ItemInfo(ItemID.FairyBell)
                        };
                    }
                    break;
                    case 8:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.StaffofEarth)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.RainbowBrick, stack: 300),
                                new ItemInfo(ItemID.ScalyTruffle)
                        };
                    }
                    break;
                    case 21:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.MoonlordBullet, stack: 30)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.MythrilBar, stack: 6),
                        };
                    }
                    break;
                    case 22:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.MoonlordArrow, stack: 20)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.OrichalcumBar, stack: 6),
                        };
                    }
                    break;
                    case 51:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.GoblinBattleStandard)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.TitaniumOre, stack: 30),
                            // new ItemInfo(ItemID.QueenSlimeMountSaddle)
                        };
                    }
                    break;
                    case 52:
                    {
                        this.Items = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.GoblinBattleStandard)
                        };
                        this.Prices = new ItemInfo[]
                        {
                                new ItemInfo(ItemID.AdamantiteOre, stack: 30),
                            // new ItemInfo(ItemID.QueenSlimeMountSaddle)
                        };
                    }
                    break;
                }
                this.Text = string.Join("", (object[]) this.Items) + ": " + string.Join("", (object[]) this.Prices);
            }
            public override string ToString()
            {
                return this.Text;
            }
        }

        public Commodity[] Commodities
        {
            get;
            private set;
        }
        public PlayerIdentity TargetParty { get; }

        public Shop(PlayerIdentity targetParty)
        {
            this.TargetParty = targetParty;
        }
        public void Reset()
        {
            if (this.TargetParty == PlayerIdentity.Traitor)
            {
                this.Commodities = new Commodity[8];
                var weaponIDs = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                SurvivalCrisis.Rand.Shuffle(weaponIDs);
                this.Commodities[0] = new Commodity(weaponIDs[0]);
                this.Commodities[1] = new Commodity(weaponIDs[1]);
                this.Commodities[2] = new Commodity(weaponIDs[2]);
                this.Commodities[3] = new Commodity(weaponIDs[3]);
                this.Commodities[4] = new Commodity(21);
                this.Commodities[5] = new Commodity(22);
                this.Commodities[6] = new Commodity(51);
                this.Commodities[7] = new Commodity(52);
            }
        }
        public void DisplayTo(GamePlayer player)
        {
            var title = this.TargetParty == PlayerIdentity.Traitor ? "————背叛者商店————" : "————生存者商店————";
            player.SendText(title, Color.CadetBlue);
            for (var i = 0; i < this.Commodities.Length; i++)
            {
                player.SendText($"[{i}]" + this.Commodities[i], Color.DarkBlue);
            }
        }
        public void DisplayToByStatusText(GamePlayer player)
        {
            var title = this.TargetParty == PlayerIdentity.Traitor ? "————[c/f33e1f:背叛者商店]————" : "————[c/5f9ea0:生存者商店]————";
            player.AddStatusMessage(title);
            for (var i = 0; i < this.Commodities.Length; i++)
            {
                player.AddStatusMessage(this.Commodities[i].ToString());
            }
        }
        public void TryBuy(GamePlayer player, int index)
        {
            if (index < 0 || this.Commodities.Length <= index)
            {
                player.SendText("无效的序号", Color.YellowGreen);
            }
            else
            {
                var commodity = this.Commodities[index];
                var matched = 0;
                foreach (var item in commodity.Prices)
                {
                    for (var i = 0; i < player.Inventory.Count; i++)
                    {
                        if (player.Inventory[i].Type == item.ID && player.Inventory[i].Stack >= item.Stack)
                        {
                            matched++;
                        }
                    }
                }
                if (matched == commodity.Prices.Length)
                {
                    foreach (var item in commodity.Prices)
                    {
                        for (var i = 0; i < player.Inventory.Count; i++)
                        {
                            if (player.Inventory[i].Type == item.ID && player.Inventory[i].Stack >= item.Stack)
                            {
                                player.Inventory[i].Stack -= item.Stack;
                            }
                        }
                    }
                    foreach (var item in commodity.Items)
                    {
                        player.TSPlayer.GiveItem(item.ID, item.Stack, item.Prefix);
                    }
                    player.SendText("购买成功", Color.DarkGreen);
                }
                else
                {
                    player.SendText("购买失败", Color.GreenYellow);
                }
            }
        }
        public void TryBuyFromPiggy(GamePlayer player)
        {
            for (var index = 0; index < this.Commodities.Length; index++)
            {
                this.TryBuyFromPiggy(player, index);
            }
        }
        public void TryBuyFromPiggy(GamePlayer player, int index)
        {
            if (index < 0 || this.Commodities.Length <= index)
            {
                player.SendText("无效的序号", Color.YellowGreen);
            }
            else
            {
                var commodity = this.Commodities[index];
                var matched = 0;
                foreach (var item in commodity.Prices)
                {
                    for (var i = 0; i < player.Piggybank.Count; i++)
                    {
                        if (player.Piggybank[i].Type == item.ID && player.Piggybank[i].Stack >= item.Stack)
                        {
                            matched++;
                        }
                    }
                }
                if (matched == commodity.Prices.Length)
                {
                    foreach (var item in commodity.Prices)
                    {
                        for (var i = 0; i < player.Piggybank.Count; i++)
                        {
                            if (player.Piggybank[i].Type == item.ID && player.Piggybank[i].Stack >= item.Stack)
                            {
                                player.Piggybank[i].Stack -= item.Stack;
                            }
                        }
                    }
                    foreach (var item in commodity.Items)
                    {
                        player.TSPlayer.GiveItem(item.ID, item.Stack, item.Prefix);
                    }
                    player.SendText("购买成功", Color.DarkGreen);
                }
                else
                {
                    // player.SendText("购买失败", Color.GreenYellow);
                }
            }
        }
    }
}