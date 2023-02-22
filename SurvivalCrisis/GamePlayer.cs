using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace SurvivalCrisis
{
    using NetInventory = Nets.NetInventory;
    using NetPiggybank = Nets.NetPiggybank;
    public class GamePlayer
    {
        #region Fields
        private int timer;
        private PlayerIdentity identity;
        #endregion
        #region Properties
        public int Index { get; }
        public PlayerData Data { get; }
        public NetInventory Inventory { get; }
        public NetPiggybank Piggybank { get; }
        public Queue<Effect> Effects { get; }
        public bool IsGhost
        {
            get;
            set;
        }
        public PlayerIdentity Party
        {
            get;
            set;
        }
        public PlayerIdentity Identity
        {
            get => this.identity;
            set
            {
                this.identity = value;
                if (SurvivalCrisis.Instance.IsInGame)
                {
                    switch (this.identity)
                    {
                        case PlayerIdentity.Traitor:
                            this.SendText(Texts.YouAreTraitor, Color.Yellow);
                            break;
                        case PlayerIdentity.Survivor:
                            this.SendText(Texts.YouAreSurvivor, Color.Yellow);
                            break;
                    }
                }
            }
        }
        public Player TPlayer => Main.player[this.Index];
        public TSPlayer TSPlayer => TShock.Players[this.Index];
        public string Name => this.TPlayer.name;
        public string Prefix => SurvivalCrisis.Instance.Prefixs[this.Data.CurrentPrefixID];
        public string Title => SurvivalCrisis.Instance.Titles[this.Data.CurrentTitleID];
        public Item HeldItem => this.TPlayer.HeldItem;
        public int Life
        {
            get => this.TPlayer.statLife;
            set
            {
                this.TPlayer.statLife = value;
                this.TSPlayer.SendData(PacketTypes.PlayerHp, string.Empty, this.Index);
            }
        }
        public int LifeMax
        {
            get => this.TPlayer.statLifeMax2;
            set
            {
                this.TPlayer.statLifeMax = value;
                this.TSPlayer.SendData(PacketTypes.PlayerHp, string.Empty, this.Index);
            }
        }
        public int ManaMax
        {
            get => this.TPlayer.statManaMax2;
            set
            {
                this.TPlayer.statManaMax = value;
                this.TSPlayer.SendData(PacketTypes.PlayerMana, string.Empty, this.Index);
            }
        }
        public int Team
        {
            get => this.TSPlayer.Team;
            set => this.TSPlayer.SetTeam(value);
        }
        public bool Pvp
        {
            get => this.TPlayer.hostile;
            set
            {
                this.TPlayer.hostile = value;
                TSPlayer.All.SendData(PacketTypes.TogglePvp, string.Empty, this.Index);
            }
        }
        public bool IsGuest => this.Data == null;
        public string LastStatusMessage
        {
            get;
            private set;
        }
        public string StatusMessage
        {
            get;
            set;
        }

        public int KillingCount
        {
            get;
            set;
        }
        /// <summary>
        /// 当前游戏的
        /// </summary>
        public int KilledCount
        {
            get;
            set;
        }
        /// <summary>
        /// 当前游戏的
        /// </summary>
        public int DamageCaused
        {
            get;
            set;
        }
        public int ChestsOpened
        {
            get;
            set;
        }
        public int SurvivedFrames
        {
            get;
            set;
        }
        public double PerformanceScore
        {
            get;
            private set;
        }
        public bool CanVote
        {
            get;
            set;
        }
        public int VotedCount
        {
            get;
            set;
        }
        public int WarpingCountDown
        {
            get;
            set;
        }
        public int WarpingCount
        {
            get;
            set;
        }
        public int ChatCount
        {
            get;
            set;
        }
        public int GoldenKeyFound
        {
            get;
            set;
        }
        #endregion
        #region Ctor
        public GamePlayer(int index, PlayerData data)
        {
            this.Index = index;
            this.Data = data;
            this.Inventory = new NetInventory(this);
            this.Piggybank = new NetPiggybank(this);
            this.Effects = new Queue<Effect>();
            this.identity = PlayerIdentity.Watcher;
        }
        #endregion
        #region Methods
        public bool Equipped(int itemType)
        {
            return this.TPlayer.armor.Count(item => item.active && item.type == itemType) > 0;
        }
        public void OnHurt(GetDataHandlers.PlayerDamageEventArgs args)
        {
            if (args.PVP)
            {
                var source = SurvivalCrisis.Instance.Players[args.PlayerDeathReason._sourcePlayerIndex];
                if (source.Identity == PlayerIdentity.Traitor)
                {
                    source.DamageCaused += args.Damage;
                }
            }
        }
        public void OnStrikeNpc(NpcStrikeEventArgs args)
        {
            if (this.Identity == PlayerIdentity.Watcher)
            {
                args.Handled = true;
                return;
            }
            var index = args.Npc.realLife > 0 ? args.Npc.realLife : args.Npc.whoAmI;
            if (this.Identity == PlayerIdentity.Survivor)
            {
                if (index == SurvivalCrisis.Instance.FinalBossIndex)
                {
                    this.DamageCaused += (int) Main.CalculateDamageNPCsTake(args.Damage, args.Npc.defense);
                }
            }
        }
        public void OnKill(GetDataHandlers.KillMeEventArgs args)
        {
            // var reason = PlayerDeathReason.LegacyEmpty();
            this.KilledCount++;
            if (args.Pvp)
            {
                var killer = SurvivalCrisis.Instance.Players[args.PlayerDeathReason._sourcePlayerIndex];
                killer.KillingCount++;
                if (killer.Party == PlayerIdentity.Survivor && this.Party == PlayerIdentity.Traitor)
                {
                    if (!SurvivalCrisis.Instance.IsFinalBattleTime && !killer.HasPrefix(-2))
                    {
                        killer.AddPrefix(-2);
                    }
                }
                else if (killer.Party == PlayerIdentity.Traitor && SurvivalCrisis.Instance.GameTime <= SurvivalCrisis.NightTime + (2 * 60 * 60))
                {
                    if (!killer.HasPrefix(-7))
                    {
                        killer.AddPrefix(-7);
                    }
                }
            }
            if (!args.Pvp && !SurvivalCrisis.Instance.IsFinalBattleTime && SurvivalCrisis.Instance.TraitorEMPTime == 0)
            {
                var pos = SurvivalCrisis.Regions.Hall.Center;
                this.TSPlayer.Spawn(pos.X, pos.Y, PlayerSpawnContext.ReviveFromDeath);
                SurvivalCrisis.Instance.BCToAll(this.Index + "号失去了意识...", Color.Yellow);
                args.Handled = true;
                return;
            }
            this.DropItems();
            this.ToGhost();
            this.SurvivedFrames = SurvivalCrisis.Instance.GameTime;
            switch (this.Party)
            {
                case PlayerIdentity.Survivor:
                    this.Data.SurvivorDatas.KillingCount += this.KillingCount;
                    this.Data.SurvivorDatas.KilledCount += this.KilledCount;
                    this.Data.SurvivorDatas.DamageCaused += this.DamageCaused;
                    this.Data.SurvivorDatas.MaxSurvivalFrames = Math.Max(this.Data.SurvivorDatas.MaxSurvivalFrames, SurvivalCrisis.Instance.GameTime);
                    break;
                case PlayerIdentity.Traitor:
                    this.Data.TraitorDatas.KillingCount += this.KillingCount;
                    this.Data.TraitorDatas.KilledCount += this.KilledCount;
                    this.Data.TraitorDatas.DamageCaused += this.DamageCaused;
                    this.Data.TraitorDatas.MaxSurvivalFrames = Math.Max(this.Data.TraitorDatas.MaxSurvivalFrames, SurvivalCrisis.Instance.GameTime);
                    break;
            }
            this.identity = PlayerIdentity.Watcher;
            // NetMessage.SendPlayerDeath(Index, reason, short.MaxValue, 1, false, -1, Index);
            SurvivalCrisis.Instance.BCToAll(Texts.SomeoneKilled, Color.Yellow);
            this.TSPlayer.Spawn(PlayerSpawnContext.ReviveFromDeath, 0);
        }
        public void OnGameEnd()
        {
            this.ToActive();
            this.Identity = PlayerIdentity.Watcher;
            this.Team = 0;
            this.ClearEffects();
            if (this.ChatCount == 0)
            {
                if (!this.HasPrefix(-1))
                {
                    this.AddPrefix(-1);
                }
            }
            if (this.KillingCount >= 4)
            {
                if (!this.HasPrefix(-3))
                {
                    this.AddPrefix(-3);
                }
            }
            if (this.ChestsOpened >= 75)
            {
                if (!this.HasPrefix(-4))
                {
                    this.AddPrefix(-4);
                }
            }
            if (this.WarpingCount == 0 && this.KilledCount == 0)
            {
                if (!this.HasPrefix(-5))
                {
                    this.AddPrefix(-5);
                }
            }
            if (this.GoldenKeyFound >= 2)
            {
                if (!this.HasPrefix(-8))
                {
                    this.AddPrefix(-8);
                }
            }
        }
        public void ToGhost()
        {
            if (this.TSPlayer == null)
            {
                return;
            }
            this.TSPlayer.RespawnTimer = int.MaxValue;
            this.IsGhost = true;
            NetMessage.SendData(14, -1, this.Index, null, this.Index, false.GetHashCode());
            this.TSPlayer.GodMode = true;
            var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
            power.SetEnabledState(this.Index, true);

            if (false)
            {
                NetMessage.SendData(4, -1, this.Index, null, this.Index, 0f, 0f, 0f, 0, 0, 0);
                NetMessage.SendData(13, -1, this.Index, null, this.Index, 0f, 0f, 0f, 0, 0, 0);
            }
        }
        public void SetGodmode(bool value)
        {
            var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
            power.SetEnabledState(this.Index, value);
        }
        public void ToActive()
        {
            this.TPlayer.active = true;
            this.IsGhost = false;
            var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
            power.SetEnabledState(this.Index, false);
            this.TSPlayer.GodMode = false;
            NetMessage.SendData(14, -1, this.Index, null, this.Index, true.GetHashCode());
            if (true)
            {
                NetMessage.SendData(4, -1, this.Index, null, this.Index, 0f, 0f, 0f, 0, 0, 0);
                NetMessage.SendData(13, -1, this.Index, null, this.Index, 0f, 0f, 0f, 0, 0, 0);
            }
        }
        public void Update()
        {
            if (this.TSPlayer == null)
            {
                return;
            }
            this.timer++;
            if (this.timer % 60 == 0 && this.StatusMessage != null)
            {
                this.SendStatusMessage(this.StatusMessage);
                this.LastStatusMessage = this.StatusMessage;
                this.StatusMessage = null;
            }
            if (!SurvivalCrisis.Instance.IsInGame)
            {
                return;
            }
            if (this.timer % 60 == 0)
            {
                if (!SurvivalCrisis.Regions.GamingZone.InRange(this))
                {
                    this.TeleportTo(SurvivalCrisis.Regions.Hall.Center);
                }
                if (this.Equipped(ItemID.DiscountCard))
                {
                    SurvivalCrisis.Instance.TraitorShop.DisplayToByStatusText(this);
                }
                if (this.identity == PlayerIdentity.Traitor)
                {
                    if (this.TPlayer.FindBuffIndex(BuffID.Battle) == -1)
                    {
                        this.TSPlayer.SetBuff(BuffID.Battle, 60 * 3000);
                    }
                    if (this.Equipped(ItemID.SpectreGoggles))
                    {
                        var players = SurvivalCrisis.Instance.Players.Where(p => p?.Identity == PlayerIdentity.Survivor);
                        foreach (var player in players)
                        {
                            var dist = player.TPlayer.Distance(this.TPlayer.Center) / 16;
                            var msg = $@"{player.Index}号:  {(int) dist}格";
                            if (SurvivalCrisis.Instance.IsMagnetStorm)
                            {
                                msg = $@"烫烫烫:  锟斤拷";
                            }
                            var textPos = player.TPlayer.Center - this.TPlayer.Center;
                            textPos.Normalize();
                            textPos *= 16 * 20;
                            Utils.SendCombatText(msg, Color.MediumPurple, this.TPlayer.Center + textPos, this.Index);
                        }
                    }
                }
                if (SurvivalCrisis.Instance.CurrentTask != null)
                {
                    this.AddStatusMessage("当前任务: " + SurvivalCrisis.Instance.CurrentTask.Name);
                    this.AddStatusMessage(SurvivalCrisis.Instance.CurrentTask.CurrentProcess());
                }
            }
            if (SurvivalCrisis.Instance.IsInGame && this.identity == PlayerIdentity.Watcher)
            {
                if (!this.IsGhost)
                {
                    this.ToGhost();
                }
            }
            if (this.IsGhost)
            {
                if (this.Pvp)
                {
                    this.Pvp = false;
                }
                if (this.Team != 0)
                {
                    this.Team = 0;
                }
                this.TSPlayer.SetBuff(BuffID.Invisibility, short.MaxValue);
                if (this.TPlayer.active)
                {
                    this.ToGhost();
                }
            }
            else
            {
                if (!this.TPlayer.active)
                {
                    this.ToActive();
                }
            }
        }
        public void TeleportTo(Point pos)
        {
            this.TeleportTo(new Vector2(pos.X, pos.Y) * 16);
        }
        public void TeleportTo(Vector2 pos)
        {
            this.TSPlayer.Teleport(pos.X, pos.Y);
        }
        public void DropItems()
        {
            var player = this.TPlayer;
            var box = new Vector2(player.width, player.height);
            var pos = player.Center;
            for (var i = 0; i < player.inventory.Length - 1; i++)
            {
                var item = player.inventory[i];
                var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
                TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
                item.netDefaults(0);
            }
            for (var i = 0; i < player.miscEquips.Length; i++)
            {
                var item = player.miscEquips[i];
                var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
                TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
                item.netDefaults(0);
            }
            for (var i = 0; i < 3; i++)
            {
                var item = player.armor[i];
                var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
                TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
                item.netDefaults(0);
            }
            for (var i = 5; i < 13; i++)
            {
                var item = player.armor[i];
                var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
                TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
                item.netDefaults(0);
            }
            for (var i = 15; i < player.armor.Length; i++)
            {
                var item = player.armor[i];
                var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
                TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
                item.netDefaults(0);
            }
            for (var i = 0; i < 59; i++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, i, this.TPlayer.inventory[i].prefix);
            }
            for (var j = 0; j < this.TPlayer.armor.Length; j++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 59 + j, this.TPlayer.armor[j].prefix);
            }
            for (var k = 0; k < this.TPlayer.dye.Length; k++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 79 + k, this.TPlayer.dye[k].prefix);
            }
            for (var l = 0; l < this.TPlayer.miscEquips.Length; l++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 89 + l, this.TPlayer.miscEquips[l].prefix);
            }
            for (var m = 0; m < this.TPlayer.miscDyes.Length; m++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 94 + m, this.TPlayer.miscDyes[m].prefix);
            }
        }
        public void Reset()
        {
            this.DamageCaused = 0;
            this.KilledCount = 0;
            this.KillingCount = 0;
            this.PerformanceScore = 0;
            this.ChestsOpened = 0;
            this.SurvivedFrames = 0;
            this.VotedCount = 0;
            this.WarpingCountDown = 0;
            this.WarpingCount = 0;
            this.ChatCount = 0;
            this.GoldenKeyFound = 0;
            this.SetGodmode(false);
            this.ResetInventory();
        }
        public void ResetInventory()
        {
            this.Piggybank.Clear();
            var inventory = this.TPlayer.inventory;
            for (var i = 0; i < inventory.Length; i++)
            {
                inventory[i].TurnToAir();
            }
            for (var i = 0; i < this.TPlayer.armor.Length; i++)
            {
                this.TPlayer.armor[i].TurnToAir();
            }
            for (var i = 0; i < this.TPlayer.miscEquips.Length; i++)
            {
                this.TPlayer.miscEquips[i].TurnToAir();
            }
            if (this.Identity != PlayerIdentity.Watcher)
            {
                var t = 0;
                inventory[t++].SetDefaults(ItemID.MagicMirror);
                inventory[t++].SetDefaults(ItemID.PlatinumBroadsword);
                inventory[t++].SetDefaults(ItemID.ReaverShark);
                inventory[t++].SetDefaults(ItemID.SawtoothShark);
                inventory[t++].SetDefaults(ItemID.TungstenBow);
                inventory[t++].SetDefaults(ItemID.SlimeStaff);
                inventory[t++].SetDefaults(ItemID.SpectreBoots);
                inventory[t++].SetDefaults(ItemID.MoneyTrough);
                inventory[t++].SetDefaults(ItemID.AncientChisel);
                inventory[t++].SetDefaults(ItemID.LuckyHorseshoe);
                inventory[t++].SetDefaults(ItemID.EoCShield);
                inventory[t++].SetDefaults(ItemID.FamiliarWig);
                inventory[t++].SetDefaults(ItemID.FamiliarShirt);
                inventory[t++].SetDefaults(ItemID.FamiliarPants);
                inventory[t++].SetDefaults(ItemID.BatHook);
                inventory[t++].SetDefaults(ItemID.WeatherRadio);
                inventory[t].SetDefaults(ItemID.MusketBall);
                inventory[t].stack = 150;
                t++;
                inventory[t].SetDefaults(ItemID.WoodenArrow);
                inventory[t].stack = 100;
                t++;
                inventory[t].SetDefaults(ItemID.Wood);
                inventory[t].stack = 100;
                t++;
                inventory[t].SetDefaults(ItemID.Torch);
                inventory[t].stack = 50;
                t++;
                inventory[t].SetDefaults(ItemID.Bomb);
                inventory[t].stack = 20;
                t++;
                inventory[t].SetDefaults(ItemID.LesserHealingPotion);
                inventory[t].stack = 5;
                t++;
                inventory[t].SetDefaults(ItemID.NightOwlPotion);
                inventory[t].stack = 2;
                t++;
                inventory[t].SetDefaults(ItemID.ShinePotion);
                inventory[t].stack = 2;
                t++;
                inventory[t].SetDefaults(ItemID.SpelunkerPotion);
                inventory[t].stack = 2;
                t++;
                inventory[t].SetDefaults(ItemID.MiningPotion);
                inventory[t].stack = 2;
                t++;
                inventory[t].SetDefaults(ItemID.GravitationPotion);
                inventory[t].stack = 1;
                t++;
                inventory[t].SetDefaults(ItemID.GoldCoin);
                inventory[t].stack = 2;
                t++;

                if (this.Identity == PlayerIdentity.Traitor)
                {
                    inventory[t++].SetDefaults(ItemID.PhoenixBlaster);
                    inventory[t++].SetDefaults(ItemID.SpectreGoggles);
                    inventory[t++].SetDefaults(ItemID.DiscountCard);
                    inventory[t].SetDefaults(ItemID.ChlorophyteBullet);
                    inventory[t].stack = 70;
                    t++;
                    inventory[t].SetDefaults(ItemID.WrathPotion);
                    inventory[t].stack = 3;
                    t++;
                    inventory[t].SetDefaults(ItemID.RagePotion);
                    inventory[t].stack = 3;
                    t++;
                }
            }
            for (var i = 0; i < 59; i++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, i, this.TPlayer.inventory[i].prefix);
            }
            for (var j = 0; j < this.TPlayer.armor.Length; j++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 59 + j, this.TPlayer.armor[j].prefix);
            }
            for (var k = 0; k < this.TPlayer.dye.Length; k++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 79 + k, this.TPlayer.dye[k].prefix);
            }
            for (var l = 0; l < this.TPlayer.miscEquips.Length; l++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 89 + l, this.TPlayer.miscEquips[l].prefix);
            }
            for (var m = 0; m < this.TPlayer.miscDyes.Length; m++)
            {
                this.TSPlayer.SendData(PacketTypes.PlayerSlot, "", this.Index, 94 + m, this.TPlayer.miscDyes[m].prefix);
            }

            this.LifeMax = 200;
            this.ManaMax = 40;
        }
        public void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0, float number3 = 0, float number4 = 0, int number5 = 0)
        {
            this.TSPlayer.SendData(msgType, text, number, number2, number3, number4, number5);
        }
        public void AddPerformanceScore(double score, string reason)
        {
            this.PerformanceScore += score;
            this.SendText($"表现分+{score}({reason})", Color.YellowGreen);
        }
        public bool HasBuff(int buffID)
        {
            return this.TPlayer.FindBuffIndex(buffID) != -1;
        }
        public bool IsValid()
        {
            return this.TSPlayer != null && this.TSPlayer.Account.ID == this.Data.UserID;
        }
        public void HideName()
        {
            // var name = Name;
            // TPlayer.name = Index + "号";
            // NetMessage.TrySendData((int)PacketTypes.PlayerInfo, -1, Index, null, Index);
            // TPlayer.name = name;


            var name = this.TPlayer.name;
            var hair = this.TPlayer.hair;
            var hairColor = this.TPlayer.hairColor;
            var skinColor = this.TPlayer.skinColor;
            var skinVariant = this.TPlayer.skinVariant;
            var pantsColor = this.TPlayer.pantsColor;
            var shirtColor = this.TPlayer.shirtColor;
            var shoeColor = this.TPlayer.shoeColor;
            var shoe = this.TPlayer.shoe;


            this.TPlayer.name = this.Index + "号";
            this.TPlayer.hair = 0;
            this.TPlayer.hairColor = new Color(215, 90, 55);
            this.TPlayer.skinColor = new Color(255, 125, 90);
            this.TPlayer.skinVariant = 0;
            this.TPlayer.pantsColor = new Color(255, 230, 175);
            this.TPlayer.shirtColor = new Color(175, 165, 140);
            this.TPlayer.shoeColor = new Color(160, 105, 60);
            this.TPlayer.shoe = 0;

            NetMessage.SendData((int) PacketTypes.PlayerInfo, -1, this.Index, null, this.Index);

            this.TPlayer.name = name;
            this.TPlayer.hair = hair;
            this.TPlayer.hairColor = hairColor;
            this.TPlayer.skinColor = skinColor;
            this.TPlayer.skinVariant = skinVariant;
            this.TPlayer.pantsColor = pantsColor;
            this.TPlayer.shirtColor = shirtColor;
            this.TPlayer.shoeColor = shoeColor;
            this.TPlayer.shoe = shoe;
        }

        public void ToNextTitleID()
        {
            var t = this.Data.UnlockedTitles.FindIndex(id => id == this.Data.CurrentTitleID);
            t++;
            t %= this.Data.UnlockedTitles.Count;
            this.Data.CurrentTitleID = this.Data.UnlockedTitles[t];
            if (this.Data.CurrentTitleID != 0)
            {
                this.SendText($"你的称号已更改为 {this.Title}", Color.White);
            }
        }
        public void ToNextPrefixID()
        {
            var t = this.Data.UnlockedPrefixs.FindIndex(id => id == this.Data.CurrentPrefixID);
            t++;
            t %= this.Data.UnlockedPrefixs.Count;
            this.Data.CurrentPrefixID = this.Data.UnlockedPrefixs[t];
            if (this.Data.CurrentPrefixID != 0)
            {
                this.SendText($"你的前缀已更改为 {this.Prefix}", Color.White);
            }
        }
        public bool HasPrefix(int prefixID)
        {
            return this.Data.UnlockedPrefixs.FindIndex(id => id == prefixID) > 0;
        }
        public bool HasTitle(int titleID)
        {
            return this.Data.UnlockedTitles.FindIndex(id => id == titleID) > 0;
        }
        public void AddPrefix(int prefixID)
        {
            this.Data.UnlockedPrefixs.Add(prefixID);
            this.Data.UnlockedPrefixs.Sort();
            this.SendText($"你已获得前缀 {SurvivalCrisis.Instance.Prefixs[prefixID]}", Color.White);
        }
        public void AddTitle(int titleID)
        {
            this.Data.UnlockedTitles.Add(titleID);
            this.Data.UnlockedTitles.Sort();
            this.SendText($"你已获得称号 {SurvivalCrisis.Instance.Titles[titleID]}", Color.White);
        }
        public void BuyRandomPrefix(int cost)
        {
            if (this.Data.Coins < cost)
            {
                this.SendText($"你的可兑换积分不足(至少需要{cost}分)", Color.YellowGreen);
                return;
            }
            if (this.Data.UnlockedPrefixs.Count(id => id > 0) == SurvivalCrisis.Instance.Prefixs.Count - 8 - 1) // 特殊前缀和空前缀id为非正
            {
                this.SendText("你已解锁所有前缀", Color.DarkGreen);
                return;
            }
            var t = 0;
            var prefixesCanGet = new int[SurvivalCrisis.Instance.Prefixs.Count - 8 - 1 - this.Data.UnlockedPrefixs.Count(id => id > 0)];
            for (var i = 1; i < SurvivalCrisis.Instance.Prefixs.Count - 8 - 1; i++)
            {
                if (!this.HasPrefix(i))
                {
                    prefixesCanGet[t++] = i;
                }
            }
            this.AddPrefix(SurvivalCrisis.Rand.Next(prefixesCanGet));
            this.Data.Coins -= cost;
        }
        public void BuyRandomTitle(int cost)
        {
            if (this.Data.Coins < cost)
            {
                this.SendText($"你的可兑换积分不足(至少需要{cost}分)", Color.YellowGreen);
                return;
            }
            if (this.Data.UnlockedTitles.Count(id => id > 0) == SurvivalCrisis.Instance.Titles.Count - 1) // 特殊称号和空称号id为非正
            {
                this.SendText("你已解锁所有称号", Color.DarkGreen);
                return;
            }
            var t = 0;
            var titlesCanGet = new int[SurvivalCrisis.Instance.Titles.Count - 8 - 1 - this.Data.UnlockedTitles.Count(id => id > 0)];
            for (var i = 1; i < SurvivalCrisis.Instance.Titles.Count - 1; i++)
            {
                if (!this.HasTitle(i))
                {
                    titlesCanGet[t++] = i;
                }
            }
            this.AddTitle(SurvivalCrisis.Rand.Next(titlesCanGet));
            this.Data.Coins -= cost;
        }
        #region Effects
        public void AddEffect(Effect effect)
        {
            effect.Apply();
            this.Effects.Enqueue(effect);
        }
        public void UpdateEffects()
        {
            var count = this.Effects.Count;
            while (count-- > 0)
            {
                var effect = this.Effects.Dequeue();
                effect.Update();
                if (!effect.IsEnd)
                {
                    this.Effects.Enqueue(effect);
                }
            }
        }
        public void RemoveEffect(Effect effect)
        {
            var count = this.Effects.Count;
            while (count-- > 0)
            {
                var e = this.Effects.Dequeue();
                if (e == effect)
                {
                    e.Abort();
                    break;
                }
                this.Effects.Enqueue(e);
            }
        }
        public void ClearEffects()
        {
            foreach (var effect in this.Effects)
            {
                effect.Abort();
            }
            this.Effects.Clear();
        }
        #endregion
        #region Sends
        public void AddStatusMessage(string msg, Color color)
        {
            var hex = color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
            this.AddStatusMessage("[c/" + hex + ":" + msg + "]");
        }
        public void AddStatusMessage(string msg)
        {
            this.StatusMessage += msg + "\n";
        }
        public void SendStatusMessage(string msg)
        {
            this.TSPlayer.SendData(PacketTypes.Status, "\n\n\\nn\n\n\n\n\n\n\n" + msg);
        }
        public void SendText(string msg, Color color)
        {
            this.TSPlayer.SendMessage(msg, color);
        }
        #endregion
        #region ToString
        public override string ToString()
        {
            return this.Name;
        }
        #endregion
        #endregion
        #region Statics
        public static GamePlayer Guest(int index)
        {
            return new GamePlayer(index, null);
        }
        #endregion
    }
}