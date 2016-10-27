using System;
using System.Collections.Generic;
using OfficeHVAC.Models.Devices;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Simulators.Tests
{
    public class TemperatureSimulator
    {
        private static readonly float startingTemperature = 36.6f;
        private static DateTime startDateTime = new DateTime(2016, 10, 16, 16, 45, 00);
        private static readonly DateTime fiveMinutesLater = startDateTime.AddMinutes(5);

        [Fact]
        public void does_not_change_temperature_value_when_devices_are_turned_off()
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(startDateTime);
            var simulator = new Simulators.TemperatureSimulator(timeSourceFake, startingTemperature)
            {
                Devices = new List<ITemperatureDevice> { new TemperatureDeviceFake() { MaxPower = 40 } }
            };

            //Act
            timeSourceFake.Now = fiveMinutesLater;

            //Assert
            var temperatureAfter = simulator.Temperature;
            temperatureAfter.ShouldBe(startingTemperature);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public void change_temperature_value_when_devices_are_turned_on(float temperatureChange)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(startDateTime);
            var simulator = new Simulators.TemperatureSimulator(timeSourceFake, startingTemperature)
            {
                Devices = new List<ITemperatureDevice>
                {
                    new TemperatureDeviceFake()
                    {
                        MaxPower = 40,
                        HeatingParameter = temperatureChange
                    }
                }
            };

            //Act
            timeSourceFake.Now = fiveMinutesLater;

            //Assert
            var temperatureAfter = simulator.Temperature;
            temperatureAfter.ShouldNotBe(startingTemperature);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void does_not_change_temperature_value_when_time_does_not_change(float temperatureChange)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(startDateTime);
            var simulator = new Simulators.TemperatureSimulator(timeSourceFake, startingTemperature)
            {
                Devices = new List<ITemperatureDevice>
                {
                    new TemperatureDeviceFake()
                    {
                        MaxPower = 40,
                        HeatingParameter = temperatureChange
                    }
                }
            };

            //Act
            var temperatureAfter = simulator.Temperature;

            //Assert
            temperatureAfter.ShouldBe(startingTemperature);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void should_turn_off_all_devices_when_temperature_is_set(float temperatureChange)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(startDateTime);
            var simulator = new Simulators.TemperatureSimulator(timeSourceFake, startingTemperature)
            {
                Devices = new List<ITemperatureDevice>
                {
                    new TemperatureDeviceFake()
                    {
                        MaxPower = 40,
                        HeatingParameter = temperatureChange
                    }
                }
            };
            
            //Act
            simulator.Temperature = 35;
            
            //Assert
            simulator.Temperature.ShouldBe(35);
            simulator.Devices.ShouldAllBe(device => !device.IsTurnedOn);
        }

    }
}
