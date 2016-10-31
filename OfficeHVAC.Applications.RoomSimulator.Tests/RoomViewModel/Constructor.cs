using NSubstitute;
using OfficeHVAC.Applications.RoomSimulator.Factories;
using OfficeHVAC.Factories.Configs;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Applications.RoomSimulator.Tests.RoomViewModel
{
    public class Constructor
    {
        [Fact]
        public void creates_disconnected_viewModel()
        {
            //Arrange & Act
            var vm = new ViewModels.RoomSimulatorViewModel(
                Substitute.For<IConfigBuilder>(),
                Substitute.For<IBridgeRoomActorPropsFactory>()
            );

            //Assert
            vm.IsConnected.ShouldBe(false);
        }
    }
}
