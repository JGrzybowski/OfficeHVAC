using Akka.TestKit;
using Akka.TestKit.Xunit2;
using NodaTime;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.ControlledTimeSource
{
    public class AddXYZ : TestKit
    {
        private static readonly Instant InitialTime = Instant.FromUtc(2000, 12, 01, 12, 00, 00);

        [Fact]
        public void can_change_time_in_hours()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, this.Sys.Scheduler as TestScheduler);

            //Act
            clock.AddHours(3);

            //Assert
            clock.Now.ShouldBe(InitialTime + Duration.FromHours(3));
        }

        [Fact]
        public void can_change_time_in_minutes()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, this.Sys.Scheduler as TestScheduler);

            //Act
            clock.AddMinutes(3);

            //Assert
            clock.Now.ShouldBe(InitialTime + Duration.FromMinutes(3));
        }

        [Fact]
        public void can_change_time_in_seconds()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, this.Sys.Scheduler as TestScheduler);

            //Act
            clock.AddSeconds(3);

            //Assert
            clock.Now.ShouldBe(InitialTime + Duration.FromSeconds(3));
        }

    }
}