using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Commands.Interfaces
{
    internal interface ICommand
    {
        public string[] Name { get; set; }
        public object[]? Args { get; }
        public Func<bool, string> Description { get; set; }
        public delegate void Command(string[]? args = null);

        public void Execute(object[]? args);
    }
}
