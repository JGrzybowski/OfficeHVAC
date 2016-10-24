using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Prism.Mvvm;
using Akka.Configuration;

namespace OfficeHVAC.Actors
{
    public class ConnectionConfig : IConnectionConfig
    {
        public string ServerAddress { get; }
        public string ServerPort { get; }
        public string ListeningPort { get; }

        public ConnectionConfig(string serverAddress, string serverPort, string listeningPort, string CompanyActorName)
        {
            throw  new NotImplementedException();
            ServerAddress = serverAddress;
            ServerPort = serverPort;
            ListeningPort = listeningPort;
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

            private string serverPort;

            public string ServerPort
            {
                get { return serverPort; }
                set { SetProperty(ref serverPort, value); }
            }

            private string listeningPort;

            public string ListeningPort
            {
                get { return listeningPort; }
                set { SetProperty(ref listeningPort, value); }
            }

            private string companyActorName;
            public string ComapnyActorName
            {
                get { return companyActorName; }
                set { SetProperty(ref companyActorName, value); }
            }

            public ConnectionConfig Build() => new ConnectionConfig(ServerAddress, ServerPort, ListeningPort, ComapnyActorName);
        }
    }
}
