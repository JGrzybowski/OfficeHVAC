﻿using Akka.Configuration;
using Prism.Mvvm;

namespace OfficeHVAC.Factories.Configs
{
    public class RemoteConfigBuilder : BindableBase, IConfigBuilder
    {
        private int? _port;
        public int? Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }
        
        public Config Config()
        {
            string configString =
                @"akka {
                    actor { provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote"" }
                    remote {
                        helios.tcp {" +
                            $"port = {Port}," +
                            @"hostname = localhost
                        }
                    }
                }";
            return ConfigurationFactory.ParseString(configString);
        }
    }
}
