using NSubstitute;
using OfficeHVAC.Models;
using OfficeHVAC.Simulators;
using Shouldly;
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
            var simulatorFactory = new Factories.Simulators.Temperature.TemperatureSimulatorFactory(timeSourceFake, temperatureModelFake);
            
            //Act
            var temperatureSimulator = simulatorFactory.TemperatureSimulator();

            //Assert
            temperatureSimulator.TimeSource.ShouldBe(timeSourceFake);
        }
    }
}
