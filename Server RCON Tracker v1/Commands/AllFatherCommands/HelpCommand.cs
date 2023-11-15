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
    internal class HelpCommand : ICommand
    {
        public string[] Name { get; set; } = new string[] { "help" };
        public object[]? Args { get; set; }
        public Func<bool, string> Description { get; set; } = 
            (_) => "Shows all the commands";

        private Func<CommandTask[]> _tasks;
        public HelpCommand(Func<CommandTask[]> tasks)
        {
            _tasks = tasks;
        }

        public void Execute(object[]? args)
        {
            List<CommandTask> cmds = Page.CurrentPage.Tasks.ToList();
            cmds.AddRange(_tasks.Invoke());

            int x = 0;
            cmds.OrderBy(x => x.Name[0]).ToList().ForEach(ct =>
            {
                x++;
                string result = string.Empty;
                Array.ForEach(ct.Name, name =>
                {
                    result += $"{name} ";
                });
                result = result.Trim().Replace(" ", "|");
                Console.Write("* [");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(x);
                Console.ResetColor();
                Console.WriteLine($"]: {result} - {ct.Description.Invoke(Page.CurrentPage.Equals(Page.HomePage))}");
                //Logger.Log($"* [{x}]: {result} - {ct.Description.Invoke(Page.CurrentPage.Equals(Page.HomePage))}");
            }
            );
        }
    }
}
