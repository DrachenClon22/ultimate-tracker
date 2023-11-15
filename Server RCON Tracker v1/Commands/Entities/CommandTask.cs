using Server_RCON_Tracker_v1.Commands.Interfaces;
using Server_RCON_Tracker_v1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Commands.Entities
{
    internal class CommandTask
    {
        public string[] Name { get; set; }
        public Func<bool, string> Description { get; set; }

        private ICommand.Command? handler;
        private bool _log = false;

        public CommandTask(string[] name, string description, ICommand.Command cmd, bool log = false)
            : this(name, (a) => description, cmd, log)
        { }

        public CommandTask(string name, string description, ICommand.Command cmd, bool log = false)
            : this(name, (a) => description, cmd, log)
        { }

        public CommandTask(string name, Func<bool, string> description, ICommand.Command cmd, bool log = false)
            : this(new string[] { name }, description, cmd, log)
        { }

        public CommandTask(ICommand command, bool log = false)
        {
            Name = command.Name;
            Description = command.Description;
            handler = command.Execute;
            _log = log;
        }

        public CommandTask(string[] name, Func<bool, string> description, ICommand.Command cmd, bool log = false)
        {
            Name = name;
            Description = description;
            handler = cmd;
            _log = log;
        }

        public void Execute(string name, string[]? args = null)
        {
            if (_log)
            {
                Logger.LogLine($"Command \"{name.ToLower()}\" executed.", LogLevel.INFO);
            }
            handler?.Invoke(args);
        }
    }
}
