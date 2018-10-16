using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Renci.SshNet;

namespace Services
{
    public class AsteriskService
    {
        public const string Dahdi = "DAHDI";

        private readonly string _server;
        private readonly string _user;
        private readonly string _pass;
        private readonly int _port;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AsteriskService(string server, int port, string user, string pass)
        {
            _server = server;
            _port = port;
            _user = user;
            _pass = pass;
        }

        public async Task OriginateExt(string ifcId, string dnis, string extension)
        {
            //originate DAHDI/i2/966 extension 966@local
            await ExecuteSsh($"originate DAHDI/{ifcId}/{dnis} extension {extension}");
        }

        public async Task OriginateApp(string ifcId, string dnis, string appName)
        {
            await ExecuteSsh($"originate DAHDI/{ifcId}/{dnis} extension {appName}");
        }

        public async Task<List<CoreChannelStatus>> CoreGetChannels()
        {
            string TryGet(string[] s, int idx) { return s.Length > idx ? s[idx] : string.Empty; };

            var list = (await ExecuteSsh("core show channels"))
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => s.StartsWith(Dahdi))
                .Select(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => new CoreChannelStatus
                {
                    Id = TryGet(s, 0),
                    Location = TryGet(s, 1),
                    State = TryGet(s, 2),
                    Data = TryGet(s, 3)
                });
            return list.ToList();
        }

        public async Task<List<PriChannelStatus>> PriGetChannels()
        {
            string TryGet(string[] s, int idx) { return s.Length > idx ? s[idx] : string.Empty; };

            var list = (await ExecuteSsh("pri show channels"))
                .Split('\n')
                .Skip(2)
                .Select(s => s.Trim())
                .Select(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => new PriChannelStatus
                {
                    SpanId = TryGet(s, 0),
                    ChanId = TryGet(s, 1),
                    ChanB = TryGet(s, 2),
                    Idle = TryGet(s, 3),
                    CallLevel = TryGet(s, 4),
                    PriCall = TryGet(s, 5),
                    Channel = TryGet(s, 6)
                });
            return list.ToList();
        }

        private async Task<string> ExecuteSsh(string cmd)
        {
            using (var client = new SshClient(_server, _port, _user, _pass))
            {
                _logger.Info($"ExecuteSsh: {cmd}");
                client.Connect();
                var cmdTxt = $"asterisk -rx \"{cmd}\"";
                var command = client.CreateCommand(cmdTxt);
                var result = await Task.Factory.FromAsync(command.BeginExecute(), command.EndExecute);
                _logger.Info($"ExecuteSsh: {result}");
                return result;
            }
        }
    }
}