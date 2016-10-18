using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;

namespace OfficeHVAC.Simulators
{
    public class TemperatureSimulator : ITemperatureSimulator
    {
        private float lastTemperature;
        private readonly DateTime lastTime;

        private const int WattsToChangeOneDegreeInOneHour = 20;

        private float CalculateChange()
        {
            var hoursSinceLastUpdate = (this.TimeSource.Time - this.lastTime).TotalHours;
            return (float)(this.Devices
                            .Sum(device => device.MaxPower * device.HeatingParameter)
                            / WattsToChangeOneDegreeInOneHour
                            * hoursSinceLastUpdate);
        }

        public TemperatureSimulator(ITimeSource timeSource, float initialTemperature)
        {
            this.TimeSource = timeSource;
            this.lastTime = timeSource.Time;
            this.lastTemperature = initialTemperature;
        }

        public IEnumerable<ITemperatureDevice> Devices { get; set; } = new List<ITemperatureDevice>();
        public ITimeSource TimeSource { get; set; }
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
