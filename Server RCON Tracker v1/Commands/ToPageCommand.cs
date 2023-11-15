using Server_RCON_Tracker_v1.Commands.Interfaces;
using Server_RCON_Tracker_v1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Commands
{
    internal class ToPageCommand : ICommand
    {
        private Func<Page> _targetPage;

        public string[] Name { get; set; }
        public object[]? Args { get; set; }
        public Func<bool, string> Description { get; set; } =
            (_) => $"Unknown command";

        public ToPageCommand(string[] name, Func<Page> targetPage)
        {
            Name = name;
            _targetPage = targetPage;

            Description = 
                (_) => $"Go to {_targetPage?.Invoke().PageName ?? "unknown"} page";
        }

        public void Execute(object[]? args)
        {
            Page.PageStack.Push(_targetPage.Invoke() ?? Page.HomePage);
        }
    }
}
