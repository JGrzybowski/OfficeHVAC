using NodaTime;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests
{
    public class TemperatureDeviceFake : ITemperatureDevice
    {
        public string Id { get; set; }

        public bool IsTurnedOn => PowerConsumption != 0.0;

        public int MaxPower { get; set; }

        public double PowerConsumption { get; set; }

        public double EffectivePower { get; set; }

        public void TurnOff() => PowerConsumption = 0;

        private string setActiveModeByName;

        public string GetActiveModeByName()
        {
            return setActiveModeByName;
        }

        public void SetActiveModeByName(string value)
        {
            setActiveModeByName = value;
        }

        public IReadOnlyCollection<string> ModesNames { get; set; }

        public ICollection<ITemperatureMode> Modes { get; set; }

        public double DesiredTemperature { get; set; }

        public Func<string, Duration, double> CalculatePowerConsumptionFunction { get; set; }

        public double CalculatePowerConsumption(string name, Duration time) => CalculatePowerConsumptionFunction(name, time);

        IEnumerable<ITemperatureMode> ITemperatureDeviceDefinition.Modes => this.Modes;
        public IDevice Clone() => new TemperatureDeviceFake()
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
