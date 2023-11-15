using CoreRCON;
using Server_RCON_Tracker_v1.JSON;
using Server_RCON_Tracker_v1.Server;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Utils
{
    internal class ProcessHandler
    {
        public static Process? Server { get; set; }

        public static object ServerLock = new object();

        private static bool _serverIsStarting = false;
        public static bool TryFindProcess(bool silent = false)
        {
            if (ServerHandler.GetUpstate(silent).Result)
            {
                Process[] localByName = Process.GetProcessesByName("java");
                if (localByName is not null)
                {
                    foreach(Process p in localByName)
                    {
                        bool result = p.MainWindowTitle.ToLower().Contains("minecraft") ||
                                        p.MainWindowTitle.ToLower().Contains("server");
                        if (result)
                        {
                            Server = p;
                            break;
                        }
                    }
                    if (Server is not null)
                    {
                        return true;
                    }
                }
            } else
            {
                if (Server is not null)
                {
                    if (!_serverIsStarting)
                    {
                        KillServer();
                    } else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool TryFindActiveServer(bool silent = false)
        {
            bool result = false;
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                var token = cts.Token;
                var loadingTask = Loading.ShowLoading(token, "Trying to find an active server, please wait...", silent);
                result = TryFindProcess(silent);
                cts.Cancel();
                loadingTask.Wait();
                if (!silent)
                {
                    Logger.LogLine($"Active server {(result ? "FOUND" : "NOT FOUND")}", result ? LogLevel.COLOR_GREEN : LogLevel.COLOR_RED);
                }
            }
            return result;
        }
        private static bool CloseOrTimeout(int timeout)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.CancelAfter(TimeSpan.FromSeconds(timeout));
                CancellationToken token = cts.Token;
                Task waitForExit = Task.Run(() =>
                {
                    if (Server != null)
                    {
                        Server.WaitForExitAsync(token).Wait();
                        Server.Close();
                    }
                    cts.Cancel();
                });

                try
                {
                    Task.Delay(TimeSpan.FromSeconds(timeout * 2), token).Wait();
                } catch { }
            }

            return !ServerHandler.GetUpstate().Result;
        }

        public static void CloseServer()
        {
            lock (ServerLock)
            {
                if (Server != null)
                {
                    Logger.LogLine("Closing the server...", LogLevel.INFO);
                    Server.CloseMainWindow();

                    int timeout = 20;
                    if (CloseOrTimeout(timeout))
                    {
                        Logger.LogLine("Server closed!", LogLevel.INFO);
                    }
                    else
                    {
                        Logger.LogLine("Server closing timeout! Check if server still online manually and close it if needed. " +
                            $"This error caused because server didn't close in {timeout} seconds.", LogLevel.ERROR);
                    }

                    Server.Dispose();
                    Server = null;
                }
                else
                {
                    Logger.LogLine("Server is offline!", LogLevel.DEBUG);
                }
            }
        }

        public static void KillServer()
        {
            if (ServerLock != null)
            {
                Server?.Kill();

                int timeout = 20;
                if (CloseOrTimeout(timeout))
                {
                    Logger.LogLine("Server killed!", LogLevel.DEBUG);
                }
                else
                {
                    Logger.LogLine("Server killing timeout! Check if server still online manually and close it if needed. " +
                            $"This error caused because server wasn't killed in {timeout} seconds.", LogLevel.ERROR);
                }

                Server?.Dispose();
                Server = null;
            }
            else
            {
                Logger.LogLine("Server is offline!", LogLevel.DEBUG);
            }
        }
        public static void StartServerProcess()
        {
            Logger.LogLine("Please, do not close this window, server is starting...", LogLevel.INFO);
            lock (ServerLock)
            {
                _serverIsStarting = true;
                if (Server != null)
                {
                    CloseServer();
                }

                var processInfo = new ProcessStartInfo(JSONFileManager.Config.StartFilepath)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = JSONFileManager.Config.ServerPath
                };

                try
                {
                    if ((Server = Process.Start(processInfo)!) == null)
                    {
                        throw new InvalidOperationException("??");
                    }
                    else
                    {
                        using (CancellationTokenSource cts = new CancellationTokenSource())
                        {
                            CancellationToken token = cts.Token;
                            var loadingTask = Loading.ShowLoading(token);
                            ServerHandler.ConnectToServer().Wait();
                            cts.Cancel();
                            loadingTask.Wait();
                        }
                    }
                } catch (Exception e)
                {
                    Logger.LogLine(e.Message, LogLevel.ERROR, true);
                } finally
                {
                    _serverIsStarting = false;
                }
                return;
            }
        }
    }
}
