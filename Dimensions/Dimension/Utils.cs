using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria;
using TShockAPI;

namespace Dimension;

internal class Utils
{
    public static string GetOnline()
    {
        try
        {
            byte[] array = new byte[5242880];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(Dimensions.Config.HostIP), Dimensions.Config.HostRestPort));
            int count = socket.Receive(array);
            socket.Close();
            JObject jObject = (JObject)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(array, 0, count))!;
            return "当前全服在线:" + jObject["playercount"]?.ToString() + "/" + jObject["maxplayers"];
        }
        catch
        {
            return "";
        }
    }

    public static void InitNPC(int index)
    {
        for (int i = 0; i < Main.npc.Count(); i++)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write((short)0);
                binaryWriter.Write((byte)23);
                binaryWriter.Write((short)i);
                binaryWriter.WriteVector2(Main.npc[i].position);
                binaryWriter.WriteVector2(Main.npc[i].velocity);
                binaryWriter.Write((short)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((byte)0);
                binaryWriter.Write((short)0);
                binaryWriter.BaseStream.Position = 0L;
                binaryWriter.Write((short)memoryStream.ToArray().Length);
            }
            TShock.Players[index].SendRawData(memoryStream.ToArray());
        }
    }
}
