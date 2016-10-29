using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class InitializeSimulator : TestKit
    {
        private readonly IRoomSimulatorActorPropsFactory _roomSimulatorActorPropsFactoryFake;
        private readonly IConfigBuilder _configBuilderFake;
        private readonly IBridgeRoomActorPropsFactory _bridgeRoomActorPropsFactoryFake;

        public InitializeSimulator()
        {
            _roomSimulatorActorPropsFactoryFake = Substitute.For<IRoomSimulatorActorPropsFactory>();
            _roomSimulatorActorPropsFactoryFake.Props().Returns(BlackHoleActor.Props);

            _configBuilderFake = Substitute.For<IConfigBuilder>();
            _configBuilderFake.Config().Returns(Config.Empty);

            _bridgeRoomActorPropsFactoryFake = Substitute.For<IBridgeRoomActorPropsFactory>();
            _bridgeRoomActorPropsFactoryFake.Props().Returns(BlackHoleActor.Props);
        }
     
        [Fact]
        public void sets_is_running_property_to_true()
        {
            //Arrange
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake, _configBuilderFake, _bridgeRoomActorPropsFactoryFake);

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.IsConnected.ShouldBe(true);
        }

        [Fact]
        public void stops_initialization_if_exception_is_thrown()
        {
            //Arrange
            this._configBuilderFake
                .WhenForAnyArgs(builder => builder.Config())
                .Do(x => { throw new ArgumentException(); });
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake, _configBuilderFake, _bridgeRoomActorPropsFactoryFake);

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
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake, _configBuilderFake, _bridgeRoomActorPropsFactoryFake);

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
            _configBuilderFake.Config().Returns(ConfigurationFactory.ParseString(
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
            }"));
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake, _configBuilderFake, _bridgeRoomActorPropsFactoryFake);

            //Act
            vm.InitializeSimulator();

            //Assert
            vm.LocalActorSystem.Settings.Config.GetInt("akka.remote.helios.tcp.port").ShouldBe(8090);
        }
    }
}
