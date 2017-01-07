using NodaTime;
using OfficeHVAC.Models.Devices;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OfficeHVAC.Simulators.Tests
{
    public class TemperatureSimulatorTests
    {
        private static readonly float StartingTemperature = 36.6f;
        private static readonly Instant StartDateTime = new Instant();

        private static readonly Instant FiveMinutesLater = StartDateTime + Duration.FromMinutes(5);
        private static readonly Instant TenMinutesLater = StartDateTime + Duration.FromMinutes(10);
        private static readonly Instant FifteenMinutesLater = StartDateTime + Duration.FromMinutes(15);

        [Fact]
        public void does_not_change_temperature_value_when_devices_are_turned_off()
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var simulator = new TemperatureSimulator(timeSourceFake, StartingTemperature)
            {
                Devices = new List<ITemperatureDevice> { new TemperatureDeviceFake() { MaxPower = 40 } }
            };

            //Act
            timeSourceFake.Now = FiveMinutesLater;

            //Assert
            var temperatureAfter = simulator.Temperature;
            temperatureAfter.ShouldBe(StartingTemperature);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public void change_temperature_value_when_devices_are_turned_on(float temperatureChange)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var simulator = new TemperatureSimulator(timeSourceFake, StartingTemperature)
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
            timeSourceFake.Now = FiveMinutesLater;

            //Assert
            var temperatureAfter = simulator.Temperature;
            temperatureAfter.ShouldNotBe(StartingTemperature);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void does_not_change_temperature_value_when_time_does_not_change(float temperatureChange)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var simulator = new Simulators.TemperatureSimulator(timeSourceFake, StartingTemperature)
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
            temperatureAfter.ShouldBe(StartingTemperature);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void should_turn_off_all_devices_when_temperature_is_set(float temperatureChange)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var simulator = new TemperatureSimulator(timeSourceFake, StartingTemperature)
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

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public void takes_time_into_consideration(int heatingParameter)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var simulator = new TemperatureSimulator(timeSourceFake, StartingTemperature)
            {
                Devices = new List<ITemperatureDevice>
                {
                    new TemperatureDeviceFake()
                    {
                        MaxPower = 40,
                        HeatingParameter = heatingParameter
                    }
                }
            };
            timeSourceFake.Now = FiveMinutesLater;
            var temperatureAfterFiveMinutes = simulator.Temperature;

            simulator.Devices.First().HeatingParameter = 0;
            timeSourceFake.Now = TenMinutesLater;
            var temperatureAfterTenMinutes = simulator.Temperature;

            simulator.Devices.First().HeatingParameter = -heatingParameter;
            timeSourceFake.Now = FifteenMinutesLater;

            //Act
            var temperatureAfterFifteenMinutes = simulator.Temperature;

            //Assert
            temperatureAfterFifteenMinutes.ShouldBe(StartingTemperature);
            temperatureAfterFifteenMinutes.ShouldNotBe(temperatureAfterFiveMinutes);
            temperatureAfterFifteenMinutes.ShouldNotBe(temperatureAfterTenMinutes);
        }
    }
}
