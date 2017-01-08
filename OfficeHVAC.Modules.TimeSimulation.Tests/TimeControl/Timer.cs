using NSubstitute;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Shouldly;
using System.ComponentModel;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.TimeControl
{
    public class Timer
    {
        public class SpeedProperty
        {
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
                vm.Speed = 3;

                //Assert
                UI_notified.ShouldBe(true);
                notificationArgs.ShouldNotBeNull();
                notificationArgs?.PropertyName.ShouldBe(nameof(TimeControlViewModel.Speed));
            }

            [Fact]
            public void changes_time_source_Speed()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock);

                //Act
                vm.Speed = 3;

                //Assert
                timeSourceMock.Received(1).Speed = 3;
            }

            [Fact]
            public void returns_time_source_Speed()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                timeSourceMock.Speed.Returns(5);
                var vm = new TimeControlViewModel(timeSourceMock);

                //Act
                var s = vm.Speed;

                //Assert
                s.ShouldBe(5);
            }
        }

        public class TimerTick
        {
            [Fact]
            public void updates_time_source_Now()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock);
                
                //Act
                vm.TimerTick(null, null);

                //Assert
                timeSourceMock.Received(1).UpdateClock();
            }
        }
    }
}
