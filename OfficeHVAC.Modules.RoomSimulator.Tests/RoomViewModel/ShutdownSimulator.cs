using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Factories.Configs;
using OfficeHVAC.Modules.RoomSimulator.Factories;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.RoomViewModel
{
    public class ShutdownSimulator : TestKit
    {
        private readonly IRoomSimulatorActorPropsFactory _roomSimulatorActorPropsFactoryFake;
        private readonly IConfigBuilder _configBuilderFake;
        private readonly IBridgeRoomActorPropsFactory _bridgeRoomActorPropsFactoryFake;

        public ShutdownSimulator()
        {
            _roomSimulatorActorPropsFactoryFake = Substitute.For<IRoomSimulatorActorPropsFactory>();
            _roomSimulatorActorPropsFactoryFake.Props().Returns(EchoActor.Props(this,false));

            _configBuilderFake = Substitute.For<IConfigBuilder>();
            _configBuilderFake.Config().Returns(Config.Empty);

            _bridgeRoomActorPropsFactoryFake = Substitute.For<IBridgeRoomActorPropsFactory>();
            _bridgeRoomActorPropsFactoryFake.Props().Returns(BlackHoleActor.Props);
        }

        [Fact]
        public async Task cleans_up_viewModel()
        {
            //Arrange
            var vm = new ViewModels.RoomSimulatorViewModel(_configBuilderFake, _bridgeRoomActorPropsFactoryFake);
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
