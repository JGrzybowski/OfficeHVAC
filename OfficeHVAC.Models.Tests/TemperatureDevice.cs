using OfficeHVAC.Models.Devices;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Models.Tests
{
    public class TemperatureDevice
    {
        [Fact]
        public void is_created_turned_off()
        {
            //Arrange
            ITemperatureDevice device = new Devices.TemperatureDevice();

            //Assert
            device.IsTurnedOn.ShouldBe(false);
        }

        [Fact]
        public void power_consumption_is_zero_when_device_is_not_working()
        {
            //Arrange
            ITemperatureDevice device = new Devices.TemperatureDevice();
            device.HeatingParameter = 1;

            //Act
            device.TurnOff();

            //Assert
            device.PowerConsumption.ShouldBe(0);
        }

        [Fact]
        public void temperature_change_is_zero_when_turned_off()
        {
            //Arrange
            ITemperatureDevice device = new Devices.TemperatureDevice();
            device.HeatingParameter = 1;

            //Act
            device.TurnOff();

            //Assert
            device.HeatingParameter.ShouldBe(0);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(+1)]
        public void power_consumption_is_max_when_using_device(float temperatureChange)
        {
            //Arrange
            ITemperatureDevice device = new Devices.TemperatureDevice();

            //Act
            device.HeatingParameter = temperatureChange;

            //Assert
            device.PowerConsumption.ShouldBe(device.MaxPower);
        }
    }
}
