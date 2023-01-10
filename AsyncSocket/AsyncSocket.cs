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
            var ipendPoint = @base.Client.RemoteEndPoint as IPEndPoint;
            this.address = new TcpAddress(ipendPoint.Address, ipendPoint.Port);
            this._connection = @base;
            this.networkStream = @base.GetStream();
            this.recvThread = new Thread(this.RecvThread);
            this.sendThread = new Thread(this.SendThread);
            this.recvThread.Start();
            this.sendThread.Start();
            this.connected = true;
        }

        private void SendThread()
        {
            try
            {
                for (; ; )
                {
                    var sendData = this.sendQueue.Take();
                    if (sendData.cancel)
                    {
                        return;
                    }

                    {
                        var data = sendData;
                        this.networkStream.BeginWrite(sendData.data, sendData.offset, sendData.size, result =>
                        {
                            this.networkStream.EndWrite(result);
                            data.callback(data.state);
                        }, null);
                    }
                }
            }
            catch
            {
                this.Close();
            }
        }

        private void RecvThread()
        {
            try
            {
                for (; ; )
                {
                    var recvData = this.recvQueue.Take();
                    if (recvData.cancel)
                    {
                        return;
                    }

                    {
                        var data = recvData;

                        this.networkStream.BeginRead(recvData.data, recvData.offset, recvData.size, result => data.callback(data.state, this.networkStream.EndRead(result)), null);
                    }
                }
            }
            catch
            {
                this.Close();
            }
        }

        public void Close()
        {
            try
            {
                this.connected = false;

                this.sendQueue.Add(new SendData { cancel = true });
                this.recvQueue.Add(new RecvData { cancel = true });
                this.networkStream.Dispose();

                this._connection.Close();
            }
            catch
            {
            }
        }

        private bool connected;

        public bool IsConnected()
        {
            return this.connected;
        }

        public void Connect(RemoteAddress address)
        {
            throw new NotImplementedException();
        }

        public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
        {
            this.sendQueue.Add(new SendData
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
            this.recvQueue.Add(new RecvData
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
            return this._connection.Connected && this._connection.GetStream().DataAvailable;
        }

        public void SendQueuedPackets()
        {
        }

        private void ListenLoop()
        {
            while (this._isListening && !Netplay.Disconnect)
            {
                try
                {
                    ISocket socket = new AsyncSocket(this._listener.AcceptTcpClient());
                    Console.WriteLine(Language.GetTextValue("Net.ClientConnecting", socket.GetRemoteAddress()));
                    this._listenerCallback(socket);
                }
                catch (Exception)
                {
                }
            }
            this._listener.Stop();
        }

        public bool StartListening(SocketConnectionAccepted callback)
        {
            var any = IPAddress.Any;
            var flag = Program.LaunchParameters.TryGetValue("-ip", out var text) && !IPAddress.TryParse(text, out any);
            if (flag)
            {
                any = IPAddress.Any;
            }
            this._isListening = true;
            this._listenerCallback = callback;
            var flag2 = this._listener == null;
            if (flag2)
            {
                this._listener = new TcpListener(any, Netplay.ListenPort);
            }
            try
            {
                this._listener.Start();
            }
            catch (Exception)
            {
                return false;
            }
            new Thread(this.ListenLoop)
            {
                IsBackground = true,
                Name = "TCP Listen Thread"
            }.Start();
            return true;
        }

        public void StopListening()
        {
            this._isListening = false;
        }

        public RemoteAddress GetRemoteAddress()
        {
            return this.address;
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