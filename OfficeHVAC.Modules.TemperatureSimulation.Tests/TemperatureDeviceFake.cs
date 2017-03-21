using NodaTime;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;

namespace OfficeHVAC.Modules.TemperatureSimulation.Tests
{
    public class TemperatureDeviceFake : ITemperatureDevice
    {
        public bool IsTurnedOn => PowerConsumption != 0.0;

        public int MaxPower { get; set; }

        public double PowerConsumption { get; set; }

        public double EffectivePower { get; set; }

        public void TurnOff() => PowerConsumption = 0;

        public string SetActiveModeByName { get; set; }

        public IReadOnlyCollection<string> ModesNames { get; set; }

        public ICollection<ITemperatureMode> Modes { get; set; }

        public double DesiredTemperature { get; set; }

        public Func<string, Duration, double> CalculatePowerConsumptionFunction { get; set; }

        public double CalculatePowerConsumption(string name, Duration time) => CalculatePowerConsumptionFunction(name, time);
    }
}
