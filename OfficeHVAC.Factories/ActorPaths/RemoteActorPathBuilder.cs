using Akka.Actor;
using Prism.Mvvm;

namespace OfficeHVAC.Factories.ActorPaths
{
    public class RemoteActorPathBuilder : BindableBase, IRemoteActorPathBuilder
    {
        private string _serverAddress;
        public string ServerAddress
        {
            get { return _serverAddress; }
            set { SetProperty(ref _serverAddress, value); }
        }

        private int? _serverPort;
        public int? ServerPort
        {
            get { return _serverPort; }
            set { SetProperty(ref _serverPort, value); }
        }

        private string _companyActorName;
        public string CompanyActorName
        {
            get { return _companyActorName; }
            set { SetProperty(ref _companyActorName, value); }
        }

        public virtual ActorPath ActorPath()
        {
            ServerAddress = ServerAddress?.Trim();
            bool isLocalAddress = string.IsNullOrWhiteSpace(ServerAddress) ||
                                  ServerAddress.Equals("localhost") ||
                                  ServerAddress.Equals("127.0.0.1");
            return new ChildActorPath(
                new RootActorPath(
                    new Address(
                        protocol: isLocalAddress ? "akka" : "akka.tcp",
                        system: "HVACsystem",
                        host: ServerAddress,
                        port: ServerPort
                    )
                ),
                $"user/{CompanyActorName}", 0);
        }
    }
}
