using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TeleportRequest;

[ApiVersion(2, 1)]
public class TeleportRequest : TerrariaPlugin
{
	private System.Timers.Timer Timer;

	private bool[] TPAllows = new bool[256];

	private bool[] TPacpt = new bool[256];

	private TPRequest[] TPRequests = new TPRequest[256];

	public override string Author => "原作者: MarioE, 修改者: Dr.Toxic";

	public static Config tpConfig { get; set; }

	internal static string tpConfigPath => Path.Combine(TShock.SavePath, "tpconfig.json");

	public override string Description => "传送前需要被传送者接受或拒绝请求";

	public override string Name => "传送请求（汉化版）";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public TeleportRequest(Main game)
		: base(game)
	{
		tpConfig = new Config();
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequests[i] = new TPRequest();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.GameInitialize.Deregister((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)OnInitialize);
			ServerApi.Hooks.ServerLeave.Deregister((TerrariaPlugin)(object)this, (HookHandler<LeaveEventArgs>)OnLeave);
			Timer.Dispose();
		}
	}

	public override void Initialize()
	{
		ServerApi.Hooks.GameInitialize.Register((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)OnInitialize);
		ServerApi.Hooks.ServerLeave.Register((TerrariaPlugin)(object)this, (HookHandler<LeaveEventArgs>)OnLeave);
	}

	private void OnElapsed(object sender, ElapsedEventArgs e)
	{
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout <= 0)
			{
				continue;
			}
			TSPlayer val = TShock.Players[tPRequest.dst];
			TSPlayer val2 = TShock.Players[i];
			tPRequest.timeout--;
			if (tPRequest.timeout == 0)
			{
				val2.SendErrorMessage("传送请求已超时.");
				val.SendInfoMessage("玩家[{0}]的传送请求已超时.", new object[1] { val2.Name });
				continue;
			}
			string text = string.Format("玩家[{{0}}]要求传送到你当前位置. ({0}接受tp ({0}atp) 或 {0}拒绝tp ({0}dtp))", Commands.Specifier);
			if (tPRequest.dir)
			{
				text = string.Format("你被请求传送到玩家[{{0}}]的当前位置. ({0}接受tp ({0}atp) 或 {0}拒绝tp ({0}dtp))", Commands.Specifier);
			}
			val.SendInfoMessage(text, new object[1] { val2.Name });
		}
	}

	private void OnInitialize(EventArgs e)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		Commands.ChatCommands.Add(new Command("tprequest.gettpr", new CommandDelegate(TPAccept), new string[2] { "接受tp", "atp" })
		{
			AllowServer = false,
			HelpText = "接受传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpauto", new CommandDelegate(TPAutoDeny), new string[2] { "自动拒绝tp", "autodeny" })
		{
			AllowServer = false,
			HelpText = "自动拒绝所有人的传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpauto", new CommandDelegate(TPAutoAccept), new string[2] { "自动接受tp", "autoaccept" })
		{
			AllowServer = false,
			HelpText = "自动接受所有人的传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.gettpr", new CommandDelegate(TPDeny), new string[2] { "拒绝tp", "dtp" })
		{
			AllowServer = false,
			HelpText = "拒绝传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpathere", new CommandDelegate(TPAHere), new string[1] { "tpahere" })
		{
			AllowServer = false,
			HelpText = "发出把指定玩家传送到你当前位置的请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpat", new CommandDelegate(TPA), new string[1] { "tpa" })
		{
			AllowServer = false,
			HelpText = "发出传送到指定玩家当前位置的请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.reload", new CommandDelegate(ReloadTPR), new string[2] { "重读传送配置", "tprreload" })
		{
			AllowServer = false,
			HelpText = "输入 /重读传送配置 会重新读取重读传送配置表."
		});
		SetupConfig();
		Timer = new (tpConfig.间隔秒数 * 1000);
		Timer.Elapsed += OnElapsed;
		Timer.Start();
	}

	private void OnLeave(LeaveEventArgs e)
	{
		TPAllows[e.Who] = false;
		TPacpt[e.Who] = false;
		TPRequests[e.Who].timeout = 0;
	}

	private void TPA(CommandArgs e)
	{
		if (e.Parameters.Count == 0)
		{
			e.Player.SendErrorMessage("格式错误! 正确格式为: {0}tpa <玩家>", new object[1] { Commands.Specifier });
			return;
		}
		string text = string.Join(" ", e.Parameters.ToArray());
		List<TSPlayer> list = TSPlayer.FindByNameOrID(text);
		if (list.Count == 0)
		{
			e.Player.SendErrorMessage("找不到这位玩家!");
			return;
		}
		if (list.Count > 1)
		{
			e.Player.SendErrorMessage("匹对到多于一位玩家!");
			return;
		}
		if (((object)list[0]).Equals((object?)e.Player))
		{
			e.Player.SendErrorMessage("禁止向自己发送传送请求！");
			return;
		}
		if ((!list[0].TPAllow || TPAllows[list[0].Index]) && !e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			e.Player.SendErrorMessage("你无法传送到玩家[{0}].", new object[1] { list[0].Name });
			return;
		}
		if ((list[0].TPAllow && TPacpt[list[0].Index]) || e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			bool flag = false;
			TSPlayer val = (flag ? TShock.Players[list[0].Index] : e.Player);
			TSPlayer val2 = (flag ? e.Player : TShock.Players[list[0].Index]);
			if (val.Teleport(val2.X, val2.Y, (byte)1))
			{
				val.SendSuccessMessage("已经传送到玩家[{0}]的当前位置.", new object[1] { val2.Name });
				val2.SendSuccessMessage("玩家[{0}]已传送到你的当前位置.", new object[1] { val.Name });
			}
			return;
		}
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == list[0].Index)
			{
				e.Player.SendErrorMessage("玩家[{0}]已被其他玩家发出传送请求.", new object[1] { list[0].Name });
				return;
			}
		}
		TPRequests[e.Player.Index].dir = false;
		TPRequests[e.Player.Index].dst = (byte)list[0].Index;
		TPRequests[e.Player.Index].timeout = tpConfig.超时次数 + 1;
		e.Player.SendSuccessMessage("已成功向玩家[{0}]发出传送请求.", new object[1] { list[0].Name });
	}

	private void TPAccept(CommandArgs e)
	{
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == e.Player.Index)
			{
				TSPlayer val = (tPRequest.dir ? e.Player : TShock.Players[i]);
				TSPlayer val2 = (tPRequest.dir ? TShock.Players[i] : e.Player);
				if (val.Teleport(val2.X, val2.Y, (byte)1))
				{
					val.SendSuccessMessage("已经传送到玩家[{0}]的当前位置.", new object[1] { val2.Name });
					val2.SendSuccessMessage("玩家[{0}]已传送到你的当前位置.", new object[1] { val.Name });
				}
				tPRequest.timeout = 0;
				return;
			}
		}
		e.Player.SendErrorMessage("你暂时没有收到其他玩家的传送请求.");
	}

	private void TPAHere(CommandArgs e)
	{
		if (e.Parameters.Count == 0)
		{
			e.Player.SendErrorMessage("格式错误! 正确格式为: {0}tpahere <玩家>", new object[1] { Commands.Specifier });
			return;
		}
		string text = string.Join(" ", e.Parameters.ToArray());
		List<TSPlayer> list = TSPlayer.FindByNameOrID(text);
		if (list.Count == 0)
		{
			e.Player.SendErrorMessage("找不到这位玩家!");
			return;
		}
		if (list.Count > 1)
		{
			e.Player.SendErrorMessage("匹对到多于一位玩家!");
			return;
		}
		if ((!list[0].TPAllow || TPAllows[list[0].Index]) && !e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			e.Player.SendErrorMessage("你无法传送到玩家[{0}].", new object[1] { list[0].Name });
			return;
		}
		if ((list[0].TPAllow && TPacpt[list[0].Index]) || e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			bool flag = true;
			TSPlayer val = (flag ? TShock.Players[list[0].Index] : e.Player);
			TSPlayer val2 = (flag ? e.Player : TShock.Players[list[0].Index]);
			if (val.Teleport(val2.X, val2.Y, (byte)1))
			{
				val.SendSuccessMessage("已经传送到玩家[{0}]的当前位置.", new object[1] { val2.Name });
				val2.SendSuccessMessage("玩家[{0}]已传送到你的当前位置.", new object[1] { val.Name });
			}
			return;
		}
		if (!list[0].HasPermission("tprequest.tpat"))
		{
			e.Player.SendErrorMessage("玩家[{0}]没有被授予传送权限，发出传送请求无效！", new object[1] { list[0].Name });
			return;
		}
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == list[0].Index)
			{
				e.Player.SendErrorMessage("玩家[{0}]已被其他玩家发出传送请求.", new object[1] { list[0].Name });
				return;
			}
		}
		TPRequests[e.Player.Index].dir = true;
		TPRequests[e.Player.Index].dst = (byte)list[0].Index;
		TPRequests[e.Player.Index].timeout = tpConfig.超时次数 + 1;
		e.Player.SendSuccessMessage("已成功向玩家[{0}]发出传送请求.", new object[1] { list[0].Name });
	}

	private void TPAutoDeny(CommandArgs e)
	{
		if (TPacpt[e.Player.Index])
		{
			e.Player.SendErrorMessage("请先解除自动接受传送");
			return;
		}
		TPAllows[e.Player.Index] = !TPAllows[e.Player.Index];
		e.Player.SendInfoMessage("{0}自动拒绝传送请求.", new object[1] { TPAllows[e.Player.Index] ? "启用" : "解除" });
	}

	private void TPDeny(CommandArgs e)
	{
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == e.Player.Index)
			{
				e.Player.SendSuccessMessage("已拒绝玩家[{0}]的传送请求.", new object[1] { TShock.Players[i].Name });
				TShock.Players[i].SendErrorMessage("玩家[{0}]拒绝你的传送请求.", new object[1] { e.Player.Name });
				tPRequest.timeout = 0;
				return;
			}
		}
		e.Player.SendErrorMessage("你暂时没有收到其他玩家的传送请求.");
	}

	private void TPAutoAccept(CommandArgs e)
	{
		if (TPAllows[e.Player.Index])
		{
			e.Player.SendErrorMessage("请先解除自动拒绝传送");
			return;
		}
		TPacpt[e.Player.Index] = !TPacpt[e.Player.Index];
		e.Player.SendInfoMessage("{0}自动接受传送请求.", new object[1] { TPacpt[e.Player.Index] ? "启用" : "解除" });
	}

	private void SetupConfig()
	{
		try
		{
			if (File.Exists(tpConfigPath))
			{
				tpConfig = Config.Read(tpConfigPath);
			}
			tpConfig.Write(tpConfigPath);
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("TPR配置发生错误");
			Console.ForegroundColor = ConsoleColor.Gray;
			TShock.Log.ConsoleError("TPR配置出现异常");
			TShock.Log.ConsoleError(ex.ToString());
		}
	}

	private void ReloadTPR(CommandArgs e)
	{
		SetupConfig();
		TShock.Log.ConsoleInfo("TPR重载已完成");
		e.Player.SendSuccessMessage("TPR重载已完成");
	}
}
