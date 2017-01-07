using NodaTime;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Models.Tests.ControlledTimeSource
{
    public class Reset
    {
        private static readonly Instant InitialTime = Instant.FromUtc(2000, 12, 01, 12, 00, 00);

        [Fact]
        public void sets_new_value_of_Now()
        {
            //Arrange
            var clock = new TimeSources.ControlledTimeSource(InitialTime);
            var newInstant = InitialTime - Duration.FromHours(-15);

            //Act
            clock.Reset(newInstant);

            //Assert
            clock.Now.ShouldBe(newInstant);
        }
    }

}