using NodaTime;
using OfficeHVAC.Models;
using OfficeHVAC.Models.Devices;
using System.Collections.Generic;

namespace OfficeHVAC.Modules.TemperatureSimulation
{
    public class TemperatureSimulator : ITemperatureSimulator
    {
        public ITemperatureModel Model { get; }
        protected double LastTemperature { get; set; }
        protected Instant LastTime { get; set; }

        public TemperatureSimulator(ITimeSource timeSource, double initialTemperature, ITemperatureModel model)
        {
            this.Model = model;
            this.TimeSource = timeSource;
            this.LastTime = timeSource.Now;
            this.LastTemperature = initialTemperature;
        }

        public IEnumerable<ITemperatureDevice> Devices { get; set; } = new List<ITemperatureDevice>();
        public ITimeSource TimeSource { get; }
        public double Temperature 
        {
            get
            {
                var now = this.TimeSource.Now;
                var timeDelta = (now - LastTime);
                LastTime = now;

                LastTemperature += Model.CalculateChange(LastTemperature, Devices, timeDelta);
                return LastTemperature;
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
