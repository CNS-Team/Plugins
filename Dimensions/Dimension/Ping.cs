//Copy From @SGKoishi
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using TShockAPI;

namespace Chireiden.TShock.Omni;

public static class PingClass
{
    public static async Task<TimeSpan> Ping(TSPlayer player, CancellationToken token = default)
    {
        var result = TimeSpan.MaxValue;

        var inv = -1;
        for (var i = 0; i < Terraria.Main.item.Length; i++)
        {
            if (!Terraria.Main.item[i].active || Terraria.Main.item[i].playerIndexTheItemIsReservedFor == 255)
            {
                inv = i;
                break;
            }
        }
        if (inv == -1)
        {
            return result;
        }

        var start = DateTime.Now;
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(30)
        {
            SingleReader = true,
            SingleWriter = true
        });
        player.SetData("chireiden.data.pingchannel1", channel);
        Terraria.NetMessage.TrySendData((int) PacketTypes.RemoveItemOwner, -1, -1, null, inv);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var end = await channel.Reader.ReadAsync(token);
                if (end == inv)
                {
                    result = DateTime.Now - start;
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout
            }
        }
        player.SetData<Channel<int>?>("chireiden.data.pingchannel1", null);
        return result;
    }

    public static void Hook_Ping_GetData(object? sender, OTAPI.Hooks.MessageBuffer.GetDataEventArgs args)
    {
        if (args.PacketId != (byte) PacketTypes.ItemOwner)
        {
            return;
        }

        var owner = args.Instance.readBuffer[args.ReadOffset + 2];
        if (owner != byte.MaxValue)
        {
            return;
        }

        var whoami = args.Instance.whoAmI;
        var pingresponse = TShockAPI.TShock.Players[whoami]?.GetData<Channel<int>?>("chireiden.data.pingchannel1");
        if (pingresponse == null)
        {
            return;
        }

        var index = BitConverter.ToInt16(args.Instance.readBuffer.AsSpan(args.ReadOffset, 2));
        pingresponse.Writer.TryWrite(index);
    }

    public static async Task<string> PingPlayer(TSPlayer plr)
    {
        try
        {
            var player = plr;
            var result = await Ping(player, new System.Threading.CancellationTokenSource(3000).Token);
            return result.TotalMilliseconds switch
            {
                double ms when ms >= 200 => $"[c/FF0000:{ms:F1}ms]",
                double ms when ms > 80 && ms < 200 => $"[c/FFA500:{ms:F1}ms]",
                double ms when ms <= 80 => $"[c/00FF00:{ms:F1}ms]",
                _ => throw new SwitchExpressionException()
            };
        }
        catch (Exception e)
        {
            TShockAPI.TShock.Log.Error(e.ToString());
            return "[c/FF0000:不可用]";
        }
    }
}
