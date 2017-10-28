using NSubstitute;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Shouldly;
using System.ComponentModel;
using System.Threading;
using Akka.TestKit.Xunit2;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.TimeControl
{
    public class Timer
    {
        public class SpeedProperty : TestKit
        {
            [Fact]
            public void notifies_UI()
            {
                //Arrange
                bool UI_notified = false;
                PropertyChangedEventArgs notificationArgs = null;

                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, this.Sys);

                vm.PropertyChanged += (sender, args) =>
                {
                    UI_notified = true;
                    notificationArgs = args;
                };

                //Act
                vm.Speed = 3;
                Thread.Sleep(500);

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
                var vm = new TimeControlViewModel(timeSourceMock, this.Sys);

                //Act
                vm.Speed = 3;
                Thread.Sleep(500);

                //Assert
                timeSourceMock.Speed.ShouldBe(3);
            }
        }

        public class TimerTick : TestKit
        {
            [Fact]
            public void updates_time_source_Now()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, this.Sys);

                //Act
                vm.TimerTick(null, null);
                Thread.Sleep(500);

                //Assert
                timeSourceMock.Received(1).UpdateClock();
            }
        }
    }
}
