using Server_RCON_Tracker_v1.Commands.Entities;
using Server_RCON_Tracker_v1.Commands.Interfaces;
using Server_RCON_Tracker_v1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Commands.AllFatherCommands
{
    internal class BackCommand : ICommand
    {
        public string[] Name { get; set; } = new string[] { "back", "exit" };
        public object[]? Args { get; set; }
        public Func<bool, string> Description { get; set; } =
            (homePage) => homePage ? "Exits the program" : "Back to previous page";

        public BackCommand() { }
        public BackCommand(object[] args)
        {
            Args = args;
        }

        public void Execute(object[]? args)
        {
            if (Page.PageStack.TryPop(out Page? result))
            {
                Page.CurrentPage = result;
            }
            else
            {
                Program.ExitStatus = true;
            }
        }
    }
}
