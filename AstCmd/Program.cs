using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using Services;

namespace AstCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggingConfiguration = new LoggingConfiguration();
            loggingConfiguration.AddTarget(new ConsoleTarget("console"));
            loggingConfiguration.AddRuleForAllLevels("console");
            LogManager.Configuration = loggingConfiguration;            

            var ssh = new AsteriskService(args[0], int.Parse(args[1]), args[2], args[3]);
            Console.WriteLine("originate");
            ssh.OriginateExt("i1", "966", "966@local").GetAwaiter().GetResult();
            Console.WriteLine("get channels");
            var channels = ssh.CoreGetChannels().GetAwaiter().GetResult();
            Console.WriteLine($"{string.Join("\n", channels)}");

            var priChannels = ssh.PriGetChannels().GetAwaiter().GetResult();
            Console.WriteLine($"{string.Join("\n", priChannels)}");

            Console.ReadLine();
        }
    }
}
