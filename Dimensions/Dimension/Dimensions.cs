using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chireiden.TShock.Omni;
using LazyUtils;
using MaxMind;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PChrome.Core;
using StatusTxtMgr;
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

        private int timer;

        private string path = Path.Combine(TShock.SavePath, "Dimensions.json");

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
            ServerApi.Hooks.NetGetData.Register(this, GetData);
            string filename = "Dimensions-GeoIP.dat";
            if (!File.Exists(path))
            {
                Config.WriteTemplates(path);
            }
            Config = Config.Read(path);
            if (Config.EnableGeoIP && File.Exists(filename))
            {
                Geo = new GeoIPCountry(filename);
            }
            TShockAPI.Commands.ChatCommands.Add(new Command("", server, "server"));
            TShockAPI.Commands.ChatCommands.Add(new Command("", listPlayers, "list"));
            TShockAPI.Commands.ChatCommands.Add(new Command("", advtp, "advtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("", advwarp, "advwarp"));
            OTAPI.Hooks.MessageBuffer.GetData += PingClass.Hook_Ping_GetData;
            ServerApi.Hooks.GameInitialize.Register(this, OnGameInvitatize);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            new Thread((ThreadStart)async delegate
            { 
                while (true)
                {
                    try
                    {
                        foreach (var i in TShock.Players)
                        {
                            if (i != null && i.Active)
                            {
                                pings[i.Index] = await PingClass.PingPlayer(i);
                            }
                        }
                    }
                    catch{}
                    Thread.Sleep(1000);
                }
            }).Start();
            new Thread((ThreadStart) delegate
            {
                while (true)
                {
                    StringBuilder stringBuilder2 = new StringBuilder();
                    stringBuilder2.AppendLine(Utils.GetOnline());
                    stringBuilder2.AppendLine("当前世界:" + Main.worldName);
                    stringBuilder2.AppendLine("当前世界在线:" + TShock.Players.ToList().FindAll((TSPlayer pl) => pl?.Active ?? false).Count + "/" + TShock.Config.Settings.MaxSlots);
                    online = stringBuilder2.ToString();
                    Thread.Sleep(1000);
                    
                }
            }).Start();
            StatusTxtMgr.StatusTxtMgr.Hooks.StatusTextUpdate.Register((StatusTextUpdateDelegate)delegate (StatusTextUpdateEventArgs args)
            {
                TSPlayer tsplayer = args.tsplayer;
                StringBuilder statusTextBuilder = args.statusTextBuilder; 
                statusTextBuilder.Append(online);
                statusTextBuilder.AppendLine("主城等级:" + tsplayer.Group.Prefix + tsplayer.Group.Name + tsplayer.Group.Suffix);
                statusTextBuilder.AppendLine("Ping(延迟):" + pings[tsplayer.Index]);
                if (tsplayer.Account != null)
                {
                    DisposableQuery<Money> val = Db.Get<Money>(tsplayer, null!);
                    try
                    {
                        StringBuilder stringBuilder = statusTextBuilder;
                        StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder);
                        handler.AppendLiteral("经济:");
                        handler.AppendFormatted(((IQueryable<Money>)val).Single().money);
                        stringBuilder.AppendLine(ref handler);
                    }
                    finally
                    {
                        ((IDisposable)val)?.Dispose();
                    }
                }
                foreach (StringBuilder value in status.Values)
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
            TSPlayer player = TSPlayer.FindByNameOrID(args.Parameters[0])[0];
            int num = int.Parse(args.Parameters[1]);
            TShockAPI.Commands.HandleCommand(player, string.Format("/tppos {0} {1}", arg1: int.Parse(args.Parameters[2]), arg0: num));
        }

        private void listPlayers(CommandArgs args)
        {
            try
            {
                byte[] array = new byte[5242880];
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(Config.HostIP), Config.HostRestPort));
                int count = socket.Receive(array);
                socket.Close();
                JObject jObject = (JObject)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(array, 0, count))!;
                args.Player.SendInfoMessage("当前在线：" + jObject["players"]);
            }
            catch
            {
            }
        }

        private void server(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                MemoryStream memoryStream = new MemoryStream();
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write((short)0);
                    binaryWriter.Write((byte)67);
                    binaryWriter.Write((short)2);
                    binaryWriter.Write(args.Parameters[0]);
                    binaryWriter.BaseStream.Position = 0L;
                    binaryWriter.Write((short)memoryStream.ToArray().Length);
                }
                Netplay.Clients[args.Player.Index].Socket.AsyncSend(memoryStream.ToArray(), 0, memoryStream.ToArray().Length, delegate
                {
                });
                new Task(delegate
                {
                    Rest[] rests = Config.Read(path).Rests;
                    Rest[] array = rests;
                    Rest[] array2 = array;
                    foreach (Rest rest in array2)
                    {
                        if (rest.Name == args.Parameters[0])
                        {
                            HttpClient httpClient = new HttpClient();
                            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
                            defaultInterpolatedStringHandler.AppendLiteral("http://");
                            defaultInterpolatedStringHandler.AppendFormatted(rest.IP);
                            defaultInterpolatedStringHandler.AppendLiteral(":");
                            defaultInterpolatedStringHandler.AppendFormatted(rest.Port);
                            defaultInterpolatedStringHandler.AppendLiteral("/RestLogin/Add?Player=");
                            defaultInterpolatedStringHandler.AppendFormatted(args.Player.Name);
                            httpClient.GetAsync(defaultInterpolatedStringHandler.ToStringAndClear());
                            break;
                        }
                    }
                }).Start();
            }
            else
            {
                TShockAPI.Commands.HandleCommand(TSPlayer.FindByNameOrID(args.Parameters[1])[0], "/server " + args.Parameters[0]);
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
            string file = Path.Combine(TShock.SavePath, "Dimensions.json");
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
            using MemoryStream input = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
            using BinaryReader binaryReader = new BinaryReader(input);
            if (binaryReader.ReadInt16() == 0)
            {
                string[] array = binaryReader.ReadString().Split(':');
                TcpAddress? tcpAddress = Netplay.Clients[args.Msg.whoAmI].Socket.GetRemoteAddress() as TcpAddress;
                tcpAddress!.Address = IPAddress.Parse(array[0]);
                TSPlayer tSPlayer = TShock.Players[args.Msg.whoAmI];
                Type type = tSPlayer.GetType();
                type.GetField("CacheIP", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(tSPlayer, tcpAddress.Address.ToString());
                ILog log = TShock.Log;
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
                defaultInterpolatedStringHandler.AppendLiteral("remote address of client #");
                defaultInterpolatedStringHandler.AppendFormatted(args.Msg.whoAmI);
                defaultInterpolatedStringHandler.AppendLiteral(" set to ");
                defaultInterpolatedStringHandler.AppendFormatted(tcpAddress.GetFriendlyName());
                log.ConsoleInfo(defaultInterpolatedStringHandler.ToStringAndClear());
            }
        }

        static Dimensions()
        {
            Config = new Config();
            status = new Dictionary<string, StringBuilder>();
        }
    }
}
