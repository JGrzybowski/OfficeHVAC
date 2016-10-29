using System;

namespace OfficeHVAC.Models.Devices
{
    public class TemperatureDevice : ITemperatureDevice
    {
        public bool IsTurnedOn => HeatingParameter != 0;

        public int MaxPower { get; set; }
        public float HeatingParameter { get; set; }

        public double PowerConsumption => MaxPower * Math.Abs(HeatingParameter);

        public void TurnOff()
        {
            HeatingParameter = 0;
        }
    }
}
