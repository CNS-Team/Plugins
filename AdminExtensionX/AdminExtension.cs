using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;
using static TShockAPI.Hooks.GeneralHooks;

namespace AdminExtension;

[ApiVersion(2, 1)]
public class AdminExtension : TerrariaPlugin
{
	public class Config
	{
		public string 插件名 = "AdminExtensionX汉化版";

		public string 作者 = "ProfessorX,Ghasty制作,nnt汉化";

		public string 插件描述 = "添加一些有用的小命令,方便管理使用";

		public string[] 权限描述 = new string[10] { "ae.killall = 击杀全部权限", "ae.autokill = 自动击杀权限", "ae.healall = 恢复全部权限", "ae.autoheal = 自动恢复权限", "ae.forcepvp = 强制对战权限", "ae.ghost = 隐身上下线权限", "ae.butcher.npc = 清除NPC权限", "ae.butcher.friendly = 清除友好NPC权限", "ae.butcher.friendly = 查权限", "ae.killall.bypass = 拥有此权限的用户不会被击杀全部命令所击杀" };

		public string 插一句 = "如有BUG或者翻译错误等问题,请及时@nnt修改";
	}

	private Config config;

	public bool[] PvPForced = new bool[255];

	public bool[] ghostMode = new bool[255];

	public List<TSPlayer> autoKilledPlayers = new List<TSPlayer>();

	public List<TSPlayer> autoHealedPlayers = new List<TSPlayer>();

	private System.Timers.Timer updater;

	public static string Tag => TShock.Utils.ColorTag("AdminExtensionX:", Color.Teal);

	public override string Name => "AdminExtensionX汉化版";

	public override string Author => "ProfessorX,Ghasty制作,nnt汉化";

	public override string Description => "添加一些有用的命令.";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public AdminExtension(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		updater = new System.Timers.Timer(1000.0);
		updater.Elapsed += OnUpdate;
		updater.Start();
		ReadConfig();
		GeneralHooks.ReloadEvent += new ReloadEventD(OnReload);
		ServerApi.Hooks.GameInitialize.Register((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)OnInitialize);
	}

	private void OnReload(ReloadEventArgs args)
	{
		if (ReadConfig())
		{
			args.Player.PluginSuccessMessage("已重载配置文件.");
		}
		else
		{
			args.Player.PluginErrorMessage("重载配置文件出错.点击日志查看更多.");
		}
	}

	private void OnUpdate(object sender, ElapsedEventArgs e)
	{
		TSPlayer[] players = TShock.Players;
		TSPlayer[] array = players;
		TSPlayer[] array2 = array;
		foreach (TSPlayer val in array2)
		{
			if (val != null)
			{
				if (autoKilledPlayers.Contains(val))
				{
					val.DamagePlayer(15000);
				}
				if (autoHealedPlayers.Contains(val))
				{
					val.Heal(600);
				}
				if (PvPForced[val.Index] && !val.TPlayer.hostile)
				{
					val.TPlayer.hostile = true;
					NetMessage.SendData(30, -1, -1, (NetworkText)null, val.Index, 0f, 0f, 0f, 0, 0, 0);
				}
				if (ghostMode[val.Index])
				{
					((Entity)val.TPlayer).position.X = 0f;
					((Entity)val.TPlayer).position.Y = 0f;
					val.TPlayer.team = 0;
					NetMessage.SendData(13, -1, -1, (NetworkText)null, val.Index, 0f, 0f, 0f, 0, 0, 0);
					NetMessage.SendData(45, -1, -1, (NetworkText)null, val.Index, 0f, 0f, 0f, 0, 0, 0);
				}
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		if (disposing)
		{
			GeneralHooks.ReloadEvent -= new ReloadEventD(OnReload);
			ServerApi.Hooks.GameInitialize.Deregister((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)OnInitialize);
			updater.Stop();
			updater.Dispose();
		}
		this.Dispose(disposing);
	}

	public void OnInitialize(EventArgs args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		Commands.ChatCommands.Add(new Command("ae.killall", new CommandDelegate(KillAll), new string[2] { "killall", "击杀全部" }));
		Commands.ChatCommands.Add(new Command("ae.autokill", new CommandDelegate(AutoKill), new string[2] { "autokill", "自动击杀" }));
		Commands.ChatCommands.Add(new Command("ae.healall", new CommandDelegate(HealAll), new string[2] { "healall", "恢复全部" }));
		Commands.ChatCommands.Add(new Command("ae.autoheal", new CommandDelegate(AutoHeal), new string[2] { "autoheal", "自动恢复" }));
		Commands.ChatCommands.Add(new Command("ae.forcepvp", new CommandDelegate(ForcePvP), new string[2] { "forcepvp", "强制对战" }));
		Commands.ChatCommands.Add(new Command("ae.ghost", new CommandDelegate(Ghost), new string[2] { "ghost", "隐身上下线" }));
		Commands.ChatCommands.Add(new Command("ae.butcher.npc", new CommandDelegate(ButcherNPC), new string[2] { "butchernpc", "清除NPC" }));
		Commands.ChatCommands.Add(new Command("ae.butcher.friendly", new CommandDelegate(ButcherFriendly), new string[3] { "butcherfriendly", "清除友好NPC", "butcherf" }));
		Commands.ChatCommands.Add(new Command("ae.findpermission", new CommandDelegate(FindPermission), new string[2] { "findpermission", "查权限" }));
	}

	private void FindPermission(CommandArgs args)
	{
		if (args.Parameters.Count == 0)
		{
			args.Player.PluginErrorMessage("错误的命令,正确的命令: /查权限 <命令名>");
			return;
		}
		string text = args.Parameters[0].ToLowerInvariant();
		if (text.StartsWith(((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier))
		{
			text = text.Substring(1);
		}
		int num = 0;
		while (true)
		{
			if (num < Commands.ChatCommands.Count)
			{
				if (Commands.ChatCommands[num].Names.Contains(text))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		if (Commands.ChatCommands[num].Permissions.Count == 1)
		{
			args.Player.PluginInfoMessage(string.Format("所查询命令{2}{1}的权限是{0}", Commands.ChatCommands[num].Permissions[0], Commands.ChatCommands[num].Name, ((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier));
			return;
		}
		args.Player.PluginInfoMessage("所查询命令" + ((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier + Commands.ChatCommands[num].Name + "的权限是:");
		for (int i = 0; i < Commands.ChatCommands[num].Permissions.Count; i++)
		{
			args.Player.PluginInfoMessage(Commands.ChatCommands[num].Permissions[i]);
		}
	}

	public void KillAll(CommandArgs args)
	{
		TSPlayer[] players = TShock.Players;
		TSPlayer[] array = players;
		TSPlayer[] array2 = array;
		foreach (TSPlayer val in array2)
		{
			if (val != null && !val.Group.HasPermission("ae.killall.bypass") && val != args.Player && TShock.Utils.GetActivePlayerCount() > 1)
			{
				val.DamagePlayer(15000);
			}
		}
		args.Player.PluginSuccessMessage("你已击杀所有人!");
		if (!args.Silent)
		{
			TSPlayer.All.PluginErrorMessage(args.Player.Name + "使用指令击杀了所有人.");
		}
	}

	public void AutoKill(CommandArgs args)
	{
		if (args.Parameters.Count != 1)
		{
			args.Player.PluginErrorMessage("错误的命令,正确的命令: " + ((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier + "自动击杀 <用户名/列表>");
			return;
		}
		if (args.Parameters[0].ToLower() == "list" || args.Parameters[0].ToLower() == "列表")
		{
			IEnumerable<string> enumerable = from p in autoKilledPlayers
				orderby p.Name
				select p.Name;
			if (enumerable.Count() == 0)
			{
				args.Player.PluginErrorMessage("自动击杀列表里没有用户.");
			}
			else
			{
				args.Player.PluginSuccessMessage(string.Format("正在自动击杀的用户: {0}", string.Join(", ", enumerable)));
			}
			return;
		}
		List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
		if (list.Count == 0)
		{
			args.Player.PluginErrorMessage("无效的用户.");
		}
		else if (list.Count > 1)
		{
			args.Player.SendMultipleMatchError((IEnumerable<object>)list.Select((TSPlayer p) => p.Name));
		}
		else if (list[0].Group.HasPermission("ae.autokill.bypass"))
		{
			args.Player.PluginErrorMessage("你不能将该用户添加进自动击杀列表.");
		}
		else if (!autoKilledPlayers.Contains(list[0]))
		{
			autoKilledPlayers.Add(list[0]);
			args.Player.PluginSuccessMessage("已将" + list[0].Name + "添加进自动击杀列表,再次输入可以移除效果.");
			list[0].PluginInfoMessage("你已被自动击杀.");
		}
		else
		{
			autoKilledPlayers.Remove(list[0]);
			args.Player.PluginSuccessMessage("已将" + list[0].Name + "移除出自动击杀列表.");
			list[0].PluginInfoMessage("已移除自动击杀效果.");
		}
	}

	public void HealAll(CommandArgs args)
	{
		TSPlayer[] players = TShock.Players;
		foreach (TSPlayer obj in players)
		{
			if (obj != null)
			{
				obj.Heal(600);
			}
		}
		args.Player.PluginSuccessMessage("你已治疗所有人!");
		if (!args.Silent)
		{
			TSPlayer.All.PluginInfoMessage(args.Player.Name + "治疗了所有人.");
		}
	}

	public void AutoHeal(CommandArgs args)
	{
		if (args.Parameters.Count != 1)
		{
			args.Player.PluginErrorMessage("错误的命令,正确的命令: " + ((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier + "自动恢复 <用户名/列表>");
			return;
		}
		if (args.Parameters[0].ToLower() == "list" || args.Parameters[0].ToLower() == "列表")
		{
			IEnumerable<string> enumerable = from p in autoHealedPlayers
				orderby p.Name
				select p.Name;
			if (enumerable.Count() == 0)
			{
				args.Player.PluginErrorMessage("自动恢复列表里没有用户.");
			}
			else
			{
				args.Player.PluginSuccessMessage(string.Format("正在自动恢复的用户: {0}", string.Join(", ", enumerable)));
			}
			return;
		}
		List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
		if (list.Count == 0)
		{
			args.Player.PluginErrorMessage("无效的用户.");
		}
		else if (list.Count > 1)
		{
			args.Player.SendMultipleMatchError((IEnumerable<object>)list.Select((TSPlayer p) => p.Name));
		}
		else if (!autoHealedPlayers.Contains(list[0]))
		{
			autoHealedPlayers.Add(list[0]);
			args.Player.PluginSuccessMessage("已将" + list[0].Name + "添加进自动恢复列表,再次输入可以移除效果.");
			list[0].PluginInfoMessage("你现在正处于自动恢复状态.");
		}
		else
		{
			autoHealedPlayers.Remove(list[0]);
			args.Player.PluginSuccessMessage("已将" + list[0].Name + "移除出自动恢复列表.");
			list[0].PluginInfoMessage("已移除自动恢复效果.");
		}
	}

	public void ForcePvP(CommandArgs args)
	{
		if (args.Parameters.Count != 1)
		{
			args.Player.PluginErrorMessage("错误的命令,正确的命令: " + ((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier + "强制对战 <用户名>");
			return;
		}
		List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
		if (list.Count == 0)
		{
			args.Player.PluginErrorMessage("无效的用户.");
		}
		else if (list.Count > 1)
		{
			args.Player.SendMultipleMatchError((IEnumerable<object>)list.Select((TSPlayer p) => p.Name));
		}
		else if (list[0].Group.HasPermission("ae.forcepvp.bypass"))
		{
			args.Player.PluginErrorMessage("你不能强制开启该玩家的对战状态.");
		}
		else
		{
			PvPForced[list[0].Index] = !PvPForced[list[0].Index];
			args.Player.PluginSuccessMessage(string.Format("{0}的对战状态已{1}.", list[0].Name, PvPForced[list[0].Index] ? "强制开启并锁定" : "不再锁定"));
			list[0].PluginInfoMessage(string.Format("你的对战状态已{0}.", PvPForced[list[0].Index] ? "强制开启并锁定" : "不再锁定"));
		}
	}

	public void Ghost(CommandArgs args)
	{
		if (args.Parameters.Count == 1)
		{
			List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (list.Count == 0)
			{
				args.Player.PluginErrorMessage("无效的用户.");
				return;
			}
			if (list.Count > 1)
			{
				args.Player.SendMultipleMatchError((IEnumerable<object>)list.Select((TSPlayer p) => p.Name));
				return;
			}
			ghostMode[list[0].Index] = !ghostMode[list[0].Index];
			args.Player.PluginSuccessMessage(string.Format("{0}了{1}的隐身上下线模式.", ghostMode[list[0].Index] ? "已开启" : "已关闭", list[0].Name));
			list[0].PluginInfoMessage(string.Format("{0} {1}了隐身上下线模式.", args.Player.Name, ghostMode[list[0].Index] ? "已为你开启" : "已关闭"));
			TSPlayer.All.SendInfoMessage(string.Format("{0} {1}.", list[0].Name, ghostMode[list[0].Index] ? "离开游戏" : "加入游戏"));
		}
		else if (args.Parameters.Count == 0)
		{
			if (!args.Player.RealPlayer)
			{
				args.Player.SendErrorMessage("你必须在游戏中使用.");
				return;
			}
			ghostMode[args.Player.Index] = !ghostMode[args.Player.Index];
			args.Player.PluginSuccessMessage(string.Format("{0}隐身上下线模式.", ghostMode[args.Player.Index] ? "已开启" : "已关闭"));
			TSPlayer.All.SendInfoMessage(string.Format("{0} {1}.", args.Player.Name, ghostMode[args.Player.Index] ? "离开游戏" : "加入游戏"));
		}
	}

	public void ButcherNPC(CommandArgs args)
	{
		if (args.Parameters.Count != 1)
		{
			args.Player.PluginErrorMessage("错误的命令,正确的命令: " + ((ConfigFile<TShockSettings>)(object)TShock.Config).Settings.CommandSpecifier + "清除NPC <npc ID/名字>");
			return;
		}
		int num = 0;
		List<NPC> nPCByIdOrName = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
		if (nPCByIdOrName.Count == 0)
		{
			args.Player.PluginErrorMessage("无效的NPC.");
			return;
		}
		for (int i = 0; i < Main.npc.Length; i++)
		{
			if (Main.npc[i].friendly && Main.npc[i].type == nPCByIdOrName[0].type)
			{
				num++;
				TSPlayer.Server.StrikeNPC(i, 9999, 0f, 0);
			}
		}
		TSPlayer.All.PluginSuccessMessage(string.Format("{0}清除了{1}个{2}{3}.", args.Player.Name, num.ToString(), nPCByIdOrName[0].GivenOrTypeName, (num > 1) ? "" : ""));
	}

	private void CreateConfig()
	{
		string path = Path.Combine(TShock.SavePath, "AdminExtensionX使用说明.json");
		try
		{
			using FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
			using (StreamWriter streamWriter = new StreamWriter(fileStream))
			{
				config = new Config();
				string value = JsonConvert.SerializeObject((object)config, (Formatting)1);
				streamWriter.Write(value);
			}
			fileStream.Close();
		}
		catch (Exception ex)
		{
			TShock.Log.ConsoleError(ex.Message);
		}
	}

	private bool ReadConfig()
	{
		string path = Path.Combine(TShock.SavePath, "AdminExtensionX使用说明.json");
		try
		{
			if (File.Exists(path))
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader streamReader = new StreamReader(fileStream))
					{
						string text = streamReader.ReadToEnd();
						config = JsonConvert.DeserializeObject<Config>(text);
					}
					fileStream.Close();
				}
				return true;
			}
			TShock.Log.ConsoleError("[AdminExtensionX] 使用说明未找到.正在重新生成...");
			CreateConfig();
			return false;
		}
		catch (Exception ex)
		{
			TShock.Log.ConsoleError(ex.Message);
		}
		return false;
	}

	public void ButcherFriendly(CommandArgs args)
	{
		int num = 0;
		for (int i = 0; i < Main.npc.Length; i++)
		{
			if (((Entity)Main.npc[i]).active && Main.npc[i].townNPC)
			{
				num++;
				TSPlayer.Server.StrikeNPC(i, 9999, 0f, 0);
			}
		}
		TSPlayer.All.PluginInfoMessage(string.Format("{0}已清除{1}个友好NPC{2}.", args.Player.Name, num.ToString(), (num > 1) ? "" : ""));
	}
}
