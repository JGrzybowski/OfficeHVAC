using NSubstitute;
using OfficeHVAC.Models;
using OfficeHVAC.Modules.TemperatureSimulation.Factories;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests.Factories
{
    public class TemperatureSimulatorFactoryTests
    {
        [Fact]
        public void should_create_simulator_with_initial_temperature()
        {
            //Arrange
            var temperatureModelFake = Substitute.For<ITemperatureModel>();
            var simulatorFactory = new TemperatureSimulatorFactory(Substitute.For<ITimeSource>(),temperatureModelFake);
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
            var simulatorFactory = new TemperatureSimulatorFactory(timeSourceFake, temperatureModelFake);
            
            //Act
            var temperatureSimulator = simulatorFactory.TemperatureSimulator();

            //Assert
            temperatureSimulator.TimeSource.ShouldBe(timeSourceFake);
        }
    }
}
