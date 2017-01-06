using NSubstitute;
using OfficeHVAC.Factories.Configs;
using OfficeHVAC.Modules.RoomSimulator.Factories;
using OfficeHVAC.Modules.RoomSimulator.ViewModels;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.RoomSimulator.Tests.RoomViewModel
{
    public class Constructor
    {
        [Fact]
        public void creates_disconnected_viewModel()
        {
            //Arrange & Act
            var vm = new RoomSimulatorViewModel(
                Substitute.For<IConfigBuilder>(),
                Substitute.For<IBridgeRoomActorPropsFactory>()
            );

            //Assert
            vm.IsConnected.ShouldBe(false);
        }
    }
}
