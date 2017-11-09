using Akka.Configuration;
using Prism.Mvvm;

namespace OfficeHVAC.Factories.Configs
{
    public class RemoteConfigBuilder : BindableBase, IConfigBuilder
    {
        private int? port;
        public int? Port
        {
            get => port;
            set => SetProperty(ref port, value);
        }
        
        public Config Config()
        {
            string configString =
                @"akka {
                    actor { 
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        serializers {
                            hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                        }
                        serialization-bindings {
                            ""System.Object"" = hyperion
                        }
                    }
                    remote {
                        helios.tcp {" +
                            $"port = {Port}," +
                            @"hostname = localhost
                        }
                    }
                }
                akka.scheduler.implementation = ""Akka.TestKit.TestScheduler, Akka.TestKit""";
            return ConfigurationFactory.ParseString(configString);
        }
    }
}
