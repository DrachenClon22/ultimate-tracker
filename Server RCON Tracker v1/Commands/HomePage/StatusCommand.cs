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
    internal class StatusCommand : ICommand
    {
        public string[] Name { get; set; } = new string[] { "status" };
        public object[]? Args { get; set; }
        public Func<bool, string> Description { get; set; } =
            (_) => "Get server status";

        public StatusCommand() { }
        public StatusCommand(object[] args)
        {
            Args = args;
        }

        public void Execute(object[]? args)
        {
            bool status = ServerHandler.GetUpstate().Result;
            Logger.LogLine($"Server online status: {(status ? "ONLINE" : "OFFLINE")}", status ? LogLevel.COLOR_GREEN : LogLevel.COLOR_RED);
            if (!status)
            {
                Logger.LogLine($"Note: OFFLINE means server didn't respond to RCON query.", LogLevel.INFO);
            }
        }
    }
}
