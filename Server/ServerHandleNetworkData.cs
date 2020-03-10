using Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerHandleNetworkData
    {
        private static Dictionary<int, Action<int, byte[]>> Packets;
        public static void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Network Packagtes");
            Packets = new Dictionary<int, Action<int, byte[]>>
            {
                {(int)ClientPackets.CThankYou,HandleThankYou }
            };

        }

        public static void HandleNetworkInformation(int index, byte[] data)
        {
            int packetNum;
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            packetNum = buffer.ReadInteger();
            buffer.Dispose();
            if (Packets.TryGetValue(packetNum, out Action<int, byte[]> action))
            {
                action.Invoke(index, data);
            }
        }

        private static void HandleThankYou(int index, byte[] data)
        {
            PacketBuffer packet = new PacketBuffer();
            packet.WriteBytes(data);
            packet.ReadInteger();
            string msg = packet.ReadString();
            packet.Dispose();

            Console.Write($"Client message to server: {msg}");
        }
    }
}
