using OfficeHVAC.Models.Devices;
using System.Collections.Generic;

namespace OfficeHVAC.Simulators.Tests
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
        public ICollection<ITemperatureMode> Modes{ get; set; }
        public double DesiredTemperature { get; set; }
    }
}
