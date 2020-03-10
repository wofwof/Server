using System;
using System.Net.Sockets;
using System.Net;
using Bindings;

namespace Server
{
    class ServerTCP
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] _buffer = new byte[1024];

        public static Client[] _clients = new Client[Constants.MAX_PLAYERS];

        public static void SetupServer()
        {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                _clients[i] = new Client();
            }
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (_clients[i].socket == null)
                {
                    _clients[i].socket = socket;
                    _clients[i].index = i;
                    _clients[i].ip = socket.RemoteEndPoint.ToString();
                    _clients[i].StartClient();
                    Console.WriteLine($"Connection from {_clients[i].ip} recieved");
                    SendConnectionOK(i);
                    return;
                }
            }
        }

        public static void SendDataTo(int index, byte[] data)
        {
            byte[] sizeInfo = new byte[4];
            sizeInfo[0] = (byte)data.Length;
            sizeInfo[1] = (byte)(data.Length >> 8);
            sizeInfo[2] = (byte)(data.Length >> 16);
            sizeInfo[3] = (byte)(data.Length >> 24);

            _clients[index].socket.Send(sizeInfo);
            _clients[index].socket.Send(data);
        }

        public static void SendConnectionOK(int index)
        {
            PacketBuffer packetBuffer = new PacketBuffer();
            packetBuffer.WriteInteger((int)ServerPackets.SConnectionOk);
            string data = "Ping" + Environment.NewLine;
            packetBuffer.WriteString(data);
            SendDataTo(index, packetBuffer.ToArray());
            packetBuffer.Dispose();
        }
    }

    class Client
    {
        public int index;
        public string ip;
        public Socket socket;
        public bool closing = false;
        private byte[] _buffer = new byte[1024];

        public void StartClient()
        {
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
            closing = false;
        }

        private void RecieveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                int recieved = socket.EndReceive(ar);
                if (recieved <= 0)
                {
                    CloseClient(index);
                }
                else
                {
                    byte[] databuffer = new byte[recieved];
                    Array.Copy(_buffer, databuffer, recieved);
                    ServerHandleNetworkData.HandleNetworkInformation(index, databuffer);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Error while retrieving data from client: {e.SocketErrorCode.ToString()}");
                //CloseClient(index);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception is: { e.Message }");
            }
        }

        private void CloseClient(int index)
        {
            closing = true;
            Console.WriteLine($"Connection from {ip} has been terminated");
            //Player left Game;
            socket.Close();
        }
    }
}
