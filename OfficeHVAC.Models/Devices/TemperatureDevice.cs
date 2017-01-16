using System;
using System.Collections.Generic;

namespace OfficeHVAC.Models.Devices
{
    public class TemperatureDevice : ITemperatureDevice
    {
        public bool IsTurnedOn => HeatingParameter != 0;

        public int MaxPower { get; set; }
        public float HeatingParameter { get; set; }

        public double PowerConsumption => MaxPower * Math.Abs(HeatingParameter);

        public void TurnOff() => HeatingParameter = 0;

        public IReadOnlyCollection<string> Modes { get; private set; } = new List<string>();
        public IReadOnlyDictionary<string, double> EstimatedPowerConsumption { get; private set; } = new Dictionary<string, double>();
        public double DesiredTemperature { get; set; }
    }
}
