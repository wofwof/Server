using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientHandleNetworkData.InitializeNetworkPackages();
            ClientTCP.ConnectToServer();
            Console.ReadLine();
        }
    }
}
