using System.Net;
using System.Windows.Input;
using SyncPaintBoard.Transport;

namespace SyncPaintBoard
{
    public class ServerMainViewModel : MainViewModel
    {
        private Replicator.Replicator _replicator;
        private ServerTransport _transport;
        private bool _isStarted;
        private int _clientsCount;

        public ICommand StartServerCommand
        {
            get
            {
                return new ActionCommand(StartServer);
            }
        }

        public ICommand StopServerCommand
        {
            get
            {
                return new ActionCommand(StopServer);
            }
        }

        public void StartServer()
        {
            if (!string.IsNullOrEmpty(this["Port"]) || string.IsNullOrEmpty(IpAddress))
                return;
            var port = int.Parse(Port);

            Objects.Clear();

            var endPoint = new IPEndPoint(IPAddress.Parse(IpAddress), port);
            _transport = new ServerTransport(endPoint);
            _replicator = new Replicator.Replicator(Objects, _transport);
            _transport.Receive += _transport.Retranslate;
            _transport.ClientConnected += ClientConnected;
            _transport.ClientDisconnected += s => ClientsCount--;
            _transport.StartFailed += s => { ConnectionError = s; IsStarted = false; };
            _transport.Start();
            IsStarted = true;
        }

        private void ClientConnected(string clientId)
        {
            _replicator.SendState(str => _transport.Send(clientId, str));
            ClientsCount++;
        }

        public void StopServer()
        {
            _transport.Stop();
            ClientsCount = 0;
            IsStarted = false;
        }

        public int ClientsCount
        {
            get { return _clientsCount; }
            set
            {
                if (value == _clientsCount) return;
                _clientsCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsStarted
        {
            get
            {
                return _isStarted;
            }
            set
            {
                if (value.Equals(_isStarted)) return;
                _isStarted = value;
                OnPropertyChanged();
            }
        }
    }
}
