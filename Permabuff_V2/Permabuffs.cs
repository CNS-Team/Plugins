using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Permabuffs_V2
{
	[ApiVersion(2, 1)]
	public class Permabuffs : TerrariaPlugin
	{
		public override string Name { get { return "Permabuffs"; } }
		public override string Author { get { return "Zaicon&Cai改"; } }
		public override string Description { get { return "永久Buff插件."; } }
		public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version!; } }

		private static System.Timers.Timer ?update;
		private static List<int> globalbuffs = new List<int>();
		private static List<RegionBuff> regionbuffs = new List<RegionBuff>();
		private static Dictionary<int, List<string>> hasAnnounced = new Dictionary<int, List<string>>();

		public static string configPath = Path.Combine(TShock.SavePath, "PermabuffsConfig.json");
		public static Config config = Config.Read(configPath);// = new Config();

		public Permabuffs(Main game)
			: base(game)
		{
			base.Order = 1;
		}

		#region Initalize/Dispose
		public override void Initialize()
		{
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
			ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			AccountHooks.AccountDelete += OnAccDelete;
			PlayerHooks.PlayerPostLogin += OnPostLogin;
			RegionHooks.RegionEntered += OnRegionEnter;
			GeneralHooks.ReloadEvent += PBReload;
		}

		protected override void Dispose(bool Disposing)
		{
			if (Disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
				ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
				ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
				AccountHooks.AccountDelete -= OnAccDelete;
				RegionHooks.RegionEntered -= OnRegionEnter;
				PlayerHooks.PlayerPostLogin -= OnPostLogin;
				GeneralHooks.ReloadEvent -= PBReload;
			}
			base.Dispose(Disposing);
		}
		#endregion

		#region Hooks
		public void OnInitialize(EventArgs args)
		{
			DB.Connect();

			update = new System.Timers.Timer { Interval = 1000, AutoReset = true, Enabled = true };
			update.Elapsed += OnElapsed!;

			Commands.ChatCommands.Add(new Command("pb.use", PBuffs, "permabuff") { AllowServer = false, HelpText = "给你一个永久Buff." });
			Commands.ChatCommands.Add(new Command("pb.check", PBCheck, "buffcheck") { HelpText = "列出玩家有效的永久Buff." });
			Commands.ChatCommands.Add(new Command("pb.give", PBGive, "gpermabuff") { HelpText = "给一个玩家永久Buff." });
			Commands.ChatCommands.Add(new Command("pb.region", PBRegion, "regionbuff"));
			Commands.ChatCommands.Add(new Command("pb.global", PBGlobal, "globalbuff"));
			Commands.ChatCommands.Add(new Command("pb.use", PBClear, "clearbuffs") { HelpText = "清除所有Buff." });
		}

		public static void OnGreet(GreetPlayerEventArgs args)
		{
			if (TShock.Players[args.Who] == null)
				return;

			if (globalbuffs.Count > 0)
				TShock.Players[args.Who].SendInfoMessage("此服务器具有以下有效的全局PermaBuffs: {0}", string.Join(", ", globalbuffs.Select(p => TShock.Utils.GetBuffName(p))));

			if (!hasAnnounced.ContainsKey(args.Who))
				hasAnnounced.Add(args.Who, new List<string>());

			if (!TShock.Players[args.Who].IsLoggedIn)
				return;

			int id = TShock.Players[args.Who].Account.ID;

// 			if (!DB.PlayerBuffs.ContainsKey(id))
// 			{
// 				if (DB.LoadUserBuffs(id))
// 				{
// 					if (DB.PlayerBuffs[id].bufflist.Count > 0)
// 						TShock.Players[args.Who].SendInfoMessage("你上一个会话({0})的PeraBuff仍处于有效状态!", string.Join(", ", DB.PlayerBuffs[id].bufflist.Select(p => TShock.Utils.GetBuffName(p))));
// 				}
// 				else
// 					DB.AddNewUser(TShock.Players[args.Who].Account.ID);
// 			}
// 			else
// 			{
// 				//loadDBInfo(args.Who);
// 				if (DB.PlayerBuffs[id].bufflist.Count > 0)
// 					TShock.Players[args.Who].SendInfoMessage("你上一个会话({0})的PeraBuff仍处于有效状态!", string.Join(", ", DB.PlayerBuffs[id].bufflist.Select(p => TShock.Utils.GetBuffName(p))));
// 			}
		}

		public static void OnPostLogin(PlayerPostLoginEventArgs args)
		{
			if (!DB.PlayerBuffs.ContainsKey(args.Player.Account.ID))
			{
				if (DB.LoadUserBuffs(args.Player.Account.ID))
				{
					if (DB.PlayerBuffs[args.Player.Account.ID].bufflist.Count > 0)
						args.Player.SendInfoMessage("你上一个会话({0})的PeraBuff仍处于有效状态!", string.Join(", ", DB.PlayerBuffs[args.Player.Account.ID].bufflist.Select(p => TShock.Utils.GetBuffName(p))));
				}
				else
					DB.AddNewUser(args.Player.Account.ID);
			}
			else
			{
				DB.PlayerBuffs.Remove(args.Player.Account.ID);
				DB.LoadUserBuffs(args.Player.Account.ID);
				if (DB.PlayerBuffs[args.Player.Account.ID].bufflist.Count > 0)
					args.Player.SendInfoMessage("你上一个会话({0})的PeraBuff仍处于有效状态!", string.Join(", ", DB.PlayerBuffs[args.Player.Account.ID].bufflist.Select(p => TShock.Utils.GetBuffName(p))));
			}
		}

		public static void OnAccDelete(AccountDeleteEventArgs args)
		{
			DB.ClearPlayerBuffs(args.Account.ID);
		}

		public void OnRegionEnter(RegionHooks.RegionEnteredEventArgs args)
		{
			RegionBuff rb = config.regionbuffs.FirstOrDefault(p => p.regionName == args.Region.Name && p.buffs.Count > 0)!;

			if (rb == null)
				return;

			//Probably occurs when this is thrown before Greet (ie when spawning)
			if (!hasAnnounced.ContainsKey(args.Player.Index))
				return;

			if (hasAnnounced[args.Player.Index].Contains(args.Region.Name))
				return;

			args.Player.SendSuccessMessage("你进入了一个启用了以下buff的区域: {0}", string.Join(", ", rb.buffs.Keys.Select(p => TShock.Utils.GetBuffName(p))));
			hasAnnounced[args.Player.Index].Add(args.Region.Name);
		}

		public static void OnLeave(LeaveEventArgs args)
		{
			var plr = TShock.Players[args.Who];
			if (plr == null)
				return;

			if (hasAnnounced.Keys.Contains(args.Who))
				hasAnnounced.Remove(args.Who);

			if (!plr.IsLoggedIn)
				return;

			if (DB.PlayerBuffs.ContainsKey(plr.Account.ID))
				DB.PlayerBuffs.Remove(plr.Account.ID);
		}

		private void OnElapsed(object sender, ElapsedEventArgs args)
		{
			for (int i = 0; i < TShock.Players.Length; i++)
			{
				if (TShock.Players[i] == null)
					continue;

				foreach (int buff in globalbuffs)
				{
					TShock.Players[i].SetBuff(buff, 18000);
				}

				if (TShock.Players[i].CurrentRegion != null)
				{
					RegionBuff rb = config.regionbuffs.FirstOrDefault(p => TShock.Players[i].CurrentRegion.Name == p.regionName && p.buffs.Count > 0)!;

					if (rb != null)
					{
						foreach (KeyValuePair<int, int> kvp in rb.buffs)
						{
							TShock.Players[i].SetBuff(kvp.Key, kvp.Value * 60);
						}
					}
				}

				if (!TShock.Players[i].IsLoggedIn)
					continue;

				if (DB.PlayerBuffs.ContainsKey(TShock.Players[i].Account.ID))
					foreach (var buff in DB.PlayerBuffs[TShock.Players[i].Account.ID].bufflist)
						TShock.Players[i].SetBuff(buff, 18000);
			}
		}
		#endregion

		#region Buff Commands
		private void PBuffs(CommandArgs args)
		{
			if (config.buffgroups.Length == 0)
			{
				args.Player.SendErrorMessage("服务器管理员尚未定义任何Buff组.请联系管理员以解决此问题.");
				return;
			}

			List<BuffGroup> availableBuffGroups = config.buffgroups.Where(e => args.Player.HasPermission($"pb.{e.groupPerm}") || args.Player.HasPermission("pb.useall")).ToList();

			int bufftype = -1;

			if (args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("参数无效: {0}permabuff <Buff名/BuffID>", (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier));
				return;
			}

			string buff = string.Join(" ", args.Parameters);

			// Get buff type by name
			if (!int.TryParse(args.Parameters[0], out bufftype))
			{
				List<int> bufftypelist = TShock.Utils.GetBuffByName(buff);

				if (bufftypelist.Count < 1)
				{
					args.Player.SendErrorMessage("没有找到这个Buff.");
					return;
				}
				else if (bufftypelist.Count > 1)
				{
                    
					args.Player.SendMultipleMatchError(bufftypelist.Select(p => TShock.Utils.GetBuffName(p)));
					return;
				}
				else
					bufftype = bufftypelist[0];
			}
			else if (bufftype > Terraria.ID.BuffID.Count || bufftype < 1) // Buff ID is not valid (less than 1 or higher than 206 (1.3.5.3)).
				args.Player.SendErrorMessage("BuffID无效!");


			int playerid = args.Player.Account.ID;

			availableBuffGroups.RemoveAll(e => !e.buffIDs.Contains(bufftype));

			if (availableBuffGroups.Count == 0)
			{
				args.Player.SendErrorMessage("你没有权限使用这个PermaBuff!");
				return;
			}

			if (DB.PlayerBuffs[playerid].bufflist.Contains(bufftype))
			{
				DB.PlayerBuffs[playerid].bufflist.Remove(bufftype);
				DB.UpdatePlayerBuffs(playerid, DB.PlayerBuffs[playerid].bufflist);
				args.Player.SendInfoMessage("你移除了 " + TShock.Utils.GetBuffName(bufftype) + " PermaBuff.");
				return;
			}
			DB.PlayerBuffs[playerid].bufflist.Add(bufftype);
			DB.UpdatePlayerBuffs(playerid, DB.PlayerBuffs[playerid].bufflist);
			args.Player.SendSuccessMessage($"你已经被赋予了Buff {TShock.Utils.GetBuffName(bufftype)}! 重新输入该指令移除这个Buf.");
		}

		private void PBCheck(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("参数无效: {0}buffcheck <玩家名>", (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier));
				return;
			}

			string playername = string.Join(" ", args.Parameters);

			List<TSPlayer> players = TShockAPI.TSPlayer.FindByNameOrID(playername);

			if (players.Count < 1)
				args.Player.SendErrorMessage("没有找到该玩家.");
			else if (players.Count > 1)
				args.Player.SendMultipleMatchError( players.Select(p => p.Name));
			else if (!players[0].IsLoggedIn)
				args.Player.SendErrorMessage("{0} 没有有效的PermaBuff.", players[0].Name);
			else
			{
				if (DB.PlayerBuffs[players[0].Account.ID].bufflist.Count == 0)
					args.Player.SendInfoMessage("{0} 没有有效的PermaBuff.", players[0].Name);
				else
					args.Player.SendInfoMessage("{0} 有如下Permabuff: {1}", players[0].Name, string.Join(", ", DB.PlayerBuffs[players[0].Account.ID].bufflist.Select(p => TShock.Utils.GetBuffName(p))));
			}
		}

		private void PBGive(CommandArgs args)
		{
			if (config.buffgroups.Length == 0)
			{
				args.Player.SendErrorMessage("服务器管理员尚未定义任何Buff组.请联系管理员以解决此问题.");
				return;
			}

			List<BuffGroup> availableBuffGroups = config.buffgroups.Where(e => args.Player.HasPermission($"pb.{e.groupPerm}") || args.Player.HasPermission("pb.useall")).ToList();

			if (args.Parameters.Count == 2)
			{
				// /gpermabuffs -g list
				if (args.Parameters[0].Equals("-g", StringComparison.CurrentCultureIgnoreCase) && args.Parameters[1].Equals("list", StringComparison.CurrentCultureIgnoreCase))
				{
					args.Player.SendInfoMessage($"有效PermaBuff组: {string.Join(", ", availableBuffGroups.Select(e => e.groupName))}");
					return;
				}

				// Get player id from args.Parameters[1]
				string playername = args.Parameters[1];
				List<TSPlayer> players = TShockAPI.TSPlayer.FindByNameOrID(playername);
				if (players.Count < 1)
				{
					args.Player.SendErrorMessage("没有找到该玩家.");
					return;
				}
				else if (players.Count > 1)
				{
					args.Player.SendMultipleMatchError( players.Select(p => p.Name));
					return;
				}
				else if (!players[0].IsLoggedIn)
				{
					args.Player.SendErrorMessage("此玩家无法接收PermaBuff(没有登录)!");
					return;
				}
				int playerid = players[0].Account.ID;

				//Get buff name/id from args.Parameters[0]
				string buff = args.Parameters[0];
				if (!int.TryParse(args.Parameters[0], out int bufftype))
				{
					List<int> bufftypelist = new List<int>();
					bufftypelist = TShock.Utils.GetBuffByName(buff);

					if (bufftypelist.Count < 1)
					{
						args.Player.SendErrorMessage("没有找到这个Buff.");
						return;
					}
					else if (bufftypelist.Count > 1)
					{
						args.Player.SendMultipleMatchError( bufftypelist.Select(p => TShock.Utils.GetBuffName(p)));
						return;
					}
					else
						bufftype = bufftypelist[0];
				}
				else if (bufftype > Terraria.ID.BuffID.Count || bufftype < 1) // Buff ID is not valid (less than 1 or higher than 192 (1.3.1)).
					args.Player.SendErrorMessage("BuffID无效!");

				//Removes all groups where the buff isn't included, leaving only a list of groups where player has access AND contains the buff
				availableBuffGroups.RemoveAll(e => !e.buffIDs.Contains(bufftype));

				if (availableBuffGroups.Count == 0)
				{
					args.Player.SendErrorMessage("你没有权限使用这个Permabuff!");
					return;
				}

				if (DB.PlayerBuffs[playerid].bufflist.Contains(bufftype))
				{
					DB.PlayerBuffs[playerid].bufflist.Remove(bufftype);
					DB.UpdatePlayerBuffs(playerid, DB.PlayerBuffs[playerid].bufflist);
					args.Player.SendInfoMessage($"你移除了 {players[0].Name} 的PermaBuff {TShock.Utils.GetBuffName(bufftype)}.");
					if (!args.Silent)
						players[0].SendInfoMessage($"{args.Player.Name} 移除了你的PermaBuff {TShock.Utils.GetBuffName(bufftype)}.");
				}
				else
				{
					DB.PlayerBuffs[playerid].bufflist.Add(bufftype);
					DB.UpdatePlayerBuffs(playerid, DB.PlayerBuffs[playerid].bufflist);
					args.Player.SendSuccessMessage($"你赋予了玩家 {players[0].Name} PermaBuff {TShock.Utils.GetBuffName(bufftype)}!");
					if (!args.Silent)
						players[0].SendInfoMessage($"{args.Player.Name} 赋予了你PermaBuff {TShock.Utils.GetBuffName(bufftype)}!");
				}
			}
			//gpermabuff -g <group> <player>
			else if (args.Parameters.Count == 3)
			{
				if (args.Parameters[0] != "-g")
				{
					args.Player.SendErrorMessage("参数无效:");
					args.Player.SendErrorMessage("{0}gpermabuff <Buff名/BuffID> <玩家名>", TShock.Config.Settings.CommandSpecifier);
					args.Player.SendErrorMessage("{0}gpermabuff -g <Buff组> <玩家名>", TShock.Config.Settings.CommandSpecifier);
				}

				var matchedPlayers = TShockAPI.TSPlayer.FindByNameOrID(args.Parameters[2]);

				if (matchedPlayers.Count == 0)
				{
					args.Player.SendErrorMessage($"没有找到该玩家: {args.Parameters[2]}");
					return;
				}
				else if (matchedPlayers.Count > 1)
				{
					args.Player.SendMultipleMatchError( matchedPlayers.Select(p => p.Name));
					return;
				}
				else if (!matchedPlayers[0].IsLoggedIn)
				{
					args.Player.SendErrorMessage("此玩家无法接收PermaBuff!");
					return;
				}
				else if (!availableBuffGroups.Any(e => e.groupName.Equals(args.Parameters[1], StringComparison.CurrentCultureIgnoreCase)))
				{
					args.Player.SendErrorMessage("无法查询到指定的Buff组!");
				}

				TSPlayer player = matchedPlayers[0];
				int id = matchedPlayers[0].Account.ID;

				foreach (var buff in availableBuffGroups.First(e => e.groupName.Equals(args.Parameters[1], StringComparison.CurrentCultureIgnoreCase)).buffIDs)
				{
					if (!DB.PlayerBuffs[id].bufflist.Contains(buff))
						DB.PlayerBuffs[id].bufflist.Add(buff);
				}
				DB.UpdatePlayerBuffs(id, DB.PlayerBuffs[id].bufflist);

				args.Player.SendSuccessMessage($"成功赋予玩家 {player.Name} PermaBuff组 {args.Parameters[1]} 中的所有Permabuff!");

				if (!args.Silent)
					args.Player.SendInfoMessage($"{args.Player.Name} 已赋予你PermaBuff组 {args.Parameters[1]} 中所有Permabuff!");
			}
			else
			{
				args.Player.SendErrorMessage("参数无效:");
				args.Player.SendErrorMessage("{0}gpermabuff <Buff名/BuffID> <玩家名>", TShock.Config.Settings.CommandSpecifier);
				args.Player.SendErrorMessage("{0}gpermabuff -g <Buff组> <玩家名>", TShock.Config.Settings.CommandSpecifier);
				return;
			}
		}

		private void PBReload(ReloadEventArgs args)
		{
			config = Config.Read(configPath);
			args.Player.SendWarningMessage("[Permabuff]:插件配置已重载!");
		}

		private void PBRegion(CommandArgs args)
		{
			//regionbuff <add/del> <region> <buff>

			if (args.Parameters.Count < 3 || args.Parameters.Count > 4)
			{
				args.Player.SendErrorMessage("参数无效: {0}regionbuff <add/del> <区域名> <Buff名/BuffID> [持续时间]", (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier));
				return;
			}

			if (args.Parameters[0].Equals("add", StringComparison.CurrentCultureIgnoreCase))
			{
				string regionname = args.Parameters[1];
				Region region = TShock.Regions.GetRegionByName(regionname);
				string buffinput = args.Parameters[2];
				if (args.Parameters.Count != 4)
				{
					args.Player.SendErrorMessage("参数无效: {0}regionbuff <add/del> <区域名> <Buff名/BuffID> [持续时间]", (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier));
					return;
				}
				string durationinput = args.Parameters[3];
				int bufftype = -1;

				if (region == null)
				{
					args.Player.SendErrorMessage("区域无效: {0}", regionname);
					return;
				}

				if (!int.TryParse(buffinput, out bufftype))
				{
					List<int> bufflist = TShock.Utils.GetBuffByName(buffinput);

					if (bufflist.Count == 0)
					{
						args.Player.SendErrorMessage("没有找到与 {0} 匹配的Buff.", buffinput);
						return;
					}

					if (bufflist.Count > 1)
					{
						args.Player.SendMultipleMatchError( bufflist.Select(p => TShock.Utils.GetBuffName(p)));
						return;
					}

					bufftype = bufflist[0];
				}

				if (bufftype < 0 || bufftype > Terraria.ID.BuffID.Count)
				{
					args.Player.SendErrorMessage("BuffID无效: {0}", bufftype.ToString());
					return;
				}

				int duration = -1;

				if (!int.TryParse(durationinput, out duration) || (duration < 1 || duration > 540))
				{
					args.Player.SendErrorMessage("持续时间无效!");
					return;
				}

				bool found = false;

				for (int i = 0; i < config.regionbuffs.Length; i++)
				{
					if (config.regionbuffs[i].regionName == region.Name)
					{
						found = true;
						if (config.regionbuffs[i].buffs.Keys.Contains(bufftype))
						{
							args.Player.SendErrorMessage("区域 {0} 已经添加了Buff {1}!", region.Name, TShock.Utils.GetBuffName(bufftype));
							return;
						}
						else
						{
							config.regionbuffs[i].buffs.Add(bufftype, duration);
							args.Player.SendSuccessMessage("成功添加Buff {0} 至区域 {1} 持续时间{2}秒!", TShock.Utils.GetBuffName(bufftype), region.Name, duration.ToString());
							config.Write(configPath);
							return;
						}
					}
				}

				if (!found)
				{
					List<RegionBuff> temp = config.regionbuffs.ToList();
					temp.Add(new RegionBuff() { buffs = new Dictionary<int, int>() { { bufftype, duration } }, regionName = region.Name });
					config.regionbuffs = temp.ToArray();
					args.Player.SendSuccessMessage("成功添加Buff {0} 至区域 {1} 持续时间{2}秒!", TShock.Utils.GetBuffName(bufftype), region.Name, duration.ToString());
					config.Write(configPath);
					return;
				}
			}

			if (args.Parameters[0].Equals("del", StringComparison.CurrentCultureIgnoreCase) || args.Parameters[0].Equals("delete", StringComparison.CurrentCultureIgnoreCase))
			{
				string regionname = args.Parameters[1];
				Region region = TShock.Regions.GetRegionByName(regionname);
				string buffinput = args.Parameters[2];
				int bufftype = -1;

				if (region == null)
				{
					args.Player.SendErrorMessage("区域无效: {0}", regionname);
					return;
				}

				if (!int.TryParse(buffinput, out bufftype))
				{
					List<int> bufflist = TShock.Utils.GetBuffByName(buffinput);

					if (bufflist.Count == 0)
					{
						args.Player.SendErrorMessage("没有找到与 {0} 匹配的Buff.", buffinput);
						return;
					}

					if (bufflist.Count > 1)
					{
						args.Player.SendMultipleMatchError( bufflist.Select(p => TShock.Utils.GetBuffName(p)));
						return;
					}

					bufftype = bufflist[0];
				}

				if (bufftype < 0 || bufftype > Terraria.ID.BuffID.Count)
				{
					args.Player.SendErrorMessage("BuffID无效: {0}", bufftype.ToString());
					return;
				}

				bool found = false;

				for (int i = 0; i < config.regionbuffs.Length; i++)
				{
					if (config.regionbuffs[i].regionName == region.Name)
					{
						if (config.regionbuffs[i].buffs.ContainsKey(bufftype))
						{
							config.regionbuffs[i].buffs.Remove(bufftype);
							args.Player.SendSuccessMessage("成功从区域 {1} 移除Buff {0}!", TShock.Utils.GetBuffName(bufftype), region.Name);
							config.Write(configPath);
							found = true;
							return;
						}
					}
				}

				if (!found)
				{
					args.Player.SendSuccessMessage("Buff {0} 不在区域 {1} 里!", TShock.Utils.GetBuffName(bufftype), region.Name);
					return;
				}
			}

			args.Player.SendErrorMessage("参数无效: {0}regionbuff <add/del> <区域名> <Buff名/BuffID>", (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier));
		}

		private void PBGlobal(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("参数无效: {0}globalbuff <Buff名>", (args.Silent ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier));
				return;
			}

			string buff = string.Join(" ", args.Parameters);

			if (!int.TryParse(args.Parameters[0], out int bufftype))
			{
				List<int> bufftypelist = TShock.Utils.GetBuffByName(buff);

				if (bufftypelist.Count < 1)
				{
					args.Player.SendErrorMessage("没有找到与之匹配的Buff.");
					return;
				}
				else if (bufftypelist.Count > 1)
				{
					args.Player.SendMultipleMatchError( bufftypelist.Select(p => TShock.Utils.GetBuffName(p)));
					return;
				}
				else
					bufftype = bufftypelist[0];
			}

			if (bufftype > Terraria.ID.BuffID.Count || bufftype < 1) // Buff ID is not valid (less than 1 or higher than 190).
				args.Player.SendErrorMessage("无效的BuffID!");

			if (!config.buffgroups.Any(e => e.buffIDs.Contains(bufftype)))
				args.Player.SendErrorMessage("此Buff不能作为全局buff使用!");
			else if (globalbuffs.Contains(bufftype))
			{
				globalbuffs.Remove(bufftype);
				args.Player.SendSuccessMessage("Buff {0} 成功从全局PermaBuff被移除 .", TShock.Utils.GetBuffName(bufftype));
			}
			else
			{
				globalbuffs.Add(bufftype);
				args.Player.SendSuccessMessage("Buff {0} 成功被加入全局PermaBuff!", TShock.Utils.GetBuffName(bufftype));
			}
		}

		private void PBClear(CommandArgs args)
		{
			if (args.Parameters.Count == 1 && (args.Parameters[0] == "*" || args.Parameters[0].Equals("all", StringComparison.CurrentCultureIgnoreCase)))
			{
				if (!args.Player.HasPermission("pb.clear"))
				{
					args.Player.SendErrorMessage("你没有权限清除所有Buff.");
					return;
				}
				foreach (KeyValuePair<int, DBInfo> kvp in DB.PlayerBuffs)
				{
					kvp.Value.bufflist.Clear();
					DB.ClearDB();
				}
				args.Player.SendSuccessMessage("所有玩家的所有PermaBuff被成功移除.");
				if (!args.Silent)
				{
					TSPlayer.All.SendInfoMessage("{0} 移除了所有玩家的所有PermaBuff!", args.Player.Account.Name);
				}
			}
			else if (args.Parameters.Count == 1)
			{
				var plrs = TSPlayer.FindByNameOrID(args.Parameters[0]);
				if (plrs.Count> 1)
				{
					args.Player.SendMultipleMatchError(plrs.Select(x=>x.Name));
					return;
				}
				else if (plrs.Count == 1)
				{
					DB.PlayerBuffs[plrs[0].Account.ID].bufflist.Clear();
					DB.ClearPlayerBuffs(plrs[0].Account.ID);
					args.Player.SendSuccessMessage($"玩家{plrs[0].Name}所有的PermaBuff都被清除了.");
					return;
                }
				var acc = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
				if (acc == null)
				{
                    args.Player.SendErrorMessage("没有找到该玩家或账户");
                    return;
                }
                DB.ClearPlayerBuffs(acc.ID);
                args.Player.SendSuccessMessage($"用户{acc.Name}所有的PermaBuff都被清除了.");

            }
            else
			{
				if (!args.Player.RealPlayer)
				{
					args.Player.SendErrorMessage("你必须在游戏中使用该指令.");
					return;
				}
				DB.PlayerBuffs[args.Player.Account.ID].bufflist.Clear();

				DB.ClearPlayerBuffs(args.Player.Account.ID);
				args.Player.SendSuccessMessage("你所有的PermaBuff都被清除了.");
			}
		}
		#endregion
	}
}
