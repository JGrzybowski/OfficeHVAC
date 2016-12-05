using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHVAC.Simulators
{
    public class RandomTemperatureSimulator : ITemperatureSimulator
    {
        private Random random = new Random();
        private float lastTemperature { get; set; }
        private DateTime lastTime { get; set; }

        private const int WattsToChangeOneDegreeInOneHour = 20;

        private float CalculateChange()
        {
            var now = this.TimeSource.Now;
            var hoursSinceLastUpdate = (now - this.lastTime).TotalHours;
            this.lastTime = now;
            var variance = random.Next(-15, 15)/1000f;
            return (float)(variance);

        }

        public RandomTemperatureSimulator(ITimeSource timeSource, float initialTemperature)
        {
            this.TimeSource = timeSource;
            this.lastTime = timeSource.Now;
            this.lastTemperature = initialTemperature;
        }

        public IEnumerable<ITemperatureDevice> Devices { get; set; } = new List<ITemperatureDevice>();
        public ITimeSource TimeSource { get; }
        public float Temperature 
        {
            get
            {
                this.lastTemperature += CalculateChange();
                return this.lastTemperature;
            }

            set
            {
                foreach (var device in this.Devices)
                    device.TurnOff();
                this.lastTemperature = value;
            }
        }

    }
}
