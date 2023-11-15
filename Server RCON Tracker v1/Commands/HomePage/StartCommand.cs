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
    internal class StartCommand : ICommand
    {
        public string[] Name { get; set; } = new string[] { "start" };
        public object[]? Args { get; set; }
        public Func<bool, string> Description { get; set; } = 
            (_) => "Starts the server";

        public void Execute(object[]? args)
        {
            Logger.LogLine("RCON Connection Info:", LogLevel.INFO);
            Logger.LogLine($"IP Address: {(ServerHandler.GetServerInfo()?.Address?.ToString() ?? "Unidentified")}", LogLevel.INFO);
            Logger.LogLine($"Port: {(ServerHandler.GetServerInfo()?.Port)}", LogLevel.INFO);
            ProcessHandler.StartServerProcess();
        }
    }
}
