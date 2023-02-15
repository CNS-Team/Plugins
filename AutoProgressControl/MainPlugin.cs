using System.Reflection;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Rests;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using ProgressCommonSystem;

namespace AutoProgressControl;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
	private static readonly string TimerPath = Path.Combine(TShock.SavePath, "AutoProgressControl", "time_zero.txt");

	private DateTime TimeZero;

	private Config cfg;

	public override string Name => "Auto Progress Control";

	public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

	public override string Author => "棱镜";

	public override string Description => "自动控制进度";

	public MainPlugin(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		Directory.CreateDirectory(Path.Combine(TShock.SavePath, "AutoProgressControl"));
		if (!File.Exists(Config.SavePath))
		{
			File.WriteAllText(Config.SavePath, JsonConvert.SerializeObject((object)(cfg = new Config()), (Formatting)1));
		}
		else
		{
			cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config.SavePath));
		}
		if (!File.Exists(TimerPath) || string.IsNullOrWhiteSpace(File.ReadAllText(TimerPath)))
		{
			string timerPath = TimerPath;
			DateTime dateTime = (TimeZero = DateTime.Now);
			File.WriteAllText(timerPath, dateTime.ToString());
		}
		else
		{
			TimeZero = DateTime.Parse(File.ReadAllText(TimerPath));
		}
		ServerApi.Hooks.GameUpdate.Register((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)OnGameUpdate);
		ProgressCommonPlugin.OnEventProgressAdvanced += OnEventProgressAdvanced;
		ProgressCommonPlugin.OnBossProgressAdvanced += OnBossProgressAdvanced;
		((Rest)TShock.RestApi).Register("/autoProgress", new RestCommandD(query));
	}

	private object query(RestRequestArgs args)
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_017d: Expected O, but got Unknown
		List<AutoBoss> list = new List<AutoBoss>();
		List<AutoEvent> list2 = new List<AutoEvent>();
		foreach (KeyValuePair<BossProgress, int> item in cfg.BossProgressTime)
		{
			TimeSpan timeSpan = DateTime.Now - TimeZero;
			if (timeSpan.TotalMinutes <= (double)item.Value)
			{
				int unlockTime = item.Value - (int)timeSpan.TotalMinutes;
				list.Add(new AutoBoss
				{
					BossName = BossOrEvent.Boss[item.Key],
					UnlockTime = unlockTime
				});
			}
		}
		foreach (KeyValuePair<EventProgress, int> item2 in cfg.EventProgressTime)
		{
			TimeSpan timeSpan2 = DateTime.Now - TimeZero;
			if (timeSpan2.TotalMinutes <= (double)item2.Value)
			{
				int unlockTime2 = item2.Value - (int)timeSpan2.TotalMinutes;
				list2.Add(new AutoEvent
				{
					EventType = BossOrEvent.Event[item2.Key],
					UnlockTime = unlockTime2
				});
			}
		}
		RestObject val = new RestObject();
		((Dictionary<string, object>)val).Add("Boss", (object)list);
		((Dictionary<string, object>)val).Add("Event", (object)list2);
		return (object)val;
	}

	private void OnGameUpdate(EventArgs args)
	{
	}

	private void OnBossProgressAdvanced(BossProgressAdvancedEventArgs args)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		int num = cfg.BossProgressTime[args.BossProgress];
		TimeSpan timeSpan = DateTime.Now - TimeZero;
		if (timeSpan.TotalMinutes <= (double)num)
		{
			args.Handled = true;
			TSPlayer all = TSPlayer.All;
			all.SendMessage($"Boss {BossOrEvent.Boss[args.BossProgress]}暂未解锁，距离解锁还剩{num - (int)timeSpan.TotalMinutes}分钟", Color.Aqua);
		}
	}

	private void OnEventProgressAdvanced(EventProgressAdvancedEventArgs args)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		int num = cfg.EventProgressTime[args.EventProgress];
		TimeSpan timeSpan = DateTime.Now - TimeZero;
		if (timeSpan.TotalMinutes <= (double)num)
		{
			args.Handled = true;
			TSPlayer all = TSPlayer.All;
			all.SendMessage($"事件 {BossOrEvent.Event[args.EventProgress]}暂未解锁，距离解锁还剩{num - (int)timeSpan.TotalMinutes}分钟", Color.Aqua);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.GameUpdate.Deregister((TerrariaPlugin)(object)this, (HookHandler<EventArgs>)OnGameUpdate);
			ProgressCommonPlugin.OnEventProgressAdvanced -= OnEventProgressAdvanced;
			ProgressCommonPlugin.OnBossProgressAdvanced -= OnBossProgressAdvanced;
		}
		this.Dispose(disposing);
	}
}
