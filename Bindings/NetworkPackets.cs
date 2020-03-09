using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    //get send from server to client
    //client has to listen to serverPackets
    public enum ServerPackets
    {
        SConnectionOk = 1,
    }

    //get send from client to server
    //server has to listen to clientPackets
    public enum ClientPackets
    {
        CThankYou = 1,
    }
}
