using Akka.Actor;
using Prism.Mvvm;

namespace OfficeHVAC.Factories.ActorPaths
{
    public class RemoteActorPathBuilder : BindableBase, IRemoteActorPathBuilder
    {
        private string systemName = "OfficeHVAC";
        public string SystemName
        {
            get => systemName;
            set => SetProperty(ref systemName, value);
        }

        private string serverAddress;
        public string ServerAddress
        {
            get => serverAddress;
            set => SetProperty(ref serverAddress, value);
        }

        private int? serverPort;
        public int? ServerPort
        {
            get => serverPort;
            set => SetProperty(ref serverPort, value);
        }

        private string companyActorName;
        public string CompanyActorName
        {
            get => companyActorName;
            set => SetProperty(ref companyActorName, value);
        }

        public virtual ActorPath ActorPath()
        {
            ServerAddress = ServerAddress?.Trim();

            return 
                new ChildActorPath(
                    new ChildActorPath(
                        new RootActorPath(
                            new Address(
                                protocol: "akka",
                                system: SystemName
                            )
                        ), 
                        "user",0),
                    CompanyActorName, 0);
        }
    }
}
