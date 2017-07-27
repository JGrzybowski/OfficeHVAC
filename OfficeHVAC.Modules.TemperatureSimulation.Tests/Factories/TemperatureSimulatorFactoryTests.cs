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
            var simulatorFactory = new TemperatureSimulatorFactory(Substitute.For<ITimeSource>(), temperatureModelFake);
            simulatorFactory.InitialTemperature = 45;
            var status = new RoomStatus() { };


            //Act
            var temperatureSimulator = simulatorFactory.TemperatureSimulator();

            //Assert
            temperatureSimulator.Temperature.ShouldBe(45);
        }
    }
}
