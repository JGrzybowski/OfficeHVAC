using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Actors;
using OfficeHVAC.Factories.Propses;
using Shouldly;
using System.Threading.Tasks;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.ActorPaths;
using OfficeHVAC.Factories.Configs;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class ShutdownSimulator : TestKit
    {
        private readonly IRoomSimulatorActorPropsFactory _roomSimulatorActorPropsFactoryFake;
        private readonly IRemoteActorPathBuilder _remoteActorPathBuilderFake;
        private readonly IConfigBuilder _configBuilderFake;

        public ShutdownSimulator()
        {
            var blackHole = ActorOf(BlackHoleActor.Props);

            _roomSimulatorActorPropsFactoryFake = Substitute.For<IRoomSimulatorActorPropsFactory>();
            _roomSimulatorActorPropsFactoryFake.Props().Returns(EchoActor.Props(this,false));

            _remoteActorPathBuilderFake = Substitute.For<IRemoteActorPathBuilder>();
            _remoteActorPathBuilderFake.ActorPath().Returns(blackHole.Path);

            _configBuilderFake = Substitute.For<IConfigBuilder>();
            _configBuilderFake.Config().Returns(Config.Empty);
        }

        [Fact]
        public async Task cleans_up_viewModel()
        {
            //Arrange
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake, _remoteActorPathBuilderFake, _configBuilderFake)
            {
                BridgeActorProps = BlackHoleActor.Props,
            };
            vm.RoomSimulatorActorPropsFactory.RoomName = "Room 101";
            vm.InitializeSimulator();
            vm.IsConnected.ShouldBe(true);

            //Act
            await vm.ShutdownSimulator();

            //Assert
            vm.IsConnected.ShouldBe(false);
            vm.BridgeActor.ShouldBeNull();
            vm.LocalActorSystem.ShouldBeNull();
        }
    }
}
