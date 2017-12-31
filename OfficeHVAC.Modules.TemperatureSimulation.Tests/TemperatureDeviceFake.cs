using NodaTime;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests
{
    public class TemperatureDeviceFake : ITemperatureDeviceStatus
    {
        public string Id { get; set; }

        public bool IsTurnedOn => PowerConsumption != 0.0;

        public int MaxPower { get; set; }
        public TemperatureModeType ActiveModeType { get; }

        public double PowerConsumption { get; set; }

        public double EffectivePower { get; set; }

        public void TurnOff() => PowerConsumption = 0;

        public ITemperatureMode ActiveMode { get; set; }

        public void SetActiveMode(TemperatureModeType modeType)
        {
            ActiveMode = Modes.Single(m => m.Type == modeType);
        }

        public IEnumerable<ITemperatureMode> Modes { get; set; }

        public double DesiredTemperature { get; set; }

        public Func<TemperatureModeType, Duration, double> CalculatePowerConsumptionFunction { get; set; }

        public double CalculatePowerConsumption(TemperatureModeType modeType, Duration time) => CalculatePowerConsumptionFunction(modeType, time);

        public ITemperatureDeviceStatus Clone() => new TemperatureDeviceFake()
        {
            Id = Id,
            DesiredTemperature = DesiredTemperature,
            EffectivePower = EffectivePower,
            MaxPower = MaxPower,
            Modes = Modes.Select(m => m.Clone()).ToList(),
            PowerConsumption = PowerConsumption,
            CalculatePowerConsumptionFunction = CalculatePowerConsumptionFunction
        };
    }
}
