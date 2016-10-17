using System;

namespace OfficeHVAC.Models.Devices
{
    public class TemperatureDevice : ITemperatureDevice
    {
        public bool IsTurnedOn => TemperatureChange != 0;

        public int MaxPower { get; set; }

        public double PowerConsumption
        {
            get { return MaxPower*Math.Abs(TemperatureChange); }

            set
            {
                throw new NotImplementedException();
            }
        }

        public float TemperatureChange { get; set; }

        public void TurnOff()
        {
            TemperatureChange = 0;
        }
    }
}
