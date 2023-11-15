using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Utils
{
    internal class Loading
    {
        public static Task ShowLoading(CancellationToken token, string? info = null, bool silent = false)
        {
            return Task.Run(()=> 
            {
                string loading = "/|\\-";
                int i = 0;
                while (!token.IsCancellationRequested)
                {
                    if (!silent)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write($"* [");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($"{loading[i % loading.Length]}");
                        Console.ResetColor();
                        Console.Write($"]{(info is null ? "" : " " + info)}");
                        i++;
                    }
                    Task.Delay(100).Wait();
                }
                if (!silent)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
            });
        }
    }
}
