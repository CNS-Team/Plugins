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
		byte[] array = new byte[4];
		while (!disposed)
		{
			try
			{
				Thread.Sleep(0);
				stream.Read(array, 0, 4);
				switch (BitConverter.ToInt32(array, 0))
				{
				case 0:
					Name = br.ReadString();
					break;
				case 1:
				{
					string arg = br.ReadString();
					uint arg2 = br.ReadUInt32();
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
				Connect();
				break;
			}
			catch
			{
			}
		}
	}

	private void Heartbeat()
	{
		while (!disposed)
		{
			try
			{
				Thread.Sleep(1000);
				lock (bw)
				{
					bw.Write(2);
				}
			}
			catch
			{
				ConnectTillSuc();
			}
		}
	}

	public void Connect()
	{
		client = new TcpClient();
		client.Connect(host, port);
		stream = client.GetStream();
		br = new BinaryReader(stream);
		bw = new BinaryWriter(stream);
		recvthread = new Thread(Listener);
		tickthread = new Thread(Heartbeat);
		recvthread.Start();
		tickthread.Start();
		Valid = true;
	}

	public GameServer(string host, ushort port)
	{
		this.host = host;
		this.port = port;
		Connect();
	}

	public void SetName(string name)
	{
		lock (bw)
		{
			bw.Write(0);
			bw.Write(name);
		}
	}

	public void SendMsg(string message, uint color = 16777215u)
	{
		if (string.IsNullOrEmpty(message))
		{
			return;
		}
		lock (bw)
		{
			bw.Write(1);
			bw.Write(message);
			bw.Write(color);
		}
	}

	public void Dispose()
	{
		try
		{
			disposed = true;
			client.Close();
			br.Dispose();
			bw.Dispose();
		}
		catch
		{
		}
		Valid = false;
	}
}
