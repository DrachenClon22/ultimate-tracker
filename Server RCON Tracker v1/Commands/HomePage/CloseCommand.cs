using Server_RCON_Tracker_v1.Commands.Interfaces;
using Server_RCON_Tracker_v1.Server;
using Server_RCON_Tracker_v1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Commands.HomePage
{
    internal class CloseCommand : ICommand
    {
        public string[] Name { get; set; } = new string[] { "close" };
        public object[]? Args { get; set; }
        public Func<bool, string> Description { get; set; } =
            (_) => "Gently tries to close the server. (Recommended)";

        public CloseCommand() { }
        public CloseCommand(object[] args)
        {
            Args = args;
        }

        public void Execute(object[]? args)
        {
            ProcessHandler.CloseServer();
        }
    }
}
