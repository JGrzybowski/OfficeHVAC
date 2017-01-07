using NodaTime;
using NSubstitute;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Shouldly;
using System.ComponentModel;
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
                var vm = new TimeControlViewModel(timeSourceMock);

                //Act
                vm.ToggleTimer();

                //Assert
                vm.IsRunning.ShouldBeTrue();
            }

            [Fact]
            public void can_stop_time_change()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock);

                //Act
                vm.ToggleTimer();
                vm.ToggleTimer();

                //Assert
                vm.IsRunning.ShouldBeFalse();
            }

            [Fact]
            public void notifies_UI()
            {
                //Arrange
                bool UI_notified = false;
                PropertyChangedEventArgs notificationArgs = null;

                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock);

                vm.PropertyChanged += (sender, args) =>
                {
                    UI_notified = true;
                    notificationArgs = args;
                };

                //Act
                vm.ToggleTimer();

                //Assert
                UI_notified.ShouldBe(true);
                notificationArgs.ShouldNotBeNull();
                notificationArgs?.PropertyName.ShouldBe(nameof(TimeControlViewModel.IsRunning));
            }

        }

        public class AddTime
        {
            [Fact]
            public void adds_minutes_to_time_source()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock);

                //Act
                vm.AddMinutes(5);

                //Assert
                timeSourceMock.Received(1).AddMinutes(5);
            }
        }

        public class Tick
        {
            [Fact]
            public void updates_time_source()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock);

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
                var vm = new TimeControlViewModel(timeSourceMock) {ResetTime = resetInstant};
                
                //Act
                vm.Reset();

                //Assert
                timeSourceMock.Received(1).Reset(resetInstant);
            }
        }
    }
}
