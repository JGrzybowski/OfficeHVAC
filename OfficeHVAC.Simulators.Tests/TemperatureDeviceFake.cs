using OfficeHVAC.Models.Devices;
using System.Collections.Generic;

namespace OfficeHVAC.Simulators.Tests
{
    public class TemperatureDeviceFake : ITemperatureDevice
    {
        public bool IsTurnedOn => HeatingParameter != 0.0f;
        
        public int MaxPower { get; set; }

        public double PowerConsumption { get; set; }

        public float HeatingParameter { get; set; }

        public void TurnOff() => HeatingParameter = 0;

        public IReadOnlyCollection<string> Modes { get; set; }
        public IReadOnlyDictionary<string, double> EstimatedPowerConsumption { get; set; }
        public double DesiredTemperature { get; set; }
    }
}
