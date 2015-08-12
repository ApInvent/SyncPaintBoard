using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SyncPaintBoard.Transport
{
    public class ClientTransport : TransportBase
    {
        public event Action Connected;
        public event Action<string> ConnectionFailuer;

        private Socket _socket;
        private readonly IPEndPoint _remoteEp;
        private readonly ManualResetEvent _waitConnectionEvent = new ManualResetEvent(false);

        public ClientTransport(IPEndPoint endPoint)
        {
            Connected += () => { };
            ConnectionFailuer += s => { };
            _remoteEp = endPoint;
        }

        public void Connect()
        {
            if (_socket != null && IsConnected(_socket))
                Disconnect();
            _waitConnectionEvent.Reset();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(_remoteEp, ConnectCallback, _socket);
        }

        public void Disconnect()
        {
            CloseSocket(_socket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;
            try
            {
                client.EndConnect(ar);
            }
            catch (SocketException e)
            {
                ConnectionFailuer(e.Message);
                return;
            }
            finally
            {
                _waitConnectionEvent.Set();
            }

            Connected();
            var state = new StateObject { WorkSocket = client };
            client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        }

        public override void Send(string data)
        {
            if (!_waitConnectionEvent.WaitOne(5000))
                return;
            if (IsConnected(_socket) && _socket.Connected)
                Send(_socket, data);
        }
    }
}
