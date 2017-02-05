using OfficeHVAC.Models.Devices;
using Shouldly;
using Xunit;

namespace OfficeHVAC.Models.Tests
{
    public class ModeEffectivePowerCalculation
    {
        [Theory]
        [InlineData(0, 0.5)]
        [InlineData(0.5, 0)]
        public void returns_zero_if_consumption_or_efficiency_is_equal_to_0(double powerConsumption, double powerEfficency)
        {
            //Arrange
            var mode = new TemperatureMode()
            {
                PowerConsumption = powerConsumption,
                PowerEfficiency = powerEfficency
            };

            //Act
            var effectivePower = mode.CalculateEffectivePower(100);

            //Assert
            effectivePower.ShouldBe(0);
        }

        [Fact]
        public void should_be_less_than_max_if_consumption_and_efficiency_are_less_than_1()
        {
            //Arrange
            var Pmax = 100;
            var mode = new TemperatureMode()
            {
                PowerConsumption = 0.5,
                PowerEfficiency = 0.5
            };

            //Act
            var effectivePower = mode.CalculateEffectivePower(Pmax);

            //Assert
            effectivePower.ShouldBeLessThan(Pmax);
        }

        [Fact]
        public void should_be_above_max_if_consumption_and_efficiency_is_greater_than_1()
        {
            //Arrange
            var Pmax = 100;
            var mode = new TemperatureMode()
            {
                PowerConsumption = 1.5,
                PowerEfficiency = 1.5
            };

            //Act
            var effectivePower = mode.CalculateEffectivePower(Pmax);

            //Assert
            effectivePower.ShouldBeGreaterThan(Pmax);
        }

        [Fact]
        public void should_be_equal_to_max_if_consumption_and_efficiency_are_equal_to_1()
        {
            //Arrange
            var Pmax = 100;
            var mode = new TemperatureMode()
            {
                PowerConsumption = 1,
                PowerEfficiency = 1
            };

            //Act
            var effectivePower = mode.CalculateEffectivePower(Pmax);

            //Assert
            effectivePower.ShouldBe(Pmax);
        }
    }
}
