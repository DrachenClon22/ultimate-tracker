using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Server.Entities
{
    internal class ServerInfo
    {
        public IPAddress? Address { get; set; }
        public ushort Port { get; set; }

        public ServerInfo(IPAddress? address, ushort port)
        {
            Address = address;
            Port = port;
        }
    }
}
