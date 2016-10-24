using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Actors;
using OfficeHVAC.Messages;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class InitializeSimulator : TestKit
    {
        private IActorRef blackHole;
        private IActorRef echoActor;
        private ConnectionConfig.Builder connectionConfigFake;

        public InitializeSimulator()
        {
            blackHole = ActorOf(BlackHoleActor.Props);
            echoActor = ActorOf(EchoActor.Props(this, false));
        }

        private void SetupFakes(Config hostingConfig, ActorPath companyActorPath)
        {
            var configStub = new ConnectionConfig(hostingConfig,companyActorPath);
            connectionConfigFake = Substitute.For<ConnectionConfig.Builder>();
            connectionConfigFake.Build().Returns(configStub);
        }
        
        [Fact]
        public void sets_is_running_property()
        {
            //Arrange
            SetupFakes(Config.Empty, blackHole.Path);
            var vm = new ViewModels.RoomViewModel
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
                RoomActorProps = BlackHoleActor.Props
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.IsConnected.ShouldBe(true);
        }

        [Fact]
        public void creates_room_actor()
        {
            //Arrange
            SetupFakes(Config.Empty, blackHole.Path);
            var vm = new ViewModels.RoomViewModel
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
                RoomActorProps = BlackHoleActor.Props
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.ActorSystem.ActorSelection("/user/bridge/room").ResolveOne(TimeSpan.FromSeconds(2)).ShouldNotBeNull();
        }

        [Fact]
        public void creates_bridge_actor()
        {
            //Arrange
            SetupFakes(Config.Empty, blackHole.Path);
            var vm = new ViewModels.RoomViewModel
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
                RoomActorProps = BlackHoleActor.Props
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.ActorSystem.ActorSelection("/user/bridge").ResolveOne(TimeSpan.FromSeconds(2)).ShouldNotBeNull();
        }

        [Fact]
        public void starts_actor_system_on_selected_port()
        {
            //Arrange
            SetupFakes(ConfigurationFactory.ParseString(
            @"akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }
                remote {
                    helios.tcp {
                        port = 8090
                        hostname = localhost
                    }
                }
            }"
            ), blackHole.Path);
            var vm = new ViewModels.RoomViewModel
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
                RoomActorProps = BlackHoleActor.Props
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.ActorSystem.Settings.Config.GetInt("akka.remote.helios.tcp.port").ShouldBe(8090);
        }
    }
}
