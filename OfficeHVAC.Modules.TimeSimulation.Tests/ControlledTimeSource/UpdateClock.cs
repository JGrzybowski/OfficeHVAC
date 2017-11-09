using Akka.TestKit;
using Akka.TestKit.Xunit2;
using NodaTime;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.ControlledTimeSource
{
    public class UpdateClock : TestKit
    {
        private static readonly Instant InitialTime = Instant.FromUtc(2000, 12, 01, 12, 00, 00);

        [Fact]
        public void does_not_change_Now_value_when_speed_is_set_to_0()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, Sys.Scheduler as TestScheduler){ Speed = 0 };

            //Act
            clock.UpdateClock();

            //Assert
            clock.Now.ShouldBe(InitialTime);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(60.0)]
        [InlineData(150.0)]
        public void changes_Now_value_accoding_to_speed_value(double speed)
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, Sys.Scheduler as TestScheduler){ Speed = speed };

            //Act
            clock.UpdateClock();
            clock.UpdateClock();

            //Assert
            clock.Now.ShouldBe(InitialTime + Duration.FromMilliseconds((long)(2 * speed * 1000)));
        }
    }

}