using NSubstitute;
using OfficeHVAC.Models.Devices;
using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Models.Tests
{
    public class TemperatureDevice
    {
        public ITemperatureMode OffMode = new TemperatureMode() { Name = "Off", Type = TemperatureModeType.Off, PowerEfficiency = 0, PowerConsumption = 0, TemperatureRange = new Range<double>(0, 10) };
        public ITemperatureMode WorkingMode = new TemperatureMode() { Name = "WorkingMode", Type = TemperatureModeType.Normal, PowerEfficiency = 0.5, PowerConsumption = 0.6, TemperatureRange = new Range<double>(0, 10) };

        [Fact]
        public void is_created_turned_off()
        {
            //Arrange
            var device = new Devices.TemperatureDevice();

            //Assert
            device.IsTurnedOn.ShouldBe(false);
        }

        [Fact]
        public void power_consumption_is_zero_when_device_is_not_working()
        {
            //Arrange
            var device = new Devices.TemperatureDevice();

            //Act
            device.TurnOff();

            //Assert
            device.PowerConsumption.ShouldBe(0);
        }

        [Fact]
        public void setting_ActiveModeName_changes_PowerConsumption()
        {
            //Arrange
            var device = new Devices.TemperatureDevice()
            {
                Modes = new ModesCollection { OffMode, WorkingMode },
                MaxPower = 1000
            };

            //Act
            device.SetActiveMode(WorkingMode.Type);

            //Assert
            device.ActiveMode.ShouldBe(WorkingMode);
            device.PowerConsumption.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void setting_ActiveModeName_throws_ArgumentOutOfRangeException_when_mode_is_not_on_the_list()
        {
            //Arrange
            var device = new Devices.TemperatureDevice()
            {
                Modes = new ModesCollection { OffMode, WorkingMode },
                MaxPower = 1000
            };
            var invalidModeType = TemperatureModeType.Turbo;
            device.Modes.ShouldNotContain(m => m.Type == invalidModeType);

            //Act & Assert
            var ex = Should.Throw<ArgumentOutOfRangeException>(() => device.SetActiveMode(invalidModeType));
            ex.ParamName.ShouldBe("value");
            ex.ActualValue.ShouldBe(invalidModeType);
        }

        [Fact]
        public void EffectivePower_is_calculated_using_active_modes_EffectivePower()
        {
            //Arrange
            ITemperatureMode workingModeFake = Substitute.For<ITemperatureMode>();
            workingModeFake.Type.Returns(TemperatureModeType.Normal);
            workingModeFake.CalculateEffectivePower(1000).Returns(750);

            var device = new Devices.TemperatureDevice
            {
                Modes = new ModesCollection { OffMode, workingModeFake },
                MaxPower = 1000
            };
            device.SetActiveMode(workingModeFake.Type);

            //Act
            var p_eff = device.EffectivePower;

            //Assert
            p_eff.ShouldBe(750);
            workingModeFake.Received(1).CalculateEffectivePower(1000);
        }
    }
}
