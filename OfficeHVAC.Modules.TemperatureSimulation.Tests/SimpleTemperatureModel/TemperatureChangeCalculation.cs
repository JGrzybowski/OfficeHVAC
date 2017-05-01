using System.Collections.Generic;
using NodaTime;
using OfficeHVAC.Models.Devices;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.SimpleTemperatureModel
{
    public class TemperatureChangeCalculation
    {
        private static readonly double StartingTemperature = 36.6f;
        private static readonly Duration FiveMinutes = Duration.FromMinutes(5);

        [Fact]
        public void returns_0_when_devices_are_turned_off()
        {
            //Arrange
            var temperatureModel = new TemperatureSimulation.SimpleTemperatureModel();
            var devices = new List<ITemperatureDevice> { new TemperatureDeviceFake() { MaxPower = 40 } };

            //Act
            var temperatureDelta = temperatureModel.CalculateChange(StartingTemperature, devices, FiveMinutes, 250);

            //Assert
            temperatureDelta.ShouldBe(0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public void returns_non_zero_value_when_devices_are_turned_on(int desiredTemperatureDelta)
        {
            //Arrange
            var temperatureModel = new TemperatureSimulation.SimpleTemperatureModel();
            var devices = new List<ITemperatureDevice>
            {
                new TemperatureDeviceFake()
                {
                    EffectivePower = 400,
                    DesiredTemperature = StartingTemperature + desiredTemperatureDelta
                }
            };

            //Act
            var temperatureDelta = temperatureModel.CalculateChange(StartingTemperature, devices, FiveMinutes, 250);

            //Assert
            (temperatureDelta * desiredTemperatureDelta).ShouldBeGreaterThan(0);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void returns_zero_when_time_does_not_change(float desiredTemperatureDelta)
        {
            //Arrange
            var temperatureModel = new TemperatureSimulation.SimpleTemperatureModel();
            var devices = new List<ITemperatureDevice>
            {
                new TemperatureDeviceFake()
                {
                    EffectivePower = 400,
                    DesiredTemperature = StartingTemperature + desiredTemperatureDelta
                }
            };

            //Act
            var temperatureDelta = temperatureModel.CalculateChange(StartingTemperature, devices, Duration.Zero, 250);

            //Assert
            (temperatureDelta * desiredTemperatureDelta).ShouldBe(0);
        }
    }
}
