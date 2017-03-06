using Akka.Actor;
using Fclp;
using OfficeHVAC.Factories.Configs;
using System;

namespace OfficeHVAC.Applications.LogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var configBuilder = new RemoteConfigBuilder();
            var options = new FluentCommandLineParser();
            options.Setup<int?>('p', "port").Callback(port => configBuilder.Port = port).SetDefault(8000);
            options.Parse(args);
            
            var system = ActorSystem.Create("HVACsystem", configBuilder.Config());

            var actor = system.ActorOf<LoggerActor>("Logger");

            do
            {

            } while (Console.ReadKey(true).Key == ConsoleKey.Escape);

            system.Terminate();
            
        }
    }
}
