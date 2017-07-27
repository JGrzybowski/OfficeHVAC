using Akka.TestKit;
using Akka.TestKit.Xunit2;
using NodaTime;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TimeSimulation.Tests.ControlledTimeSource
{
    public class Constructor : TestKit
    {
        private static readonly Instant InitialTime = Instant.FromUtc(2000, 12, 01, 12, 00, 00);

        [Fact]
        public void returns_initial_time_after_being_constructed()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, this.Sys.Scheduler as TestScheduler);

            //Assert
            clock.Now.ShouldBe(InitialTime);
        }

        [Fact]
        public void sets_initial_speed_to_1()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime, this.Sys.Scheduler as TestScheduler);

            //Assert
            clock.Speed.ShouldBe(1);
        }
    }
}