using System.Net.Sockets;
using System.Text;

namespace SyncPaintBoard.Transport
{
    public class StateObject
    {
        public Socket WorkSocket = null;
        public const int BufferSize = 5;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder Sb = new StringBuilder();
    }
}
