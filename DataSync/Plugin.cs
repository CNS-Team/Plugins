using MySql.Data.MySqlClient;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace DataSync;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "DataSync";
    internal static Dictionary<int, List<ProgressType>> _related = new Dictionary<int, List<ProgressType>>();
    internal static Dictionary<ProgressType, Func<bool?, bool>> _flagaccessors = new Dictionary<ProgressType, Func<bool?, bool>>();
    public Plugin(Main game) : base(game)
    {
        foreach (var variant in typeof(ProgressType).GetFields())
        {
            var matches = variant.GetCustomAttributes<MatchAttribute>()!;
            foreach (var match in matches)
            {
                if (match.NPCID.Length > 0)
                {
                    foreach (var id in match.NPCID)
                    {
                        if (!_related.ContainsKey(id))
                        {
                            _related[id] = new List<ProgressType>();
                        }
                        _related[id].Add((ProgressType) variant.GetValue(null)!);
                    }
                }
            }
            var mappings = variant.GetCustomAttributes<MappingAttribute>()!;
            foreach (var mapping in mappings)
            {
                if (mapping.Key is not null)
                {
                    var targetField = typeof(NPC).GetField(mapping.Key!, BindingFlags.Public | BindingFlags.Static)
                        ?? typeof(Main).GetField(mapping.Key!, BindingFlags.Public | BindingFlags.Static)
                        ?? typeof(Terraria.GameContent.Events.DD2Event).GetField(mapping.Key!, BindingFlags.Public | BindingFlags.Static)!;

                    _flagaccessors.Add((ProgressType) variant.GetValue(null)!, (newvalue) =>
                    {
                        if (newvalue is not null && targetField.FieldType == typeof(bool))
                        {
                            targetField.SetValue(null, newvalue);
                        }
                        return targetField.GetValue(null) == mapping.Value;
                    });
                }
            }
        }
    }

    public static event Action? OnProgressChanged;
    private void Remove(CommandArgs args)
    {
        TShock.DB.Query("TRUNCATE TABLE synctable;");
    }

    public static Dictionary<ProgressType, bool> LocalProgress { get; set; } = new Dictionary<ProgressType, bool>();
    public static Dictionary<ProgressType, bool> SyncedProgress { get; set; } = new Dictionary<ProgressType, bool>();

    private static void EnsureTable()
    {
        var db = TShock.DB;
        var sqlTable = new SqlTable("synctable", new SqlColumn[]
        {
            new SqlColumn("key", MySqlDbType.VarChar)
            {
                Primary = true,
                Length = 256
            },
            new SqlColumn("value", MySqlDbType.VarChar)
            {
                Length = 256
            }
        });

        var sqlTableCreator = new SqlTableCreator(db, (db.GetSqlType() == SqlType.Sqlite)
            ? new SqliteQueryCreator()
            : new MysqlQueryCreator());
        sqlTableCreator.EnsureTableStructure(sqlTable);

        using var result = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", nameof(ProgressType.KingSlime));
        if (!result.Read())
        {
            foreach (var t in Config._default)
            {
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1)", t.Key, false);
            }
        }
    }

    private int _frameCount = 0;
    public override void Initialize()
    {
        Config.LoadConfig();
        EnsureTable();

        ServerApi.Hooks.NpcKilled.Register(this, this.NpcKilled);
        ServerApi.Hooks.GameUpdate.Register(this, args =>
        {
            this._frameCount++;
            if (this._frameCount % 300 == 0)
            {
                LoadProgress();
            }
        });
        ServerApi.Hooks.GamePostInitialize.Register(this, args => LoadProgress());
        Commands.ChatCommands.Add(new Command("DataSync", this.Re, "reload"));
        Commands.ChatCommands.Add(new Command("DataSync", this.Remove, "重置进度同步"));
    }

    public static void UpdateProgress(ProgressType type, bool value, bool force = false)
    {
        if (!force && LocalProgress.TryGetValue(type, out var oldValue) && oldValue == value)
        {
            return;
        }

        LocalProgress[type] = value;

        if (Config.ShouldSyncProgress.TryGetValue(type, out var progress) && progress)
        {
            if (!SyncedProgress.TryGetValue(type, out var syncedValue) || syncedValue != value)
            {
                TSPlayer.Server.SendInfoMessage($"[DataSync]上传进度 {Config.GetProgressName(type)} {value}");
                TShock.DB.Query("UPDATE synctable SET value = @1 WHERE `key` = @0", type, value);
            }
        }

        OnProgressChanged?.Invoke();
    }

    private void NpcKilled(NpcKilledEventArgs args)
    {
        if (_related.TryGetValue(args.npc.netID, out var types))
        {
            foreach (var type in types)
            {
                if (!_flagaccessors.TryGetValue(type, out var accessor) || accessor(null))
                {
                    UpdateProgress(type, true);
                }
            }
        }
    }

    private void Re(CommandArgs args)
    {
        try
        {
            Config.LoadConfig();
            EnsureTable();
            LoadProgress();
            //args.Player.SendErrorMessage($"[QwRPG.Shop]重载成功！");
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage($"[DataSync]配置文件读取错误");
        }
    }

    public static void LoadProgress()
    {
        if (!Monitor.TryEnter(kg))
        {
            return;
        }

        foreach (var (pg, ac) in _flagaccessors)
        {
            if (ac(null) && (!LocalProgress.TryGetValue(pg, out var value) || !value))
            {
                UpdateProgress(pg, true);
            }
        }

        using (var reader = TShock.DB.QueryReader("SELECT * FROM synctable"))
        {
            while (reader.Read())
            {
                var key = reader.Get<string>("key");
                var value = reader.Get<bool>("value");
                if (Config.GetProgressType(key) is ProgressType type)
                {
                    SyncedProgress[type] = value;
                    if (Config.ShouldSyncProgress.TryGetValue(type, out var progress) && progress)
                    {
                        LocalProgress[type] = value;
                        if (_flagaccessors.TryGetValue(type, out var accessor))
                        {
                            accessor(value);
                        }
                    }
                }
            }
        }

        Monitor.Exit(kg);
    }

    public static object kg = new object();
}