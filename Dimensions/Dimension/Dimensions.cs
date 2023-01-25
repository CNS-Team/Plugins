using Chireiden.TShock.Omni;
using LazyUtils;
using MaxMind;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PChrome.Core;
using StatusTxtMgr;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.Net;
using TerrariaApi.Server;
using TShockAPI;

namespace Dimension
{
    [ApiVersion(2, 1)]
    public class Dimensions : TerrariaPlugin
    {
        public GeoIPCountry Geo;

        public static Config Config;

        private readonly int timer;

        private readonly string path = Path.Combine(TShock.SavePath, "Dimensions.json");

        public static Dictionary<string, StringBuilder> status;

        private string online;

        public override string Author => "popstarfreas";

        public override string Description => "Adds more Dimensions to Terraria Travel";

        public override string Name => "Dimensions";

        public override Version Version => new Version(1, 5, 0);

        public Dimensions(Main game)
            : base(game)
        {
            base.Order = 1;
        }

        public static string[] pings { get; set; } = new string[255];
        public override void Initialize()
        {
            //IL_0199: Unknown result type (might be due to invalid IL or missing references)
            //IL_01a6: Expected O, but got Unknown
            ServerApi.Hooks.NetGetData.Register(this, this.GetData);
            var filename = "Dimensions-GeoIP.dat";
            if (!File.Exists(this.path))
            {
                Config.WriteTemplates(this.path);
            }
            Config = Config.Read(this.path);
            if (Config.EnableGeoIP && File.Exists(filename))
            {
                this.Geo = new GeoIPCountry(filename);
            }
            TShockAPI.Commands.ChatCommands.Add(new Command("", this.server, "server"));
            TShockAPI.Commands.ChatCommands.Add(new Command("", this.listPlayers, "list"));
            TShockAPI.Commands.ChatCommands.Add(new Command("", this.advtp, "advtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("", this.advwarp, "advwarp"));
            OTAPI.Hooks.MessageBuffer.GetData += PingClass.Hook_Ping_GetData;
            ServerApi.Hooks.GameInitialize.Register(this, this.OnGameInvitatize);
            ServerApi.Hooks.ServerLeave.Register(this, this.OnServerLeave);
            new Thread((ThreadStart) delegate
            {
                while (true)
                {
                    Task.WaitAll(TShock.Players.Select(async i =>
                    {
                        try
                        {
                            if (i?.Active == true)
                            {
                                pings[i.Index] = await PingClass.PingPlayer(i);
                            }
                            else
                            {
                                await Task.Delay(3000);
                            }
                        }
                        catch (Exception ex)
                        {
                            TShockAPI.TShock.Log.ConsoleError($"PingException {ex}");
                        }
                    }).ToArray());
                }
            }).Start();
            new Thread((ThreadStart) delegate
            {
                while (true)
                {
                    var stringBuilder2 = new StringBuilder();
                    stringBuilder2.AppendLine(Utils.GetOnline());
                    stringBuilder2.AppendLine($"[c/66FF94:当][c/71FF66:前][c/ABFF66:世][c/E4FF66:界]:[c/FFAE66:{Main.worldName}]");
                    stringBuilder2.AppendLine($"[c/FFD766:当][c/FFBE66:前][c/FFA466:世][c/FF8B66:界][c/FF7166:在][c/FF6674:线]: [c/C8FF66:{TShock.Players.ToList().FindAll((TSPlayer pl) => pl?.Active ?? false).Count}]/[c/FF7866:{TShock.Config.Settings.MaxSlots}]");
                    this.online = stringBuilder2.ToString();
                    Thread.Sleep(1000);
                }
            }).Start();
            StatusTxtMgr.StatusTxtMgr.Hooks.StatusTextUpdate.Register(delegate (StatusTextUpdateEventArgs args)
            {
                var tsplayer = args.tsplayer;
                var statusTextBuilder = args.statusTextBuilder;
                statusTextBuilder.Append(this.online);
                statusTextBuilder.AppendLine($"[c/66FFF6:主][c/66FFDC:城][c/66FFC3:等][c/66FFA9:级]:{tsplayer.Group.Prefix}{tsplayer.Group.Name}{tsplayer.Group.Suffix}");
                if (!string.IsNullOrWhiteSpace(pings[tsplayer.Index]))
                {
                    statusTextBuilder.AppendLine($"[c/FFC566:P][c/FF8B66:i][c/FF667A:n][c/FF66B3:g][c/FF66ED:(][c/D866FF:延][c/9F66FF:迟][c/6667FF:)]:{pings[tsplayer.Index]}");
                }
                if (tsplayer.Account != null)
                {
                    using var val = Db.Get<Money>(tsplayer, null!);
                    statusTextBuilder.Append($"[c/FFE766:经][c/FFC266:济]:[c/66FFFC:{val.Single().money}]");
                }
                foreach (var value in status.Values)
                {
                    statusTextBuilder.AppendLine(value.ToString());
                }
            }, 60uL);
        }

        private void OnServerLeave(LeaveEventArgs args)
        {
            Console.WriteLine(args.Who);
        }

        private void OnNetSendData(SendDataEventArgs args)
        {
        }

        private void OnGameInvitatize(EventArgs args)
        {
        }

        private void advwarp(CommandArgs args)
        {
            TShockAPI.Commands.HandleCommand(TSPlayer.FindByNameOrID(args.Parameters[0])[0], "/warp " + args.Parameters[1]);
        }

        private void advtp(CommandArgs args)
        {
            var player = TSPlayer.FindByNameOrID(args.Parameters[0])[0];
            var num = int.Parse(args.Parameters[1]);
            TShockAPI.Commands.HandleCommand(player, string.Format("/tppos {0} {1}", arg1: int.Parse(args.Parameters[2]), arg0: num));
        }

        private void listPlayers(CommandArgs args)
        {
            try
            {
                var array = new byte[5242880];
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(Config.HostIP), Config.HostRestPort));
                var count = socket.Receive(array);
                socket.Close();
                var jObject = (JObject) JsonConvert.DeserializeObject(Encoding.UTF8.GetString(array, 0, count))!;
                args.Player.SendInfoMessage("当前在线：" + jObject["players"]);
            }
            catch
            {
            }
        }

        private void server(CommandArgs args)
        {
            var args2 = args;
            if (args2.Parameters.Count == 1)
            {
                var memoryStream = new MemoryStream();
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write((short) 0);
                    binaryWriter.Write((byte) 67);
                    binaryWriter.Write((short) 2);
                    binaryWriter.Write(args2.Parameters[0]);
                    binaryWriter.BaseStream.Position = 0L;
                    binaryWriter.Write((short) memoryStream.ToArray().Length);
                }
                Netplay.Clients[args2.Player.Index].Socket.AsyncSend(memoryStream.ToArray(), 0, memoryStream.ToArray().Length, delegate
                {
                });
                new Task(delegate
                {
                    var rests = Dimension.Config.Read(this.path).Rests;
                    foreach (var rest in rests)
                    {
                        if (rest.Name == args2.Parameters[0])
                        {
                            new HttpClient().GetAsync($"http://{rest.IP}:{rest.Port}/RestLogin/Add?Player={args2.Player.Name}");
                            break;
                        }
                    }
                }).Start();
            }
            else
            {
                TShockAPI.Commands.HandleCommand(TSPlayer.FindByNameOrID(args2.Parameters[1])[0], "/server " + args2.Parameters[0]);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                OTAPI.Hooks.MessageBuffer.GetData -= PingClass.Hook_Ping_GetData;
            }
            base.Dispose(disposing);
        }

        private void Reload(CommandArgs e)
        {
            var file = Path.Combine(TShock.SavePath, "Dimensions.json");
            if (!File.Exists(file))
            {
                Config.WriteTemplates(file);
            }
            Config = Config.Read(file);
            e.Player.SendSuccessMessage("Reloaded Dimensions config.");
        }

        private void GetData(GetDataEventArgs args)
        {
            if (args.MsgID != PacketTypes.Placeholder)
            {
                return;
            }
            using var input = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
            using var binaryReader = new BinaryReader(input);
            if (binaryReader.ReadInt16() == 0)
            {
                var array = binaryReader.ReadString().Split(':');
                var tcpAddress = (TcpAddress) Netplay.Clients[args.Msg.whoAmI].Socket.GetRemoteAddress();
                tcpAddress.Address = IPAddress.Parse(array[0]);
                var tSPlayer = TShock.Players[args.Msg.whoAmI];
                tSPlayer.GetType().GetField("CacheIP", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(tSPlayer, tcpAddress.Address.ToString());
                TShock.Log.ConsoleInfo($"remote address of client #{args.Msg.whoAmI} set to {tcpAddress.GetFriendlyName()}");
            }
        }

        static Dimensions()
        {
            Config = new Config();
            status = new Dictionary<string, StringBuilder>();
        }
    }
}
