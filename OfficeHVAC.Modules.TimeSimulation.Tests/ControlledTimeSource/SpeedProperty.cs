using Akka.TestKit;
using Akka.TestKit.Xunit2;
using NodaTime;
using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.ControlledTimeSource
{
    public class SpeedProperty : TestKit
    {
        private static readonly Instant InitialTime = Instant.FromUtc(2000, 12, 01, 12, 00, 00);

        [Fact]
        public void throws_ArgumentOutOfRangeException_when_trying_to_set_to_negative_value()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, this.Sys.Scheduler as TestScheduler);

            //Act & Assert
            var ex = Should.Throw<ArgumentOutOfRangeException>(() => clock.Speed = -3);
            ex.ParamName.ShouldBe("value");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void sets_speed_when_value_is_non_negative(int newSpeed)
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, this.Sys.Scheduler as TestScheduler);

            //Act
            clock.Speed = newSpeed;

            //Assert
            clock.Speed.ShouldBe(newSpeed);
        }
    }
}