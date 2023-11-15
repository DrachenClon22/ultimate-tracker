using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_RCON_Tracker_v1.JSON
{
    sealed internal class JSONConfig
    {
        public string IP = "127.0.0.1";
        public string StartFilepath = @"";
        public string ServerPath = @"";
        public string RconPassword = "password";
        public ushort Port = 25575;

        public JSONConfig(string ip, string path, string serverpath, string password, ushort port)
        {
            IP = ip;
            StartFilepath = path;
            ServerPath = serverpath;
            RconPassword = password;
            Port = port;
        }
    }

    internal static class JSONFileManager
    {
        public static JSONConfig Config { get; private set; }

        public static (bool,string) LoadConfig()
        {
            string path = @"config.json";
            bool result = File.Exists(path);

            if (result)
            {
                Config = JsonConvert.DeserializeObject<JSONConfig>(File.ReadAllText(path).Replace("\\","/"))!;
            } else
            {
                JSONConfig config = new JSONConfig("", @"start.bat", @"", "password", 25575);
                File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            return (result, Path.GetFullPath(path));
        }
    }
}
