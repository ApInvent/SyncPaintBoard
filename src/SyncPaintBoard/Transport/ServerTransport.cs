using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SyncPaintBoard.Transport
{
    public class ServerTransport : TransportBase
    {
        public event Action<string> ClientConnected;
        public event Action<string> ClientDisconnected;
        public event Action<string> StartFailed;

        private readonly IPEndPoint _endPoint;
        private readonly Dictionary<string, Socket> _clients = new Dictionary<string, Socket>();
        private Socket _socket;

        public ServerTransport(IPEndPoint endPoint)
        {
            ClientConnected += s => { };
            ClientDisconnected += s => { };
            StartFailed += s => { };
            _endPoint = endPoint;
        }

        public void Start()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Bind(_endPoint);
            }
            catch (SocketException e)
            {
                StartFailed(e.Message);
                return;
            }
            _socket.Listen(100);
            _socket.BeginAccept(AcceptCallback, _socket);
        }

        public void Stop()
        {
            _clients.Values.ToList().ForEach(CloseSocket);
            _clients.Clear();
            CloseSocket(_socket);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            if (!IsConnected(socket))
                return;
            socket.BeginAccept(AcceptCallback, socket);

            var clientSocket = socket.EndAccept(ar);
            AddClient(clientSocket);

            var state = new StateObject { WorkSocket = clientSocket };
            clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        }

        private void AddClient(Socket socket)
        {
            var id = Guid.NewGuid().ToString();
            _clients.Add(id, socket);
            ClientConnected(id);
        }

        public void Retranslate(ReceiveData receiveData)
        {
            foreach (var client in _clients.Keys.Where(k => k != receiveData.SenderId).ToList())
                Send(client, receiveData.Data);
        }

        public override void Send(string data)
        {
            foreach (var client in _clients.Keys.ToList())
                Send(client, data);
        }

        public void Send(string clientId, string data)
        {
            if (!_clients.ContainsKey(clientId))
                return;
            var socket = _clients[clientId];

            if (!IsConnected(socket))
            {
                _clients.Remove(clientId);
                ClientDisconnected(clientId);
                return;
            }

            Send(socket, data);
        }

        protected override string GetSocketId(Socket socket)
        {
            return _clients.FirstOrDefault(kvp => kvp.Value == socket).Key;
        }
    }
}
