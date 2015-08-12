using System.Net;
using System.Windows.Input;
using SyncPaintBoard.Transport;

namespace SyncPaintBoard
{
    public class ClientMainViewModel : MainViewModel
    {
        private bool _isConnected;
        private ClientTransport _transport;
        private Replicator.Replicator _replicator;

        public ICommand ConnectToServerCommand
        {
            get
            {
                return new ActionCommand(ConnectToServer);
            }
        }

        public ICommand DisconnectFromServerCommand
        {
            get
            {
                return new ActionCommand(DisconnectFromServer);
            }
        }

        public void ConnectToServer()
        {
            if (!string.IsNullOrEmpty(this["Port"]) || !string.IsNullOrEmpty(this["IpAddress"]))
                return;

            var port = int.Parse(Port);
            var endPoint = new IPEndPoint(IPAddress.Parse(IpAddress), port);
            _transport = new ClientTransport(endPoint);
            _replicator = new Replicator.Replicator(Objects, _transport);
            _transport.ConnectionFailuer += s=> ConnectionError = s;
            _transport.Connected += () => { IsConnected = true; ConnectionError = string.Empty; };
            Objects.Clear();
            _transport.Connect();
        }

        public void DisconnectFromServer()
        {
            _transport.Disconnect();
            IsConnected = false;
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (value.Equals(_isConnected)) return;
                _isConnected = value;
                OnPropertyChanged();
            }
        }
    }
}
