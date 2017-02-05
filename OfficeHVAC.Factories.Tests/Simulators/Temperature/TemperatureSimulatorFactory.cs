using NodaTime;
using NSubstitute;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using OfficeHVAC.Simulators;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace OfficeHVAC.Factories.Tests.Simulators.Temperature
{
    public class TemperatureSimulatorFactory
    {
        [Fact]
        public void should_create_simulator_with_initial_temperature()
        {
            //Arrange
            var temperatureModelFake = Substitute.For<ITemperatureModel>();
            var simulatorFactory = new Factories.Simulators.Temperature.TemperatureSimulatorFactory(Substitute.For<ITimeSource>(),temperatureModelFake);
            simulatorFactory.InitialTemperature = 45;

            //Act
            var temperatureSimulator = simulatorFactory.TemperatureSimulator();

            //Assert
            temperatureSimulator.Temperature.ShouldBe(45);
        }

        [Fact]
        public void should_create_simulator_with_timeSource()
        {
            //Arrange
            var timeSourceFake = Substitute.For<ITimeSource>();
            var temperatureModelFake = Substitute.For<ITemperatureModel>();
            temperatureModelFake
                .CalculateChange(Arg.Any<double>(), Arg.Any<IEnumerable<ITemperatureDevice>>(), Arg.Any<Duration>())
                .ReturnsForAnyArgs(25.0);
            var simulatorFactory = new Factories.Simulators.Temperature.TemperatureSimulatorFactory(timeSourceFake, temperatureModelFake);
            
            //Act
            var temperatureSimulator = simulatorFactory.TemperatureSimulator();

            //Assert
            temperatureSimulator.TimeSource.ShouldBe(timeSourceFake);
        }
    }
}
