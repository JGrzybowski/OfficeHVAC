using System;
using Akka.Actor;
using Prism.Mvvm;

namespace OfficeHVAC.Factories.ActorPaths
{
    public class RemoteActorPathBuilder : BindableBase, IRemoteActorPathBuilder
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

        private string companyActorName;
        public string CompanyActorName
        {
            get { return companyActorName; }
            set { SetProperty(ref companyActorName, value); }
        }

        public virtual ActorPath ActorPath()
        {
            serverAddress = serverAddress?.Trim();
            bool isLocalAddress = string.IsNullOrWhiteSpace(serverAddress) ||
                                  serverAddress.Equals("localhost") ||
                                  serverAddress.Equals("127.0.0.1");
            return new ChildActorPath(
                new RootActorPath(
                    new Address(
                        protocol: isLocalAddress ? "akka" : "akka.tcp",
                        system: "HVACsystem",
                        host: serverAddress,
                        port: serverPort
                    )
                ),
                $"user/{companyActorName}", 0);
        }
    }
}
