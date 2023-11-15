using Server_RCON_Tracker_v1.Commands.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Utils
{
    internal class Page
    {
        public static Page CurrentPage { get; set; } = null!;

        private static Dictionary<string, Page> _pages = new Dictionary<string, Page>();
        public static Page HomePage = null!;
        //public static Page ConfigPage = null!;
        //public static Page SubConfigPage = null!; 

        public static Stack<Page> PageStack { get; set; } = new Stack<Page>();
        public string PageName { get; set; } = string.Empty;
        public List<CommandTask> Tasks { get; set; } = null!;

        public Page(string name, List<CommandTask> tasks)
        {
            PageName = name;
            Tasks = tasks;
        }

        public static void SetPage(string pageName, Page page)
        {
            _pages[pageName] = page;
        }
        public static void AddPage(string pageName, Page pageToAdd)
        {
            if (_pages.ContainsKey(pageName))
            {
                Page result = _pages[pageName];
                result.Tasks.AddRange(pageToAdd.Tasks);
                _pages[pageName] = result;
            }
        }
        public static Page? GetPage(string pageName)
        {
            if (_pages.ContainsKey(pageName))
            {
                return _pages[pageName];
            }
            else
            {
                return null;
            }
        }
    }
}
