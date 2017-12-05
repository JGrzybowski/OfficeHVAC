using System.Collections.Generic;
using Akka.TestKit.Xunit2;
using NSubstitute;
using OfficeHVAC.Modules.TimeSimulation.TimeSources;
using OfficeHVAC.Modules.TimeSimulation.ViewModels;
using Shouldly;
using System.ComponentModel;
using System.Threading;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.TimeControl
{
    public class Buttons 
    {
        public class Toogle :TestKit
        {
            [Fact]
            public void can_start_time_change()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, Sys);

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
                var vm = new TimeControlViewModel(timeSourceMock, Sys);

                //Act
                vm.ToggleTimer();
                vm.ToggleTimer();

                //Assert
                vm.IsRunning.ShouldBeFalse();
            }

            [Fact]
            public void notifies_UI_when_turning_on()
            {
                //Arrange
                bool uiNotified = false;
                List<PropertyChangedEventArgs> notificationArgs = new List<PropertyChangedEventArgs>();

                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, Sys);

                vm.PropertyChanged += (sender, args) =>
                {
                    uiNotified = true;
                    notificationArgs.Add(args);
                };

                //Act
                vm.ToggleTimer();

                //Assert
                uiNotified.ShouldBe(true);
                notificationArgs.ShouldContain(notif => notif.PropertyName == nameof(TimeControlViewModel.IsRunning));
            }

            [Fact]
            public void notifies_UI_when_turning_off()
            {
                //Arrange
                bool uiNotified = false;
                PropertyChangedEventArgs notificationArgs = null;

                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, Sys);
                vm.ToggleTimer();

                vm.PropertyChanged += (sender, args) =>
                {
                    uiNotified = true;
                    notificationArgs = args;
                };

                //Act
                vm.ToggleTimer();

                //Assert
                uiNotified.ShouldBe(true);
                notificationArgs.ShouldNotBeNull();
                notificationArgs?.PropertyName.ShouldBe(nameof(TimeControlViewModel.IsRunning));
            }
        }

        public class AddTime : TestKit
        {
            [Fact]
            public void adds_minutes_to_time_source()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, Sys);

                //Act
                vm.AddMinutes(5);

                //Assert
                timeSourceMock.Received(1).AddMinutes(5);
            }

            [Fact]
            public void notifies_UI()
            {
                //Arrange
                bool uiNotified = false;
                List<PropertyChangedEventArgs> notificationArgs = new List<PropertyChangedEventArgs>();

                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, Sys);
                vm.PropertyChanged += (sender, args) =>
                {
                    uiNotified = true;
                    notificationArgs.Add(args);
                };

                //Act
                vm.AddMinutes(5);
                Thread.Sleep(300);

                //Assert
                uiNotified.ShouldBe(true);
                notificationArgs.ShouldContain(notif => notif.PropertyName == nameof(TimeControlViewModel.TimeText));
            }
        }

        public class Tick : TestKit
        {
            [Fact]
            public void updates_time_source()
            {
                //Arrange
                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, Sys);

                //Act
                vm.TickManually();
                Thread.Sleep(500);

                //Assert
                timeSourceMock.Received(1).UpdateClock();
            }

            [Fact]
            public void notifies_UI()
            {
                //Arrange
                bool uiNotified = false;
                PropertyChangedEventArgs notificationArgs = null;

                var timeSourceMock = Substitute.For<IControlledTimeSource>();
                var vm = new TimeControlViewModel(timeSourceMock, Sys);
                vm.PropertyChanged += (sender, args) =>
                {
                    uiNotified = true;
                    notificationArgs = args;
                };

                //Act
                vm.TickManually();
                Thread.Sleep(500);

                //Assert
                uiNotified.ShouldBe(true);
                notificationArgs.ShouldNotBeNull();
                notificationArgs?.PropertyName.ShouldBe(nameof(TimeControlViewModel.TimeText));
            }
        }

        //public class Reset : TestKit
        //{
        //    [Fact]
        //    public void resets_time_source_to_specified_date()
        //    {
        //        //Arrange
        //        var resetInstant = Instant.FromUtc(2012, 12, 31, 23, 59, 59);
        //        var timeSourceMock = Substitute.For<IControlledTimeSource>();
        //        var vm = new TimeControlViewModel(timeSourceMock, this.Sys) { ResetTime = resetInstant};
                
        //        //Act
        //        vm.Reset();

        //        //Assert
        //        timeSourceMock.Received(1).Reset(resetInstant);
        //    }

        //    [Fact]
        //    public void notifies_UI()
        //    {
        //        //Arrange
        //        bool UI_notified = false;
        //        PropertyChangedEventArgs notificationArgs = null;

        //        var timeSourceMock = Substitute.For<IControlledTimeSource>();
        //        var vm = new TimeControlViewModel(timeSourceMock, this.Sys);
        //        vm.PropertyChanged += (sender, args) =>
        //        {
        //            UI_notified = true;
        //            notificationArgs = args;
        //        };

        //        //Act
        //        vm.Reset();

        //        //Assert
        //        UI_notified.ShouldBe(true);
        //        notificationArgs.ShouldNotBeNull();
        //        notificationArgs?.PropertyName.ShouldBe(nameof(TimeControlViewModel.TimeText));
        //    }
        //}
    }
}
