using System;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Simulators.Tests
{
    public class TemperatureDeviceFake : ITemperatureDevice
    {
        public bool IsTurnedOn => TemperatureChange != 0.0f;
        
        public int MaxPower { get; set; }

        public double PowerConsumption { get; set; }

        public float TemperatureChange { get; set; }

        public void TurnOff()
        {
            TemperatureChange = 0;
        }
    }
}
