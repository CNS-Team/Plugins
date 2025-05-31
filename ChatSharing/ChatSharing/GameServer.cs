using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ChatSharing;

public sealed class GameServer : IDisposable
{
    private enum MsgType
    {
        SetServerName,
        WriteMessage,
        Heartbeat
    }

    private TcpClient client;

    private NetworkStream stream;

    private BinaryReader br;

    private BinaryWriter bw;

    private Thread recvthread;

    private Thread tickthread;

    private string host;

    private ushort port;

    private bool disposed;

    public string Name { get; set; }

    public bool Valid { get; private set; }

    public event Action<string, uint> OnMessage;

    private void Listener()
    {
        var array = new byte[4];
        while (!this.disposed)
        {
            try
            {
                Thread.Sleep(0);
                this.stream.Read(array, 0, 4);
                switch (BitConverter.ToInt32(array, 0))
                {
                    case 0:
                        this.Name = this.br.ReadString();
                        break;
                    case 1:
                    {
                        var arg = this.br.ReadString();
                        var arg2 = this.br.ReadUInt32();
                        this.OnMessage?.Invoke(arg, arg2);
                        break;
                    }
                }
            }
            catch
            {
            }
        }
    }

    private void ConnectTillSuc()
    {
        while (true)
        {
            try
            {
                this.Connect();
                break;
            }
            catch
            {
            }
        }
    }

    private void Heartbeat()
    {
        while (!this.disposed)
        {
            try
            {
                Thread.Sleep(1000);
                lock (this.bw)
                {
                    this.bw.Write(2);
                }
            }
            catch
            {
                this.ConnectTillSuc();
            }
        }
    }

    public void Connect()
    {
        this.client = new TcpClient();
        this.client.Connect(this.host, this.port);
        this.stream = this.client.GetStream();
        this.br = new BinaryReader(this.stream);
        this.bw = new BinaryWriter(this.stream);
        this.recvthread = new Thread(this.Listener);
        this.tickthread = new Thread(this.Heartbeat);
        this.recvthread.Start();
        this.tickthread.Start();
        this.Valid = true;
    }

    public GameServer(string host, ushort port)
    {
        this.host = host;
        this.port = port;
        this.Connect();
    }

    public void SetName(string name)
    {
        lock (this.bw)
        {
            this.bw.Write(0);
            this.bw.Write(name);
        }
    }

    public void SendMsg(string message, uint color = 16777215u)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        lock (this.bw)
        {
            this.bw.Write(1);
            this.bw.Write(message);
            this.bw.Write(color);
        }
    }

    public void Dispose()
    {
        try
        {
            this.disposed = true;
            this.client.Close();
            this.br.Dispose();
            this.bw.Dispose();
        }
        catch
        {
        }

        this.Valid = false;
    }
}