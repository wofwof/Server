using System;
using System.Collections.Generic;
using System.Text;
using Bindings;

namespace Client
{
    class ClientHandleNetworkData
    {
        private static Dictionary<int, Action<byte[]>> Packets;
        public static void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Network Packagtes");
            Packets = new Dictionary<int, Action<byte[]>>
            {
                {(int)ServerPackets.SConnectionOk,HandleConnectionOK }
            };

        }

        public static void HandleNetworkInformation(byte[] data)
        {
            int packetNum;
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            packetNum = buffer.ReadInteger();
            buffer.Dispose();
            if (Packets.TryGetValue(packetNum, out Action<byte[]> action))
            {
                action.Invoke(data);
            }
        }

        private static void HandleConnectionOK(byte[] data)
        {
            PacketBuffer packet = new PacketBuffer();
            packet.WriteBytes(data);
            packet.ReadInteger();
            string msg = packet.ReadString();
            packet.Dispose();

            Console.Write($"Server message to licnet on connection ok: {msg}");
        }
    }
}
