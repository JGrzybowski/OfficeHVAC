using System;

namespace OfficeHVAC.Models.Devices
{
    public class TemperatureDevice : ITemperatureDevice
    {
        public bool IsTurnedOn => this.HeatingParameter != 0;

        public int MaxPower { get; set; }
        public float HeatingParameter { get; set; }

        public double PowerConsumption
        {
            get { return this.MaxPower * Math.Abs(this.HeatingParameter); }

            set
            {
                throw new NotImplementedException();
            }
        }
        
        public void TurnOff()
        {
            this.HeatingParameter = 0;
        }
    }
}
