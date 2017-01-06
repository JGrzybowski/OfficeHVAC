using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Simulators
{
    public class TemperatureSimulator : ITemperatureSimulator
    {
        protected float LastTemperature { get; set; }
        protected DateTime LastTime { get; set; }

        protected const int WattsToChangeOneDegreeInOneHour = 20;

        protected virtual float CalculateChange()
        {
            var now = this.TimeSource.Now;
            var hoursSinceLastUpdate = (now - this.LastTime).TotalHours;
            this.LastTime = now;
            return (float)(this.Devices
                            .Sum(device => device.MaxPower * device.HeatingParameter)
                            / WattsToChangeOneDegreeInOneHour
                            * hoursSinceLastUpdate);

        }

        public TemperatureSimulator(ITimeSource timeSource, float initialTemperature)
        {
            this.TimeSource = timeSource;
            this.LastTime = timeSource.Now;
            this.LastTemperature = initialTemperature;
        }

        public IEnumerable<ITemperatureDevice> Devices { get; set; } = new List<ITemperatureDevice>();
        public ITimeSource TimeSource { get; }
        public float Temperature 
        {
            get
            {
                this.LastTemperature += CalculateChange();
                return this.LastTemperature;
            }

            set
            {
                foreach (var device in this.Devices)
                    device.TurnOff();
                this.LastTemperature = value;
            }
        }

    }
}
