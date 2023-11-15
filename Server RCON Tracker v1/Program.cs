using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using CoreRCON;
using Server_RCON_Tracker_v1.Commands;
using Server_RCON_Tracker_v1.Commands.AllFatherCommands;
using Server_RCON_Tracker_v1.Commands.Entities;
using Server_RCON_Tracker_v1.Commands.HomePage;
using Server_RCON_Tracker_v1.Commands.Interfaces;
using Server_RCON_Tracker_v1.JSON;
using Server_RCON_Tracker_v1.Server;
using Server_RCON_Tracker_v1.Utils;
using Server_RCON_Tracker_v1.Utils.Debug;

static class Program
{
    public static bool ExitStatus { get; set; } = false;
    public static string[]? Args { get; private set; }

    private static bool _lockConsole = false;
    private static bool _silentServerCheck = false;

    // These commands will be executed and shown on help section on any page
    public static List<CommandTask> AllfatherCommands = new List<CommandTask>();

    static async Task Main(string[] args)
    {
        Args = args;
        /*
        * This program needed for tracking current state of machine.
        * 
        * For this current instance, Minecraft server will be launched on my laptop, 
        * so if it starts to run on battery, means power went off, so server will save and shut down
        * till power restore. Server automatically starts after laptop starts and laptop starts after
        * power on again, via this console program settings can be changed and some other stuff tracked
        * 
        * Also server will shut off if laptop overheats or internet connection is off.
        */

        /*
        * No Russian language because any russian words become "?????? ?? ???" in VS2022 and
        * I dont know (care) how to fix it
        */
        Console.OutputEncoding = Encoding.Unicode;

        Server_RCON_Tracker_v1.Utils.Debug.Debugger.DebugMode = true;

        AllfatherCommands.AddRange( new CommandTask[]
        {
            new CommandTask(new BackCommand(), false),
            new CommandTask(new HelpCommand(AllfatherCommands.ToArray), false)
        });

        // Register all pages
        Page.HomePage = new Page("", new List<CommandTask>()
        {
            new CommandTask(new ToPageCommand(new[]{ "debug" }, ()=>Page.GetPage("debug")??Page.HomePage)),
            new CommandTask(new StartCommand()),
            new CommandTask(new StatusCommand()),
            new CommandTask(new KillCommand()),
            new CommandTask(new CloseCommand())
        });
        Page.SetPage("debug", new Page("debug", new List<CommandTask>()
        {
            new CommandTask("errortest", "Generates scary error", (taskArgs) => {
                Logger.LogLine("This error is generated and only for testing.", LogLevel.ERROR, true);
            }),
            new CommandTask(new[]{ "processes", "proc" }, "Shows all active processes. Can be used as \"proc [name]\"", (taskArgs) => {
                Process[] localByName = Process.GetProcessesByName(taskArgs?[0]);
                if (taskArgs?[0] is null)
                {
                    int initLength = localByName.Length;
                    localByName = localByName.ToList().Where(t=>t.MainWindowTitle.Length>0).ToArray();
                    Logger.LogLine($"Skipped {initLength-localByName.Length} processes without main window title");
                }
                int i = 0;
                Logger.LogLine($"Processes {(taskArgs?[0]is not null?"\""+taskArgs?[0]+"\" ":"")}found: {localByName.Length}");
                localByName.ToList().ForEach(p =>
                {
                    i++;
                    Console.Write("* [");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(i);
                    Console.ResetColor();
                    Console.WriteLine($"]: ({p.ProcessName}) - {p.MainWindowTitle}");
                });
            }),
            new CommandTask(new[]{ "debug" },"Enabling or disabling debug mode. Can be used as \"debug [true/false]\"", (taskArgs) =>
            {
                if (taskArgs?.Length > 0)
                {
                    try
                    {
                        Server_RCON_Tracker_v1.Utils.Debug.Debugger.DebugMode = bool.Parse(taskArgs[0].Replace("\"",""));
                    } catch
                    {
                        Logger.LogLine("Specify args. Use \"debug [true/false]\"");
                        return;
                    }
                }
                Logger.LogLine($"Debug Mode: {(Server_RCON_Tracker_v1.Utils.Debug.Debugger.DebugMode?"ENABLED":"DISABLED")}",
                        Server_RCON_Tracker_v1.Utils.Debug.Debugger.DebugMode?LogLevel.COLOR_GREEN:LogLevel.COLOR_RED);
            }),
            new CommandTask(new[]{ "silent","sil" },"Enabling or disabling silent server upstate check", (taskArgs) =>
            {
                if (taskArgs?.Length > 0)
                {
                    try
                    {
                        _silentServerCheck = bool.Parse(taskArgs[0].Replace("\"",""));
                    } catch
                    {
                        Logger.LogLine("Specify args. Use \"sil [true/false]\"");
                        return;
                    }
                }
                Logger.LogLine($"Silent Mode: {(_silentServerCheck?"ENABLED":"DISABLED")}",
                        _silentServerCheck?LogLevel.COLOR_GREEN:LogLevel.COLOR_RED);
            })
        }));

        await Console.Out.WriteLineAsync();

        // Cool intro image
        Logger.LogLine("""
            _   _ _ _   _                 _         _____               _             
            | | | | | |_(_)_ __ ___   __ _| |_ ___  |_   _| __ __ _  ___| | _____ _ __ 
            | | | | | __| | '_ ` _ \ / _` | __/ _ \   | || '__/ _` |/ __| |/ / _ \ '__|
            | |_| | | |_| | | | | | | (_| | ||  __/   | || | | (_| | (__|   <  __/ |   
             \___/|_|\__|_|_| |_| |_|\__,_|\__\___|   |_||_|  \__,_|\___|_|\_\___|_|   
                                                   by DrachenClon22
                                                        v1.0.0 

            """, LogLevel.COLOR_YELLOW);

        Thread consoleHandler = new Thread(ConsoleHandler);
        Thread connectionPingChecker = new Thread(() =>
        {
            int maxConnectionTries = 5;
            ICommand startCommand = new StartCommand();
            while (!ExitStatus)
            {
                _lockConsole = true;
                for (int i = 0; i < maxConnectionTries; i++)
                {
                    if (ProcessHandler.TryFindActiveServer(_silentServerCheck))
                    {
                        break;
                    }
                    else
                    {
                        startCommand.Execute(null);
                        if (i == maxConnectionTries - 1)
                        {
                            Logger.LogLine($"Cannot find and start server after {maxConnectionTries} tries!", LogLevel.ERROR, true);
                        }
                    }
                }

                _lockConsole = false;

                if (!consoleHandler.IsAlive)
                {
                    // Start console and console locker
                    consoleHandler.Start();
                }

                // Check server every 120 seconds
                Task.Delay(TimeSpan.FromSeconds(120)).Wait();
            }
        });

        (bool, string) result = JSONFileManager.LoadConfig();
        if (result.Item1)
        {
            connectionPingChecker.Start();
        } else
        {
            Logger.LogLine("Config \"config.json\" created, please, set values.", LogLevel.WARNING);
            Logger.LogLine($"Config file can be found at {result.Item2}", LogLevel.WARNING);
            ExitStatus = true;
        }

        while (!ExitStatus) { await Task.Delay(500); }
        ProcessHandler.CloseServer();
        Process.GetCurrentProcess().Kill();
    }

    static void DrawConsoleInputTriangle()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Page.CurrentPage.PageName}");
        Console.ResetColor();
        Console.Write("> ");
    }

    static void ConsoleHandler()
    {
        Logger.LogLine("Console started. Use \"help\" for commands list", LogLevel.INFO);

        // These are for buffering
        string? input = null;
        string[]? args = null;
        CommandTask? task = null!;

        Page.CurrentPage = Page.HomePage;
        DrawConsoleInputTriangle();

        do
        {
            try
            {
                if (!_lockConsole)
                {
                    input = Console.ReadLine();

                    if (input is not null)
                    {
                        args = (input.Contains(' ') ? input.Split(' ').Skip(1).ToArray() : null) ?? null;
                        input = input.Contains(' ') ? input.Substring(0, input.IndexOf(' ')) : input;

                        /*
                        * Allfather commands have a higher priority and are executed first, so if 
                        * the commands among regular and such ones match, then Allfather commands are executed
                        */
                        task = AllfatherCommands.Find(x => x.Name.Contains(input ?? "help")) ??
                            Page.CurrentPage.Tasks.Find(x => x.Name.Contains(input ?? "help"));

                        if (task is null)
                        {
                            if (input?.Length > 0)
                            {
                                Logger.LogLine($"No \"{input}\" command found. Use \"help\" for commands list", LogLevel.WARNING);
                            }
                            else
                            {
                                Logger.LogLine($"Enter a command. Use \"help\" for commands list", LogLevel.WARNING);
                            }
                        }
                        else
                        {
                            task.Execute(input!, args);

                            Page.CurrentPage = Page.PageStack.TryPeek(out Page? result) ? result! : Page.HomePage;

                            DrawConsoleInputTriangle();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogLine($"{e.GetType()} {e.Message}", LogLevel.ERROR, true);
            }
        } while (!ExitStatus);
    }
}