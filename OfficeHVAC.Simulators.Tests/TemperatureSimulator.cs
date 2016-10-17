using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeHVAC.Models;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Simulators.Tests
{
    public class TemperatureSimulator
    {
        [Fact]
        public void does_not_change_with_time_value_when_devices_are_not_turned_on()
        {
            //Arrange
            var devicesList = new List<IDevice>() { new Device() };
            var temperatureBefore = 36.6f;
            var timeSourceFake = new TimeSourceFake() {Time = new DateTime(19,10,2016,16,45,00)};
            var simulator = new Simulators.TemperatureSimulator(
                (f, devices, timeSpan) => f + 5,
                timeSourceFake,
                temperatureBefore
            );
            timeSourceFake.Time = timeSourceFake.Time.AddHours(4);
            
            //Act
            var temperatureAfter = simulator.Temperature;

            //Assert
            temperatureAfter.ShouldBe(temperatureBefore);
        }

        [Fact]
        public void should_change_accoding_to_model_when_devices_are_turned_on()
        {
            //Arrange
            var devicesList = new List<IDevice>() { new Device() };
            var temperatureBefore = 36.6f;
            var timeSourceFake = new TimeSourceFake() { Time = new DateTime(19, 10, 2016, 16, 45, 00) };
            var simulator = new Simulators.TemperatureSimulator(
                (f, devices, timeSpan) => f + 5,
                timeSourceFake,
                temperatureBefore
            );
            timeSourceFake.Time = timeSourceFake.Time.AddHours(4);

            //Act
            var temperatureAfter = simulator.Temperature;

            //Assert
            temperatureAfter.ShouldBe(temperatureBefore);
        }

        [Fact]
        public void should_turn_off_all_devices_when_temperature_is_set()
        {
            //Arrange
            var devicesList = new List<IDevice>()
            {
                new Device() {UsedPower = 40},
                new Device() {UsedPower = 75}
            };
            var timeSourceFake = new TimeSourceFake() { Time = new DateTime(19, 10, 2016, 16, 45, 00) };
            var simulator = new Simulators.TemperatureSimulator(
                (f, devices, timeSpan) => f + 5,
                timeSourceFake
            );
            timeSourceFake.Time = timeSourceFake.Time.AddHours(4);

            //Act
            simulator.Temperature = 35;

            //Assert
            simulator.Temperature = 35;
            simulator.Devices.ShouldAllBe(device => !device.IsTurnedOn);
        }

    }
}
