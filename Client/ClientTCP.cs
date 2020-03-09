using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Bindings;

namespace Client
{
    class ClientTCP
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private byte[] _asyncbuffer = new byte[1024];

        public static void ConnectToServer()
        {
            Console.WriteLine("Connecting to server...");
            _clientSocket.BeginConnect("127.0.0.1", 5555, new AsyncCallback(ConnectCallback), _clientSocket);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            _clientSocket.EndConnect(ar);
            while (true)
            {
                OnResieve();
            }
        }

        private static void OnResieve()
        {
            byte[] _sizeInfo = new byte[4];
            byte[] _recievedBuffer = new byte[1024];

            int totalRead, currentRead = 0;

            try
            {
                currentRead = totalRead = _clientSocket.Receive(_sizeInfo);
                if (totalRead <= 0)
                {
                    Console.WriteLine("You are not connected to the server");
                }
                else
                {
                    while(totalRead < _sizeInfo.Length && currentRead > 0)
                    {
                        currentRead = _clientSocket.Receive(_sizeInfo, totalRead, _sizeInfo.Length - totalRead, SocketFlags.None);
                        totalRead += currentRead;
                    }
                    int messagesize = 0;
                    messagesize |= _sizeInfo[0];
                    messagesize |= (_sizeInfo[1] << 8);
                    messagesize |= (_sizeInfo[2] << 16);
                    messagesize |= (_sizeInfo[3] << 24);

                    byte[] data = new byte[messagesize];
                    totalRead = 0;
                    currentRead = totalRead = _clientSocket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);

                    while (totalRead < messagesize && currentRead > 0)
                    {
                        currentRead = _clientSocket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);
                        totalRead += currentRead;
                    }
                    ClientHandleNetworkData.HandleNetworkInformation(data);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Socket Exception: {e.SocketErrorCode.ToString()}");
            }
        }

        public static void SendData(byte[] data)
        {
            _clientSocket.Send(data);
        }

        public static void ThankYouServer()
        {
            PacketBuffer packetBuffer = new PacketBuffer();
            packetBuffer.WriteInteger((int)ClientPackets.CThankYou);
            packetBuffer.WriteString("Thank you for letting me connect");
            SendData(packetBuffer.ToArray());
            packetBuffer.Dispose();
        }
    }
}
