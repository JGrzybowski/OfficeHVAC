using NSubstitute;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Shouldly;
using System.ComponentModel;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.TimeController
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
                var vm = new TimeControllerViewModel(timeSourceMock);

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
                notificationArgs?.PropertyName.ShouldBe(nameof(TimeControllerViewModel.Speed));
            }

            [Fact]
            public void changes_timeSource_Speed()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock);

                //Act
                vm.Speed = 3;

                //Assert
                timeSourceMock.Received(1).Speed = 3;
            }
        }

        public class Tick
        {
            [Fact]
            public void notifies_UI()
            {
                //Arrange
                bool UI_notified = false;
                PropertyChangedEventArgs notificationArgs = null;

                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock);

                vm.PropertyChanged += (sender, args) =>
                {
                    UI_notified = true;
                    notificationArgs = args;
                };

                //Act
                vm.TimerTick(null, null);

                //Assert
                UI_notified.ShouldBe(true);
                notificationArgs.ShouldNotBeNull();
                notificationArgs?.PropertyName.ShouldBe(nameof(IControlledTimeSource.Now));
            }

            [Fact]
            public void updates_timeSource_Now()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControllerViewModel(timeSourceMock);
                
                //Act
                vm.TimerTick(null, null);

                //Assert
                timeSourceMock.Received(1).UpdateClock();
            }
        }
    }
}
