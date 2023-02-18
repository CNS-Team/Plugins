using LazyUtils;
using Terraria;
using TerrariaApi.Server;
using TrProtocol;

namespace PacketMonitor;

[ApiVersion(2, 1)]
public class PluginContainer : LazyPlugin
{
    private readonly StreamWriter _clientWriter = new(
        new FileStream("packets_client.log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
    private readonly PacketSerializer _clientSerializer = new(true, $"Terraria{Main.curRelease}");

    private readonly StreamWriter _serverWriter = new(
        new FileStream("packets_server.log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
    private readonly PacketSerializer _serverSerializer = new(false, $"Terraria{Main.curRelease}");

    public PluginContainer(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        ServerApi.Hooks.NetGetData.Register(this, this.NetGetData, -100);
        ServerApi.Hooks.NetSendBytes.Register(this, this.NetSendBytes, -100);

        this._serverSerializer.ErrorLogger = this._serverWriter;
        this._clientSerializer.ErrorLogger = this._clientWriter;
    }

    private void NetSendBytes(SendBytesEventArgs args)
    {
        lock (this._clientWriter)
        {
            try
            {
                using var br = new BinaryReader(new MemoryStream(args.Buffer, args.Offset, args.Count));
                var packet = this._clientSerializer.Deserialize(br);
                //_clientWriter.WriteLine($"[Player #{args.Socket.Id}::SendData] {packet}");
            }
            catch (Exception)
            {
                this._clientWriter.WriteLine(
                    $"[Player #{args.Socket.Id}::SendData] " +
                    $"{string.Join(' ', Enumerable.Range(args.Offset, args.Count).Select(i => $"{args.Buffer[i]:x2}"))}");
            }
        }
    }

    private void NetGetData(GetDataEventArgs args)
    {
        lock (this._serverWriter)
        {
            try
            {
                using var br = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index - 3, args.Length + 2));
                var packet = this._serverSerializer.Deserialize(br);
                //_serverWriter.WriteLine($"[Player #{args.Msg.whoAmI}::GetData] {packet}");
            }
            catch (Exception)
            {
                this._serverWriter.WriteLine(
                    $"[Player #{args.Msg.whoAmI}::GetData] " +
                    $"{string.Join(' ', Enumerable.Range(args.Index - 3, args.Length + 2).Select(i => $"{args.Msg.readBuffer[i]:x2}"))}");
            }
        }
    }
}