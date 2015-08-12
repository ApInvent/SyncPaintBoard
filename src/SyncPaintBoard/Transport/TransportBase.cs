using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace SyncPaintBoard.Transport
{
    public abstract class TransportBase
    {
        public event Action<ReceiveData> Receive;

        protected TransportBase()
        {
            Receive += rd => { };
        }

        protected void ReceiveCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var sender = state.WorkSocket;

            if (!IsConnected(sender))
                return;

            var bytesRead = sender.EndReceive(ar);
            state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
            if (state.Sb.ToString().IndexOf("<EOF>", StringComparison.Ordinal) > -1)
            {
                var match = Regex.Split(state.Sb.ToString(), @"<EOF>");
                OnReceive(sender, match[0]);
                state = new StateObject { WorkSocket = sender };
                state.Sb.Append(match[1]);
            }

            try
            {
                sender.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }
            catch (SocketException)
            {
            }
        }

        public abstract void Send(string data);

        protected void Send(Socket socket, string data)
        {
            var byteData = Encoding.ASCII.GetBytes(data + "<EOF>");
            socket.BeginSend(byteData, 0, byteData.Length, 0, ar => socket.EndSend(ar), null);
        }

        protected static bool IsConnected(Socket socket)
        {
            if (socket == null)
                return false;

            try
            {
                if (!socket.Poll(1, SelectMode.SelectRead) || socket.Available != 0)
                    return true;
                socket.Close();
                return false;
            }
            catch
            {
                socket.Close();
                return false;
            }
        }

        protected void OnReceive(Socket sender, string data)
        {
            Receive(new ReceiveData { Data = data, SenderId = GetSocketId(sender) });
        }

        protected virtual string GetSocketId(Socket socket)
        {
            return string.Empty;
        }

        protected void CloseSocket(Socket socket)
        {
            try
            {
                socket.Close();
                socket.Disconnect(false);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }
        }
    }
}
