using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Terraria;
using TShockAPI;

namespace Dimension;

internal class Utils
{
    public static string GetOnline()
    {
        try
        {
            var array = new byte[5242880];
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(Dimensions.Config.HostIP), Dimensions.Config.HostRestPort));
            var count = socket.Receive(array);
            socket.Close();
            var jObject = (JObject) JsonConvert.DeserializeObject(Encoding.UTF8.GetString(array, 0, count))!;
            return $"[c/DAFF66:当][c/EDFF66:前][c/FFFE66:全][c/FFEC66:服][c/FFD966:在][c/FFC666:线:][c/C8FF66:{TShock.Players.ToList().FindAll((TSPlayer pl) => pl?.Active ?? false).Count}]/[c/FF7866:9999]";
        }
        catch
        {
            return "";
        }
    }

    public static void InitNPC(int index)
    {
        for (var i = 0; i < Main.npc.Count(); i++)
        {
            var memoryStream = new MemoryStream();
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write((short) 0);
                binaryWriter.Write((byte) 23);
                binaryWriter.Write((short) i);
                binaryWriter.WriteVector2(Main.npc[i].position);
                binaryWriter.WriteVector2(Main.npc[i].velocity);
                binaryWriter.Write((short) 0);
                binaryWriter.Write((byte) 0);
                binaryWriter.Write((byte) 0);
                binaryWriter.Write((short) 0);
                binaryWriter.BaseStream.Position = 0L;
                binaryWriter.Write((short) memoryStream.ToArray().Length);
            }
            TShock.Players[index].SendRawData(memoryStream.ToArray());
        }
    }
}