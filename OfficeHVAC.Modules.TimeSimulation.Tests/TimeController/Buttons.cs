using NodaTime;
using NSubstitute;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using System.Threading;
using Xunit;
namespace OfficeHVAC.Modules.TimeSimulation.Tests.TimeController
{
    public class Buttons
    {
        public class Toogle
        {
            [Fact]
            public void can_start_time_change()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock,60);

                //Act
                vm.ToggleTimer();

                //Assert
                Thread.Sleep(100);
                timeSourceMock.Received(1).UpdateClock();
            }

            [Fact]
            public void can_stop_time_change()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock,90);

                //Act
                vm.ToggleTimer();
                Thread.Sleep(100);
                vm.ToggleTimer();
                Thread.Sleep(100);

                //Assert
                timeSourceMock.Received(1).UpdateClock();
            }
        }

        public class AddTime
        {
            [Fact]
            public void can_start_time_change()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock);

                //Act
                vm.AddMinutes(5);

                //Assert
                timeSourceMock.Received(1).AddMinutes(5);
            }
        }

        public class Tick
        {
            [Fact]
            public void updates_timeSource()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock);

                //Act
                vm.TickManually();

                //Assert
                timeSourceMock.Received(1).UpdateClock();
            }
        }

        public class Reset
        {
            [Fact]
            public void resets_time_source_to_specified_date()
            {
                //Arrange
                var resetInstant = Instant.FromUtc(2012, 12, 31, 23, 59, 59);
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock) {ResetTime = resetInstant};
                
                //Act
                vm.Reset();

                //Assert
                timeSourceMock.Received(1).Reset(resetInstant);
            }
        }
    }
}
