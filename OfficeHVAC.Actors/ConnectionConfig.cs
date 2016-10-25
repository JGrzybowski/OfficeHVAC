using Akka.Actor;
using Akka.Configuration;
using Prism.Mvvm;

namespace OfficeHVAC.Actors
{
    public class ConnectionConfig : IConnectionConfig
    {
        public ConnectionConfig(Config configuration, ActorPath companyActorPath)
        {
            Configuration = configuration;
            CompanyActorPath = companyActorPath;
        }

        public ConnectionConfig(string serverAddress, int? serverPort, int? listeningPort, string companyActorName)
        {
            serverAddress = serverAddress?.Trim();
            bool isLocalAddress = string.IsNullOrWhiteSpace(serverAddress) || 
                                  serverAddress.Equals("localhost") ||
                                  serverAddress.Equals("127.0.0.1");
            CompanyActorPath = new ChildActorPath(
                new RootActorPath(
                    new Address(
                        protocol:
                        isLocalAddress ? "akka" : "akka.tcp",
                        system: "HVACsystem",
                        host: serverAddress,
                        port: serverPort
                    )
                ),
                $"user/{companyActorName}", 0);

            string configString =
                @"akka {
                    actor { provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote"" }
                    remote {
                        helios.tcp {" +
                            $"port = {listeningPort}," +
                            @"hostname = localhost
                        }
                    }
                }";
            Configuration = ConfigurationFactory.ParseString(configString);

        }

        public ActorPath CompanyActorPath { get; }
        public Config Configuration { get; }

        public class Builder : BindableBase
        {
            private string serverAddress;
            public string ServerAddress
            {
                get { return serverAddress; }
                set { SetProperty(ref serverAddress, value); }
            }

            private int? serverPort;
            public int? ServerPort
            {
                get { return serverPort; }
                set { SetProperty(ref serverPort, value); }
            }

            private int? listeningPort;
            public int? ListeningPort
            {
                get { return listeningPort; }
                set { SetProperty(ref listeningPort, value); }
            }

            private string companyActorName;
            public string CompanyActorName
            {
                get { return companyActorName; }
                set { SetProperty(ref companyActorName, value); }
            }

            public virtual ConnectionConfig Build() => new ConnectionConfig(
                this.ServerAddress,
                this.ServerPort,
                this.ListeningPort,
                this.CompanyActorName
            );
        }
    }
}
