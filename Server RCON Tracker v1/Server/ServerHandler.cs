using CoreRCON;
using Server_RCON_Tracker_v1.JSON;
using Server_RCON_Tracker_v1.Server.Entities;
using Server_RCON_Tracker_v1.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.Server
{
    static internal class ServerHandler
    {
        static string? _ip;
        static string? _password;
        static ushort? _port;

        static RCON? _connection;

        static ServerHandler()
        {
            try
            {
                SetConnectionInfo(IPHandler.GetExternalIpAddress().Result?.ToString(), JSONFileManager.Config.Port, JSONFileManager.Config.RconPassword);
                SetConnection();
                _connection?.ConnectAsync().Wait();
            } catch { }
        }
        public static ServerInfo? GetServerInfo()
        {
            if (_ip is null || _port is null)
            {
                return null;
            }
            return new ServerInfo(IPAddress.Parse(_ip), _port.Value);
        }

        public static async Task<bool> GetUpstate(bool silent = false)
        {
            try
            {
                if (_connection is null)
                {
                    return false;
                }
                string reply = await _connection.SendCommandAsync("ping");
                if (!silent)
                {
                    Logger.LogLine($"Server responded with: {reply}", LogLevel.DEBUG);
                }

                return reply.Length > 0;
            } catch
            {
                return false;
            }
        }
        private static void SetConnectionInfo(string? ip, ushort port, string password)
        {
            try
            {
                _ip = ip ?? JSONFileManager.Config.IP ?? "127.0.0.1";
            } catch
            {
                _ip = "127.0.0.1";
            }
            _port = port;
            _password = password;
        }
        public static async Task<bool> ConnectToServer()
        {
            try
            {
                if (_connection is not null)
                {
                    Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                    bool connected = false;
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            await _connection.ConnectAsync();
                            connected = true;
                        }
                        catch
                        {
                            continue;
                        }

                        if (connected)
                        {
                            break;
                        }

                        if (i == 4)
                        {
                            throw new Exception("No reply from the server after 5 attempts");
                        }
                    }
                } else
                {
                    throw new Exception("No connection to the server...");
                }

                Logger.LogLine("Connection successful. Server started!", LogLevel.COLOR_GREEN);
            }
            catch (Exception e)
            {
                Logger.LogLine(e.Message, LogLevel.ERROR);
                ProcessHandler.KillServer();
                return false;
            }
            return true;
        }

        private static void SetConnection()
        {
            _connection = new RCON(IPAddress.Parse(_ip!), _port.GetValueOrDefault(), _password!);
        }
    }
}
