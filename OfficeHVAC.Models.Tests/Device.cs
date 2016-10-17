using System;
using Shouldly;
using Xunit;
namespace OfficeHVAC.Models.Tests
{
    public class Device
    {
        [Fact]
        public void is_created_turned_off()
        {
            //Arrange
            IDevice device = new Models.Device();

            //Assert
            device.IsTurnedOn.ShouldBe(false);
        }

        [Fact]
        public void throws_ArgumentOutOfRangeException_when_power_set_above_100_percent()
        {
            //Arrange
            IDevice device = new Models.Device();
            
            //Act & Assert
            Should.Throw<ArgumentOutOfRangeException>(() => device.UsedPower = 150);
        }
    }
}
