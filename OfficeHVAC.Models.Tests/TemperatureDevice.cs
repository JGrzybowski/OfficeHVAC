using NSubstitute;
using OfficeHVAC.Models.Devices;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace OfficeHVAC.Models.Tests
{
    public class TemperatureDevice
    {
        public ITemperatureMode OffMode = new TemperatureMode() {Name = "Off", PowerEfficiency = 0, PowerConsumption = 0, TemperatureRange = new Range<double>(0,10)};
        public ITemperatureMode WorkingMode = new TemperatureMode() {Name = nameof(WorkingMode), PowerEfficiency = 0.5, PowerConsumption = 0.6, TemperatureRange = new Range<double>(0,10)};
        
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
                Modes = new List<ITemperatureMode> { OffMode, WorkingMode },
                MaxPower = 1000
            };

            //Act
            device.SetActiveModeByName = nameof(WorkingMode);

            //Assert
            device.SetActiveModeByName.ShouldBe(nameof(WorkingMode));
            device.PowerConsumption.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void setting_ActiveModeName_throws_ArgumentOutOfRangeException_when_mode_is_not_on_the_list()
        {
            //Arrange
            var device = new Devices.TemperatureDevice()
            {
                Modes = new List<ITemperatureMode> { OffMode, WorkingMode },
                MaxPower = 1000
            };

            //Act & Assert
            var ex = Should.Throw<ArgumentOutOfRangeException>(() => device.SetActiveModeByName = "Invalid Mode Name");
            ex.ParamName.ShouldBe("value");
            ex.ActualValue.ShouldBe("Invalid Mode Name");
        }

        [Fact]
        public void EffectivePower_is_calculated_using_active_modes_EffectivePower()
        {
            //Arrange
            ITemperatureMode workingModeFake = Substitute.For<ITemperatureMode>();
            workingModeFake.Name.Returns(nameof(workingModeFake));
            workingModeFake.CalculateEffectivePower(1000).Returns(750);

            var device = new Devices.TemperatureDevice
            {
                Modes = new List<ITemperatureMode> {OffMode, workingModeFake},
                MaxPower = 1000,
                SetActiveModeByName = nameof(workingModeFake)
            };

            //Act
            var Peff = device.EffectivePower;

            //Assert
            Peff.ShouldBe(750);
            workingModeFake.Received(1).CalculateEffectivePower(1000);
        }

        [Fact]
        public void ModeNames_lists_all_modes_names()
        {
            //Arrange
            var device = new Devices.TemperatureDevice()
            {
                Modes = new List<ITemperatureMode> { OffMode, WorkingMode },
                MaxPower = 1000
            };
            
            //Act
            var names = device.ModesNames;

            //Assert
            names.Count.ShouldBe(2);
            names.ShouldContain(OffMode.Name);
            names.ShouldContain(WorkingMode.Name);
        }
    }
}
