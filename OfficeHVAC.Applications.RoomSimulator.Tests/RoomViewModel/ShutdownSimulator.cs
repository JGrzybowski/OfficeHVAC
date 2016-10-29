using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class ShutdownSimulator : TestKit
    {
        private readonly IRoomSimulatorActorPropsFactory _roomSimulatorActorPropsFactoryFake;
        private readonly IConfigBuilder _configBuilderFake;

        public ShutdownSimulator()
        {
            var blackHole = ActorOf(BlackHoleActor.Props);

            _roomSimulatorActorPropsFactoryFake = Substitute.For<IRoomSimulatorActorPropsFactory>();
            _roomSimulatorActorPropsFactoryFake.Props().Returns(EchoActor.Props(this,false));

            _configBuilderFake = Substitute.For<IConfigBuilder>();
            _configBuilderFake.Config().Returns(Config.Empty);
        }

        [Fact]
        public async Task cleans_up_viewModel()
        {
            //Arrange
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake, _configBuilderFake)
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
