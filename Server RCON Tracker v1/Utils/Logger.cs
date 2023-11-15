using Server_RCON_Tracker_v1.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Utils
{
    enum LogLevel
    {
        NONE = 0,
        INFO,
        WARNING,
        ERROR,
        DEBUG,
        COLOR_GREEN,
        COLOR_RED,
        COLOR_WHITE,
        COLOR_YELLOW
    }

    static internal class Logger
    {
        public static void LogLine(string msg, LogLevel level = LogLevel.NONE, bool scaryImage = false)
        {
            string? prefix = string.Empty;
            switch (level)
            {
                case LogLevel.COLOR_GREEN: Console.ForegroundColor = ConsoleColor.Green; break;
                case LogLevel.COLOR_RED: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogLevel.COLOR_WHITE: Console.ForegroundColor = ConsoleColor.White; break;
                case LogLevel.COLOR_YELLOW: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case LogLevel.INFO: prefix = "[INFO]"; break;
                case LogLevel.WARNING: prefix = "[WARNING]"; Console.ForegroundColor = ConsoleColor.Yellow; break;
                case LogLevel.ERROR:
                    {
                        prefix = "* [ERROR]";
                        if (scaryImage)
                        {
                            LogLine("Something went wrong", LogLevel.ERROR);
                            Console.WriteLine("""
                                    ⠛⠛⣿⣿⣿⣿⣿⡷⢶⣦⣶⣶⣤⣤⣤⣀   
                                  ⠀ ⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀ 
                                  ⠀ ⠀⠀⠉⠉⠉⠙⠻⣿⣿⠿⠿⠛⠛⠛⠻⣿⣿⣇ 
                                  ⠀ ⠀⢤⣀⣀⣀⠀⠀⢸⣷⡄⠀⣁⣀⣤⣴⣿⣿⣿⣆
                                  ⠀ ⠀⠀⠀⠹⠏⠀⠀⠀⣿⣧⠀⠹⣿⣿⣿⣿⣿⡿⣿
                                  ⠀ ⠀⠀⠀⠀⠀⠀⠀⠀⠛⠿⠇⢀⣼⣿⣿⠛⢯⡿⡟
                                  ⠀ ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠦⠴⢿⢿⣿⡿⠷⠀⣿ 
                                  ⠀ ⠀⠀⠀⠀⠀⠀⠙⣷⣶⣶⣤⣤⣤⣤⣤⣶⣦⠃ 
                                  ⠀ ⠀⠀⠀⠀⠀⠀⢐⣿⣾⣿⣿⣿⣿⣿⣿⣿⣿⠀ 
                                  ⠀ ⠀⠀⠀⠀⠀⠀⠈⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇  
                                  ⠀ ⠀⠀⠀⠀⠀⠀⠀⠀⠙⠻⢿⣿⣿⣿⣿⠟⠁  
                                """);
                        }
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    }
                case LogLevel.DEBUG:
                    {
                        if (!Debugger.DebugMode)
                        {
                            return;
                        }
                        prefix = "[DEBUG]";
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    }
            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"{prefix} {msg}"); Console.ResetColor();
            Console.WriteLine();
        }
    }
}
