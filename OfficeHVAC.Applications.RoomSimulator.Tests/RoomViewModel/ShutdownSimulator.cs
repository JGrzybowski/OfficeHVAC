using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Actors;
using OfficeHVAC.Factories.Propses;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class ShutdownSimulator : TestKit
    {
        private ConnectionConfig.Builder connectionConfigFake;
        private readonly IPropsFactory _roomSimulatorActorPropsFactoryFake;

        public ShutdownSimulator()
        {
            _roomSimulatorActorPropsFactoryFake = Substitute.For<IPropsFactory>();
            _roomSimulatorActorPropsFactoryFake.Props().Returns(BlackHoleActor.Props);
        }

        private void SetupFakes(Config hostingConfig, ActorPath companyActorPath)
        {
            var configStub = new ConnectionConfig(hostingConfig, companyActorPath);
            connectionConfigFake = Substitute.For<ConnectionConfig.Builder>();
            connectionConfigFake.Build().Returns(configStub);
        }

        [Fact]
        public async Task cleans_up_viewModel()
        {
            //Arrange
            SetupFakes(Config.Empty, TestActor.Path);
            var vm = new ViewModels.RoomViewModel(_roomSimulatorActorPropsFactoryFake)
            {
                ConnectionConfigBuilder = connectionConfigFake,
                BridgeActorProps = BlackHoleActor.Props,
                RoomName = "room"
            };
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
