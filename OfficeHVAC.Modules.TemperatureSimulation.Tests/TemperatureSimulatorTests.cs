using NodaTime;
using NSubstitute;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests
{
    public class TemperatureSimulatorTests
    {
        private static readonly double StartingTemperature = 36.6f;
        private static readonly Instant StartDateTime = new Instant();
        private static readonly Duration FiveMinutes = Duration.FromMinutes(5);
        private static readonly Instant FiveMinutesLater = StartDateTime + FiveMinutes;
       
        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public void stores_initial_temperature(float desiredTemperatureDelta)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var temperatureModelFake = Substitute.For<ITemperatureModel>();
            var simulator = new TemperatureSimulator(timeSourceFake, StartingTemperature, temperatureModelFake)
            {
                Devices = new List<ITemperatureDevice>
                {
                    new TemperatureDeviceFake()
                    {
                        EffectivePower = 40,
                        DesiredTemperature = StartingTemperature + desiredTemperatureDelta
                    }
                }
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
        public void uses_model_to_calculate_change(int desiredTemperatureDelta)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var temperatureModelFake = Substitute.For<ITemperatureModel>();
            var simulator = new TemperatureSimulator(timeSourceFake, StartingTemperature,temperatureModelFake)
            {
                Devices = new List<TemperatureDeviceFake>
                {
                    new TemperatureDeviceFake()
                    {
                        EffectivePower = 1500,
                        DesiredTemperature = StartingTemperature + desiredTemperatureDelta
                    }
                }
            };
            timeSourceFake.Now = FiveMinutesLater;
            
            //Act
            var temperatureAfter = simulator.Temperature;

            //Assert
            temperatureModelFake.Received().CalculateChange(StartingTemperature, simulator.Devices, FiveMinutes);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void should_turn_off_all_devices_when_temperature_is_set(float desiredTemperatureDelta)
        {
            //Arrange
            var timeSourceFake = new TimeSourceFake(StartDateTime);
            var temperatureModelFake = Substitute.For<ITemperatureModel>();
            var simulator = new TemperatureSimulator(timeSourceFake, StartingTemperature, temperatureModelFake)
            {
                Devices = new List<ITemperatureDevice>
                {
                    new TemperatureDeviceFake()
                    {
                        EffectivePower = 1500,
                        DesiredTemperature = StartingTemperature + desiredTemperatureDelta
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
