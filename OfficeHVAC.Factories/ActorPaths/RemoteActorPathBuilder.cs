﻿using Akka.Actor;
using Prism.Mvvm;

namespace OfficeHVAC.Factories.ActorPaths
{
    public class RemoteActorPathBuilder : BindableBase, IRemoteActorPathBuilder
    {
        private string _systemName = "HVACsystem";
        public string SystemName
        {
            get { return _systemName; }
            set { SetProperty(ref _systemName, value); }
        }

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
            
            return new ChildActorPath(
                new RootActorPath(
                    new Address(
                        protocol: "akka.tcp",
                        system: SystemName,
                        host: ServerAddress,
                        port: ServerPort
                    )
                ),
                $"user/{CompanyActorName}", 0);
        }
    }
}
