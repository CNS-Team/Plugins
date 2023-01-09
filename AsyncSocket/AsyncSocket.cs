using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Terraria;
using Terraria.Localization;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace AsyncSocket
{
	public class AsyncSocket : ISocket
	{
		public AsyncSocket()
		{
		}

		public AsyncSocket(TcpClient @base)
		{
			IPEndPoint ipendPoint = @base.Client.RemoteEndPoint as IPEndPoint;
			address = new TcpAddress(ipendPoint.Address, ipendPoint.Port);
			_connection = @base;
			networkStream = @base.GetStream();
			recvThread = new Thread(RecvThread);
			sendThread = new Thread(SendThread);
			recvThread.Start();
			sendThread.Start();
            connected = true;
        }

		private void SendThread()
		{
			try
			{
				for (;;)
				{
					SendData sendData = sendQueue.Take();
                    if (sendData.cancel) return;
                    {
                        var data = sendData;
                        networkStream.BeginWrite(sendData.data, sendData.offset, sendData.size, result =>
                        {
							networkStream.EndWrite(result);
                            data.callback(data.state);
                        }, null);
                    }
                }
			}
			catch
            {
                Close();
            }
		}
		
		private void RecvThread()
		{
			try
			{
				for (;;)
				{
					RecvData recvData = recvQueue.Take();
                    if (recvData.cancel) return;

                    {
                        var data = recvData;

                        networkStream.BeginRead(recvData.data, recvData.offset, recvData.size, result =>
                        {
                            data.callback(data.state, networkStream.EndRead(result));
                        }, null);
                    }
				}
			}
			catch
            {
                Close();
            }
		}
		
		public void Close()
        {
            try
            {
                connected = false;

                sendQueue.Add(new SendData { cancel = true });
                recvQueue.Add(new RecvData { cancel = true });
                networkStream.Dispose();

                _connection.Close();
			}
			catch
			{
			}
		}

        private bool connected;
        
		public bool IsConnected()
        {
            return connected;
        }
		
		public void Connect(RemoteAddress address)
		{
			throw new NotImplementedException();
		}
		
		public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
		{
			sendQueue.Add(new SendData
			{
				callback = callback,
				data = data,
				offset = offset,
				size = size,
				state = state
			});
		}
		
		public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null)
		{
			recvQueue.Add(new RecvData
			{
				callback = callback,
				data = data,
				offset = offset,
				size = size,
				state = state
			});
		}
		
		public bool IsDataAvailable()
		{
			return _connection.Connected && _connection.GetStream().DataAvailable;
		}
		
		public void SendQueuedPackets()
		{
		}
		
		private void ListenLoop()
		{
			while (_isListening && !Netplay.Disconnect)
			{
				try
				{
					ISocket socket = new AsyncSocket(_listener.AcceptTcpClient());
					Console.WriteLine(Language.GetTextValue("Net.ClientConnecting", socket.GetRemoteAddress()));
					_listenerCallback(socket);
				}
				catch (Exception)
				{
				}
			}
			_listener.Stop();
		}
		
		public bool StartListening(SocketConnectionAccepted callback)
		{
			IPAddress any = IPAddress.Any;
			string text;
			bool flag = Program.LaunchParameters.TryGetValue("-ip", out text) && !IPAddress.TryParse(text, out any);
			if (flag)
			{
				any = IPAddress.Any;
			}
			_isListening = true;
			_listenerCallback = callback;
			bool flag2 = _listener == null;
			if (flag2)
			{
				_listener = new TcpListener(any, Netplay.ListenPort);
			}
			try
			{
				_listener.Start();
			}
			catch (Exception)
			{
				return false;
			}
			new Thread(ListenLoop)
			{
				IsBackground = true,
				Name = "TCP Listen Thread"
			}.Start();
			return true;
		}
		
		public void StopListening()
		{
			_isListening = false;
		}
		
		public RemoteAddress GetRemoteAddress()
		{
			return address;
		}
		
		private readonly TcpClient _connection;
		
		private readonly RemoteAddress address;
		
		private readonly Stream networkStream;
		
		private readonly Thread recvThread;
		
		private readonly Thread sendThread;

		private readonly BlockingCollection<SendData> sendQueue = new BlockingCollection<SendData>();
		
		private readonly BlockingCollection<RecvData> recvQueue = new BlockingCollection<RecvData>();
		
		private bool _isListening;
		
		private TcpListener _listener;
		
		private SocketConnectionAccepted _listenerCallback;
		
		private struct SendData
		{
			public byte[] data;
			
			public int offset;
			
			public int size;
			
			public SocketSendCallback callback;
			
			public object state;
            public bool cancel;
        }
		
		private struct RecvData
		{
			public byte[] data;
			
			public int offset;
			
			public int size;
			
			public SocketReceiveCallback callback;
			
			public object state;
            public bool cancel;
        }
	}
}
