using Server_RCON_Tracker_v1.Commands.Entities;
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
    internal class KillCommand : ICommand
    {
        public string[] Name { get; set; } = new string[] { "kill" };
        public object[]? Args { get; set; }
        public Func<bool, string> Description { get; set; } =
            (_) => "Kills the server immediately. (Not recommended)";

        public KillCommand() { }
        public KillCommand(object[] args)
        {
            Args = args;
        }

        public void Execute(object[]? args)
        {
            ProcessHandler.KillServer();
        }
    }
}
