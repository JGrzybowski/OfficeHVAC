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

        private float CalculateChange()
        {
            throw new NotImplementedException();
        }

        public TemperatureSimulator(
            ITimeSource timeSource,
            float initialTemperature = 25
            )
        {
            throw new NotImplementedException();

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
                throw new NotImplementedException();
                return CalculateChange();
            }

            set
            {
                throw new NotImplementedException();

                foreach (var device in Devices)
                {
                    device.TurnOff();
                }
                lastTemperature = value;
            }
        }

    }
}
