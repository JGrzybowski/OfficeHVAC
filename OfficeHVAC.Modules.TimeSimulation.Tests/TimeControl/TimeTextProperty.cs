using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.TimeControl
{
    public class TimeTextProperty : TestKit
    {
        [Fact]
        public void returns_string_based_on_time_source_Now()
        {
            //Arrange
            var timeSourceMock = Substitute.For<IControlledTimeSource>();
            var vm = new TimeControlViewModel(timeSourceMock, this.Sys);

            //Act
            var text = vm.TimeText;

            //Assert
            var n = timeSourceMock.Received(1).Now;
        }
    }
}
