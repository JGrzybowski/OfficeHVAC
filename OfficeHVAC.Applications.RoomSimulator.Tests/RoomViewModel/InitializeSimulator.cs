using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Actors;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class InitializeSimulator : TestKit
    {
        private readonly IActorRef blackHole;
        private readonly IRoomSimulatorActorPropsFactory _roomSimulatorActorPropsFactoryFake;
        private ConnectionConfig.Builder connectionConfigFake;

        public InitializeSimulator()
        {
            blackHole = ActorOf(BlackHoleActor.Props);

            _roomSimulatorActorPropsFactoryFake = Substitute.For<IRoomSimulatorActorPropsFactory>();
            _roomSimulatorActorPropsFactoryFake.Props().Returns(BlackHoleActor.Props);
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
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake)
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
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
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake)
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            var roomActor = vm.LocalActorSystem.ActorSelection($"/user/{ViewModels.RoomViewModel.RoomActorName}").ResolveOne(TimeSpan.FromSeconds(2)).Result;
            roomActor.ShouldNotBeNull();
        }
        
        [Fact]
        public void stops_initialization_if_exception_is_thrown()
        {
            //Arrange
            SetupFakes(Config.Empty, blackHole.Path);
            this.connectionConfigFake
                .WhenForAnyArgs(builder => builder.Build())
                .Do(x => { throw new ArgumentException(); });
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake)
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.IsConnected.ShouldBe(false);
            vm.LocalActorSystem.ShouldBeNull();
        }

        [Fact]
        public void creates_bridge_actor()
        {
            //Arrange
            SetupFakes(Config.Empty, blackHole.Path);
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake)
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            var bridgeActor = vm.LocalActorSystem.ActorSelection("/user/bridge").ResolveOne(TimeSpan.FromSeconds(2)).Result;
            bridgeActor.ShouldNotBeNull();
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
            }"), 
            blackHole.Path);
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake)
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
            };

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.LocalActorSystem.Settings.Config.GetInt("akka.remote.helios.tcp.port").ShouldBe(8090);
        }
    }
}
